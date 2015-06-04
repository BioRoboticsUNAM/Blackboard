using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using Robotics;
using Blk.Api;
using Blk.Engine.Actions;
using Blk.Engine.SharedVariables;
using Robotics.Utilities;

namespace Blk.Engine
{
	/// <summary>
	/// Implements an interface to access a remote module via a TCP/IP client
	/// </summary>
	public class ModuleClientTcp : ModuleClient, IModuleClientTcp
	{
		#region Variables

		#region Message flow vars

		/// <summary>
		/// Stores all data received trough socket
		/// </summary>
		protected ProducerConsumer<TcpPacket> dataReceived;

		#endregion

		#region Socket and connection vars

		/// <summary>
		/// Connection socket to the Application Module
		/// </summary>
		private SocketTcpClient client;
		/// <summary>
		/// IP Address of Application Module's computer
		/// </summary>
		private ServerAddressCollection serverAddresses;

		/// <summary>
		/// Port to connect to the Application ModuleServer
		/// </summary>
		private int port;

		#endregion

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the ModuleClientTcp class
		/// </summary>
		/// <param name="name">Module name</param>
		protected ModuleClientTcp(string name) : base (name)
		{
			this.dataReceived = new ProducerConsumer<TcpPacket>(100);
			this.serverAddresses = new ServerAddressCollection(this);
			this.port = 0;
		}

		/// <summary>
		/// Initializes a new instance of the ModuleClientTcp class 
		/// </summary>
		/// <param name="name">Module name</param>
		/// <param name="ipAddress">IP Address of Application Module's computer</param>
		/// <param name="port">Port of the Application Module</param>
		public ModuleClientTcp(string name, IPAddress ipAddress, int port)
			: this(name, new IPAddress[] { ipAddress }, port)
		{
		}

		/// <summary>
		/// Initializes a new instance of the ModuleClientTcp class 
		/// </summary>
		/// <param name="name">Module name</param>
		/// <param name="ipAddresses">IP Address of Application Module's computer</param>
		/// <param name="port">Port of the Application Module</param>
		public ModuleClientTcp(string name, IEnumerable<IPAddress> ipAddresses, int port)
			: this(name)
		{
			if ((port < 1024) || (port > 65535)) throw new ArgumentException("Port must be between 1024 and 65535", "port");
			if (!Regex.IsMatch(name, @"\w[\w\-_\d]{2,}")) throw new ArgumentException("Invalid module name", "name");
			this.serverAddresses.AddRange(ipAddresses);
			this.port = port;
		}

