using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Blk.Engine;
using Robotics;
using Robotics.Sockets;
using Robotics.Utilities;
using Blk.Api;

namespace Blk.Engine.Remote
{
	/// <summary>
	/// Manages local and remote program execution/termination
	/// </summary>
	public class ModuleProcessManager
	{
		#region Constants

		/// <summary>
		/// Number of attempts to kill a process
		/// </summary>
		public const int KILL_ATTEMPTS = 10;

		#endregion

		#region Variables

		/// <summary>
		/// Object used to dump log
		/// </summary>
		protected readonly LogWriter log;

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
		/// Initialzes a new instance of the RemoteModuleProcessManager object
		/// </summary>
		/// <param name="log">Object used to dump log</param>
		public ModuleProcessManager(LogWriter log)
		{
			this.log = log;
			this.oLock = new Object();
			this.processManager = new ProcessManager(log);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the blackboard object to which this instance belongs to
		/// </summary>
		private LogWriter Log
		{
			get { return this.log; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Kills a module
		/// </summary>
		/// <param name="mc">The IModuleClientTcp object which contains the information about the module to kill</param>
		private bool KillLocalModule(IModuleClientTcp mc)
		{
			return processManager.CloseThenKillProcess(mc.ProcessInfo);
		}

		/// <summary>
		/// Checks if the process of the specified module is running
		/// </summary>
		/// <param name="module">The module to check</param>
		/// <returns>The number of running instances of the module</returns>
		public int CheckModule(IModuleClientTcp module)
		{
			lock (oLock)
			{
				if (!module.IsLocal)
				{
					return RemoteCheck(module);
				}
				return processManager.RunningProcessesCount(module.ProcessInfo);
			}
		}

		/// <summary>
		/// Closes a module
		/// </summary>
		/// <param name="mc">The IModuleClientTcp object which contains the information about the module to close</param>
		private bool CloseLocalModule(IModuleClientTcp mc)
		{
			return processManager.CloseProcessWindow(mc.ProcessInfo);
		}

		/// <summary>
		/// Launches a module
		/// </summary>
		/// <param name="mc">The IModuleClientTcp object which contains the information about the module to launch</param>
		private bool LaunchLocalModule(IModuleClientTcp mc)
		{
			return processManager.LaunchProcess(mc.ProcessInfo);
		}

		/// <summary>
		/// Launches a module if it is not running
		/// </summary>
		/// <param name="mc">The IModuleClientTcp object which contains the information about the module to launch</param>
		private bool LaunchLocalModuleIfNotRunning(IModuleClientTcp mc)
		{
			return processManager.LaunchProcessIfNotRunning(mc.ProcessInfo);
		}

		/// <summary>
		/// Starts the specified module
		/// </summary>
		/// <param name="module">The module to start</param>
		/// <param name="method">The method used for startup</param>
		public void LaunchModule(IModuleClientTcp module, ModuleStartupMethod method)
		{
			if ((method == ModuleStartupMethod.None) || (module == null) || (module.ProcessInfo == null))
				return;

			lock (oLock)
			{
				if (!module.IsLocal)
				{
					// bool result = RemoteStartup(module, method);
					//if (result && (method == ModuleStartupMethod.LaunchAndWaitReady))
					//	WaitModuleReady(module);
					RemoteStartup(module, method);
					return;
				}

				switch (method)
				{
					case ModuleStartupMethod.LaunchAlways:
						LaunchLocalModule(module);
						break;

					case ModuleStartupMethod.LaunchAndWaitReady:
						//if (KillModule(module) && LaunchModule(module))
						//	WaitModuleReady(module);
						KillLocalModule(module);
						LaunchLocalModule(module);
						break;

					case ModuleStartupMethod.LaunchIfNotRunning:
						LaunchLocalModuleIfNotRunning(module);
						break;

					case ModuleStartupMethod.KillThenLaunch:
						if (KillLocalModule(module))
							LaunchLocalModule(module);
						break;

					case ModuleStartupMethod.KillOnly:
						KillLocalModule(module);
						break;
				}
			}
		}

		/// <summary>
		/// Request the number of running instances of a module on remote computer
		/// </summary>
		/// <param name="mc">The module to check</param>
		/// <returns>The number of running instances of the module</returns>
		private int RemoteCheck(IModuleClientTcp mc)
		{
			RemoteCheckRequest request;
			RemoteCheckResponse response;
			TcpClient client;
			AutoResetEvent dataReceivedEvent;
			string serialized;

			Log.WriteLine(5, "Checking instances of module '" + mc.Name + "': on remote computer.");
			client = null;
			foreach (IPAddress ip in mc.ServerAddresses)
			{
				client = new TcpClient(ip, 2300);
				if (client.TryConnect())
					break;
			}
			if ((client == null) || !client.IsConnected)
			{
				Log.WriteLine(5, "Can not check module '" + mc.Name + "': unable to connect to remote computer.");
				return -1;
			}

			dataReceivedEvent = new AutoResetEvent(false);
			client.DataReceived += new EventHandler<TcpClient, TcpPacket>(delegate(TcpClient c, TcpPacket packet)
			{
				response = RemoteCheckResponse.FromXml(UTF8Encoding.UTF8.GetString(packet.Data));
				dataReceivedEvent.Set();
			});

			try
			{
				request = new RemoteCheckRequest(mc.Name, mc.ProcessInfo);
				serialized = RemoteCheckRequest.ToXml(request);
				client.Send(serialized);
				response = null;
				dataReceivedEvent.WaitOne(10000);
				if (response == null)
				{
					Log.WriteLine(5, "Can't check module '" + mc.Name + "': no response received");
					client.Disconnect();
					return -1;
				}
				if (response.ModuleName != request.ModuleName)
				{
					Log.WriteLine(5, "Can't check module '" + mc.Name + "': invalid response");
					client.Disconnect();
					return -1;
				}
				if (!response.Success)
				{
					Log.WriteLine(5, "Can't check module '" + mc.Name + "': " + response.Message);
					client.Disconnect();
					return -1;
				}
				Log.WriteLine(5, "Check module '" + mc.Name + "': Success");
				client.Disconnect();
				return response.Instances;
			}
			catch (Exception ex)
			{
				Log.WriteLine(5, "Can't check module '" + mc.Name + "': " + ex.Message);
				return -1;
			}
			finally
			{
				if ((client != null) && client.IsConnected)
					client.Disconnect();
			}
		}

		/// <summary>
		/// Request to execute the module startup on remote computer
		/// </summary>
		/// <param name="mc">The module to start</param>
		/// <param name="method">The startup sequence method</param>
		private bool RemoteShutdown(IModuleClientTcp mc, ModuleShutdownMethod method)
		{
			RemoteShutdownRequest request;
			RemoteShutdownResponse response;
			TcpClient client;
			AutoResetEvent dataReceivedEvent;
			string serialized;

			Log.WriteLine(5, "Shutting down module '" + mc.Name + "': on remote computer.");
			client = null;
			foreach (IPAddress ip in mc.ServerAddresses)
			{
				client = new TcpClient(ip, 2300);
				if (client.TryConnect())
					break;
			}
			if ((client == null) || !client.IsConnected)
			{
				Log.WriteLine(5, "Can not shutdown module '" + mc.Name + "': unable to connect to remote computer.");
				return false;
			}

			dataReceivedEvent = new AutoResetEvent(false);
			client.DataReceived += new EventHandler<TcpClient, TcpPacket>(delegate(TcpClient c, TcpPacket packet)
			{
				response = RemoteShutdownResponse.FromXml(UTF8Encoding.UTF8.GetString(packet.Data));
				dataReceivedEvent.Set();
			});

			try
			{
				request = new RemoteShutdownRequest(mc.Name, method, mc.ProcessInfo);
				serialized = RemoteShutdownRequest.ToXml(request);
				client.Send(serialized);
				response = null;
				dataReceivedEvent.WaitOne(10000);
				if (response == null)
				{
					Log.WriteLine(5, "Can not shutdown module '" + mc.Name + "': no response received");
					client.Disconnect();
					return false;
				}
				if ((response.ModuleName != request.ModuleName) ||
					(response.Method != request.Method))
				{
					Log.WriteLine(5, "Can not shutdown module '" + mc.Name + "': invalid response");
					client.Disconnect();
					return false;
				}
				if (!response.Success)
				{
					Log.WriteLine(5, "Can not shutdown module '" + mc.Name + "': " + response.Message);
					client.Disconnect();
					return false;
				}
				Log.WriteLine(5, "Shutdown module '" + mc.Name + "': Success");
				client.Disconnect();
				return true;
			}
			catch (Exception ex)
			{
				Log.WriteLine(5, "Can not shutdown module '" + mc.Name + "': " + ex.Message);
				return false;
			}
			finally
			{
				if ((client != null) && client.IsConnected)
					client.Disconnect();
			}
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

			Log.WriteLine(5, "Starting module '" + mc.Name + "': on remote computer.");
			client = null;
			foreach (IPAddress ip in mc.ServerAddresses)
			{
				client = new TcpClient(ip, 2300);
				if (client.TryConnect())
					break;
			}
			if ((client == null) || !client.IsConnected)
			{
				Log.WriteLine(5, "Can not start module '" + mc.Name + "': unable to connect to remote computer.");
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
				if (response == null)
				{
					Log.WriteLine(5, "Can not start module '" + mc.Name + "': no response received");
					client.Disconnect();
					return false;
				}
				if ((response.ModuleName != request.ModuleName) ||
					(response.Method != request.Method))
				{
					Log.WriteLine(5, "Can not start module '" + mc.Name + "': invalid response");
					client.Disconnect();
					return false;
				}
				if (!response.Success)
				{
					Log.WriteLine(5, "Can not start module '" + mc.Name + "': " + response.Message);
					client.Disconnect();
					return false;
				}
				Log.WriteLine(5, "Start module '" + mc.Name + "': Success");
				client.Disconnect();
				return true;
			}
			catch (Exception ex)
			{
				Log.WriteLine(5, "Can not start module '" + mc.Name + "': " + ex.Message);
				return false;
			}
			finally
			{
				if ((client != null) && client.IsConnected)
					client.Disconnect();
			}
		}

		/// <summary>
		/// Shutsdown the specified module
		/// </summary>
		/// <param name="module">The module to shutdown</param>
		/// <param name="method">The method used for startup</param>
		public void ShutdownModule(IModuleClientTcp module, ModuleShutdownMethod method)
		{
			if ((method == ModuleShutdownMethod.None) || (module == null) || (module.ProcessInfo == null))
				return;

			lock (oLock)
			{
				Log.WriteLine(4, "Executing shutdown sequence...");
				if (!module.IsLocal)
				{
					// bool result = RemoteShutdown(module, method);
					RemoteShutdown(module, method);
					return;
				}

				switch (method)
				{
					case ModuleShutdownMethod.CloseOnly:
						CloseLocalModule(module);
						break;

					case ModuleShutdownMethod.KillOnly:
						KillLocalModule(module);
						break;

					case ModuleShutdownMethod.CloseThenKill:
						if (!CloseLocalModule(module))
							KillLocalModule(module);
						break;
				}
				Log.WriteLine(4, "Shutdown sequence excecution complete!");
			}
		}

		#endregion
	}
}
