namespace Blk.Gui
{
	partial class FrmBlackboard
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
			this.gbModuleList = new System.Windows.Forms.GroupBox();
			this.flpModuleList = new System.Windows.Forms.FlowLayoutPanel();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.dlgOpenSettingsFile = new System.Windows.Forms.OpenFileDialog();
			this.dlgSaveLog = new System.Windows.Forms.SaveFileDialog();
			this.mnuStartSequence = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.ttToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.scTop = new System.Windows.Forms.SplitContainer();
			this.btnStartStop = new System.Windows.Forms.Button();
			this.gbGeneralActions = new System.Windows.Forms.GroupBox();
			this.btnStartSequence = new System.Windows.Forms.Button();
			this.btnRestartTimer = new System.Windows.Forms.Button();
			this.btnRestartBlackboard = new System.Windows.Forms.Button();
			this.btnRestartTest = new System.Windows.Forms.Button();
			this.gbBlackboardFiles = new System.Windows.Forms.GroupBox();
			this.chkAutoLog = new System.Windows.Forms.CheckBox();
			this.chkAutostart = new System.Windows.Forms.CheckBox();
			this.cbModuleStartupMode = new System.Windows.Forms.ComboBox();
			this.btnLoad = new System.Windows.Forms.Button();
			this.btnExploreLog = new System.Windows.Forms.Button();
			this.btnExploreConfig = new System.Windows.Forms.Button();
			this.lblModuleStartupMode = new System.Windows.Forms.Label();
			this.lblLogFile = new System.Windows.Forms.Label();
			this.lblConfigurationFile = new System.Windows.Forms.Label();
			this.txtLogFile = new System.Windows.Forms.TextBox();
			this.txtConfigurationFile = new System.Windows.Forms.TextBox();
			this.gbBlackboardSettings = new System.Windows.Forms.GroupBox();
			this.nudVerbosity = new System.Windows.Forms.NumericUpDown();
			this.nudRetrySendAttempts = new System.Windows.Forms.NumericUpDown();
			this.lblRetrySendAttempts = new System.Windows.Forms.Label();
			this.txtTimeLeft = new System.Windows.Forms.TextBox();
			this.txtClientsConnected = new System.Windows.Forms.TextBox();
			this.lblClientsConnected = new System.Windows.Forms.Label();
			this.txtBBAddresses = new System.Windows.Forms.TextBox();
			this.txtBBInputPort = new System.Windows.Forms.TextBox();
			this.lblTimeLeft = new System.Windows.Forms.Label();
			this.lblBBAddresses = new System.Windows.Forms.Label();
			this.lblVerbosity = new System.Windows.Forms.Label();
			this.lblBBInputPort = new System.Windows.Forms.Label();
			this.tcLog = new System.Windows.Forms.TabControl();
			this.tpDiagnostics = new System.Windows.Forms.TabPage();
			this.gbRemoteMachineStatus = new System.Windows.Forms.GroupBox();
			this.flpRemoteMachineStatus = new System.Windows.Forms.FlowLayoutPanel();
			this.tpInjector = new System.Windows.Forms.TabPage();
			this.tpMessagePendingList = new System.Windows.Forms.TabPage();
			this.gbCommandsPending = new System.Windows.Forms.GroupBox();
			this.lstCommandsPending = new System.Windows.Forms.ListBox();
			this.gbCommandsWaiting = new System.Windows.Forms.GroupBox();
			this.lstCommandsWaiting = new System.Windows.Forms.ListBox();
			this.tpModuleInfo = new System.Windows.Forms.TabPage();
			this.tsModuleToolBar = new System.Windows.Forms.ToolStrip();
			this.btnModuleEnable = new System.Windows.Forms.ToolStripButton();
			this.tsModuletoolBarSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.btnModuleStartStopModule = new System.Windows.Forms.ToolStripButton();
			this.btnModuleRestartModule = new System.Windows.Forms.ToolStripButton();
			this.btnModule = new System.Windows.Forms.ToolStripButton();
			this.btnShowModuleActions = new System.Windows.Forms.ToolStripButton();
			this.tsModuletoolBarSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.btnModuleSimulation = new System.Windows.Forms.ToolStripDropDownButton();
			this.btnSimulationEnabled = new System.Windows.Forms.ToolStripMenuItem();
			this.btnSimulationDisabled = new System.Windows.Forms.ToolStripMenuItem();
			this.tsModuleProcess = new System.Windows.Forms.ToolStripDropDownButton();
			this.btnModuleProcessRun = new System.Windows.Forms.ToolStripMenuItem();
			this.btnModuleProcessCheckRun = new System.Windows.Forms.ToolStripMenuItem();
			this.btnModuleProcessKill = new System.Windows.Forms.ToolStripMenuItem();
			this.tsModuleProcessSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.btnModuleProcessZombieCheck = new System.Windows.Forms.ToolStripMenuItem();
			this.gbModuleInfo = new System.Windows.Forms.GroupBox();
			this.txtModuleAuthor = new System.Windows.Forms.TextBox();
			this.txtOutputPort = new System.Windows.Forms.TextBox();
			this.txtModuleAlias = new System.Windows.Forms.TextBox();
			this.txtModuleName = new System.Windows.Forms.TextBox();
			this.txtModuleExeArgs = new System.Windows.Forms.TextBox();
			this.txtModuleProcessName = new System.Windows.Forms.TextBox();
			this.txtModuleExePath = new System.Windows.Forms.TextBox();
			this.txtRemoteIPAddress = new System.Windows.Forms.TextBox();
			this.lblModuleAlias = new System.Windows.Forms.Label();
			this.lblModuleAuthor = new System.Windows.Forms.Label();
			this.lblOutputPort = new System.Windows.Forms.Label();
			this.lblModuleName = new System.Windows.Forms.Label();
			this.lblModuleExeArgs = new System.Windows.Forms.Label();
			this.lblModuleProcessName = new System.Windows.Forms.Label();
			this.lblModuleExePath = new System.Windows.Forms.Label();
			this.lblRemoteIPAddress = new System.Windows.Forms.Label();
			this.tpOutputLog = new System.Windows.Forms.TabPage();
			this.btnClearOutputLog = new System.Windows.Forms.Button();
			this.txtOutputLog = new System.Windows.Forms.TextBox();
			this.btnSaveOutputLog = new System.Windows.Forms.Button();
			this.tpRedirectionHistory = new System.Windows.Forms.TabPage();
			this.lvwRedirectionHistory = new System.Windows.Forms.ListView();
			this.chCommand = new System.Windows.Forms.ColumnHeader();
			this.chResponse = new System.Windows.Forms.ColumnHeader();
			this.chCommandSource = new System.Windows.Forms.ColumnHeader();
			this.chCommandDestination = new System.Windows.Forms.ColumnHeader();
			this.chCommandSent = new System.Windows.Forms.ColumnHeader();
			this.chResponseArrival = new System.Windows.Forms.ColumnHeader();
			this.chInfo = new System.Windows.Forms.ColumnHeader();
			this.chResponseSent = new System.Windows.Forms.ColumnHeader();
			this.tpSharedVars = new System.Windows.Forms.TabPage();
			this.pgSelectedSharedVar = new System.Windows.Forms.PropertyGrid();
			this.lvSharedVariables = new System.Windows.Forms.ListView();
			this.mainMenu = new System.Windows.Forms.MenuStrip();
			this.mnuiBlackboard = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuiBlackboard_Load = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuiBlackboard_Reload = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuiBlackboard_Separator1 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuiBlackboard_StartStop = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuiBlackboard_Restart = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuiBlackboard_Restart_Blackboard = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuiBlackboard_Restart_Test = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuiBlackboard_Restart_Timer = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuiBlackboard_Separator2 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuiBlackboard_OpenCfgFile = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuiBlackboard_OpenLog = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuiBlackboard_Separator3 = new System.Windows.Forms.ToolStripSeparator();
			this.mnuiBlackboard_Exit = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuiModules = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuiPlugins = new System.Windows.Forms.ToolStripMenuItem();
			this.cbModuleShutdownMode = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnSutdownSequence = new System.Windows.Forms.Button();
			this.mnuShutdownSequence = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.injectorTool = new Blk.Gui.InjectorTool();
			this.interactionTool = new Blk.Gui.InteractionTool();
			this.gbModuleList.SuspendLayout();
			this.scTop.Panel1.SuspendLayout();
			this.scTop.Panel2.SuspendLayout();
			this.scTop.SuspendLayout();
			this.gbGeneralActions.SuspendLayout();
			this.gbBlackboardFiles.SuspendLayout();
			this.gbBlackboardSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudVerbosity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudRetrySendAttempts)).BeginInit();
			this.tcLog.SuspendLayout();
			this.tpDiagnostics.SuspendLayout();
			this.gbRemoteMachineStatus.SuspendLayout();
			this.tpInjector.SuspendLayout();
			this.tpMessagePendingList.SuspendLayout();
			this.gbCommandsPending.SuspendLayout();
			this.gbCommandsWaiting.SuspendLayout();
			this.tpModuleInfo.SuspendLayout();
			this.tsModuleToolBar.SuspendLayout();
			this.gbModuleInfo.SuspendLayout();
			this.tpOutputLog.SuspendLayout();
			this.tpRedirectionHistory.SuspendLayout();
			this.tpSharedVars.SuspendLayout();
			this.mainMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbModuleList
			// 
			this.gbModuleList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbModuleList.Controls.Add(this.flpModuleList);
			this.gbModuleList.Location = new System.Drawing.Point(621, 27);
			this.gbModuleList.Name = "gbModuleList";
			this.gbModuleList.Size = new System.Drawing.Size(211, 504);
			this.gbModuleList.TabIndex = 1;
			this.gbModuleList.TabStop = false;
			this.gbModuleList.Text = "Module List";
			// 
			// flpModuleList
			// 
			this.flpModuleList.AutoScroll = true;
			this.flpModuleList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flpModuleList.Location = new System.Drawing.Point(3, 16);
			this.flpModuleList.Name = "flpModuleList";
			this.flpModuleList.Size = new System.Drawing.Size(205, 485);
			this.flpModuleList.TabIndex = 0;
			// 
			// timer
			// 
			this.timer.Interval = 2000;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// dlgOpenSettingsFile
			// 
			this.dlgOpenSettingsFile.DefaultExt = "*.xml";
			this.dlgOpenSettingsFile.FileName = "Blackboard.xml";
			this.dlgOpenSettingsFile.Filter = "Blackboard configuration files|*.xml|All Files|*.*";
			// 
			// dlgSaveLog
			// 
			this.dlgSaveLog.DefaultExt = "*.log.xml";
			this.dlgSaveLog.Filter = "log files|*.log.xml|Text file|*.txt|All Files|*.*";
			// 
			// mnuStartSequence
			// 
			this.mnuStartSequence.Name = "contextMenuStrip1";
			this.mnuStartSequence.ShowImageMargin = false;
			this.mnuStartSequence.Size = new System.Drawing.Size(36, 4);
			// 
			// scTop
			// 
			this.scTop.Dock = System.Windows.Forms.DockStyle.Left;
			this.scTop.Location = new System.Drawing.Point(0, 24);
			this.scTop.Name = "scTop";
			this.scTop.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// scTop.Panel1
			// 
			this.scTop.Panel1.Controls.Add(this.btnStartStop);
			this.scTop.Panel1.Controls.Add(this.gbGeneralActions);
			this.scTop.Panel1.Controls.Add(this.gbBlackboardFiles);
			this.scTop.Panel1.Controls.Add(this.gbBlackboardSettings);
			this.scTop.Panel1MinSize = 270;
			// 
			// scTop.Panel2
			// 
			this.scTop.Panel2.Controls.Add(this.tcLog);
			this.scTop.Size = new System.Drawing.Size(615, 507);
			this.scTop.SplitterDistance = 270;
			this.scTop.TabIndex = 0;
			// 
			// btnStartStop
			// 
			this.btnStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnStartStop.Image = global::Blk.Gui.Properties.Resources.run16;
			this.btnStartStop.Location = new System.Drawing.Point(451, 244);
			this.btnStartStop.Name = "btnStartStop";
			this.btnStartStop.Size = new System.Drawing.Size(153, 23);
			this.btnStartStop.TabIndex = 6;
			this.btnStartStop.Text = "Start Blackbord";
			this.btnStartStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnStartStop.UseVisualStyleBackColor = true;
			this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
			// 
			// gbGeneralActions
			// 
			this.gbGeneralActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbGeneralActions.Controls.Add(this.btnSutdownSequence);
			this.gbGeneralActions.Controls.Add(this.btnStartSequence);
			this.gbGeneralActions.Controls.Add(this.btnRestartTimer);
			this.gbGeneralActions.Controls.Add(this.btnRestartBlackboard);
			this.gbGeneralActions.Controls.Add(this.btnRestartTest);
			this.gbGeneralActions.Enabled = false;
			this.gbGeneralActions.Location = new System.Drawing.Point(445, 12);
			this.gbGeneralActions.Name = "gbGeneralActions";
			this.gbGeneralActions.Size = new System.Drawing.Size(165, 226);
			this.gbGeneralActions.TabIndex = 7;
			this.gbGeneralActions.TabStop = false;
			this.gbGeneralActions.Text = "General Actions";
			// 
			// btnStartSequence
			// 
			this.btnStartSequence.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnStartSequence.Location = new System.Drawing.Point(6, 153);
			this.btnStartSequence.Name = "btnStartSequence";
			this.btnStartSequence.Size = new System.Drawing.Size(153, 23);
			this.btnStartSequence.TabIndex = 101;
			this.btnStartSequence.Text = "Run start sequence";
			this.btnStartSequence.UseVisualStyleBackColor = true;
			this.btnStartSequence.Click += new System.EventHandler(this.btnStartSequence_Click);
			// 
			// btnRestartTimer
			// 
			this.btnRestartTimer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnRestartTimer.Location = new System.Drawing.Point(6, 124);
			this.btnRestartTimer.Name = "btnRestartTimer";
			this.btnRestartTimer.Size = new System.Drawing.Size(153, 23);
			this.btnRestartTimer.TabIndex = 100;
			this.btnRestartTimer.Text = "Restart Timer";
			this.btnRestartTimer.UseVisualStyleBackColor = true;
			this.btnRestartTimer.Click += new System.EventHandler(this.btnRestartTimer_Click);
			// 
			// btnRestartBlackboard
			// 
			this.btnRestartBlackboard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnRestartBlackboard.Location = new System.Drawing.Point(6, 95);
			this.btnRestartBlackboard.Name = "btnRestartBlackboard";
			this.btnRestartBlackboard.Size = new System.Drawing.Size(153, 23);
			this.btnRestartBlackboard.TabIndex = 100;
			this.btnRestartBlackboard.Text = "Restart Blackboard";
			this.btnRestartBlackboard.UseVisualStyleBackColor = true;
			this.btnRestartBlackboard.Click += new System.EventHandler(this.btnRestartBlackboard_Click);
			// 
			// btnRestartTest
			// 
			this.btnRestartTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnRestartTest.Image = global::Blk.Gui.Properties.Resources.refresh48;
			this.btnRestartTest.Location = new System.Drawing.Point(6, 19);
			this.btnRestartTest.Name = "btnRestartTest";
			this.btnRestartTest.Size = new System.Drawing.Size(153, 70);
			this.btnRestartTest.TabIndex = 99;
			this.btnRestartTest.Text = "Restart Test";
			this.btnRestartTest.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnRestartTest.UseVisualStyleBackColor = true;
			this.btnRestartTest.Click += new System.EventHandler(this.btnRestartTest_Click);
			// 
			// gbBlackboardFiles
			// 
			this.gbBlackboardFiles.Controls.Add(this.chkAutoLog);
			this.gbBlackboardFiles.Controls.Add(this.chkAutostart);
			this.gbBlackboardFiles.Controls.Add(this.cbModuleStartupMode);
			this.gbBlackboardFiles.Controls.Add(this.btnLoad);
			this.gbBlackboardFiles.Controls.Add(this.btnExploreLog);
			this.gbBlackboardFiles.Controls.Add(this.btnExploreConfig);
			this.gbBlackboardFiles.Controls.Add(this.lblModuleStartupMode);
			this.gbBlackboardFiles.Controls.Add(this.lblLogFile);
			this.gbBlackboardFiles.Controls.Add(this.lblConfigurationFile);
			this.gbBlackboardFiles.Controls.Add(this.txtLogFile);
			this.gbBlackboardFiles.Controls.Add(this.txtConfigurationFile);
			this.gbBlackboardFiles.Location = new System.Drawing.Point(12, 12);
			this.gbBlackboardFiles.Name = "gbBlackboardFiles";
			this.gbBlackboardFiles.Size = new System.Drawing.Size(427, 111);
			this.gbBlackboardFiles.TabIndex = 4;
			this.gbBlackboardFiles.TabStop = false;
			this.gbBlackboardFiles.Text = "Blackboard Files";
			// 
			// chkAutoLog
			// 
			this.chkAutoLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.chkAutoLog.AutoSize = true;
			this.chkAutoLog.Location = new System.Drawing.Point(327, 52);
			this.chkAutoLog.Name = "chkAutoLog";
			this.chkAutoLog.Size = new System.Drawing.Size(65, 17);
			this.chkAutoLog.TabIndex = 7;
			this.chkAutoLog.Text = "Auto log";
			this.chkAutoLog.UseVisualStyleBackColor = true;
			this.chkAutoLog.CheckedChanged += new System.EventHandler(this.chkAutoLog_CheckedChanged);
			// 
			// chkAutostart
			// 
			this.chkAutostart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.chkAutostart.AutoSize = true;
			this.chkAutostart.Location = new System.Drawing.Point(247, 81);
			this.chkAutostart.Name = "chkAutostart";
			this.chkAutostart.Size = new System.Drawing.Size(68, 17);
			this.chkAutostart.TabIndex = 7;
			this.chkAutostart.Text = "Autostart";
			this.chkAutostart.UseVisualStyleBackColor = true;
			// 
			// cbModuleStartupMode
			// 
			this.cbModuleStartupMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbModuleStartupMode.FormattingEnabled = true;
			this.cbModuleStartupMode.Items.AddRange(new object[] {
            "None",
            "Launch All",
            "Kill, then launch",
            "Kill, launch and wait ready",
            "Kill All"});
			this.cbModuleStartupMode.Location = new System.Drawing.Point(103, 79);
			this.cbModuleStartupMode.Name = "cbModuleStartupMode";
			this.cbModuleStartupMode.Size = new System.Drawing.Size(137, 21);
			this.cbModuleStartupMode.TabIndex = 6;
			this.cbModuleStartupMode.SelectedIndexChanged += new System.EventHandler(this.cbModuleStartupMode_SelectedIndexChanged);
			// 
			// btnLoad
			// 
			this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLoad.Location = new System.Drawing.Point(321, 77);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(100, 23);
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
			this.btnExploreLog.Location = new System.Drawing.Point(398, 48);
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
			this.btnExploreConfig.Location = new System.Drawing.Point(398, 19);
			this.btnExploreConfig.Name = "btnExploreConfig";
			this.btnExploreConfig.Size = new System.Drawing.Size(23, 23);
			this.btnExploreConfig.TabIndex = 2;
			this.btnExploreConfig.UseVisualStyleBackColor = true;
			this.btnExploreConfig.Click += new System.EventHandler(this.btnExploreConfig_Click);
			// 
			// lblModuleStartupMode
			// 
			this.lblModuleStartupMode.AutoSize = true;
			this.lblModuleStartupMode.Location = new System.Drawing.Point(7, 82);
			this.lblModuleStartupMode.Name = "lblModuleStartupMode";
			this.lblModuleStartupMode.Size = new System.Drawing.Size(73, 13);
			this.lblModuleStartupMode.TabIndex = 2;
			this.lblModuleStartupMode.Text = "Startup mode:";
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
			this.txtLogFile.Size = new System.Drawing.Size(218, 20);
			this.txtLogFile.TabIndex = 3;
			this.txtLogFile.TextChanged += new System.EventHandler(this.txtLogFile_TextChanged);
			// 
			// txtConfigurationFile
			// 
			this.txtConfigurationFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtConfigurationFile.Location = new System.Drawing.Point(103, 21);
			this.txtConfigurationFile.Name = "txtConfigurationFile";
			this.txtConfigurationFile.Size = new System.Drawing.Size(289, 20);
			this.txtConfigurationFile.TabIndex = 1;
			this.txtConfigurationFile.TextChanged += new System.EventHandler(this.txtConfigurationFile_TextChanged);
			// 
			// gbBlackboardSettings
			// 
			this.gbBlackboardSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.gbBlackboardSettings.Controls.Add(this.nudVerbosity);
			this.gbBlackboardSettings.Controls.Add(this.nudRetrySendAttempts);
			this.gbBlackboardSettings.Controls.Add(this.cbModuleShutdownMode);
			this.gbBlackboardSettings.Controls.Add(this.lblRetrySendAttempts);
			this.gbBlackboardSettings.Controls.Add(this.txtTimeLeft);
			this.gbBlackboardSettings.Controls.Add(this.txtClientsConnected);
			this.gbBlackboardSettings.Controls.Add(this.label1);
			this.gbBlackboardSettings.Controls.Add(this.lblClientsConnected);
			this.gbBlackboardSettings.Controls.Add(this.txtBBAddresses);
			this.gbBlackboardSettings.Controls.Add(this.txtBBInputPort);
			this.gbBlackboardSettings.Controls.Add(this.lblTimeLeft);
			this.gbBlackboardSettings.Controls.Add(this.lblBBAddresses);
			this.gbBlackboardSettings.Controls.Add(this.lblVerbosity);
			this.gbBlackboardSettings.Controls.Add(this.lblBBInputPort);
			this.gbBlackboardSettings.Location = new System.Drawing.Point(12, 129);
			this.gbBlackboardSettings.Name = "gbBlackboardSettings";
			this.gbBlackboardSettings.Size = new System.Drawing.Size(427, 138);
			this.gbBlackboardSettings.TabIndex = 3;
			this.gbBlackboardSettings.TabStop = false;
			this.gbBlackboardSettings.Text = "Blackboard Settings";
			// 
			// nudVerbosity
			// 
			this.nudVerbosity.Location = new System.Drawing.Point(103, 71);
			this.nudVerbosity.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
			this.nudVerbosity.Name = "nudVerbosity";
			this.nudVerbosity.Size = new System.Drawing.Size(100, 20);
			this.nudVerbosity.TabIndex = 4;
			this.nudVerbosity.ValueChanged += new System.EventHandler(this.nudVerbosity_ValueChanged);
			// 
			// nudRetrySendAttempts
			// 
			this.nudRetrySendAttempts.Location = new System.Drawing.Point(321, 71);
			this.nudRetrySendAttempts.Name = "nudRetrySendAttempts";
			this.nudRetrySendAttempts.Size = new System.Drawing.Size(100, 20);
			this.nudRetrySendAttempts.TabIndex = 3;
			this.nudRetrySendAttempts.ValueChanged += new System.EventHandler(this.nudRetrySendAttempts_ValueChanged);
			// 
			// lblRetrySendAttempts
			// 
			this.lblRetrySendAttempts.AutoSize = true;
			this.lblRetrySendAttempts.Location = new System.Drawing.Point(214, 73);
			this.lblRetrySendAttempts.Name = "lblRetrySendAttempts";
			this.lblRetrySendAttempts.Size = new System.Drawing.Size(104, 13);
			this.lblRetrySendAttempts.TabIndex = 2;
			this.lblRetrySendAttempts.Text = "Retry send attempts:";
			// 
			// txtTimeLeft
			// 
			this.txtTimeLeft.Location = new System.Drawing.Point(321, 19);
			this.txtTimeLeft.Name = "txtTimeLeft";
			this.txtTimeLeft.ReadOnly = true;
			this.txtTimeLeft.Size = new System.Drawing.Size(100, 20);
			this.txtTimeLeft.TabIndex = 1;
			// 
			// txtClientsConnected
			// 
			this.txtClientsConnected.Location = new System.Drawing.Point(321, 45);
			this.txtClientsConnected.Name = "txtClientsConnected";
			this.txtClientsConnected.ReadOnly = true;
			this.txtClientsConnected.Size = new System.Drawing.Size(100, 20);
			this.txtClientsConnected.TabIndex = 1;
			// 
			// lblClientsConnected
			// 
			this.lblClientsConnected.AutoSize = true;
			this.lblClientsConnected.Location = new System.Drawing.Point(225, 48);
			this.lblClientsConnected.Name = "lblClientsConnected";
			this.lblClientsConnected.Size = new System.Drawing.Size(95, 13);
			this.lblClientsConnected.TabIndex = 0;
			this.lblClientsConnected.Text = "Clients connected:";
			// 
			// txtBBAddresses
			// 
			this.txtBBAddresses.Location = new System.Drawing.Point(103, 19);
			this.txtBBAddresses.Name = "txtBBAddresses";
			this.txtBBAddresses.ReadOnly = true;
			this.txtBBAddresses.Size = new System.Drawing.Size(100, 20);
			this.txtBBAddresses.TabIndex = 1;
			this.txtBBAddresses.Text = "255.255.255.255";
			// 
			// txtBBInputPort
			// 
			this.txtBBInputPort.Location = new System.Drawing.Point(103, 45);
			this.txtBBInputPort.Name = "txtBBInputPort";
			this.txtBBInputPort.ReadOnly = true;
			this.txtBBInputPort.Size = new System.Drawing.Size(100, 20);
			this.txtBBInputPort.TabIndex = 1;
			// 
			// lblTimeLeft
			// 
			this.lblTimeLeft.AutoSize = true;
			this.lblTimeLeft.Location = new System.Drawing.Point(225, 22);
			this.lblTimeLeft.Name = "lblTimeLeft";
			this.lblTimeLeft.Size = new System.Drawing.Size(54, 13);
			this.lblTimeLeft.TabIndex = 0;
			this.lblTimeLeft.Text = "Time Left:";
			// 
			// lblBBAddresses
			// 
			this.lblBBAddresses.AutoSize = true;
			this.lblBBAddresses.Location = new System.Drawing.Point(7, 22);
			this.lblBBAddresses.Name = "lblBBAddresses";
			this.lblBBAddresses.Size = new System.Drawing.Size(89, 13);
			this.lblBBAddresses.TabIndex = 0;
			this.lblBBAddresses.Text = "BB IP Addresses:";
			// 
			// lblVerbosity
			// 
			this.lblVerbosity.AutoSize = true;
			this.lblVerbosity.Location = new System.Drawing.Point(7, 73);
			this.lblVerbosity.Name = "lblVerbosity";
			this.lblVerbosity.Size = new System.Drawing.Size(74, 13);
			this.lblVerbosity.TabIndex = 0;
			this.lblVerbosity.Text = "Log Verbosity:";
			// 
			// lblBBInputPort
			// 
			this.lblBBInputPort.AutoSize = true;
			this.lblBBInputPort.Location = new System.Drawing.Point(7, 48);
			this.lblBBInputPort.Name = "lblBBInputPort";
			this.lblBBInputPort.Size = new System.Drawing.Size(56, 13);
			this.lblBBInputPort.TabIndex = 0;
			this.lblBBInputPort.Text = "Input Port:";
			// 
			// tcLog
			// 
			this.tcLog.Controls.Add(this.tpDiagnostics);
			this.tcLog.Controls.Add(this.tpInjector);
			this.tcLog.Controls.Add(this.tpMessagePendingList);
			this.tcLog.Controls.Add(this.tpModuleInfo);
			this.tcLog.Controls.Add(this.tpOutputLog);
			this.tcLog.Controls.Add(this.tpRedirectionHistory);
			this.tcLog.Controls.Add(this.tpSharedVars);
			this.tcLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcLog.Location = new System.Drawing.Point(0, 0);
			this.tcLog.Name = "tcLog";
			this.tcLog.SelectedIndex = 0;
			this.tcLog.Size = new System.Drawing.Size(615, 233);
			this.tcLog.TabIndex = 2;
			this.tcLog.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tcLog_Selecting);
			// 
			// tpDiagnostics
			// 
			this.tpDiagnostics.Controls.Add(this.gbRemoteMachineStatus);
			this.tpDiagnostics.Location = new System.Drawing.Point(4, 22);
			this.tpDiagnostics.Name = "tpDiagnostics";
			this.tpDiagnostics.Padding = new System.Windows.Forms.Padding(3);
			this.tpDiagnostics.Size = new System.Drawing.Size(607, 207);
			this.tpDiagnostics.TabIndex = 4;
			this.tpDiagnostics.Text = "Diagnostics";
			this.tpDiagnostics.UseVisualStyleBackColor = true;
			// 
			// gbRemoteMachineStatus
			// 
			this.gbRemoteMachineStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.gbRemoteMachineStatus.Controls.Add(this.flpRemoteMachineStatus);
			this.gbRemoteMachineStatus.Location = new System.Drawing.Point(8, 6);
			this.gbRemoteMachineStatus.Name = "gbRemoteMachineStatus";
			this.gbRemoteMachineStatus.Size = new System.Drawing.Size(200, 187);
			this.gbRemoteMachineStatus.TabIndex = 0;
			this.gbRemoteMachineStatus.TabStop = false;
			this.gbRemoteMachineStatus.Text = "Remote Machine Status";
			// 
			// flpRemoteMachineStatus
			// 
			this.flpRemoteMachineStatus.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flpRemoteMachineStatus.Location = new System.Drawing.Point(3, 16);
			this.flpRemoteMachineStatus.Name = "flpRemoteMachineStatus";
			this.flpRemoteMachineStatus.Size = new System.Drawing.Size(194, 168);
			this.flpRemoteMachineStatus.TabIndex = 0;
			// 
			// tpInjector
			// 
			this.tpInjector.Controls.Add(this.injectorTool);
			this.tpInjector.Location = new System.Drawing.Point(4, 22);
			this.tpInjector.Name = "tpInjector";
			this.tpInjector.Size = new System.Drawing.Size(607, 207);
			this.tpInjector.TabIndex = 6;
			this.tpInjector.Text = "Injection Tool";
			this.tpInjector.UseVisualStyleBackColor = true;
			// 
			// tpMessagePendingList
			// 
			this.tpMessagePendingList.Controls.Add(this.gbCommandsPending);
			this.tpMessagePendingList.Controls.Add(this.gbCommandsWaiting);
			this.tpMessagePendingList.Location = new System.Drawing.Point(4, 22);
			this.tpMessagePendingList.Name = "tpMessagePendingList";
			this.tpMessagePendingList.Padding = new System.Windows.Forms.Padding(3);
			this.tpMessagePendingList.Size = new System.Drawing.Size(607, 207);
			this.tpMessagePendingList.TabIndex = 1;
			this.tpMessagePendingList.Text = "Message Pending List";
			this.tpMessagePendingList.UseVisualStyleBackColor = true;
			// 
			// gbCommandsPending
			// 
			this.gbCommandsPending.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbCommandsPending.Controls.Add(this.lstCommandsPending);
			this.gbCommandsPending.Location = new System.Drawing.Point(297, 6);
			this.gbCommandsPending.Name = "gbCommandsPending";
			this.gbCommandsPending.Size = new System.Drawing.Size(303, 195);
			this.gbCommandsPending.TabIndex = 0;
			this.gbCommandsPending.TabStop = false;
			this.gbCommandsPending.Text = "Commands Pending";
			// 
			// lstCommandsPending
			// 
			this.lstCommandsPending.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lstCommandsPending.FormattingEnabled = true;
			this.lstCommandsPending.Location = new System.Drawing.Point(7, 20);
			this.lstCommandsPending.Name = "lstCommandsPending";
			this.lstCommandsPending.Size = new System.Drawing.Size(290, 160);
			this.lstCommandsPending.TabIndex = 0;
			// 
			// gbCommandsWaiting
			// 
			this.gbCommandsWaiting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbCommandsWaiting.Controls.Add(this.lstCommandsWaiting);
			this.gbCommandsWaiting.Location = new System.Drawing.Point(6, 6);
			this.gbCommandsWaiting.Name = "gbCommandsWaiting";
			this.gbCommandsWaiting.Size = new System.Drawing.Size(285, 195);
			this.gbCommandsWaiting.TabIndex = 0;
			this.gbCommandsWaiting.TabStop = false;
			this.gbCommandsWaiting.Text = "Commands Waiting";
			// 
			// lstCommandsWaiting
			// 
			this.lstCommandsWaiting.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lstCommandsWaiting.FormattingEnabled = true;
			this.lstCommandsWaiting.Location = new System.Drawing.Point(7, 20);
			this.lstCommandsWaiting.Name = "lstCommandsWaiting";
			this.lstCommandsWaiting.Size = new System.Drawing.Size(272, 160);
			this.lstCommandsWaiting.TabIndex = 0;
			// 
			// tpModuleInfo
			// 
			this.tpModuleInfo.Controls.Add(this.interactionTool);
			this.tpModuleInfo.Controls.Add(this.tsModuleToolBar);
			this.tpModuleInfo.Controls.Add(this.gbModuleInfo);
			this.tpModuleInfo.Location = new System.Drawing.Point(4, 22);
			this.tpModuleInfo.Name = "tpModuleInfo";
			this.tpModuleInfo.Padding = new System.Windows.Forms.Padding(3);
			this.tpModuleInfo.Size = new System.Drawing.Size(607, 207);
			this.tpModuleInfo.TabIndex = 2;
			this.tpModuleInfo.Text = "Module Information";
			this.tpModuleInfo.UseVisualStyleBackColor = true;
			// 
			// tsModuleToolBar
			// 
			this.tsModuleToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnModuleEnable,
            this.tsModuletoolBarSeparator1,
            this.btnModuleStartStopModule,
            this.btnModuleRestartModule,
            this.btnModule,
            this.btnShowModuleActions,
            this.tsModuletoolBarSeparator2,
            this.btnModuleSimulation,
            this.tsModuleProcess});
			this.tsModuleToolBar.Location = new System.Drawing.Point(3, 3);
			this.tsModuleToolBar.Name = "tsModuleToolBar";
			this.tsModuleToolBar.Size = new System.Drawing.Size(601, 25);
			this.tsModuleToolBar.TabIndex = 3;
			this.tsModuleToolBar.Text = "Module Toolbar";
			// 
			// btnModuleEnable
			// 
			this.btnModuleEnable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnModuleEnable.Image = global::Blk.Gui.Properties.Resources.ExitRed_16;
			this.btnModuleEnable.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnModuleEnable.Name = "btnModuleEnable";
			this.btnModuleEnable.Size = new System.Drawing.Size(23, 22);
			this.btnModuleEnable.Text = "Disable module";
			this.btnModuleEnable.Click += new System.EventHandler(this.btnModuleEnable_Click);
			// 
			// tsModuletoolBarSeparator1
			// 
			this.tsModuletoolBarSeparator1.Name = "tsModuletoolBarSeparator1";
			this.tsModuletoolBarSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// btnModuleStartStopModule
			// 
			this.btnModuleStartStopModule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnModuleStartStopModule.Image = global::Blk.Gui.Properties.Resources.run16;
			this.btnModuleStartStopModule.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnModuleStartStopModule.Name = "btnModuleStartStopModule";
			this.btnModuleStartStopModule.Size = new System.Drawing.Size(23, 22);
			this.btnModuleStartStopModule.Text = "Stop Module";
			this.btnModuleStartStopModule.Click += new System.EventHandler(this.btnModuleStartStopModule_Click);
			// 
			// btnModuleRestartModule
			// 
			this.btnModuleRestartModule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnModuleRestartModule.Image = global::Blk.Gui.Properties.Resources.refresh16;
			this.btnModuleRestartModule.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnModuleRestartModule.Name = "btnModuleRestartModule";
			this.btnModuleRestartModule.Size = new System.Drawing.Size(23, 22);
			this.btnModuleRestartModule.Text = "Restart Module";
			this.btnModuleRestartModule.Click += new System.EventHandler(this.btnModuleRestartModule_Click);
			// 
			// btnModule
			// 
			this.btnModule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnModule.Enabled = false;
			this.btnModule.Image = global::Blk.Gui.Properties.Resources.Unknown16;
			this.btnModule.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnModule.Name = "btnModule";
			this.btnModule.Size = new System.Drawing.Size(23, 22);
			this.btnModule.Text = "Stop Client";
			this.btnModule.Visible = false;
			// 
			// btnShowModuleActions
			// 
			this.btnShowModuleActions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnShowModuleActions.Image = global::Blk.Gui.Properties.Resources.Thunder_16;
			this.btnShowModuleActions.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnShowModuleActions.Name = "btnShowModuleActions";
			this.btnShowModuleActions.Size = new System.Drawing.Size(23, 22);
			this.btnShowModuleActions.Text = "Show module actions";
			this.btnShowModuleActions.Click += new System.EventHandler(this.btnShowModuleActions_Click);
			// 
			// tsModuletoolBarSeparator2
			// 
			this.tsModuletoolBarSeparator2.Name = "tsModuletoolBarSeparator2";
			this.tsModuletoolBarSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// btnModuleSimulation
			// 
			this.btnModuleSimulation.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSimulationEnabled,
            this.btnSimulationDisabled});
			this.btnModuleSimulation.Image = global::Blk.Gui.Properties.Resources.CircleGray_16;
			this.btnModuleSimulation.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnModuleSimulation.Name = "btnModuleSimulation";
			this.btnModuleSimulation.Size = new System.Drawing.Size(93, 22);
			this.btnModuleSimulation.Text = "Simulation";
			// 
			// btnSimulationEnabled
			// 
			this.btnSimulationEnabled.Name = "btnSimulationEnabled";
			this.btnSimulationEnabled.Size = new System.Drawing.Size(119, 22);
			this.btnSimulationEnabled.Text = "&Enabled";
			this.btnSimulationEnabled.Click += new System.EventHandler(this.btnSimulationEnabled_Click);
			// 
			// btnSimulationDisabled
			// 
			this.btnSimulationDisabled.Name = "btnSimulationDisabled";
			this.btnSimulationDisabled.Size = new System.Drawing.Size(119, 22);
			this.btnSimulationDisabled.Text = "&Disabled";
			this.btnSimulationDisabled.Click += new System.EventHandler(this.btnSimulationDisabled_Click);
			// 
			// tsModuleProcess
			// 
			this.tsModuleProcess.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnModuleProcessRun,
            this.btnModuleProcessCheckRun,
            this.btnModuleProcessKill,
            this.tsModuleProcessSeparator1,
            this.btnModuleProcessZombieCheck});
			this.tsModuleProcess.Image = global::Blk.Gui.Properties.Resources.Gear16;
			this.tsModuleProcess.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsModuleProcess.Name = "tsModuleProcess";
			this.tsModuleProcess.Size = new System.Drawing.Size(120, 22);
			this.tsModuleProcess.Text = "Module process";
			// 
			// btnModuleProcessRun
			// 
			this.btnModuleProcessRun.Image = global::Blk.Gui.Properties.Resources.ProcessWarning16;
			this.btnModuleProcessRun.Name = "btnModuleProcessRun";
			this.btnModuleProcessRun.Size = new System.Drawing.Size(221, 22);
			this.btnModuleProcessRun.Text = "Run (&unconditionally)";
			this.btnModuleProcessRun.Click += new System.EventHandler(this.btnModuleProcessRun_Click);
			// 
			// btnModuleProcessCheckRun
			// 
			this.btnModuleProcessCheckRun.Image = global::Blk.Gui.Properties.Resources.ProcessOk16;
			this.btnModuleProcessCheckRun.Name = "btnModuleProcessCheckRun";
			this.btnModuleProcessCheckRun.Size = new System.Drawing.Size(221, 22);
			this.btnModuleProcessCheckRun.Text = "&Run (if not running)";
			this.btnModuleProcessCheckRun.Click += new System.EventHandler(this.btnModuleProcessCheckRun_Click);
			// 
			// btnModuleProcessKill
			// 
			this.btnModuleProcessKill.Image = global::Blk.Gui.Properties.Resources.Skull16;
			this.btnModuleProcessKill.Name = "btnModuleProcessKill";
			this.btnModuleProcessKill.Size = new System.Drawing.Size(221, 22);
			this.btnModuleProcessKill.Text = "Close and &Kill";
			this.btnModuleProcessKill.Click += new System.EventHandler(this.btnModuleProcessKill_Click);
			// 
			// tsModuleProcessSeparator1
			// 
			this.tsModuleProcessSeparator1.Name = "tsModuleProcessSeparator1";
			this.tsModuleProcessSeparator1.Size = new System.Drawing.Size(218, 6);
			// 
			// btnModuleProcessZombieCheck
			// 
			this.btnModuleProcessZombieCheck.Image = global::Blk.Gui.Properties.Resources.ProcessInfo16;
			this.btnModuleProcessZombieCheck.Name = "btnModuleProcessZombieCheck";
			this.btnModuleProcessZombieCheck.Size = new System.Drawing.Size(221, 22);
			this.btnModuleProcessZombieCheck.Text = "&Check for zombie processes";
			this.btnModuleProcessZombieCheck.Click += new System.EventHandler(this.btnModuleProcessZombieCheck_Click);
			// 
			// gbModuleInfo
			// 
			this.gbModuleInfo.Controls.Add(this.txtModuleAuthor);
			this.gbModuleInfo.Controls.Add(this.txtOutputPort);
			this.gbModuleInfo.Controls.Add(this.txtModuleAlias);
			this.gbModuleInfo.Controls.Add(this.txtModuleName);
			this.gbModuleInfo.Controls.Add(this.txtModuleExeArgs);
			this.gbModuleInfo.Controls.Add(this.txtModuleProcessName);
			this.gbModuleInfo.Controls.Add(this.txtModuleExePath);
			this.gbModuleInfo.Controls.Add(this.txtRemoteIPAddress);
			this.gbModuleInfo.Controls.Add(this.lblModuleAlias);
			this.gbModuleInfo.Controls.Add(this.lblModuleAuthor);
			this.gbModuleInfo.Controls.Add(this.lblOutputPort);
			this.gbModuleInfo.Controls.Add(this.lblModuleName);
			this.gbModuleInfo.Controls.Add(this.lblModuleExeArgs);
			this.gbModuleInfo.Controls.Add(this.lblModuleProcessName);
			this.gbModuleInfo.Controls.Add(this.lblModuleExePath);
			this.gbModuleInfo.Controls.Add(this.lblRemoteIPAddress);
			this.gbModuleInfo.Location = new System.Drawing.Point(6, 31);
			this.gbModuleInfo.Name = "gbModuleInfo";
			this.gbModuleInfo.Size = new System.Drawing.Size(595, 129);
			this.gbModuleInfo.TabIndex = 1;
			this.gbModuleInfo.TabStop = false;
			this.gbModuleInfo.Text = "Module Information";
			// 
			// txtModuleAuthor
			// 
			this.txtModuleAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtModuleAuthor.Location = new System.Drawing.Point(460, 45);
			this.txtModuleAuthor.Name = "txtModuleAuthor";
			this.txtModuleAuthor.ReadOnly = true;
			this.txtModuleAuthor.Size = new System.Drawing.Size(129, 20);
			this.txtModuleAuthor.TabIndex = 1;
			// 
			// txtOutputPort
			// 
			this.txtOutputPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtOutputPort.Location = new System.Drawing.Point(460, 19);
			this.txtOutputPort.Name = "txtOutputPort";
			this.txtOutputPort.ReadOnly = true;
			this.txtOutputPort.Size = new System.Drawing.Size(129, 20);
			this.txtOutputPort.TabIndex = 1;
			// 
			// txtModuleAlias
			// 
			this.txtModuleAlias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtModuleAlias.Location = new System.Drawing.Point(264, 19);
			this.txtModuleAlias.Name = "txtModuleAlias";
			this.txtModuleAlias.ReadOnly = true;
			this.txtModuleAlias.Size = new System.Drawing.Size(120, 20);
			this.txtModuleAlias.TabIndex = 1;
			// 
			// txtModuleName
			// 
			this.txtModuleName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtModuleName.Location = new System.Drawing.Point(100, 19);
			this.txtModuleName.Name = "txtModuleName";
			this.txtModuleName.ReadOnly = true;
			this.txtModuleName.Size = new System.Drawing.Size(120, 20);
			this.txtModuleName.TabIndex = 1;
			// 
			// txtModuleExeArgs
			// 
			this.txtModuleExeArgs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtModuleExeArgs.Location = new System.Drawing.Point(100, 97);
			this.txtModuleExeArgs.Name = "txtModuleExeArgs";
			this.txtModuleExeArgs.ReadOnly = true;
			this.txtModuleExeArgs.Size = new System.Drawing.Size(489, 20);
			this.txtModuleExeArgs.TabIndex = 1;
			// 
			// txtModuleProcessName
			// 
			this.txtModuleProcessName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtModuleProcessName.Location = new System.Drawing.Point(100, 71);
			this.txtModuleProcessName.Name = "txtModuleProcessName";
			this.txtModuleProcessName.ReadOnly = true;
			this.txtModuleProcessName.Size = new System.Drawing.Size(120, 20);
			this.txtModuleProcessName.TabIndex = 1;
			// 
			// txtModuleExePath
			// 
			this.txtModuleExePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtModuleExePath.Location = new System.Drawing.Point(264, 71);
			this.txtModuleExePath.Name = "txtModuleExePath";
			this.txtModuleExePath.ReadOnly = true;
			this.txtModuleExePath.Size = new System.Drawing.Size(325, 20);
			this.txtModuleExePath.TabIndex = 1;
			// 
			// txtRemoteIPAddress
			// 
			this.txtRemoteIPAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtRemoteIPAddress.Location = new System.Drawing.Point(100, 45);
			this.txtRemoteIPAddress.Name = "txtRemoteIPAddress";
			this.txtRemoteIPAddress.ReadOnly = true;
			this.txtRemoteIPAddress.Size = new System.Drawing.Size(284, 20);
			this.txtRemoteIPAddress.TabIndex = 1;
			// 
			// lblModuleAlias
			// 
			this.lblModuleAlias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblModuleAlias.AutoSize = true;
			this.lblModuleAlias.Location = new System.Drawing.Point(226, 22);
			this.lblModuleAlias.Name = "lblModuleAlias";
			this.lblModuleAlias.Size = new System.Drawing.Size(32, 13);
			this.lblModuleAlias.TabIndex = 0;
			this.lblModuleAlias.Text = "Alias:";
			// 
			// lblModuleAuthor
			// 
			this.lblModuleAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblModuleAuthor.AutoSize = true;
			this.lblModuleAuthor.Location = new System.Drawing.Point(390, 48);
			this.lblModuleAuthor.Name = "lblModuleAuthor";
			this.lblModuleAuthor.Size = new System.Drawing.Size(41, 13);
			this.lblModuleAuthor.TabIndex = 0;
			this.lblModuleAuthor.Text = "Author:";
			// 
			// lblOutputPort
			// 
			this.lblOutputPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblOutputPort.AutoSize = true;
			this.lblOutputPort.Location = new System.Drawing.Point(390, 22);
			this.lblOutputPort.Name = "lblOutputPort";
			this.lblOutputPort.Size = new System.Drawing.Size(64, 13);
			this.lblOutputPort.TabIndex = 0;
			this.lblOutputPort.Text = "Output Port:";
			// 
			// lblModuleName
			// 
			this.lblModuleName.AutoSize = true;
			this.lblModuleName.Location = new System.Drawing.Point(6, 22);
			this.lblModuleName.Name = "lblModuleName";
			this.lblModuleName.Size = new System.Drawing.Size(76, 13);
			this.lblModuleName.TabIndex = 0;
			this.lblModuleName.Text = "Module Name:";
			// 
			// lblModuleExeArgs
			// 
			this.lblModuleExeArgs.AutoSize = true;
			this.lblModuleExeArgs.Location = new System.Drawing.Point(6, 100);
			this.lblModuleExeArgs.Name = "lblModuleExeArgs";
			this.lblModuleExeArgs.Size = new System.Drawing.Size(60, 13);
			this.lblModuleExeArgs.TabIndex = 0;
			this.lblModuleExeArgs.Text = "Arguments:";
			// 
			// lblModuleProcessName
			// 
			this.lblModuleProcessName.AutoSize = true;
			this.lblModuleProcessName.Location = new System.Drawing.Point(6, 74);
			this.lblModuleProcessName.Name = "lblModuleProcessName";
			this.lblModuleProcessName.Size = new System.Drawing.Size(77, 13);
			this.lblModuleProcessName.TabIndex = 0;
			this.lblModuleProcessName.Text = "Process name:";
			// 
			// lblModuleExePath
			// 
			this.lblModuleExePath.AutoSize = true;
			this.lblModuleExePath.Location = new System.Drawing.Point(226, 74);
			this.lblModuleExePath.Name = "lblModuleExePath";
			this.lblModuleExePath.Size = new System.Drawing.Size(32, 13);
			this.lblModuleExePath.TabIndex = 0;
			this.lblModuleExePath.Text = "Path:";
			// 
			// lblRemoteIPAddress
			// 
			this.lblRemoteIPAddress.AutoSize = true;
			this.lblRemoteIPAddress.Location = new System.Drawing.Point(6, 48);
			this.lblRemoteIPAddress.Name = "lblRemoteIPAddress";
			this.lblRemoteIPAddress.Size = new System.Drawing.Size(88, 13);
			this.lblRemoteIPAddress.TabIndex = 0;
			this.lblRemoteIPAddress.Text = "Remote Address:";
			// 
			// tpOutputLog
			// 
			this.tpOutputLog.Controls.Add(this.btnClearOutputLog);
			this.tpOutputLog.Controls.Add(this.txtOutputLog);
			this.tpOutputLog.Controls.Add(this.btnSaveOutputLog);
			this.tpOutputLog.Location = new System.Drawing.Point(4, 22);
			this.tpOutputLog.Name = "tpOutputLog";
			this.tpOutputLog.Padding = new System.Windows.Forms.Padding(3);
			this.tpOutputLog.Size = new System.Drawing.Size(607, 207);
			this.tpOutputLog.TabIndex = 0;
			this.tpOutputLog.Text = "Output Log";
			this.tpOutputLog.UseVisualStyleBackColor = true;
			// 
			// btnClearOutputLog
			// 
			this.btnClearOutputLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClearOutputLog.Location = new System.Drawing.Point(370, 178);
			this.btnClearOutputLog.Name = "btnClearOutputLog";
			this.btnClearOutputLog.Size = new System.Drawing.Size(75, 23);
			this.btnClearOutputLog.TabIndex = 1;
			this.btnClearOutputLog.Text = "Clear Log";
			this.btnClearOutputLog.UseVisualStyleBackColor = true;
			this.btnClearOutputLog.Click += new System.EventHandler(this.btnClearOutputLog_Click);
			// 
			// txtOutputLog
			// 
			this.txtOutputLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtOutputLog.Location = new System.Drawing.Point(6, 6);
			this.txtOutputLog.MaxLength = 65536;
			this.txtOutputLog.Multiline = true;
			this.txtOutputLog.Name = "txtOutputLog";
			this.txtOutputLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtOutputLog.Size = new System.Drawing.Size(595, 166);
			this.txtOutputLog.TabIndex = 0;
			this.txtOutputLog.WordWrap = false;
			// 
			// btnSaveOutputLog
			// 
			this.btnSaveOutputLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSaveOutputLog.Location = new System.Drawing.Point(451, 178);
			this.btnSaveOutputLog.Name = "btnSaveOutputLog";
			this.btnSaveOutputLog.Size = new System.Drawing.Size(150, 23);
			this.btnSaveOutputLog.TabIndex = 1;
			this.btnSaveOutputLog.Text = "Save Output Log to...";
			this.btnSaveOutputLog.UseVisualStyleBackColor = true;
			this.btnSaveOutputLog.Click += new System.EventHandler(this.btnSaveOutputLog_Click);
			// 
			// tpRedirectionHistory
			// 
			this.tpRedirectionHistory.Controls.Add(this.lvwRedirectionHistory);
			this.tpRedirectionHistory.Location = new System.Drawing.Point(4, 22);
			this.tpRedirectionHistory.Name = "tpRedirectionHistory";
			this.tpRedirectionHistory.Padding = new System.Windows.Forms.Padding(3);
			this.tpRedirectionHistory.Size = new System.Drawing.Size(607, 207);
			this.tpRedirectionHistory.TabIndex = 3;
			this.tpRedirectionHistory.Text = "Redirection History";
			this.tpRedirectionHistory.UseVisualStyleBackColor = true;
			// 
			// lvwRedirectionHistory
			// 
			this.lvwRedirectionHistory.AllowColumnReorder = true;
			this.lvwRedirectionHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chCommand,
            this.chResponse,
            this.chCommandSource,
            this.chCommandDestination,
            this.chCommandSent,
            this.chResponseArrival,
            this.chInfo,
            this.chResponseSent});
			this.lvwRedirectionHistory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvwRedirectionHistory.FullRowSelect = true;
			this.lvwRedirectionHistory.Location = new System.Drawing.Point(3, 3);
			this.lvwRedirectionHistory.Name = "lvwRedirectionHistory";
			this.lvwRedirectionHistory.ShowGroups = false;
			this.lvwRedirectionHistory.Size = new System.Drawing.Size(601, 201);
			this.lvwRedirectionHistory.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.lvwRedirectionHistory.TabIndex = 0;
			this.lvwRedirectionHistory.UseCompatibleStateImageBehavior = false;
			this.lvwRedirectionHistory.View = System.Windows.Forms.View.Details;
			this.lvwRedirectionHistory.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwRedirectionHistory_ColumnClick);
			// 
			// chCommand
			// 
			this.chCommand.Text = "Command";
			this.chCommand.Width = 250;
			// 
			// chResponse
			// 
			this.chResponse.Text = "Response";
			this.chResponse.Width = 250;
			// 
			// chCommandSource
			// 
			this.chCommandSource.Text = "Cmd Source";
			this.chCommandSource.Width = 78;
			// 
			// chCommandDestination
			// 
			this.chCommandDestination.Text = "Cmd Destination";
			this.chCommandDestination.Width = 102;
			// 
			// chCommandSent
			// 
			this.chCommandSent.Text = "Command Sent On";
			this.chCommandSent.Width = 111;
			// 
			// chResponseArrival
			// 
			this.chResponseArrival.Text = "Response Arrival";
			this.chResponseArrival.Width = 101;
			// 
			// chInfo
			// 
			this.chInfo.Text = "Info";
			// 
			// chResponseSent
			// 
			this.chResponseSent.Text = "Response Sent";
			// 
			// tpSharedVars
			// 
			this.tpSharedVars.Controls.Add(this.pgSelectedSharedVar);
			this.tpSharedVars.Controls.Add(this.lvSharedVariables);
			this.tpSharedVars.Location = new System.Drawing.Point(4, 22);
			this.tpSharedVars.Name = "tpSharedVars";
			this.tpSharedVars.Padding = new System.Windows.Forms.Padding(3);
			this.tpSharedVars.Size = new System.Drawing.Size(607, 207);
			this.tpSharedVars.TabIndex = 5;
			this.tpSharedVars.Text = "Shared Variables";
			this.tpSharedVars.UseVisualStyleBackColor = true;
			// 
			// pgSelectedSharedVar
			// 
			this.pgSelectedSharedVar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pgSelectedSharedVar.Location = new System.Drawing.Point(441, 6);
			this.pgSelectedSharedVar.Name = "pgSelectedSharedVar";
			this.pgSelectedSharedVar.Size = new System.Drawing.Size(160, 195);
			this.pgSelectedSharedVar.TabIndex = 1;
			// 
			// lvSharedVariables
			// 
			this.lvSharedVariables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lvSharedVariables.Location = new System.Drawing.Point(6, 6);
			this.lvSharedVariables.Name = "lvSharedVariables";
			this.lvSharedVariables.Size = new System.Drawing.Size(429, 195);
			this.lvSharedVariables.TabIndex = 0;
			this.lvSharedVariables.UseCompatibleStateImageBehavior = false;
			this.lvSharedVariables.View = System.Windows.Forms.View.List;
			this.lvSharedVariables.SelectedIndexChanged += new System.EventHandler(this.lvSharedVariables_SelectedIndexChanged);
			// 
			// mainMenu
			// 
			this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuiBlackboard,
            this.mnuiModules,
            this.mnuiPlugins});
			this.mainMenu.Location = new System.Drawing.Point(0, 0);
			this.mainMenu.Name = "mainMenu";
			this.mainMenu.Size = new System.Drawing.Size(838, 24);
			this.mainMenu.TabIndex = 1;
			this.mainMenu.Text = "menuStrip1";
			// 
			// mnuiBlackboard
			// 
			this.mnuiBlackboard.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuiBlackboard_Load,
            this.mnuiBlackboard_Reload,
            this.mnuiBlackboard_Separator1,
            this.mnuiBlackboard_StartStop,
            this.mnuiBlackboard_Restart,
            this.mnuiBlackboard_Separator2,
            this.mnuiBlackboard_OpenCfgFile,
            this.mnuiBlackboard_OpenLog,
            this.mnuiBlackboard_Separator3,
            this.mnuiBlackboard_Exit});
			this.mnuiBlackboard.Name = "mnuiBlackboard";
			this.mnuiBlackboard.Size = new System.Drawing.Size(78, 20);
			this.mnuiBlackboard.Text = "&Blackboard";
			// 
			// mnuiBlackboard_Load
			// 
			this.mnuiBlackboard_Load.Name = "mnuiBlackboard_Load";
			this.mnuiBlackboard_Load.Size = new System.Drawing.Size(197, 22);
			this.mnuiBlackboard_Load.Text = "Load Blackboard";
			this.mnuiBlackboard_Load.Click += new System.EventHandler(this.mnuiBlackboard_Load_Click);
			// 
			// mnuiBlackboard_Reload
			// 
			this.mnuiBlackboard_Reload.Enabled = false;
			this.mnuiBlackboard_Reload.Name = "mnuiBlackboard_Reload";
			this.mnuiBlackboard_Reload.Size = new System.Drawing.Size(197, 22);
			this.mnuiBlackboard_Reload.Text = "Reload Blackboard";
			this.mnuiBlackboard_Reload.Click += new System.EventHandler(this.mnuiBlackboard_Reload_Click);
			// 
			// mnuiBlackboard_Separator1
			// 
			this.mnuiBlackboard_Separator1.Name = "mnuiBlackboard_Separator1";
			this.mnuiBlackboard_Separator1.Size = new System.Drawing.Size(194, 6);
			// 
			// mnuiBlackboard_StartStop
			// 
			this.mnuiBlackboard_StartStop.Enabled = false;
			this.mnuiBlackboard_StartStop.Name = "mnuiBlackboard_StartStop";
			this.mnuiBlackboard_StartStop.Size = new System.Drawing.Size(197, 22);
			this.mnuiBlackboard_StartStop.Text = "&Start/Stop";
			this.mnuiBlackboard_StartStop.Click += new System.EventHandler(this.mnuiBlackboard_StartStop_Click);
			// 
			// mnuiBlackboard_Restart
			// 
			this.mnuiBlackboard_Restart.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuiBlackboard_Restart_Blackboard,
            this.mnuiBlackboard_Restart_Test,
            this.mnuiBlackboard_Restart_Timer});
			this.mnuiBlackboard_Restart.Enabled = false;
			this.mnuiBlackboard_Restart.Name = "mnuiBlackboard_Restart";
			this.mnuiBlackboard_Restart.Size = new System.Drawing.Size(197, 22);
			this.mnuiBlackboard_Restart.Text = "Restart";
			// 
			// mnuiBlackboard_Restart_Blackboard
			// 
			this.mnuiBlackboard_Restart_Blackboard.Name = "mnuiBlackboard_Restart_Blackboard";
			this.mnuiBlackboard_Restart_Blackboard.Size = new System.Drawing.Size(133, 22);
			this.mnuiBlackboard_Restart_Blackboard.Text = "&Blackboard";
			this.mnuiBlackboard_Restart_Blackboard.Click += new System.EventHandler(this.mnuiBlackboard_Restart_Blackboard_Click);
			// 
			// mnuiBlackboard_Restart_Test
			// 
			this.mnuiBlackboard_Restart_Test.Name = "mnuiBlackboard_Restart_Test";
			this.mnuiBlackboard_Restart_Test.Size = new System.Drawing.Size(133, 22);
			this.mnuiBlackboard_Restart_Test.Text = "T&est";
			this.mnuiBlackboard_Restart_Test.Click += new System.EventHandler(this.mnuiBlackboard_Restart_Test_Click);
			// 
			// mnuiBlackboard_Restart_Timer
			// 
			this.mnuiBlackboard_Restart_Timer.Name = "mnuiBlackboard_Restart_Timer";
			this.mnuiBlackboard_Restart_Timer.Size = new System.Drawing.Size(133, 22);
			this.mnuiBlackboard_Restart_Timer.Text = "T&imer";
			this.mnuiBlackboard_Restart_Timer.Click += new System.EventHandler(this.mnuiBlackboard_Restart_Timer_Click);
			// 
			// mnuiBlackboard_Separator2
			// 
			this.mnuiBlackboard_Separator2.Name = "mnuiBlackboard_Separator2";
			this.mnuiBlackboard_Separator2.Size = new System.Drawing.Size(194, 6);
			// 
			// mnuiBlackboard_OpenCfgFile
			// 
			this.mnuiBlackboard_OpenCfgFile.Name = "mnuiBlackboard_OpenCfgFile";
			this.mnuiBlackboard_OpenCfgFile.Size = new System.Drawing.Size(197, 22);
			this.mnuiBlackboard_OpenCfgFile.Text = "Open configuration file";
			this.mnuiBlackboard_OpenCfgFile.Click += new System.EventHandler(this.mnuiBlackboard_OpenCfgFile_Click);
			// 
			// mnuiBlackboard_OpenLog
			// 
			this.mnuiBlackboard_OpenLog.Name = "mnuiBlackboard_OpenLog";
			this.mnuiBlackboard_OpenLog.Size = new System.Drawing.Size(197, 22);
			this.mnuiBlackboard_OpenLog.Text = "Open log file";
			this.mnuiBlackboard_OpenLog.Click += new System.EventHandler(this.mnuiBlackboard_OpenLog_Click);
			// 
			// mnuiBlackboard_Separator3
			// 
			this.mnuiBlackboard_Separator3.Name = "mnuiBlackboard_Separator3";
			this.mnuiBlackboard_Separator3.Size = new System.Drawing.Size(194, 6);
			// 
			// mnuiBlackboard_Exit
			// 
			this.mnuiBlackboard_Exit.Name = "mnuiBlackboard_Exit";
			this.mnuiBlackboard_Exit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.mnuiBlackboard_Exit.Size = new System.Drawing.Size(197, 22);
			this.mnuiBlackboard_Exit.Text = "&Exit";
			this.mnuiBlackboard_Exit.Click += new System.EventHandler(this.mnuiBlackboard_Exit_Click);
			// 
			// mnuiModules
			// 
			this.mnuiModules.Name = "mnuiModules";
			this.mnuiModules.Size = new System.Drawing.Size(65, 20);
			this.mnuiModules.Text = "&Modules";
			// 
			// mnuiPlugins
			// 
			this.mnuiPlugins.Name = "mnuiPlugins";
			this.mnuiPlugins.Size = new System.Drawing.Size(58, 20);
			this.mnuiPlugins.Text = "&Plugins";
			// 
			// cbModuleShutdownMode
			// 
			this.cbModuleShutdownMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbModuleShutdownMode.FormattingEnabled = true;
			this.cbModuleShutdownMode.Items.AddRange(new object[] {
            "None",
            "Close only",
            "Kill only",
            "Close, if not closes kill"});
			this.cbModuleShutdownMode.Location = new System.Drawing.Point(103, 97);
			this.cbModuleShutdownMode.Name = "cbModuleShutdownMode";
			this.cbModuleShutdownMode.Size = new System.Drawing.Size(100, 21);
			this.cbModuleShutdownMode.TabIndex = 6;
			this.cbModuleShutdownMode.SelectedIndexChanged += new System.EventHandler(this.cbModuleShutdownMode_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 100);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Shutdown mode:";
			// 
			// btnSutdownSequence
			// 
			this.btnSutdownSequence.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnSutdownSequence.Location = new System.Drawing.Point(6, 182);
			this.btnSutdownSequence.Name = "btnSutdownSequence";
			this.btnSutdownSequence.Size = new System.Drawing.Size(153, 23);
			this.btnSutdownSequence.TabIndex = 101;
			this.btnSutdownSequence.Text = "Run shutdown sequence";
			this.btnSutdownSequence.UseVisualStyleBackColor = true;
			this.btnSutdownSequence.Click += new System.EventHandler(this.btnShutdownSequence_Click);
			// 
			// mnuShutdownSequence
			// 
			this.mnuShutdownSequence.Name = "contextMenuStrip1";
			this.mnuShutdownSequence.ShowImageMargin = false;
			this.mnuShutdownSequence.Size = new System.Drawing.Size(36, 4);
			// 
			// injectorTool
			// 
			this.injectorTool.Blackboard = null;
			this.injectorTool.Dock = System.Windows.Forms.DockStyle.Fill;
			this.injectorTool.Location = new System.Drawing.Point(0, 0);
			this.injectorTool.Name = "injectorTool";
			this.injectorTool.Size = new System.Drawing.Size(607, 207);
			this.injectorTool.TabIndex = 0;
			// 
			// interactionTool
			// 
			this.interactionTool.Blackboard = null;
			this.interactionTool.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.interactionTool.Location = new System.Drawing.Point(3, 142);
			this.interactionTool.MaximumSize = new System.Drawing.Size(0, 62);
			this.interactionTool.MinimumSize = new System.Drawing.Size(300, 62);
			this.interactionTool.Name = "interactionTool";
			this.interactionTool.SelectedModule = null;
			this.interactionTool.Size = new System.Drawing.Size(601, 62);
			this.interactionTool.TabIndex = 4;
			// 
			// FrmBlackboard
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(838, 531);
			this.Controls.Add(this.scTop);
			this.Controls.Add(this.gbModuleList);
			this.Controls.Add(this.mainMenu);
			this.Icon = global::Blk.Gui.Properties.Resources.star2;
			this.MainMenuStrip = this.mainMenu;
			this.MinimumSize = new System.Drawing.Size(700, 500);
			this.Name = "FrmBlackboard";
			this.Text = "Blackboard";
			this.Load += new System.EventHandler(this.FrmBlackboard_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmBlackboard_FormClosed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmBlackboard_FormClosing);
			this.gbModuleList.ResumeLayout(false);
			this.scTop.Panel1.ResumeLayout(false);
			this.scTop.Panel2.ResumeLayout(false);
			this.scTop.ResumeLayout(false);
			this.gbGeneralActions.ResumeLayout(false);
			this.gbBlackboardFiles.ResumeLayout(false);
			this.gbBlackboardFiles.PerformLayout();
			this.gbBlackboardSettings.ResumeLayout(false);
			this.gbBlackboardSettings.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudVerbosity)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudRetrySendAttempts)).EndInit();
			this.tcLog.ResumeLayout(false);
			this.tpDiagnostics.ResumeLayout(false);
			this.gbRemoteMachineStatus.ResumeLayout(false);
			this.tpInjector.ResumeLayout(false);
			this.tpMessagePendingList.ResumeLayout(false);
			this.gbCommandsPending.ResumeLayout(false);
			this.gbCommandsWaiting.ResumeLayout(false);
			this.tpModuleInfo.ResumeLayout(false);
			this.tpModuleInfo.PerformLayout();
			this.tsModuleToolBar.ResumeLayout(false);
			this.tsModuleToolBar.PerformLayout();
			this.gbModuleInfo.ResumeLayout(false);
			this.gbModuleInfo.PerformLayout();
			this.tpOutputLog.ResumeLayout(false);
			this.tpOutputLog.PerformLayout();
			this.tpRedirectionHistory.ResumeLayout(false);
			this.tpSharedVars.ResumeLayout(false);
			this.mainMenu.ResumeLayout(false);
			this.mainMenu.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer scTop;
		private System.Windows.Forms.Button btnSaveOutputLog;
		private System.Windows.Forms.TextBox txtOutputLog;
		private System.Windows.Forms.GroupBox gbModuleList;
		private System.Windows.Forms.Button btnClearOutputLog;
		private System.Windows.Forms.FlowLayoutPanel flpModuleList;
		private System.Windows.Forms.TabControl tcLog;
		private System.Windows.Forms.TabPage tpOutputLog;
		private System.Windows.Forms.TabPage tpMessagePendingList;
		private System.Windows.Forms.Button btnStartStop;
		private System.Windows.Forms.GroupBox gbCommandsWaiting;
		private System.Windows.Forms.ListBox lstCommandsWaiting;
		private System.Windows.Forms.GroupBox gbCommandsPending;
		private System.Windows.Forms.ListBox lstCommandsPending;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.GroupBox gbBlackboardSettings;
		private System.Windows.Forms.Label lblConfigurationFile;
		private System.Windows.Forms.TextBox txtConfigurationFile;
		private System.Windows.Forms.TextBox txtBBInputPort;
		private System.Windows.Forms.Label lblBBInputPort;
		private System.Windows.Forms.GroupBox gbBlackboardFiles;
		private System.Windows.Forms.Button btnExploreConfig;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.OpenFileDialog dlgOpenSettingsFile;
		private System.Windows.Forms.TextBox txtClientsConnected;
		private System.Windows.Forms.Label lblClientsConnected;
		private System.Windows.Forms.SaveFileDialog dlgSaveLog;
		private System.Windows.Forms.Button btnExploreLog;
		private System.Windows.Forms.Label lblLogFile;
		private System.Windows.Forms.TextBox txtLogFile;
		private System.Windows.Forms.TabPage tpModuleInfo;
		private System.Windows.Forms.GroupBox gbModuleInfo;
		private System.Windows.Forms.TextBox txtOutputPort;
		private System.Windows.Forms.TextBox txtRemoteIPAddress;
		private System.Windows.Forms.Label lblOutputPort;
		private System.Windows.Forms.Label lblRemoteIPAddress;
		private System.Windows.Forms.TextBox txtModuleName;
		private System.Windows.Forms.Label lblModuleName;
		private System.Windows.Forms.TextBox txtBBAddresses;
		private System.Windows.Forms.Label lblBBAddresses;
		private System.Windows.Forms.GroupBox gbGeneralActions;
		private System.Windows.Forms.Button btnRestartTest;
		private System.Windows.Forms.TextBox txtTimeLeft;
		private System.Windows.Forms.Label lblTimeLeft;
		private System.Windows.Forms.Button btnRestartBlackboard;
		private System.Windows.Forms.TabPage tpRedirectionHistory;
		private System.Windows.Forms.ListView lvwRedirectionHistory;
		private System.Windows.Forms.ColumnHeader chCommandSent;
		private System.Windows.Forms.ColumnHeader chCommandSource;
		private System.Windows.Forms.ColumnHeader chCommandDestination;
		private System.Windows.Forms.ColumnHeader chCommand;
		private System.Windows.Forms.ColumnHeader chResponse;
		private System.Windows.Forms.ColumnHeader chResponseArrival;
		private System.Windows.Forms.ColumnHeader chInfo;
		private System.Windows.Forms.ColumnHeader chResponseSent;
		private System.Windows.Forms.Label lblRetrySendAttempts;
		private System.Windows.Forms.NumericUpDown nudRetrySendAttempts;
		private System.Windows.Forms.Button btnRestartTimer;
		private System.Windows.Forms.TabPage tpDiagnostics;
		private System.Windows.Forms.GroupBox gbRemoteMachineStatus;
		private System.Windows.Forms.FlowLayoutPanel flpRemoteMachineStatus;
		private System.Windows.Forms.NumericUpDown nudVerbosity;
		private System.Windows.Forms.Label lblVerbosity;
		private System.Windows.Forms.TabPage tpSharedVars;
		private System.Windows.Forms.PropertyGrid pgSelectedSharedVar;
		private System.Windows.Forms.ListView lvSharedVariables;
		private System.Windows.Forms.ComboBox cbModuleStartupMode;
		private System.Windows.Forms.Label lblModuleStartupMode;
		private System.Windows.Forms.Button btnStartSequence;
		private System.Windows.Forms.ContextMenuStrip mnuStartSequence;
		private System.Windows.Forms.CheckBox chkAutostart;
		private System.Windows.Forms.ToolStrip tsModuleToolBar;
		private System.Windows.Forms.ToolStripButton btnModuleStartStopModule;
		private System.Windows.Forms.ToolStripButton btnModuleRestartModule;
		private System.Windows.Forms.ToolStripButton btnModule;
		private System.Windows.Forms.ToolStripDropDownButton btnModuleSimulation;
		private System.Windows.Forms.ToolStripMenuItem btnSimulationEnabled;
		private System.Windows.Forms.ToolStripMenuItem btnSimulationDisabled;
		private System.Windows.Forms.ToolStripDropDownButton tsModuleProcess;
		private System.Windows.Forms.ToolStripMenuItem btnModuleProcessZombieCheck;
		private System.Windows.Forms.ToolStripMenuItem btnModuleProcessRun;
		private System.Windows.Forms.ToolStripMenuItem btnModuleProcessCheckRun;
		private System.Windows.Forms.ToolStripMenuItem btnModuleProcessKill;
		private System.Windows.Forms.ToolStripSeparator tsModuleProcessSeparator1;
		private System.Windows.Forms.ToolStripSeparator tsModuletoolBarSeparator1;
		private System.Windows.Forms.ToolStripButton btnShowModuleActions;
		private System.Windows.Forms.TabPage tpInjector;
		private InjectorTool injectorTool;
		private System.Windows.Forms.Label lblModuleAlias;
		private System.Windows.Forms.TextBox txtModuleAlias;
		private System.Windows.Forms.TextBox txtModuleAuthor;
		private System.Windows.Forms.Label lblModuleAuthor;
		private System.Windows.Forms.ToolTip ttToolTip;
		private InteractionTool interactionTool;
		private System.Windows.Forms.TextBox txtModuleExeArgs;
		private System.Windows.Forms.TextBox txtModuleExePath;
		private System.Windows.Forms.Label lblModuleExePath;
		private System.Windows.Forms.Label lblModuleExeArgs;
		private System.Windows.Forms.TextBox txtModuleProcessName;
		private System.Windows.Forms.Label lblModuleProcessName;
		private System.Windows.Forms.ToolStripButton btnModuleEnable;
		private System.Windows.Forms.ToolStripSeparator tsModuletoolBarSeparator2;
		private System.Windows.Forms.CheckBox chkAutoLog;
		private System.Windows.Forms.MenuStrip mainMenu;
		private System.Windows.Forms.ToolStripMenuItem mnuiBlackboard;
		private System.Windows.Forms.ToolStripMenuItem mnuiBlackboard_StartStop;
		private System.Windows.Forms.ToolStripSeparator mnuiBlackboard_Separator1;
		private System.Windows.Forms.ToolStripMenuItem mnuiBlackboard_Exit;
		private System.Windows.Forms.ToolStripMenuItem mnuiPlugins;
		private System.Windows.Forms.ToolStripSeparator mnuiBlackboard_Separator2;
		private System.Windows.Forms.ToolStripMenuItem mnuiBlackboard_Reload;
		private System.Windows.Forms.ToolStripMenuItem mnuiBlackboard_Restart;
		private System.Windows.Forms.ToolStripMenuItem mnuiBlackboard_Restart_Blackboard;
		private System.Windows.Forms.ToolStripMenuItem mnuiBlackboard_Restart_Test;
		private System.Windows.Forms.ToolStripMenuItem mnuiBlackboard_Restart_Timer;
		private System.Windows.Forms.ToolStripMenuItem mnuiBlackboard_OpenCfgFile;
		private System.Windows.Forms.ToolStripMenuItem mnuiBlackboard_OpenLog;
		private System.Windows.Forms.ToolStripMenuItem mnuiModules;
		private System.Windows.Forms.ToolStripMenuItem mnuiBlackboard_Load;
		private System.Windows.Forms.ToolStripSeparator mnuiBlackboard_Separator3;
		private System.Windows.Forms.ComboBox cbModuleShutdownMode;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSutdownSequence;
		private System.Windows.Forms.ContextMenuStrip mnuShutdownSequence;
	}
}

