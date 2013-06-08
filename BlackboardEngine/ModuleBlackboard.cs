using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Text;
using System.Xml;
using Robotics;
using Blk.Api;
using Blk.Api.SharedVariables;
using Blk.Engine.Remote;
using Blk.Engine.SharedVariables;

namespace Blk.Engine
{
	/// <summary>
	/// Implements a ModuleBlackboard
	/// </summary>
	public sealed class ModuleBlackboard : ModuleClient, IModuleBlackboard
	{
		#region Variables
		
		/// <summary>
		/// Stores all received commands
		/// </summary>
		private ProducerConsumer<ITextCommand> commandsReceived;

		/// <summary>
		/// Collection of retrieved Machine Status
		/// </summary>
		MachineStatusCollection msc;

		/// <summary>
		/// Stores the blackboard variables
		/// </summary>
		private SharedVariableCollection sharedVariables;

		/// <summary>
		/// Stores the list of subscriptions to not-yet-created blackboard variables
		/// </summary>
		private SortedList<string, List<SharedVariableSubscription>> pendingSubscriptions;

		/// <summary>
		/// Regular expression used to parse SetupModule command arguments
		/// </summary>
		private static readonly Regex rxSetupModule =
			new Regex(@"(?<moduleName>[A-Z][A-Z\-]+[A-Z])\s+ip\s*=\s*(?<ip>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\s+port\s*=\s*(?<port>\d{4,5})", RegexOptions.Compiled);

		/// <summary>
		/// A process manager object which can be used to start and stop remote modules.
		/// </summary>
		private ProcessManager processManager;

		/// <summary>
		/// Thread used to asynchronously launches the applications specified in the blackboad configuration file
		/// </summary>
		private Thread appLauncherThread;

		#region Shared Variables

		/// <summary>
		/// Shared variable which contains the list of alive modules
		/// </summary>
		private AliveModulesSharedVariable svAliveModules;

		/// <summary>
		/// Shared variable which contains the list of busy modules
		/// </summary>
		private BusyModulesSharedVariable svBusyModules;

		/// <summary>
		/// Shared variable which contains the list of connected modules
		/// </summary>
		private ConnectedModulesSharedVariable svConnectedModules;

		/// <summary>
		/// Shared variable which contains the list of registered modules
		/// </summary>
		private ModulesSharedVariable svModules;

		/// <summary>
		/// Shared variable which contains the list of ready modules
		/// </summary>
		private ReadyModulesSharedVariable svReadyModules;

		/// <summary>
		/// Shared variable which contains the list of shared variables
		/// </summary>
		private VariableListSharedVariable svVariableList;

