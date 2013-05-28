using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Robotics;
using Robotics.Utilities;
using Blk.Api;

namespace Blk.Engine.Remote
{
	/// <summary>
	/// Indicates how the modules will be launched at blackboard startup
	/// </summary>
	public enum ModuleShutdownMethod
	{
		/// <summary>
		/// No startup sequence is realized
		/// </summary>
		None = 0,
		/// <summary>
		/// Requests the main window to close
		/// </summary>
		CloseOnly = 1,
		/// <summary>
		/// Kills all existing processes
		/// </summary>
		KillOnly = 2,
		/// <summary>
		/// Requests the main window to close, if application does not end, the process is killed
		/// </summary>
		CloseThenKill = 3
	}


	/// <summary>
	/// Closes the module programs specified in a blackboard configuration file
	/// </summary>
	public class ShutdownSequenceManager
	{
		#region Variables

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
		/// Stores the method of shutdown for modules
		/// </summary>
		public ModuleShutdownMethod method;

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
		/// Initialzes a new instance of the ShutdownSequenceManager object
		/// </summary>
		/// <param name="parent"></param>
		public ShutdownSequenceManager(Blackboard parent)
		{
			this.parent = parent;
			this.method = ModuleShutdownMethod.CloseThenKill;
			this.moduleSequence = new List<string>(20);
			this.oLock = new Object();
			this.processManager = new ProcessManager(parent.Log);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the method of shutdown for modules
		/// </summary>
		public ModuleShutdownMethod Method
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
		/// Closes a module
		/// </summary>
		/// <param name="mc">The IModuleClientTcp object which contains the information about the module to close</param>
		private bool CloseModule(IModuleClientTcp mc)
		{
			return processManager.CloseProcessWindow(mc.ProcessInfo);
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
		private bool RemoteShutdown(IModuleClientTcp mc, ModuleShutdownMethod method)
		{
			RemoteShutdownRequest request;
			RemoteShutdownResponse response;
			SocketTcpClient client;
			AutoResetEvent dataReceivedEvent;
			string serialized;

			Parent.Log.WriteLine(3, "Shutting down module '" + mc.Name + "': on remote computer.");
			client = null;
			foreach (IPAddress ip in mc.ServerAddresses)
			{
				client = new SocketTcpClient(ip, 2300);
				if (client.TryConnect())
					break;
			}
			if ((client == null) || !client.IsConnected)
			{
				Parent.Log.WriteLine(3, "Can not shutdown module '" + mc.Name + "': unable to connect to remote computer.");
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
				if(response == null)
				{
					Parent.Log.WriteLine(5, "Can not shutdown module '" + mc.Name + "': no response received");
					client.Disconnect();
					return false;
				}
				if ((response.ModuleName != request.ModuleName) ||
					(response.Method != request.Method))
				{
					Parent.Log.WriteLine(5, "Can not shutdown module '" + mc.Name + "': invalid response");
					client.Disconnect();
					return false;
				}
				if(!response.Success)
				{
					Parent.Log.WriteLine(5, "Can not shutdown module '" + mc.Name + "': " + response.Message);
					client.Disconnect();
					return false;
				}
				Parent.Log.WriteLine(5, "Shutdown module '" + mc.Name + "': Success");
				client.Disconnect();
				return true;
			}
			catch (Exception ex)
			{
				Parent.Log.WriteLine(5, "Can not shutdown module '" + mc.Name + "': " + ex.Message);
				return false;
			}
			finally
			{
				if((client != null) && client.IsConnected)
				client.Disconnect();
				if (client.Socket != null)
					client.Socket.Close();
			}
		}

		/// <summary>
		/// Executes the startup sequence of the registered modules with the default method
		/// </summary>
		public void ShutdownModules()
		{
			ShutdownModules(this.method);
		}

		/// <summary>
		/// Executes the startup sequence of the registered modules with the specified method
		/// </summary>
		/// <param name="method">The method used for startup</param>
		private void ShutdownModules(object method)
		{
			ShutdownModules((ModuleShutdownMethod)method);
		}

		/// <summary>
		/// Executes the startup sequence of the registered modules with the specified method
		/// </summary>
		/// <param name="method">The method used for startup</param>
		public void ShutdownModules(ModuleShutdownMethod method)
		{
			IModuleClientTcp mc;
			bool result;

			if ((method == ModuleShutdownMethod.None) || (moduleSequence.Count < 1))
				return;

			lock (oLock)
			{
				Parent.Log.WriteLine(4, "Executing shutdown sequence...");

				for (int i = 0; i < moduleSequence.Count && (parent.RunningStatus == BlackboardRunningStatus.Stopping); ++i)
				{
					if (!Parent.Modules.Contains(moduleSequence[i]))
					{
						Parent.Log.WriteLine(5, "Can not shutdown module '" + moduleSequence[i] + "': The module does not exist.");
						continue;
					}
					mc = Parent.Modules[moduleSequence[i]] as IModuleClientTcp;
					if(!mc.Enabled)
					{
						Parent.Log.WriteLine(5, "Can not shutdown module '" + moduleSequence[i] + "': The module is disabled.");
						continue;
					}
					if (mc == null) continue;
					if (!mc.IsLocal)
					//if (true)
					{
						//Parent.Log.WriteLine(5, "Can not start module '" + moduleSequence[i] + "': unable to start remote process.");
						result = RemoteShutdown(mc, method);
						continue;
					}

					switch (method)
					{
						case ModuleShutdownMethod.CloseOnly:
							CloseModule(mc);
							break;

						case ModuleShutdownMethod.KillOnly:
							KillModule(mc);
							break;

						case ModuleShutdownMethod.CloseThenKill:
							if(!CloseModule(mc))
								KillModule(mc);
							break;
					}

				}
				Parent.Log.WriteLine(4, "Shutdown sequence excecution complete!");
			}
		}

		/// <summary>
		/// Executes the startup sequence of the registered modules with the default method
		/// </summary>
		public void ShutdownModulesAsync()
		{
			ShutdownModulesAsync(this.method);
		}

		/// <summary>
		/// Executes the startup sequence of the registered modules with the specified method
		/// </summary>
		/// <param name="method">The method used for startup</param>
		public void ShutdownModulesAsync(ModuleShutdownMethod method)
		{
			if ((this.mainThread != null) && this.mainThread.IsAlive)
				return;

			this.mainThread = new Thread(new ParameterizedThreadStart(ShutdownModules));
			this.mainThread.IsBackground = true;
			this.mainThread.Start(method);
		}

		#endregion
	}
}
