using System;
using System.ComponentModel;

namespace Blk.Engine.SharedVariables
{
	internal class WritersConverter : ExpandableObjectConverter
	{

		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			return base.CreateInstance(context, propertyValues);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			//return base.GetProperties(context, value, attributes);
			SharedVariable sv;
			//PropertyDescriptorCollection pdc;
			AllowedWriterPropertyDescriptor[] descriptors;

			if ((context == null) || !(context.Instance is SharedVariable))
				return null;

			sv = (SharedVariable)context.Instance;
			if (sv.AllowedWriters.Count < 1)
				return null;
			descriptors = new AllowedWriterPropertyDescriptor[sv.AllowedWriters.Count];

			for (int i = 0; i < sv.AllowedWriters.Count; ++i)
			{
				descriptors[i] = new AllowedWriterPropertyDescriptor(i, sv.AllowedWriters[i], attributes);
			}
			return new PropertyDescriptorCollection(descriptors);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type sourceType)
		{
			return (sourceType == typeof(string));
			//return false;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			bool result = base.CanConvertFrom(context, sourceType);
			return (sourceType == typeof(string));
			//return false;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			SharedVariable sv;
			if ((destinationType != typeof(string)) || (context == null) || !(context.Instance is SharedVariable))
				return null;
			sv = (SharedVariable)context.Instance;
			if (sv.AllowedWriters.Count == 0)
				return "(All)";
			else if (sv.AllowedWriters.Count == 1)
				return (sv.AllowedWriters[0] == "*") ? "(All)" : sv.AllowedWriters[0];
			return sv.AllowedWriters.Count.ToString() + " writers";
		}

		//public override 
	}
}