		#endregion

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the ModuleBlackboard class
		/// </summary>
		/// <param name="name">Module name</param>
		public ModuleBlackboard(string name) : base(name)
		{
			sharedVariables = new SharedVariableCollection();
			sharedVariables.Add(svAliveModules = new AliveModulesSharedVariable(this));
			sharedVariables.Add(svBusyModules = new BusyModulesSharedVariable(this));
			sharedVariables.Add(svConnectedModules = new ConnectedModulesSharedVariable(this));
			sharedVariables.Add(svModules = new ModulesSharedVariable(this));
			sharedVariables.Add(svReadyModules = new ReadyModulesSharedVariable(this));
			sharedVariables.Add(svVariableList = new VariableListSharedVariable(this));

			pendingSubscriptions = new SortedList<string,List<SharedVariableSubscription>>();
			// Add default blackboard commands
			//this.Prototypes.Add(pttAlive);
			//this.Prototypes.Add(pttBin);
			//this.Prototypes.Add(pttBusy);
			//this.Prototypes.Add(pttConnected);
			//this.Prototypes.Add(pttReady);
			this.Prototypes.Add(new Prototype("alive", true, true, 300, true));
			this.Prototypes.Add(new Prototype("bin", true, true, 300, true));
			this.Prototypes.Add(new Prototype("busy", true, true, 300, true));
			this.Prototypes.Add(new Prototype("connected", true, true, 300, true));
			this.Prototypes.Add(new Prototype("ready", true, true, 300, true));
			this.Prototypes.Add(new Prototype("modules", false, true, 300, true));
			this.Prototypes.Add(new Prototype("idletime", true, true, 300, true));
			this.Prototypes.Add(new Prototype("setupmodule", true, true, 500, true));
			this.Prototypes.Add(new Prototype("querymodule", true, true, 300, true));
			this.Prototypes.Add(new Prototype("reset", true, false, 300, true));
			this.Prototypes.Add(new Prototype("restart_test", true, false, 300, true));
			this.Prototypes.Add(new Prototype("bbstatus", true, true, 300, true));
			this.Prototypes.Add(new Prototype("mstatus", true, true, 300, true));
			this.Prototypes.Add(new Prototype("mystat", true, false, 300, true));

			// Remote module start and stop commands
			this.Prototypes.Add(new Prototype("start_module_app", true, true, 3000, true));
			this.Prototypes.Add(new Prototype("stop_module_app", true, false, 3000, true));

			// Shared Variable related
			this.Prototypes.Add(new Prototype("suscribe_var", true, true, 500, true));
			this.Prototypes.Add(new Prototype("subscribe_var", true, true, 500, true));
			this.Prototypes.Add(new Prototype("unsubscribe_var", true, true, 500, true));
			this.Prototypes.Add(new Prototype("stat_var", true, true, 500, true));

			// Moved to parent class
			//this.Prototypes.Add(new Prototype("create_var", true, true, 300, true));
			//this.Prototypes.Add(new Prototype("list_vars", true, true, 300, true));
			//this.Prototypes.Add(new Prototype("read_var", true, true, 300, true));
			//this.Prototypes.Add(new Prototype("read_vars", true, true, 300, true));
			//this.Prototypes.Add(new Prototype("write_var", true, true, 300, true));

			commandsReceived = new ProducerConsumer<ITextCommand>(50);
			processManager = new ProcessManager(parent.Log);
			msc = new MachineStatusCollection(this);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating if the ModuleBlackboard must check if Module is alive
		/// </summary>
		public override bool AliveCheck
		{
			get { return aliveCheck; }
			set
			{
				if (aliveCheck == value) return;
				aliveCheck = value;
			}
		}

		/// <summary>
		/// Tells if the connection to the ModuleServer has been stablished
		/// </summary>
		public override bool IsConnected
		{
			get
			{
				return running;
			}
		}

		/// <summary>
		/// Always return true
		/// </summary>
		public override bool IsLocal
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets a value indicating if module is ready for normal operation
		/// </summary>
		public override bool Ready
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the list of shared variables
		/// </summary>
		public SharedVariableCollection SharedVariables
		{
			get { return this.sharedVariables; }
		}

		/// <summary>
		/// Gets the list of shared variables
		/// </summary>
		ISharedVariableCollection IModuleBlackboard.SharedVariables
		{
			get { return this.sharedVariables; }
		}

		/// <summary>
		/// Gets the collection of status of each connected machine.
		/// </summary>
		public MachineStatusCollection MachineStatusList
		{
			get { return msc; }
		}

		#region Shared Variables

		/// <summary>
		/// Gets the shared variable which contains the list of alive modules
		/// </summary>
		internal AliveModulesSharedVariable SvAliveModules
		{
			get { return this.svAliveModules; }
		}

		/// <summary>
		/// Gets the shared variable which contains the list of busy modules
		/// </summary>
		internal BusyModulesSharedVariable SvBusyModules
		{
			get { return this.svBusyModules; }
		}

		/// <summary>
		/// Gets the shared variable which contains the list of connected modules
		/// </summary>
		internal ConnectedModulesSharedVariable SvConnectedModules
		{
			get { return this.svConnectedModules; }
		}

		/// <summary>
		/// Gets the shared variable which contains the list of registered modules
		/// </summary>
		internal ModulesSharedVariable SvModules
		{
			get { return this.svModules; }
		}

		/// <summary>
		/// Gets the shared variable which contains the list of ready modules
		/// </summary>
		internal ReadyModulesSharedVariable SvReadyModules
		{
			get { return this.svReadyModules;}
		}

		/// <summary>
		/// Gets the shared variable which contains the list of shared variables
		/// </summary>
		internal VariableListSharedVariable SvVariableList
		{
			get { return this.svVariableList; }
		}

		#endregion

		#endregion

		#region Events

		#endregion

		#region Methods

		/// <summary>
		/// Performs Restart Tasks
		/// </summary>
		protected override void DoRestartModule()
		{
			Busy = true;

			//dataReceived.Clear();
			ExecuteRestartActions();

			Busy = false;
		}

		/// <summary>
		/// Performs Restart-test Tasks
		/// </summary>
		protected override void DoRestartTest()
		{
			Busy = true;

			commandsReceived.Clear();
			ExecuteRestartTestActions();

			Busy = false;
		}

		/// <summary>
		/// Fills out the MachineStatusCollection list with information from blackboard
		/// </summary>
		private void FillMachineStatussList()
		{
			//msc.Add(new MachineStatus(IPAddress.Parse("127.0.0.1"), 50, BatteryChargeStatus.Unknown));
			foreach (IModuleClient m in this.Parent.Modules)
			{
				IModuleClientTcp module = m as IModuleClientTcp;
				if (module == null) continue;
				for (int i = 0; i < module.ServerAddresses.Count; ++i)
				{
					if (!msc.Contains(module.ServerAddresses[i]) &&
						!module.ServerAddresses[i].Equals(IPAddress.Any) &&
						!module.ServerAddresses[i].Equals(IPAddress.Broadcast))
					{
						msc.Add(new MachineStatus(module.ServerAddresses[i], 50, BatteryChargeStatus.Unknown));
					}
				}
			}
		}
		
		/// <summary>
		/// No data parsing is required.
		/// All process is made by the ParsePendingCommands method
		/// </summary>
		protected override void ParsePendingData()
		{
		}

		/// <summary>
		/// Parses all pending TCPPackets received
		/// </summary>
		private void ParsePendingCommands()
		{
			Command c;
			Response r;

			do
			{
				c = commandsReceived.Consume(100) as Command;
				if (c == null) continue;

				switch (c.Command)
				{
					case "alive":
						AliveCommand(c);
						break;

					case "bbstatus":
						BbStatus(c);
						break;

					case "busy":
						BusyCommand(c);
						break;

					case "connected":
						ConnectedCommand(c);
						break;

					case "create_var":
						CreateVarCommand(c);
						break;

					case "idletime":
						IdleTimeCommand(c);
						break;

					case "list_vars":
						ListVarsCommand(c);
						break;

					case "modules":
						ModulesCommand(c);
						break;

					case "mstatus":
						MstatusCommand(c);
						break;

					case "mystat":
						MystatCommand(c);
						break;

					case "querymodule":
						QueryModuleCommand(c);
						break;

					case "read_var":
						ReadVarCommand(c);
						break;

					case "reset":
						ResetCommand(c);
						break;

					case "ready":
						ReadyCommand(c);
						break;

					case "setupmodule":
						SetupModuleCommand(c);
						break;

					case "start_module_app":
						StartModuleAppCommand(c);
						break;

					case "stop_module_app":
						StopModuleAppCommand(c);
						break;

					case "suscribe_var":
					case "subscribe_var":
						SubscribeVarCommand(c);
						break;

					case "stat_var":
						StatVarCommand(c);
						break;

					case "unsubscribe_var":
						UnsubscribeVarCommand(c);
						break;

					case "write_var":
						WriteVarCommand(c);
						break;

					default:
						r = null;
						try { r = Response.CreateFromCommand(c, ResponseFailReason.Unknown); }
						catch { }
						if (r != null)
							OnResponseReceived(r);
						break;

				}
			} while (!stopMainThread && (commandsReceived.Count > 0));
		}

		/// <summary>
		/// Sends a command to the module
		/// </summary>
		/// <param name="command">Response to send</param>
		/// <returns>true if the command has been sent, false otherwise</returns>
		public override bool Send(ITextCommand command)
		{
			// Check if this module is the destination module for the command
			if (this != command.Destination)
				throw new Exception("Command marked to be sent through other module");

			// Check if module is not busy nor running and command is not a priority command
			if (!running || (Busy && !command.Prototype.HasPriority))
				return false;
			// Send the command
			commandsReceived.Produce(command);
			return true;
		}

		/// <summary>
		/// Synchronusly sends a command response to the module
		/// </summary>
		/// <param name="response">Response to send</param>
		/// <returns>true if the response has been sent, false otherwise</returns>
		public override bool Send(ITextResponse response)
		{
			// Check if this module is the destination module for the response
			if (this != response.Destination)
				throw new Exception("Response marked to be sent through other module");

			// Simulation mode
			#region Simulation Mode
			if (simOptions != ModuleSimulationOptions.SimulationDisabled)
			{
				return true;
			}
			#endregion

			return true;
		}

		/// <summary>
		/// Sends the provided text string through socket client
		/// </summary>
		/// <param name="stringToSend">string to send through socket</param>
		protected override bool Send(string stringToSend)
		{
			return true;
		}

		/// <summary>
		/// Send a response using a command as base
		/// </summary>
		/// <param name="command">Base command</param>
		/// <param name="success">true if command succeded, false otherwise</param>
		protected override void SendResponse(Command command, bool success)
		{
			try
			{
				Response r = Response.CreateFromCommand(command, success);
				r.FailReason = ResponseFailReason.None;
				OnResponseReceived(r);
			}
			catch { }
		}

		/// <summary>
		/// Connect to the remote application and starts the command management system.
		/// If the ModuleBlackboard is already running, it has no effect.
		/// </summary>
		public override void Start()
		{
			if (running) return;
			running = true;
			SetupMainThread();
			this.lastDataInTime = DateTime.Now;
			OnStarted();
			OnStatusChanged();

		}

		/// <summary>
		/// Disconnects from remote application and stops command management system.
		/// If the ModuleBlackboard is not running, it has no effect.
		/// </summary>
		public override void Stop()
		{
			if (!running) return;
			stopMainThread = true;

			if (Simulation != ModuleSimulationOptions.SimulationDisabled)
			{
				OnStopped();
				OnStatusChanged();
				return;
			}

			if (mainThread != null)
			{
				//while (mainThread.IsAlive && (attemptt++ < 3))
				//	Thread.Sleep(250);
				mainThread.Abort();
				mainThread.Join();
				mainThread = null;
			}

			ExecuteStopActions();

			running = false;

			OnStopped();
			OnStatusChanged();
		}

		/// <summary>
		/// Returns a String that represents the current Module. 
		/// </summary>
		/// <returns>A String that represents the current Module</returns>
		public override string ToString()
		{
			return name;
		}

		#region Command Execution Methods

		/// <summary>
		/// Executes a alive command.
		/// Asks the blackboard for the alive state of the requested module by the busy command
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void AliveCommand(Command command)
		{
			string moduleName = command.Parameters;
			if (!parent.Modules.Contains(moduleName))
				SendResponse(command, false);
			else
				SendResponse(command, parent.Modules[moduleName].IsAlive);
		}

		/// <summary>
		/// Generic command that always is success.
		/// It's used for get environment values
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void BbStatus(Command command)
		{
			SendResponse(command, true);
		}

		/// <summary>
		/// Executes a busy command.
		/// Asks the blackboard for the busy state of the requested module by the busy command
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void BusyCommand(Command command)
		{
			string moduleName = command.Parameters;
			if (!parent.Modules.Contains(moduleName))
				SendResponse(command, false);
			else
				SendResponse(command, parent.Modules[moduleName].Busy);
		}

		/// <summary>
		/// Executes a connected command.
		/// Asks the blackboard if the requested module is connected
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void ConnectedCommand(Command command)
		{
			string moduleName = command.Parameters;
			if (!parent.Modules.Contains(moduleName))
				SendResponse(command, false);
			else
				SendResponse(command, parent.Modules[moduleName].IsConnected);
		}

		/*
		/// <summary>
		/// Executes a create_var command.
		/// Requests the blackboard to create a variable with the specified name and size
		/// </summary>
		/// <param name="command">Command to execute</param>
		protected override void CreateVarCommand(Command command)
		{
			Match match = SharedVariable.RxCreateSharedVariableValidator.Match(command.Parameters);
			if (!match.Success)
			{
				SendResponse(command, false);
				return;
			}
			string varName = match.Result("${name}");
			int varSize;

			if (sharedVariables.Contains(varName))
			{
				SendResponse(command, true);
				return;
			}
			if (!String.IsNullOrEmpty(match.Result("${size}")))
				varSize = Int32.Parse(match.Result("${size}"));
			else
				varSize = 0;
			SharedVariable sv = new SharedVariable(this, varName, varSize);
			sv.AllowedWriters.Add("*");
			sharedVariables.Add(sv);
			SendResponse(command, true);

			if (pendingSubscriptions.ContainsKey(varName))
			{
				List<SharedVariableSubscription> svsList = pendingSubscriptions[varName];
				for (int i = 0; i < svsList.Count; ++i)
					sv.Subscriptions.Add(svsList[i]);
				pendingSubscriptions.Remove(varName);
			}
		}
		*/

		/// <summary>
		/// Executes a idletime command.
		/// Asks the blackboard for the idle time state of the requested module
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void IdleTimeCommand(Command command)
		{
			string moduleName = command.Parameters;
			if (!parent.Modules.Contains(moduleName))
			{
				SendResponse(command, false);
				return;
			}
			command.Parameters = moduleName + " " + parent.Modules[moduleName].IdleTime.TotalSeconds.ToString("0");
			SendResponse(command, parent.Modules[moduleName].IsConnected);
		}

		/// <summary>
		/// Executes a list_vars command.
		/// Requests the blackboard to send the list of shared variables
		/// </summary>
		/// <param name="command">Command to execute</param>
		protected override void ListVarsCommand(Command command)
		{
			command.Parameters = sharedVariables.NameList;
			SendResponse(command, true);
		}

		/// <summary>
		/// Executes a modules command.
		/// Asks the blackboard for the list of supported modules
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void ModulesCommand(Command command)
		{
			int i = 0;
			StringBuilder sb = new StringBuilder();
			for (i = 0; i < parent.Modules.Count -1; ++i)
			{
				sb.Append(parent.Modules[i].Name);
				sb.Append(' ');
			}
			sb.Append(parent.Modules[i].Name);
			command.Parameters = sb.ToString();
			SendResponse(command, true);
		}

		/// <summary>
		/// Executes a mstatus command.
		/// Asks the blackboard for the state of a connected machine by the mstatus command
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void MstatusCommand(Command command)
		{
			IPAddress ip;
			MachineStatus ms;
			Command cmd;

			if (!IPAddress.TryParse(command.Parameters, out ip) || msc.Contains(ip))
			{
				SendResponse(command, false);
				return;
			}
			ms = msc[ip];

			cmd = new Command(command.Source, command.Destination, command.Command, ms.ToString(), command.Id);
			SendResponse(cmd, true);
		}

		/// <summary>
		/// Executes a mystat command..
		/// Parses a mystat report.
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void MystatCommand(Command command)
		{
			MachineStatus status;

			if(!MachineStatus.TryParse(command.Parameters, out status))
				return;
			if (msc.Contains(status.IPAddress))
				msc[status.IPAddress] = status;
			else msc.Add(status);
		}

		/// <summary>
		/// Executes a querymodule command.
		/// Asks the blackboard for the whole state of the requested module
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void QueryModuleCommand(Command command)
		{
			string moduleName = command.Parameters;
			if (!parent.Modules.Contains(moduleName))
			{
				SendResponse(command, false);
				return;
			}
			StringBuilder sb = new StringBuilder();
			int i = 0;
			IModuleClient module = parent.Modules[moduleName];
			sb.Append(moduleName);
			sb.Append(" alive=");
			sb.Append(module.IsAlive);
			sb.Append(" bin=");
			sb.Append(module.Bin);
			sb.Append(" busy=");
			sb.Append(module.Busy);
			sb.Append(" connected=");
			sb.Append(module.IsConnected);
			if (module is ModuleClientTcp)
			{
				sb.Append(" endpoint=");
				sb.Append(((ModuleClientTcp)module).ServerAddresses.ToString());
				sb.Append(':');
				sb.Append(((ModuleClientTcp)module).Port.ToString());
			}
			sb.Append(" idletime=");
			sb.Append(module.IdleTime.TotalSeconds.ToString("0"));
			sb.Append(" ready=");
			sb.Append(module.Ready);
			sb.Append(" running=");
			sb.Append(module.IsRunning);
			if (module is ModuleClient)
			{
				sb.Append(" simulated=");
				sb.Append(((ModuleClient)module).Simulation != ModuleSimulationOptions.SimulationDisabled);
			}

			sb.Append(" supportedcommands=");
			sb.Append('{');
			for (i = 0; i < module.Prototypes.Count - 1; ++i)
			{
				sb.Append(module.Prototypes[i].Command);
				sb.Append(',');
			}
			sb.Append(module.Prototypes[i].Command);
			sb.Append('}');

			command.Parameters = sb.ToString();
			SendResponse(command, true);
		}

		/// <summary>
		/// Executes a reset command.
		/// Resets one of the blackboard options:
		///		blackboard - resets blackboard
		///		test - resets the test
		///		time - blackboard execution time
		///		module MODULE_NAME - resets specified module
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void ResetCommand(Command command)
		{
			string p0;
			string[] parts;
			char[] separator = {' ', '\t', '\r', '\n'};

			if (command.Parameters.Length < 0)
			{
				SendResponse(command, false);
				return;
			}
			parts = command.Parameters.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length < 1)
			{
				SendResponse(command, false);
				return;
			}
			p0 = parts[0];

			switch (p0.ToLower())
			{
				case "blackboard":
					SendResponse(command, true);
					parent.Restart();
					break;

				case "test":
					SendResponse(command, true);
					parent.RestartTest();
					break;

				case "time":
					SendResponse(command, true);
					parent.RestartTimer();
					break;

				case "module":
					if ((parts.Length < 2) || !parent.Modules.Contains(parts[1]) || !(parent.Modules[parts[1]] is ModuleClient))
						SendResponse(command, false);
					((ModuleClient)parent.Modules[parts[1]]).Restart();
					SendResponse(command, true);
					break;
			}
		}

