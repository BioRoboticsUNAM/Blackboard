using System;
using System.Collections.Generic;
using Blk.Api;

namespace Blk.Engine.SharedVariables
{
	/// <summary>
	/// Implements a shared variable for the list of alive modules
	/// </summary>
	internal class AliveModulesSharedVariable : SharedVariable
	{
		#region Constructors

		/// <summary>
		/// Initializes a new Instance of BusyModulesSharedVariable
		/// </summary>
		/// <param name="owner">The IModule object which this SharedVariable object is bound to</param>
		public AliveModulesSharedVariable(ModuleBlackboard owner)
			: base(owner, "string", "alive", false, -1)
		{
			this.allowedWriters.Clear();
			this.allowedWriters.Add(owner.Name);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the data of the SharedVariable
		/// </summary>
		internal override string Data
		{
			get { return ReadStringData(); }
			set { }
		}

		#endregion

		#region Events
		#endregion

		#region Methods

		/// <summary>
		/// Reads the content of the SharedVariable
		/// </summary>
		/// <returns>the content of the SharedVariable</returns>
		public override string ReadStringData()
		{
			if (owner == null)
				return String.Empty;

			List<string> aliveModules = new List<string>(owner.Parent.Modules.Count);
			foreach (IModuleClient module in owner.Parent.Modules)
			{
				if (module.IsAlive)
					aliveModules.Add(module.Name);
			}
			return "\\\"" + String.Join(" ", aliveModules.ToArray()) + "\\\"";
		}

		/// <summary>
		/// Sends all subscription messages to the subscribers
		/// </summary>
		public void ReportSubscribers()
		{
			ReportSubscribers(this.Owner);
		}

		/// <summary>
		/// Overrides the base method WriteStringData to always return false
		/// </summary>
		/// <param name="writer">The IModule object which requested the write operation</param>
		/// <param name="dataType">The received type of the data to update the variable</param>
		/// <param name="arraySize">Specifies the size of the array if the variable was created as an array. If the variable is not an array must be -1</param>
		/// <param name="content">The data in hexadecimal string representation</param>
		/// <returns>false</returns>
		public override bool WriteStringData(IModuleClient writer, string dataType, int arraySize, string content)
		{
			return false;
		}

		#endregion
	}
}
