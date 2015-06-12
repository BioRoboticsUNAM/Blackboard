using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Robotics;
using Robotics.Sockets;
using Robotics.Utilities;
using Blk.Api;

namespace Blk.Engine.Remote
{
	/// <summary>
	/// Indicates how the modules will be launched at blackboard startup
	/// </summary>
	public enum ModuleStartupMethod
	{
		/// <summary>
		/// No startup sequence is realized
		/// </summary>
		None = 0,
		/// <summary>
		/// All modules are launched without verification
		/// </summary>
		LaunchAlways = 1,
		/// <summary>
		/// Existing processes are killed, then a new one is launched
		/// </summary>
		KillThenLaunch = 2,
		/// <summary>
		/// Existing processes are killed, then a new one is launched. After the module is launched, the blackboard will wait until this module is ready before launch the next
		/// </summary>
		LaunchAndWaitReady = 3,
		/// <summary>
		/// Kills all existing processes
		/// </summary>
		KillOnly = 4,
		/// <summary>
		/// If the process is not running a new one is launched
		/// </summary>
		LaunchIfNotRunning = 5
	}

	/// <summary>
	/// Executes the module programs specified in a blackboard configuration file
	/// </summary>
	public class StartupSequenceManager
	{
		#region Variables

		/// <summary>
		/// Stores the method of startup sequence for modules
		/// </summary>
		private ModuleStartupMethod method;

		/// <summary>
		/// Stores the program startup sequence for the modules
		/// </summary>
		private List<string> moduleSequence;

		/// <summary>
		/// The blackboard object to which this instance belongs to
		/// </summary>
		private readonly Blackboard parent;

		/// <summary>
		/// The main thread used for async exection
		/// </summary>
		private Thread mainThread;

		/// <summary>
		/// Object used to allow only one thread to access the methods at a time
		/// </summary>
		private object oLock;

		/// <summary>
		/// Launch, kills and manages processes
		/// </summary>
		private ProcessManager processManager;

		#endregion

		#region Constructors

