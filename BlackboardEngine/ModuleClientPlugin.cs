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
	/// Implements a ModuleClient for interfacing module applications running as plugins
	/// </summary>
	public class ModuleClientPlugin : ModuleClient, IModuleClientPlugin
	{
		#region Variables

		/// <summary>
		/// The plugin application module which this ModuleClient interfaces
		/// </summary>
		private readonly IModulePlugin plugin;

		/// <summary>
		/// Stores the received messages
		/// </summary>
		private readonly ProducerConsumer<string> dataReceived;

		/*
		/// <summary>
		/// Regular expression used to parse SetupModule command arguments
		/// </summary>
		private static readonly Regex rxSetupModule =
			new Regex(@"(?<moduleName>[A-Z][A-Z\-]+[A-Z])\s+ip\s*=\s*(?<ip>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\s+port\s*=\s*(?<port>\d{4,5})", RegexOptions.Compiled);
		*/

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the ModuleClientPlugin class
		/// </summary>
		/// <param name="plugin">The plugin application module</param>
		internal ModuleClientPlugin(IModulePlugin plugin)
			: base(plugin.ModuleName)
		{
			if (plugin == null)
				throw new ArgumentNullException();
			this.plugin = plugin;
			dataReceived = new ProducerConsumer<string>(100);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the plugin application module which this ModuleClient interfaces
		/// </summary>
		public IModulePlugin Plugin
		{
			get { return this.plugin; }
		}

		/// <summary>
		/// Tells if the connection to the ModuleServer has been stablished
		/// </summary>
		public override bool IsConnected
		{
			get
			{
				return running && this.plugin.Connector.IsRunning;
			}
		}

		/// <summary>
		/// Always return true
		/// </summary>
		public override bool IsLocal
		{
			get { return true; }
		}

		#endregion

		#region Methods

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
		/// Parses all received messages
		/// </summary>
		protected override void ParsePendingData()
		{
			string message;

			do
			{
				message = dataReceived.Consume(20);
				if (message == null)
					return;

				ParseMessage(message);
			} while (!stopMainThread && dataReceived.Count > 0);
		}

		/// <summary>
		/// Receives a message and enqueues it for processing and redirection
		/// </summary>
		/// <param name="message">The message to receive</param>
		public virtual void Receive(string message)
		{
			this.dataReceived.Produce(message);
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
		/// When overriden in a derived class, it sends the provided text string to the module application
		/// </summary>
		/// <param name="stringToSend">string to send to the module application</param>
		protected override bool Send(string stringToSend)
		{
			return false;
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

		#endregion
	}
}
