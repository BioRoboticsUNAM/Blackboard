using System;
using System.Collections.Generic;
using System.Text;
using Robotics.API;

namespace Blk.Api
{
	/// <summary>
	/// Serves as base class for implementing a plugin which will act as a module
	/// </summary>
	public abstract class ModulePlugin : IBlackboardPlugin//, IModule
	{
		#region Variables

		#endregion

		#region Properties

		/// <summary>
		/// The blackboard this Plugin is bound to
		/// </summary>
		public IBlackboard Blackboard
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets a value indicaing if the plugin has been initialized
		/// </summary>
		public bool Initialized
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets a value indicating if the Plugin is running
		/// </summary>
		public bool IsRunning
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the name of the module that this IMessageSource object interfaces.
		/// </summary>
		public string ModuleName
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the name of the Plugin
		/// </summary>
		public string Name
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Attaches a Blackboard object to the plugin. If a blackboard is already attached, it throws an exception
		/// </summary>
		/// <param name="blackboard">The blackboard object which will be used to initialize the plugin</param>
		public void AttachBlackboard(IBlackboard blackboard)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Initializes the plugin
		/// </summary>
		public void Initialize()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Starts the Plugin
		/// </summary>
		public void Start()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Stops the Plugin
		/// </summary>
		public void Stop()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
