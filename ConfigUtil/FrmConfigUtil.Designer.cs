namespace ConfigUtil
{
	partial class FrmConfigUtil
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConfigUtil));
			this.tsMainToolBar = new System.Windows.Forms.ToolStrip();
			this.statusBar = new System.Windows.Forms.StatusStrip();
			this.lstBBModules = new System.Windows.Forms.ListBox();
			this.sfdSave = new System.Windows.Forms.SaveFileDialog();
			this.ofdOpen = new System.Windows.Forms.OpenFileDialog();
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tpLeftTabs = new System.Windows.Forms.TabControl();
			this.tpBBConfig = new System.Windows.Forms.TabPage();
			this.pgBlackboardProperties = new System.Windows.Forms.PropertyGrid();
			this.tpModules = new System.Windows.Forms.TabPage();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.pgModuleProperties = new System.Windows.Forms.PropertyGrid();
			this.tpSharedVariables = new System.Windows.Forms.TabPage();
			this.lstBBVars = new System.Windows.Forms.ListBox();
			this.gbBlackboard = new System.Windows.Forms.GroupBox();
			this.gbSharedVariables = new System.Windows.Forms.GroupBox();
			this.pgVariableProperties = new System.Windows.Forms.PropertyGrid();
			this.dgvSharedVariables = new System.Windows.Forms.DataGridView();
			this.colModuleWriter = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.gbModule = new System.Windows.Forms.GroupBox();
			this.dgvModuleCommands = new System.Windows.Forms.DataGridView();
			this.colCommandName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colAnswer = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colParameters = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colPriority = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colTimeOut = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiSaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
			this.editMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiSelectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiToolBar = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiStatusBar = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsMenu = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.prototypeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.moveCommandPrototypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiNew = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiUndo = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiRedo = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiCut = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.tsbNewModule = new System.Windows.Forms.ToolStripButton();
			this.tsbDeleteModule = new System.Windows.Forms.ToolStripButton();
			this.tsbNewSharedVariable = new System.Windows.Forms.ToolStripButton();
			this.tsbDeleteSharedVariable = new System.Windows.Forms.ToolStripButton();
			this.deleteCommandPrototypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tsbEnableModule = new System.Windows.Forms.ToolStripButton();
			this.tsbDisableModule = new System.Windows.Forms.ToolStripButton();
			this.tssModuleSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.tssSharedVariableSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.tsMainToolBar.SuspendLayout();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tpLeftTabs.SuspendLayout();
			this.tpBBConfig.SuspendLayout();
			this.tpModules.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.tpSharedVariables.SuspendLayout();
			this.gbSharedVariables.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvSharedVariables)).BeginInit();
			this.gbModule.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvModuleCommands)).BeginInit();
			this.menuStrip.SuspendLayout();
			this.prototypeContextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// tsMainToolBar
			// 
			this.tsMainToolBar.Dock = System.Windows.Forms.DockStyle.None;
			this.tsMainToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.tssModuleSeparator,
            this.tsbNewModule,
            this.tsbDeleteModule,
            this.tsbEnableModule,
            this.tsbDisableModule,
            this.tssSharedVariableSeparator,
            this.tsbNewSharedVariable,
            this.tsbDeleteSharedVariable,
            this.toolStripButton1});
			this.tsMainToolBar.Location = new System.Drawing.Point(3, 24);
			this.tsMainToolBar.Name = "tsMainToolBar";
			this.tsMainToolBar.Size = new System.Drawing.Size(254, 25);
			this.tsMainToolBar.TabIndex = 2;
			this.tsMainToolBar.Text = "ToolStrip";
			// 
			// statusBar
			// 
			this.statusBar.Location = new System.Drawing.Point(0, 424);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(722, 22);
			this.statusBar.TabIndex = 3;
			this.statusBar.Text = "statusStrip1";
			// 
			// lstBBModules
			// 
			this.lstBBModules.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstBBModules.FormattingEnabled = true;
			this.lstBBModules.Location = new System.Drawing.Point(0, 0);
			this.lstBBModules.Name = "lstBBModules";
			this.lstBBModules.Size = new System.Drawing.Size(201, 160);
			this.lstBBModules.TabIndex = 5;
			this.lstBBModules.SelectedIndexChanged += new System.EventHandler(this.lstBBModules_SelectedIndexChanged);
			// 
			// sfdSave
			// 
			this.sfdSave.DefaultExt = "xml";
			this.sfdSave.FileName = "Blackboard";
			this.sfdSave.Filter = "XML Files|*.xml";
			this.sfdSave.Title = "Save Blackboard Configuration File";
			// 
			// ofdOpen
			// 
			this.ofdOpen.DefaultExt = "xml";
			this.ofdOpen.FileName = "Blackboard";
			this.ofdOpen.Filter = "XML Files|*.xml";
			this.ofdOpen.Title = "Open Blackboard Configuration File";
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(722, 375);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(722, 424);
			this.toolStripContainer1.TabIndex = 6;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsMainToolBar);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.tpLeftTabs);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.gbBlackboard);
			this.splitContainer1.Panel2.Controls.Add(this.gbSharedVariables);
			this.splitContainer1.Panel2.Controls.Add(this.gbModule);
			this.splitContainer1.Size = new System.Drawing.Size(722, 375);
			this.splitContainer1.SplitterDistance = 215;
			this.splitContainer1.TabIndex = 0;
			// 
			// tpLeftTabs
			// 
			this.tpLeftTabs.Controls.Add(this.tpBBConfig);
			this.tpLeftTabs.Controls.Add(this.tpModules);
			this.tpLeftTabs.Controls.Add(this.tpSharedVariables);
			this.tpLeftTabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tpLeftTabs.Location = new System.Drawing.Point(0, 0);
			this.tpLeftTabs.Multiline = true;
			this.tpLeftTabs.Name = "tpLeftTabs";
			this.tpLeftTabs.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.tpLeftTabs.SelectedIndex = 0;
			this.tpLeftTabs.Size = new System.Drawing.Size(215, 375);
			this.tpLeftTabs.TabIndex = 1;
			this.tpLeftTabs.SelectedIndexChanged += new System.EventHandler(this.tpLeftTabs_SelectedIndexChanged);
			// 
			// tpBBConfig
			// 
			this.tpBBConfig.Controls.Add(this.pgBlackboardProperties);
			this.tpBBConfig.Location = new System.Drawing.Point(4, 22);
			this.tpBBConfig.Name = "tpBBConfig";
			this.tpBBConfig.Padding = new System.Windows.Forms.Padding(3);
			this.tpBBConfig.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.tpBBConfig.Size = new System.Drawing.Size(207, 349);
			this.tpBBConfig.TabIndex = 0;
			this.tpBBConfig.Text = "Settings";
			this.tpBBConfig.UseVisualStyleBackColor = true;
			this.tpBBConfig.Click += new System.EventHandler(this.tpBBConfig_Click);
			// 
			// pgBlackboardProperties
			// 
			this.pgBlackboardProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgBlackboardProperties.Location = new System.Drawing.Point(3, 3);
			this.pgBlackboardProperties.Name = "pgBlackboardProperties";
			this.pgBlackboardProperties.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.pgBlackboardProperties.Size = new System.Drawing.Size(201, 343);
			this.pgBlackboardProperties.TabIndex = 6;
			// 
			// tpModules
			// 
			this.tpModules.Controls.Add(this.splitContainer2);
			this.tpModules.Location = new System.Drawing.Point(4, 22);
			this.tpModules.Name = "tpModules";
			this.tpModules.Padding = new System.Windows.Forms.Padding(3);
			this.tpModules.Size = new System.Drawing.Size(207, 349);
			this.tpModules.TabIndex = 1;
			this.tpModules.Text = "Modules";
			this.tpModules.UseVisualStyleBackColor = true;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(3, 3);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.lstBBModules);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.pgModuleProperties);
			this.splitContainer2.Size = new System.Drawing.Size(201, 343);
			this.splitContainer2.SplitterDistance = 161;
			this.splitContainer2.TabIndex = 0;
			// 
			// pgModuleProperties
			// 
			this.pgModuleProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgModuleProperties.Location = new System.Drawing.Point(0, 0);
			this.pgModuleProperties.Name = "pgModuleProperties";
			this.pgModuleProperties.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.pgModuleProperties.Size = new System.Drawing.Size(201, 178);
			this.pgModuleProperties.TabIndex = 0;
			// 
			// tpSharedVariables
			// 
			this.tpSharedVariables.Controls.Add(this.lstBBVars);
			this.tpSharedVariables.Location = new System.Drawing.Point(4, 22);
			this.tpSharedVariables.Name = "tpSharedVariables";
			this.tpSharedVariables.Size = new System.Drawing.Size(207, 349);
			this.tpSharedVariables.TabIndex = 2;
			this.tpSharedVariables.Text = "Variables";
			this.tpSharedVariables.UseVisualStyleBackColor = true;
			// 
			// lstBBVars
			// 
			this.lstBBVars.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstBBVars.FormattingEnabled = true;
			this.lstBBVars.Location = new System.Drawing.Point(0, 0);
			this.lstBBVars.Name = "lstBBVars";
			this.lstBBVars.Size = new System.Drawing.Size(207, 342);
			this.lstBBVars.TabIndex = 6;
			this.lstBBVars.SelectedIndexChanged += new System.EventHandler(this.lstBBVars_SelectedIndexChanged);
			// 
			// gbBlackboard
			// 
			this.gbBlackboard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbBlackboard.Location = new System.Drawing.Point(6, 3);
			this.gbBlackboard.Name = "gbBlackboard";
			this.gbBlackboard.Size = new System.Drawing.Size(485, 207);
			this.gbBlackboard.TabIndex = 12;
			this.gbBlackboard.TabStop = false;
			this.gbBlackboard.Text = "Blackboard Configuration";
			// 
			// gbSharedVariables
			// 
			this.gbSharedVariables.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.gbSharedVariables.Controls.Add(this.pgVariableProperties);
			this.gbSharedVariables.Controls.Add(this.dgvSharedVariables);
			this.gbSharedVariables.Location = new System.Drawing.Point(220, 191);
			this.gbSharedVariables.Name = "gbSharedVariables";
			this.gbSharedVariables.Size = new System.Drawing.Size(271, 181);
			this.gbSharedVariables.TabIndex = 11;
			this.gbSharedVariables.TabStop = false;
			this.gbSharedVariables.Text = "Shared Variable Configuration";
			// 
			// pgVariableProperties
			// 
			this.pgVariableProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pgVariableProperties.Location = new System.Drawing.Point(3, 19);
			this.pgVariableProperties.Name = "pgVariableProperties";
			this.pgVariableProperties.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.pgVariableProperties.Size = new System.Drawing.Size(106, 156);
			this.pgVariableProperties.TabIndex = 11;
			// 
			// dgvSharedVariables
			// 
			this.dgvSharedVariables.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dgvSharedVariables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSharedVariables.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colModuleWriter});
			this.dgvSharedVariables.Location = new System.Drawing.Point(115, 19);
			this.dgvSharedVariables.Name = "dgvSharedVariables";
			this.dgvSharedVariables.Size = new System.Drawing.Size(150, 156);
			this.dgvSharedVariables.TabIndex = 10;
			this.dgvSharedVariables.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgvSharedVariables_UserDeletingRow);
			this.dgvSharedVariables.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSharedVariables_RowEnter);
			this.dgvSharedVariables.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSharedVariables_CellValidated);
			this.dgvSharedVariables.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvSharedVariables_CellValidating);
			this.dgvSharedVariables.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvSharedVariables_EditingControlShowing);
			// 
			// colModuleWriter
			// 
			this.colModuleWriter.HeaderText = "Writer Name";
			this.colModuleWriter.Name = "colModuleWriter";
			// 
			// gbModule
			// 
			this.gbModule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbModule.Controls.Add(this.dgvModuleCommands);
			this.gbModule.Location = new System.Drawing.Point(3, 191);
			this.gbModule.Name = "gbModule";
			this.gbModule.Size = new System.Drawing.Size(214, 156);
			this.gbModule.TabIndex = 11;
			this.gbModule.TabStop = false;
			this.gbModule.Text = "Module Configuration";
			// 
			// dgvModuleCommands
			// 
			this.dgvModuleCommands.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvModuleCommands.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCommandName,
            this.colAnswer,
            this.colParameters,
            this.colPriority,
            this.colTimeOut});
			this.dgvModuleCommands.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvModuleCommands.Location = new System.Drawing.Point(3, 16);
			this.dgvModuleCommands.MultiSelect = false;
			this.dgvModuleCommands.Name = "dgvModuleCommands";
			this.dgvModuleCommands.Size = new System.Drawing.Size(208, 137);
			this.dgvModuleCommands.TabIndex = 1;
			this.dgvModuleCommands.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgvModuleCommands_UserDeletingRow);
			this.dgvModuleCommands.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvModuleCommands_RowEnter);
			this.dgvModuleCommands.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvModuleCommands_CellValidating);
			this.dgvModuleCommands.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvModuleCommands_RowValidated);
			this.dgvModuleCommands.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvModuleCommands_EditingControlShowing);
			this.dgvModuleCommands.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvModuleCommands_KeyPress);
			// 
			// colCommandName
			// 
			this.colCommandName.Frozen = true;
			this.colCommandName.HeaderText = "Command Name";
			this.colCommandName.MaxInputLength = 128;
			this.colCommandName.MinimumWidth = 50;
			this.colCommandName.Name = "colCommandName";
			this.colCommandName.Width = 120;
			// 
			// colAnswer
			// 
			this.colAnswer.HeaderText = "Answer";
			this.colAnswer.Name = "colAnswer";
			this.colAnswer.Width = 75;
			// 
			// colParameters
			// 
			this.colParameters.HeaderText = "Parameters";
			this.colParameters.Name = "colParameters";
			this.colParameters.Width = 75;
			// 
			// colPriority
			// 
			this.colPriority.HeaderText = "Has Priority";
			this.colPriority.Name = "colPriority";
			this.colPriority.Width = 75;
			// 
			// colTimeOut
			// 
			this.colTimeOut.HeaderText = "Timeout";
			this.colTimeOut.MaxInputLength = 7;
			this.colTimeOut.Name = "colTimeOut";
			this.colTimeOut.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colTimeOut.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.colTimeOut.Width = 75;
			// 
			// menuStrip
			// 
			this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.editMenu,
            this.viewMenu,
            this.toolsMenu});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(722, 24);
			this.menuStrip.TabIndex = 2;
			this.menuStrip.Text = "MenuStrip";
			// 
			// fileMenu
			// 
			this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNew,
            this.tsmiOpen,
            this.toolStripSeparator3,
            this.tsmiSave,
            this.tsmiSaveAs,
            this.toolStripSeparator4,
            this.tsmiExit});
			this.fileMenu.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
			this.fileMenu.Name = "fileMenu";
			this.fileMenu.Size = new System.Drawing.Size(37, 20);
			this.fileMenu.Text = "&File";
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(143, 6);
			// 
			// tsmiSaveAs
			// 
			this.tsmiSaveAs.Name = "tsmiSaveAs";
			this.tsmiSaveAs.Size = new System.Drawing.Size(146, 22);
			this.tsmiSaveAs.Text = "Save &As";
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(143, 6);
			// 
			// tsmiExit
			// 
			this.tsmiExit.Name = "tsmiExit";
			this.tsmiExit.Size = new System.Drawing.Size(146, 22);
			this.tsmiExit.Text = "E&xit";
			// 
			// editMenu
			// 
			this.editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiUndo,
            this.tsmiRedo,
            this.toolStripSeparator6,
            this.tsmiCut,
            this.tsmiCopy,
            this.tsmiPaste,
            this.toolStripSeparator7,
            this.tsmiSelectAll});
			this.editMenu.Name = "editMenu";
			this.editMenu.Size = new System.Drawing.Size(39, 20);
			this.editMenu.Text = "&Edit";
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(161, 6);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(161, 6);
			// 
			// tsmiSelectAll
			// 
			this.tsmiSelectAll.Name = "tsmiSelectAll";
			this.tsmiSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.tsmiSelectAll.Size = new System.Drawing.Size(164, 22);
			this.tsmiSelectAll.Text = "Select &All";
			// 
			// viewMenu
			// 
			this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiToolBar,
            this.tsmiStatusBar});
			this.viewMenu.Name = "viewMenu";
			this.viewMenu.Size = new System.Drawing.Size(44, 20);
			this.viewMenu.Text = "&View";
			// 
			// tsmiToolBar
			// 
			this.tsmiToolBar.Checked = true;
			this.tsmiToolBar.CheckOnClick = true;
			this.tsmiToolBar.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tsmiToolBar.Name = "tsmiToolBar";
			this.tsmiToolBar.Size = new System.Drawing.Size(126, 22);
			this.tsmiToolBar.Text = "&Toolbar";
			// 
			// tsmiStatusBar
			// 
			this.tsmiStatusBar.Checked = true;
			this.tsmiStatusBar.CheckOnClick = true;
			this.tsmiStatusBar.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tsmiStatusBar.Name = "tsmiStatusBar";
			this.tsmiStatusBar.Size = new System.Drawing.Size(126, 22);
			this.tsmiStatusBar.Text = "&Status Bar";
			// 
			// toolsMenu
			// 
			this.toolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOptions});
			this.toolsMenu.Name = "toolsMenu";
			this.toolsMenu.Size = new System.Drawing.Size(48, 20);
			this.toolsMenu.Text = "&Tools";
			// 
			// tsmiOptions
			// 
			this.tsmiOptions.Enabled = false;
			this.tsmiOptions.Name = "tsmiOptions";
			this.tsmiOptions.Size = new System.Drawing.Size(116, 22);
			this.tsmiOptions.Text = "&Options";
			// 
			// prototypeContextMenu
			// 
			this.prototypeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteCommandPrototypeToolStripMenuItem,
            this.moveCommandPrototypeToolStripMenuItem});
			this.prototypeContextMenu.Name = "prototypeContextMenu";
			this.prototypeContextMenu.Size = new System.Drawing.Size(221, 48);
			// 
			// moveCommandPrototypeToolStripMenuItem
			// 
			this.moveCommandPrototypeToolStripMenuItem.Name = "moveCommandPrototypeToolStripMenuItem";
			this.moveCommandPrototypeToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.moveCommandPrototypeToolStripMenuItem.Text = "Move command prototype";
			// 
			// tsmiNew
			// 
			this.tsmiNew.Image = global::ConfigUtil.Properties.Resources.DocumentHS;
			this.tsmiNew.ImageTransparentColor = System.Drawing.Color.Black;
			this.tsmiNew.Name = "tsmiNew";
			this.tsmiNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.tsmiNew.Size = new System.Drawing.Size(146, 22);
			this.tsmiNew.Text = "&New";
			// 
			// tsmiOpen
			// 
			this.tsmiOpen.Image = global::ConfigUtil.Properties.Resources.openfolderHS;
			this.tsmiOpen.ImageTransparentColor = System.Drawing.Color.Black;
			this.tsmiOpen.Name = "tsmiOpen";
			this.tsmiOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.tsmiOpen.Size = new System.Drawing.Size(146, 22);
			this.tsmiOpen.Text = "&Open";
			// 
			// tsmiSave
			// 
			this.tsmiSave.Image = global::ConfigUtil.Properties.Resources.saveHS;
			this.tsmiSave.ImageTransparentColor = System.Drawing.Color.Black;
			this.tsmiSave.Name = "tsmiSave";
			this.tsmiSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.tsmiSave.Size = new System.Drawing.Size(146, 22);
			this.tsmiSave.Text = "&Save";
			// 
			// tsmiUndo
			// 
			this.tsmiUndo.Enabled = false;
			this.tsmiUndo.Image = global::ConfigUtil.Properties.Resources.Edit_UndoHS;
			this.tsmiUndo.ImageTransparentColor = System.Drawing.Color.Black;
			this.tsmiUndo.Name = "tsmiUndo";
			this.tsmiUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.tsmiUndo.Size = new System.Drawing.Size(164, 22);
			this.tsmiUndo.Text = "&Undo";
			// 
			// tsmiRedo
			// 
			this.tsmiRedo.Enabled = false;
			this.tsmiRedo.Image = global::ConfigUtil.Properties.Resources.Edit_RedoHS;
			this.tsmiRedo.ImageTransparentColor = System.Drawing.Color.Black;
			this.tsmiRedo.Name = "tsmiRedo";
			this.tsmiRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
			this.tsmiRedo.Size = new System.Drawing.Size(164, 22);
			this.tsmiRedo.Text = "&Redo";
			// 
			// tsmiCut
			// 
			this.tsmiCut.Image = global::ConfigUtil.Properties.Resources.CutHS;
			this.tsmiCut.ImageTransparentColor = System.Drawing.Color.Black;
			this.tsmiCut.Name = "tsmiCut";
			this.tsmiCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.tsmiCut.Size = new System.Drawing.Size(164, 22);
			this.tsmiCut.Text = "Cu&t";
			// 
			// tsmiCopy
			// 
			this.tsmiCopy.Image = global::ConfigUtil.Properties.Resources.CopyHS;
			this.tsmiCopy.ImageTransparentColor = System.Drawing.Color.Black;
			this.tsmiCopy.Name = "tsmiCopy";
			this.tsmiCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.tsmiCopy.Size = new System.Drawing.Size(164, 22);
			this.tsmiCopy.Text = "&Copy";
			// 
			// tsmiPaste
			// 
			this.tsmiPaste.Image = global::ConfigUtil.Properties.Resources.PasteHS;
			this.tsmiPaste.ImageTransparentColor = System.Drawing.Color.Black;
			this.tsmiPaste.Name = "tsmiPaste";
			this.tsmiPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.tsmiPaste.Size = new System.Drawing.Size(164, 22);
			this.tsmiPaste.Text = "&Paste";
			// 
			// newToolStripButton
			// 
			this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.newToolStripButton.Image = global::ConfigUtil.Properties.Resources.DocumentHS;
			this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
			this.newToolStripButton.Name = "newToolStripButton";
			this.newToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.newToolStripButton.Text = "New";
			this.newToolStripButton.Click += new System.EventHandler(this.newToolStripButton_Click);
			// 
			// openToolStripButton
			// 
			this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.openToolStripButton.Image = global::ConfigUtil.Properties.Resources.openfolderHS;
			this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
			this.openToolStripButton.Name = "openToolStripButton";
			this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.openToolStripButton.Text = "Open";
			this.openToolStripButton.Click += new System.EventHandler(this.openToolStripButton_Click);
			// 
			// saveToolStripButton
			// 
			this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.saveToolStripButton.Image = global::ConfigUtil.Properties.Resources.saveHS;
			this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
			this.saveToolStripButton.Name = "saveToolStripButton";
			this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.saveToolStripButton.Text = "Save";
			this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
			// 
			// tsbNewModule
			// 
			this.tsbNewModule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbNewModule.Image = global::ConfigUtil.Properties.Resources.AddModule_16;
			this.tsbNewModule.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbNewModule.Name = "tsbNewModule";
			this.tsbNewModule.Size = new System.Drawing.Size(23, 22);
			this.tsbNewModule.Text = "New Moule";
			// 
			// tsbDeleteModule
			// 
			this.tsbDeleteModule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbDeleteModule.Image = global::ConfigUtil.Properties.Resources.DeleteModule_16;
			this.tsbDeleteModule.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbDeleteModule.Name = "tsbDeleteModule";
			this.tsbDeleteModule.Size = new System.Drawing.Size(23, 22);
			this.tsbDeleteModule.Text = "Delete Module";
			// 
			// tsbNewSharedVariable
			// 
			this.tsbNewSharedVariable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbNewSharedVariable.Image = ((System.Drawing.Image)(resources.GetObject("tsbNewSharedVariable.Image")));
			this.tsbNewSharedVariable.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbNewSharedVariable.Name = "tsbNewSharedVariable";
			this.tsbNewSharedVariable.Size = new System.Drawing.Size(23, 22);
			this.tsbNewSharedVariable.Text = "New Shared Variable";
			// 
			// tsbDeleteSharedVariable
			// 
			this.tsbDeleteSharedVariable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbDeleteSharedVariable.Image = ((System.Drawing.Image)(resources.GetObject("tsbDeleteSharedVariable.Image")));
			this.tsbDeleteSharedVariable.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbDeleteSharedVariable.Name = "tsbDeleteSharedVariable";
			this.tsbDeleteSharedVariable.Size = new System.Drawing.Size(23, 22);
			this.tsbDeleteSharedVariable.Text = "Delete Shared Variable";
			// 
			// deleteCommandPrototypeToolStripMenuItem
			// 
			this.deleteCommandPrototypeToolStripMenuItem.Image = global::ConfigUtil.Properties.Resources.DeleteHS;
			this.deleteCommandPrototypeToolStripMenuItem.Name = "deleteCommandPrototypeToolStripMenuItem";
			this.deleteCommandPrototypeToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
			this.deleteCommandPrototypeToolStripMenuItem.Text = "Delete command prototype";
			// 
			// tsbEnableModule
			// 
			this.tsbEnableModule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbEnableModule.Image = global::ConfigUtil.Properties.Resources.EnableModule_16;
			this.tsbEnableModule.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbEnableModule.Name = "tsbEnableModule";
			this.tsbEnableModule.Size = new System.Drawing.Size(23, 22);
			this.tsbEnableModule.Text = "Enable Module";
			// 
			// tsbDisableModule
			// 
			this.tsbDisableModule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbDisableModule.Image = global::ConfigUtil.Properties.Resources.DisableModule_16;
			this.tsbDisableModule.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbDisableModule.Name = "tsbDisableModule";
			this.tsbDisableModule.Size = new System.Drawing.Size(23, 22);
			this.tsbDisableModule.Text = "Disable Module";
			// 
			// tssModuleSeparator
			// 
			this.tssModuleSeparator.Name = "tssModuleSeparator";
			this.tssModuleSeparator.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton1
			// 
			this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton1.Text = "toolStripButton1";
			// 
			// tssSharedVariableSeparator
			// 
			this.tssSharedVariableSeparator.Name = "tssSharedVariableSeparator";
			this.tssSharedVariableSeparator.Size = new System.Drawing.Size(6, 25);
			// 
			// FrmConfigUtil
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(722, 446);
			this.Controls.Add(this.toolStripContainer1);
			this.Controls.Add(this.statusBar);
			this.Name = "FrmConfigUtil";
			this.Text = "Blackboard Configuration Utility";
			this.Load += new System.EventHandler(this.FrmConfigUtil_Load);
			this.tsMainToolBar.ResumeLayout(false);
			this.tsMainToolBar.PerformLayout();
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tpLeftTabs.ResumeLayout(false);
			this.tpBBConfig.ResumeLayout(false);
			this.tpModules.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.tpSharedVariables.ResumeLayout(false);
			this.gbSharedVariables.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvSharedVariables)).EndInit();
			this.gbModule.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvModuleCommands)).EndInit();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.prototypeContextMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStripButton newToolStripButton;
		private System.Windows.Forms.ToolStripButton openToolStripButton;
		private System.Windows.Forms.ToolStripButton saveToolStripButton;
		private System.Windows.Forms.ToolStrip tsMainToolBar;
		private System.Windows.Forms.StatusStrip statusBar;
		private System.Windows.Forms.ListBox lstBBModules;
		private System.Windows.Forms.SaveFileDialog sfdSave;
		private System.Windows.Forms.OpenFileDialog ofdOpen;
		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileMenu;
		private System.Windows.Forms.ToolStripMenuItem tsmiNew;
		private System.Windows.Forms.ToolStripMenuItem tsmiOpen;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem tsmiSave;
		private System.Windows.Forms.ToolStripMenuItem tsmiSaveAs;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem tsmiExit;
		private System.Windows.Forms.ToolStripMenuItem editMenu;
		private System.Windows.Forms.ToolStripMenuItem tsmiUndo;
		private System.Windows.Forms.ToolStripMenuItem tsmiRedo;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem tsmiCut;
		private System.Windows.Forms.ToolStripMenuItem tsmiCopy;
		private System.Windows.Forms.ToolStripMenuItem tsmiPaste;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.ToolStripMenuItem tsmiSelectAll;
		private System.Windows.Forms.ToolStripMenuItem viewMenu;
		private System.Windows.Forms.ToolStripMenuItem tsmiToolBar;
		private System.Windows.Forms.ToolStripMenuItem tsmiStatusBar;
		private System.Windows.Forms.ToolStripMenuItem toolsMenu;
		private System.Windows.Forms.ToolStripMenuItem tsmiOptions;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TabControl tpLeftTabs;
		private System.Windows.Forms.TabPage tpBBConfig;
		private System.Windows.Forms.TabPage tpModules;
		private System.Windows.Forms.TabPage tpSharedVariables;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.PropertyGrid pgModuleProperties;
		private System.Windows.Forms.PropertyGrid pgBlackboardProperties;
		private System.Windows.Forms.ListBox lstBBVars;
		private System.Windows.Forms.DataGridView dgvSharedVariables;
		private System.Windows.Forms.GroupBox gbSharedVariables;
		private System.Windows.Forms.GroupBox gbModule;
		private System.Windows.Forms.GroupBox gbBlackboard;
		private System.Windows.Forms.DataGridView dgvModuleCommands;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCommandName;
		private System.Windows.Forms.DataGridViewCheckBoxColumn colAnswer;
		private System.Windows.Forms.DataGridViewCheckBoxColumn colParameters;
		private System.Windows.Forms.DataGridViewCheckBoxColumn colPriority;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTimeOut;
		private System.Windows.Forms.ToolStripButton tsbNewModule;
		private System.Windows.Forms.ToolStripButton tsbDeleteModule;
		private System.Windows.Forms.ToolStripButton tsbDeleteSharedVariable;
		private System.Windows.Forms.PropertyGrid pgVariableProperties;
		private System.Windows.Forms.DataGridViewTextBoxColumn colModuleWriter;
		private System.Windows.Forms.ContextMenuStrip prototypeContextMenu;
		private System.Windows.Forms.ToolStripMenuItem deleteCommandPrototypeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem moveCommandPrototypeToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton tsbNewSharedVariable;
		private System.Windows.Forms.ToolStripSeparator tssModuleSeparator;
		private System.Windows.Forms.ToolStripButton tsbEnableModule;
		private System.Windows.Forms.ToolStripButton tsbDisableModule;
		private System.Windows.Forms.ToolStripSeparator tssSharedVariableSeparator;
		private System.Windows.Forms.ToolStripButton toolStripButton1;

	}
}

