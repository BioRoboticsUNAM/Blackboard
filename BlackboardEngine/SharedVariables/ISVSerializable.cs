using System;
using System.Collections.Generic;
using System.Text;

namespace Blk.Engine.SharedVariables
{
	/// <summary>
	/// Provides methods for data serialization/deserialization
	/// </summary>
	public interface ISVSerializable
	{
		/// <summary>
		/// Deserializes an object from an array of bytes
		/// </summary>
		/// <param name="input">Array of bytes containing the serialized object</param>
		/// <returns>true if object was deserialized successfully, false otherwise</returns>
		bool DeserializeFromByteArray(byte[] input);

		/// <summary>
		/// Deserializes an object from a string.
		/// The string contains the hex string representation of each byte of the object
		/// </summary>
		/// <param name="s">string containing the serialized object</param>
		/// <returns>true if object was deserialized successfully, false otherwise</returns>
		bool DeserializeFromHexString(string s);

		/// <summary>
		/// Deserializes an object from a string.
		/// The string contains the string representation of each object field separated by spaces
		/// </summary>
		/// <param name="s">string containing the serialized object</param>
		/// <returns>true if object was deserialized successfully, false otherwise</returns>
		bool DeserializeFromRawText(string s);

		/// <summary>
		/// Deserializes an object from a Xml string
		/// </summary>
		/// <param name="s">Xml string containing the serialized object</param>
		/// <returns>true if object was deserialized successfully, false otherwise</returns>
		bool DeserializeFromXml(string s);

		/// <summary>
		/// Serializes the object to an array of bytes
		/// </summary>
		/// <returns>Byte array that contains the IBlackboardSerializable object serialized</returns>
		byte[] SerializeToByteArray();

		/// <summary>
		/// Serializes the object to a string.
		/// The string contains the hex string representation of each byte of the object
		/// </summary>
		/// <returns>string that contains the IBlackboardSerializable object serialized</returns>
		string SerializeToHexString();

		/// <summary>
		/// Serializes the object to a string.
		/// The string contains the string representation of each object field separated by spaces
		/// </summary>
		/// <returns>string that contains the IBlackboardSerializable object serialized</returns>
		string SerializeToRawText();

		/// <summary>
		/// Serializes the object to a Xml string
		/// </summary>
		/// <returns>string that contains the IBlackboardSerializable object serialized</returns>
		string SerializeToXml();
	}
}
