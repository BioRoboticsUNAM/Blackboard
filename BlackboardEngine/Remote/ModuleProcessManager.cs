using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Blk.Engine;
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
			bool result;

			if ((method == ModuleStartupMethod.None) || (module == null) || (module.ProcessInfo == null))
				return;

			lock (oLock)
			{
				if (!module.IsLocal)
				{
					result = RemoteStartup(module, method);
					//if (result && (method == ModuleStartupMethod.LaunchAndWaitReady))
					//	WaitModuleReady(module);
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
		/// Request to execute the module startup on remote computer
		/// </summary>
		/// <param name="mc">The module to start</param>
		/// <param name="method">The startup sequence method</param>
		private bool RemoteShutdown(IModuleClientTcp mc, ModuleShutdownMethod method)
		{
			RemoteShutdownRequest request;
			RemoteShutdownResponse response;
			SocketTcpClient client;
			AutoResetEvent dataReceivedEvent;
			string serialized;

			Log.WriteLine(5, "Shutting down module '" + mc.Name + "': on remote computer.");
			client = null;
			foreach (IPAddress ip in mc.ServerAddresses)
			{
				client = new SocketTcpClient(ip, 2300);
				if (client.TryConnect())
					break;
			}
			if ((client == null) || !client.IsConnected)
			{
				Log.WriteLine(5, "Can not shutdown module '" + mc.Name + "': unable to connect to remote computer.");
				return false;
			}

			dataReceivedEvent = new AutoResetEvent(false);
			client.DataReceived += new TcpDataReceivedEventHandler(delegate(TcpPacket packet)
			{
				response = RemoteShutdownResponse.FromXml(packet.DataString);
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
				if (client.Socket != null)
					client.Socket.Close();
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
			SocketTcpClient client;
			AutoResetEvent dataReceivedEvent;
			string serialized;

			Log.WriteLine(5, "Starting module '" + mc.Name + "': on remote computer.");
			client = null;
			foreach (IPAddress ip in mc.ServerAddresses)
			{
				client = new SocketTcpClient(ip, 2300);
				if (client.TryConnect())
					break;
			}
			if ((client == null) || !client.IsConnected)
			{
				Log.WriteLine(5, "Can not start module '" + mc.Name + "': unable to connect to remote computer.");
				return false;
			}

			dataReceivedEvent = new AutoResetEvent(false);
			client.DataReceived += new TcpDataReceivedEventHandler(delegate(TcpPacket packet)
			{
				response = RemoteStartupResponse.FromXml(packet.DataString);
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
				if (client.Socket != null)
					client.Socket.Close();
			}
		}

		/// <summary>
		/// Shutsdown the specified module
		/// </summary>
		/// <param name="module">The module to shutdown</param>
		/// <param name="method">The method used for startup</param>
		public void ShutdownModule(IModuleClientTcp module, ModuleShutdownMethod method)
		{
			bool result;

			if ((method == ModuleShutdownMethod.None) || (module == null) || (module.ProcessInfo == null))
				return;

			lock (oLock)
			{
				Log.WriteLine(4, "Executing shutdown sequence...");
				if (!module.IsLocal)
				{
					result = RemoteShutdown(module, method);
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
