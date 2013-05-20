namespace Blk.Gui
{
	partial class FrmModuleActions
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tcModuleActions = new System.Windows.Forms.TabControl();
			this.tpModuleStartupActions = new System.Windows.Forms.TabPage();
			this.lstModuleStartupActions = new System.Windows.Forms.ListBox();
			this.tpModuleStopActions = new System.Windows.Forms.TabPage();
			this.lstModuleStopActions = new System.Windows.Forms.ListBox();
			this.tpModuleRestartActions = new System.Windows.Forms.TabPage();
			this.lstModuleRestartActions = new System.Windows.Forms.ListBox();
			this.tpModuleRestartTestActions = new System.Windows.Forms.TabPage();
			this.lstModuleRestartTestActions = new System.Windows.Forms.ListBox();
			this.tpModuleExecTimeOutActions = new System.Windows.Forms.TabPage();
			this.lstModuleExecTimeOutActions = new System.Windows.Forms.ListBox();
			this.tcModuleActions.SuspendLayout();
			this.tpModuleStartupActions.SuspendLayout();
			this.tpModuleStopActions.SuspendLayout();
			this.tpModuleRestartActions.SuspendLayout();
			this.tpModuleRestartTestActions.SuspendLayout();
			this.tpModuleExecTimeOutActions.SuspendLayout();
			this.SuspendLayout();
			// 
			// tcModuleActions
			// 
			this.tcModuleActions.Controls.Add(this.tpModuleStartupActions);
			this.tcModuleActions.Controls.Add(this.tpModuleStopActions);
			this.tcModuleActions.Controls.Add(this.tpModuleRestartActions);
			this.tcModuleActions.Controls.Add(this.tpModuleRestartTestActions);
			this.tcModuleActions.Controls.Add(this.tpModuleExecTimeOutActions);
			this.tcModuleActions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcModuleActions.Location = new System.Drawing.Point(0, 0);
			this.tcModuleActions.Name = "tcModuleActions";
			this.tcModuleActions.SelectedIndex = 0;
			this.tcModuleActions.Size = new System.Drawing.Size(584, 162);
			this.tcModuleActions.TabIndex = 1;
			// 
			// tpModuleStartupActions
			// 
			this.tpModuleStartupActions.Controls.Add(this.lstModuleStartupActions);
			this.tpModuleStartupActions.Location = new System.Drawing.Point(4, 22);
			this.tpModuleStartupActions.Name = "tpModuleStartupActions";
			this.tpModuleStartupActions.Padding = new System.Windows.Forms.Padding(3);
			this.tpModuleStartupActions.Size = new System.Drawing.Size(576, 136);
			this.tpModuleStartupActions.TabIndex = 0;
			this.tpModuleStartupActions.Text = "Startup Actions";
			this.tpModuleStartupActions.UseVisualStyleBackColor = true;
			// 
			// lstModuleStartupActions
			// 
			this.lstModuleStartupActions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstModuleStartupActions.FormattingEnabled = true;
			this.lstModuleStartupActions.Location = new System.Drawing.Point(3, 3);
			this.lstModuleStartupActions.Name = "lstModuleStartupActions";
			this.lstModuleStartupActions.Size = new System.Drawing.Size(570, 121);
			this.lstModuleStartupActions.TabIndex = 0;
			// 
			// tpModuleStopActions
			// 
			this.tpModuleStopActions.Controls.Add(this.lstModuleStopActions);
			this.tpModuleStopActions.Location = new System.Drawing.Point(4, 22);
			this.tpModuleStopActions.Name = "tpModuleStopActions";
			this.tpModuleStopActions.Padding = new System.Windows.Forms.Padding(3);
			this.tpModuleStopActions.Size = new System.Drawing.Size(183, 149);
			this.tpModuleStopActions.TabIndex = 1;
			this.tpModuleStopActions.Text = "Stop Actions";
			this.tpModuleStopActions.UseVisualStyleBackColor = true;
			// 
			// lstModuleStopActions
			// 
			this.lstModuleStopActions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstModuleStopActions.FormattingEnabled = true;
			this.lstModuleStopActions.Location = new System.Drawing.Point(3, 3);
			this.lstModuleStopActions.Name = "lstModuleStopActions";
			this.lstModuleStopActions.Size = new System.Drawing.Size(177, 134);
			this.lstModuleStopActions.TabIndex = 0;
			// 
			// tpModuleRestartActions
			// 
			this.tpModuleRestartActions.Controls.Add(this.lstModuleRestartActions);
			this.tpModuleRestartActions.Location = new System.Drawing.Point(4, 22);
			this.tpModuleRestartActions.Name = "tpModuleRestartActions";
			this.tpModuleRestartActions.Padding = new System.Windows.Forms.Padding(3);
			this.tpModuleRestartActions.Size = new System.Drawing.Size(183, 149);
			this.tpModuleRestartActions.TabIndex = 2;
			this.tpModuleRestartActions.Text = "Restart Actions";
			this.tpModuleRestartActions.UseVisualStyleBackColor = true;
			// 
			// lstModuleRestartActions
			// 
			this.lstModuleRestartActions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstModuleRestartActions.FormattingEnabled = true;
			this.lstModuleRestartActions.Location = new System.Drawing.Point(3, 3);
			this.lstModuleRestartActions.Name = "lstModuleRestartActions";
			this.lstModuleRestartActions.Size = new System.Drawing.Size(177, 134);
			this.lstModuleRestartActions.TabIndex = 1;
			// 
			// tpModuleRestartTestActions
			// 
			this.tpModuleRestartTestActions.Controls.Add(this.lstModuleRestartTestActions);
			this.tpModuleRestartTestActions.Location = new System.Drawing.Point(4, 22);
			this.tpModuleRestartTestActions.Name = "tpModuleRestartTestActions";
			this.tpModuleRestartTestActions.Padding = new System.Windows.Forms.Padding(3);
			this.tpModuleRestartTestActions.Size = new System.Drawing.Size(183, 149);
			this.tpModuleRestartTestActions.TabIndex = 3;
			this.tpModuleRestartTestActions.Text = "Restart Test Actions";
			this.tpModuleRestartTestActions.UseVisualStyleBackColor = true;
			// 
			// lstModuleRestartTestActions
			// 
			this.lstModuleRestartTestActions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstModuleRestartTestActions.FormattingEnabled = true;
			this.lstModuleRestartTestActions.Location = new System.Drawing.Point(3, 3);
			this.lstModuleRestartTestActions.Name = "lstModuleRestartTestActions";
			this.lstModuleRestartTestActions.Size = new System.Drawing.Size(177, 134);
			this.lstModuleRestartTestActions.TabIndex = 2;
			// 
			// tpModuleExecTimeOutActions
			// 
			this.tpModuleExecTimeOutActions.Controls.Add(this.lstModuleExecTimeOutActions);
			this.tpModuleExecTimeOutActions.Location = new System.Drawing.Point(4, 22);
			this.tpModuleExecTimeOutActions.Name = "tpModuleExecTimeOutActions";
			this.tpModuleExecTimeOutActions.Padding = new System.Windows.Forms.Padding(3);
			this.tpModuleExecTimeOutActions.Size = new System.Drawing.Size(183, 149);
			this.tpModuleExecTimeOutActions.TabIndex = 4;
			this.tpModuleExecTimeOutActions.Text = "Test Timeout Actions";
			this.tpModuleExecTimeOutActions.UseVisualStyleBackColor = true;
			// 
			// lstModuleExecTimeOutActions
			// 
			this.lstModuleExecTimeOutActions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstModuleExecTimeOutActions.FormattingEnabled = true;
			this.lstModuleExecTimeOutActions.Location = new System.Drawing.Point(3, 3);
			this.lstModuleExecTimeOutActions.Name = "lstModuleExecTimeOutActions";
			this.lstModuleExecTimeOutActions.Size = new System.Drawing.Size(177, 134);
			this.lstModuleExecTimeOutActions.TabIndex = 3;
			// 
			// FrmModuleActions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 162);
			this.Controls.Add(this.tcModuleActions);
			this.Name = "FrmModuleActions";
			this.Text = "Actions";
			this.tcModuleActions.ResumeLayout(false);
			this.tpModuleStartupActions.ResumeLayout(false);
			this.tpModuleStopActions.ResumeLayout(false);
			this.tpModuleRestartActions.ResumeLayout(false);
			this.tpModuleRestartTestActions.ResumeLayout(false);
			this.tpModuleExecTimeOutActions.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tcModuleActions;
		private System.Windows.Forms.TabPage tpModuleStartupActions;
		private System.Windows.Forms.ListBox lstModuleStartupActions;
		private System.Windows.Forms.TabPage tpModuleStopActions;
		private System.Windows.Forms.ListBox lstModuleStopActions;
		private System.Windows.Forms.TabPage tpModuleRestartActions;
		private System.Windows.Forms.ListBox lstModuleRestartActions;
		private System.Windows.Forms.TabPage tpModuleRestartTestActions;
		private System.Windows.Forms.ListBox lstModuleRestartTestActions;
		private System.Windows.Forms.TabPage tpModuleExecTimeOutActions;
		private System.Windows.Forms.ListBox lstModuleExecTimeOutActions;
	}
}