		/// <summary>
		/// Initialzes a new instance of the StartupSequenceExecuter object
		/// </summary>
		/// <param name="parent"></param>
		public StartupSequenceManager(Blackboard parent)
		{
			this.parent = parent;
			this.method = ModuleStartupMethod.KillThenLaunch;
			this.moduleSequence = new List<string>(20);
			this.oLock = new Object();
			this.processManager = new ProcessManager(parent.Log);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the method of startup sequence for modules
		/// </summary>
		public ModuleStartupMethod Method
		{
			get { return this.method; }
			set { this.method = value; }
		}

		/// <summary>
		/// Gets the startup sequence of the modules
		/// </summary>
		public List<string> ModuleSequence
		{
			get { return this.moduleSequence; }
		}

		/// <summary>
		/// Gets the blackboard object to which this instance belongs to
		/// </summary>
		private Blackboard Parent
		{
			get { return this.parent; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Launches a module if it is not running
		/// </summary>
		/// <param name="mc">The IModuleClientTcp object which contains the information about the module to launch</param>
		private bool LaunchIfNotRunning(IModuleClientTcp mc)
		{
			return processManager.LaunchProcessIfNotRunning(mc.ProcessInfo);
		}

		/// <summary>
		/// Launches a module
		/// </summary>
		/// <param name="mc">The IModuleClientTcp object which contains the information about the module to launch</param>
		private bool LaunchModule(IModuleClientTcp mc)
		{
			return processManager.LaunchProcess(mc.ProcessInfo);
		}

		/// <summary>
		/// Kills a module
		/// </summary>
		/// <param name="mc">The IModuleClientTcp object which contains the information about the module to kill</param>
		private bool KillModule(IModuleClientTcp mc)
		{
			return processManager.CloseThenKillProcess(mc.ProcessInfo);
		}

		/// <summary>
		/// Request to execute the module startup on remote computer
		/// </summary>
		/// <param name="mc">The module to start</param>
		/// <param name="method">The startup sequence method</param>
		private bool RemoteStartup(IModuleClientTcp mc, ModuleStartupMethod method)
		{
			RemoteStartupRequest request;
			RemoteStartupResponse response;
			TcpClient client;
			AutoResetEvent dataReceivedEvent;
			string serialized;

			Parent.Log.WriteLine(5, "Starting module '" + mc.Name + "': on remote computer.");
			client = null;
			foreach (IPAddress ip in mc.ServerAddresses)
			{
				client = new TcpClient(ip, 2300);
				if (client.TryConnect())
					break;
			}
			if ((client == null) || !client.IsConnected)
			{
				Parent.Log.WriteLine(5, "Can not start module '" + mc.Name + "': unable to connect to remote computer.");
				return false;
			}

			dataReceivedEvent = new AutoResetEvent(false);
			client.DataReceived += new EventHandler<TcpClient, TcpPacket>(delegate(TcpClient c, TcpPacket packet)
				{
					response = RemoteStartupResponse.FromXml(UTF8Encoding.UTF8.GetString(packet.Data));
					dataReceivedEvent.Set();
				});

			try
			{
				request = new RemoteStartupRequest(mc.Name, method, mc.ProcessInfo);
				serialized = RemoteStartupRequest.ToXml(request);
				client.Send(serialized);
				response = null;
				dataReceivedEvent.WaitOne(10000);
				if(response == null)
				{
					Parent.Log.WriteLine(5, "Can not start module '" + mc.Name + "': no response received");
					client.Disconnect();
					return false;
				}
				if ((response.ModuleName != request.ModuleName) ||
					(response.Method != request.Method))
				{
					Parent.Log.WriteLine(5, "Can not start module '" + mc.Name + "': invalid response");
					client.Disconnect();
					return false;
				}
				if(!response.Success)
				{
					Parent.Log.WriteLine(5, "Can not start module '" + mc.Name + "': " + response.Message);
					client.Disconnect();
					return false;
				}
				Parent.Log.WriteLine(5, "Start module '" + mc.Name + "': Success");
				client.Disconnect();
				return true;
			}
			catch (Exception ex)
			{
				Parent.Log.WriteLine(5, "Can not start module '" + mc.Name + "': " + ex.Message);
				return false;
			}
			finally
			{
				if((client != null) && client.IsConnected)
				client.Disconnect();
			}
		}

		/// <summary>
		/// Executes the startup sequence of the registered modules with the default method
		/// </summary>
		public void StartModules()
		{
			StartModules(this.method);
		}

		/// <summary>
		/// Executes the startup sequence of the registered modules with the specified method
		/// </summary>
		/// <param name="method">The method used for startup</param>
		private void StartModules(object method)
		{
			StartModules((ModuleStartupMethod)method);
		}

		/// <summary>
		/// Executes the startup sequence of the registered modules with the specified method
		/// </summary>
		/// <param name="method">The method used for startup</param>
		public void StartModules(ModuleStartupMethod method)
		{
			IModuleClientTcp mc;
			bool result;

			if ((method == ModuleStartupMethod.None) || (moduleSequence.Count < 1))
				return;

			lock (oLock)
			{
				Parent.Log.WriteLine(4, "Executing startup sequence...");

				for (int i = 0; i < moduleSequence.Count && parent.IsRunning; ++i)
				{
					if (!Parent.Modules.Contains(moduleSequence[i]))
					{
						Parent.Log.WriteLine(5, "Can not start module '" + moduleSequence[i] + "': The module does not exist.");
						continue;
					}
					mc = Parent.Modules[moduleSequence[i]] as IModuleClientTcp;
					if(!mc.Enabled)
					{
						Parent.Log.WriteLine(5, "Can not start module '" + moduleSequence[i] + "': The module is disabled.");
						continue;
					}
					if (mc == null) continue;
					if (!mc.IsLocal)
					//if (true)
					{
						//Parent.Log.WriteLine(5, "Can not start module '" + moduleSequence[i] + "': unable to start remote process.");
						result = RemoteStartup(mc, method);
						if (result && (method == ModuleStartupMethod.LaunchAndWaitReady))
							WaitModuleReady(mc);
						continue;
					}

					switch (method)
					{
						case ModuleStartupMethod.LaunchAlways:
							LaunchModule(mc);
							break;

						case ModuleStartupMethod.LaunchAndWaitReady:
							if (KillModule(mc) && LaunchModule(mc))
								WaitModuleReady(mc);
							break;

						case ModuleStartupMethod.LaunchIfNotRunning:
							LaunchIfNotRunning(mc);
							break;

						case ModuleStartupMethod.KillThenLaunch:
							if (KillModule(mc))
								LaunchModule(mc);
							break;

						case ModuleStartupMethod.KillOnly:
							KillModule(mc);
							break;
					}

				}
				Parent.Log.WriteLine(4, "Startup sequence excecution complete!");
			}
		}

		/// <summary>
		/// Executes the startup sequence of the registered modules with the default method
		/// </summary>
		public void StartModulesAsync()
		{
			StartModulesAsync(this.method);
		}

		/// <summary>
		/// Executes the startup sequence of the registered modules with the specified method
		/// </summary>
		/// <param name="method">The method used for startup</param>
		public void StartModulesAsync(ModuleStartupMethod method)
		{
			if ((this.mainThread != null) && this.mainThread.IsAlive)
				return;

			this.mainThread = new Thread(new ParameterizedThreadStart(StartModules));
			this.mainThread.IsBackground = true;
			this.mainThread.Start(method);
		}

		/// <summary>
		/// Kills all modules
		/// </summary>
		public void StopModules()
		{
			lock (oLock)
			{

			}
		}

		/*
		/// <summary>
		/// Kills all modules
		/// </summary>
		public void StopModules()
		{
			lock (oLock)
			{

			}
		}
		*/

		/// <summary>
		/// Waits untill a module becomes ready
		/// </summary>
		/// <param name="mc">The IModuleClientTcp object which contains the information about the module to wait for</param>
		private void WaitModuleReady(IModuleClientTcp mc)
		{
			Parent.Log.WriteLine(6, "Waiting for '" + mc.Name + "' to be ready.");
			do
			{
				mc.WaitReady(500);
			} while (parent.IsRunning && !mc.Ready);
			Parent.Log.WriteLine(6, "IModuleClientTcp '" + mc.Name + "' is ready.");
		}

		#endregion
	}
}
