namespace ConfigUtil
{
	partial class ModuleView
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModuleView));
			this.tcModule = new System.Windows.Forms.TabControl();
			this.tpCommands = new System.Windows.Forms.TabPage();
			this.dgvModuleCommands = new System.Windows.Forms.DataGridView();
			this.tpStartupActions = new System.Windows.Forms.TabPage();
			this.tpStopActions = new System.Windows.Forms.TabPage();
			this.tpRestartActions = new System.Windows.Forms.TabPage();
			this.tpRestartTestActions = new System.Windows.Forms.TabPage();
			this.tpTestTimeOutActions = new System.Windows.Forms.TabPage();
			this.chkModuleCheckAlive = new System.Windows.Forms.CheckBox();
			this.chkModuleRequirePrefix = new System.Windows.Forms.CheckBox();
			this.chkModuleSimulate = new System.Windows.Forms.CheckBox();
			this.chkModuleEnabled = new System.Windows.Forms.CheckBox();
			this.txtModuleAddress = new System.Windows.Forms.TextBox();
			this.txtModuleName = new System.Windows.Forms.TextBox();
			this.lblModuleName = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.nudModulePort = new System.Windows.Forms.NumericUpDown();
			this.lblModuleAddress = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.colCommandName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colAnswer = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colParameters = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colPriority = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colTimeOut = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.tcModule.SuspendLayout();
			this.tpCommands.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvModuleCommands)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudModulePort)).BeginInit();
			this.SuspendLayout();
			// 
			// tcModule
			// 
			this.tcModule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tcModule.Controls.Add(this.tpCommands);
			this.tcModule.Controls.Add(this.tpStartupActions);
			this.tcModule.Controls.Add(this.tpStopActions);
			this.tcModule.Controls.Add(this.tpRestartActions);
			this.tcModule.Controls.Add(this.tpRestartTestActions);
			this.tcModule.Controls.Add(this.tpTestTimeOutActions);
			this.tcModule.Location = new System.Drawing.Point(3, 157);
			this.tcModule.Name = "tcModule";
			this.tcModule.SelectedIndex = 0;
			this.tcModule.Size = new System.Drawing.Size(364, 120);
			this.tcModule.TabIndex = 19;
			// 
			// tpCommands
			// 
			this.tpCommands.Controls.Add(this.dgvModuleCommands);
			this.tpCommands.Location = new System.Drawing.Point(4, 22);
			this.tpCommands.Name = "tpCommands";
			this.tpCommands.Padding = new System.Windows.Forms.Padding(3);
			this.tpCommands.Size = new System.Drawing.Size(356, 94);
			this.tpCommands.TabIndex = 0;
			this.tpCommands.Text = "Supported Commands";
			this.tpCommands.UseVisualStyleBackColor = true;
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
			this.dgvModuleCommands.Location = new System.Drawing.Point(3, 3);
			this.dgvModuleCommands.MultiSelect = false;
			this.dgvModuleCommands.Name = "dgvModuleCommands";
			this.dgvModuleCommands.Size = new System.Drawing.Size(350, 88);
			this.dgvModuleCommands.TabIndex = 0;
			this.dgvModuleCommands.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgvModuleCommands_UserDeletingRow);
			this.dgvModuleCommands.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvModuleCommands_RowEnter);
			this.dgvModuleCommands.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvModuleCommands_CellValidated);
			this.dgvModuleCommands.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvModuleCommands_CellValidating);
			this.dgvModuleCommands.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvModuleCommands_EditingControlShowing);
			this.dgvModuleCommands.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvModuleCommands_KeyPress);
			// 
			// tpStartupActions
			// 
			this.tpStartupActions.Location = new System.Drawing.Point(4, 22);
			this.tpStartupActions.Name = "tpStartupActions";
			this.tpStartupActions.Padding = new System.Windows.Forms.Padding(3);
			this.tpStartupActions.Size = new System.Drawing.Size(477, 193);
			this.tpStartupActions.TabIndex = 1;
			this.tpStartupActions.Text = "Startup Actions";
			this.tpStartupActions.UseVisualStyleBackColor = true;
			// 
			// tpStopActions
			// 
			this.tpStopActions.Location = new System.Drawing.Point(4, 22);
			this.tpStopActions.Name = "tpStopActions";
			this.tpStopActions.Padding = new System.Windows.Forms.Padding(3);
			this.tpStopActions.Size = new System.Drawing.Size(477, 193);
			this.tpStopActions.TabIndex = 2;
			this.tpStopActions.Text = "Stop Actions";
			this.tpStopActions.UseVisualStyleBackColor = true;
			// 
			// tpRestartActions
			// 
			this.tpRestartActions.Location = new System.Drawing.Point(4, 22);
			this.tpRestartActions.Name = "tpRestartActions";
			this.tpRestartActions.Size = new System.Drawing.Size(470, 174);
			this.tpRestartActions.TabIndex = 3;
			this.tpRestartActions.Text = "Restart Actions";
			this.tpRestartActions.UseVisualStyleBackColor = true;
			// 
			// tpRestartTestActions
			// 
			this.tpRestartTestActions.Location = new System.Drawing.Point(4, 22);
			this.tpRestartTestActions.Name = "tpRestartTestActions";
			this.tpRestartTestActions.Size = new System.Drawing.Size(470, 174);
			this.tpRestartTestActions.TabIndex = 4;
			this.tpRestartTestActions.Text = "Restart Test Actions";
			this.tpRestartTestActions.UseVisualStyleBackColor = true;
			// 
			// tpTestTimeOutActions
			// 
			this.tpTestTimeOutActions.Location = new System.Drawing.Point(4, 22);
			this.tpTestTimeOutActions.Name = "tpTestTimeOutActions";
			this.tpTestTimeOutActions.Size = new System.Drawing.Size(470, 174);
			this.tpTestTimeOutActions.TabIndex = 5;
			this.tpTestTimeOutActions.Text = "Test Timeout Actions";
			this.tpTestTimeOutActions.UseVisualStyleBackColor = true;
			// 
			// chkModuleCheckAlive
			// 
			this.chkModuleCheckAlive.AutoSize = true;
			this.chkModuleCheckAlive.Location = new System.Drawing.Point(3, 77);
			this.chkModuleCheckAlive.Name = "chkModuleCheckAlive";
			this.chkModuleCheckAlive.Size = new System.Drawing.Size(130, 17);
			this.chkModuleCheckAlive.TabIndex = 16;
			this.chkModuleCheckAlive.Text = "Monitor Module status";
			this.toolTip.SetToolTip(this.chkModuleCheckAlive, "When enabled, the Blackboard will monitor the status of\r\nthis module using the Al" +
					"ive, Busy and Ready commands.");
			this.chkModuleCheckAlive.UseVisualStyleBackColor = true;
			this.chkModuleCheckAlive.CheckedChanged += new System.EventHandler(this.chkModuleCheckAlive_CheckedChanged);
			// 
			// chkModuleRequirePrefix
			// 
			this.chkModuleRequirePrefix.AutoSize = true;
			this.chkModuleRequirePrefix.Location = new System.Drawing.Point(194, 77);
			this.chkModuleRequirePrefix.Name = "chkModuleRequirePrefix";
			this.chkModuleRequirePrefix.Size = new System.Drawing.Size(96, 17);
			this.chkModuleRequirePrefix.TabIndex = 18;
			this.chkModuleRequirePrefix.Text = "Requires prefix";
			this.toolTip.SetToolTip(this.chkModuleRequirePrefix, "When enabled, the Blackboard will pre append the source\r\nmodule to all command/re" +
					"sponses sent to this module.");
			this.chkModuleRequirePrefix.UseVisualStyleBackColor = true;
			this.chkModuleRequirePrefix.CheckedChanged += new System.EventHandler(this.chkModuleRequirePrefix_CheckedChanged);
			// 
			// chkModuleSimulate
			// 
			this.chkModuleSimulate.AutoSize = true;
			this.chkModuleSimulate.Location = new System.Drawing.Point(194, 54);
			this.chkModuleSimulate.Name = "chkModuleSimulate";
			this.chkModuleSimulate.Size = new System.Drawing.Size(146, 17);
			this.chkModuleSimulate.TabIndex = 17;
			this.chkModuleSimulate.Text = "Enable Module simulation";
			this.toolTip.SetToolTip(this.chkModuleSimulate, resources.GetString("chkModuleSimulate.ToolTip"));
			this.chkModuleSimulate.UseVisualStyleBackColor = true;
			this.chkModuleSimulate.CheckedChanged += new System.EventHandler(this.chkModuleSimulate_CheckedChanged);
			// 
			// chkModuleEnabled
			// 
			this.chkModuleEnabled.AutoSize = true;
			this.chkModuleEnabled.Location = new System.Drawing.Point(3, 54);
			this.chkModuleEnabled.Name = "chkModuleEnabled";
			this.chkModuleEnabled.Size = new System.Drawing.Size(103, 17);
			this.chkModuleEnabled.TabIndex = 15;
			this.chkModuleEnabled.Text = "Module Enabled";
			this.toolTip.SetToolTip(this.chkModuleEnabled, "Enables or disables the load of this module.\r\nA disabled module will not be loade" +
					"d by the Blackboard,\r\nnor it will be their command prototypes.");
			this.chkModuleEnabled.UseVisualStyleBackColor = true;
			this.chkModuleEnabled.CheckedChanged += new System.EventHandler(this.chkModuleEnabled_CheckedChanged);
			// 
			// txtModuleAddress
			// 
			this.txtModuleAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtModuleAddress.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtModuleAddress.Location = new System.Drawing.Point(85, 28);
			this.txtModuleAddress.Name = "txtModuleAddress";
			this.txtModuleAddress.Size = new System.Drawing.Size(100, 20);
			this.txtModuleAddress.TabIndex = 12;
			this.txtModuleAddress.Text = "127.0.0.1";
			this.txtModuleAddress.Leave += new System.EventHandler(this.txtModuleAddress_Leave);
			this.txtModuleAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtModuleAddress_KeyPress);
			// 
			// txtModuleName
			// 
			this.txtModuleName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtModuleName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtModuleName.Location = new System.Drawing.Point(85, 3);
			this.txtModuleName.Name = "txtModuleName";
			this.txtModuleName.Size = new System.Drawing.Size(282, 20);
			this.txtModuleName.TabIndex = 13;
			this.txtModuleName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtModuleName_KeyDown);
			this.txtModuleName.Leave += new System.EventHandler(this.txtModuleName_Leave);
			this.txtModuleName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtModuleName_KeyPress);
			// 
			// lblModuleName
			// 
			this.lblModuleName.AutoSize = true;
			this.lblModuleName.Location = new System.Drawing.Point(3, 6);
			this.lblModuleName.Name = "lblModuleName";
			this.lblModuleName.Size = new System.Drawing.Size(76, 13);
			this.lblModuleName.TabIndex = 9;
			this.lblModuleName.Text = "Module Name:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(191, 31);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(86, 13);
			this.label2.TabIndex = 10;
			this.label2.Text = "Connection Port:";
			// 
			// nudModulePort
			// 
			this.nudModulePort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.nudModulePort.Location = new System.Drawing.Point(283, 28);
			this.nudModulePort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.nudModulePort.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
			this.nudModulePort.Name = "nudModulePort";
			this.nudModulePort.Size = new System.Drawing.Size(84, 20);
			this.nudModulePort.TabIndex = 14;
			this.nudModulePort.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
			this.nudModulePort.ValueChanged += new System.EventHandler(this.nudModulePort_ValueChanged);
			// 
			// lblModuleAddress
			// 
			this.lblModuleAddress.AutoSize = true;
			this.lblModuleAddress.Location = new System.Drawing.Point(3, 31);
			this.lblModuleAddress.Name = "lblModuleAddress";
			this.lblModuleAddress.Size = new System.Drawing.Size(61, 13);
			this.lblModuleAddress.TabIndex = 11;
			this.lblModuleAddress.Text = "IP Address:";
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
			this.colAnswer.Width = 50;
			// 
			// colParameters
			// 
			this.colParameters.HeaderText = "Parameters";
			this.colParameters.Name = "colParameters";
			this.colParameters.Width = 60;
			// 
			// colPriority
			// 
			this.colPriority.HeaderText = "Priority";
			this.colPriority.Name = "colPriority";
			this.colPriority.Width = 50;
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
			// ModuleView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tcModule);
			this.Controls.Add(this.chkModuleCheckAlive);
			this.Controls.Add(this.chkModuleRequirePrefix);
			this.Controls.Add(this.chkModuleSimulate);
			this.Controls.Add(this.chkModuleEnabled);
			this.Controls.Add(this.txtModuleAddress);
			this.Controls.Add(this.txtModuleName);
			this.Controls.Add(this.lblModuleName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.nudModulePort);
			this.Controls.Add(this.lblModuleAddress);
			this.MinimumSize = new System.Drawing.Size(370, 280);
			this.Name = "ModuleView";
			this.Size = new System.Drawing.Size(370, 280);
			this.tcModule.ResumeLayout(false);
			this.tpCommands.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvModuleCommands)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudModulePort)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tcModule;
		private System.Windows.Forms.TabPage tpCommands;
		private System.Windows.Forms.DataGridView dgvModuleCommands;
		private System.Windows.Forms.TabPage tpStartupActions;
		private System.Windows.Forms.TabPage tpStopActions;
		private System.Windows.Forms.TabPage tpRestartActions;
		private System.Windows.Forms.TabPage tpRestartTestActions;
		private System.Windows.Forms.TabPage tpTestTimeOutActions;
		private System.Windows.Forms.CheckBox chkModuleCheckAlive;
		private System.Windows.Forms.CheckBox chkModuleRequirePrefix;
		private System.Windows.Forms.CheckBox chkModuleSimulate;
		private System.Windows.Forms.CheckBox chkModuleEnabled;
		private System.Windows.Forms.TextBox txtModuleAddress;
		private System.Windows.Forms.TextBox txtModuleName;
		private System.Windows.Forms.Label lblModuleName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown nudModulePort;
		private System.Windows.Forms.Label lblModuleAddress;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.DataGridViewTextBoxColumn colCommandName;
		private System.Windows.Forms.DataGridViewCheckBoxColumn colAnswer;
		private System.Windows.Forms.DataGridViewCheckBoxColumn colParameters;
		private System.Windows.Forms.DataGridViewCheckBoxColumn colPriority;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTimeOut;
	}
}
