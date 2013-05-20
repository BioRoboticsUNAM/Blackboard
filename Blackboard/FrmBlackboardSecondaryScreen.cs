using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Blk.Gui
{
	public partial class FrmBlackboardSecondaryScreen : Form
	{
		private FrmBlackboard bbForm;

		public FrmBlackboardSecondaryScreen()
		{
			InitializeComponent();
		}

		public FrmBlackboardSecondaryScreen(FrmBlackboard bbForm)
		{
			InitializeComponent();
			this.bbForm = bbForm;
		}

		#region Properties

		public string ConfigFile
		{
			get { return this.txtConfigurationFile.Text; }
			set { this.txtConfigurationFile.Text = value; }
		}

		public string LogFile
		{
			get { return this.txtLogFile.Text; }
			set { this.txtLogFile.Text = value; }
		}

		public Button BtnStartStop
		{
			get { return this.btnStartStop; }
		}

		public Button BtnRestartTest
		{
			get { return this.btnRestartTest; }
		}

		public Button BtnRestartBlackboard
		{
			get { return this.btnRestartBlackboard; }
		}

		public Button BtnRestartTimer
		{
			get { return this.btnRestartTimer; }
		}

		public Button BtnExploreConfig
		{
			get { return this.btnExploreConfig; }
		}

		public Button BtnExploreLog
		{
			get { return this.btnExploreLog; }
		}

		public Button BtnLoad
		{
			get { return this.btnLoad; }
		}

		public GroupBox GbBlackboardFiles
		{
			get { return this.gbBlackboardFiles; }
		}

		public GroupBox GbGeneralActions
		{
			get { return this.gbGeneralActions; }
		}

		public GroupBox GbModuleList
		{
			get { return this.gbModuleList; }
		}

		public FlowLayoutPanel FlpModuleList
		{
			get { return this.flpModuleList; }
		}

		#endregion

		protected override void OnShown(EventArgs e)
		{
			if (Screen.PrimaryScreen.WorkingArea.Height < 600)
			{
				this.DesktopLocation = Screen.PrimaryScreen.Bounds.Location;
				this.DesktopBounds = Screen.PrimaryScreen.Bounds;
				this.TopMost = true;
				this.Focus();
			}
			else
			{
				this.Hide();
				return;
			}
			//if (Screen.AllScreens.Length < 2)
			//{
			//    this.Hide();
			//    return;
			//}
			//int snum = Screen.AllScreens.Length -1;
			//this.DesktopLocation = Screen.AllScreens[snum].Bounds.Location;
			//this.DesktopBounds = Screen.AllScreens[snum].Bounds;
		}

		#region EventHandlers

		private void btnStartStop_Click(object sender, EventArgs e)
		{
			bbForm.BlackboardStartStopToggle();
		}

		private void btnRestartTest_Click(object sender, EventArgs e)
		{
			bbForm.btnRestartTest_Click(sender, e);
		}

		private void btnRestartBlackboard_Click(object sender, EventArgs e)
		{
			bbForm.btnRestartBlackboard_Click(sender, e);
		}

		private void btnRestartTimer_Click(object sender, EventArgs e)
		{
			bbForm.btnRestartTimer_Click(sender, e);
		}

		private void btnExploreConfig_Click(object sender, EventArgs e)
		{
			bbForm.btnExploreConfig_Click(sender, e);
		}

		private void btnExploreLog_Click(object sender, EventArgs e)
		{
			bbForm.btnExploreLog_Click(sender, e);
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{
			bbForm.LoadBlackboard();
		}

		#endregion
	}
}