		/// <summary>
		/// Executes a read_var command.
		/// Requests the blackboard to send the content of a stored a variable
		/// </summary>
		/// <param name="command">Command to execute</param>
		protected override void ReadVarCommand(Command command)
		{
			Match match = SharedVariable.RxVarNameValidator.Match(command.Parameters);
			if (!match.Success)
			{
				SendResponse(command, false);
				return;
			}
			string varName =command.Parameters;

			if (!sharedVariables.Contains(varName))
			{
				SendResponse(command, false);
				return;
			}

			command.Parameters = sharedVariables[varName].StringToSend;
			SendResponse(command, true);
		}

		/// <summary>
		/// Executes a ready command.
		/// Asks the blackboard for the ready state of the requested module by the ready command
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void ReadyCommand(Command command)
		{
			string moduleName = command.Parameters;
			if (!parent.Modules.Contains(moduleName))
				SendResponse(command, false);
			else
				SendResponse(command, parent.Modules[moduleName].Ready);
		}

		/// <summary>
		/// Executes a restart_test command.
		/// Requests the blackboard to send a restart_test command to the specified module
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void Reset_TestCommand(Command command)
		{
			string moduleName = command.Parameters;
			if (!parent.Modules.Contains(moduleName) || !(parent.Modules[moduleName] is ModuleClient))
			{
				SendResponse(command, false);
				return;
			}
			((ModuleClient)parent.Modules[moduleName]).RestartTest();
			SendResponse(command, true);
		}
		
