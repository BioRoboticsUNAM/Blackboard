using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Blk.Api;

namespace Blk.Engine
{
	/// <summary>
	/// Implements a command between Blackboard and Modules
	/// </summary>
	public abstract class CommandBase : ITextMessage, IComparable<CommandBase>
	{
		#region Variables

		/// <summary>
		/// Command name
		/// </summary>
		protected string command;
		
		/// <summary>
		/// The destination Module of this command
		/// </summary>
		protected IModuleClient destination;

		/// <summary>
		/// Command Id
		/// </summary>
		protected int id = -1;
		
		/// <summary>
		/// The string from where this command was built
		/// </summary>
		protected string originalString;
		
		/// <summary>
		/// Command parameters
		/// </summary>
		protected string parameters;

		/// <summary>
		/// Prototype asociated to this message
		/// </summary>
		protected IPrototype prototype;
		
		/// <summary>
		/// The source Module of this command
		/// </summary>
		protected IModuleClient source;

		/// <summary>
		/// Stores the last sent attempt result for this CommandBase instance
		/// </summary>
		protected SentStatus sentStatus;

		/// <summary>
		/// Stores the number of send attempts for this CommandBase Instance
		/// </summary>
		protected int sendAttempts;

		#endregion

		#region Constructors
		/// <summary>
		/// Default constructor for further initialization
		/// </summary>
		protected CommandBase()
		{
			this.sentStatus = SentStatus.NotSentYet;
			this.sendAttempts = 0;
		}

		/// <summary>
		/// Initiates a new instance of CommandBase
		/// </summary>
		/// <param name="source">Module which generated the command</param>
		/// <param name="destination">Module to which this command will be sent</param>
		/// <param name="command">Command sent</param>
		/// <param name="param">Param sent</param>
		public CommandBase(ModuleClient source, ModuleClient destination, string command, string param) : this(source, destination, command, param, -1) { }

