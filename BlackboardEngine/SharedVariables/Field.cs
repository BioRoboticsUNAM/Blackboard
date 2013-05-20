using System;
using System.Collections.Generic;
using System.Text;

namespace Blk.Engine.SharedVariables
{
	/// <summary>
	/// Represents a field of a structure
	/// </summary>
	public class Field
	{
		#region Variables

		/// <summary>
		/// The name of the field
		/// </summary>
		private string name;

		/// <summary>
		/// The type of the field
		/// </summary>
		private string type;

		/// <summary>
		/// Indicates if the field is an array
		/// </summary>
		private bool isArray;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Field
		/// </summary>
		/// <param name="type">The type of the field</param>
		/// <param name="name">The name of the field</param>
		public Field(string type, string name)
		{
			this.type = type;
			this.name = name;
			this.isArray = type.EndsWith("[]");
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value ndicating if the field is an array
		/// </summary>
		public bool IsArray
		{
			get { return isArray; }
		}

		/// <summary>
		/// Gets the name of the field
		/// </summary>
		public string Name
		{
			get { return this.name; }
		}

		/// <summary>
		/// Gets the type of the field
		/// </summary>
		public string Type
		{
			get { return this.type; }
		}

		#endregion

		#region Events
		#endregion

		#region Methods
		#endregion
	}
}
