using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Blk.Engine;
using BatteryChargeStatus = Blk.Engine.BatteryChargeStatus;

namespace Blk.Gui
{
	public partial class MachineStatusControl : UserControl
	{
		#region Variables

		/// <summary>
		/// Stores the Batery Load Percentage shown in the current control.
		/// </summary>
		private MachineStatus machineStatus;

		#endregion

		#region Constructors
		
		/// <summary>
		/// Initializes a new instance of PowerMeter
		/// </summary>
		public MachineStatusControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of PowerMeter
		/// </summary>
		/// <param name="status">Initial status to start with</param>
		public MachineStatusControl(MachineStatus status) : this()
		{
			this.MachineStatus = status;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the Batery Load Percentage shown in the current control.
		/// </summary>
		public MachineStatus MachineStatus
		{
			get { return machineStatus; }
			set
			{
				if (value == null) throw new ArgumentNullException();
				machineStatus = value;
				Refresh();
			}
		}

		/// /// <summary>
		/// Gets or sets the caption text of the current control.
		/// </summary>
		public override string Text
		{
			get { return machineStatus.IPAddress.ToString(); }
		}

		#endregion

		#region Methods

		public override void Refresh()
		{
			this.lblPercentage.Text = machineStatus.BatteryLifePercent.ToString() + "%";
			this.lblIP.Text = machineStatus.IPAddress.ToString();

			if ((machineStatus.BatteryChargeStatus & BatteryChargeStatus.Unknown) == BatteryChargeStatus.Unknown)
			{
				pbxPercentage.Image = Properties.Resources.BatUnknown_48;
				this.lblPercentage.Text = "Unknown";
			}
			else if ((machineStatus.BatteryChargeStatus & BatteryChargeStatus.NoSystemBattery) == BatteryChargeStatus.NoSystemBattery)
			{
				pbxPercentage.Image = Properties.Resources.ACPpow_48;
				this.lblPercentage.Text = "100%";
			}
			else if ((machineStatus.BatteryChargeStatus & BatteryChargeStatus.Charging) == BatteryChargeStatus.Charging)
			{
				if (machineStatus.BatteryLifePercent >= 85)
					pbxPercentage.Image = Properties.Resources.BatChr100_48;
				else if ((machineStatus.BatteryLifePercent >= 70) && (machineStatus.BatteryLifePercent < 85))
					pbxPercentage.Image = Properties.Resources.BatChr080_48;
				else if ((machineStatus.BatteryLifePercent >= 55) && (machineStatus.BatteryLifePercent < 70))
					pbxPercentage.Image = Properties.Resources.BatChr060_48;
				else if ((machineStatus.BatteryLifePercent >= 25) && (machineStatus.BatteryLifePercent < 40))
					pbxPercentage.Image = Properties.Resources.BatChr040_48;
				else if ((machineStatus.BatteryLifePercent >= 10) && (machineStatus.BatteryLifePercent < 25))
					pbxPercentage.Image = Properties.Resources.BatChr020_48;
				else
					pbxPercentage.Image = Properties.Resources.BatChr000_48;
			}
			else
			{
				if (machineStatus.BatteryLifePercent >= 85)
					pbxPercentage.Image = Properties.Resources.BatDis100_48;
				else if ((machineStatus.BatteryLifePercent >= 70) && (machineStatus.BatteryLifePercent < 85))
					pbxPercentage.Image = Properties.Resources.BatDis080_48;
				else if ((machineStatus.BatteryLifePercent >= 55) && (machineStatus.BatteryLifePercent < 70))
					pbxPercentage.Image = Properties.Resources.BatDis060_48;
				else if ((machineStatus.BatteryLifePercent >= 25) && (machineStatus.BatteryLifePercent < 40))
					pbxPercentage.Image = Properties.Resources.BatDis040_48;
				else if ((machineStatus.BatteryLifePercent >= 10) && (machineStatus.BatteryLifePercent < 25))
					pbxPercentage.Image = Properties.Resources.BatDis020_48;
				else
					pbxPercentage.Image = Properties.Resources.BatDis000_48;
			}

			base.Refresh();
		}

		#endregion

		#region Event Handler Functions
		
		#endregion
	}
}
