using System;
using System.Collections.Generic;
using System.Text;
using Blk.Api;

namespace Blk.Engine
{
	/// <summary>
	/// Implements
	/// </summary>
	public class BlackboardPluginManager : Robotics.Plugins.PluginManager<IBlackboardPlugin>, IBlackboardPluginManager
	{
		#region Variables

		/// <summary>
		/// The blackboard this PluginManager is bound to
		/// </summary>
		protected readonly Blackboard owner;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of PluginManager
		/// </summary>
		/// <param name="owner">The blackboard this PluginManager is bound to</param>
		public BlackboardPluginManager(Blackboard owner)
		{
			if (owner == null)
				throw new ArgumentNullException();
			this.owner = owner;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The blackboard this PluginManager is bound to
		/// </summary>
		public Blackboard Owner
		{
			get { return this.owner; }
		}

		/// <summary>
		/// The blackboard this PluginManager is bound to
		/// </summary>
		IBlackboard IBlackboardPluginManager.Owner
		{
			get { return this.owner; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Attaches the owner blackboard to all the plugins stored in the PluginManager
		/// and initializes them.
		/// </summary>
		public override void InitializePlugins()
		{
			lock (this.lockObject)
			{
				this.rwPluginsLock.AcquireReaderLock(-1);
				foreach (IBlackboardPlugin plugin in this.plugins.Values)
				{
					try
					{
						plugin.AttachBlackboard(this.owner);
						if (plugin is IModulePlugin)
							AddClientForModulePlugin((IModulePlugin)plugin);
					}
					catch { }
				}
				this.rwPluginsLock.ReleaseReaderLock();
				base.InitializePlugins();
			}
		}

		/// <summary>
		/// Adds a ModuleClientPlugin to the blackboard so it can communicate to the module plugin application
		/// </summary>
		/// <param name="plugin">The plugin to initialize as module</param>
		private void AddClientForModulePlugin(IModulePlugin plugin)
		{
			ModuleClientPlugin module;

			module = new ModuleClientPlugin(plugin);
			if (this.owner.Modules.Contains(plugin.ModuleName))
				return;
			this.owner.Modules.Add(module);
			plugin.AttachClient(module);
		}

		#endregion
	}
}
