using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Blackboard
{
	/// <summary>
	/// Implements a message between Blackboard and Modules
	/// </summary>
	public class Message
	{
		#region Variables

		/// <summary>
		/// The source Module of this message
		/// </summary>
		protected Module source;
		/// <summary>
		/// The destination Module of this message
		/// </summary>
		protected Module destination;
		/// <summary>
		/// The string from where this message was built
		/// </summary>
		protected string originalString;
		/// <summary>
		/// The string to send
		/// </summary>
		private string messageString;

		#endregion

		#region Constructores
		/*
		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the message</param>
		/// <param name="command">Command sent</param>
		/// <param name="param">Param sent</param>
		public Message(Module source, string command, string param) : this(source, null, command, param, -1) { }

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the message</param>
		/// <param name="command">Command sent</param>
		/// <param name="param">Param sent</param>
		/// <param name="id">id of the message</param>
		public Message(Module source, string command, string param, int id) : this(source, null, command, param, id) { }
		*/

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the message</param>
		/// <param name="destination">Module to which this message will be sent</param>
		/// <param name="command">Command sent</param>
		/// <param name="param">Param sent</param>
		public Message(Module source, Module destination, string command, string param) : this(source, destination, command, param, -1) { }

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the message</param>
		/// <param name="destination">Module to which this message will be sent</param>
		/// <param name="command">Command sent</param>
		/// <param name="param">Param sent</param>
		/// <param name="id">id of the message</param>
		public Message(Module source, Module destination, string command, string param, int id)
		{
			this.source = source;
			this.destination = destination;
			this.parameters = param;
			this.id = id;
		}

		#endregion

		#region Propiedades

		/// <summary>
		/// Gets the source Module of this message
		/// </summary>
		public Module Source
		{
			get { return source; }
		}

		/// <summary>
		/// Gets the destination Module of this message
		/// </summary>
		public Module Destination
		{
			get { return destination; }
		}

		/// <summary>
		/// Gets a which can be sent to a module
		/// </summary>
		public string MessageString
		{
			get { return messageString; }
		}

		/// <summary>
		/// Gets the string from where this command was build
		/// </summary>
		public string OriginalString
		{
			get { return originalString; }
		}
		#endregion

		#region Eventos
		#endregion

		#region Metodos

		/// <summary>
		/// Returns a String that represents the current Object. (Inherited from Object.)
		/// </summary>
		/// <returns>String that represents the current Object</returns>
		public override string ToString()
		{
			return (source.Name == null ? "Unknown" : source.Name) + "" + (destination.Name == null ? "Unknown" : destination.Name) + " " + messageString;
		}

		#endregion

		#region Operators
		/// <summary>
		/// Implicitly converts the Message to a string which can be sent to a module
		/// </summary>
		/// <param name="m">Message to be converted</param>
		/// <returns>A string well formated</returns>
		public static implicit operator string(Message m)
		{
			return m.messageString;
		}

		#endregion

		#region Metodos estaticos

		/// <summary>
		/// Converts the string representation of a message to a Message object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s"></param>
		/// <param name="result">When this method returns, contains the Message equivalent to the message
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>A message object that represents the message contained in s</returns>
		public static bool TryParse(string s, out Message result)
		{
			try
			{
				result = Parse(s);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		/// <summary>
		/// Converts the string representation of a message to a Message object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the message to convert</param>
		/// <param name="b">A blackboard which contains all information about modules</param>
		/// <param name="result">When this method returns, contains the Message equivalent to the message
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>A message object that represents the message contained in s</returns>
		public static bool TryParse(string s, Blackboard b, out Message result)
		{
			try
			{
				result = Parse(s, b);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		/// <summary>
		/// Converts the string representation of a message to a Message object.
		/// </summary>
		/// <param name="s">A string containing the message to convert</param>
		/// <param name="b">A blackboard which contains all information about modules</param>
		/// <returns>A message object that represents the message contained in s</returns>
		public static Message Parse(string s, Blackboard b)
		{
			return null;
		}

		/// <summary>
		/// Converts the string representation of a message to a Message object
		/// </summary>
		/// <param name="s">A string containing the message to convert</param>
		/// <returns>A message object that represents the message contained in s</returns>
		public static Message Parse(string s)
		{
			Regex rx;
			Match m;
			Message msg;

			rx = new Regex(@"(?<cmd>[A-Za-z_]+)\s+""(?<param>[^""]*)""(\s+(?<result>[01]))?(\s+@(?<result>\d+))?");
			m = rx.Match(s);
			//msg = new Message(
			return null;
		}

		#endregion

	}
}

/*

		#region Variables
		
		/// <summary>
		/// The source Module of this message
		/// </summary>
		protected Module source;
		/// <summary>
		/// The destination Module of this message
		/// </summary>
		protected Module destination;
		/// <summary>
		/// The string from where this message was built
		/// </summary>
		protected string originalString;
		/// <summary>
		/// Command name
		/// </summary>
		protected string command;
		/// <summary>
		/// Command parameters
		/// </summary>
		protected string parameters;
		/// <summary>
		/// Prototype asociated to this message
		/// </summary>
		protected MessagePrototype prototype;
		/// <summary>
		/// Command Id
		/// </summary>
		protected int id = -1;

		#endregion

		#region Constructores

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the message</param>
		/// <param name="command">Command sent</param>
		/// <param name="param">Param sent</param>
		public Message(Module source, string command, string param) : this(source, null, command, param, -1) { }

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the message</param>
		/// <param name="command">Command sent</param>
		/// <param name="param">Param sent</param>
		/// <param name="id">id of the message</param>
		public Message(Module source, string command, string param, int id) : this(source, null, command, param, id) { }

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the message</param>
		/// <param name="destination">Module to which this message will be sent</param>
		/// <param name="command">Command sent</param>
		/// <param name="param">Param sent</param>
		public Message(Module source, Module destination, string command, string param) : this(source, destination, command, param, -1) { }

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the message</param>
		/// <param name="destination">Module to which this message will be sent</param>
		/// <param name="command">Command sent</param>
		/// <param name="param">Param sent</param>
		/// <param name="id">id of the message</param>
		public Message(Module source, Module destination, string command, string param, int id)
		{
			this.source = source;
			this.destination = destination;
			this.parameters = param;
			this.id = id;
		}

		#endregion

		#region Propiedades

		/// <summary>
		/// Gets the source Module of this message
		/// </summary>
		public Module Source
		{
			get { return source; }
		}

		/// <summary>
		/// Gets the destination Module of this message
		/// </summary>
		public Module Destination
		{
			get { return destination; }
		}

		/// <summary>
		/// Gets the command name
		/// </summary>
		public string Command
		{
			get { return command; }
		}

		/// <summary>
		/// Gets the parameters of this message
		/// </summary>
		public string Parameters
		{
			get { return parameters; }
		}

		/// <summary>
		/// Gets the id of the message
		/// </summary>
		public int Id
		{
			get { return id; }
		}

		/// <summary>
		/// Gets the string from where this command was build
		/// </summary>
		public string OriginalString
		{
			get { return originalString; }
		}
		#endregion

		#region Eventos
		#endregion

		#region Metodos

		/// <summary>
		/// Returns a String that represents the current Object. (Inherited from Object.)
		/// </summary>
		/// <returns>String that represents the current Object</returns>
		public override string ToString()
		{
			return (source.Name == null ? "Unknown" : source.Name) + "" + (destination.Name == null ? "Unknown" : destination.Name) + " " + command + " " + parameters + (id >= 0 ? " @" + id : "");
		}

		#endregion

		#region Operators
		/// <summary>
		/// Implicitly converts the Message to a string which can be sent to a module
		/// </summary>
		/// <param name="m">Message to be converted</param>
		/// <returns>A string well formated</returns>
		public static implicit operator string(Message m)
		{
			string s = m.command;
			if (m.parameters.Length > 0) s += " \"" + m.parameters + "\"";
			if (m.id >= 0) s += " @" + m.id.ToString();
			return s;
		}

		#endregion

		#region Metodos estaticos

		/// <summary>
		/// Converts the string representation of a message to a Message object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s"></param>
		/// <param name="result">When this method returns, contains the Message equivalent to the message
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>A message object that represents the message contained in s</returns>
		public static bool TryParse(string s, out Message result)
		{
			try
			{
				result = Parse(s);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		/// <summary>
		/// Converts the string representation of a message to a Message object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the message to convert</param>
		/// <param name="b">A blackboard which contains all information about modules</param>
		/// <param name="result">When this method returns, contains the Message equivalent to the message
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>A message object that represents the message contained in s</returns>
		public static bool TryParse(string s, Blackboard b, out Message result)
		{
			try
			{
				result = Parse(s, b);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		/// <summary>
		/// Converts the string representation of a message to a Message object.
		/// </summary>
		/// <param name="s">A string containing the message to convert</param>
		/// <param name="b">A blackboard which contains all information about modules</param>
		/// <returns>A message object that represents the message contained in s</returns>
		public static Message Parse(string s, Blackboard b)
		{
			return null;
		}

		/// <summary>
		/// Converts the string representation of a message to a Message object
		/// </summary>
		/// <param name="s">A string containing the message to convert</param>
		/// <returns>A message object that represents the message contained in s</returns>
		public static Message Parse(string s)
		{
			Regex rx;
			Match m;
			Message msg;

			rx = new Regex(@"(?<cmd>[A-Za-z_]+)\s+""(?<param>[^""]*)""(\s+(?<result>[01]))?(\s+@(?<result>\d+))?");
			m = rx.Match(s);
			//msg = new Message(
			return null;
		}

		#endregion
*/