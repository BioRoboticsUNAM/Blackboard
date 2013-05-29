using System;
using System.Collections.Generic;
using Blk.Api;
using Blk.Api.SharedVariables;

namespace Blk.Engine.SharedVariables
{
	/// <summary>
	/// Implements a shared variable for the list of registeres variables
	/// </summary>
	internal class VariableListSharedVariable : SharedVariable
	{
		#region Constructors

		/// <summary>
		/// Initializes a new Instance of BusyModulesSharedVariable
		/// </summary>
		/// <param name="owner">The IModule object which this SharedVariable object is bound to</param>
		public VariableListSharedVariable(ModuleBlackboard owner)
			: base(owner, "string", "vars", false, -1)
		{
			this.allowedWriters.Clear();
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
			if (this.owner == null)
				return String.Empty;

			SharedVariableCollection collection = (SharedVariableCollection)owner.Parent.VirtualModule.SharedVariables;
			List<string> variables = new List<string>(collection.Count);
			foreach (SharedVariable sv in collection)
			{
				variables.Add(sv.Type + " " + sv.Name);
			}
			return "\\\"" + String.Join(" ", variables.ToArray()) + "\\\"";
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