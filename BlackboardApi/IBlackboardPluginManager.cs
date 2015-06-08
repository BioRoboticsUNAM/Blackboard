using System;
using Robotics.Plugins;

namespace Blk.Api
{
	/// <summary>
	/// Represents a PluginManager interfaced with a Blackboard
	/// </summary>
	public interface IBlackboardPluginManager :IPluginManager<IBlackboardPlugin>
	{
		#region Properties

		/// <summary>
		/// The blackboard this IPluginManager is bound to
		/// </summary>
		IBlackboard Owner { get; }

		#endregion

		#region Methods

		#endregion
	}
}
