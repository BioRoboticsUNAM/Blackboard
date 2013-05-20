using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Blk.Gui
{
	public enum PowerMeterMode { Compact, Graphic };

	public partial class PowerMeter : UserControl
	{
		#region Variables

		/// <summary>
		/// Stores the display mode of the current control
		/// </summary>
		private PowerMeterMode mode;
		/// <summary>
		/// Stores the Batery Load Percentage shown in the current control.
		/// </summary>
		private int bateryLoadPercentage;
		/// <summary>
		/// Stores the caption text of the current control.
		/// </summary>
		private string text;
		/// <summary>
		/// Stores the last width used in compact mode
		/// </summary>
		private int compactWidth;

		#endregion

		#region Constructors
		
		/// <summary>
		/// Initializes a new instance of PowerMeter
		/// </summary>
		public PowerMeter()
		{
			InitializeComponent();
			compactWidth = 247;
			Mode = PowerMeterMode.Graphic;
			text = "255.255.255.255";
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the Batery Load Percentage shown in the current control.
		/// </summary>
		public int BateryLoadPercentage
		{
			get { return bateryLoadPercentage; }
			set
			{
				if ((value < 0) || (value > 100)) throw new ArgumentOutOfRangeException("Value must be between 0 and 100");
				bateryLoadPercentage = value;
				Refresh();
			}
		}

		/// /// <summary>
		/// Gets or sets the caption text of the current control.
		/// </summary>
		public override string Text
		{
			get { return text; }
			set
			{
				text = value;
				Refresh();
			}
		}

		/// <summary>
		/// Gets or sets the display mode of the current control
		/// </summary>
		public PowerMeterMode Mode
		{
			get { return mode; }
			set
			{
				mode = value;
				if (mode == PowerMeterMode.Compact)
				{
					this.lblIP.Visible = true;
					this.lblIP.Location = new Point(0, 3);

					this.pbrPercentage.Location = new Point(97, 0);
					this.pbrPercentage.Visible = true;
					this.pbrPercentage.Size = new Size(this.Width - 97, 20);

					this.lblPercentage.Visible = false;
					this.lblPercentage.Location = new Point(0, 0);

					this.pbxPercentage.Visible = false;
					this.pbxPercentage.Location = new Point(0, 3);
					this.pbxPercentage.Size = new Size(0, 0);

					this.MaximumSize = new Size(0, 20);
					this.MinimumSize = new Size(247, 20);
					this.Size = new Size(compactWidth, 20);
				}
				else if (mode == PowerMeterMode.Graphic)
				{
					this.lblIP.Visible = true;
					this.lblIP.Location = new Point(54, 32);

					this.pbrPercentage.Location = new Point(0, 0);
					this.pbrPercentage.Visible = false;
					this.pbrPercentage.Size = new Size(0, 0);

					this.lblPercentage.Location = new Point(54, 16);
					this.lblPercentage.Visible = true;

					this.pbxPercentage.Visible = true;
					this.pbxPercentage.Location = new Point(0, 0);
					this.pbxPercentage.Size = new Size(48, 48);

					this.MaximumSize = new Size(145, 48);
					this.MinimumSize = new Size(145, 48);
					this.Size = new Size(145, 48);
				}
				Refresh();
			}
		}

		#endregion

		#region Methods

		public override void Refresh()
		{
			this.lblPercentage.Text = this.bateryLoadPercentage + "%";
			this.lblIP.Text = this.text + (mode == PowerMeterMode.Compact ? ":" : "");
			base.Refresh();
		}

		#endregion

		#region Event Handler Functions

		private void PowerMeter_Resize(object sender, EventArgs e)
		{
			if (mode == PowerMeterMode.Compact)
			{
				lblIP.Location = new Point(0, 3);
				pbrPercentage.Location = new Point(97, 0);
				compactWidth = this.Width - 97;
				pbrPercentage.Size = new Size(compactWidth, 20);
			}
		}
		
		#endregion
	}
}
