using System;
using System.ComponentModel;

namespace Blk.Engine.SharedVariables
{
	internal class AllowedWriterPropertyDescriptor : PropertyDescriptor
	{
		private string moduleName;

		public AllowedWriterPropertyDescriptor(int index, string moduleName)
			: this(index, moduleName, null)
		{
		}

		public AllowedWriterPropertyDescriptor(int index, string moduleName, Attribute[] attrs)
			: base(index.ToString(), attrs)
		{
			this.moduleName = moduleName;
		}

		public override bool CanResetValue(object component)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override Type ComponentType
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override object GetValue(object component)
		{
			return moduleName;
		}

		public override bool IsReadOnly
		{
			get { return true; }
		}

		public override Type PropertyType
		{
			get { return typeof(string); }
		}

		public override void ResetValue(object component)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void SetValue(object component, object value)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