		/// <summary>
		/// Initiates a new instance of CommandBase
		/// </summary>
		/// <param name="source">Module which generated the command</param>
		/// <param name="destination">Module to which this command will be sent</param>
		/// <param name="command">Command sent</param>
		/// <param name="param">Param sent</param>
		/// <param name="id">id of the command</param>
		public CommandBase(ModuleClient source, ModuleClient destination, string command, string param, int id)
		{
			this.source = source;
			this.destination = destination;
			this.parameters = param;
			this.id = id;
			this.sentStatus = SentStatus.NotSentYet;
			this.sendAttempts = 0;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the command name
		/// </summary>
		public virtual string Command
		{
			get { return command; }
		}

		/// <summary>
		/// Gets the destination Module of this command
		/// </summary>
		public virtual IModuleClient Destination
		{
			get { return destination; }
		}

		/// <summary>
		/// Gets the id of the message
		/// </summary>
		public virtual int Id
		{
			get { return id; }
			internal set
			{
				if (value < 0) id = -1;
				else id = value;
			}
		}

		/// <summary>
		/// Gets a value indicating id the Command contains params
		/// </summary>
		public virtual bool HasParams
		{
			get { return ((parameters != null) && (parameters.Length > 0)); }
		}

		/// <summary>
		/// Gets the string from where this command was build
		/// </summary>
		public virtual string OriginalString
		{
			get { return originalString; }
		}

		/// <summary>
		/// Gets the parameters of this message
		/// </summary>
		public virtual string Parameters
		{
			get { return parameters; }
			internal set
			{
				if (this.prototype.ParamsRequired && ((value == null) || (value.Length < 1)))
					throw new ArgumentException("This command requires parameters", "value");
				this.parameters = value;
			}
		}

		/// <summary>
		/// Prototype asociated to this command
		/// </summary>
		public virtual IPrototype Prototype
		{
			get { return prototype; }
		}

		/// <summary>
		/// Gets the number of send attempts for this CommandBase Instance
		/// </summary>
		public virtual int SendAttempts
		{
			get { return sendAttempts; }
			internal set
			{
				if (sendAttempts > value) throw new ArgumentOutOfRangeException();
				sendAttempts = value;
			}
		}

		/// <summary>
		/// Stores the last sent attempt result for this CommandBase instance
		/// </summary>
		public virtual SentStatus SentStatus
		{
			get { return sentStatus; }
			internal set
			{
				if ((sentStatus != SentStatus.NotSentYet) && (value == SentStatus.NotSentYet)) throw new ArgumentException("The command has been sent");
				sentStatus = value;
			}
		}
		
		/// <summary>
		/// Gets the source Module of this command
		/// </summary>
		public virtual IModuleClient Source
		{
			get { return source; }
		}

		/// <summary>
		/// Gets a string which can be sent to a module
		/// </summary>
		public abstract string StringToSend
		{
			get;
		}
		
		#endregion

		#region Events
		#endregion

		#region Methods

		/// <summary>
		/// Sends itself trough its Destination IModule
		/// </summary>
		/// <returns>true if the command has been sent, false otherwise</returns>
		public abstract bool Send();

		/// <summary>
		/// Returns the hash code for this instance
		/// </summary>
		/// <returns>A 32-bit signed integer hash code</returns>
		public override int GetHashCode()
		{
			return StringToSend.GetHashCode();
		}

		/// <summary>
		/// Returns a String that represents the current Object. (Inherited from Object.)
		/// </summary>
		/// <returns>String that represents the current Object</returns>
		public override string ToString()
		{
			//return (source.Name == null ? "Unknown" : source.Name) + "" + (destination.Name == null ? "Unknown" : destination.Name) + " " + commandString;
			return StringToSend;
		}

		/// <summary>
		/// Replaces the environment variables in the input string
		/// </summary>
		/// <param name="input">String in which to replace the environment parameters</param>
		protected virtual void ReplaceEnvironmentVars(ref string input)
		{
			if ((destination == null) || (destination.Parent == null))
				return;
			//if (destination.Parent.VirtualModule != null)
			//{
			//    destination.Parent.VirtualModule.ReplaceEnvironmentVars(ref input);
			//    return;
			//}
			if (input.IndexOf('%') != -1)
			{
				input = input.Replace("%TestTimeLeft%", destination.Parent.TestTimeLeft.TotalSeconds.ToString("0"));
				input = input.Replace("%TestTimeOut%", destination.Parent.TestTimeOut.TotalSeconds.ToString("0"));
				input = input.Replace("%AutoStopTime%", destination.Parent.AutoStopTime.TotalSeconds.ToString("0"));
				input = input.Replace("%AutoStopTimeLeft%", destination.Parent.AutoStopTimeLeft.TotalSeconds.ToString("0"));
			}
		}

		#region IComparable<CommandBase> Members

		/// <summary>
		/// Compares this instance to a specified CommandBase object and returns an indication of their relative values
		/// </summary>
		/// <param name="other">A CommandBase object to compare</param>
		/// <returns>A signed number indicating the relative values of this instance and the other parameter</returns>
		public virtual int CompareTo(CommandBase other)
		{
			return StringToSend.GetHashCode() - other.StringToSend.GetHashCode();
		}

		#endregion

		#region IComparable<ITextMessage> Members

		/// <summary>
		/// Compares this instance to a specified CommandBase object and returns an indication of their relative values
		/// </summary>
		/// <param name="other">A CommandBase object to compare</param>
		/// <returns>A signed number indicating the relative values of this instance and the other parameter</returns>
		public virtual int CompareTo(ITextMessage other)
		{
			return StringToSend.GetHashCode() - other.StringToSend.GetHashCode();
		}

		#endregion

		#endregion

		#region Operators
		/// <summary>
		/// Implicitly converts the Command to a string which can be sent to a module
		/// </summary>
		/// <param name="m">Command to be converted</param>
		/// <returns>A string well formated</returns>
		public static explicit operator string(CommandBase m)
		{
			return m.StringToSend;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Extracts module name, command name, command params, result and @id elements from the input string
		/// </summary>
		/// <param name="input">The input string</param>
		/// <param name="module1">First module name found if any, null otherwise</param>
		/// <param name="module2">Second module name found if any, null otherwise</param>
		/// <param name="commandName">Command name found if any, null otherwise</param>
		/// <param name="commandParams">Parameters found if any, null otherwise</param>
		/// <param name="result">Result found if any, -1 otherwise</param>
		/// <param name="id">Id found if any, -1 otherwise</param>
		public static void XtractCommandElements(string input, out string module1, out string module2, out string commandName, out string commandParams, out int result, out int id)
		{
			int cc = 0;

			module1 = null;
			module2 = null;
			commandName = null;
			commandParams = null;
			result = -1;
			id = -1;

			while ((cc < input.Length) && (input[cc] != 0) && (input[cc] < 128) && !Parser.IsAlpha(input[cc]))
				++cc;
			Parser.XtractModuleName(input, ref cc, out module1);
			Parser.SkipSpaces(input, ref cc);
			Parser.XtractModuleName(input, ref cc, out module2);
			Parser.SkipSpaces(input, ref cc);
			if (!Parser.XtractCommandName(input, ref cc, out commandName))
				return;
			Parser.SkipSpaces(input, ref cc);
			Parser.XtractCommandParams(input, ref cc, out commandParams);
			Parser.SkipSpaces(input, ref cc);
			Parser.XtractResult(input, ref cc, out result);
			Parser.SkipSpaces(input, ref cc);
			Parser.XtractId(input, ref cc, out id);
		}

		/// <summary>
		/// Extracts all possible command/response elements from the input string
		/// </summary>
		/// <param name="input">The input string</param>
		/// <param name="module">Module name found if any, null otherwise</param>
		/// <param name="commandName">Command name found if any, null otherwise</param>
		/// <param name="commandParams">Parameters found if any, null otherwise</param>
		/// <param name="result">Result found if any, -1 otherwise</param>
		/// <param name="id">Id found if any, -1 otherwise</param>
		public static void XtractCommandElements(string input, out string module, out string commandName, out string commandParams, out int result, out int id)
		{
			int cc = 0;

			module = null;
			commandName = null;
			commandParams = null;
			result = -1;
			id = -1;

			while ((cc < input.Length) && (input[cc] != 0) && (input[cc] < 128) && !Parser.IsAlpha(input[cc]))
				++cc;
			Parser.XtractModuleName(input, ref cc, out module);
			Parser.SkipSpaces(input, ref cc);
			if (!Parser.XtractCommandName(input, ref cc, out commandName))
				return;
			Parser.SkipSpaces(input, ref cc);
			Parser.XtractCommandParams(input, ref cc, out commandParams);
			Parser.SkipSpaces(input, ref cc);
			Parser.XtractResult(input, ref cc, out result);
			Parser.SkipSpaces(input, ref cc);
			Parser.XtractId(input, ref cc, out id);
		}

		/// <summary>
		/// Extracts command name, command params, result and @id elements from the input string
		/// </summary>
		/// <param name="input">The input string</param>
		/// <param name="commandName">Command name found if any, null otherwise</param>
		/// <param name="commandParams">Parameters found if any, null otherwise</param>
		/// <param name="result">Result found if any, -1 otherwise</param>
		/// <param name="id">Id found if any, -1 otherwise</param>
		public static void XtractCommandElements(string input, out string commandName, out string commandParams, out int result, out int id)
		{
			int cc = 0;

			commandName = null;
			commandParams = null;
			result = -1;
			id = -1;

			while ((cc < input.Length) && (input[cc] != 0) && (input[cc] < 128) && !Parser.IsAlpha(input[cc]))
				++cc;
			if (!Parser.XtractCommandName(input, ref cc, out commandName))
				return;
			Parser.SkipSpaces(input, ref cc);
			Parser.XtractCommandParams(input, ref cc, out commandParams);
			Parser.SkipSpaces(input, ref cc);
			Parser.XtractResult(input, ref cc, out result);
			Parser.SkipSpaces(input, ref cc);
			Parser.XtractId(input, ref cc, out id);
		}

		/*
		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="blackboard">A blackboard which contains all information about modules</param>
		/// <param name="result">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string s, Blackboard blackboard, out CommandBase result);

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="source">The module which sent the command</param>
		/// <param name="blackboard">A blackboard which contains all information about modules</param>
		/// <param name="result">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string s, Module source, Blackboard blackboard, out CommandBase result);

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="source">The module which sent the command</param>
		/// <param name="destination">The destination module for the command</param>
		/// <param name="blackboard">A blackboard which contains all information about modules</param>
		/// <param name="result">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string s, Module source, Module destination, Blackboard blackboard, out CommandBase result);

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="blackboard">A blackboard which contains all information about modules</param>
		/// <param name="result">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>A command object that represents the command contained in s</returns>
		public static CommandBase TryParse(string s, Blackboard blackboard);

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="source">The module which sent the command</param>
		/// <param name="blackboard">A blackboard which contains all information about modules</param>
		/// <param name="result">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>A command object that represents the command contained in s</returns>
		public static CommandBase TryParse(string s, Module source, Blackboard blackboard);

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="source">The module which sent the command</param>
		/// <param name="destination">The destination module for the command</param>
		/// <param name="blackboard">A blackboard which contains all information about modules</param>
		/// <param name="result">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>A command object that represents the command contained in s</returns>
		public static CommandBase Parse(string s, Module source, Module destination, Blackboard blackboard);

		*/
		#endregion
	}
}