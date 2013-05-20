namespace Blk.Gui
{
	partial class FrmBlackboardSecondaryScreen
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmBlackboardSecondaryScreen));
			this.btnStartStop = new System.Windows.Forms.Button();
			this.gbGeneralActions = new System.Windows.Forms.GroupBox();
			this.btnRestartTimer = new System.Windows.Forms.Button();
			this.btnRestartBlackboard = new System.Windows.Forms.Button();
			this.btnRestartTest = new System.Windows.Forms.Button();
			this.gbBlackboardFiles = new System.Windows.Forms.GroupBox();
			this.btnLoad = new System.Windows.Forms.Button();
			this.btnExploreLog = new System.Windows.Forms.Button();
			this.btnExploreConfig = new System.Windows.Forms.Button();
			this.lblLogFile = new System.Windows.Forms.Label();
			this.lblConfigurationFile = new System.Windows.Forms.Label();
			this.txtLogFile = new System.Windows.Forms.TextBox();
			this.txtConfigurationFile = new System.Windows.Forms.TextBox();
			this.gbModuleList = new System.Windows.Forms.GroupBox();
			this.flpModuleList = new System.Windows.Forms.FlowLayoutPanel();
			this.gbGeneralActions.SuspendLayout();
			this.gbBlackboardFiles.SuspendLayout();
			this.gbModuleList.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnStartStop
			// 
			this.btnStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnStartStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnStartStop.Image = global::Blk.Gui.Properties.Resources.run48;
			this.btnStartStop.Location = new System.Drawing.Point(229, 12);
			this.btnStartStop.Name = "btnStartStop";
			this.btnStartStop.Size = new System.Drawing.Size(391, 70);
			this.btnStartStop.TabIndex = 10;
			this.btnStartStop.Text = "Start Blackbord";
			this.btnStartStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnStartStop.UseVisualStyleBackColor = true;
			this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
			// 
			// gbGeneralActions
			// 
			this.gbGeneralActions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbGeneralActions.Controls.Add(this.btnRestartTimer);
			this.gbGeneralActions.Controls.Add(this.btnRestartBlackboard);
			this.gbGeneralActions.Controls.Add(this.btnRestartTest);
			this.gbGeneralActions.Enabled = false;
			this.gbGeneralActions.Location = new System.Drawing.Point(229, 88);
			this.gbGeneralActions.Name = "gbGeneralActions";
			this.gbGeneralActions.Size = new System.Drawing.Size(391, 202);
			this.gbGeneralActions.TabIndex = 11;
			this.gbGeneralActions.TabStop = false;
			this.gbGeneralActions.Text = "General Actions";
			// 
			// btnRestartTimer
			// 
			this.btnRestartTimer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnRestartTimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnRestartTimer.Location = new System.Drawing.Point(6, 147);
			this.btnRestartTimer.Name = "btnRestartTimer";
			this.btnRestartTimer.Size = new System.Drawing.Size(379, 46);
			this.btnRestartTimer.TabIndex = 100;
			this.btnRestartTimer.Text = "Restart Timer";
			this.btnRestartTimer.UseVisualStyleBackColor = true;
			this.btnRestartTimer.Click += new System.EventHandler(this.btnRestartTimer_Click);
			// 
			// btnRestartBlackboard
			// 
			this.btnRestartBlackboard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnRestartBlackboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnRestartBlackboard.Location = new System.Drawing.Point(6, 95);
			this.btnRestartBlackboard.Name = "btnRestartBlackboard";
			this.btnRestartBlackboard.Size = new System.Drawing.Size(379, 46);
			this.btnRestartBlackboard.TabIndex = 100;
			this.btnRestartBlackboard.Text = "Restart Blackboard";
			this.btnRestartBlackboard.UseVisualStyleBackColor = true;
			this.btnRestartBlackboard.Click += new System.EventHandler(this.btnRestartBlackboard_Click);
			// 
			// btnRestartTest
			// 
			this.btnRestartTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnRestartTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnRestartTest.Image = global::Blk.Gui.Properties.Resources.refresh48;
			this.btnRestartTest.Location = new System.Drawing.Point(6, 19);
			this.btnRestartTest.Name = "btnRestartTest";
			this.btnRestartTest.Size = new System.Drawing.Size(379, 70);
			this.btnRestartTest.TabIndex = 99;
			this.btnRestartTest.Text = "Restart Test";
			this.btnRestartTest.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnRestartTest.UseVisualStyleBackColor = true;
			this.btnRestartTest.Click += new System.EventHandler(this.btnRestartTest_Click);
			// 
			// gbBlackboardFiles
			// 
			this.gbBlackboardFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.gbBlackboardFiles.Controls.Add(this.btnLoad);
			this.gbBlackboardFiles.Controls.Add(this.btnExploreLog);
			this.gbBlackboardFiles.Controls.Add(this.btnExploreConfig);
			this.gbBlackboardFiles.Controls.Add(this.lblLogFile);
			this.gbBlackboardFiles.Controls.Add(this.lblConfigurationFile);
			this.gbBlackboardFiles.Controls.Add(this.txtLogFile);
			this.gbBlackboardFiles.Controls.Add(this.txtConfigurationFile);
			this.gbBlackboardFiles.Location = new System.Drawing.Point(229, 296);
			this.gbBlackboardFiles.Name = "gbBlackboardFiles";
			this.gbBlackboardFiles.Size = new System.Drawing.Size(391, 138);
			this.gbBlackboardFiles.TabIndex = 9;
			this.gbBlackboardFiles.TabStop = false;
			this.gbBlackboardFiles.Text = "Blackboard Files";
			// 
			// btnLoad
			// 
			this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnLoad.Location = new System.Drawing.Point(210, 82);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(175, 50);
			this.btnLoad.TabIndex = 5;
			this.btnLoad.Text = "Load blackboard";
			this.btnLoad.UseVisualStyleBackColor = true;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// btnExploreLog
			// 
			this.btnExploreLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnExploreLog.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnExploreLog.Image = global::Blk.Gui.Properties.Resources.ZoomHS;
			this.btnExploreLog.Location = new System.Drawing.Point(362, 48);
			this.btnExploreLog.Name = "btnExploreLog";
			this.btnExploreLog.Size = new System.Drawing.Size(23, 23);
			this.btnExploreLog.TabIndex = 4;
			this.btnExploreLog.UseVisualStyleBackColor = true;
			this.btnExploreLog.Click += new System.EventHandler(this.btnExploreLog_Click);
			// 
			// btnExploreConfig
			// 
			this.btnExploreConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnExploreConfig.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnExploreConfig.Image = global::Blk.Gui.Properties.Resources.ZoomHS;
			this.btnExploreConfig.Location = new System.Drawing.Point(362, 19);
			this.btnExploreConfig.Name = "btnExploreConfig";
			this.btnExploreConfig.Size = new System.Drawing.Size(23, 23);
			this.btnExploreConfig.TabIndex = 2;
			this.btnExploreConfig.UseVisualStyleBackColor = true;
			this.btnExploreConfig.Click += new System.EventHandler(this.btnExploreConfig_Click);
			// 
			// lblLogFile
			// 
			this.lblLogFile.AutoSize = true;
			this.lblLogFile.Location = new System.Drawing.Point(7, 53);
			this.lblLogFile.Name = "lblLogFile";
			this.lblLogFile.Size = new System.Drawing.Size(78, 13);
			this.lblLogFile.TabIndex = 2;
			this.lblLogFile.Text = "Output log File:";
			// 
			// lblConfigurationFile
			// 
			this.lblConfigurationFile.AutoSize = true;
			this.lblConfigurationFile.Location = new System.Drawing.Point(7, 24);
			this.lblConfigurationFile.Name = "lblConfigurationFile";
			this.lblConfigurationFile.Size = new System.Drawing.Size(91, 13);
			this.lblConfigurationFile.TabIndex = 2;
			this.lblConfigurationFile.Text = "Configuration File:";
			// 
			// txtLogFile
			// 
			this.txtLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLogFile.Location = new System.Drawing.Point(103, 50);
			this.txtLogFile.Name = "txtLogFile";
			this.txtLogFile.Size = new System.Drawing.Size(253, 20);
			this.txtLogFile.TabIndex = 3;
			// 
			// txtConfigurationFile
			// 
			this.txtConfigurationFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtConfigurationFile.Location = new System.Drawing.Point(103, 21);
			this.txtConfigurationFile.Name = "txtConfigurationFile";
			this.txtConfigurationFile.Size = new System.Drawing.Size(253, 20);
			this.txtConfigurationFile.TabIndex = 1;
			// 
			// gbModuleList
			// 
			this.gbModuleList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.gbModuleList.Controls.Add(this.flpModuleList);
			this.gbModuleList.Location = new System.Drawing.Point(12, 12);
			this.gbModuleList.MaximumSize = new System.Drawing.Size(212, 422);
			this.gbModuleList.MinimumSize = new System.Drawing.Size(212, 211);
			this.gbModuleList.Name = "gbModuleList";
			this.gbModuleList.Size = new System.Drawing.Size(212, 422);
			this.gbModuleList.TabIndex = 8;
			this.gbModuleList.TabStop = false;
			this.gbModuleList.Text = "Module List";
			// 
			// flpModuleList
			// 
			this.flpModuleList.AutoScroll = true;
			this.flpModuleList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flpModuleList.Location = new System.Drawing.Point(3, 16);
			this.flpModuleList.Name = "flpModuleList";
			this.flpModuleList.Size = new System.Drawing.Size(206, 403);
			this.flpModuleList.TabIndex = 0;
			// 
			// FrmBlackboardSecondaryScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(632, 446);
			this.ControlBox = false;
			this.Controls.Add(this.btnStartStop);
			this.Controls.Add(this.gbGeneralActions);
			this.Controls.Add(this.gbBlackboardFiles);
			this.Controls.Add(this.gbModuleList);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(640, 480);
			this.MinimumSize = new System.Drawing.Size(320, 240);
			this.Name = "FrmBlackboardSecondaryScreen";
			this.Text = "Blackboard";
			this.gbGeneralActions.ResumeLayout(false);
			this.gbBlackboardFiles.ResumeLayout(false);
			this.gbBlackboardFiles.PerformLayout();
			this.gbModuleList.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnStartStop;
		private System.Windows.Forms.GroupBox gbGeneralActions;
		private System.Windows.Forms.Button btnRestartTimer;
		private System.Windows.Forms.Button btnRestartBlackboard;
		private System.Windows.Forms.Button btnRestartTest;
		private System.Windows.Forms.GroupBox gbBlackboardFiles;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.Button btnExploreLog;
		private System.Windows.Forms.Button btnExploreConfig;
		private System.Windows.Forms.Label lblLogFile;
		private System.Windows.Forms.Label lblConfigurationFile;
		private System.Windows.Forms.TextBox txtLogFile;
		private System.Windows.Forms.TextBox txtConfigurationFile;
		private System.Windows.Forms.GroupBox gbModuleList;
		private System.Windows.Forms.FlowLayoutPanel flpModuleList;
	}
}