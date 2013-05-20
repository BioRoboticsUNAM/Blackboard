using System;
using System.ComponentModel;
using System.Collections.Generic;

/*
namespace Blk.Engine
{
	public class StringArrayConverter : CollectionConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if(destinationType == typeof(string))
			{
				if (value is IEnumerable<string>)
				{
					IEnumerable<string> sValue = value as IEnumerable<string>;
				}
				else if (value == null)
					return "(null)";
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}

*/