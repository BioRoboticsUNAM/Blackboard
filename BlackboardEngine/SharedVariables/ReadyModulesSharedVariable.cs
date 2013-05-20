using System;
using System.Collections.Generic;
using Blk.Api;

namespace Blk.Engine.SharedVariables
{
	/// <summary>
	/// Implements a shared variable for the list of ready modules
	/// </summary>
	internal class ReadyModulesSharedVariable : SharedVariable
	{
		#region Constructors

		/// <summary>
		/// Initializes a new Instance of ReadySharedVariable
		/// </summary>
		/// <param name="owner">The IModule object which this SharedVariable object is bound to</param>
		public ReadyModulesSharedVariable(ModuleBlackboard owner)
			:base(owner, "string", "ready", false, -1)
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
			get	{ return ReadStringData(); }
			set {}
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

			List<string> readyModules = new List<string>(owner.Parent.Modules.Count);
			foreach (IModule module in owner.Parent.Modules)
			{
				if(module.Ready) 
					readyModules.Add(module.Name);
			}
			return "\\\"" + String.Join(" ", readyModules.ToArray()) + "\\\"";
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
		public override bool WriteStringData(IModule writer, string dataType, int arraySize, string content)
		{
			return false;
		}
		#endregion
	}
}
