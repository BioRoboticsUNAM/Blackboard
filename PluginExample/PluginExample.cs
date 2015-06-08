using System;
using System.Windows.Forms;
using Blk.Api;
using Robotics.API;
using PluginExample.CommandExecuters;

namespace PluginExample
{
	public class PluginExample : ModulePlugin
	{
		private CommandManager cmdMan;
		private FrmPluginExample gui;

		public PluginExample()
		{
			this.cmdMan = new CommandManager();
			Engine engine = new Engine();
			this.cmdMan.CommandExecuters.Add(new FactorialCommandExecuter(engine));
			this.cmdMan.CommandExecuters.Add(new StatusCommandExecuter(engine));
			this.gui = new FrmPluginExample();
		}

		public override string ModuleName
		{
			get { return "PLG-SAMPLE"; }
		}

		public override string Name
		{
			get { return "Sample ModulePlugin"; }
		}

		public override CommandManager CommandManager
		{
			get { return this.cmdMan; }
		}

		public override bool Initialized
		{
			get { return true; }
		}

		public override void Initialize()
		{
			gui.Show();
		}
	}
}
