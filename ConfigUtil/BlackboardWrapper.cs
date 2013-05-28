using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Blk.Engine;
using Blackboard = Blk.Engine.Blackboard;

namespace ConfigUtil
{
	//[Serializable]
	[XmlRootAttribute("blackboard", Namespace = "", IsNullable = false)]
	public class BlackboardWrapper
	{
		#region Variables

		private Blackboard blackboard;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of BlackboardConfig
		/// </summary>
		public BlackboardWrapper()
		{
			this.blackboard = null;
		}

		/// <summary>
		/// Initializes a new instance of BlackboardConfig
		/// </summary>
		public BlackboardWrapper(Blackboard blackboard)
		{
			this.blackboard = blackboard;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the amount of time to wait before stop automatically the blackboard
		/// </summary>
		[
			DescriptionAttribute("Specifies the time (in milliseconds) when the AutoStop event will occur and the blackboard will shutdown automatically"),
			Category("Settings"),
			DisplayName("Autostop time"),
			DefaultValue(-1)
		]
		public int AutoStopTime
		{
			get
			{
				return ((this.blackboard != null) && (this.blackboard.AutoStopTime.TotalMilliseconds >= 0)) ?
					(int)this.blackboard.AutoStopTime.TotalMilliseconds : -1;
			}
			set
			{
				if (this.blackboard != null)
				{
					if (value < -1) value = -1;
					this.blackboard.AutoStopTime = new TimeSpan(0, 0, 0, 0, value);
				}
			}
		}

		[Browsable(false)]
		public Blackboard Blackboard
		{
			get { return this.blackboard; }
			set { this.blackboard = value; }
		}

		/// <summary>
		/// Gets or sets the name of the Blackboard virtual module
		/// </summary>
		[
			ParenthesizePropertyName(true),
			DescriptionAttribute("Name used to identify the module"),
			Category("General"),
			DisplayName("Module Name"),
			DefaultValue("BLACKBOARD")
		]
		public string Name
		{
			get
			{
				return (this.blackboard != null) ? this.blackboard.VirtualModule.Name : String.Empty;
			}
			set
			{
				if (this.blackboard != null)
				{
					if (!Validator.IsValidModuleName(value))
						throw new ArgumentException("Invalid module name");
					this.blackboard.VirtualModule.Name = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the port used to connect to the blackboard
		/// </summary>
		[
			DescriptionAttribute("The input connection port of the blackboard. Must be between 1024 and 65535"),
			Category("General"),
			DisplayName("Port"),
			DefaultValue(2300)
		]
		public int Port
		{
			get
			{
				return (this.blackboard != null) ? this.blackboard.Port : -1;
			}
			set
			{

				if (this.blackboard != null)
				{
					if ((value < 1024) || (value > 65535))
						throw new ArgumentOutOfRangeException("Port must be between 1024 and 65535");
					this.blackboard.Port = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the number of attempts while redirecting a response
		/// </summary>
		[
			DescriptionAttribute("Specifies the number of retry send attempts for command/response redirection"),
			Category("General"),
			DisplayName("Retry Send Attempts"),
			DefaultValue(0)
		]
		public int SendAttempts
		{
			get
			{
				return (this.blackboard != null) ? this.blackboard.SendAttempts : -1;
			}
			set
			{
				if (this.blackboard != null)
				{
					if (value < 0) value = 0;
					this.blackboard.SendAttempts = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the amount of time to wait before perform Test-Timeout actions
		/// </summary>
		[
			DescriptionAttribute("Specifies the time (in milliseconds) when the TestTimeOut event will occur"),
			Category("Settings"),
			DisplayName("Test Timeout"),
			DefaultValue(-1)
		]
		public int TestTimeOut
		{
			get
			{
				return ((this.blackboard != null) && (this.blackboard.TestTimeOut.TotalMilliseconds >= 0)) ?
					(int)this.blackboard.TestTimeOut.TotalMilliseconds : -1;
			}
			set
			{
				if (this.blackboard != null)
				{
					if (value < -1) value = -1;
					this.blackboard.TestTimeOut = new TimeSpan(0, 0, 0, 0, value);
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether a module in the blackboard.
		/// </summary>
		/// <param name="moduleName">Name of module to locate in the module collection of the blackboard</param>
		/// <returns>true if module is in the collection. false otherwise</returns>
		public bool ContainsModule(string moduleName)
		{
			if (this.blackboard == null)
				return false;
			foreach (ModuleClient m in this.blackboard.Modules)
			{
				if ((moduleName == m.Name) || (moduleName == m.Alias))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Determines whether a module in the blackboard.
		/// </summary>
		/// <param name="module">Module to locate in the module collection of the blackboard</param>
		/// <returns>true if module is in the collection. false otherwise</returns>
		public bool ContainsModule(ModuleClient module)
		{
			if (this.blackboard == null)
				return false;
			return this.blackboard.Modules.Contains(module);
		}

		/// <summary>
		/// Determines whether a prototype is in the blackboard.
		/// </summary>
		/// <param name="prototype">Prototype to locate in the blackboard</param>
		/// <returns>true if Prototype is contained in the blackboard. false otherwise</returns>
		public bool ContainsPrototype(Prototype prototype)
		{
			if (this.blackboard == null)
				return false;
			foreach (ModuleClient m in this.blackboard.Modules)
				if (m.Prototypes.Contains(prototype)) return true;
			return false;
		}

		/// <summary>
		/// Determines whether a command is supported by the blackboard.
		/// </summary>
		/// <param name="moduleName">Name of the command to locate in the blackboard</param>
		/// <returns>true if command is supported by the blackboard. false otherwise</returns>
		public bool SupportsCommand(string commandName)
		{
			if (this.blackboard == null)
				return false;
			foreach (ModuleClient m in this.blackboard.Modules)
			{
				foreach (Prototype prototype in m.Prototypes)
					if (prototype.Command == commandName) return true;
			}
			return false;
		}

		#endregion
	}
}