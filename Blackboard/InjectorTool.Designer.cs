namespace Blk.Gui
{
	partial class InjectorTool
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
			this.lblSourceModule = new System.Windows.Forms.Label();
			this.cbModules = new System.Windows.Forms.ComboBox();
			this.lblMessage = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.btnInject = new System.Windows.Forms.Button();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miUndo = new System.Windows.Forms.ToolStripMenuItem();
			this.miRedo = new System.Windows.Forms.ToolStripMenuItem();
			this.miSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.miCut = new System.Windows.Forms.ToolStripMenuItem();
			this.miCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.miPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.miDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.miSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.miClear = new System.Windows.Forms.ToolStripMenuItem();
			this.miSelectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.miSendToMessage = new System.Windows.Forms.ToolStripMenuItem();
			this.gbHistory = new System.Windows.Forms.GroupBox();
			this.cmbHystoryFilter = new System.Windows.Forms.ComboBox();
			this.lblHystoryFilter = new System.Windows.Forms.Label();
			this.lstHistory = new System.Windows.Forms.ListBox();
			this.gbInjector = new System.Windows.Forms.GroupBox();
			this.chkBulk = new System.Windows.Forms.CheckBox();
			this.lblQuickCommand = new System.Windows.Forms.Label();
			this.cmbQuickCommand = new System.Windows.Forms.ComboBox();
			this.btnQuickCommand = new System.Windows.Forms.Button();
			this.btnHystoryFilter = new System.Windows.Forms.Button();
			this.contextMenu.SuspendLayout();
			this.gbHistory.SuspendLayout();
			this.gbInjector.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblSourceModule
			// 
			this.lblSourceModule.AutoSize = true;
			this.lblSourceModule.Location = new System.Drawing.Point(6, 22);
			this.lblSourceModule.Name = "lblSourceModule";
			this.lblSourceModule.Size = new System.Drawing.Size(44, 13);
			this.lblSourceModule.TabIndex = 0;
			this.lblSourceModule.Text = "Source:";
			// 
			// cbModules
			// 
			this.cbModules.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cbModules.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbModules.FormattingEnabled = true;
			this.cbModules.Location = new System.Drawing.Point(65, 19);
			this.cbModules.Name = "cbModules";
			this.cbModules.Size = new System.Drawing.Size(214, 21);
			this.cbModules.TabIndex = 1;
			// 
			// lblMessage
			// 
			this.lblMessage.AutoSize = true;
			this.lblMessage.Location = new System.Drawing.Point(6, 76);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(53, 13);
			this.lblMessage.TabIndex = 2;
			this.lblMessage.Text = "Message:";
			// 
			// btnInject
			// 
			this.btnInject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnInject.Location = new System.Drawing.Point(158, 145);
			this.btnInject.Name = "btnInject";
			this.btnInject.Size = new System.Drawing.Size(121, 23);
			this.btnInject.TabIndex = 3;
			this.btnInject.Text = "Inject to blackboard";
			this.btnInject.UseVisualStyleBackColor = true;
			this.btnInject.Click += new System.EventHandler(this.btnInject_Click);
			// 
			// txtMessage
			// 
			this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtMessage.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.txtMessage.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.txtMessage.ContextMenuStrip = this.contextMenu;
			this.txtMessage.Location = new System.Drawing.Point(65, 73);
			this.txtMessage.Multiline = true;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.Size = new System.Drawing.Size(217, 66);
			this.txtMessage.TabIndex = 4;
			this.toolTip.SetToolTip(this.txtMessage, "Write a message to inject to the blackboard");
			this.txtMessage.TextChanged += new System.EventHandler(this.txtMessage_TextChanged);
			this.txtMessage.Leave += new System.EventHandler(this.txtMessage_Leave);
			this.txtMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMessage_KeyPress);
			this.txtMessage.Enter += new System.EventHandler(this.txtMessage_Enter);
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miUndo,
            this.miRedo,
            this.miSeparator2,
            this.miCut,
            this.miCopy,
            this.miPaste,
            this.miDelete,
            this.miSeparator1,
            this.miClear,
            this.miSelectAll,
            this.miSendToMessage});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(164, 214);
			this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
			// 
			// miUndo
			// 
			this.miUndo.Name = "miUndo";
			this.miUndo.Size = new System.Drawing.Size(163, 22);
			this.miUndo.Text = "Undo";
			this.miUndo.Click += new System.EventHandler(this.miUndo_Click);
			// 
			// miRedo
			// 
			this.miRedo.Name = "miRedo";
			this.miRedo.Size = new System.Drawing.Size(163, 22);
			this.miRedo.Text = "Redo";
			this.miRedo.Click += new System.EventHandler(this.miRedo_Click);
			// 
			// miSeparator2
			// 
			this.miSeparator2.Name = "miSeparator2";
			this.miSeparator2.Size = new System.Drawing.Size(160, 6);
			// 
			// miCut
			// 
			this.miCut.Name = "miCut";
			this.miCut.Size = new System.Drawing.Size(163, 22);
			this.miCut.Text = "Cut";
			this.miCut.Click += new System.EventHandler(this.miCut_Click);
			// 
			// miCopy
			// 
			this.miCopy.Name = "miCopy";
			this.miCopy.Size = new System.Drawing.Size(163, 22);
			this.miCopy.Text = "&Copy";
			this.miCopy.Click += new System.EventHandler(this.miCopy_Click);
			// 
			// miPaste
			// 
			this.miPaste.Name = "miPaste";
			this.miPaste.Size = new System.Drawing.Size(163, 22);
			this.miPaste.Text = "Paste";
			this.miPaste.Click += new System.EventHandler(this.miPaste_Click);
			// 
			// miDelete
			// 
			this.miDelete.Name = "miDelete";
			this.miDelete.Size = new System.Drawing.Size(163, 22);
			this.miDelete.Text = "&Delete";
			this.miDelete.Click += new System.EventHandler(this.miDelete_Click);
			// 
			// miSeparator1
			// 
			this.miSeparator1.Name = "miSeparator1";
			this.miSeparator1.Size = new System.Drawing.Size(160, 6);
			// 
			// miClear
			// 
			this.miClear.Name = "miClear";
			this.miClear.Size = new System.Drawing.Size(163, 22);
			this.miClear.Text = "Clear";
			this.miClear.Click += new System.EventHandler(this.miClear_Click);
			// 
			// miSelectAll
			// 
			this.miSelectAll.Name = "miSelectAll";
			this.miSelectAll.Size = new System.Drawing.Size(163, 22);
			this.miSelectAll.Text = "Select All";
			this.miSelectAll.Click += new System.EventHandler(this.miSelectAll_Click);
			// 
			// miSendToMessage
			// 
			this.miSendToMessage.Name = "miSendToMessage";
			this.miSendToMessage.Size = new System.Drawing.Size(163, 22);
			this.miSendToMessage.Text = "&Send to message";
			this.miSendToMessage.Click += new System.EventHandler(this.miSendToMessage_Click);
			// 
			// gbHistory
			// 
			this.gbHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.gbHistory.Controls.Add(this.btnHystoryFilter);
			this.gbHistory.Controls.Add(this.cmbHystoryFilter);
			this.gbHistory.Controls.Add(this.lblHystoryFilter);
			this.gbHistory.Controls.Add(this.lstHistory);
			this.gbHistory.Location = new System.Drawing.Point(3, 3);
			this.gbHistory.Name = "gbHistory";
			this.gbHistory.Size = new System.Drawing.Size(253, 174);
			this.gbHistory.TabIndex = 5;
			this.gbHistory.TabStop = false;
			this.gbHistory.Text = "Injection History";
			// 
			// cmbHystoryFilter
			// 
			this.cmbHystoryFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cmbHystoryFilter.FormattingEnabled = true;
			this.cmbHystoryFilter.Location = new System.Drawing.Point(44, 19);
			this.cmbHystoryFilter.Name = "cmbHystoryFilter";
			this.cmbHystoryFilter.Size = new System.Drawing.Size(176, 21);
			this.cmbHystoryFilter.TabIndex = 8;
			this.toolTip.SetToolTip(this.cmbHystoryFilter, "Type to filter the injection history list");
			this.cmbHystoryFilter.SelectedIndexChanged += new System.EventHandler(this.cmbHystoryFilter_SelectedIndexChanged);
			this.cmbHystoryFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbHystoryFilter_KeyPress);
			// 
			// lblHystoryFilter
			// 
			this.lblHystoryFilter.AutoSize = true;
			this.lblHystoryFilter.Location = new System.Drawing.Point(6, 22);
			this.lblHystoryFilter.Name = "lblHystoryFilter";
			this.lblHystoryFilter.Size = new System.Drawing.Size(32, 13);
			this.lblHystoryFilter.TabIndex = 7;
			this.lblHystoryFilter.Text = "Filter:";
			// 
			// lstHistory
			// 
			this.lstHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lstHistory.ContextMenuStrip = this.contextMenu;
			this.lstHistory.FormattingEnabled = true;
			this.lstHistory.Location = new System.Drawing.Point(6, 46);
			this.lstHistory.Name = "lstHistory";
			this.lstHistory.Size = new System.Drawing.Size(241, 121);
			this.lstHistory.TabIndex = 6;
			this.toolTip.SetToolTip(this.lstHistory, "Double click to inject the selected message");
			this.lstHistory.DoubleClick += new System.EventHandler(this.lstHistory_DoubleClick);
			// 
			// gbInjector
			// 
			this.gbInjector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbInjector.Controls.Add(this.btnQuickCommand);
			this.gbInjector.Controls.Add(this.chkBulk);
			this.gbInjector.Controls.Add(this.lblQuickCommand);
			this.gbInjector.Controls.Add(this.lblSourceModule);
			this.gbInjector.Controls.Add(this.cmbQuickCommand);
			this.gbInjector.Controls.Add(this.cbModules);
			this.gbInjector.Controls.Add(this.lblMessage);
			this.gbInjector.Controls.Add(this.txtMessage);
			this.gbInjector.Controls.Add(this.btnInject);
			this.gbInjector.Location = new System.Drawing.Point(262, 3);
			this.gbInjector.Name = "gbInjector";
			this.gbInjector.Size = new System.Drawing.Size(285, 174);
			this.gbInjector.TabIndex = 6;
			this.gbInjector.TabStop = false;
			this.gbInjector.Text = "Inject command/response";
			// 
			// chkBulk
			// 
			this.chkBulk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.chkBulk.AutoSize = true;
			this.chkBulk.Location = new System.Drawing.Point(65, 149);
			this.chkBulk.Name = "chkBulk";
			this.chkBulk.Size = new System.Drawing.Size(77, 17);
			this.chkBulk.TabIndex = 5;
			this.chkBulk.Text = "Bulk Mode";
			this.toolTip.SetToolTip(this.chkBulk, "Enable to inject multiple commands simultaneously");
			this.chkBulk.UseVisualStyleBackColor = true;
			// 
			// lblQuickCommand
			// 
			this.lblQuickCommand.AutoSize = true;
			this.lblQuickCommand.Location = new System.Drawing.Point(6, 49);
			this.lblQuickCommand.Name = "lblQuickCommand";
			this.lblQuickCommand.Size = new System.Drawing.Size(38, 13);
			this.lblQuickCommand.TabIndex = 0;
			this.lblQuickCommand.Text = "Quick:";
			// 
			// cmbQuickCommand
			// 
			this.cmbQuickCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cmbQuickCommand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbQuickCommand.FormattingEnabled = true;
			this.cmbQuickCommand.Location = new System.Drawing.Point(65, 46);
			this.cmbQuickCommand.Name = "cmbQuickCommand";
			this.cmbQuickCommand.Size = new System.Drawing.Size(187, 21);
			this.cmbQuickCommand.TabIndex = 1;
			this.toolTip.SetToolTip(this.cmbQuickCommand, "Select to append a command prototype to the message textbox");
			this.cmbQuickCommand.SelectedIndexChanged += new System.EventHandler(this.cmbQuickCommand_SelectedIndexChanged);
			// 
			// btnQuickCommand
			// 
			this.btnQuickCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQuickCommand.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnQuickCommand.Image = global::Blk.Gui.Properties.Resources.Add_16;
			this.btnQuickCommand.Location = new System.Drawing.Point(258, 46);
			this.btnQuickCommand.Name = "btnQuickCommand";
			this.btnQuickCommand.Size = new System.Drawing.Size(21, 21);
			this.btnQuickCommand.TabIndex = 9;
			this.toolTip.SetToolTip(this.btnQuickCommand, "Append a command prototype to the message textbox");
			this.btnQuickCommand.UseVisualStyleBackColor = true;
			this.btnQuickCommand.Click += new System.EventHandler(this.btnQuickCommand_Click);
			// 
			// btnHystoryFilter
			// 
			this.btnHystoryFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnHystoryFilter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnHystoryFilter.Image = global::Blk.Gui.Properties.Resources.Filter2HS;
			this.btnHystoryFilter.Location = new System.Drawing.Point(226, 19);
			this.btnHystoryFilter.Name = "btnHystoryFilter";
			this.btnHystoryFilter.Size = new System.Drawing.Size(21, 21);
			this.btnHystoryFilter.TabIndex = 9;
			this.toolTip.SetToolTip(this.btnHystoryFilter, "Apply the selected filter to the injection history list");
			this.btnHystoryFilter.UseVisualStyleBackColor = true;
			this.btnHystoryFilter.Click += new System.EventHandler(this.btnHystoryFilter_Click);
			// 
			// InjectorTool
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gbInjector);
			this.Controls.Add(this.gbHistory);
			this.Name = "InjectorTool";
			this.Size = new System.Drawing.Size(550, 180);
			this.contextMenu.ResumeLayout(false);
			this.gbHistory.ResumeLayout(false);
			this.gbHistory.PerformLayout();
			this.gbInjector.ResumeLayout(false);
			this.gbInjector.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblSourceModule;
		private System.Windows.Forms.ComboBox cbModules;
		private System.Windows.Forms.Label lblMessage;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button btnInject;
		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.GroupBox gbHistory;
		private System.Windows.Forms.ListBox lstHistory;
		private System.Windows.Forms.GroupBox gbInjector;
		private System.Windows.Forms.CheckBox chkBulk;
		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.ToolStripMenuItem miCut;
		private System.Windows.Forms.ToolStripMenuItem miCopy;
		private System.Windows.Forms.ToolStripMenuItem miDelete;
		private System.Windows.Forms.ToolStripMenuItem miPaste;
		private System.Windows.Forms.ToolStripMenuItem miSendToMessage;
		private System.Windows.Forms.ToolStripMenuItem miUndo;
		private System.Windows.Forms.ToolStripMenuItem miRedo;
		private System.Windows.Forms.ToolStripSeparator miSeparator2;
		private System.Windows.Forms.ToolStripSeparator miSeparator1;
		private System.Windows.Forms.ToolStripMenuItem miClear;
		private System.Windows.Forms.ToolStripMenuItem miSelectAll;
		private System.Windows.Forms.Button btnHystoryFilter;
		private System.Windows.Forms.ComboBox cmbHystoryFilter;
		private System.Windows.Forms.Label lblHystoryFilter;
		private System.Windows.Forms.Label lblQuickCommand;
		private System.Windows.Forms.ComboBox cmbQuickCommand;
		private System.Windows.Forms.Button btnQuickCommand;
	}
}
