using System;
using System.Collections.Generic;
using System.Text;

namespace Blk.Api
{
	/// <summary>
	/// Represents a plugin for the blackboard
	/// </summary>
	public interface IBlackboardPlugin : Robotics.Plugins.IPlugin
	{
		#region Properties

		/// <summary>
		/// The blackboard this PluginManager is bound to
		/// </summary>
		IBlackboard Blackboard { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Attaches a Blackboard object to the plugin. If a blackboard is already attached, it throws an exception
		/// </summary>
		/// <param name="blackboard">The blackboard object which will be used to initialize the plugin</param>
		void AttachBlackboard(IBlackboard blackboard);

		#endregion
	}
}
