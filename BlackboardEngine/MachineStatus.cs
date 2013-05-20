using System;
using System.Text.RegularExpressions;
using System.Net;

namespace Blk.Engine
{
	/// <summary>
	/// Enumerates the status of battery
	/// </summary>
	[FlagsAttribute]
	public enum BatteryChargeStatus
	{
		/// <summary>
		/// Indicates a battery is charging.
		/// </summary>
		Charging = 8,
		/// <summary>
		/// Indicates a critically low level of battery charge.
		/// </summary>
		Critical = 4,
		/// <summary>
		/// Indicates a high level of battery charge.
		/// </summary>
		High = 1,
		/// <summary>
		/// Indicates a low level of battery charge.
		/// </summary>
		Low = 2,
		/// <summary>
		/// Indicates that no battery is present.
		/// </summary>
		NoSystemBattery = 128,
		/// <summary>
		/// Indicates an unknown battery condition.
		/// </summary>
		Unknown = 255
	};

	/// <summary>
	/// Represents the status of a remote machine
	/// </summary>
	public class MachineStatus : IComparable<MachineStatus>
	{
		#region Variables

		/// <summary>
		/// Stores the IP Address of the current represented machine
		/// </summary>
		private IPAddress ip;
		/// <summary>
		/// Stores the battery charge status
		/// </summary>
		private BatteryChargeStatus batteryChargeStatus;
		/// <summary>
		/// Stores the battery remaining load percentage
		/// </summary>
		private int batteryLifePercent;
		/// <summary>
		/// Stores the average CPU usage
		/// </summary>
		private int cpu;
		/// <summary>
		/// Stores the average memory usage
		/// </summary>
		private int memory;
		/// <summary>
		/// Stores the percentage of free space in the default HDD
		/// </summary>
		private int disk;
		/// <summary>
		/// Stores aditional info
		/// </summary>
		private string info;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of MachineStatus
		/// </summary>
		/// <param name="ip">IP Address of the monitored device</param>
		/// <param name="blf">The remaining Battery Life Percentage</param>
		/// <param name="bcs">the Battery Charge Status</param>
		internal MachineStatus(IPAddress ip, int blf, BatteryChargeStatus bcs)
			: this(ip, blf, bcs, -1, -1, -1, "")
		{
		}

		/// <summary>
		/// Initializes a new instance of MachineStatus
		/// </summary>
		/// <param name="ip">IP Address of the monitored device</param>
		/// <param name="blf">The remaining Battery Life Percentage</param>
		/// <param name="bcs">the Battery Charge Status</param>
		/// <param name="cpu">The Average CPU usage percentage</param>
		internal MachineStatus(IPAddress ip, int blf, BatteryChargeStatus bcs, int cpu)
			: this(ip, blf, bcs, cpu, -1, -1, "")
		{
		}

		/// <summary>
		/// Initializes a new instance of MachineStatus
		/// </summary>
		/// <param name="ip">IP Address of the monitored device</param>
		/// <param name="blf">The remaining Battery Life Percentage</param>
		/// <param name="bcs">the Battery Charge Status</param>
		/// <param name="cpu">The Average CPU usage percentage</param>
		/// <param name="mem">The Average Memory usage percentage</param>
		internal MachineStatus(IPAddress ip, int blf, BatteryChargeStatus bcs, int cpu, int mem)
			: this(ip, blf, bcs, cpu, mem, -1, "")
		{
		}

		/// <summary>
		/// Initializes a new instance of MachineStatus
		/// </summary>
		/// <param name="ip">IP Address of the monitored device</param>
		/// <param name="blf">The remaining Battery Life Percentage</param>
		/// <param name="bcs">the Battery Charge Status</param>
		/// <param name="cpu">The Average CPU usage percentage</param>
		/// <param name="mem">The Average Memory usage percentage</param>
		/// <param name="disk">The percentage of free space in the deault HDD</param>
		internal MachineStatus(IPAddress ip, int blf, BatteryChargeStatus bcs, int cpu, int mem, int disk)
			: this(ip, blf, bcs, cpu, mem, disk, "")
		{
		}

