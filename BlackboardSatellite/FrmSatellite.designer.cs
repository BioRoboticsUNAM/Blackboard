namespace BlackboardSatellite
{
	partial class FrmSatellite
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSatellite));
			this.notifyIconMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.txtLog = new System.Windows.Forms.TextBox();
			this.gbLog = new System.Windows.Forms.GroupBox();
			this.notifyIconMenu.SuspendLayout();
			this.gbLog.SuspendLayout();
			this.SuspendLayout();
			// 
			// notifyIconMenu
			// 
			this.notifyIconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.notifyIconMenu.Name = "notifyIconMenu";
			this.notifyIconMenu.Size = new System.Drawing.Size(104, 48);
			// 
			// showToolStripMenuItem
			// 
			this.showToolStripMenuItem.Name = "showToolStripMenuItem";
			this.showToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.showToolStripMenuItem.Text = "&Show";
			this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// notifyIcon
			// 
			this.notifyIcon.ContextMenuStrip = this.notifyIconMenu;
			this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			this.notifyIcon.Text = "Blackboard Satellite";
			this.notifyIcon.Visible = true;
			this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
			// 
			// txtLog
			// 
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtLog.Location = new System.Drawing.Point(3, 16);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLog.Size = new System.Drawing.Size(578, 243);
			this.txtLog.TabIndex = 1;
			this.txtLog.WordWrap = false;
			// 
			// gbLog
			// 
			this.gbLog.Controls.Add(this.txtLog);
			this.gbLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbLog.Location = new System.Drawing.Point(0, 0);
			this.gbLog.Name = "gbLog";
			this.gbLog.Size = new System.Drawing.Size(584, 262);
			this.gbLog.TabIndex = 2;
			this.gbLog.TabStop = false;
			this.gbLog.Text = "Log";
			// 
			// FrmSatellite
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 262);
			this.Controls.Add(this.gbLog);
			this.Name = "FrmSatellite";
			this.Text = "Blackboard Satellite";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSatellite_FormClosing);
			this.Resize += new System.EventHandler(this.FrmSatellite_Resize);
			this.notifyIconMenu.ResumeLayout(false);
			this.gbLog.ResumeLayout(false);
			this.gbLog.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip notifyIconMenu;
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.TextBox txtLog;
		private System.Windows.Forms.GroupBox gbLog;
		private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
	}
}