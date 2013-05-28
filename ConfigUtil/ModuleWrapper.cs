using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Blk.Engine;

namespace ConfigUtil
{
	public class ModuleWrapper
	{
		#region Variables

		private ModuleClient module;

		#endregion

		#region Constructor

		public ModuleWrapper()
		{
		}

		public ModuleWrapper(ModuleClient module)
		{
			this.module = module;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the name of the module
		/// </summary>
		[
			ParenthesizePropertyName(true),
			DescriptionAttribute("Name used to identify the module"),
			Category("General"),
			DisplayName("Module Name")
		]
		public string Name
		{
			get
			{
				return (this.module != null) ? this.module.Name : String.Empty;
			}
			set
			{
				if (this.module != null)
				{
					if (!Validator.IsValidModuleName(value))
						throw new ArgumentException("Invalid module name");
					this.module.Name = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the alias of the module
		/// </summary>
		[
			DescriptionAttribute("Alternative name used to identify the module"),
			Category("General"),
			DisplayName("Module Alias")
		]
		public string Alias
		{
			get { return (this.module != null) ? this.module.Alias : String.Empty; }
			set
			{
				if (this.module != null)
				{
					if (!Validator.IsValidModuleName(value))
						throw new ArgumentException("Invalid module alias");
					this.module.Alias = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets a flag that indicates if the blackboard will check the status of the module
		/// </summary>
		[
			DescriptionAttribute("When enabled, the Blackboard will monitor the status of this module using the Alive, Busy and Ready commands."),
			Category("Settings"),
			DisplayName("Check Status"),
			DefaultValue(true)
		]
		public bool AliveCheck
		{
			get { return (this.module != null) ? this.module.AliveCheck : false; }
			set { if (this.module != null)this.module.AliveCheck = value; }
		}

		/// <summary>
		/// Gets or sets a flag that indicates if the interval of alive check
		/// </summary>
		[
			DescriptionAttribute("Interval between monitor commands in milliseconds"),
			Category("Settings"),
			DisplayName("Check Interval")
		]
		public int CheckInterval
		{
			get
			{
				return (this.module != null) ? this.module.CheckInterval : -1;
			}
			set
			{
				if (this.module != null)
				{
					if (value < 0)
						value = -1;
					else if (value < 1000)
						value = 1000;
					this.module.CheckInterval = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the name of the author of the module
		/// </summary>
		[
			DescriptionAttribute("Stores the author's name of the remote application"),
			Category("General"),
			DisplayName("Module Author")
		]
		public string Author
		{
			get { return (this.module != null) ? this.module.Author : String.Empty; }
			set
			{
				if (this.module != null)
					this.module.Author = value;
			}
		}

		/// <summary>
		/// Gets or sets a flag that indicates if the module is enabled
		/// </summary>
		[
			DescriptionAttribute("Enables or disables the load of this module. A disabled module will not be loaded by the Blackboard, nor it will be their command prototypes."),
			Category("General"),
			DisplayName("Module Enabled"),
			DefaultValue(true)
		]
		public bool Enabled
		{
			get { return (this.module != null) ? this.module.Enabled : false; }
			set
			{
				if (this.module != null)
					this.module.Enabled = value;
			}
		}

		/*
		/// <summary>
		/// Gets or sets the ip address used to connect to the module
		/// </summary>
		[
			DescriptionAttribute("An array of IP Addresses of the machine hosting the remote application"),
			Category("General"),
			DisplayName("IP Addresses"),
			DefaultValue("127.0.0.1"),
			TypeConverter(typeof(System.ComponentModel.IPAddressConverter))
		]
		public IPAddress[] Addresses
		{
			get { return (this.module != null) ? this.module.ServerAddresses.ToArray() : null; }
			
			set {
				if((value == null) || (this.module == null))
					return;
				for (int i = 0; i < value.Length; ++i)
				{
					if ((value[i] == IPAddress.Any) || (value[i] == IPAddress.Broadcast) || module.ServerAddresses.Contains(value[i]))
						continue;
					module.ServerAddresses.Add(value[i]);
				}
			}
			
		}

		/// <summary>
		/// Gets or sets the port used to connect to the module
		/// </summary>
		[
			DescriptionAttribute("The connection port of the remote application. Must be between 1024 and 65535"),
			Category("General"),
			DisplayName("Port")
		]
		public int Port
		{
			get { return (this.module != null) ? this.module.Port : -1; }
			set
			{
				if (this.module != null)
				{
					if ((value < 1024) || (value > 65535))
						throw new ArgumentOutOfRangeException("Port must be between 1024 and 65535");
					this.module.Port = value;
				}
			}
		}
		*/

		/// <summary>
		/// Gets or sets the delay used before send commands to the module
		/// </summary>
		[
			DescriptionAttribute("The amount of time the blackboard will wait before send the next command/response to the module. A -1 value means disabled."),
			Category("Settings"),
			DisplayName("Send Delay"),
			DefaultValue(-1)
		]
		public int SendDelay
		{
			get { return (this.module != null) ? this.module.SendDelay : -1; }
			set
			{
				if (this.module != null)
				{
					if (value < 0)
						value = -1;
					else if (value > 300)
						value = 300;
					this.module.sendDelay = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets a flag that indicates if the module requires sender prefix
		/// </summary>
		[
			DescriptionAttribute("When enabled, the Blackboard will pre append the source module to all command/responses sent to this module."),
			Category("Settings"),
			DisplayName("Require Prefix"),
			DefaultValue(false)
		]
		public bool RequirePrefix
		{
			get { return (this.module != null) ? this.module.RequirePrefix : false; }
			set { if (this.module != null) this.module.RequirePrefix = value; }
		}

		/// <summary>
		/// Gets or sets a flag that indicates if the module requires sender prefix
		/// </summary>
		[
			DescriptionAttribute("Enables or disables the module simulation. A simulated module is managed by the blackboard so no connection is stablished and a successfull response is automatically generated for each received command with the same parameters of the source command."),
			Category("Settings"),
			DisplayName("Simulate"),
			DefaultValue(false)
		]
		public double Simulate
		{
			get { return (this.module != null) ? this.module.Simulation.SuccessRatio : 2; }
			set
			{
				if (this.module != null)
				{
					if (value < 0)
						value = 2;
					else if (value > 1)
						value = 2;
					this.module.Simulation.SuccessRatio = value;
				}
			}
		}

		#region Action Lists Properties

		/*

		/// <summary>
		/// Gets the list of actions to perform when module is restarted
		/// </summary>
		[
			DescriptionAttribute("List of actions the blackboard will perform when a module is restarted"),
			Category("Actions"),
			DisplayName("On Restart")
		]
		public List<ActionConfigBase> RestartActions
		{
			get
			{
				return restartActions;
			}
		}

		/// <summary>
		/// Gets the list of actions to perform when a test restart is requested
		/// </summary>
		[
			DescriptionAttribute("List of actions the blackboard will perform when a RestartTest event occurs"),
			Category("Actions"),
			DisplayName("On Restart Test")
		]
		public List<ActionConfigBase> RestartTestActions
		{
			get
			{
				return restartTestActions;
			}
		}

		/// <summary>
		/// Gets the list of actions to perform when module is started
		/// </summary>
		[
			DescriptionAttribute("List of actions the blackboard will perform when a module is started"),
			Category("Actions"),
			DisplayName("On Start")
		]
		public List<ActionConfigBase> StartupActions
		{
			get
			{
				return startupActions;
			}
		}

		/// <summary>
		/// Gets the list of actions to perform when module is stopped
		/// </summary>
		[
			DescriptionAttribute("List of actions the blackboard will perform when a module is stopped"),
			Category("Actions"),
			DisplayName("On Stop")
		]
		public List<ActionConfigBase> StopActions
		{
			get
			{
				return stopActions;
			}
		}

		/// <summary>
		/// Gets the list of actions to perform when TestTimeOut is requested
		/// </summary>
		[
			DescriptionAttribute("List of actions the blackboard will perform when a TestTimeOut event occurs"),
			Category("Actions"),
			DisplayName("On Test Timeout")
		]
		public List<ActionConfigBase> TestTimeOutActions
		{
			get { return testTimeOutActions; }
		}

		*/

		#endregion

		#endregion

		public override string ToString()
		{
			return (this.module != null) ? this.module.Name : base.ToString();
		}
	}
}