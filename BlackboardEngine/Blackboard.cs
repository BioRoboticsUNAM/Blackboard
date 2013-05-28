using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;

using Blk.Api;
using Blk.Engine.Actions;
using Blk.Engine.Remote;
using Blk.Engine.SharedVariables;
using Robotics;
using Robotics.Utilities;


namespace Blk.Engine
{
	/// <summary>
	/// Implements a blackboard
	/// </summary>
	public class Blackboard : IBlackboard
	{
		#region Variables
		/// <summary>
		/// Controls how much info is displayed
		/// </summary>
		private int verbosity = 0;
		/// <summary>
		/// Contains all modules supported by the blackboard
		/// </summary>
		private ModuleCollection modules;
		/// <summary>
		/// A LogWriter to log all operations
		/// </summary>
		private ILogWriter log;
		/// <summary>
		/// Stores the Running Status of the blackboard
		/// </summary>
		private BlackboardRunningStatus runningStatus;
		/// <summary>
		/// Represents the virtual module that handles blackboard commands
		/// </summary>
		private ModuleBlackboard virtualModule;
		/// <summary>
		/// Indicates when a restart is requested
		/// </summary>
		private bool restartRequested;
		/// <summary>
		/// Indicates when a RestartTest is requested
		/// </summary>
		private bool restartTestRequested;
		/// <summary>
		/// Plugins repository
		/// </summary>
		private BlackboardPluginManager pluginManager;

		/// <summary>
		/// Manages the excecution sequence of the module programs
		/// </summary>
		private StartupSequenceManager startupSequence;

		/// <summary>
		/// Manages the shutdown sequence of the module programs
		/// </summary>
		private ShutdownSequenceManager shutdownSequence;

		#region Socket and connection vars

		/// <summary>
		/// Connection socket to the Blackboard
		/// </summary>
		private SocketTcpServer server;
		/// <summary>
		/// Port where to accept incomming connections
		/// </summary>
		private int port;
		/// <summary>
		/// Indicates if the Blackboard is runnuing
		/// </summary>
		private bool running;

		#endregion

		#region Message flow vars
		/// <summary>
		/// Queue of responses waiting for retry redirect
		/// </summary>
		private Queue<Response> retryQueue;
		/// <summary>
		/// Stores the number of attempts while redirecting a response
		/// </summary>
		private int sendAttempts;
		/// <summary>
		/// Thread for parse messages received throug server
		/// </summary>
		private Thread threadMessageParser;
		/// <summary>
		/// Main Thread (manages startup and message redirection)
		/// </summary>
		private Thread mainThread;
		/// <summary>
		/// Queue of commands received and pending to send
		/// </summary>
		private ProducerConsumer<Command> commandsPending;
		/// <summary>
		/// List of sent commands which are waiting for response
		/// </summary>
		private List<Command> commandsWaiting;
		/// <summary>
		/// List of responses received
		/// </summary>
		private List<Response> responses;
		/// <summary>
		/// Stores messages received trough Blackboard server waiting to be parsed
		/// </summary>
		private ProducerConsumer<TcpPacket> dataReceived;
		/// <summary>
		/// Indicates that the main thread has been started an is running
		/// </summary>
		private ManualResetEvent mainThreadRunningEvent;
		/// <summary>
		/// Indicates that the parser thread has been started an is running
		/// </summary>
		private ManualResetEvent parserThreadRunningEvent;
		#endregion

		#region Time Limit Vars

		/// <summary>
		/// Stores the time when the blackboard was started
		/// </summary>
		private DateTime startupTime;
		/// <summary>
		/// Stores the amount of time to wait before stop automatically the blackboard
		/// </summary>
		private TimeSpan autoStopTime;
		/// <summary>
		/// Stores the amount of time to wait before launch Test-Timeout actions
		/// </summary>
		private TimeSpan testTimeOut;
		/// <summary>
		/// Indicates if Test-Timeout actions has been executed.
		/// </summary>
		private bool testTimeOutExecuted;
		/// <summary>
		/// The time delay between load of each module in milliseconds
		/// </summary>
		private int moduleLoadDelay;

		#endregion

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Blackboard
		/// </summary>
		private Blackboard()
		{
			this.modules = new ModuleCollection(this, 6);
			//modules.ModuleAdded += new ModuleClientAddRemoveEH(modules_ModuleAdded);
			//modules.ModuleRemoved += new ModuleClientAddRemoveEH(modules_ModuleRemoved);
			this.modules.ModuleAdded += new IModuleAddRemoveEH(modules_ModuleAdded);
			this.modules.ModuleRemoved += new IModuleAddRemoveEH(modules_ModuleRemoved);
			this.commandsPending = new ProducerConsumer<Command>(1000);
			this.commandsWaiting = new List<Command>(100);
			this.responses = new List<Response>(100);
			this.log = new LogWriter();
			this.log.VerbosityTreshold = verbosity;
			this.dataReceived = new ProducerConsumer<TcpPacket>(1000);
			this.retryQueue = new Queue<Response>(100);
			this.sendAttempts = 0;
			this.testTimeOut = TimeSpan.MinValue;
			this.autoStopTime = TimeSpan.MinValue;
			this.startupSequence = new StartupSequenceManager(this);
			this.shutdownSequence = new ShutdownSequenceManager(this);
			this.pluginManager = new BlackboardPluginManager(this);

			this.mainThreadRunningEvent = new ManualResetEvent(false);
			this.parserThreadRunningEvent = new ManualResetEvent(false);
		}

