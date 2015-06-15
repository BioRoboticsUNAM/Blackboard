using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Robotics;
using Blk.Api;
using Blk.Engine.Actions;
using Blk.Engine.SharedVariables;
using Robotics.Utilities;
using Robotics.Sockets;

namespace Blk.Engine
{

	#region Delegates
	/// <summary>
	/// Represents the method that will handle the Connected and Disconnected event of a Module object.
	/// </summary>
	/// <param name="m">Module connected/disconnected</param>
	public delegate void ModuleConnectionEH(ModuleClient m);
	/// <summary>
	/// Represents the method that will handle the ActionExecuted event of a Module object.
	/// </summary>
	/// <param name="m">Module which performed the action</param>
	/// <param name="action">Action executed</param>
	/// <param name="success">true if the action executed successfully, false otherwise</param>
	internal delegate void ActionExecutedEH(ModuleClient m, IAction action, bool success);
	#endregion

	/// <summary>
	/// Serves as base class to implement an interface to access a module
	/// </summary>
	public abstract class ModuleClient : IModuleClient
	{
		#region Constants

		/// <summary>
		/// Stores the Regular Expression used to match the known system control commands
		/// </summary>
		protected const string RxControlCommandNames = "alive|bin|busy|prototypes|ready|restart_test";

		/// <summary>
		/// Maximum check interval for alive, busy and ready checkup messages in milliseconds
		/// </summary>
		public const int MaxCheckInterval = 60000;

		/// <summary>
		/// Minimum check interval for alive, busy and ready checkup messages in milliseconds
		/// </summary>
		public const int MinCheckInterval = 1000;

		#endregion

		#region Variables

		/// <summary>
		/// Alias of the module. Will be equals to the modulename if not valid
		/// </summary>
		protected string alias;
		
		/// <summary>
		/// Stores the name of the person reponsible of the module, or its author
		/// </summary>
		protected string author;

		/// <summary>
		/// Module name
		/// </summary>
		protected string name;
		/// <summary>
		/// Indicates if the module is enabled.
		/// If module is disabled can not be started and will be ignored by the Blackboard.
		/// </summary>
		private bool enabled;
		/// <summary>
		/// The blackboard this module is bind to
		/// </summary>
		internal Blackboard parent;
		/// <summary>
		/// List of the command prototypes supported by the Application Module
		/// </summary>
		protected PrototypeCollection prototypes;
		/// <summary>
		/// Indicates if the Module require the "SENDER DESTINATION" prefix before the command name
		/// </summary>
		protected bool requirePrefix;
		/// <summary>
		/// Indicates if simulation is active
		/// </summary>
		protected IModuleSimulationOptions simOptions;
		/// <summary>
		/// Specifies if the thread is waiting while trying to connect
		/// </summary>
		protected bool connecting;
		/// <summary>
		/// Interval time for alive, busy and ready checkup messages in milliseconds
		/// </summary>
		private int checkInterval;
		/// <summary>
		/// The delay between send operations in milliseconds
		/// </summary>
		/// <remarks>
		/// A negative value disables the delay
		/// A zero value postpones the send operation untill the next execution of the thread
		/// The maximum value is 300
		/// </remarks>
		public int sendDelay;
		/// <summary>
		/// Random number generator
		/// </summary>
		private Random rnd;

		/// <summary>
		/// Stores data about the program asociated to this module
		/// </summary>
		private ModuleProcessInfo executableInfo;

		#region Execution and Thread Vars

		/// <summary>
		/// Thread for connecting to remote application and perform async tasks
		/// </summary>
		protected Thread mainThread;
		/// <summary>
		/// Object used to lock access to Start and Stop methods
		/// </summary>
		protected object lockObject = new Object();
		/// <summary>
		/// Object used to lock access to main thread pointer
		/// </summary>
		protected object mainTheadLockObject = new Object();
		/// <summary>
		/// Indicates if the Module is runnuing
		/// </summary>
		protected bool running;
		/// <summary>
		/// Indicates if the MainThread must be stopped
		/// </summary>
		protected bool stopMainThread;
		/// <summary>
		/// Indicates if a module restart has been requested
		/// </summary>
		protected bool restartRequested;
		/// <summary>
		/// Indicates if a test restart has been requested
		/// </summary>
		protected bool restartTestRequested;
		/// <summary>
		/// Indicates if a test timeout has been requested
		/// </summary>
		protected bool testTimeOutRequested;
		/// <summary>
		/// Event used to synchronize changes on the Ready property
		/// </summary>
		private ManualResetEvent readyEvent;

		#endregion

		#region Action Lists
		
		/// <summary>
		/// List of actions to perform when module is restarted
		/// </summary>
		protected ActionCollection restartActions;
		/// <summary>
		/// List of actions to perform when a restart test is requested
		/// </summary>
		protected ActionCollection restartTestActions;
		/// <summary>
		/// List of actions to perform when module is started
		/// </summary>
		protected ActionCollection startupActions;
		/// <summary>
		/// List of actions to perform when module is started
		/// </summary>
		protected ActionCollection stopActions;
		/// <summary>
		/// List of actions to perform when TestTimeOut is requested
		/// </summary>
		protected ActionCollection testTimeOutActions;

		#endregion

		#region Message flow vars
		/// <summary>
		/// Indicates if the module is responding
		/// </summary>
		protected bool alive;
		/// <summary>
		/// Indicates if the Module must check if Module is alive
		/// </summary>
		protected bool aliveCheck;
		/// <summary>
		/// Tells if the module is busy
		/// </summary>
		protected bool busy;
		/// <summary>
		/// Indicates that the module explicitly reported a busy status
		/// </summary>
		protected bool busyModule;
		/// <summary>
		/// Tells if the module supports binary commands
		/// </summary>
		protected bool bin;
		/// <summary>
		/// Indicates if module is running normaly and ready for operations
		/// </summary>
		protected bool ready;
		/// <summary>
		/// Contains the time when the last packet was received
		/// </summary>
		protected DateTime lastDataInTime;
		/// <summary>
		/// Contains the time when the last packet was received
		/// </summary>
		protected DateTime lastActivityTime;
		/// <summary>
		/// List of commands sent which are waiting for response
		/// </summary>
		private List<Command> lockList;
		/// <summary>
		/// Commands the main thread to clear the LockList to prevent list to be modified and get trouble
		/// </summary>
		protected bool clearLockList = false;
		#endregion

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Module class
		/// </summary>
		/// <param name="name">Module name</param>
		protected ModuleClient(string name)
		{
			this.name = name;
			this.alias = name;
			this.busy = false;
			this.ready = false;
			this.enabled = true;
			this.checkInterval = GlobalCheckInterval;
			this.executableInfo = new ModuleProcessInfo();
			//this.waitingResponse = new List<Command>(10);
			//this.responses = new List<Response>();
			//this.commands = new List<Command>();
			this.prototypes = new PrototypeCollection(this);
			this.lockList = new List<Command>();
			this.restartActions = new ActionCollection(this);
			this.restartTestActions = new ActionCollection(this);
			this.startupActions = new ActionCollection(this);
			this.stopActions = new ActionCollection(this);
			this.testTimeOutActions = new ActionCollection(this);

			this.restartRequested = false;
			this.restartTestRequested = false;
			this.rnd = new Random(name.GetHashCode());
			this.simOptions = ModuleSimulationOptions.SimulationDisabled;

			// Shared Variable related prototypes
			this.Prototypes.Add(Prototype.CreateVar);
			this.Prototypes.Add(Prototype.ListVars);
			this.Prototypes.Add(Prototype.Prototypes);
			this.Prototypes.Add(Prototype.ReadVar);
			this.Prototypes.Add(Prototype.ReadVars);
			this.Prototypes.Add(Prototype.StatVar);
			this.Prototypes.Add(Prototype.WriteVar);

			this.readyEvent = new ManualResetEvent(false);
		}

