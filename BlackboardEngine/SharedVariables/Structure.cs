using System;
using System.Collections.Generic;
using System.Text;

namespace Blk.Engine.SharedVariables
{
	/// <summary>
	/// Represents a structure
	/// </summary>
	public partial class Structure
	{
		#region Variables

		/// <summary>
		/// The name of the field
		/// </summary>
		private string name;
		/// <summary>
		/// The fields of the structure
		/// </summary>
		private FieldCollection fields;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Structure
		/// </summary>
		/// <param name="name">The name of the structure</param>
		public Structure(string name)
		{
			this.name = name;
			fields = new FieldCollection(this);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the name of the field
		/// </summary>
		public string Name
		{
			get { return this.name; }
		}

		/// <summary>
		/// Gets the collection of fields of the structure
		/// </summary>
		public FieldCollection Fields
		{
			get { return this.fields; }
		}

		#endregion

		#region Events
		#endregion

		#region Methods
		#endregion
	}
}