		/// <summary>
		/// Initializes a new instance of Blackboard
		/// </summary>
		/// <param name="port">The port where messajes will arrive</param>
		public Blackboard(int port) : this()
		{
			this.Port = port;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the amount of time to wait before stop automatically the blackboard
		/// </summary>
		public TimeSpan AutoStopTime
		{
			get { return autoStopTime; }
			set { autoStopTime = value; }
		}

		/// <summary>
		/// Gets the remaining time for AutoStop
		/// </summary>
		public TimeSpan AutoStopTimeLeft
		{
			get 
			{
				if ((startupTime == null) || (autoStopTime < TimeSpan.Zero)) return TimeSpan.Zero;
				return startupTime.Add(autoStopTime) - DateTime.Now;
			}
		}

		/// <summary>
		/// Gets the number of clients connected to de Blackboard Server
		/// </summary>
		public int ClientsConnected
		{
			get { if (server == null) return 0;
				return server.ClientsConnected; }
		}

		/// <summary>
		/// Gets or sets a textwriter to write in all operations
		/// </summary>
		public ILogWriter Log
		{
			get { return log; }
			set
			{
				if (value == null) throw new ArgumentNullException("value");
				//log = TextWriter.Synchronized(value);
				log = value;
			}
		}

		/// <summary>
		/// Gets the modules managed by the Blackboard
		/// </summary>
		public IModuleCollection Modules
		{
			get { return modules; }
		}

		/// <summary>
		/// Gets an array of commands pending to send
		/// </summary>
		public Command[] PendingCommands
		{
			get { return commandsPending.ToArray(); }
		}

		/// <summary>
		/// Gets an array of commands pending to send
		/// </summary>
		ITextCommand[] IBlackboard.PendingCommands
		{
			get { return commandsPending.ToArray(); }
		}

		/// <summary>
		/// Gets the Plugins repository where blackboard plugins are stored
		/// </summary>
		public IBlackboardPluginManager PluginManager
		{
			get { return this.pluginManager; }
		}

		/// <summary>
		/// Gets the port when the blackboard accept incomming connections
		/// </summary>
		public int Port
		{
			get { return port; }
			set
			{
				if ((value < 1) || (value > 65535))
					throw new ArgumentOutOfRangeException("Port must be between 1 and 65535");
				if (this.IsRunning)
					throw new Exception("Can not change the port when the blackboard is running");
				this.port = value;
			}
		}

		/// <summary>
		/// Gets the Runnin Status of this blackboard instance
		/// </summary>
		public BlackboardRunningStatus RunningStatus
		{
			get { return this.runningStatus; }
			protected set
			{
				if (value == this.runningStatus) return;
				runningStatus = value;
				if (this.StatusChanged != null)
					this.StatusChanged(this);
			}
		}

		/// <summary>
		/// Gets or sets the the virtual module that handles blackboard commands
		/// </summary>
		public ModuleBlackboard VirtualModule
		{
			get { return virtualModule; }
			private set
			{
				if (value == null) throw new ArgumentNullException();
				virtualModule = value;
				if (!modules.Contains(value))
					modules.Add(value);
			}
		}

		/// <summary>
		/// Gets or sets the the virtual module that handles blackboard commands
		/// </summary>
		IModuleBlackboard IBlackboard.VirtualModule
		{
			get { return this.virtualModule; }
		}

		/// <summary>
		/// Gets a value indicating if the Blackboard is runnuing
		/// </summary>
		public bool IsRunning
		{
			//get { return running || threadMessageParser.IsAlive || threadRedirector.IsAlive; }
			get { return running; }
		}

		/// <summary>
		/// Gets or sets the number of attempts while redirecting a response
		/// </summary>
		public int SendAttempts
		{
			get { return sendAttempts; }
			set
			{
				if (sendAttempts < 0) throw new ArgumentOutOfRangeException();
				sendAttempts = value;
			}
		}

		/// <summary>
		/// Gets the ShutdownSequence object used to terminate the module programs
		/// </summary>
		public ShutdownSequenceManager ShutdownSequence
		{
			get { return this.shutdownSequence; }
		}

		/// <summary>
		/// Gets the StartupSequence object used to execute the module programs
		/// </summary>
		public StartupSequenceManager StartupSequence
		{
			get { return this.startupSequence; }
		}

		/// <summary>
		/// Gets the remaining time for perform Test-Timeout actions
		/// </summary>
		public TimeSpan TestTimeLeft
		{
			get
			{
				if ((startupTime == null) || (testTimeOut < TimeSpan.Zero)) return TimeSpan.Zero;
				TimeSpan timeLeft = startupTime.Add(testTimeOut) - DateTime.Now;
				if (timeLeft < TimeSpan.Zero) return TimeSpan.Zero;
				return timeLeft;
			}
		}

		/// <summary>
		/// Gets or sets the amount of time to wait before perform Test-Timeout actions
		/// </summary>
		public TimeSpan TestTimeOut
		{
			get { return testTimeOut; }
			set
			{
				if ((value == null) || (value <= TimeSpan.Zero)) throw new ArgumentException("Value cannot be negative nor null");
				testTimeOut = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the time delay between loads of each module in milliseconds
		/// </summary>
		public int ModuleLoadDelay
		{
			get { return this.moduleLoadDelay;}
			set
			{
				if (value < 0)
					throw new ArgumentException("Value must be equal or greater than zero");
				moduleLoadDelay = value;
			}
		}

		/// <summary>
		/// Gets or sets how uch data is sent to the console
		/// </summary>
		public int VerbosityLevel
		{
			get { return verbosity; }
			set
			{
				if ((value < 0) || (value > 9))
					throw new ArgumentOutOfRangeException();
				verbosity = value;
				if (log != null) log.VerbosityTreshold = verbosity;
			}
		}

		/// <summary>
		/// Gets an array of commands waiting for response
		/// </summary>
		public Command[] WaitingCommands
		{
			get { return commandsWaiting.ToArray(); }
		}

		/// <summary>
		/// Gets an array of commands waiting for response
		/// </summary>
		ITextCommand[] IBlackboard.WaitingCommands
		{
			get { return commandsWaiting.ToArray(); }
		}

		#endregion

		#region Events
		/// <summary>
		/// Raises when a client connects to Blackboard TCPServer
		/// </summary>
		public event TcpClientConnectedEventHandler ClientConnected;
		/// <summary>
		/// Raises when a client disconnects from Blackboard TCPServer
		/// </summary>
		public event TcpClientDisconnectedEventHandler ClientDisconnected;
		/// <summary>
		/// Raises when a Blackboard Module connects to the remote module
		/// </summary>
		public event ModuleConnectionEH Connected;
		/// <summary>
		/// Raises when a Blackboard Module disconnects from the remote module
		/// </summary>
		public event ModuleConnectionEH Disconnected;
		/// <summary>
		/// Raises when this blackboard changes its status
		/// </summary>
		public event BlackboardStatusChangedEH StatusChanged;
		/// <summary>
		/// Raises when a Response for a received command is redirected
		/// </summary>
		public event ResponseRedirectedEH ResponseRedirected;

		#endregion

		#region Methods

		/// <summary>
		/// Asynchronously starts the Blackboard.
		/// If the Blackboard is running, it has no effect.
		/// </summary>
		/// <returns></returns>
		public void BeginStart()
		{
			if (running)
			{
				Log.WriteLine(4, "Blackboard Start: Blackboard is already running");
				return;
			}

			SetupThreads();
			StartThreads();
		}

		/// <summary>
		/// Asynchronously stops the Blackboard.
		/// If the Blackboard is not running, it has no effect.
		/// </summary>
		public void BeginStop()
		{
			if (!running)
			{
				Log.WriteLine(4, "Blackboard Stop: Blackboard is not running");
				return;
			}

			// Begin to Stop threads
			running = false;
			
			// Stop threads
			//StopThreads(false);
		}

		/// <summary>
		/// Starts the Blackboard
		/// </summary>
		/// <returns></returns>
		public void Start()
		{
			if (running)
			{
				Log.WriteLine(8, "Blackboard Start: Blackboard is already running");
				return;
			}

			BeginStart();
			this.mainThreadRunningEvent.WaitOne();
			this.parserThreadRunningEvent.WaitOne();
		}

		/// <summary>
		/// Stops the Blackboard
		/// If the Blackboard is not running, it has no effect.
		/// </summary>
		public void Stop()
		{
			if (!running)
			{
				Log.WriteLine(6, "Blackboard Stop: Blackboard is not running");
				return;
			}
			/*

			// Begin to Stop modules
			foreach (Module module in modules)
			{
				module.BeginStop();
			}
			
			// Begin to Stop threads
			running = false;

			// Stop TCP server
			if (server.Started)
				server.Stop();
			Log.WriteLine("Blackboard server Stopped");
			server = null;

			// Stop threads
			StopThreads(false);
			Log.WriteLine("All Threads Stopped");
			foreach (Module module in modules)
			{
				if(module.IsRunning)module.Stop();
			}

			Log.WriteLine("Blackboard Stopped");
			*/

			BeginStop();
			while (!threadMessageParser.IsAlive || !mainThread.IsAlive) Thread.Sleep(1);
		}

		/// <summary>
		/// Restarts the blackboard.
		/// This action restart the timers
		/// </summary>
		public void Restart()
		{
			if (this.RunningStatus == BlackboardRunningStatus.Running) restartRequested = true;
		}

		/// <summary>
		/// Requests to restart the test.
		/// This action does not restart the timers
		/// </summary>
		public void RestartTest()
		{
			if (this.RunningStatus == BlackboardRunningStatus.Running)
				restartTestRequested = true;
		}

		/// <summary>
		/// Sets the startup time to current time
		/// </summary>
		public void RestartTimer()
		{
			startupTime = DateTime.Now;
		}

		/// <summary>
		/// Injects a message to the blackboard unparsed message queue
		/// </summary>
		/// <param name="textToInject">Message string to inject</param>
		/// <returns>true if injection was successfull, false otherwise</returns>
		public bool Inject(string textToInject)
		{
			textToInject = textToInject.Trim();
			if (textToInject.Length == 0) return false;
			// Parse Text message
			Log.WriteLine(1, "INJECTED <= " + textToInject);
			return ParseMessage(textToInject);
		}

		#region Socket Methods

		/// <summary>
		/// Configures the TcpServer of the Blackboard
		/// </summary>
		private void SetupServer()
		{
			server = new SocketTcpServer(port);
			server.ClientConnected += new TcpClientConnectedEventHandler(server_ClientConnected);
			server.ClientDisconnected += new TcpClientDisconnectedEventHandler(server_ClientDisconnected);
			server.DataReceived += new TcpDataReceivedEventHandler(server_DataReceived);
		}

		#endregion

		#region Module process startup



		#endregion

		#region Blackboard Main Tasks

		/// <summary>
		/// Restarts the test.
		/// This action does not restart the timers
		/// </summary>
		private void AsyncRestartTest()
		{
			bool sentSuccess = false;

			// Change running status
			this.RunningStatus = BlackboardRunningStatus.RestartingTest;
			Log.WriteLine(4, "Restarting the test");
			// Remove all pending commands
			commandsPending.Clear();
			// Send a failed response to all waiting commands
			while (commandsWaiting.Count > 0)
			{
				try
				{
					Response r;
					Command cmd = commandsWaiting[0];
					// Remove command from waiting list
					commandsWaiting.RemoveAt(0);
					Log.WriteLine(4, "\tCommand [" + cmd.ToString() + "] failed due to restart");
					// Prepare generic response
					r = Response.CreateFromCommand(cmd, ResponseFailReason.BlackboardRestartingTest);
					Log.WriteLine(6, "\t\tSending fail response:" + r.ToString() + " to " + r.Destination.Name);
					// Seding failed generic response to source module
					//sentSuccess = cmd.Source.Send(r);
					cmd.SendResponse(r);
					if (!sentSuccess)
					{
						//retryQueue.Enqueue(r);
						//Log.WriteLine("Can't send failure response to " + r.Destination.Name + ". Enqueued");
						Log.WriteLine(6, "\t\tCan't send failure response to " + r.Destination.Name);
					}
					OnResponseRedirected(cmd, r, sentSuccess);
				}
				catch { }
			}
			// Execute RestartTest Actions
			ExecuteRestartTestActions();
			// Clear command list and messages queue
			dataReceived.Clear();
			lock (responses)
			{
				responses.Clear();
			}
			commandsPending.Clear();
			commandsWaiting.Clear();
			// Reset the running status
			this.RunningStatus = BlackboardRunningStatus.Running;
			Log.WriteLine(4, "Test restart Complete.");
		}

		/// <summary>
		/// Restarts the blackboard.
		/// This action will restart the timers
		/// </summary>
		private void AsyncRestart()
		{
			bool sentSuccess;
			// Change running status
			this.RunningStatus = BlackboardRunningStatus.Restarting;
			Log.WriteLine(4, "Restarting blackboard");
			// Remove all pending commands
			commandsPending.Clear();
			// Send a failed response to all waiting commands
			while (commandsWaiting.Count > 0)
			{
				try
				{
					Response r;
					Command cmd = commandsWaiting[0];
					// Remove command from waiting list
					commandsWaiting.RemoveAt(0);
					Log.WriteLine(4, "Command [" + cmd.ToString() + "] failed due to restart");
					// Prepare generic response
					r = Response.CreateFromCommand(cmd, ResponseFailReason.BlackboardRestarting);
					Log.WriteLine(5, "\rSending fail response:" + r.ToString() + " to " + r.Destination.Name);
					// Seding failed generic response to source module
					//sentSuccess = cmd.Source.Send(r);
					sentSuccess = cmd.SendResponse(r);
					if (!sentSuccess)
						Log.WriteLine(6, "\tCan't send failure response to " + r.Destination.Name);
					OnResponseRedirected(cmd, r, sentSuccess);
				}
				catch { }
			}
			// Disconnecting client modules
			Log.WriteLine(4, "\tRestarting modules");
			// Restart modules and execute Restart Actions
			RestartModules();
			// Clear command list and messages queue
			dataReceived.Clear();
			lock(responses)
				responses.Clear();
			commandsPending.Clear();
			commandsWaiting.Clear();
			this.startupTime = DateTime.Now;
			// Reset the running status
			this.RunningStatus = BlackboardRunningStatus.Running;
			Log.WriteLine(4, "Blackboard restart Complete.");
		}

		/// <summary>
		/// Executes all Test Timeout actions.
		/// </summary>
		protected void ExecuteTestTimeOutActions()
		{
			for (int i = 0; i < modules.Count; ++i)
			{
				if (modules[i] is ModuleClient)
					((ModuleClient)Modules[i]).RequestExecuteTestTimeOutActions();
			}
		}

		/// <summary>
		/// Executes all Restart actions.
		/// </summary>
		protected void RestartModules()
		{
			for (int i = 0; i < modules.Count; ++i)
			{
				if (modules[i] is ModuleClient)
					((ModuleClient)Modules[i]).Restart();
			}
		}

		/// <summary>
		/// Executes all Restart actions.
		/// </summary>
		protected void ExecuteRestartTestActions()
		{
			for (int i = 0; i < modules.Count; ++i)
			{
				try
				{
					if (modules[i] is ModuleClient)
						((ModuleClient)Modules[i]).RestartTest();
				}
				catch { }
			}
		}

		/// <summary>
		/// Look for a module in the blackboard that supports specified command
		/// </summary>
		/// <param name="commandName">The name of the command to look for</param>
		/// <param name="destination">When this method returns, contains the Module that supports the command
		/// specified, if the conversion succeeded, or null if no module supports the command.
		/// The search fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if a destination module was found, false otherwise</returns>
		public bool FindDestinationModule(string commandName, out IModule destination)
		{
			IPrototype p;
			return FindDestinationModule(commandName, out destination, out p);
		}

		/// <summary>
		/// Look for a module in the blackboard that supports specified command
		/// </summary>
		/// <param name="commandName">The name of the command to look for</param>
		/// <param name="destination">When this method returns, contains the Module that supports the command
		/// specified, if the conversion succeeded, or null if no module supports the command.
		/// The search fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="prototype">When this method returns, contains the Prototype for the command
		/// specified, if the conversion succeeded, or null if no module supports the command.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if a destination module was found, false otherwise</returns>
		public bool FindDestinationModule(string commandName, out IModule destination, out IPrototype prototype)
		{
			destination = null;
			prototype = null;
			if ((commandName == null) || !Regex.IsMatch(commandName, @"[A-Za-z_]+")) return false;
			if (VirtualModule.SupportCommand(commandName, out prototype))
			{
				destination = VirtualModule;
				return true;
			}
			foreach (ModuleClient m in modules)
			{
				if (!m.Enabled || (m == VirtualModule))
					continue;
				if (m.SupportCommand(commandName, out prototype))
				{
					destination = m;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Finds the perfect match response in a response array. If not found returns null
		/// </summary>
		/// <param name="responses">A Response array to search in</param>
		/// <param name="command">The command for which response to search</param>
		/// <returns>Returns the perfect match Response if found. If not, returns null</returns>
		public Response FindResponse(Command command, Response[] responses)
		{
			return FindResponse(command, responses, false);
		}

		/// <summary>
		/// Finds the first suitable response in a response array. If not found returns null
		/// </summary>
		/// <param name="responses">A Response array to search in</param>
		/// <param name="command">The command for which response to search</param>
		/// <param name="heuristic">Indicates if use a heuristic to find the most adequate response. Set value to false to find only perfect matches</param>
		/// <returns>Returns the first suitable Response if found. If not, returns null</returns>
		public Response FindResponse(Command command, Response[] responses, bool heuristic)
		{
			if (responses.Length == 0) return null;
			// Candidates to be a response for provided command
			List<ResponseCandidate> candidates;
			Response r;
			int affinity = 0;

			candidates = new List<ResponseCandidate>(responses.Length);
			// Search for all suitable candidates
			for (int i = 0; i < responses.Length; ++i)
			{
				// Check if suitable...
				if ((responses[i].Source == command.Destination) && (responses[i].Command == command.Command))
				{
					// If is the most suitable, then return it
					if (command.IsMatch(responses[i]))
						return responses[i];
					if (heuristic && Command.IsMatch(command, responses[i], out affinity))
						candidates.Add(new ResponseCandidate(command, responses[i], affinity));
				}
			}

			// Heuristic: Sort by affinity
			if (!heuristic || (candidates.Count == 0))
				return null;
			candidates.Sort();
			//r = candidates[0].Response;
			r = candidates[0].Response;

			// The selected one is modified. The condition is always true, but its added by security
			if (r.Destination == null) r.Destination = command.Source;
			try
			{
				command.Response = r;
			}
			catch (Exception ex)
			{
				Log.WriteLine(6, "Error while searching for adequate response");
				Log.WriteLine(6, "\tCommand: " + command.ToString());
				Log.WriteLine(6, "\tResponse: " + r.ToString());
				Log.WriteLine(6, "Error: " + ex.ToString());
			}
			if (r.Source is ModuleClient)
				((ModuleClient)r.Source).Unlock(r);
			return r;
		}

		/// <summary>
		/// Retry to send responses which failed in firsts attempts
		/// </summary>
		private void RetrySendResponses()
		{
			Response response;
			int initialCount;

			initialCount = retryQueue.Count;
			for (int i = 0; (i < initialCount) && (retryQueue.Count > 0); ++i)
			{
				response = retryQueue.Dequeue();
				if (response.Send())
				{
					Log.WriteLine(5, "-> Redirection attempt "+ response.SendAttempts.ToString() +" [" + response.Destination.Name + "]: " + response.ToString());
					OnResponseRedirected(response.CommandResponded, response, true);
				}
				else if (response.SendAttempts < SendAttempts)
				{
					Log.WriteLine(6, "Can't send response to " + response.Destination.Name + ". " + sendAttempts.ToString() + " attempts failed");
					OnResponseRedirected(response.CommandResponded, response, false);
				}
				else
				{
					retryQueue.Enqueue(response);
				}
			}
		}

		/// <summary>
		/// Sends all pending messages. It's 1st MainThread task
		/// </summary>
		private void SendPendingMessages()
		{
			//int i;
			Command c;
			Response r;
			string reason = "";

			while (commandsPending.Count > 0)
			{
				// Dequeuing and trying to send command
				c = commandsPending.Consume();

				if (c.Send())
				{
					if (c.Destination == VirtualModule)
						Log.WriteLine(8, "-> [" + c.Destination.Name + "]: " + c.ToString());
					else
						Log.WriteLine(1, "-> [" + c.Destination.Name + "]: " + c.ToString());
					if (c.Prototype.ResponseRequired && !commandsWaiting.Contains(c))
					{
						// Command has been sent and needs response, can be added to waiting list
						commandsWaiting.Add(c);
					}
					Log.WriteLine(7, "Waiting response from " + c.Destination.Name);
				}
				else
				{
					reason = "";
					if (!c.Destination.IsRunning)
						reason = ". Module client is not running";
					else if (!c.Destination.IsConnected)
						reason = ". Not connected";
					else if (c.Destination.Busy)
						reason = ". Module is busy";
					else if (!c.Destination.Ready)
						reason = ". Module is not responding? (not ready)";
					else if (!c.Destination.IsAlive)
						reason = ". Module is not responding? (not alive)";
					Log.WriteLine(1, "Can't send to " + c.Destination.Name + ": " + c.ToString() + " [Send failed " + reason+ " ]");
					// Command sent failed. Generating generic response
					r = Response.CreateFromCommand(c, ResponseFailReason.Unknown);
					Log.WriteLine(5, "Generated fail response:" + r.ToString());
					// Seding failed generic response to source module
					if (c.SendResponse(r))
					{
						Log.WriteLine(1, "-> [" + r.Destination.Name + "]: " + r.ToString());
						OnResponseRedirected(c, r, true);
					}
					else
					{
						if(sendAttempts > 1)
							retryQueue.Enqueue(r);
						reason = "";
						if (!r.Destination.IsRunning)
							reason = ". Module client is not running";
						else if (!r.Destination.IsConnected)
							reason = ". Not connected";
						else if (r.Destination.Busy)
							reason = ". Module is busy";
						else if (!r.Destination.Ready)
							reason = ". Module is not responding? (not ready)";
						else if (!r.Destination.IsAlive)
							reason = ". Module is not responding? (not alive)";
						Log.WriteLine(6, "Can't send failure response to " + r.Destination.Name + reason + (sendAttempts>1 ? ". Enqueued" : ""));
						OnResponseRedirected(c, r, false);
					}
				}
			}
		}

		/// <summary>
		/// Finds and sends the best response for each waiting command
		/// </summary>
		private void RedirectResponses()
		{
			int i;
			Response[] aResponses;
			Queue<Command> commandsToRespond;
			//Dictionary<Command, Response> sendPairs;
			List<Response> responsesToSend;
			Response response;
			Command command;

			
			// Fill a queue with commands to respond
			commandsToRespond = new Queue<Command>(commandsWaiting);
			//sendPairs = new Dictionary<Command, Response>(commandsToRespond.Count);
			responsesToSend = new List<Response>(commandsToRespond.Count);

			// Find a Perfect Match for commands
			#region Find a Perfect Match for commands
			i = 0;
			while (i < commandsToRespond.Count)
			{
				lock (responses)
				{
					aResponses = responses.ToArray();
				}
				if (aResponses.Length < 1)
					break;
				command = commandsToRespond.Dequeue();
				response = FindResponse(command, aResponses);
				// If a perfect match is not found, the command is enqueued (again) to continue with next.
				// Ir a match is found, is added to send list
				if (response == null)
				{
					commandsToRespond.Enqueue(command);
					++i;
				}
				else
				{
					// Register response
					command.Response = response;
					// Add pair to send list
					//sendPairs.Add(command, response);
					responsesToSend.Add(response);
					// Perfect match response is removed from response list
					lock (responses)
					{
						responses.Remove(response);
					}
					// Command is removed from waiting list
					commandsWaiting.Remove(command);
					Log.WriteLine(7, "Response for [" + command.ToString() + "] found in [" + response.ToString() + "]");
				}
			}
			#endregion

			// Find a Heuristic Best Match for commands
			#region Find a Heuristic Best Match for commands
			while (commandsToRespond.Count > 0)
			{
				lock (responses)
				{
					aResponses = responses.ToArray();
				}
				if (aResponses.Length < 1)
					break;
				command = commandsToRespond.Dequeue();
				response = FindResponse(command, aResponses, true);
				// Ir a match is found, is added to send list
				if (response != null)
				{
					// Add pair to send list
					//sendPairs.Add(command, response);

					// Asign response to command
					command.Response = response;
					// Add response to send list
					responsesToSend.Add(response);
					// Heuristic match response is removed from response list
					lock (responses)
					{
						responses.Remove(response);
					}
					// Command is removed from waiting list
					commandsWaiting.Remove(command);
					Log.WriteLine(5, "Response for [" + command.ToString() + "] found in [" + response.ToString() + "] using heuristic");
				}
			}
			#endregion

			// Send all responses
			#region Send all responses
			for (i = 0; i < responsesToSend.Count; ++i)
			{
				response = responsesToSend[i];
				if (response.Send())
				{
					Log.WriteLine(1, "-> [" + response.Destination.Name + "]: " + response.ToString());
					OnResponseRedirected(response.CommandResponded, response, true);
				}
				else
				{
					Log.WriteLine(5, "Can't send response to " + response.Destination.Name);
					if (sendAttempts == 1)
					{
						Log.WriteLine(7, "Deleted both: command and response from Blackboard Queue");
						OnResponseRedirected(response.CommandResponded, response, false);
					}
					else retryQueue.Enqueue(response);
				}
			}
			#endregion
		}

		/// <summary>
		/// Sends a failure Response if destination module is not connected
		/// </summary>
		private void SendDisconnectedResponses()
		{
			int i;
			Response rsp;
			Command cmd;
			Command[] tmpCommandsWaiting;
			string reason;

			lock (commandsWaiting)
			{
				tmpCommandsWaiting = commandsWaiting.ToArray();
			}

			for (i = 0; i < tmpCommandsWaiting.Length; ++i)
			{
				cmd = tmpCommandsWaiting[i];
				if (!cmd.Destination.IsConnected)
				{
					Log.WriteLine(5, "Waiting response for " + cmd.ToString() + " failed: Destination module disconnected");
					// Prepare generic response
					rsp = Response.CreateFromCommand(cmd, ResponseFailReason.ModuleDisconnected);
					Log.WriteLine(6, "Generated fail response:" + rsp.ToString());
					// Seding failed generic response to source module
					//if (cmd.Source.Send(r))
					if (cmd.SendResponse(rsp))
					{
						Log.WriteLine(1, "-> [" + rsp.Destination.Name + "]: " + rsp.ToString());
						OnResponseRedirected(cmd, rsp, true);
					}
					else
					{
						if (sendAttempts > 1)
							retryQueue.Enqueue(rsp);
						reason = "";
						if (!rsp.Destination.IsRunning)
							reason = ". Module client is not running";
						else if (!rsp.Destination.IsConnected)
							reason = ". Not connected";
						else if (rsp.Destination.Busy)
							reason = ". Module is busy";
						else if (!rsp.Destination.Ready)
							reason = ". Module is not responding? (not ready)";
						else if (!rsp.Destination.IsAlive)
							reason = ". Module is not responding? (not alive)";
						Log.WriteLine(6, "Can't send failure response to " + rsp.Destination.Name + reason + (sendAttempts > 1 ? ". Enqueued" : ""));
						OnResponseRedirected(cmd, rsp, false);
					}
					// Remove command from waiting list
					lock (commandsWaiting)
					{
						if(commandsWaiting.Contains(cmd))
						commandsWaiting.Remove(cmd);
					}
				}
			}
		}

		/// <summary>
		/// Sends a failure Response for timed out commands
		/// </summary>
		private void SendTimedOutResponses()
		{
			int i;
			Response rsp;
			Command cmd;
			Command[] tmpCommandsWaiting;
			string reason;

			lock (commandsWaiting)
			{
				tmpCommandsWaiting = commandsWaiting.ToArray();
			}

			for (i = 0; i < tmpCommandsWaiting.Length; ++i)
			{
				cmd = tmpCommandsWaiting[i];
				// No response has arrived Check for timeout
				if (cmd.MillisecondsLeft <= 0)
				{
					Log.WriteLine(1, "Command for " + cmd.Destination.Name + " timed out: " + cmd.ToString());
					// Prepare generic response
					rsp = Response.CreateFromCommand(cmd, ResponseFailReason.TimedOut);
					Log.WriteLine(6, "Generated fail response:" + rsp.ToString());
					// Seding failed generic response to source module
					//if (cmd.Source.Send(r))
					if (cmd.SendResponse(rsp))
					{
						Log.WriteLine(1, "-> [" + rsp.Destination.Name + "]: " + rsp.ToString());
						OnResponseRedirected(cmd, rsp, true);
					}
					else
					{
						if (sendAttempts > 1)
							retryQueue.Enqueue(rsp);
						reason = "";
						if (!rsp.Destination.IsRunning)
							reason = ". Module client is not running";
						else if (!rsp.Destination.IsConnected)
							reason = ". Not connected";
						else if (rsp.Destination.Busy)
							reason = ". Module is busy";
						else if (!rsp.Destination.Ready)
							reason = ". Module is not responding? (not ready)";
						else if (!rsp.Destination.IsAlive)
							reason = ". Module is not responding? (not alive)";
						Log.WriteLine(6, "Can't send failure response to " + rsp.Destination.Name + reason + (sendAttempts > 1 ? ". Enqueued" : ""));
						OnResponseRedirected(cmd, rsp, false);
					}
					// Remove command from waiting list
					lock (commandsWaiting)
					{
						commandsWaiting.Remove(cmd);
					}
				}
			}
		}

		#endregion

		#region Parser Methods

		/// <summary>
		/// Parces a received or injected mensaje for further redirection
		/// </summary>
		/// <param name="message">Message string to parse</param>
		/// <returns>true if the message was parsed, false otherwise</returns>
		private bool ParseMessage(string message)
		{
			Command command = null;
			Response response = null;

			Log.WriteLine(8, "Parsing: " + message);
			// Discard unsuitable messages
			if (!Response.TryParse(message, this, out response) && !Command.TryParse(message, this, out command))
			{
				Log.WriteLine(8, "Rejected: " + message);
				return false;
			}

			// Response found
			if (response != null)
			{
				Log.WriteLine(8, "Parsed [" + message + "] as response");
				lock (responses)
				{
					responses.Add(response);
				}
				//if ((response.Source != null) && (response.Source is Module))
				//	((Module)response.Source).IsAlive = true;
				return true;
			}
			// Command found
			else if (command != null)
			{
				commandsPending.Produce(command);
				Log.WriteLine(8, "Parsed [" + message + "] as command");
				//if ((command.Source != null) && (command.Source is Module))
				//	((Module)command.Source).IsAlive = true;
				return true;
			}
			
			
			return false;
		}

		#endregion

		#region Thread Control Methods

		/// <summary>
		/// Configures the threads
		/// </summary>
		private void SetupThreads()
		{
			this.mainThread = new Thread(new ThreadStart(MainThreadTask));
			this.threadMessageParser = new Thread(new ThreadStart(ThreadMessageParser));
			this.mainThread.IsBackground = true;
			this.threadMessageParser.IsBackground = true;
			this.mainThreadRunningEvent.Reset();
			this.parserThreadRunningEvent.Reset();
		}

		/// <summary>
		/// Starts threads
		/// </summary>
		private void StartThreads()
		{
			running = true;
			mainThread.Start();
			//this.mainThreadRunningEvent.WaitOne();
			threadMessageParser.Start();
		}

		/// <summary>
		/// Stops threads
		/// </summary>
		private void StopThreads(bool abort)
		{
			if (abort)
			{
				threadMessageParser.Abort();
				mainThread.Abort();
				
			}
			threadMessageParser.Join();
			mainThread.Join();
			
		}
		
		/// <summary>
		/// Main thread for redirecting messages
		/// </summary>
		private void MainThreadTask()
		{
			int i;
			int retrySendCount = 0;
			List<ModuleClient> modulesPendingToStop;
			IAsyncResult stopPluginsAsyncResult = null;
			//Response[] aResponses;
			//Command c;
			//Response r;

			running = true;
			RunningStatus = BlackboardRunningStatus.Starting;
			Log.WriteLine(4, "Main Thread: Running");

			#region Start Modules
			if(!VirtualModule.IsRunning)
				VirtualModule.Start();
			while (!VirtualModule.IsRunning)
				Thread.Sleep(1);
			if (running)
			{
				foreach (ModuleClient module in modules)
				{
					if (!module.Enabled)
						continue;
					if (!module.IsRunning)
					{
						module.Start();
						Thread.Sleep(this.ModuleLoadDelay);
					}
				}
			}

			#endregion

			#region Start Plugins

			if (this.pluginManager.Count > 0)
			{
				Log.WriteLine(4, "Initializing plugins");
				pluginManager.InitializePlugins();
				Log.WriteLine(4, "Starting plugins");
				pluginManager.BeginStartPlugins(null, null);
			}

			#endregion

			// Start Server and clear queues
			#region Server Start

			if (running)
			{
				if ((server != null) && (server.Started))
					server.Stop();
				dataReceived.Clear();
				commandsPending.Clear();
				commandsWaiting.Clear();
				SetupServer();
				try
				{
					server.Start();
				}
				catch
				{
					Log.WriteLine(1, "Can not start Tcp Server");
					running = false;
					foreach (ModuleClient module in modules)
						if (!module.IsRunning) module.BeginStop();
				}
			}

			#endregion

			startupTime = DateTime.Now;
			testTimeOutExecuted = false;
			Log.WriteLine(1, "Blackboard Started");
			if(running)RunningStatus = BlackboardRunningStatus.Running;
            this.mainThreadRunningEvent.Set();
            this.parserThreadRunningEvent.WaitOne();
			this.StartupSequence.StartModules();

			while (running)
			{
				#region Redirector

				try
				{
					#region Send all pending messages

					// Send all pending messages
					SendPendingMessages();

					#endregion

					#region Remove unnecesary responses
					// Remove unnecesary responses
					lock (commandsWaiting)
					{
						lock (responses)
						{
							if ((commandsWaiting.Count == 0) && (responses.Count > 0))
								responses.Clear();
						}
					}
					#endregion

					#region Response redirection


					if (commandsWaiting.Count > 0)
					{

						#region Send failed response on module disconnection

						SendDisconnectedResponses();

						#endregion

						#region Redirect Responses

						RedirectResponses();

						#endregion

						#region Send failed response on Timeout

						SendTimedOutResponses();

						#endregion

						#region Retry Send

						if (retrySendCount++ > 10)
						{
							RetrySendResponses();
							retrySendCount = 0;
						}

						#endregion

					}

					#endregion

				}
				catch (Exception ex) { Log.Write(1, ex.ToString()); }

				#endregion

				#region Request Handle

				if (restartRequested)
				{
					restartRequested = false;
					restartTestRequested = false;
					AsyncRestart();
				}

				if (restartTestRequested)
				{
					restartTestRequested = false;
					AsyncRestartTest();
				}

				#endregion

				#region Event firing

				if ((autoStopTime > TimeSpan.Zero) && (AutoStopTimeLeft <= TimeSpan.Zero))
				{
					Log.WriteLine(1, "Auto-Stop timed out");
					running = false;
				}

				if (!testTimeOutExecuted && (testTimeOut > TimeSpan.Zero) && (TestTimeLeft <= TimeSpan.Zero))
				{
					testTimeOutExecuted = true;
					Log.WriteLine(4, "Test timed out: Executing Test-Timeout actions");
					ExecuteTestTimeOutActions();
					Log.WriteLine(5, "\tTest-Timeout actions executed");
				}

				#endregion

				Thread.Sleep(1);
			}

			RunningStatus = BlackboardRunningStatus.Stopping;

			#region Stop secondary threads

			// Stop secondary threads
			Log.WriteLine(4, "Stopping threads...");
			this.threadMessageParser.Join(100);
			if ((this.threadMessageParser != null) && this.threadMessageParser.IsAlive)
				this.threadMessageParser.Abort();
			Log.WriteLine(4, "Done!");

			#endregion

			#region Begin to Stop Modules

			// Begin to Stop modules
			Log.WriteLine(1, "Stopping modules...");
			modulesPendingToStop = new List<ModuleClient>();
			foreach (ModuleClient module in modules)
			{
				modulesPendingToStop.Add(module);
				module.BeginStop();
			}
			this.ShutdownSequence.ShutdownModules();

			#endregion

			#region Begin to stop Plugins

			if (this.pluginManager.Count > 0)
			{
				Log.WriteLine(4, "Stopping plugins");
				stopPluginsAsyncResult = this.pluginManager.BeginStopPlugins(null, null);
			}

			#endregion

			#region Stop TCP server

			// Stop TCP server
			if (server.Started)
				server.Stop();
			Log.WriteLine(1, "Blackboard server Stopped");
			server = null;

			#endregion

			#region Wait for Modules to Stop
			Log.WriteLine(4, "Waiting for modules to stop: " + modulesPendingToStop.Count.ToString() + " pending");
			do
			{
				// Waiting for modules to stop.
				// Log.WriteLine(9, "Waiting for modules to stop: " + modulesPendingToStop.Count.ToString() + " pending");
				for (i = 0; i < modulesPendingToStop.Count; ++i )
				{
					//if (!module.IsRunning || (module.Simulation != SimulationOptions.SimulationDisabled))
					if (!modulesPendingToStop[i].IsRunning)
					{
						modulesPendingToStop.RemoveAt(i);
						--i;
					}

				}
				Thread.Sleep(10);
			} while (modulesPendingToStop.Count > 0);
			
			/*
			foreach (Module module in modules)
			{
				if (!module.IsRunning || (module.Simulation != SimulationOptions.SimulationDisabled))
					module.Stop();
			}
			*/

			#endregion

			#region Wait for Plugins to stop

			if ((this.pluginManager.Count > 0) && (stopPluginsAsyncResult != null))
			{
				this.pluginManager.EndStopPlugins(stopPluginsAsyncResult);
				Log.WriteLine(4, "Plugins stopped");
			}

			#endregion

			this.mainThreadRunningEvent.Reset();
			Log.WriteLine(4, "Main Thread: Stopped");
			RunningStatus = BlackboardRunningStatus.Stopped;
			Log.WriteLine(1, "Blackboard Stopped");
			Log.Flush();
		}

		/// <summary>
		/// Thread for parse incomming messges
		/// </summary>
		private void ThreadMessageParser()
		{
			TcpPacket packet;
			string message;
			int i;

			this.parserThreadRunningEvent.Set();
			Log.WriteLine(4, "Message Parser Thread: Running");
			while (running)
			{
				try
				{
					packet = dataReceived.Consume();
					if (packet == null)
						continue;
					// Discard if not a text message
					if (!packet.IsAnsi)
						continue;
					for (i = 0; i < packet.DataStrings.Length; ++i)
					{
						// Read the TEXT message
						message = packet.DataStrings[i].Trim();
						// Parse Text message
						ParseMessage(message);
					}
				}
				catch (ThreadInterruptedException tiex)
				{
					Log.WriteLine(8, "Message Parser Thread Interrupted: " + tiex.ToString());
					continue;
				}
				catch (ThreadAbortException taex)
				{
					Log.WriteLine(8, "Message Parser Thread Aborted: " + taex.ToString());
					return;
				}
				catch (Exception ex)
				{
					Log.WriteLine(1, ex.ToString());
					Thread.Sleep(10);
				}
			}
			Log.WriteLine(4, "Message Parser Thread: Stopped");
		}

		#endregion

		#endregion

		#region Event Handler Functions

		#region ModuleCollection Event Handler Functions

		/// <summary>
		/// Configures the Module object added to the ModuleCollection of the Blackboard object
		/// </summary>
		/// <param name="module">Module to configure</param>
		private void modules_ModuleAdded(IModule module)
		{
			if (module is ModuleClient)
			{
				((ModuleClient)module).Connected += new ModuleConnectionEH(module_Connected);
				((ModuleClient)module).Disconnected += new ModuleConnectionEH(module_Disconnected);
				((ModuleClient)module).ActionExecuted += new ActionExecutedEH(module_ActionExecuted);
			}
			module.CommandReceived += new CommandReceivedEH(module_CommandReceived);
			module.ResponseReceived += new ResponseReceivedEH(module_ResponseReceived);
			module.Started += new StatusChangedEH(module_Started);
			module.Stopped += new StatusChangedEH(module_Stopped);
			Log.WriteLine(1, "Added module: " + module.Name);
		}

		/// <summary>
		/// Configures the Module object removed the ModuleCollection of the Blackboard object
		/// </summary>
		/// <param name="module">Module to configure</param>
		private void modules_ModuleRemoved(IModule module)
		{
			if (module is ModuleClient)
			{
				((ModuleClient)module).Connected -= new ModuleConnectionEH(module_Connected);
				((ModuleClient)module).Disconnected -= new ModuleConnectionEH(module_Disconnected);
				((ModuleClient)module).ActionExecuted -= new ActionExecutedEH(module_ActionExecuted);
			}
			module.CommandReceived -= new CommandReceivedEH(module_CommandReceived);
			module.ResponseReceived -= new ResponseReceivedEH(module_ResponseReceived);
			Log.WriteLine(1, "Removed module: " + module.Name);
		}

		#endregion

		#region Module Event Handler Functions

		private void module_ActionExecuted(ModuleClient m, IAction action, bool success)
		{
			Log.WriteLine(5, m.Name + ": " + action.ToString() + (success ? " Success!!!" : " failed."));
		}

		private void module_CommandReceived(IModule sender, ITextCommand c)
		{
			Command cmd = c as Command;
			if (c == null)
				return;
			// A command has been received so it is stored
			commandsPending.Produce(cmd);
			if (sender == virtualModule)
				Log.WriteLine(6, "<- [" + c.Source.Name + "]: " + c.ToString());
			else
				Log.WriteLine(5, "<- [" + c.Source.Name + "]: " + c.ToString());
		}

		private void module_Connected(ModuleClient m)
		{
			if (Connected != null) Connected(m);
			Log.WriteLine(4, "Connected to " + m.Name);
		}

		private void module_Disconnected(ModuleClient m)
		{
			if (Disconnected != null) Disconnected(m);
			Log.WriteLine(4, "Disconnected from " + m.Name);
		}

		private void module_ResponseReceived(IModule sender, ITextResponse r)
		{
			Response rsp = r as Response;
			if (rsp == null)
				return;
			// A response has been received so it is stored
			lock (responses)
			{
				responses.Add(rsp);
			}
			if(sender == virtualModule)
				Log.WriteLine(6, "<- [" + r.Source.Name + "]: " + r.ToString());
			else
				Log.WriteLine(5, "<- [" + r.Source.Name + "]: " + r.ToString());
		}

		private void module_Started(IModule m)
		{
			Log.WriteLine(4, "Started module [" + m.Name + "]");
		}

		private void module_Stopped(IModule m)
		{
			Log.WriteLine(4, "Stopped module [" + m.Name + "]");
		}

		#endregion

		/// <summary>
		/// Handles the received data trough blackboard Tcp Socket Server
		/// </summary>
		/// <param name="p">Tcp Packet received</param>
		private void server_DataReceived(TcpPacket p)
		{
			if (p.IsAnsi)
			{
				dataReceived.Produce(p);
				for(int i = 0; i < p.DataStrings.Length; ++i)
					Log.WriteLine(5, "SERVER <= " + p.DataStrings[i]);
				Log.WriteLine(6, dataReceived.Count.ToString() + " messages pending to parse");
			}
		}

		/// <summary>
		/// Handles the ClientConnected event when a Tcp Client connects to the blackboard
		/// </summary>
		/// <param name="s">Connection Socket</param>
		private void server_ClientConnected(Socket s)
		{
			if (ClientConnected != null) ClientConnected(s);
			try
			{
				Log.WriteLine(4, "Client " + ((IPEndPoint)s.RemoteEndPoint).ToString() + " connected to Blackboard server ");
			}
			catch { }
		}

		/// <summary>
		/// Handles the ClientDisconnected event when a Tcp Client disconnects from the blackboard
		/// </summary>
		/// <param name="ep">Disconnection endpoint</param>
		private void server_ClientDisconnected(EndPoint ep)
		{
			if (ClientDisconnected != null) ClientDisconnected(ep);
			try
			{
				Log.WriteLine(5, "Client " + (ep.ToString() + " disconnected from Blackboard server "));
			}
			catch { }
		}

		/// <summary>
		/// Raises the ResponseRedirected event
		/// </summary>
		/// <param name="command">Command tried to be executed</param>
		/// <param name="response">Response to command redirected</param>
		/// <param name="sendResponseSuccess">Indicates if the response was sent successfully</param>
		protected virtual void OnResponseRedirected(Command command, Response response, bool sendResponseSuccess)
		{
			if ((ResponseRedirected != null) && (command != null) && (command.Source != VirtualModule) && (command.Destination != VirtualModule))
				ResponseRedirected(command, response, sendResponseSuccess);
		}

		#endregion

		#region Static Properties

		#endregion

		#region Methods de Clase (Estáticos)

		/// <summary>
		/// Creates a blackboard from a XML configuration file
		/// </summary>
		/// <param name="path">The path of the XML configuration file</param>
		/// <returns>The configured blackboard</returns>
		public static Blackboard FromXML(string path)
		{
			LogWriter w = new LogWriter();
			return FromXML(path, w);
		}

		/// <summary>
		/// Creates a blackboard from a XML configuration file
		/// </summary>
		/// <param name="path">The path of the XML configuration file</param>
		/// <param name="log">Output Log</param>
		/// <returns>The configured blackboard</returns>
		public static Blackboard FromXML(string path, ILogWriter log)
		{
			#region Variables

			Blackboard blackboard;
			XmlDocument doc;
			XmlDocument tmpDoc;
			XmlNodeList modules;
			XmlNodeList commands;
			XmlNode node;
			int i, j;//, cmdCount;
			int clientModulesAddedd;

			string moduleName;
			bool moduleEnabled;
			string processName;
			string programPath;
			string programArgs;
			string moduleAlias;
			string moduleAuthor;
			bool aliveCheck;
			string cmdName;
			bool cmdParams;
			bool cmdAnswer;
			bool cmdPriority;
			int cmdTimeOut;
			bool requirePrefix;
			int sendDelay;
			bool simulate;
			double simulationSuccessRatio;
			ModuleSimulationOptions simOptions;
			//Command startupCommand;o
			Prototype proto;
			List<Prototype> protoList;
			ModuleClient mod;
			List<IPAddress> ips;
			SortedList<string, ModuleClient> disabledModules;

			int port;
			int checkInterval;

			#endregion

			blackboard = new Blackboard();
			//if ((Log != null) && (Log != TextWriter.Null)) blackboard.Log = Log;
			if (log != null) blackboard.Log = log;
			else log = blackboard.Log;
			log.VerbosityTreshold = blackboard.verbosity;
			if (!File.Exists(path))
			{
				log.WriteLine(0, "File does not exist");
				throw new FileLoadException("File does not exist");
			}
			doc = new XmlDocument();
			doc.Load(path);

			if (
				(doc.GetElementsByTagName("blackboard").Count != 1) ||
				(doc.GetElementsByTagName("configuration").Count != 1) ||
				(doc.GetElementsByTagName("modules").Count < 1))
			{
				log.WriteLine(0, "Incorrect format");
				throw new FileLoadException("Incorrect format");
			}

			#region Load Blackboard Configuration

			LoadBlackboardConfiguration(blackboard, doc, log);

			#endregion

			#region Load Blackboard Modules

			if (doc.GetElementsByTagName("modules").Count < 1)
			{
				log.WriteLine(0, "No modules to load");
				throw new Exception("No modules to load");
			}
			modules = doc.GetElementsByTagName("modules")[0].ChildNodes;

			#endregion

			#region Module extraction
			tmpDoc = new XmlDocument();
			clientModulesAddedd = 0;
			disabledModules = new SortedList<string, ModuleClient>();
			for (i = 0; i < modules.Count; ++i)
			{
				try
				{
					// Module check
					if ((modules[i].Name != "module") ||
						(modules[i].Attributes.Count < 1) ||
						(modules[i].Attributes["name"] == null) ||
						(modules[i].Attributes["name"].Value.Length < 1))
						continue;

					#region Module information extraction
					// Name
					moduleName = modules[i].Attributes["name"].Value.ToUpper();

					// Enabled
					//moduleEnabled = true;
					//if((modules[i].Attributes["enabled"] != null) &&
					//    Boolean.TryParse(modules[i].Attributes["enabled"].Value, out moduleEnabled) &&
					//    !moduleEnabled)
					//    continue;
					if ((modules[i].Attributes["enabled"] == null) ||
						!Boolean.TryParse(modules[i].Attributes["enabled"].Value, out moduleEnabled))
						moduleEnabled = true;

					// Alias
					if(modules[i].Attributes["alias"] != null)
						moduleAlias = modules[i].Attributes["alias"].Value;
					else moduleAlias = moduleName;

					// Author
					if (modules[i].Attributes["author"] != null)
						moduleAuthor = modules[i].Attributes["author"].Value;
					else
						moduleAuthor = null;

					log.WriteLine(1, "Loading module " + moduleName + (moduleAlias != moduleName ? " alias " + moduleAlias : ""));
					// Create a XML sub-document XML
					tmpDoc.LoadXml(modules[i].OuterXml);

					#region Get program path and program arguments

					FetchProgramInfo(tmpDoc, out processName, out programPath, out programArgs);

					#endregion

					// Leo el comando de inicio
					//if (tmpDoc.GetElementsByTagName("startupCommand").Count != 0)
					//	startupMessage = Command.Parse(tmpDoc.GetElementsByTagName("startupCommand")[0].InnerText);

					// Get the array of ip addresses where the module can be
					ips = FetchIpAddresses(tmpDoc);
					if((ips == null) || (ips.Count < 1))
					{
						log.WriteLine(2, "\tNo valid IP Address provided");
						log.WriteLine(1, "Module skipped");
						continue;
					}

					// Leo el puerto de conexion del modulo
					if (
						(tmpDoc.GetElementsByTagName("port").Count == 0) ||
						!Int32.TryParse(tmpDoc.GetElementsByTagName("port")[0].InnerText, out port) ||
						(port <= 1024))
					{
						log.WriteLine(2, "\tInvalid port");
						log.WriteLine(1, "Module skipped");
						continue;
					}
					log.WriteLine("\t" + ips[0].ToString() + ":" + port);

					// Veify if Blackbard must check Module's alive status
					checkInterval = ModuleClient.GlobalCheckInterval;
					if(tmpDoc.GetElementsByTagName("aliveCheck").Count != 0)
					{
						node=tmpDoc.GetElementsByTagName("aliveCheck")[0];
						if(!Boolean.TryParse(node.InnerText, out aliveCheck))
							aliveCheck = true;
						// Read alive/busy/ready check interval
						if ((node.Attributes["interval"] == null) ||
							!Int32.TryParse(node.Attributes["interval"].InnerText, out checkInterval) ||
							(checkInterval < ModuleClient.MinCheckInterval) ||
							(checkInterval > ModuleClient.MaxCheckInterval))
							checkInterval = ModuleClient.GlobalCheckInterval;

					}
					else aliveCheck = true;
					// Verify if the Module requires SOURCE DESTINATION prefix
					if (
						(tmpDoc.GetElementsByTagName("requirePrefix").Count == 0) ||
						!Boolean.TryParse(tmpDoc.GetElementsByTagName("requirePrefix")[0].InnerText, out requirePrefix)
						) requirePrefix = false;
					// Delay between send operations
					if (
						(tmpDoc.GetElementsByTagName("sendDelay").Count == 0) ||
						!Int32.TryParse(tmpDoc.GetElementsByTagName("sendDelay")[0].InnerText, out sendDelay)
						) sendDelay = -1;
					// Simulation options
					simOptions = ModuleSimulationOptions.SimulationDisabled;
					if(tmpDoc.GetElementsByTagName("simulate").Count != 0)
					{
						node =tmpDoc.GetElementsByTagName("simulate")[0];
						simulationSuccessRatio = -1;
						if ((node.Attributes["successRatio"] == null) || !Double.TryParse(node.Attributes["successRatio"].InnerText, out simulationSuccessRatio) || (simulationSuccessRatio < 0) || (simulationSuccessRatio > 1))
							simulationSuccessRatio = -1;
						if(Boolean.TryParse(node.InnerText, out simulate))
							simOptions = (simulationSuccessRatio != -1) ?
								new ModuleSimulationOptions(simulationSuccessRatio):
								new ModuleSimulationOptions(simulate);						
					}
					

					#endregion

					#region Module Validation
					// Module validation.
					// If a module with the same name exists, rename the current module and disable it
					if (blackboard.Modules.Contains(moduleName) || disabledModules.ContainsKey(moduleName))
					{
						int n = 1;
						string newModuleName = moduleName + n.ToString().PadLeft(2, '0');
						
						while (blackboard.Modules.Contains(newModuleName) || disabledModules.ContainsKey(newModuleName))
						{
							++n;
							newModuleName = moduleName + n.ToString().PadLeft(2, '0');
						}
						log.WriteLine(1, "Module " + moduleName + " already exists. Renamed to " + newModuleName + (moduleAlias != moduleName ? " alias " + moduleAlias : ""));
						moduleName = newModuleName;
						moduleEnabled = false;
					}

					#endregion

					// Module Creation
					mod = new ModuleClientTcp(moduleName, ips, port);
					mod.Enabled = moduleEnabled;
					mod.Author = moduleAuthor;
					mod.ProcessInfo.ProcessName = processName;
					mod.ProcessInfo.ProgramPath = programPath;
					mod.ProcessInfo.ProgramArgs = programArgs;
					mod.AliveCheck = aliveCheck;
					mod.RequirePrefix = requirePrefix;
					mod.SendDelay = sendDelay;
					mod.Simulation = simOptions;
					mod.Alias = moduleAlias;
					mod.CheckInterval = checkInterval;

					#region Actions Extraction

					// Startup actions extraction
					LoadModuleActions("onStart", "Startup", tmpDoc, mod.StartupActions, log);
					// Restart actions extraction
					LoadModuleActions("onRestart", "Restart", tmpDoc, mod.RestartActions, log);
					// Restart Test actions extraction
					LoadModuleActions("onRestartTest", "Restart-Test", tmpDoc, mod.RestartTestActions, log);
					// Stop actons extraction
					LoadModuleActions("onStop", "Stop", tmpDoc, mod.StopActions, log);
					// Test Timeout actons extraction
					LoadModuleActions("onTestTimeOut", "Test Timeout", tmpDoc, mod.TestTimeOutActions, log);

					#endregion

					#region Module Commands Extraction

					// Leo lista de comandos.
					log.WriteLine(2, "\tLoading module commands...");
					if (doc.GetElementsByTagName("commands").Count < 1)
					{
						log.WriteLine(3, "\tNo commands to load");
						log.WriteLine(2, "\tModule skipped");
						continue;
					}
					commands = tmpDoc.GetElementsByTagName("commands")[0].ChildNodes;
					protoList = new List<Prototype>(commands.Count);

					#region Extraccion de Comandos de modulo

					for (j = 0; j < commands.Count; ++j)
					{
						// Verifico que sea un comando
						cmdTimeOut = 0;
						if ((commands[j].Name == "command") &&
						(commands[j].Attributes.Count >= 3) &&
						(commands[j].Attributes["name"].Value.Length > 1) &&
						Boolean.TryParse(commands[j].Attributes["answer"].Value, out cmdAnswer) &&
						(
							(cmdAnswer && Int32.TryParse(commands[j].Attributes["timeout"].Value, out cmdTimeOut) && (cmdTimeOut >= 0)) || !cmdAnswer
						))
						{
							// Leo nombre de comando
							cmdName = commands[j].Attributes["name"].Value;
							log.WriteLine(2, "\t\tAdded command " + cmdName);
							// Verifico si requiere parametros
							if ((commands[j].Attributes["parameters"] == null) || !Boolean.TryParse(commands[j].Attributes["parameters"].Value, out cmdParams))
								cmdParams = true;
							// Verifico si tiene prioridad
							if ((commands[j].Attributes["priority"] == null) || !Boolean.TryParse(commands[j].Attributes["priority"].Value, out cmdPriority))
								cmdPriority = false;
							// Creo el prototipo
							proto = new Prototype(cmdName, cmdParams, cmdAnswer, cmdTimeOut, cmdPriority);
							// Agrego el prototipo al modulo
							mod.Prototypes.Add(proto);
							protoList.Add(proto);
						}
						else log.WriteLine(4, "\t\tInvalid Command ");
					}
					#endregion
					// Si no hay comandos soportados por el modulo, salto el modulo
					if (protoList.Count < 1)
					{
						log.WriteLine(3, "\tAll commands rejected.");
						log.WriteLine(2, "Module skipped");
						continue;
					}

					#endregion

					// Add module to blackboard
					#region Add module to Blackboard

					if (mod.Enabled)
					{
						blackboard.modules.Add(mod);
						++clientModulesAddedd;
						log.WriteLine(2, "Loading module complete!");
					}
					else
					{
						disabledModules.Add(mod.Name, mod);
						log.WriteLine(2, "Disabled module enqueued!");
					}

					#endregion
				}
				catch
				{
					// Error al cargar el modulo
					log.WriteLine(2, "Invalid module");
					// Continuo con el siguiente
					continue;
				}
			}

			#region Add disabled modules

			log.WriteLine(2, "Adding disabled modules");
			for (i = 0; i < disabledModules.Count; ++i)
			{
				try
				{
					mod = disabledModules.Values[i];
					blackboard.Modules.Add(mod);
					log.WriteLine(3, "Added module " + mod.Name + (mod.Alias != mod.Name ? " alias " + mod.Alias : String.Empty));
				}
				catch { }
			}

			#endregion

			#endregion

			#region Move program start actions to be executed by the virtual module

			foreach (ModuleClient mc in blackboard.modules)
			{
				if(mc == blackboard.virtualModule)
					continue;
				MoveActions(mc.RestartActions, blackboard.virtualModule.RestartActions);
				MoveActions(mc.RestartTestActions, blackboard.virtualModule.RestartTestActions);
				MoveActions(mc.StartupActions, blackboard.virtualModule.StartupActions);
				MoveActions(mc.StopActions, blackboard.virtualModule.StopActions);
				MoveActions(mc.TestTimeOutActions, blackboard.virtualModule.TestTimeOutActions);
			}

			#endregion

			#region Load of shared variables

			if (doc.GetElementsByTagName("sharedVariables").Count == 1)
			{
				tmpDoc.LoadXml(doc.GetElementsByTagName("sharedVariables")[0].OuterXml);
				SetupSharedVariables(tmpDoc, blackboard);
			}

			#endregion

			if (clientModulesAddedd < 1)
				throw new Exception("No modules has been added");
			log.WriteLine(0, "Load complete!");
			return blackboard;
		}

		/// <summary>
		/// Loads general blackboard configuration fro the provided XML document
		/// </summary>
		/// <param name="blackboard">The blackboard where will be loaded the settings</param>
		/// <param name="doc">The xml document from which the settings will be loaded</param>
		/// <param name="log">The log writer</param>
		private static void LoadBlackboardConfiguration(Blackboard blackboard, XmlDocument doc, ILogWriter log)
		{
			XmlDocument tmpDoc;
			string moduleName;
			ModuleClient mod;
			string sTime;
			int iTime;
			TimeSpan time;
			int sendAttempts;
			int globalCheckInterval;
			int moduleLoadDelay;

			log.WriteLine(0, "Loading configuration...");
			tmpDoc = new XmlDocument();
			tmpDoc.LoadXml(doc.GetElementsByTagName("configuration")[0].OuterXml);

			#region Blackboard Input port

			// Leo puerto
			if ((tmpDoc.GetElementsByTagName("port").Count != 1) ||
				!Int32.TryParse(tmpDoc.GetElementsByTagName("port")[0].InnerText, out blackboard.port))
			{
				blackboard.port = 2300;
				log.WriteLine(1, "No Blackboard port specified, using default: " + blackboard.Port);
			}
			else log.WriteLine(1, "Blackboard port: " + blackboard.Port);

			#endregion

			#region Blackboard Auto-Stop Time

			if (tmpDoc.GetElementsByTagName("autoStopTime").Count == 1)
			{
				sTime = tmpDoc.GetElementsByTagName("autoStopTime")[0].InnerText.Trim();
				if (Int32.TryParse(sTime, out iTime) && (iTime > 0))
				{
					blackboard.AutoStopTime = new TimeSpan(0, 0, iTime);
					log.WriteLine(1, "Blackboard Auto-Stop: " + blackboard.AutoStopTime);
				}
				else if (TimeSpan.TryParse(sTime, out time) && (time > TimeSpan.Zero))
				{
					blackboard.AutoStopTime = time;
					log.WriteLine(1, "Blackboard Auto-Stop: " + time);
				}
				else log.WriteLine(1, "Blackboard Auto-Stop disabled");
			}
			else log.WriteLine(1, "Blackboard Auto-Stop disabled");

			#endregion

			#region Blackboard Test Timeout

			if (tmpDoc.GetElementsByTagName("testTimeOut").Count == 1)
			{
				sTime = tmpDoc.GetElementsByTagName("testTimeOut")[0].InnerText.Trim();
				if (Int32.TryParse(sTime, out iTime) && (iTime > 0))
				{
					blackboard.TestTimeOut = new TimeSpan(0, 0, iTime);
					log.WriteLine(1, "Blackboard  Test Timeout: " + blackboard.TestTimeOut);
				}
				else if (TimeSpan.TryParse(sTime, out time) && (time > TimeSpan.Zero))
				{
					blackboard.TestTimeOut = time;
					log.WriteLine(1, "Blackboard Test Timeout: " + time);
				}
				else log.WriteLine(1, "Blackboard  Test Timeout disabled");
			}
			else log.WriteLine(1, "Blackboard  Test Timeout disabled");

			#endregion

			#region Load Blackboard Virtual Module
			
			if ((doc.GetElementsByTagName("name").Count != 1) || ((moduleName = doc.GetElementsByTagName("name")[0].InnerText.Trim()).Length < 3))
			{
				log.WriteLine(1, "No Virtual module name specified.");
				log.WriteLine(1, "\tUsing virtual module name: BLACKBOARD");
				blackboard.VirtualModule = new ModuleBlackboard("BLACKBOARD");
			}
			else
			{
				moduleName = moduleName.ToUpper();
				blackboard.VirtualModule = new ModuleBlackboard(moduleName.ToUpper());
				log.WriteLine(1, "Blackboard virtual module name: " + moduleName);
				moduleName = null;
			}

			#endregion

			#region Startup/Shutdown sequence

			LoadStartupSequence(blackboard, doc, log);
			LoadShutdownSequence(blackboard, doc, log);

			#endregion

			#region Other Configurations

			// Read global alive/busy/ready check interval
			if ((tmpDoc.GetElementsByTagName("globalCheckInterval").Count == 0) ||
				!Int32.TryParse(tmpDoc.GetElementsByTagName("globalCheckInterval")[0].InnerText, out globalCheckInterval) ||
				(globalCheckInterval < ModuleClient.MinCheckInterval) || (globalCheckInterval > ModuleClient.MaxCheckInterval))
			{
				log.WriteLine(1, "No alive/busy/ready check interval specified (or invalid), using default: " + ModuleClient.GlobalCheckInterval);
			}
			else
			{
				ModuleClient.GlobalCheckInterval = globalCheckInterval;
				log.WriteLine(1, "alive/busy/ready check interval: " + ModuleClient.GlobalCheckInterval);
			}

			// Module load delay
			if ((tmpDoc.GetElementsByTagName("moduleLoadDelay").Count == 0) ||
				!Int32.TryParse(tmpDoc.GetElementsByTagName("moduleLoadDelay")[0].InnerText, out moduleLoadDelay) ||
				(moduleLoadDelay < 0) || (moduleLoadDelay > 10000))
			{
				log.WriteLine(1, "Module load delay not specified (or invalid), using default: " + blackboard.ModuleLoadDelay.ToString());
			}
			else
			{
				blackboard.ModuleLoadDelay = moduleLoadDelay;
				log.WriteLine(1, "Module load delay: " + blackboard.ModuleLoadDelay.ToString());
			}

			// Leo intentos de reenvio
			if ((tmpDoc.GetElementsByTagName("sendAttempts").Count == 0) ||
				!Int32.TryParse(tmpDoc.GetElementsByTagName("sendAttempts")[0].InnerText, out sendAttempts) ||
				(sendAttempts < 0) && (sendAttempts > 5))
			{
				log.WriteLine(1, "No response send attempts specified (or invalid), using default: " + blackboard.SendAttempts);
			}
			else
			{
				blackboard.sendAttempts = sendAttempts;
				log.WriteLine(1, "Response send attempts: " + blackboard.SendAttempts);
			}

			#endregion

			#region Actions Extraction

			mod = blackboard.VirtualModule;

			// Startup actions extraction
			LoadModuleActions("onStart", "Startup", tmpDoc, mod.StartupActions, log);
			// Restart actions extraction
			LoadModuleActions("onRestart", "Restart", tmpDoc, mod.RestartActions, log);
			// Restart Test actions extraction
			LoadModuleActions("onRestartTest", "Restart-Test", tmpDoc, mod.RestartTestActions, log);
			// Stop actons extraction
			LoadModuleActions("onStop", "Stop", tmpDoc, mod.StopActions, log);
			// Test Timeout actons extraction
			LoadModuleActions("onTestTimeOut", "Test Timeout", tmpDoc, mod.TestTimeOutActions, log);

			#endregion
		}

		/// <summary>
		/// Creates a module using data contained in a xml module
		/// </summary>
		/// <param name="blackboard">The blackboard which will contin the module</param>
		/// <param name="xmlModuleNode">Xml node used to fetch the required module information.</param>
		/// <param name="log">The log writer</param>
		/// <returns>The module created with the data contained in the xml document</returns>
		private static ModuleClient CreateModule(Blackboard blackboard, XmlNode xmlModuleNode, ILogWriter log)
		{
			XmlDocument tmpDoc;
			XmlNode node;
			ModuleClient mod;
			string moduleName;
			bool moduleEnabled;
			string moduleAlias;
			bool aliveCheck;
			bool requirePrefix;
			int sendDelay;
			bool simulate;
			double simulationSuccessRatio;
			ModuleSimulationOptions simOptions;
			string processName;
			string programPath;
			string programArgs;
			List<IPAddress> ips;
			int port;
			int checkInterval;

			// Enabled
			moduleEnabled = true;
			if ((xmlModuleNode.Attributes["enabled"] != null) &&
				Boolean.TryParse(xmlModuleNode.Attributes["enabled"].Value, out moduleEnabled) &&
				!moduleEnabled)
				return null;

			moduleName = xmlModuleNode.Attributes["name"].Value.ToUpper();
			// Alias
			if (xmlModuleNode.Attributes["alias"] != null)
				moduleAlias = xmlModuleNode.Attributes["alias"].Value;
			else moduleAlias = moduleName;

			log.WriteLine(1, "Loading module " + moduleName + (moduleAlias != moduleName ? " alias " + moduleAlias : ""));
			// Create a XML sub-document XML
			tmpDoc = new XmlDocument();
			tmpDoc.LoadXml(xmlModuleNode.OuterXml);

			#region Get program path and program arguments

			FetchProgramInfo(tmpDoc, out processName, out programPath, out programArgs);

			#endregion

			// Leo el comando de inicio
			//if (tmpDoc.GetElementsByTagName("startupCommand").Count != 0)
			//	startupMessage = Command.Parse(tmpDoc.GetElementsByTagName("startupCommand")[0].InnerText);

			// Get the array of ip addresses where the module can be
			ips = FetchIpAddresses(tmpDoc);
			if ((ips == null) || (ips.Count < 1))
			{
				log.WriteLine(2, "\tNo valid IP Address provided");
				log.WriteLine(1, "Module skipped");
				return null;
			}

			// Leo el puerto de conexion del modulo
			if (
				(tmpDoc.GetElementsByTagName("port").Count == 0) ||
				!Int32.TryParse(tmpDoc.GetElementsByTagName("port")[0].InnerText, out port) ||
				(port <= 1024))
			{
				log.WriteLine(2, "\tInvalid port");
				log.WriteLine(1, "Module skipped");
				return null;
			}
			log.WriteLine("\t" + ips[0].ToString() + ":" + port);

			// Veify if Blackbard must check Module's alive status
			checkInterval = ModuleClient.GlobalCheckInterval;
			if (tmpDoc.GetElementsByTagName("aliveCheck").Count != 0)
			{
				node = tmpDoc.GetElementsByTagName("aliveCheck")[0];
				if (!Boolean.TryParse(node.InnerText, out aliveCheck))
					aliveCheck = true;
				// Read alive/busy/ready check interval
				if ((node.Attributes["interval"] == null) ||
					!Int32.TryParse(node.Attributes["interval"].InnerText, out checkInterval) ||
					(checkInterval < ModuleClient.MinCheckInterval) ||
					(checkInterval > ModuleClient.MaxCheckInterval))
					checkInterval = ModuleClient.GlobalCheckInterval;

			}
			else aliveCheck = true;
			// Verify if the Module requires SOURCE DESTINATION prefix
			if (
				(tmpDoc.GetElementsByTagName("requirePrefix").Count == 0) ||
				!Boolean.TryParse(tmpDoc.GetElementsByTagName("requirePrefix")[0].InnerText, out requirePrefix)
				) requirePrefix = false;
			// Delay between send operations
			if (
				(tmpDoc.GetElementsByTagName("sendDelay").Count == 0) ||
				!Int32.TryParse(tmpDoc.GetElementsByTagName("sendDelay")[0].InnerText, out sendDelay)
				) sendDelay = -1;
			// Simulation options
			simOptions = ModuleSimulationOptions.SimulationDisabled;
			if (tmpDoc.GetElementsByTagName("simulate").Count != 0)
			{
				node = tmpDoc.GetElementsByTagName("simulate")[0];
				simulationSuccessRatio = -1;
				if ((node.Attributes["successRatio"] == null) || !Double.TryParse(node.Attributes["successRatio"].InnerText, out simulationSuccessRatio) || (simulationSuccessRatio < 0) || (simulationSuccessRatio > 1))
					simulationSuccessRatio = -1;
				if (Boolean.TryParse(node.InnerText, out simulate))
					simOptions = (simulationSuccessRatio != -1) ?
						new ModuleSimulationOptions(simulationSuccessRatio) :
						new ModuleSimulationOptions(simulate);
			}

			// Module Creation
			mod = new ModuleClientTcp(moduleName, ips, port);
			mod.ProcessInfo.ProcessName = processName;
			mod.ProcessInfo.ProgramPath = programPath;
			mod.ProcessInfo.ProgramArgs = programArgs;
			mod.AliveCheck = aliveCheck;
			mod.RequirePrefix = requirePrefix;
			mod.SendDelay = sendDelay;
			mod.Simulation = simOptions;
			mod.Alias = moduleAlias;
			mod.CheckInterval = checkInterval;

			return mod;
		}

		/// <summary>
		/// Fetchs a list of IP addresses from the provided XML Document
		/// </summary>
		/// <param name="doc">The document from where to fecth the IP Addresses list</param>
		/// <returns>IP Addresses list</returns>
		private static List<IPAddress> FetchIpAddresses(XmlDocument doc)
		{
			XmlNodeList nodeList;
			List<IPAddress> ips;
			IPAddress ip;

			nodeList = doc.GetElementsByTagName("ip");
			if(nodeList.Count < 1)
				return null;
			ips = new List<IPAddress>(nodeList.Count);
			if (nodeList.Count > 0)
			{
				foreach (XmlNode node in nodeList)
				{
					if (IPAddress.TryParse(node.InnerText, out ip))
						ips.Add(ip);
				}
			}
			return ips;
		}

		/// <summary>
		/// Fetch program startup data used to launch a module
		/// </summary>
		/// <param name="doc">XML document which contains the program startup data</param>
		/// <param name="processName">When this method returns contains the name of the process asociated to the module if any, null otherwise</param>
		/// <param name="programPath">When this method returns contains the path used to launch the module if any, null otherwise</param>
		/// <param name="programArgs">When this method returns contains the arguments used to launch the module if any, null otherwise</param>
		private static void FetchProgramInfo(XmlDocument doc, out string processName, out string programPath, out string programArgs)
		{
			XmlNode programNode;
			processName = null;
			programPath = null;
			programArgs = null;

			if (doc.GetElementsByTagName("programPath").Count != 0)
				programPath = doc.GetElementsByTagName("programPath")[0].InnerText;

			// Leo los argumentos con que se inicia la aplicacion
			if (doc.GetElementsByTagName("programArgs").Count != 0)
				programArgs = doc.GetElementsByTagName("programArgs")[0].InnerText;

			if (doc.GetElementsByTagName("program").Count != 0)
			{
				programNode = doc.GetElementsByTagName("program")[0];
				if (programNode.Attributes["processName"] != null)
					processName = programNode.Attributes["processName"].Value.Trim();
				if (programNode.Attributes["path"] != null)
					programPath = programNode.Attributes["path"].Value.Trim();
				if (programNode.Attributes["args"] != null)
					programArgs = programNode.Attributes["args"].Value.Trim();
			}
		}

		/// <summary>
		/// Load a group of actions for the provided module action list
		/// </summary>
		/// <param name="actionGroup">The name of the group of actions in the xml file</param>
		/// <param name="actionName">The name of the actions to load (trigger)</param>
		/// <param name="doc">The xml document from which the actions will be loaded</param>
		/// <param name="ac">The action list to load the actions within</param>
		/// <param name="log">The log writer</param>
		private static void LoadModuleActions(string actionGroup, string actionName, XmlDocument doc, ActionCollection ac, ILogWriter log)
		{
			if (doc.GetElementsByTagName(actionGroup).Count > 0)
				ac.AddRange(XtractActionList(doc.GetElementsByTagName(actionGroup)[0].ChildNodes));
			if (ac.Count > 0)
				log.WriteLine(2, "\t" + ac.Count.ToString() + " " + actionName + " actions added");
		}

		/// <summary>
		/// Loads the startup sequence of the blackboard modules
		/// </summary>
		/// <param name="blackboard">The blackboard where will be loaded the startup sequence</param>
		/// <param name="doc">The xml document from which the actions will be loaded</param>
		/// <param name="log">The log writer</param>
		private static void LoadStartupSequence(Blackboard blackboard, XmlDocument doc, ILogWriter log)
		{
			XmlNodeList modules;
			string moduleName;

			if (doc.GetElementsByTagName("startupSequence").Count != 1)
			{
				log.WriteLine(1, "Startup sequence not found");
				return;
			}

			modules = doc.GetElementsByTagName("startupSequence")[0].ChildNodes;
			for(int i = 0; i < modules.Count; ++i)
			{
				try
				{
					// Module check
					if ((modules[i].Name != "module") ||
						//(modules[i].Attributes.Count < 1) ||
						//(modules[i].Attributes["name"].Value.Length < 1) ||
						(modules[i].InnerText == null) ||
						(modules[i].InnerText.Length < 1))
						continue;

					//moduleName = modules[i].Attributes["name"].Value.ToUpper();
					moduleName = modules[i].InnerText.ToUpper();
					if(blackboard.StartupSequence.ModuleSequence.Contains(moduleName))
						continue;
					blackboard.StartupSequence.ModuleSequence.Add(moduleName);
				}
				catch
				{
					continue;
				}
			}
			log.WriteLine(1, "Startup sequence: " + String.Join(", ", blackboard.StartupSequence.ModuleSequence.ToArray()));
		}

		/// <summary>
		/// Loads the shutdown sequence of the blackboard modules
		/// </summary>
		/// <param name="blackboard">The blackboard where will be loaded the shutdown sequence</param>
		/// <param name="doc">The xml document from which the actions will be loaded</param>
		/// <param name="log">The log writer</param>
		private static void LoadShutdownSequence(Blackboard blackboard, XmlDocument doc, ILogWriter log)
		{
			XmlNodeList modules;
			string moduleName;

			if (doc.GetElementsByTagName("shutdownSequence").Count != 1)
			{
				log.WriteLine(1, "Shutdown sequence not found");
				return;
			}

			modules = doc.GetElementsByTagName("shutdownSequence")[0].ChildNodes;
			for (int i = 0; i < modules.Count; ++i)
			{
				try
				{
					// Module check
					if ((modules[i].Name != "module") ||
						//(modules[i].Attributes.Count < 1) ||
						//(modules[i].Attributes["name"].Value.Length < 1) ||
						(modules[i].InnerText == null) ||
						(modules[i].InnerText.Length < 1))
						continue;

					//moduleName = modules[i].Attributes["name"].Value.ToUpper();
					moduleName = modules[i].InnerText.ToUpper();
					if (blackboard.ShutdownSequence.ModuleSequence.Contains(moduleName))
						continue;
					blackboard.ShutdownSequence.ModuleSequence.Add(moduleName);
				}
				catch
				{
					continue;
				}
			}
			log.WriteLine(1, "Shutdown sequence: " + String.Join(", ", blackboard.ShutdownSequence.ModuleSequence.ToArray()));
		}

		/// <summary>
		/// Moves the run and check actions from the source ActionCollection to the destination ActionCollection
		/// </summary>
		private static void MoveActions(ActionCollection source, ActionCollection destination)
		{
			if ((source == null) || (destination == null))
				return;
			for (int i = 0; i < source.Count; ++i)
			{
				if ((source[i] is ActionRun) || (source[i] is ActionCheck))
				{
					destination.Add(source[i]);
					source.RemoveAt(i);
					--i;
				}
			}
		}

		/// <summary>
		/// Creates and configures the shared variables specified in the provided XML document for the specified blackboard
		/// </summary>
		/// <param name="doc">XML document which contains shared variables initialization data</param>
		/// <param name="blackboard">Blackboard object in which the shared variables will be configured</param>
		private static void SetupSharedVariables(XmlDocument doc, Blackboard blackboard)
		{
			if ((doc == null) || (blackboard == null))
				throw new ArgumentNullException();
			if (blackboard.virtualModule == null)
				throw new Exception("Uninitialized blackboard");

			int i, j;
			ILogWriter log;
			XmlDocument tmpDoc;
			XmlNode node;
			XmlNodeList xmlVarList;
			XmlNodeList xmlWriterModuleList;
			SharedVariable shVar;
			List<string> writers;
			string shVarName;
			string shVarType;
			string shVarValue;
			bool shVarIsArray;
			int bracketPos;

			log = blackboard.log;
			tmpDoc = new XmlDocument();
			
			if (doc.GetElementsByTagName("sharedVariables").Count < 1)
			{
				log.WriteLine(1, "No shared variables was defined in XML file.");
				return;
			}
			tmpDoc.LoadXml(doc.GetElementsByTagName("sharedVariables")[0].OuterXml);


			#region Load of shared variables
			// Load of shared variables
			log.WriteLine(1, "Reading shared variables");
			xmlVarList = tmpDoc.GetElementsByTagName("var");
			for (i = 0; i < xmlVarList.Count; ++i)
			{
				#region Get variable Name
				node = xmlVarList[i];
				if ((node == null) || (node.Name != "var") || (node.Attributes["name"] == null))
					continue;
				shVarName = node.Attributes["name"].InnerText;
				shVarType = (node.Attributes["type"] != null) ? node.Attributes["type"].InnerText : "var";
				if (String.IsNullOrEmpty(shVarType) || !SharedVariable.RxVarTypeValidator.IsMatch(shVarType))
					shVarType = "var";
				bracketPos = shVarType.IndexOf("[");
				if (shVarIsArray = (bracketPos != -1))
					shVarType = shVarType.Remove(bracketPos);
				if (blackboard.VirtualModule.SharedVariables.Contains(shVarName))
				{
					log.WriteLine(2, "Error loading shared variable " + shVarName + ". Variable already exists.");
					continue;
				}

				#endregion

				#region Get variable initial value
				shVarValue = "";
				if (node.Attributes["value"] != null)
				{
					shVarValue = node.Attributes["value"].Value;
				}
				else if (node.Attributes["fromFile"] != null)
				{
					shVarValue = node.Attributes["fromFile"].Value;
					if (File.Exists(shVarValue))
					{
						try
						{
							shVarValue = File.ReadAllText(shVarValue);
						}
						catch
						{
							log.WriteLine(2, "Error loading variable content from file " + shVarValue + " for the shared variable " + shVarName + ". Variable was set to null");
							shVarValue = "";
						}
					}
				}
				#endregion

				#region Get list of modules with write permission

				writers = new List<string>();
				tmpDoc = new XmlDocument();
				tmpDoc.LoadXml(node.OuterXml);
				xmlWriterModuleList = tmpDoc.GetElementsByTagName("writers");
				if (xmlWriterModuleList.Count == 1)
				{
					writers.Add(blackboard.VirtualModule.Name);
					tmpDoc = new XmlDocument();
					tmpDoc.LoadXml(xmlWriterModuleList[0].OuterXml);
					xmlWriterModuleList = tmpDoc.GetElementsByTagName("writer");
					for (j = 0; j < xmlWriterModuleList.Count; ++j)
					{
						node = xmlWriterModuleList[j];
						if (node.InnerText == "*")
						{
							writers.Clear();
							writers.Add("*");
							break;
						}
						if (!blackboard.modules.Contains(node.InnerText))
							continue;
						else
							writers.Add(node.InnerText);
						/*
						if (!writers.Contains(node.InnerText))
							writers.Add(node.InnerText);
						*/
					}
				}
				else
					writers.Add("*");

				#endregion

				#region Create and add the shared variable

				shVar = new SharedVariable(blackboard.VirtualModule, shVarType, shVarName, shVarIsArray, -1);
				//shVar.Data = shVarValue;
				shVar.WriteStringData(blackboard.VirtualModule, shVarType, -1, shVarValue);
				writers.Sort();
				shVar.AllowedWriters = writers;
				blackboard.VirtualModule.SharedVariables.Add(shVar);
				log.WriteLine(4, "Added shared variable " + shVarType +" "+ shVarName);

				#endregion
			}
			#endregion
		}

		/// <summary>
		/// Extracts a list of actions from a XmlNodeList
		/// </summary>
		/// <param name="nodeList">A XMLNodeList to extract actions from</param>
		/// <returns>An array of Actions</returns>
		private static IAction[] XtractActionList(XmlNodeList nodeList)
		{
			int i;
			List<IAction> actions;
			IAction action;

			actions = new List<IAction>(nodeList.Count);
			for (i = 0; i < nodeList.Count; ++i)
			{
				switch (nodeList[i].Name.ToLower())
				{
					case "killprocess":
						 if (ActionKill.TryParse(nodeList[i], out action))
							actions.Add(action);
						break;

					case "checkprocess":
						if (ActionCheck.TryParse(nodeList[i], out action))
							actions.Add(action);
						break;

					case "run":
						if (ActionRun.TryParse(nodeList[i], out action))
							actions.Add(action);
						break;

					case "send":
						if (ActionSend.TryParse(nodeList[i], out action))
							actions.Add(action);
						break;

					case "socketsend":
						if (ActionSocketSend.TryParse(nodeList[i], out action))
							actions.Add(action);
						break;
				}
			}
			return actions.ToArray();
		}

		/// <summary>
		/// Extracts the module information from the provided xml document
		/// </summary>
		/// <param name="module">The module to load the commands in</param>
		/// <param name="doc">The document from where the commands will be loaded</param>
		/// <param name="log">The log writer</param>
		/// <returns>Returns a list of prototypes which contains the command information.</returns>
		private static Prototype[] XtractModuleCommands(ModuleClient module, XmlDocument doc, LogWriter log)
		{
			XmlNodeList commands;
			List<Prototype> protoList;
			Prototype proto;
			string cmdName;
			bool cmdParams;
			bool cmdAnswer;
			bool cmdPriority;
			int cmdTimeOut;

			// Get the list of commands. If none, return
			log.WriteLine(2, "\tLoading module commands...");
			if (doc.GetElementsByTagName("commands").Count < 1)
			{
				log.WriteLine(3, "\tNo commands to load");
				return null;
			}

			commands = doc.GetElementsByTagName("commands")[0].ChildNodes;
			protoList = new List<Prototype>(commands.Count);

			#region Module command extraction

			for (int j = 0; j < commands.Count; ++j)
			{
				// Verify that the node is a command
				cmdTimeOut = 0;
				if ((commands[j].Name == "command") &&
				(commands[j].Attributes.Count >= 3) &&
				(commands[j].Attributes["name"].Value.Length > 1) &&
				Boolean.TryParse(commands[j].Attributes["answer"].Value, out cmdAnswer) &&
				(
					(cmdAnswer && Int32.TryParse(commands[j].Attributes["timeout"].Value, out cmdTimeOut) && (cmdTimeOut >= 0)) || !cmdAnswer
				))
				{
					// Read command name
					cmdName = commands[j].Attributes["name"].Value;
					log.WriteLine(2, "\t\tAdded command " + cmdName);
					// Check if command require parameters
					if ((commands[j].Attributes["parameters"] == null) || !Boolean.TryParse(commands[j].Attributes["parameters"].Value, out cmdParams))
						cmdParams = true;
					// Check if command has preiority
					if ((commands[j].Attributes["priority"] == null) || !Boolean.TryParse(commands[j].Attributes["priority"].Value, out cmdPriority))
						cmdPriority = false;
					// Create the prototype
					proto = new Prototype(cmdName, cmdParams, cmdAnswer, cmdTimeOut, cmdPriority);
					// Add the prototype to the module
					module.Prototypes.Add(proto);
					protoList.Add(proto);
				}
				else log.WriteLine(4, "\t\tInvalid Command ");
			}
			#endregion
			// If there are no commands supported by the module, skip it.
			if (protoList.Count < 1)
				log.WriteLine(3, "\tAll commands rejected.");

			return protoList.ToArray();
		}

		#endregion

	}
}
