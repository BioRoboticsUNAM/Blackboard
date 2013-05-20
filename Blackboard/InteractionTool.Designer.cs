namespace Blk.Gui
{
	partial class InteractionTool
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
			this.gbModuleInteraction = new System.Windows.Forms.GroupBox();
			this.txtSendToModule = new System.Windows.Forms.TextBox();
			this.lblLastSendToModuleResult = new System.Windows.Forms.Label();
			this.btnModuleInject = new System.Windows.Forms.Button();
			this.btnSendToModule = new System.Windows.Forms.Button();
			this.gbModuleInteraction.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbModuleInteraction
			// 
			this.gbModuleInteraction.Controls.Add(this.txtSendToModule);
			this.gbModuleInteraction.Controls.Add(this.lblLastSendToModuleResult);
			this.gbModuleInteraction.Controls.Add(this.btnModuleInject);
			this.gbModuleInteraction.Controls.Add(this.btnSendToModule);
			this.gbModuleInteraction.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbModuleInteraction.Location = new System.Drawing.Point(0, 0);
			this.gbModuleInteraction.MinimumSize = new System.Drawing.Size(300, 62);
			this.gbModuleInteraction.Name = "gbModuleInteraction";
			this.gbModuleInteraction.Size = new System.Drawing.Size(300, 62);
			this.gbModuleInteraction.TabIndex = 3;
			this.gbModuleInteraction.TabStop = false;
			this.gbModuleInteraction.Text = "Send Command/Response to module:";
			// 
			// txtSendToModule
			// 
			this.txtSendToModule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtSendToModule.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.txtSendToModule.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.txtSendToModule.Location = new System.Drawing.Point(6, 23);
			this.txtSendToModule.Name = "txtSendToModule";
			this.txtSendToModule.Size = new System.Drawing.Size(230, 20);
			this.txtSendToModule.TabIndex = 3;
			this.txtSendToModule.TextChanged += new System.EventHandler(this.txtSendToModule_TextChanged);
			this.txtSendToModule.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSendToModule_KeyDown);
			// 
			// lblLastSendToModuleResult
			// 
			this.lblLastSendToModuleResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblLastSendToModuleResult.AutoSize = true;
			this.lblLastSendToModuleResult.Location = new System.Drawing.Point(6, 46);
			this.lblLastSendToModuleResult.Name = "lblLastSendToModuleResult";
			this.lblLastSendToModuleResult.Size = new System.Drawing.Size(169, 13);
			this.lblLastSendToModuleResult.TabIndex = 5;
			this.lblLastSendToModuleResult.Text = "Send Command/Response: Failed";
			// 
			// btnModuleInject
			// 
			this.btnModuleInject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnModuleInject.FlatAppearance.BorderSize = 0;
			this.btnModuleInject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnModuleInject.Image = global::Blk.Gui.Properties.Resources.InjectBlue_16;
			this.btnModuleInject.Location = new System.Drawing.Point(271, 21);
			this.btnModuleInject.Name = "btnModuleInject";
			this.btnModuleInject.Size = new System.Drawing.Size(23, 23);
			this.btnModuleInject.TabIndex = 4;
			this.btnModuleInject.UseVisualStyleBackColor = true;
			this.btnModuleInject.Click += new System.EventHandler(this.btnModuleInject_Click);
			// 
			// btnSendToModule
			// 
			this.btnSendToModule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSendToModule.FlatAppearance.BorderSize = 0;
			this.btnSendToModule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSendToModule.Image = global::Blk.Gui.Properties.Resources.ArrowNext_16;
			this.btnSendToModule.Location = new System.Drawing.Point(242, 21);
			this.btnSendToModule.Name = "btnSendToModule";
			this.btnSendToModule.Size = new System.Drawing.Size(23, 23);
			this.btnSendToModule.TabIndex = 4;
			this.btnSendToModule.UseVisualStyleBackColor = true;
			this.btnSendToModule.Click += new System.EventHandler(this.btnSendToModule_Click);
			// 
			// InteractionTool
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gbModuleInteraction);
			this.MaximumSize = new System.Drawing.Size(0, 62);
			this.MinimumSize = new System.Drawing.Size(300, 62);
			this.Name = "InteractionTool";
			this.Size = new System.Drawing.Size(300, 62);
			this.gbModuleInteraction.ResumeLayout(false);
			this.gbModuleInteraction.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbModuleInteraction;
		private System.Windows.Forms.TextBox txtSendToModule;
		private System.Windows.Forms.Label lblLastSendToModuleResult;
		private System.Windows.Forms.Button btnModuleInject;
		private System.Windows.Forms.Button btnSendToModule;
	}
}
