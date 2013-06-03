using System;
using System.Collections.Generic;
using System.Text;
using Robotics.API;

namespace Blk.Api
{
	/// <summary>
	/// Serves as base class for implementing a plugin which will act as a module
	/// </summary>
	public abstract class ModulePlugin : IModulePlugin, IBlackboardPlugin
	{
		#region Variables

		/// <summary>
		/// The blackboard this Plugin is bound to
		/// </summary>
		private IBlackboard blackboard;

		/// <summary>
		/// The connector object used to communicate with blackboard
		/// </summary>
		private PluginConnector connector;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the blackboard this Plugin is bound to
		/// </summary>
		public IBlackboard Blackboard
		{
			get { return this.blackboard; }
		}

		/// <summary>
		/// When overriden in a derived class, gives access to the command manager which
		/// manages the commands of this ModulePlugin
		/// </summary>
		public abstract CommandManager CommandManager { get; }

		/// <summary>
		/// Gets the connector object used to communicate with blackboard
		/// </summary>
		public PluginConnector Connector
		{
			get { return this.connector; }
		}

		/// <summary>
		/// Gets a value indicaing if the plugin has been initialized
		/// </summary>
		public abstract bool Initialized { get; }

		/// <summary>
		/// Gets a value indicating if the Plugin is running
		/// </summary>
		public bool IsRunning
		{
			get { return CommandManager.IsRunning; }
		}

		/// <summary>
		/// Gets the name of the module that this IMessageSource object interfaces.
		/// </summary>
		public abstract string ModuleName { get; }

		/// <summary>
		/// Gets the name of the Plugin
		/// </summary>
		public abstract string Name { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Attaches a Blackboard object to the plugin. If a blackboard is already attached, it throws an exception
		/// </summary>
		/// <param name="blackboard">The blackboard object which will be used to initialize the plugin</param>
		public void AttachBlackboard(IBlackboard blackboard)
		{
			this.blackboard = blackboard;
		}

		/// <summary>
		/// Attaches a IModuleClient object which will be used to communicate with blackboard
		/// </summary>
		/// <param name="moduleClient"></param>
		public void AttachClient(IModuleClientPlugin moduleClient)
		{
			if (CommandManager.IsRunning)
				throw new ArgumentException("Cannot attach an IModuleClient object while the module is running");
			if (this.connector != null)
				this.connector.Stop();
			if (moduleClient == null)
				Stop();
			this.connector = new PluginConnector(moduleClient);
			this.CommandManager.Connector = this.connector;
		}

		/// <summary>
		/// Initializes the plugin
		/// </summary>
		public abstract void Initialize();

		/// <summary>
		/// Starts the Plugin
		/// </summary>
		public void Start()
		{
			if (CommandManager.IsRunning)
				return;
			if (this.connector == null)
				throw new Exception("Cannot start the module. Communication object with blackboard not set (possibly due to duplicate  or invalid module name)");
			this.connector.Start();
			CommandManager.Start();
		}

		/// <summary>
		/// Stops the Plugin
		/// </summary>
		public void Stop()
		{
			if (!CommandManager.IsRunning)
				return;
			if (this.connector != null)
				this.connector.Stop();
			CommandManager.Stop();
		}

		#endregion
	}
}