		/// <summary>
		/// Initializes a new instance of the Module class 
		/// </summary>
		/// <param name="name">Module name</param>
		/// <param name="ipAddresses">IP Address of Application Module's computer</param>
		/// <param name="port">Port of the Application Module</param>
		/// <param name="prototypes">List of the command prototypes supported by the Application Module</param>
		public ModuleClient(string name, Prototype[] prototypes)
			: this(name)
		{
			if ((prototypes == null) || (prototypes.Length == 0)) throw new ArgumentException("The prototypes list cannot be zero-length nor null");
			for (int i = 0; i < prototypes.Length; ++i)
				this.prototypes.Add(prototypes[i]);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets an alias for the module
		/// </summary>
		public virtual string Alias
		{
			get
			{
				if (alias == null)
					return name;
				return alias;
			}
			set
			{
				Regex rxName = new Regex(@"^([A-Z][A-Z\-]*)$");
				if((value == null) || !rxName.IsMatch(value))
				{
					alias = name;
					return;
				}
				this.alias = value.ToUpper().Trim();
			}
		}

		/// <summary>
		/// Stores the name of the person reponsible of the module, or its author
		/// </summary>
		public string Author
		{
			get { return this.author; }
			set
			{
				this.author = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating if the Module must check if Module is alive
		/// </summary>
		public virtual bool AliveCheck
		{
			get { return aliveCheck; }
			set
			{
				if (aliveCheck == value) return;
				aliveCheck = value;
				OnStatusChanged();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating if the current Module supports binnary commands
		/// </summary>
		public virtual bool Bin
		{
			get
			{
				return bin;
			}
			protected set
			{
				if (bin == value) return;
				bin = value;
				if (running) OnStatusChanged();
			}
		}

		/// <summary>
		/// Gets a value indicating if the current module is busy (waiting for a response)
		/// </summary>
		public virtual bool Busy
		{
			get
			{
				return busy;
			}
			protected set
			{
				if ((busy == value) && (busyModule == value)) return;
				busy = value;
				//if(!(busy = value))
				//	clearLockList = true;
				if (running) OnBusyChanged();
				if (running) OnStatusChanged();
			}
		}

		/// <summary>
		/// Gets or sets the interval time for alive, busy and ready checkup messages in milliseconds
		/// </summary>
		public int CheckInterval
		{
			get { return globalCheckInterval; }
			set
			{
				if ((value < MinCheckInterval) || (value > MaxCheckInterval))
					throw new ArgumentOutOfRangeException("GlobalCheckInterval must be between 1000 and 60000 milliseconds");
				checkInterval = value;
			}
		}
		
		/// <summary>
		/// Gets or sets a value indicating if the module is enabled.
		/// If module is disabled can not be started and will be ignored by the Blackboard.
		/// </summary>
		public bool Enabled
		{
			get { return this.enabled;}
			set {
			if(this.enabled == value)
				return;
			this.enabled = value;
			this.OnStatusChanged();
			}
		}

		/// <summary>
		/// Gets data about the program asociated to this module
		/// </summary>
		public IModuleProcessInfo ProcessInfo
		{
			get { return this.executableInfo; }
		}

		/// <summary>
		/// Gets the idle time of the module.
		/// </summary>
		/// <remarks>The idle time of a module is the amount of time elapsed since the last activity 
		/// of the module. Tipucally this time reflects the time elapsed since the last received command/response
		/// If the module is busy the idle time is TimeSpan.Zero</remarks>
		public virtual TimeSpan IdleTime
		{
			get
			{
				if(busy) return TimeSpan.Zero;
				if (lastActivityTime.Year == 1)
					return new TimeSpan(0, 0, -1);
				return DateTime.Now - lastActivityTime;
			}
		}

		/// <summary>
		/// Tells if the Module is responding and working
		/// </summary>
		public virtual bool IsAlive
		{
			get { return IsConnected && alive; }
			protected set
			{
				if (alive == value) return;
				alive = value;
				if (running) OnAliveChanged();
				if (running) OnStatusChanged();
			}
		}

		/// <summary>
		/// Tells if the connection to the remote module application has been stablished
		/// </summary>
		public abstract bool IsConnected { get; }

		/// <summary>
		/// Gets a value indicating if the module is running in the same machine as the blackboard
		/// </summary>
		public abstract bool IsLocal { get; }

		/// <summary>
		/// Gets a value indicating if the Module is running
		/// </summary>
		public bool IsRunning
		{
			get {
				lock (mainTheadLockObject)
				{
					return running && (mainThread != null) && mainThread.IsAlive;
				}
			}
		}
		
		/// <summary>
		/// Gets the name of the Module
		/// </summary>
		public string Name
		{
			get { return name; }
			set
			{
				if (this.IsRunning || ((this.parent != null) && this.parent.IsRunning))
					throw new Exception("Can not change the name of a module when the module or the blackboard is running");
				this.name = value;
			}
		}

		/// <summary>
		/// Gets the blackboard this Module is bind to
		/// </summary>
		public virtual Blackboard Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		/// <summary>
		/// Gets the blackboard to which the ModuleCollection object belongs
		/// </summary>
		IBlackboard IModuleClient.Parent
		{
			get { return this.parent; }
			set { Parent = (Blackboard)value; }
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
		/// Gets the prototypes managed by the Module
		/// </summary>
		IPrototypeCollection IModuleClient.Prototypes
		{
			get { return prototypes; }
		}

		/// <summary>
		/// Gets a value indicating if module is ready for normal operation
		/// </summary>
		public virtual bool Ready
		{
			get
			{
				return ready;
			}
			protected set
			{
				if (ready == value)
					return;
				if (ready = value)
					this.readyEvent.Set();
				else
					this.readyEvent.Reset();

				if (running) OnReadyChanged();
				if (running) OnStatusChanged();
			}
		}

		/// <summary>
		/// Gets a value ingicating if the Module require the "SENDER DESTINATION" prefix before the command name
		/// </summary>
		public virtual bool RequirePrefix
		{
			get
			{
				return requirePrefix;
			}
			set
			{
				if (this.IsRunning || ((this.parent != null) && this.parent.IsRunning))
					throw new Exception("Can not change the prefix requirement of a module when the module or the blackboard is running");
				requirePrefix = value;
			}
		}

		/// <summary>
		/// Gets or sets the delay between send operations in milliseconds
		/// </summary>
		/// <remarks>
		/// A negative value disables the delay
		/// A zero value postpones the send operation untill the next execution of the thread
		/// The maximum value is 300
		/// </remarks>
		public int SendDelay
		{
			get{ return this.sendDelay; }
			set{
				if (value < 0)
					this.sendDelay = -1;
				else if (value > 300)
					this.sendDelay = 300;
				else
					this.sendDelay = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating which mode of simulation is active
		/// </summary>
		public IModuleSimulationOptions Simulation
		{
			get { return simOptions; }
			set
			{
				if (simOptions == value) return;
				if (running) throw new Exception("Cannot change simulation mode while running");
				simOptions = value;
				OnStatusChanged();
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

		/// <summary>
		/// Gets the list of actions to perform when TestTimeOut is requested
		/// </summary>
		internal ActionCollection TestTimeOutActions
		{
			get { return testTimeOutActions; }
		}

		#endregion

		#endregion

		#region Events

		/// <summary>
		/// Occurs when an action is executed
		/// </summary>
		internal event ActionExecutedEH ActionExecuted;

		/// <summary>
		/// Occurs when the IsAlive property of a Module object changes its value
		/// </summary>
		public event Action<IModuleClient> AliveChanged;

		/// <summary>
		/// Occurs when the Busy property of a Module object changes its value
		/// </summary>
		public event Action<IModuleClient> BusyChanged;

		/// <summary>
		/// Occurs when the Module connects to a Tcp Server
		/// </summary>
		public event ModuleConnectionEH Connected;

		/// <summary>
		/// Occurs when the Module disconnects from a Tcp Server
		/// </summary>
		public event ModuleConnectionEH Disconnected;

		/// <summary>
		/// Occurs when a Command is received trough socket
		/// </summary>
		public event EventHandler<IModuleClient, ITextCommand> CommandReceived;

		/// <summary>
		/// Occurs when the Ready property of a Module object changes its value
		/// </summary>
		public event Action<IModuleClient> ReadyChanged;

		/// <summary>
		/// Occurs when a Response is received trough socket
		/// </summary>
		public event EventHandler<IModuleClient, ITextResponse> ResponseReceived;

		/// <summary>
		/// Occurs when the status of a Module object changes
		/// </summary>
		public event Action<IModuleClient> StatusChanged;

		/// <summary>
		/// Occurs when the status of a Module object starts working
		/// </summary>
		public event Action<IModuleClient> Started;

		/// <summary>
		/// Occurs when the status of a Module object stops working
		/// </summary>
		public event Action<IModuleClient> Stopped;

		#endregion

		#region Abstract Methods

		/// <summary>
		/// When overriden in a derived class, disconnects from the Module application.
		/// This method is called by the Module Client Thread after the stop actions are executed,
		/// but before cleaning the thread
		/// </summary>
		protected abstract void MainThreadDisconnect();

		/// <summary>
		/// When overriden in a derived class, implements a loop to connect with the Module application.
		/// This method is called by the Module Client Thread after the startup actions are executed,
		/// but before entering the thread loop for send and receive messages
		/// </summary>
		protected abstract void MainThreadFirstTimeConnect();

		/// <summary>
		/// When overriden in a derived class, checks the status of the connection with the Module application,
		/// and, when broken, tries to connect with the Module application.
		/// This method is called by the Module Client Thread on each message loop
		/// </summary>
		protected abstract void MainThreadLoopAutoConnect();

		/// <summary>
		/// When overriden in a derived class, it sends the provided text string to the module application
		/// </summary>
		/// <param name="stringToSend">string to send to the module application</param>
		protected abstract bool Send(string stringToSend);

		#endregion

		#region Methods

		#region To check
		/*

		/// <summary>
		/// Checks if the response for the provided command has arrived
		/// </summary>
		/// <param name="command">The command for which response to look for</param>
		/// <param name="response">When this method returns, contains the response for the command provided if fount, or null if the response was not found.
		/// The search fails if the command parameter is a null reference (Nothing in Visual Basic), or the response was not found. This parameter is passed uninitialized. </param>
		/// <returns>true if the response has been found, false otherwise</returns>
		protected bool CheckForResponse(Command command, out Response response)
		{
			response = null;
			if (command == null) return false;
			foreach (Response r in responses)
			{
				if ((r.Command == command.Command) && (r.Id == command.Id))
				{
					response = r;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Synchronously sends a command to the module and waits for the response
		/// </summary>
		/// <param name="command">Command to send</param>
		/// <returns>Returns the response to the command</returns>
		public Response Send(Command command)
		{
			Response r;
			// Check if the module represented by this Module is the destination module for the command
			if (this != command.Destination)
				throw new Exception("Command marked to be sent through other module");
			// Check is connection has been stablished.
			if (!client.IsOpen)
				return Response.CreateFromCommand(command, false);
			// Check if module is not busy and command is not a priority command
			if(busy && !command.Prototype.HasPriority)
				return Response.CreateFromCommand(command, false);
			// Send the command
			try { client.Send(command.StringToSend + ""); }
			catch { return Response.CreateFromCommand(command, false); }
			command.SentTime = DateTime.Now;
			// If no response is required a success generic response is returned
			if(!command.Prototype.ResponseRequired)
				return Response.CreateFromCommand(command, true);
			// Add command to Waiting list
			waitingResponse.Add(command);
			// Wait untill response is received or timeout occur.
			while(command.MillisecondsLeft > 0)
			{
				Thread.Sleep(10);
				if (CheckForResponse(command, out r))
					return r;
			}
			// Since there is a TimeOut (i.e. no response received) a failed generic response is returned
			return Response.CreateFromCommand(command, false);
		}

		*/
		#endregion

		/// <summary>
		/// Performs all requested aditional actions
		/// </summary>
		protected void AditionalActions()
		{
			if (restartRequested)
			{
				restartRequested = false;
				DoRestartModule();
				return;
			}
			if (restartTestRequested)
			{
				restartRequested = false;
				DoRestartTest();
			}
			
			if (testTimeOutRequested)
			{
				testTimeOutRequested = false;
				ExecuteTestTimeOutActions();
			}
		}

		/// <summary>
		/// Asynchronously stops the socket connection and command management system.
		/// If the Module is not running, it has no effect.
		/// </summary>
		public void BeginStop()
		{
			if (!running) return;
			stopMainThread = true;
			if (Simulation.SimulationEnabled)
			{
				OnStopped();
			}
			OnStatusChanged();
		}

		/// <summary>
		/// Checks if the module is alive sending the alive command when necessary
		/// </summary>
		/// <param name="sendNextAliveTime">The time when the next alive command must be sent</param>
		protected void CheckAlive(ref DateTime sendNextAliveTime)
		{
			TimeSpan idleTime;
			if (!AliveCheck)
			{
				//IsAlive = true;
				return;
			}
			idleTime = DateTime.Now - this.lastDataInTime;

			#region Deprecated code
			/*
			if (!busy && (idleTime.TotalMilliseconds >= 15000))
			{
				IsAlive = false;
				if (DateTime.Now > sendNextAliveTime)
				{
					sendNextAliveTime = DateTime.Now.AddMilliseconds(15000);
					try
					{
						Send("alive");
					}
					catch { }
				}
			}
			else if (!busy) IsAlive = true;
			*/
			#endregion

			if (idleTime.TotalMilliseconds >= this.checkInterval)
			{
				IsAlive = false;
				if (DateTime.Now > sendNextAliveTime)
				{
					sendNextAliveTime = DateTime.Now.AddMilliseconds(this.checkInterval);
					try
					{
						if (busy) Send("busy");
						else if (!ready) Send("ready");
						else Send("alive");
					}
					catch { }
				}
			}
			else IsAlive = true;
		}

		/// <summary>
		/// Checks if the module is busy
		/// </summary>
		protected virtual void CheckBusy()
		{
			int i;

			// Remove commands caduced
			for (i = 0; i < lockList.Count; ++i)
				if ((lockList[i].Response != null) || (lockList[i].MillisecondsLeft <= 0)) lockList.RemoveAt(i);

			if (busy)
			{
				// Unlock on empty lock list
				if ((lockList.Count <= 0) || clearLockList)
					Unlock();
			}
		}

		/// <summary>
		/// Performs Restart Tasks
		/// </summary>
		protected virtual void DoRestartModule()
		{
			restartRequested = false;
			restartTestRequested = false;
			Busy = true;

			Unlock();

			ExecuteRestartActions();
			Busy = false;
		}

		/// <summary>
		/// Performs Restart-test Tasks
		/// </summary>
		protected virtual void DoRestartTest()
		{
			restartTestRequested = false;
			Busy = true;
			
			Unlock();
			ExecuteRestartTestActions();

			Busy = false;
		}

		/// <summary>
		/// Generates a Prototypes Response which includes all the prototypes for this module
		/// </summary>
		/// <param name="baseCommand">A Command object used to generate the response</param>
		/// <param name="preAppendModuleName">indicates if the name of the module will be pre-appended</param>
		/// <returns>A Response which includes all the prototypes for this module</returns>
		protected Response GetPrototypesResponse(Command baseCommand, bool preAppendModuleName)
		{
			return GetPrototypesResponse(baseCommand, this.name, preAppendModuleName);
		}

		/// <summary>
		/// Generates a Prototypes Response which includes all the prototypes for this module
		/// </summary>
		/// <param name="baseCommand">A Command object used to generate the response</param>
		/// <param name="moduleName">The name of the module from which prototypes will be fetched</param>
		/// <param name="preAppendModuleName">indicates if the name of the module will be pre-appended</param>
		/// <returns>A Response which includes all the prototypes for this module</returns>
		protected Response GetPrototypesResponse(Command baseCommand, string moduleName, bool preAppendModuleName)
		{
			if ((baseCommand == null) || !this.parent.Modules.Contains(moduleName))
				return null;
			
			ushort flags = 0;
			Response rsp= Response.CreateFromCommand(baseCommand, true);
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			IModuleClient module = this.parent.Modules[moduleName];
			IPrototype[] aProto;
			if (module.Prototypes is PrototypeCollection)
				aProto = ((PrototypeCollection)(module.Prototypes)).ToArray(false);
			else
				aProto = module.Prototypes.ToArray();

			if (preAppendModuleName)
			{
				sb.Append(moduleName);
				sb.Append(' ');
			}

			foreach (IPrototype proto in aProto)
			{
				sb.Append(proto.Command);
				sb.Append(' ');
				flags = (ushort)(proto.Priority & 0xFF);
				if (proto.ResponseRequired) flags |= 0x0100;
				if (proto.ParamsRequired) flags |= 0x0200;
				sb.Append(flags);
				sb.Append(' ');
				sb.Append(proto.Timeout);
				sb.Append(' ');
			}
			if (sb.Length > 0)
				--sb.Length;
			rsp.Parameters = sb.ToString();
			return rsp;
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
		/// Rises the ActionExecuted event
		/// </summary>
		/// <param name="action">Action executed</param>
		/// <param name="success">true if the action executed successfully, false otherwise</param>
		protected void OnActionExecuted(IAction action, bool success)
		{
			try
			{
				if (this.ActionExecuted != null)
					this.ActionExecuted(this, action, success);
			}
			catch { }
		}

		/// <summary>
		/// Rises the AliveChanged event
		/// </summary>
		protected void OnAliveChanged()
		{
			if ((this.parent != null) && (this.parent.VirtualModule != null))
				this.parent.VirtualModule.SvAliveModules.ReportSubscribers();

			try
			{
				if (this.AliveChanged != null)
					this.AliveChanged(this);
			}
			catch { }
		}

		/// <summary>
		/// Rises the BusyChanged event
		/// </summary>
		protected void OnBusyChanged()
		{
			if ((this.parent != null) && (this.parent.VirtualModule != null))
				this.parent.VirtualModule.SvBusyModules.ReportSubscribers();

			try
			{
				if (this.BusyChanged != null)
					this.BusyChanged(this);
			}
			catch { }
		}

		/// <summary>
		/// Rises the Connected event
		/// </summary>
		protected void OnConnected()
		{
			// Commented. No it is done by blackboard
			//if ((this.parent != null) && (this.parent.VirtualModule != null))
			//    this.parent.VirtualModule.SvConnectedModules.ReportSubscribers();

			try
			{
				if (this.Connected != null)
					this.Connected(this);
			}
			catch { }
		}

		/// <summary>
		/// Rises the CommandReceived event
		/// </summary>
		/// <param name="c">Command received</param>
		protected void OnCommandReceived(Command c)
		{
			try
			{
				if (this.CommandReceived != null)
					this.CommandReceived(this, c);
			}
			catch { }
		}

		/// <summary>
		/// Rises the Disconnected event
		/// </summary>
		protected void OnDisconnected()
		{
			// Commented. No it is done by blackboard
			//if ((this.parent != null) && (this.parent.VirtualModule != null))
			//	this.parent.VirtualModule.SvConnectedModules.ReportSubscribers();

			try
			{
				if (this.Disconnected != null)
					this.Disconnected(this);
			}
			catch { }
		}

		/// <summary>
		/// Rises the ReadyChanged event
		/// </summary>
		protected void OnReadyChanged()
		{
			if ((this.parent != null) && (this.parent.VirtualModule != null))
				this.parent.VirtualModule.SvReadyModules.ReportSubscribers();

			try
			{
				if (this.ReadyChanged != null)
					this.ReadyChanged(this);
			}
			catch { }
		}

		/// <summary>
		/// Rises the ResponseReceived event
		/// </summary>
		/// <param name="r">Response received</param>
		protected void OnResponseReceived(Response r)
		{
			try
			{
				if (this.ResponseReceived != null)
					this.ResponseReceived(this, r);
			}
			catch { }
		}

		/// <summary>
		/// Rises the Started event
		/// </summary>
		protected void OnStarted()
		{
			try
			{
				if (this.Started != null)
					this.Started(this);
			}
			catch { }
		}

		/// <summary>
		/// Rises the StatusChanged event
		/// </summary>
		protected void OnStatusChanged()
		{
			try
			{
				if (this.StatusChanged != null)
					this.StatusChanged(this);
			}
			catch { }
		}

		/// <summary>
		/// Rises the Stopped event
		/// </summary>
		protected void OnStopped()
		{
			try
			{
				if (this.Stopped != null)
					this.Stopped(this);
			}
			catch { }
		}
		
		/// <summary>
		/// Parses a received command
		/// </summary>
		/// <param name="command">Command to parse</param>
		protected virtual void ParseCommand(Command command)
		{
			switch (command.Command)
			{
				case "create_var":
					CreateVarCommand(command);
					break;

				case "list_vars":
					ListVarsCommand(command);
					break;

				case "prototypes":
					PrototypesCommand(command);
					break;

				case "read_var":
					ReadVarCommand(command);
					break;

				case "read_vars":
					ReadVarsCommand(command);
					break;

				case "stat_var":
					StatVarCommand(command);
					break;

				//case "suscribe_var":
				//case "subscribe_var":
				//	SubscribeVarCommand(command);
				//	break;

				case "write_var":
					WriteVarCommand(command);
					break;

				default:
					OnCommandReceived(command);
					break;
			}
		}

		/// <summary>
		/// Parses a string with a single message and, if valid, adds it to the corresponding queue
		/// </summary>
		protected void ParseMessage(string message)
		{
			Command c;
			Response r;

			if (String.IsNullOrEmpty(message))
				return;

			if (ParseControlResponse(message))
				return;

			// Check if message received is a response
			if (Response.TryParse(message, this, out r))
			{
				// A response has been received so it is redirected to blackboard
				this.lastActivityTime = DateTime.Now;
				ParseResponse(r);
			}
			// Check if message received is a command
			else if (Command.TryParse(message, this, out c))
			{
				// A command has been received so it is redirected to blackboard
				this.lastActivityTime = DateTime.Now;
				ParseCommand(c);
			}
		}

		/// <summary>
		/// Parses a control response
		/// </summary>
		/// <param name="s">String to parse</param>
		/// <returns>true if response parsed successfully, false otherwise</returns>
		protected bool ParseControlResponse(string s)
		{
			Regex rx = rxControlResponseParser;
			Match m;
			string command;
			string result;

			m = rx.Match(s);
			if (!m.Success) return false;
			command = m.Result("${command}");
			result = m.Result("${result}");
			if ((result == null) || (result.Length != 1)) result = "0";

			switch (command)
			{
				case "alive":
					IsAlive = true;
					return true;

				case "bin":
					IsAlive = true;
					return true;

				case "busy":
					if (!Busy && (result == "1"))
						busyModule = true;
					else if (Busy && (result == "0"))
						busyModule = false;

					if (!(Busy = (result == "1"))) Unlock();
					IsAlive = true;
					return true;

				case "ready":
					Ready = (result == "1");
					IsAlive = true;
					return true;

				case "restart_test":
					alive = true;
					return true;

				// This system command is handled by the Blackboard virtual module
				// case "prototypes":
				//	return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// Parses a prototype list and updates the list of prototypes
		/// </summary>
		/// <param name="s">String to parse</param>
		/// <returns>true if string parsed successfully, false otherwise</returns>
		private void ParsePrototypeList(string prototypeListStr)
		{
			Prototype proto;
			string commandName;
			int flags;
			int timeout;

			MatchCollection mc = rxPrototypeListParser.Matches(prototypeListStr);
			if (mc.Count < 1) return;
			foreach (Match m in mc)
			{
				try
				{
					if (!m.Success) continue;
					if (this.prototypes.Contains(commandName = m.Result("${command}"))) continue;
					if (!Int32.TryParse(m.Result("${flags}"), out flags)) continue;
					if (!Int32.TryParse(m.Result("${timeout}"), out timeout)) continue;
					proto = new Prototype(commandName,
						(flags & 0x0200) != 0,
						(flags & 0x0100) != 0,
						timeout,
						flags & 0x00FF);
					this.parent.AddPrototype(this, proto);
					Parent.Log.WriteLine(3, "[" + this.name + "] Added prototype for command:" + proto.ToString());
				}
				catch{ }
			}
		}

		/// <summary>
		/// When overriden in a derived class, parses all pending data received
		/// </summary>
		protected abstract void ParsePendingData();

		/// <summary>
		/// Parses a received response
		/// </summary>
		/// <param name="response">Command to parse</param>
		protected virtual void ParseResponse(Response response)
		{
			Unlock(response);
			OnResponseReceived(response);
		}

		/// <summary>
		/// Request a restart of the remote module
		/// </summary>
		public virtual void Restart()
		{
			this.restartRequested = true;
		}

		/// <summary>
		/// Executes actions to prepare a test restart
		/// </summary>
		public virtual void RestartTest()
		{
			this.restartTestRequested = true;
		}

		/// <summary>
		/// Sends a command to the module
		/// </summary>
		/// <param name="command">Response to send</param>
		/// <returns>true if the command has been sent, false otherwise</returns>
		public virtual bool Send(ITextCommand command)
		{
			if(command is Command)
				return Send((Command) command);
			return false;
		}

		/// <summary>
		/// Sends a command to the module
		/// </summary>
		/// <param name="command">Response to send</param>
		/// <returns>true if the command has been sent, false otherwise</returns>
		public virtual bool Send(Command command)
		{
			// Check if this module is the destination module for the command
			if (this != command.Destination)
				throw new Exception("Command marked to be sent through other module");
			if (!this.enabled)
				return false;
			++command.SendAttempts;

			// Simulation mode
			#region Simulation Mode
			if (simOptions.SimulationEnabled)
			{
				Response r = Response.CreateFromCommand(command, rnd.NextDouble() <= simOptions.SuccessRatio);
				command.SentStatus = SentStatus.SentSuccessfull;
				this.ParseResponse(r);
				return true;
			}
			#endregion

			// Check if module is not busy and command is not a priority command
			if (Busy && !command.Prototype.HasPriority && !IsControlCommand(command))
			{
				command.SentStatus = SentStatus.SentFailed;
				return false;
			}
			// Send the command
			if (Send(command.StringToSend + ""))
			{
				command.SentStatus = SentStatus.SentSuccessfull;
				if (command.Prototype.ResponseRequired) Lock(command);
			}
			else
			{
				command.SentStatus = SentStatus.SentFailed;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Synchronusly sends a command response to the module
		/// </summary>
		/// <param name="response">Response to send</param>
		/// <returns>true if the response has been sent, false otherwise</returns>
		public virtual bool Send(ITextResponse response)
		{
			if (response is Response)
				return Send((Response)response);
			return false;
		}

		/// <summary>
		/// Synchronusly sends a command response to the module
		/// </summary>
		/// <param name="response">Response to send</param>
		/// <returns>true if the response has been sent, false otherwise</returns>
		public virtual bool Send(Response response)
		{
			// Check if this module is the destination module for the response
			if (this != response.Destination)
				throw new Exception("Response marked to be sent through other module");
			if (!this.enabled)
				return false;
			++response.SendAttempts;

			// Simulation mode
			#region Simulation Mode
			if (simOptions.SimulationEnabled)
			{
				response.SentStatus = SentStatus.SentSuccessfull;
				return true;
			}
			#endregion

			// Send the response
			if (Send(response.StringToSend))
				response.SentStatus = SentStatus.SentSuccessfull;
			else
			{
				response.SentStatus = SentStatus.SentFailed;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Send a response using a command as base
		/// </summary>
		/// <param name="command">Base command</param>
		/// <param name="success">true if command succeded, false otherwise</param>
		protected virtual void SendResponse(Command command, bool success)
		{
			if (ResponseReceived == null) return;
			Response response = Response.CreateFromCommand(command, success);
			response.FailReason = ResponseFailReason.None;
			Send(response);
		}

		/// <summary>
		/// Returns a value indicating if the command is supported by this module
		/// </summary>
		/// <param name="commandName">The name of the command to search for</param>
		/// <returns>True if the command was found. false otherwise</returns>
		public bool SupportCommand(string commandName)
		{
			IPrototype prototype;
			return SupportCommand(commandName, out prototype);
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
		public bool SupportCommand(string commandName, out IPrototype prototype)
		{
			prototype = null;
			if (commandName == null)
				return false;
			if (!this.prototypes.Contains(commandName))
				return false;
			prototype = this.prototypes[commandName];
			return true;
		}

		/// <summary>
		/// Connect to the remote application and starts the command management system.
		/// If the Module is already running, it has no effect.
		/// </summary>
		public virtual void Start()
		{
			lock (lockObject)
			{
				if (!enabled || ((parent != null) && !parent.IsRunning)) return;
				if (running) return;
				running = true;

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
		public virtual void Stop()
		{
			lock (lockObject)
			{
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
			return name;
		}

		/// <summary>
		/// Marks the module as free
		/// </summary>
		protected void Unlock()
		{
			lockList.Clear();
			Busy = busyModule;
		}

		/// <summary>
		/// Checks if the response corresponds to a command that is ocupying the module to allow unlock it.
		/// </summary>
		/// <param name="r"></param>
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

		/// <summary>
		/// Blocks the thread call untill the module becomes ready
		/// </summary>
		public void WaitReady()
		{
			this.readyEvent.WaitOne();
		}

		/// <summary>
		/// Blocks the thread call untill the module becomes ready or the specified time elapses
		/// </summary>
		/// <param name="timeout">The amount of time in milliseconds to wait the module become ready</param>
		public void WaitReady(int timeout)
		{
			this.readyEvent.WaitOne(timeout);
		}

		#region SharedVariable and Prototype Methods

		/// <summary>
		/// Executes a create_var command.
		/// Requests the blackboard to create a variable with the specified name and size
		/// </summary>
		/// <param name="command">Command to execute</param>
		protected virtual void CreateVarCommand(Command command)
		{
			SharedVariable variable;
			bool result;
			SharedVariableCollection sharedVariables = this.Parent.VirtualModule.SharedVariables;
			Match match = SharedVariable.RxCreateSharedVariableValidator.Match(command.Parameters);
			if (!match.Success)
			{
				SendResponse(command, false);
				this.Parent.Log.WriteLine(7, this.Name + ": attempt to create variable failed");
				return;
			}
			string varName = match.Result("${name}");
			string varType = match.Result("${type}");
			bool isArray = false;
			string sArraySize;
			int arraySize = -1;
			if (String.IsNullOrEmpty(varType))
				varType = "var";
			if (!String.IsNullOrEmpty(match.Result("${array}")))
			{
				isArray = true;
				sArraySize = match.Result("${size}");
				if (!String.IsNullOrEmpty(sArraySize))
					arraySize = Int32.Parse(sArraySize);
			}

			if (sharedVariables.Contains(varName))
			{
				variable = sharedVariables[varName];
				result = variable.Type == varType;
				result &= variable.IsArray == isArray;
				result &= arraySize == variable.Size;
				SendResponse(command, result);
				this.Parent.Log.WriteLine(7, this.Name + ": attempt to create variable failed (already exists)");
				return;
			}

			SharedVariable sv = new SharedVariable(this, varType, varName, isArray, arraySize);
			sv.AllowedWriters.Add("*");
			sharedVariables.Add(sv);
			SendResponse(command, true);
			if (this.Parent.Log.VerbosityTreshold < 3)
				this.Parent.Log.WriteLine(2, this.Name + ": created variable " + varName);
			else
				this.Parent.Log.WriteLine(3, this.Name + ": created variable " + varName + " with " + Clamper.Clamp(command.StringToSend, 256));
		}

		/// <summary>
		/// Executes a list_vars command.
		/// Requests the blackboard to send the list of shared variables
		/// </summary>
		/// <param name="command">Command to execute</param>
		protected virtual void ListVarsCommand(Command command)
		{
			command.Parameters = this.Parent.VirtualModule.SharedVariables.NameList;
			SendResponse(command, true);
		}

		/// <summary>
		/// Executes a read_var command.
		/// Requests the blackboard to send the content of a stored a variable
		/// </summary>
		/// <param name="command">Command to execute</param>
		protected virtual void ReadVarCommand(Command command)
		{
			string varName;
			string varType;
			SharedVariable variable;
			SharedVariableCollection sharedVariables = this.Parent.VirtualModule.SharedVariables;
			Match match = SharedVariable.RxReadSharedVariableValidator.Match(command.Parameters);
			if (!match.Success)
			{
				SendResponse(command, false);
				this.Parent.Log.WriteLine(7, this.Name + ": attempt to read variable failed. Command is not well formed");
				return;
			}
			varName = match.Result("${name}");
			varType = match.Result("${type}");

			if (!sharedVariables.Contains(varName))
			{
				SendResponse(command, false);
				this.Parent.Log.WriteLine(7, this.Name + ": attempt to read variable " + varName + " failed (variable does not exist)");
				return;
			}
			variable = sharedVariables[varName];
			if ((variable.Type != "var") && !String.IsNullOrEmpty(varType) && (varType != variable.Type))
			{
				SendResponse(command, false);
				this.Parent.Log.WriteLine(7, this.Name + ": attempt to read variable " + varName + " failed (type mismatch)");
			}
			else
			{
				command.Parameters = variable.StringToSend;
				SendResponse(command, true);
				
				if (this.Parent.Log.VerbosityTreshold < 3)
					this.Parent.Log.WriteLine(2, this.Name + ": readed variable " + varName);
				else
					this.Parent.Log.WriteLine(3, this.Name + ": readed variable " + varName + " with " + Clamper.Clamp(command.StringToSend, 256));
			}
		}

		/// <summary>
		/// Executes a Prototypes command.
		/// Requests the blackboard to update the protype list of this module or requests the prototype list of another
		/// </summary>
		/// <param name="command">Command to execute</param>
		protected void PrototypesCommand(Command command)
		{
			if (!command.HasParams)
				return;
			Response rsp;
			if (this.parent.Modules.Contains(command.Parameters))
				rsp = GetPrototypesResponse(command, command.Parameters, true);
			else
			{
				ParsePrototypeList(command.Parameters);
				rsp = GetPrototypesResponse(command, false);
			}
			if (rsp != null)
				Send(rsp);
		}

		/// <summary>
		/// Executes a read_var command.
		/// Requests the blackboard to send the content of a stored a variable
		/// </summary>
		/// <param name="command">Command to execute</param>
		protected virtual void ReadVarsCommand(Command command)
		{
			int count;
			System.Text.StringBuilder sb;
			char[] separator ={ ' ', '\t', ',' };
			string[] varNames;
			SharedVariableCollection sharedVariables = this.Parent.VirtualModule.SharedVariables;
			Match match = SharedVariable.RxReadMultipleSharedVariableValidator.Match(command.Parameters);

			if (!match.Success)
			{
				SendResponse(command, false);
				this.Parent.Log.WriteLine(7, this.Name + ": attempt to read multiple variables failed. Command is not well formed");
				return;
			}

			count = 0;
			sb = new System.Text.StringBuilder(4096);
			varNames = match.Result("${names}").Split(separator, StringSplitOptions.RemoveEmptyEntries);

			foreach (string varName in varNames)
			{
				if (!sharedVariables.Contains(varName))
				{
					this.Parent.Log.WriteLine(7, this.Name + ": attempt to read variable " + varName + " failed (the specified variable does not exist)");
					continue;
				}
				++count;
				sb.Append(sharedVariables[varName].StringToSend);
			}

			if(count ==0)
			{
				SendResponse(command, false);
				this.Parent.Log.WriteLine(7, this.Name + ": attempt to read multiple variables failed (none of the specified variables exist)");
			}
			else
			{
				command.Parameters = sb.ToString();
				SendResponse(command, true);

				if (this.Parent.Log.VerbosityTreshold < 3)
					this.Parent.Log.WriteLine(2, this.Name + ": read of multiple variables succeeded");
				else
					this.Parent.Log.WriteLine(3, this.Name + ": readed multiple variables with " + Clamper.Clamp(command.StringToSend, 256));
				
				
			}
		}

		/// <summary>
		/// Gets all the information related to a shared variable
		/// </summary>
		/// <param name="command">Command to execute</param>
		protected virtual void StatVarCommand(Command command)
		{
			Robotics.API.SharedVariableInfo svInfo;
			string serialized;
			string varName = command.Parameters;
			if (!parent.VirtualModule.SharedVariables.Contains(varName))
			{
				SendResponse(command, false);
				this.Parent.Log.WriteLine(7, this.Name + ": attempt to get status of variable " + varName + " failed (variable does not exist)");
				return;
			}

			svInfo = parent.VirtualModule.SharedVariables[varName].GetInfo();
			if (!Robotics.API.SharedVariableInfo.Serialize(svInfo, out serialized))
			{
				SendResponse(command, false);
				this.Parent.Log.WriteLine(7, this.Name + ": attempt to get status of variable " + varName + " failed (serialization error)");
				return;
			}
			command.Parameters = serialized;
			SendResponse(command, true);
			this.Parent.Log.WriteLine(7, this.Name + ": status of variable " + varName + " obtained successfully");
		}

		/// <summary>
		/// Executes a write_var command.
		/// Requests the blackboard to write to a stored a variable the content provided
		/// </summary>
		/// <param name="command">Command to execute</param>
		protected virtual void WriteVarCommand(Command command)
		{
			SharedVariableCollection sharedVariables = this.Parent.VirtualModule.SharedVariables;
			Match match;
			string varType;
			string varName;
			string varData;
			string sArraySize;
			int arraySize;
			bool result;
			bool isArray = false;
			string parameters;

			parameters = command.Parameters;
			if ((parameters[0] == '{') && (parameters[parameters.Length - 1] == '}'))
				parameters = parameters.Substring(1, parameters.Length - 2);
			match = SharedVariable.RxWriteSharedVariableValidator.Match(parameters);
			if (!match.Success)
			{
				SendResponse(command, false);
				this.Parent.Log.WriteLine(7, this.Name + ": attempt to write variable failed");
				return;
			}
			varName = match.Result("${name}");
			varType = match.Result("${type}");
			varData = match.Result("${data}");
			arraySize = -1;
			if (String.IsNullOrEmpty(varType))
				varType = "var";
			if (!String.IsNullOrEmpty(match.Result("${array}")))
			{
				isArray = true;
				sArraySize = match.Result("${size}");
				if (!String.IsNullOrEmpty(sArraySize))
					arraySize = Int32.Parse(sArraySize);
			}
			if(isArray)
				parameters = "{ " + varType + "[" + (arraySize != -1 ? arraySize.ToString() : String.Empty) + "]" + " " + varName + " }";
			else
				parameters = "{ " + varType + " " + varName + " }";
			command.Parameters = parameters;
			if (!sharedVariables.Contains(varName))
			{
				SendResponse(command, false);
				this.Parent.Log.WriteLine(7, this.Name + ": attempt to write variable " + varName + " failed (variable does not exist)");
				return;
			}
			result = sharedVariables[varName].WriteStringData(command.Source, varType, arraySize, varData);
			SendResponse(command, result);
			

			if (this.Parent.Log.VerbosityTreshold < 3)
				this.Parent.Log.WriteLine(2, this.Name + ": written variable " + varName);
			else
				this.Parent.Log.WriteLine(3, this.Name + ": written variable " + varName + " with " + Clamper.Clamp(command.StringToSend, 256));
		}

		#endregion

		#region Action management methods

		/// <summary>
		/// Executes all startup actions.
		/// </summary>
		protected void ExecuteRestartActions()
		{
			bool result;
			for (int i = 0; i < restartActions.Count; ++i)
			{
				try
				{
					result = restartActions[i].Execute();
					OnActionExecuted(restartActions[i], result);
				}
				catch { }
			}
		}

		/// <summary>
		/// Executes all Restart-Test actions.
		/// </summary>
		protected void ExecuteRestartTestActions()
		{
			bool result;
			for (int i = 0; i < restartTestActions.Count; ++i)
			{
				try
				{
					result = restartTestActions[i].Execute();
					OnActionExecuted(restartTestActions[i], result);
				}
				catch { }
			}
		}

		/// <summary>
		/// Executes all startup actions.
		/// </summary>
		protected void ExecuteStartupActions()
		{
			bool result;
			Busy = true;
			for (int i = 0; i < startupActions.Count; ++i)
			{

				if (startupActions[i].Type == ActionType.Send) continue;
				result = startupActions[i].Execute();
				OnActionExecuted(startupActions[i], result);
			}
			Busy = false;
		}

		/// <summary>
		/// Executes all stop actions.
		/// </summary>
		protected void ExecuteStopActions()
		{
			bool result;
			for (int i = 0; i < stopActions.Count; ++i)
			{
				result = stopActions[i].Execute();
				OnActionExecuted(stopActions[i], result);
			}
		}

		/// <summary>
		/// Executes all Test Timeout actions.
		/// </summary>
		public void ExecuteTestTimeOutActions()
		{
			bool result;
			for (int i = 0; i < testTimeOutActions.Count; ++i)
			{
				result = testTimeOutActions[i].Execute();
				OnActionExecuted(testTimeOutActions[i], result);
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

		/// <summary>
		/// Returns a string array containing the string representation of the actions
		/// this module will perform on Test Timeout
		/// </summary>
		/// <returns>string array</returns>
		public string[] GetTestTimeOutActions()
		{
			string[] ttoa = new string[testTimeOutActions.Count];
			for (int i = 0; i < testTimeOutActions.Count; ++i)
				ttoa[i] = testTimeOutActions[i].ToString();
			return ttoa;
		}

		/// <summary>
		/// Request to execute actions due to Test Timeout
		/// </summary>
		public virtual void RequestExecuteTestTimeOutActions()
		{
			this.testTimeOutRequested = true;
		}

		#endregion

		#region Thread Control Methods

		/// <summary>
		/// Thread for autoconnect to remote application
		/// </summary>
		protected virtual void MainThreadTask()
		{
			DateTime sendNextAliveTime = DateTime.Now;

			MainThreadStartupTasks();
		  	ExecuteStartupActions();

			// First time connection
			MainThreadFirstTimeConnect();

			CheckAlive(ref sendNextAliveTime);

			// Send startup commands
			// This section is to allow to send the startup actions on first time connection.
			SendStartupCommands();

			while (running && !stopMainThread)
			{
				try
				{
					// Autoconnect (if disconnected)
					MainThreadLoopAutoConnect();

					// Parse received data
					ParsePendingData();

					// Check if module is alive and responding
					CheckAlive(ref sendNextAliveTime);

					// Check if module is busy and has not hang
					CheckBusy();

					// Performs additional actions
					AditionalActions();

					if (stopMainThread) break;
					// Sleep commented. There is a 10ms wait in the ProducerConumer.Consume method
					// in the ParsePendingData method.
					// Thread.Sleep(1);
				}
				catch (ThreadInterruptedException)
				{
					continue;
				}
				catch (ThreadAbortException)
				{
					Thread.ResetAbort();
					break;
				}
				catch { continue; }
			}
			
			ExecuteStopActions();
			
			Busy = true;
			Ready = false;

			// Disconnect
			MainThreadDisconnect();

			// Stop and Clear Thread
			StopAndClearMainThread();
		}

		/// <summary>
		/// Sends the startup commands to the remote module application
		/// </summary>
		protected void SendStartupCommands()
		{
			for (int i = 0; (i < startupActions.Count) && !stopMainThread; ++i)
			{
				if (startupActions[i].Type != ActionType.Send) continue;
				bool result = startupActions[i].Execute();
				OnActionExecuted(startupActions[i], result);
			}
		}

		/// <summary>
		/// Performs the first tasks to set up the main thread
		/// </summary>
		private void MainThreadStartupTasks()
		{
			this.stopMainThread = false;
			this.restartRequested = false;
			this.restartTestRequested = false;
			this.running = true;
			this.Ready = false;
		}

		/// <summary>
		/// Stops and clears the main thread
		/// </summary>
		private void StopAndClearMainThread()
		{
			this.running = false;
			this.Busy = false;
			OnStopped();
			OnStatusChanged();

			

			if (Monitor.TryEnter(mainTheadLockObject))
			{
				// Thread thisThread = mainThread;
				this.mainThread = null;
				Monitor.PulseAll(mainTheadLockObject);
				Monitor.Exit(mainTheadLockObject);
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

		/// <summary>
		/// Initializes the thread for autoconnection to ModuleServer
		/// If the server is running starts the thread automatically
		/// </summary>
		protected virtual void SetupMainThread()
		{
			lock (mainTheadLockObject)
			{
				if ((mainThread != null) && mainThread.IsAlive)
				{
					stopMainThread = true;
					mainThread.Join(100);
					if (mainThread.IsAlive)
						mainThread.Abort();
					stopMainThread = false;
				}
				if (!simOptions.SimulationEnabled)
					mainThread = new Thread(new ThreadStart(MainThreadTask));
				else
					mainThread = new Thread(new ThreadStart(MainSimulatedThread));
				mainThread.IsBackground = true;
				mainThread.Start();
			}
		}

		#endregion

		#endregion

		#region Static Variables

		/// <summary>
		/// Default interval time for alive, busy and ready checkup messages in milliseconds
		/// </summary>
		private static int globalCheckInterval = 10000;

		private static Regex rxControlCommandNameParser = new Regex(@"^" + RxControlCommandNames + "$?", RegexOptions.Compiled);
		private static Regex rxControlCommandParser = new Regex(@"^(" + RxControlCommandNames + @")(\s+@\d+)?$", RegexOptions.Compiled);
		private static Regex rxControlResponseParser = new Regex(@"(?<command>(" + RxControlCommandNames + @"))(\s+(?<params>""([^""]|(\\.))*""))?\s+(?<result>[10])(\s+@\d+)?", RegexOptions.Compiled);
		private static Regex rxPrototypeListParser = new Regex(@"(?<command>[A-Za-z_][0-9A-Za-z_]*)\s+(?<flags>\d{1,5})\s+(?<timeout>\d{1,8})", RegexOptions.Compiled);

		#endregion

		#region Static Properties

		/// <summary>
		/// Gets or sets the default interval time for alive, busy and ready checkup messages in milliseconds
		/// </summary>
		public static int GlobalCheckInterval
		{
			get { return globalCheckInterval; }
			set
			{
				if((value < MinCheckInterval) || (value > MaxCheckInterval))
					throw new ArgumentOutOfRangeException("GlobalCheckInterval must be between 1000 and 60000 milliseconds");
				globalCheckInterval = value;
			}
		}

		#endregion

		#region Methods de Clase (Estáticos)
		
		/// <summary>
		/// Check if a string is a control command
		/// </summary>
		/// <param name="s">String to verify</param>
		/// <returns>true if command parsed successfully, false otherwise</returns>
		protected static bool IsControlResponse(string s)
		{
			/*
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
			*/
			return rxControlResponseParser.IsMatch(s);
		}

		/// <summary>
		/// Check if given Command object matches a control command/response
		/// </summary>
		/// <param name="cmd">Command object to check</param>
		/// <returns>true if given CommandBase object is control command/, false otherwise</returns>
		public static bool IsControlCommand(Command cmd)
		{
			//return Regex.IsMatch(cmd.StringToSend, @"^(" + RxControlCommandNames + @")(\s+@\d+)?$") && !cmd.HasParams;
			return rxControlCommandParser.IsMatch(cmd.StringToSend) && !cmd.HasParams;
		}

		/// <summary>
		/// Check if given command name object matches a control command/response
		/// </summary>
		/// <param name="commandName">String to check</param>
		/// <returns>true if given string is control command name, false otherwise</returns>
		public static bool IsControlCommandName(string commandName)
		{
			return rxControlCommandNameParser.IsMatch(commandName);
		}

		/// <summary>
		/// Check if given Response object matches a control response
		/// </summary>
		/// <param name="rsp">Response object to check</param>
		/// <returns>true if given Response object is control response, false otherwise</returns>
		public static bool IsControlResponse(Response rsp)
		{
			return Regex.IsMatch(rsp.StringToSend, @"^" + RxControlCommandNames + @")\s+[10](\s+@\d+)?$") && !rsp.HasParams;
		}

		#endregion

		#region IComparable<IModule> Members

		/// <summary>
		/// Compares the current Module with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo(IModuleClient other)
		{
			return name.CompareTo(other.Name);
		}

		#endregion
	}
}