		/// <summary>
		/// Executes a setupmodule command.
		/// Request the blackboard to change the connection settings of the given module
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void SetupModuleCommand(Command command)
		{
			IPAddress ip;
			int port;
			string moduleName;
			ModuleClientTcp module;
			Match m;
			bool running;

			m = rxSetupModule.Match(command.Parameters);
			if (!m.Success)
			{
				SendResponse(command, false);
				return;
			}
			if (!parent.Modules.Contains(moduleName = m.Result("${moduleName}")) || ((module = this.Parent.Modules[moduleName] as ModuleClientTcp) == null))
			{
				SendResponse(command, false);
				return;
			}

			if (!IPAddress.TryParse(m.Result("${ip}"), out ip))
			{
				SendResponse(command, false);
				return;
			}
			if (!Int32.TryParse(m.Result("${port}"), out port) || (port < 1024) || (port > 65535))
			{
				SendResponse(command, false);
				return;
			}

			if ((module.Port == port) && (module.ServerAddress == ip))
				SendResponse(command, true);

			if(running = module.IsRunning)
				module.Stop();
			module.Port = port;
			module.ServerAddresses.Add(ip);
			if (running)
				module.Start();
			SendResponse(command, true);
		}

		/// <summary>
		/// Executes a start_module_app command.
		/// Request the blackboard to run the specified application module.
		/// If the module is already connected, returns true.
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void StartModuleAppCommand(Command command)
		{
			string moduleName = command.Parameters;
			if (!parent.Modules.Contains(moduleName))
				SendResponse(command, false);
			else if (parent.Modules[moduleName].IsConnected)
				SendResponse(command, true);
			Thread thread = new Thread(new ThreadStart(delegate()
				{
					bool result;
					try
					{
						result = processManager.LaunchProcessIfNotRunning(parent.Modules[moduleName].ProcessInfo);
					}
					catch { result = false; }
					SendResponse(command, result);
				}));
			thread.IsBackground = true;
			thread.Start();
		}

