using System;
using System.Xml;
using System.Xml.Serialization;


namespace Blk.Gui
{
	public class InjectedString : IComparable<InjectedString>, IEquatable<InjectedString>
	{
		#region Variables

		private string moduleName;
		private string value;

		#endregion

		#region Constructors
		#endregion

		#region Properties

		[XmlAttribute("module")]
		public string ModuleName
		{
			get { return this.moduleName; }
			set { this.moduleName = value; }
		}

		[XmlText]
		public string Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		#endregion

		public override string ToString()
		{
			return this.ModuleName + " " + this.Value;
		}

		#region IComparable<InjectedString> Members

		public int CompareTo(InjectedString other)
		{
			return this.ToString().CompareTo(other == null ? null : other.ToString());
		}

		#endregion

		#region IEquatable<InjectedString> Members

		public bool Equals(InjectedString other)
		{
			return other == null ? false :
				(String.Compare(this.ModuleName, other.ModuleName) == 0) &&
				(String.Compare(this.Value, other.Value) == 0);
		}

		#endregion
	}
}