		/// <summary>
		/// Initializes a new instance of the ModuleClientTcp class 
		/// </summary>
		/// <param name="name">Module name</param>
		/// <param name="ipAddresses">IP Address of Application Module's computer</param>
		/// <param name="port">Port of the Application Module</param>
		/// <param name="prototypes">List of the command prototypes supported by the Application Module</param>
		public ModuleClientTcp(string name, IEnumerable<IPAddress> ipAddresses, int port, Prototype[] prototypes)
			: this(name)
		{
			if ((prototypes == null) || (prototypes.Length == 0)) throw new ArgumentException("The prototypes list cannot be zero-length nor null");
			for (int i = 0; i < prototypes.Length; ++i)
				this.prototypes.Add(prototypes[i]);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Tells if the connection to the remote module application has been stablished
		/// </summary>
		public override bool IsConnected
		{
			get
			{
				if (IsRunning && simOptions.SimulationEnabled) return true;
				if (client != null)
					return client.IsConnected;
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating if the module is running in the same machine as the blackboard
		/// </summary>
		public override bool IsLocal
		{
			get
			{
				List<IPAddress> localAddresses;

				// Find host by name
				IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());

				// Enumerate IP addresses
				localAddresses = new List<IPAddress>(ipHostEntry.AddressList);
				localAddresses.Add(IPAddress.Loopback);
				return localAddresses.Contains(this.ServerAddress);
			}
		}

		/// <summary>
		/// Gets the port where to connect to the Application Module
		/// </summary>
		public int Port
		{
			get { return port; }
			set
			{
				if (IsRunning)
					throw new Exception("Cannot change port while module is running");
				else if ((value < 1024) || (value > 65535))
					throw new ArgumentOutOfRangeException("Invalid Module port (" + this.name + ")");
				else port = value;
			}
		}

		/// <summary>
		/// Gets the IP Address of Application Module's computer
		/// </summary>
		public IPAddress ServerAddress
		{
			get { return ((client != null) && IsConnected) ? client.ServerAddress : serverAddresses[0]; }
		}

		/// <summary>
		/// Gets a collection of IP Addresses of the computers where the Application Module's can be running
		/// </summary>
		public IServerAddressCollection ServerAddresses
		{
			get { return serverAddresses; }
		}

		#endregion

		#region Events
		/*
		/// <summary>
		/// Occurs when the Module connects to a Tcp Server
		/// </summary>
		public event ModuleConnectionEH Connected;

		/// <summary>
		/// Occurs when the Module disconnects from a Tcp Server
		/// </summary>
		public event ModuleConnectionEH Disconnected;
		*/
		#endregion

		#region Methods

		/// <summary>
		/// Performs Restart Tasks
		/// </summary>
		protected override void DoRestartModule()
		{
			restartRequested = false;
			restartTestRequested = false;
			Busy = true;

			Unlock();
			dataReceived.Clear();
			if ((client != null) && client.IsOpen)
			{
				try { client.Disconnect(); }
				catch { }
			}
			ExecuteRestartActions();
			Busy = false;
		}

		/// <summary>
		/// Performs Restart-test Tasks
		/// </summary>
		protected override void DoRestartTest()
		{
			restartTestRequested = false;
			Busy = true;
			
			dataReceived.Clear();
			Unlock();
			ExecuteRestartTestActions();

			Busy = false;
		}

		/// <summary>
		/// Parses all pending TCPPackets received
		/// </summary>
		protected override void ParsePendingData()
		{
			TcpPacket packet;
			string message;
			int i;

			do
			{
				packet = dataReceived.Consume(20);
				if (packet == null)
					return;
				if (!packet.IsAnsi)
					continue;
				for (i = 0; i < packet.DataStrings.Length; ++i)
				{
					message = packet.DataStrings[i].Trim();

					ParseMessage(message);
				}
			} while (!stopMainThread && dataReceived.Count > 0);
		}

		/// <summary>
		/// Sends the provided text string through socket client
		/// </summary>
		/// <param name="stringToSend">string to send through socket</param>
		protected override bool Send(string stringToSend)
		{
			if (sendDelay >= 0)
				Thread.Sleep(sendDelay);

			lock (client)
			{
				// Check is connection has been stablished.
				if (!client.IsOpen)
					return false;
				try { client.Send(stringToSend); }
				catch { return false; }
			}
			
#if DEBUG
			this.Parent.Log.WriteLine(9, this.Name + ": sent {" + stringToSend + "}");
#endif
			return true;
		}

		/// <summary>
		/// Initializes the socket
		/// </summary>
		private void setupSocket()
		{
			if (client != null)
			{
				if (client.IsConnected)
					client.Disconnect();
				client = null;
			}
			client = new SocketTcpClient(ServerAddresses[0], Port);
			client.ConnectionMode = TcpClientConnectionMode.Normal;
			client.ConnectionTimeOut = 1000;
			client.NoDelay = true;
			client.Connected += new TcpClientConnectedEventHandler(client_Connected);
			client.Disconnected += new TcpClientDisconnectedEventHandler(client_Disconnected);
			client.DataReceived += new TcpDataReceivedEventHandler(client_DataReceived);
		}

		/// <summary>
		/// Connect to the remote application and starts the command management system.
		/// If the Module is already running, it has no effect.
		/// </summary>
		public override void Start()
		{
			lock (lockObject)
			{
				if (!Enabled || ((parent != null) && !parent.IsRunning)) return;
				if (running) return;
				running = true;
				if ((client != null) && (client.IsOpen))
					client.Disconnect();
				setupSocket();
				SetupMainThread();
				this.lastDataInTime = DateTime.Now;
				this.lastActivityTime = DateTime.Now;
				OnStarted();
				OnStatusChanged();
			}
		}

		/// <summary>
		/// Disconnects from remote application and stops command management system.
		/// If the Module is not running, it has no effect.
		/// </summary>
		public override void Stop()
		{
			lock (lockObject)
			{
				int attemptt = 0;
				if (!running) return;
				stopMainThread = true;

				if (Simulation != ModuleSimulationOptions.SimulationDisabled)
				{
					OnStopped();
					OnStatusChanged();
					return;
				}

				lock (mainTheadLockObject)
				{
					if (mainThread != null)
					{
						mainThread.Join(250);
						if (mainThread.IsAlive)
						{
							mainThread.Abort();
							mainThread.Join();
							mainThread = null;
						}
					}
				}

				ExecuteStopActions();

				attemptt = 0;
				while (client.IsOpen && (attemptt++ < 3))
				{
					client.Disconnect();
					Thread.Sleep(10);
				}

				client = null;
				running = false;

				OnStopped();
				OnStatusChanged();
			}
		}

		/// <summary>
		/// Returns a String that represents the current Module. 
		/// </summary>
		/// <returns>A String that represents the current Module</returns>
		public override string ToString()
		{
			return name + " [" + serverAddresses.ToString() + ":" + port.ToString() + "]";
		}

		#region Thread Control Methods

		/// <summary>
		/// Disconnects from the Module application.
		/// </summary>
		protected override void MainThreadDisconnect()
		{
			this.dataReceived.Clear();
			if ((client != null) && client.IsOpen)
			{
				try { client.Disconnect(); }
				catch { }
			}
		}

		/// <summary>
		/// Connects with the remote module application for first time
		/// </summary>
		protected override void MainThreadFirstTimeConnect()
		{
			int addressIndex = 0;
			Stopwatch sw = new Stopwatch();

			while (running && !stopMainThread)
			{
				if (!client.TryConnect())
				{
					addressIndex = (addressIndex + 1) % this.serverAddresses.Count;
					client.ServerAddress = this.ServerAddresses[addressIndex];
					if (stopMainThread)
						break;
					AditionalActions();
					sw.Start();
					while ((sw.ElapsedMilliseconds < 100) && running && !stopMainThread)
						Thread.Sleep((int)Math.Max((100 - sw.ElapsedMilliseconds) / 2, 0));
					sw.Reset();
					continue;
				}
				else
				{
					IsAlive = true;
					Ready = !aliveCheck;
					break;
				}
			}
		}

		/// <summary>
		/// Checks the status of the connection with the Module application,
		/// and, when broken, tries to connect with the Module application.
		/// </summary>
		protected override void MainThreadLoopAutoConnect()
		{
			Stopwatch sw = new Stopwatch();

			if (!client.IsOpen)
			{
				if (stopMainThread) return;
				connecting = true;
				if (!client.TryConnect())
				{
					if (stopMainThread) return;
					sw.Start();
					while ((sw.ElapsedMilliseconds < 50) && running && !stopMainThread)
						Thread.Sleep((int)Math.Max((50 - sw.ElapsedMilliseconds) / 2, 0));
					sw.Reset();
					return;
				}

				else
				{
					IsAlive = true;
					Ready = !aliveCheck;
				}
				connecting = false;
			}
		}

		/// <summary>
		/// Thread for rimulation run mode
		/// </summary>
		private void MainSimulatedThread()
		{
			DateTime sendNextAliveTime = DateTime.Now;

			stopMainThread = false;
			running = true;
			Busy = false;
			IsAlive = true;
			Ready = true;

			while (running && !stopMainThread)
			{
				try
				{
					ParsePendingData();

					if (stopMainThread) break;
					Thread.Sleep(10);
				}
				catch (ThreadAbortException)
				{
					running = false;
					Busy = false;
					OnStopped();
					OnStatusChanged();
					return;
				}
				catch { continue; }
			}

			#region Stop and Clear Thread

			running = false;
			Busy = false;
			OnStopped();
			OnStatusChanged();

			if (Monitor.TryEnter(mainTheadLockObject))
			{
				// Thread thisThread = mainThread;
				mainThread = null;
				Monitor.PulseAll(mainTheadLockObject);
				Monitor.Exit(mainTheadLockObject);
			}

			#endregion
		}

		#endregion

		#endregion

		#region Event Handler Functions

		#region Socket Event Handler Functions

		/// <summary>
		/// Performs operations when the connection to remote application is stablished
		/// </summary>
		/// <param name="s">Socket used for connection</param>
		protected void client_Connected(Socket s)
		{
			Busy = false;
			/*
			if (AliveCheck)
			{
				try
				{
					if (busy) Send("busy");
					else if (!ready) Send("ready");
					else Send("alive");
				}
				catch { }
			}
			*/
			OnConnected();
			this.Parent.Log.WriteLine(9, this.Name + ": Client connected");
		}

		/// <summary>
		/// Performs operations when the connection to remote application has ended
		/// </summary>
		/// <param name="ep">Disconnection endpoint</param>
		protected void client_Disconnected(EndPoint ep)
		{
			OnDisconnected();
			Busy = false;
			Ready = false;
			clearLockList = true;
			this.Parent.Log.WriteLine(9, this.Name + ": Client disconnected");
		}

		/// <summary>
		/// Performs operations when data is received trough socket
		/// </summary>
		/// <param name="p">TCP Packet received</param>
		protected void client_DataReceived(TcpPacket p)
		{
			lastDataInTime = DateTime.Now;
			dataReceived.Produce(p);
			string dString = p.DataString;
#if DEBUG
			this.Parent.Log.WriteLine(9, this.Name + ": received " + p.Data.Length + "bytes {" + dString + "}");
#endif
		}

		#endregion

		#endregion

		#region Methods de Clase (Estáticos)

		/// <summary>
		/// Parses a module from a string
		/// </summary>
		/// <param name="s">String to parse</param>
		/// <returns>The module connection gateway or null if parse failed</returns>
		public static ModuleClientTcp FromString(string s)
		{
			//string rxName = @"(?<name>[a-zA-Z_][a-zA-Z0-9\-_]*)";
			string rxName = @"(?<name>[A-Z][A-Z\-]*)";
			string rxIP = @"(?<ip>(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}))";
			string rxSpace = @"[\t ]+";
			string rxPort = @"(?<port>(\d{3,5}))?[\t ]*(?<readonly>(readonly|read only))?";
			Match result;
			IPAddress ip;
			int port;

			Regex rx = new Regex(@"module" + rxSpace + rxName + rxSpace + rxIP + rxSpace + rxPort, RegexOptions.Compiled);
			result = rx.Match(s.Trim());
			if (!result.Success) return null;
			if (!IPAddress.TryParse(result.Result("${ip}"), out ip)) return null;
			if (result.Result("${readonly}") != "")
				port = 0;
			else if (!Int32.TryParse(result.Result("${port}"), out port)) return null;
			//result.Result("${name},${ip},${port}").Split(new char[] {','});
			return new ModuleClientTcp(result.Result("${name}"), new IPAddress[] { ip }, port, null);
			
		}

		#endregion
	}
}