		/// <summary>
		/// Executes a stop_module_app command.
		/// Request the blackboard to close the specified application module.
		/// If the module is not connected, returns true.
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void StopModuleAppCommand(Command command)
		{
			string moduleName = command.Parameters;
			if (!parent.Modules.Contains(moduleName))
				SendResponse(command, false);
			else if (!parent.Modules[moduleName].IsConnected)
				SendResponse(command, true);
			Thread thread = new Thread(new ThreadStart(delegate()
			{
				bool result;
				try
				{
					result = processManager.CloseThenKillProcess(parent.Modules[moduleName].ProcessInfo);
				}
				catch { result = false; }
				SendResponse(command, result);
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		/// <summary>
		/// Executes a subscribe_var command.
		/// Requests the blackboard to send a notification to the subscriber module when a shared variable is written
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void SubscribeVarCommand(Command command)
		{
			SharedVariableSubscription subscription;
			if (!SharedVariableSubscription.TryParse(command, out subscription))
			{
				SendResponse(command, false);
				return;
			}

			if (!sharedVariables.Contains(subscription.VariableName))
			{
				if (subscription.SubscriptionType == SharedVariableSubscriptionType.Creation)
				{
					string varName = subscription.VariableName;
					if (!pendingSubscriptions.ContainsKey(varName))
						pendingSubscriptions.Add(varName, new List<SharedVariableSubscription>());
					if (!pendingSubscriptions[varName].Contains(subscription))
						pendingSubscriptions[varName].Add(subscription);
					SendResponse(command, true);
				}
				else
				{
					SendResponse(command, false);
					return;
				}
			}
			else
			{
				sharedVariables[subscription.VariableName].Subscriptions.Add(subscription);
				SendResponse(command, true);
			}			
		}

		/// <summary>
		/// Gets all the information related to a shared variable
		/// </summary>
		/// <param name="command"></param>
		private void StatVarCommand(Command command)
		{
			Robotics.API.SharedVariableInfo svInfo;
			string serialized;
			string variableName = command.Parameters;
			if (!sharedVariables.Contains(variableName))
			{
				SendResponse(command, false);
				return;
			}

			svInfo = sharedVariables[variableName].GetInfo();
			if (!Robotics.API.SharedVariableInfo.Serialize(svInfo, out serialized))
			{
				SendResponse(command, false);
				return;
			}
			command.Parameters = serialized;
			SendResponse(command, true);
		}

		/// <summary>
		/// Executes a unsubscribe_var command.
		/// Requests the blackboard to do not notify when a shared variable is written
		/// </summary>
		/// <param name="command">Command to execute</param>
		private void UnsubscribeVarCommand(Command command)
		{
			string variableName = command.Parameters;
			if (!sharedVariables.Contains(variableName))
			{
				SendResponse(command, false);
				return;
			}

			sharedVariables[variableName].Subscriptions.Remove(variableName);
			SendResponse(command, true);
		}

		/*

		/// <summary>
		/// Executes a write_var command.
		/// Requests the blackboard to write to a stored a variable the content provided
		/// </summary>
		/// <param name="command">Command to execute</param>
		protected override void WriteVarCommand(Command command)
		{
			Match match;
			string varName;
			string varData;
			bool result;

			match = SharedVariable.RxSharedVariableValidator.Match(command.Parameters);
			if (!match.Success)
			{
				SendResponse(command, false);
				return;
			}
			varName = match.Result("${name}");
			varData = match.Result("${data}");
			command.Parameters = varName;
			if (!sharedVariables.Contains(varName))
			{
				SendResponse(command, false);
				return;
			}
			result = sharedVariables[varName].WriteStringData(command.Source, varData);
			SendResponse(command, result);
		}
		*/

		#endregion

		#region Thread Control Methods

		/// <summary>
		/// Asynchronously launches the applications specified in the blackboad configuration file
		/// </summary>
		private void AppLauncherThreadTask()
		{
			this.ExecuteStartupActions();
		}

		/// <summary>
		/// Thread for autoconnect to remote application
		/// </summary>
		// /// <param name="sender">Unused</param>
		protected override void MainThreadTask()
		{
			Thread thisThread;
			StatusChangedEH busyEH;
			StatusChangedEH readyEH;
			ModuleConnectionEH connectedStatusEH;

			stopMainThread = false;
			running = true;
			Busy = false;
			IsAlive = true;
			Ready = true;

			#region Setup EventHandlers

			busyEH = new StatusChangedEH(module_BusyChanged);
			readyEH = new StatusChangedEH(module_ReadyChanged);
			connectedStatusEH = new ModuleConnectionEH(module_ConnectedStatusChanged);
			if (this.Parent != null)
			{
				foreach (IModuleClient module in Parent.Modules)
				{
					if (module == this)
						continue;
					module.BusyChanged += busyEH;
					module.ReadyChanged += readyEH;
					if (module is ModuleClient)
					{
						((ModuleClient)module).Connected += connectedStatusEH;
						((ModuleClient)module).Disconnected += connectedStatusEH;
					}
				}
			}

			#endregion

			// Fills out MachineStatuss List
			FillMachineStatussList();

			while (running && !stopMainThread)
			{
				// Parse received data
				ParsePendingCommands();
			}

			#region Clear EventHandlers

			if (this.Parent != null)
			{
				foreach (IModuleClient module in Parent.Modules)
				{
					if (module == this)
						continue;
					module.BusyChanged -= busyEH;
					module.ReadyChanged -= readyEH;
					if (module is ModuleClient)
					{
						((ModuleClient)module).Connected -= connectedStatusEH;
						((ModuleClient)module).Disconnected -= connectedStatusEH;
					}
				}
			}

			#endregion

			running = false;
			OnStopped();
			OnStatusChanged();

			#region Stop and Clear Thread

			thisThread = mainThread;
			mainThread = null;

			#endregion
		}

		/// <summary>
		/// Does nothing (internal module, no connection required)
		/// </summary>
		protected override void MainThreadDisconnect()
		{
		}

		/// <summary>
		/// Does nothing (internal module, no connection required)
		/// </summary>
		protected override void MainThreadFirstTimeConnect()
		{
		}

		/// <summary>
		/// Does nothing (internal module, no connection required)
		/// </summary>
		protected override void MainThreadLoopAutoConnect()
		{
		}

		/// <summary>
		/// Initializes the thread for autoconnection to ModuleServer
		/// If the server is running starts the thread automatically
		/// </summary>
		protected override void SetupMainThread()
		{
			/*
			if (mainThread != null) try { mainThread.Abort(); }
				catch { }
			if (simOptions == ModuleClientSimulationOptions.SimulationDisabled) mainThread = new Thread(new ThreadStart(MainThreadTask));
			else mainThread = new Thread(new ThreadStart(MainSimulatedThread));
			mainThread.IsBackground = true;
			if (running) mainThread.Start();
			*/
			if ((mainThread != null) && mainThread.IsAlive)
			{
				stopMainThread = true;
				mainThread.Join(100);
				if (mainThread.IsAlive)
					mainThread.Abort();
				stopMainThread = false;
			}
			mainThread = new Thread(new ThreadStart(MainThreadTask));
			mainThread.IsBackground = true;
			if (running) mainThread.Start();
		}

		/// <summary>
		/// Initializes and starts the application launcher thread
		/// </summary>
		private void StartAppLauncherThread()
		{
			if (this.startupActions.Count == 0)
				return;

			if ((appLauncherThread != null) && appLauncherThread.IsAlive)
			{
				appLauncherThread.Join(1000);
				if (appLauncherThread.IsAlive)
					appLauncherThread.Abort();
			}
			appLauncherThread = new Thread(new ThreadStart(AppLauncherThreadTask));
			appLauncherThread.IsBackground = true;
			if (running) appLauncherThread.Start();
		}

		#endregion

		#endregion

		#region Event Handlers

		/// <summary>
		/// Manages the changes on the busy status of a module to update shared variables.
		/// </summary>
		/// <param name="sender">The module which Ready status changed</param>
		private void module_BusyChanged(IModuleClient sender)
		{
			if (sharedVariables.Contains("busy"))
				sharedVariables["busy"].ReportSubscribers(sender);
		}

		/// <summary>
		/// Manages the changes on the IsConncted status of a module to update shared variables.
		/// </summary>
		/// <param name="sender">The module which its IsConnected status changed</param>
		private void module_ConnectedStatusChanged(ModuleClient sender)
		{
			if (sharedVariables.Contains("connected"))
				sharedVariables["connected"].ReportSubscribers(sender);
			if (!sender.IsConnected)
			{
				for(int i = 0; i < sharedVariables.Count; ++i)
				{
					this.sharedVariables[i].Subscriptions.Remove(sender);
				}
			}
		}

		/// <summary>
		/// Manages the changes on the ready status of a module to update shared variables.
		/// </summary>
		/// <param name="sender">The module which Ready status changed</param>
		private void module_ReadyChanged(IModuleClient sender)
		{
			if (sharedVariables.Contains("ready"))
			sharedVariables["ready"].ReportSubscribers(sender);
		}

		#endregion

		#region Methods de Clase (Estáticos)
		#endregion

	}
}

