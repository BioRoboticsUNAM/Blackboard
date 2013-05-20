namespace Blk.Gui
{
	partial class MachineStatusControl
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
			this.lblIP = new System.Windows.Forms.Label();
			this.pbxPercentage = new System.Windows.Forms.PictureBox();
			this.lblPercentage = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pbxPercentage)).BeginInit();
			this.SuspendLayout();
			// 
			// lblIP
			// 
			this.lblIP.AutoSize = true;
			this.lblIP.Location = new System.Drawing.Point(54, 32);
			this.lblIP.Name = "lblIP";
			this.lblIP.Size = new System.Drawing.Size(91, 13);
			this.lblIP.TabIndex = 0;
			this.lblIP.Text = "255.255.255.255:";
			// 
			// pbxPercentage
			// 
			this.pbxPercentage.Image = global::Blk.Gui.Properties.Resources.BatUnknown_48;
			this.pbxPercentage.Location = new System.Drawing.Point(0, 0);
			this.pbxPercentage.Name = "pbxPercentage";
			this.pbxPercentage.Size = new System.Drawing.Size(48, 48);
			this.pbxPercentage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pbxPercentage.TabIndex = 7;
			this.pbxPercentage.TabStop = false;
			// 
			// lblPercentage
			// 
			this.lblPercentage.AutoSize = true;
			this.lblPercentage.Location = new System.Drawing.Point(54, 16);
			this.lblPercentage.Name = "lblPercentage";
			this.lblPercentage.Size = new System.Drawing.Size(33, 13);
			this.lblPercentage.TabIndex = 6;
			this.lblPercentage.Text = "100%";
			this.lblPercentage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// MachineStatusControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pbxPercentage);
			this.Controls.Add(this.lblPercentage);
			this.Controls.Add(this.lblIP);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.MaximumSize = new System.Drawing.Size(145, 48);
			this.MinimumSize = new System.Drawing.Size(145, 48);
			this.Name = "MachineStatusControl";
			this.Size = new System.Drawing.Size(145, 48);
			((System.ComponentModel.ISupportInitialize)(this.pbxPercentage)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblIP;
		private System.Windows.Forms.PictureBox pbxPercentage;
		private System.Windows.Forms.Label lblPercentage;
	}
}
