using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Blk.Engine;

namespace Blk.Gui
{
	public partial class FrmModuleActions : Form
	{
		private ModuleClient module;

		public FrmModuleActions()
		{
			this.Icon = Properties.Resources.Thunder_16_Icon;
			InitializeComponent();
		}

		public ModuleClient Module
		{
			get { return this.module; }
			set
			{
				this.module = value;
				
				if (module == null)
				{
					this.Hide();
					return;
				}
				this.Text = module.Name + "Action viewer";

				this.lstModuleStartupActions.Items.Clear();
				this.lstModuleStartupActions.Items.AddRange(this.module.GetStartupActions());

				this.lstModuleStopActions.Items.Clear();
				this.lstModuleStopActions.Items.AddRange(this.module.GetStopActions());

				this.lstModuleRestartActions.Items.Clear();
				this.lstModuleRestartActions.Items.AddRange(this.module.GetRestartActions());

				this.lstModuleExecTimeOutActions.Items.Clear();
				this.lstModuleExecTimeOutActions.Items.AddRange(this.module.GetTestTimeOutActions());
			}
		}


	}
}
