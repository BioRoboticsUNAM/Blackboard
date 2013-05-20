
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace BlackboardApi
{
	/// <summary>
	/// Implements a Blackboard Server implemented as IModule
	/// </summary>
	public class ModuleBlackboardServer : IModule
	{
		#region Variables
		/// <summary>
		/// Module name
		/// </summary>
		private string name;
		/// <summary>
		/// The blackboard this module is bind to
		/// </summary>
		internal Blackboard parent;
		/// <summary>
		/// List of the command prototypes supported by the Application Module
		/// </summary>
		private PrototypeCollection prototypes;
		/// <summary>
		/// Indicates if the Module require the "SENDER DESTINATION" prefix before the command name
		/// </summary>
		private bool requirePrefix;
		/// <summary>
		/// Indicates if simulation is active
		/// </summary>
		private SimulationOptions simOptions;
		
		#region Execution and Thread Vars

		/// <summary>
		/// Thread for start TCP server and perform async tasks
		/// </summary>
		private Thread mainThread;
		/// <summary>
		/// Indicates if the ModuleBlackboardServer is runnuing
		/// </summary>
		private bool running;
		/// <summary>
		/// Indicates if the MainThread must be stopped
		/// </summary>
		private bool stopMainThread;
		/// <summary>
		/// Indicates if a module restart has been requested
		/// </summary>
		private bool restartRequested;
		/// <summary>
		/// Indicates if a test restart has been requested
		/// </summary>
		private bool restartTestRequested;

		#endregion

		#region Action Lists
		
		/// <summary>
		/// List of actions to perform when module is restarted
		/// </summary>
		private ActionCollection restartActions;
		/// <summary>
		/// List of actions to perform when a restart test is requested
		/// </summary>
		private ActionCollection restartTestActions;
		/// <summary>
		/// List of actions to perform when module is started
		/// </summary>
		private ActionCollection startupActions;
		/// <summary>
		/// List of actions to perform when module is started
		/// </summary>
		private ActionCollection stopActions;

		#endregion

		#region Message flow vars
		/// <summary>
		/// Indicates if the module is responding
		/// </summary>
		private bool alive;
		/// <summary>
		/// Indicates if the ModuleBlackboardServer must check if Module is alive
		/// </summary>
		private bool aliveCheck;
		/// <summary>
		/// Tells if the module is busy
		/// </summary>
		private bool busy;
		/// <summary>
		/// Tells if the module supports binary commands
		/// </summary>
		private bool bin;
		/// <summary>
		/// Contains the time when the last packet was received
		/// </summary>
		private DateTime lastDataInTime;
		/// <summary>
		/// List of commands sent which are waiting for response
		/// </summary>
		private List<Command> lockList;
		/// <summary>
		/// Commands the main thread to clear the LockList to prevent list to be modified and get trouble
		/// </summary>
		private bool clearLockList = false;
		/// <summary>
		/// Stores all data received trough socket
		/// </summary>
		private Queue<byte[]> dataReceived;
		#endregion

		#region Socket and connection vars

		/// <summary>
		/// Connection socket to the Blackboard
		/// </summary>
		private SocketTcpServer server;
		/// <summary>
		/// Port where to accept incomming connections
		/// </summary>
		private int port;

		#endregion

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the ModuleBlackboardServer class 
		/// </summary>
		/// <param name="name">Module name</param>
		/// <param name="port">Port for accepting incoming connections</param>
		internal ModuleBlackboardServer(string name, int port)
		{
			if ((port < 1024) || (port > 65535)) throw new ArgumentException("Port must be between 1024 and 65535", "port");
			if (!Regex.IsMatch(name, @"\w[\w\-_\d]{2,}")) throw new ArgumentException("Invalid module name", "name");
			this.name = name;
			this.port = port;
			this.busy = false;
			//this.waitingResponse = new List<Command>(10);
			//this.responses = new List<Response>();
			//this.commands = new List<Command>();
			this.prototypes = new PrototypeCollection(this);
			this.lockList = new List<Command>();
			this.restartActions = new ActionCollection(this);
			this.restartTestActions = new ActionCollection(this);
			this.startupActions = new ActionCollection(this);
			this.stopActions = new ActionCollection(this);
			this.dataReceived = new Queue<byte[]>(10);

			this.restartRequested = false;
			this.restartTestRequested = false;
			this.simOptions = SimulationOptions.SimulationDisabled;
		}

		/// <summary>
		/// Initializes a new instance of the ModuleBlackboardServer class 
		/// </summary>
		/// <param name="name">Module name</param>
		/// <param name="port">Port of the Application Module</param>
		/// <param name="prototypes">List of the command prototypes supported by the Application Module</param>
		public ModuleBlackboardServer(string name, int port, Prototype[] prototypes)
			: this(name, port)
		{
			if ((prototypes == null) || (prototypes.Length == 0)) throw new ArgumentException("The prototypes list cannot be zero-length nor null");
			for (int i = 0; i < prototypes.Length; ++i)
				this.prototypes.Add(prototypes[i]);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating if the current ModuleBlackboardServer supports binnary commands
		/// </summary>
		public bool Bin
		{
			get
			{
				return bin;
			}
		}

		/// <summary>
		/// Gets a value indicating if the current module is busy (waiting for a response)
		/// </summary>
		public bool Busy
		{
			get
			{
				return busy;
			}
			protected set
			{
				if (busy == value) return;
				busy = value;
				//if(!(busy = value))
				//	clearLockList = true;
				if (running && (BusyChanged != null)) BusyChanged(this);
				if (running && (StatusChanged != null)) StatusChanged(this);
			}
		}

		/// <summary>
		/// Gets a value indicating if the ModuleBlackboardServer is responding and working
		/// </summary>
		public bool IsAlive
		{
			get { return alive; }
			protected set
			{
				if (alive == value) return;
				alive = value;
				if (running && (AliveChanged != null)) AliveChanged(this);
				if (running && (StatusChanged != null)) StatusChanged(this);
			}
		}

		/// <summary>
		/// Gets a value indicating if if the TcpServer has been started
		/// </summary>
		public bool IsConnected
		{
			get
			{
				return this.IsServerStarted;
			}
		}

		/// <summary>
		/// Gets a value indicating if the ModuleBlackboardServer is running
		/// </summary>
		public bool IsRunning
		{
			get { return running; }
		}

		/// <summary>
		/// Gets a value indicating if the TcpServer has been started
		/// </summary>
		public bool IsServerStarted
		{
			get
			{
				if (simOptions != SimulationOptions.SimulationDisabled) return true;
				if (server != null)
					return server.Started;
				return false;
			}
		}
		
		/// <summary>
		/// Gets the name of the Module
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Gets the blackboard this ModuleClient is bind to
		/// </summary>
		public Blackboard Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		/// <summary>
		/// Gets the port where to connect to the Application Module
		/// </summary>
		public int Port
		{
			get { return port; }
		}

		/// <summary>
		/// Gets the prototypes managed by the Module
		/// </summary>
		public PrototypeCollection Prototypes
		{
			get
			{
				return prototypes;
			}
		}

		/// <summary>
		/// Gets a value ingicating if the ModuleBlackboardServer require the "SENDER DESTINATION" prefix before the command name
		/// </summary>
		public bool RequirePrefix
		{
			get
			{
				return requirePrefix;
			}
			internal set
			{
				requirePrefix = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating which mode of simulation is active
		/// </summary>
		public SimulationOptions Simulation
		{
			get { return simOptions; }
			set
			{
				if (simOptions == value) return;
				if (running) throw new Exception("Cannot change simulation mode while running");
				simOptions = value;
				if (this.StatusChanged != null) this.StatusChanged(this);
			}
		}

		#region Action Lists Properties

		/// <summary>
		/// Gets the list of actions to perform when module is restarted
		/// </summary>
		internal ActionCollection RestartActions
		{
			get
			{
				return restartActions;
			}
		}

		/// <summary>
		/// Gets the list of actions to perform when a test restart is requested
		/// </summary>
		internal ActionCollection RestartTestActions
		{
			get
			{
				return restartTestActions;
			}
		}

		/// <summary>
		/// Gets the list of actions to perform when module is started
		/// </summary>
		internal ActionCollection StartupActions
		{
			get
			{
				return startupActions;
			}
		}

		/// <summary>
		/// Gets the list of actions to perform when module is stopped
		/// </summary>
		internal ActionCollection StopActions
		{
			get
			{
				return stopActions;
			}
		}

		#endregion

		#endregion

		#region Events

		/// <summary>
		/// Occurs when an action is executed
		/// </summary>
		internal event ActionExecutedEH ActionExecuted;

		/// <summary>
		/// Occurs when the IsAlive property of a ModuleBlackboardServer object changes its value
		/// </summary>
		public event StatusChangedEH AliveChanged;

		/// <summary>
		/// Occurs when the Busy property of a ModuleBlackboardServer object changes its value
		/// </summary>
		public event StatusChangedEH BusyChanged;

		/// <summary>
		/// Occurs when the Tcp Server of this ModuleBlackboardServer is started
		/// </summary>
		public event ModuleConnectionEH ServerStarted;

		/// <summary>
		/// Occurs when the Tcp Server of this ModuleBlackboardServeris stopped
		/// </summary>
		public event ModuleConnectionEH ServerStopped;

		/// <summary>
		/// Occurs when a Command is received trough socket
		/// </summary>
		public event CommandReceivedEH CommandReceived;

		/// <summary>
		/// Occurs when a Response is received trough socket
		/// </summary>
		public event ResponseReceivedEH ResponseReceived;

		/// <summary>
		/// Occurs when the status of a ModuleBlackboardServer object changes
		/// </summary>
		public event StatusChangedEH StatusChanged;

		/// <summary>
		/// Occurs when the status of a ModuleBlackboardServer object starts working
		/// </summary>
		public event StatusChangedEH Started;

		/// <summary>
		/// Occurs when the status of a ModuleBlackboardServer object stops working
		/// </summary>
		public event StatusChangedEH Stopped;

		#endregion

		#region Methods

		/// <summary>
		/// Asynchronously stops the socket connection and command management system.
		/// If the ModuleBlackboardServer is not running, it has no effect.
		/// </summary>
		public void BeginStop()
		{
			if (!running) return;
			if (Simulation != SimulationOptions.SimulationDisabled)
			{
				if (Stopped != null) Stopped(this);
				if (StatusChanged != null) StatusChanged(this);	
			}
			stopMainThread = true;
		}

		/// <summary>
		/// Performs Restart Tasks
		/// </summary>
		private void DoRestartModule()
		{
			Busy = true;

			Unlock();
			dataReceived.Clear();
			if ((server != null) && server.Started)
			{
				try { server.Stop(); }
				catch { }
			}
			ExecuteRestartActions();
			Busy = false;
		}

		/// <summary>
		/// Performs Restart-test Tasks
		/// </summary>
		private void DoRestartTest()
		{
			dataReceived.Clear();
			Unlock();
			ExecuteRestartTestActions();
		}

		/// <summary>
		/// Check if a string is a control command
		/// </summary>
		/// <param name="s">String to verify</param>
		/// <returns>true if command parsed successfully, false otherwise</returns>
		private bool IsControlCommand(string s)
		{
			string[] parts;
			char[] separator = { '\t', ' ', '\r', '\n' };

			if ((s == null) || (s.Length < 3)) return false;

			parts = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			switch (parts[0])
			{
				case "alive":
				case "bin":
				case "busy":
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// Marks the module as Busy and adds the command to list of blocking commands
		/// </summary>
		/// <param name="command"></param>
		private void Lock(Command command)
		{
			lockList.Add(command);
			Busy = true;
		}
		
		/// <summary>
		/// Parses a control command
		/// </summary>
		/// <param name="s">String to parse</param>
		/// <returns>true if command parsed successfully, false otherwise</returns>
		private bool ParseControlCommand(string s)
		{
			string[] parts;
			char[] separator = { '\t', ' ', '\r', '\n' };

			if ((s == null) || (s.Length < 3)) return false;

			parts = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			switch (parts[0])
			{
				case "alive":
					IsAlive = true;
					return true;

				case "bin":
					if (parts.Length < 2) return false;
					if ((parts[1] != "1") && (parts[1] != "0")) return false;
					IsAlive = true;
					return true;

				case "busy":
					if (parts.Length < 2) return false;
					if ( (parts[1] != "1") && (parts[1] != "0")) return false;
					if (!(Busy = (parts[1] == "1"))) Unlock();
					IsAlive = true;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// Parses all pending TCPPackets received
		/// </summary>
		private void ParsePendingData()
		{
			string message;
			Command c;
			Response r;

			while (dataReceived.Count > 0)
			{
				try
				{
					message = System.Text.ASCIIEncoding.ASCII.GetString(dataReceived.Dequeue());
					if (message.IndexOf('\0') != -1) message = message.Substring(0, message.IndexOf('\0'));
					message = message.Trim();
				}
				catch
				{
					continue;
					//message = "";
				}

				// Check if message received is a control command
				if (IsControlCommand(message))
					return;
				// Check if message received is a command
				if (Command.IsCommand(message) && Command.TryParse(message, this, out c))
				{
					// A command has been received so it is redirected to blackboard
					if (CommandReceived != null)
						CommandReceived(this, c);
				}
				// Check if message received is a response
				else if (Response.IsResponse(message) && Response.TryParse(message, this, out r))
				{
					// A response has been received so it is redirected to blackboard
					if (ResponseReceived != null)
						ResponseReceived(this, r);
					Unlock(r);
				}
			}
		}

		/// <summary>
		/// Request a restart of the remote module
		/// </summary>
		public void Restart()
		{
			this.restartRequested = true;
		}

		/// <summary>
		/// Executes actions to prepare a test restart
		/// </summary>
		public void RestartTest()
		{
			this.restartTestRequested = true;
		}

		/// <summary>
		/// Sends a command to the module
		/// </summary>
		/// <param name="command">Response to send</param>
		/// <returns>true if the command has been sent, false otherwise</returns>
		public bool Send(Command command)
		{
			// Check if this module is the destination module for the command
			if (this != command.Destination)
				throw new Exception("Command marked to be sent through other module");

			// Simulation mode
			#region Simulation Mode
			if (simOptions != SimulationOptions.SimulationDisabled)
			{
				//Response r = Response.CreateFromCommand(command, true);
				dataReceived.Enqueue(
					System.Text.ASCIIEncoding.Default.GetBytes(Response.CreateFromCommand(command, true).ToString())
					);
				return true;
			}
			#endregion

			// Check is connection has been stablished.
			if (!server.Started)
				return false;
			// Check if module is not busy and command is not a priority command
			if (Busy && !command.Prototype.HasPriority)
				return false;
			// Send the command
			try
			{
				server.SendToAll(command.StringToSend + "\0");
				if(command.Prototype.ResponseRequired) Lock(command);
			}
			catch { return false; }
			return true;
		}

		/// <summary>
		/// Synchronusly sends a command response to the module
		/// </summary>
		/// <param name="response">Response to send</param>
		/// <returns>true if the response has been sent, false otherwise</returns>
		public bool Send(Response response)
		{
			// Check if this module is the destination module for the response
			if (this != response.Destination)
				throw new Exception("Response marked to be sent through other module");

			// Simulation mode
			#region Simulation Mode
			if (simOptions != SimulationOptions.SimulationDisabled)
			{
				return true;
			}
			#endregion

			// Check is connection has been stablished.
			if (!server.Started)
				return false;
			// Send the response
			try { server.SendToAll(response.StringToSend + "\0"); }
			catch { return false; }
			return true;
		}

		/// <summary>
		/// Initializes the socket
		/// </summary>
		private void setupSocket()
		{
			server = new SocketTcpServer(Port);
			server.DataReceived += new TcpDataReceivedEventHandler(client_DataReceived);
		}

		/// <summary>
		/// Returns a value indicating if the command is supported by this module
		/// </summary>
		/// <param name="commandName">The name of the command to search for</param>
		/// <returns>True if the command was found. false otherwise</returns>
		public bool SupportCommand(string commandName)
		{
			if ((commandName == null) || !Regex.IsMatch(commandName, @"[A-Za-z_]+")) return false;
			commandName = commandName.ToLower();
			foreach (Prototype p in prototypes)
				if (p.Command == commandName)
					return true;
			return false;
		}

		/// <summary>
		/// Returns a value indicating if the command is supported by this module
		/// </summary>
		/// <param name="commandName">The name of the command to search for</param>
		/// <param name="prototype">When this method returns, contains the Prototype that brings support to this command
		/// if the search succeeded, or null if the module does not supports the command.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>True if the command was found. false otherwise</returns>
		public bool SupportCommand(string commandName, out Prototype prototype)
		{
			prototype = null;
			if ((commandName == null) || !Regex.IsMatch(commandName, @"[A-Za-z_]+")) return false;
			commandName = commandName.ToLower();
			foreach (Prototype p in prototypes)
			{
				if (p.Command == commandName)
				{
					prototype = p;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Connect to the remote application and starts the command management system.
		/// If the ModuleBlackboardServer is already running, it has no effect.
		/// </summary>
		public void Start()
		{
			if (running) return;
			running = true;
			if ((server != null) && (server.Started))
				server.Stop();
			setupSocket();
			SetupMainThread();
			this.lastDataInTime = DateTime.Now;
			if (Started != null) Started(this);
			if (StatusChanged != null) StatusChanged(this);
			
		}

		/// <summary>
		/// Disconnects from remote application and stops command management system.
		/// If the ModuleBlackboardServer is not running, it has no effect.
		/// </summary>
		public void Stop()
		{
			int attempt = 0;
			if (!running) return;
			stopMainThread = true;

			if (Simulation != SimulationOptions.SimulationDisabled)
			{
				if (Stopped != null) Stopped(this);
				if (StatusChanged != null) StatusChanged(this);	
				return;
			}

			if (mainThread != null)
			{
				//while (mainThread.IsAlive && (attempt++ < 3))
				//	Thread.Sleep(250);
				mainThread.Abort();
				mainThread.Join();
				mainThread = null;
			}

			ExecuteStopActions();

			attempt = 0;
			while (server.Started && (attempt++ < 3))
			{
				server.Stop();
				Thread.Sleep(10);
			}
			
			server = null;
			running = false;		

			if (Stopped != null) Stopped(this);
			if (StatusChanged != null) StatusChanged(this);			
		}

		/// <summary>
		/// Returns a String that represents the current ModuleBlackboardServer. 
		/// </summary>
		/// <returns>A String that represents the current ModuleBlackboardServer</returns>
		public override string ToString()
		{
			return name + " [" + port.ToString() + "]";
		}

		/// <summary>
		/// Marks the module as free
		/// </summary>
		private void Unlock()
		{
			lockList.Clear();
			Busy = false;
		}

		/// <summary>
		/// Checks if the response corresponds to a command that is ocupying the module to allow unlock it.
		/// </summary>
		/// <param name="r">Response to unlock with</param>
		internal void Unlock(Response r)
		{
			for (int i = 0; i < lockList.Count; ++i)
			{
				if (
					(lockList[i].Command == r.Command) &&
					//(lockList[i].Id == r.Id) &&
					(lockList[i].SentTime < r.ArrivalTime))
				{
					lockList.RemoveAt(i);
					break;
				}
			}
			if (lockList.Count <= 0) Busy = false;
		}

		#region Action management methods

		/// <summary>
		/// Executes all startup actions.
		/// </summary>
		private void ExecuteRestartActions()
		{
			bool result;
			for (int i = 0; i < restartActions.Count; ++i)
			{
				result = restartActions[i].Execute();
				if (ActionExecuted != null)
					ActionExecuted(this, restartActions[i], result);
			}
		}

		/// <summary>
		/// Executes all Restart-Test actions.
		/// </summary>
		private void ExecuteRestartTestActions()
		{
			bool result;
			for (int i = 0; i < restartTestActions.Count; ++i)
			{
				result = restartTestActions[i].Execute();
				if (ActionExecuted != null)
					ActionExecuted(this, restartTestActions[i], result);
			}
		}

		/// <summary>
		/// Executes all startup actions.
		/// </summary>
		private void ExecuteStartupActions()
		{
			bool result;
			for (int i = 0; i < startupActions.Count; ++i)
			{

				if (startupActions[i].Type == ActionType.Send) continue;
				result = startupActions[i].Execute();
				if (ActionExecuted != null)
					ActionExecuted(this, startupActions[i], result);
			}
		}

		/// <summary>
		/// Executes all stop actions.
		/// </summary>
		private void ExecuteStopActions()
		{
			bool result;
			for (int i = 0; i < stopActions.Count; ++i)
			{
				result = stopActions[i].Execute();
				if (ActionExecuted != null)
					ActionExecuted(this, stopActions[i], result);
			}
		}

		/// <summary>
		/// Returns a string array containing the string representation of the actions
		/// this module will perform on restart
		/// </summary>
		/// <returns>string array</returns>
		public string[] GetRestartActions()
		{
			string[] rsa = new string[restartActions.Count];
			for (int i = 0; i < restartActions.Count; ++i)
				rsa[i] = restartActions[i].ToString();
			return rsa;
		}

		/// <summary>
		/// Returns a string array containing the string representation of the actions
		/// this module will perform on restart-test
		/// </summary>
		/// <returns>string array</returns>
		public string[] GetRestartTestActions()
		{
			string[] rsa = new string[restartTestActions.Count];
			for (int i = 0; i < restartTestActions.Count; ++i)
				rsa[i] = restartTestActions[i].ToString();
			return rsa;
		}

		/// <summary>
		/// Returns a string array containing the string representation of the actions
		/// this module will perform on start
		/// </summary>
		/// <returns>string array</returns>
		public string[] GetStartupActions()
		{
			string[] sa = new string[startupActions.Count];
			for (int i = 0; i < startupActions.Count; ++i)
				sa[i] = startupActions[i].ToString();
			return sa;
		}

		/// <summary>
		/// Returns a string array containing the string representation of the actions
		/// this module will perform on stop
		/// </summary>
		/// <returns>string array</returns>
		public string[] GetStopActions()
		{
			string[] sa = new string[startupActions.Count];
			for (int i = 0; i < startupActions.Count; ++i)
				sa[i] = startupActions[i].ToString();
			return sa;
		}

		#endregion

		#region Thread Control Methods

		/// <summary>
		/// Thread for start the Tcp Server and perform Module tasks
		/// </summary>
		private void MainThreadTask()
		{
			DateTime sendNextAliveTime = DateTime.Now;
			Thread thisThread;
			int i;

			stopMainThread = false;
			restartRequested = false;
			restartTestRequested = false;
			running = true;
			Busy = true;
			ExecuteStartupActions();
			Busy = false;

			#region First time connection
			// This section is to allow to send the startup actions on first time connection.
			while (running && !stopMainThread)
			{
				if (!server.Started)
				{
					if (stopMainThread) break;
					server.Start();
					Thread.Sleep(100);
					continue;
				}
				else
				{
					IsAlive = true;
					break;
				}
			}

			#region Send startup commands
			for (i = 0; (i < startupActions.Count) && !stopMainThread; ++i)
			{
				if (startupActions[i].Type != ActionType.Send) continue;
				bool result = startupActions[i].Execute();
				if (ActionExecuted != null)
					ActionExecuted(this, startupActions[i], result);
			}
			#endregion

			#endregion

			while (running && !stopMainThread)
			{
				// Autoconnect (if disconnected)
				#region Autoconnect
				if (!server.Started)
				{
					if (stopMainThread) break;
					server.Start();
					Thread.Sleep(10);
					continue;
					
				}
				else IsAlive = true;
				#endregion

				// Parse received data
				ParsePendingData();

				// Check if module is alive and responding
				#region Check alive
				CheckAlive(ref sendNextAliveTime);
				#endregion

				// Check if module is busy and has not hang
				#region Check Busy
				CheckBusy();
				#endregion

				// Performs additional actions
				if (restartTestRequested) DoRestartTest();
				if (restartRequested) { DoRestartModule(); continue; }

				if (stopMainThread) break;
				Thread.Sleep(1);
			}
			Busy = true;

			ExecuteStopActions();

			if ((server != null) && server.Started)
			{
				try { server.Stop(); }
				catch { }
			}
			
			running = false;
			Busy = false;
			if (Stopped != null) Stopped(this);
			if (StatusChanged != null) StatusChanged(this);

			#region Stop and Clear Thread

			thisThread = mainThread;
			mainThread = null;
			thisThread.Abort();
			thisThread.Join();


			#endregion
		}

		/// <summary>
		/// Thread for simulation run mode
		/// </summary>
		private void MainSimulatedThread()
		{
			DateTime sendNextAliveTime = DateTime.Now;
			Thread thisThread;

			stopMainThread = false;
			running = true;
			Busy = false;
			IsAlive = true;

			while (running && !stopMainThread)
			{
				ParsePendingData();

				if (stopMainThread) break;
				Thread.Sleep(10);
			}

			#region Stop and Clear Thread

			thisThread = mainThread;
			mainThread = null;
			thisThread.Abort();
			thisThread.Join();

			#endregion
		}

		/// <summary>
		/// Initializes the thread for autoconnection to ModuleServer
		/// If the server is running starts the thread automatically
		/// </summary>
		private void SetupMainThread()
		{
			if (mainThread != null) try { mainThread.Abort(); }
				catch { }
			if(simOptions == SimulationOptions.SimulationDisabled) mainThread = new Thread(new ThreadStart(MainThreadTask));
			else mainThread = new Thread(new ThreadStart(MainSimulatedThread));
			mainThread.IsBackground = true;
			if (running) mainThread.Start();
		}

		#endregion

		#endregion

		#region Event Handler Functions

		#region Socket Event Handler Functions

		/// <summary>
		/// Performs operations when the connection to remote application is stablished
		/// </summary>
		/// <param name="s">Socket used for connection</param>
		private void client_Connected(Socket s)
		{
			Busy = false;
			if (Connected != null)
				Connected(this);
		}

		/// <summary>
		/// Performs operations when the connection to remote application has ended
		/// </summary>
		/// <param name="s">Socket used for connection</param>
		private void client_Disconnected(Socket s)
		{
			if (Disconnected != null)
				Disconnected(this);
			Busy = false;
			clearLockList = true;
		}

		/// <summary>
		/// Performs operations when data is received trough socket
		/// </summary>
		/// <param name="p">TCP Packet received</param>
		private void client_DataReceived(TcpPacket p)
		{
			lastDataInTime = DateTime.Now;
			dataReceived.Enqueue(p.Data);
		}

		#endregion

		#endregion

		#region Methods de Clase (Estáticos)

		/// <summary>
		/// Parses a module from a string
		/// </summary>
		/// <param name="s">String to parse</param>
		/// <returns>The module connection gateway or null if parse failed</returns>
		public static ModuleClient FromString(string s)
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
			return new ModuleClient(result.Result("${name}"), ip, port, null);
			
		}

		#endregion

		#region IComparable<ModuleServer> Members

		/// <summary>
		/// Compares the current ModuleClient with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(IModule other)
		{
			return name.CompareTo(other.Name);
		}

		#endregion

	}
}

