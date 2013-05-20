using System;
using System.ComponentModel;
using System.Net;
using Blk.Engine;

namespace System.ComponentModel
{
	public class IPAddressConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(String))
				return true;

			//else if(sourceType == typeof(Byte[]))
			//	return true;
			//else if (sourceType == typeof(Int64))
			//	return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value == null)
				return base.ConvertFrom(context, culture, value);

			if (value.GetType() == typeof(String))
			{
				string[] sip = ((string)value).Split(new char[]{';', ',', ' '}, StringSplitOptions.RemoveEmptyEntries);
				IPAddress[] addresses = new IPAddress[sip.Length];
				IPAddress ip;
				for (int i = 0; i < sip.Length; ++i)
				{
					if (!IPAddress.TryParse(sip[i], out ip) || (ip == IPAddress.IPv6None) || (ip == IPAddress.Broadcast))
						throw new ArgumentException("Invalid IP address: " + sip[i]);
					addresses[i] = ip;
				}
				return addresses;
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			IPAddress[] collection = value as IPAddress[];
			if (collection == null)
				return base.ConvertFrom(context, culture, value);

			if (destinationType == typeof(String))
			{
				string[] sCollection = new string[collection.Length];
				for (int i = 0; i < collection.Length; ++i)
					sCollection[i] = collection[i].ToString();
				return String.Join("; ", sCollection);
			}
			
			//else if (destinationType == typeof(Byte[]))
			//    return collection.GetAddressBytes();
			//else if (destinationType == typeof(Int64))
			//    return BitConverter.ToInt64(collection.GetAddressBytes(), 0);

			return base.ConvertFrom(context, culture, value);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (value == null)
				return false;

			if (value.GetType() == typeof(String))
			{
				IPAddress ip;
				return IPAddress.TryParse((string)value, out ip);
			}
			else if (value.GetType() == typeof(Byte[]))
				return true;
			else if (value.GetType() == typeof(Int64))
				return true;
			return base.IsValid(context, value);
		}
	}
}