		/// <summary>
		/// Initializes a new instance of MachineStatus
		/// </summary>
		/// <param name="ip">IP Address of the monitored device</param>
		/// <param name="blf">The remaining Battery Life Percentage</param>
		/// <param name="bcs">the Battery Charge Status</param>
		/// <param name="cpu">The Average CPU usage percentage</param>
		/// <param name="mem">The Average Memory usage percentage</param>
		/// <param name="disk">The percentage of free space in the deault HDD</param>
		/// <param name="info">Sores aditional info</param>
		internal MachineStatus(IPAddress ip, int blf, BatteryChargeStatus bcs, int cpu, int mem, int disk, string info)
		{
			this.ip = ip;
			this.batteryLifePercent = blf;
			this.batteryChargeStatus = bcs;
			if ((cpu > 100) || (cpu < -1)) throw new ArgumentOutOfRangeException("cpu");
			if ((mem > 100) || (mem < -1)) throw new ArgumentOutOfRangeException("mem");
			if ((disk > 100) || (disk < -1)) throw new ArgumentOutOfRangeException("disk");
			this.cpu = cpu;
			this.memory = mem;
			this.disk = disk;
			if (info == null) info = "";
			else this.info = info.Trim();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the current battery charge status.
		/// </summary>
		public BatteryChargeStatus BatteryChargeStatus
		{
			get { return this.batteryChargeStatus; }
		}

		/// <summary>
		/// Gets the approximate percentage of full battery time remaining.
		/// </summary>
		/// <remarks>A -1 value indicates an unknown value</remarks>
		public int BatteryLifePercent
		{
			get { return batteryLifePercent; }
		}

		/// <summary>
		/// Gets the average CPU usage
		/// </summary>
		/// <remarks>A -1 value indicates an unknown value</remarks>
		public int CPUPercentage
		{
			get { return cpu; }
		}
		/// <summary>
		/// Gets the IP Address of remote machine
		/// </summary>
		public IPAddress IPAddress
		{
			get { return ip; }
		}
		/// <summary>
		/// Gets the average memory usage
		/// </summary>
		/// <remarks>A -1 value indicates an unknown value</remarks>
		public int MemoryPercentage
		{
			get { return memory; }
		}
		/// <summary>
		/// Gets the percentage of free space in the default HDD
		/// </summary>
		/// <remarks>A -1 value indicates an unknown value</remarks>
		public int FreeDiskPercentage
		{
			get { return disk; }
		}
		/// <summary>
		/// Gets additional info
		/// </summary>
		public string AdditionalInfo
		{
			get { return info; }
		}

		#endregion

		#region Events
		#endregion

		#region Methods

		/// <summary>
		/// Returns the string representation of the current MachineStatus object
		/// </summary>
		/// <returns>string representation of the current object</returns>
		public override string ToString()
		{
			System.Text.StringBuilder sb;

			sb = new System.Text.StringBuilder();
			sb.Append("ip");
			sb.Append(this.IPAddress.ToString());
			sb.Append(' ');

			sb.Append("bsc");
			sb.Append(this.BatteryLifePercent);

			if ((this.batteryChargeStatus & BatteryChargeStatus.Charging) == BatteryChargeStatus.Charging)
				sb.Append("c");
			else if ((this.batteryChargeStatus & BatteryChargeStatus.NoSystemBattery) == BatteryChargeStatus.NoSystemBattery)
				sb.Append("n");
			else if ((this.batteryChargeStatus & BatteryChargeStatus.Unknown) == BatteryChargeStatus.Unknown)
				sb.Append("u");

			if (this.cpu != -1)
			{
				sb.Append(' ');
				sb.Append("cpu");
				sb.Append(this.cpu.ToString());
			}

			if (this.memory != -1)
			{
				sb.Append(' ');
				sb.Append("mem");
				sb.Append(this.memory.ToString());
			}

			if (this.disk != -1)
			{
				sb.Append(' ');
				sb.Append("disk");
				sb.Append(this.disk.ToString());
			}

			if (this.info.Length > 0)
			{
				sb.Append(' ');
				sb.Append("info");
				sb.Append(this.info);
			}

			return sb.ToString();
		}

		#region IComparable<MachineStatus> Members
		/// <summary>
		/// IP based comparison used between two MachineStatus objects
		/// </summary>
		/// <param name="other">MachineStatus object to compare with</param>
		/// <returns>Result of the operation: (int)(this.ip.Address - other.ip.Address)</returns>
		public int CompareTo(MachineStatus other)
		{
			return (int)(this.ip.ToString().CompareTo(other.ip.ToString()));
		}

		#endregion

		#endregion

		#region Static Methods

		/// <summary>
		/// Converts the string representation of a machine status into its MachineStatus equialent
		/// <param name="s">A string containing the data to convert</param>
		/// </summary>
		/// <returns>A MachineStatus equivalent to the data in s.</returns>
		public static MachineStatus Parse(string s)
		{
			string pattern;
			Regex rx;
			Match m;
			string tmp;
			int percentage;
			BatteryChargeStatus status;
			IPAddress ip;
			int cpu;
			int mem;
			int disk;
			string info;


			pattern = @"ip(?<ip>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\s+";
			pattern+= @"bcs(?<percentage>\d{1,3})(?<charging>[bcnu])";
			//pattern += @"(\s+cpu(?<cpu>\d{1,3}))?";
			//pattern += @"(\s+mem(?<mem>\d{1,3}))?";
			//pattern += @"(\s+disk(?<disk>\d{1,3}))?";
			//pattern += @"(\s+info(?<info>[\(\)\*\[\]\s\w\.\-]+))?";
			rx = new Regex(pattern, RegexOptions.Compiled);
			m = rx.Match(s);
			if (!m.Success)
				throw new Exception("Input string has incorrect format");
			if(!IPAddress.TryParse(m.Result("${ip}"), out ip))
				throw new Exception("Invalid IP Address in input istring");
			percentage = Int32.Parse(m.Result("${percentage}"));
			switch (m.Result("${charging}")[0])
			{
				case 'c':
					status = BatteryChargeStatus.Charging;
					break;

				case 'n':
					status = BatteryChargeStatus.NoSystemBattery;
					break;

				case 'u':
					status = BatteryChargeStatus.Unknown;
					break;

				default:
					status = (BatteryChargeStatus)0;
					break;
			}

			if (status != BatteryChargeStatus.Unknown)
			{
				if (percentage >= 50)
					status |= BatteryChargeStatus.High;
				else if ((percentage > 20) && (percentage < 50))
					status |= BatteryChargeStatus.Low;
				else
					status |= BatteryChargeStatus.Critical;
			}

			tmp = m.Result("${cpu}");
			if ((tmp != null) && (tmp.Length > 0) && (tmp != "${cpu}"))
				cpu = Int32.Parse(s);
			else
				cpu = -1;

			tmp = m.Result("${mem}");
			if ((tmp != null) && (tmp.Length > 0) && (tmp != "${mem}"))
				mem = Int32.Parse(s);
			else
				mem = -1;

			tmp = m.Result("${disk}");
			if ((tmp != null) && (tmp.Length > 0) && (tmp != "${disk}"))
				disk = Int32.Parse(s);
			else
				disk = -1;

			tmp = m.Result("${info}");
			if (tmp != "${info}")
				info = tmp;
			else info = "";

			return new MachineStatus(ip, percentage, status, cpu, mem, disk, info);
		}

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the machine status to convert</param>
		/// <param name="result">When this method returns, contains the MachineStatus equivalent to the machine status
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string s, out MachineStatus result)
		{
			try
			{
				result = Parse(s);
				return true;
			}
			catch 
			{
				result = null;
				return false; 
			}
		}

		#endregion
	}
}
