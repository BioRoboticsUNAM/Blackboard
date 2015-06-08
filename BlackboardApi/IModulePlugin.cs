using System;
using Robotics.API;

namespace Blk.Api
{
	/// <summary>
	/// Represents a plugin which will act as a module
	/// </summary>
	public interface IModulePlugin : Robotics.Plugins.IPlugin
	{
		#region Properties

		/// <summary>
		/// Gets the blackboard this Plugin is bound to
		/// </summary>
		IBlackboard Blackboard { get; }

		/// <summary>
		/// When overriden in a derived class, gives access to the command manager which
		/// manages the commands of this ModulePlugin
		/// </summary>
		CommandManager CommandManager { get; }

		/// <summary>
		/// Gets the connector object used to communicate with blackboard
		/// </summary>
		PluginConnector Connector { get; }

		/// <summary>
		/// Gets the name of the module that this IMessageSource object interfaces.
		/// </summary>
		string ModuleName { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Attaches a Blackboard object to the plugin. If a blackboard is already attached, it throws an exception
		/// </summary>
		/// <param name="blackboard">The blackboard object which will be used to initialize the plugin</param>
		void AttachBlackboard(IBlackboard blackboard);

		/// <summary>
		/// Attaches a IModuleClient object which will be used to communicate with blackboard
		/// </summary>
		/// <param name="moduleClient"></param>
		void AttachClient(IModuleClientPlugin moduleClient);

		#endregion

	}
}
