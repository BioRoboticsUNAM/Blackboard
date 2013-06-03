//#define RegexCommandParse

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Blk.Api;

namespace Blk.Engine
{
	/// <summary>
	/// Represents a command
	/// </summary>
	public class Command : CommandBase, Blk.Api.ITextCommand
	{
		#region Variables

		/// <summary>
		/// The time when the command was sent.
		/// Initially contains the time when the command was created
		/// </summary>
		protected DateTime sentTime;
		/// <summary>
		/// Response asociated to this Command object
		/// </summary>
		protected Response response;
		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor for further initialization
		/// </summary>
		protected Command()
			: base()
		{
		}

		/*

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the message</param>
		/// <param name="command">Command sent</param>
		/// <param name="param">Param sent</param>
		public Command(Module source, string command, string param) : this(source, null, command, param, -1) { }

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the command</param>
		/// <param name="command">Command name</param>
		/// <param name="param">Parameters of the command</param>
		/// <param name="id">id of the command</param>
		public Command(Module source, string command, string param, int id) : this(source, null, command, param, id) { }

		*/

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the message</param>
		/// <param name="destination">Module to which this message will be sent</param>
		/// <param name="command">Command sent</param>
		/// <param name="param">Param sent</param>
		public Command(IModuleClient source, IModuleClient destination, string command, string param) : this(source, destination, command, param, -1) { }

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the command</param>
		/// <param name="destination">Module to which this command will be sent</param>
		/// <param name="command">Command name</param>
		/// <param name="param">Parameters of the command</param>
		/// <param name="id">id of the command</param>
		public Command(IModuleClient source, IModuleClient destination, string command, string param, int id)
			: base()
		{
			this.source = source;
			this.destination = destination;
			this.command = command;
			this.parameters = param;
			this.id = id;

			if (source.Parent != destination.Parent)
				throw new Exception("Source and destination modules does not belong to the same blackboard");
			for (int i = 0; i < destination.Prototypes.Count; ++i)
			{
				if (destination.Prototypes[i].Command == command)
				{
					this.prototype = destination.Prototypes[i];
					break;
				}
			}
			// Check if command matchs a prototype
			if (this.prototype.ParamsRequired && ((param == null) || (param.Length < 1)))
				throw new Exception("Invalid string. The Command requires parameters");
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or Sets the time when the command was sent.
		/// Initially contains the time when the command was created
		/// </summary>
		public virtual DateTime SentTime
		{
			get { return sentTime; }
			set
			{
				if (value > DateTime.Now)
					sentTime = DateTime.Now;
				else
					sentTime = value;
			}
		}

		/// <summary>
		/// Gets a string which can be sent to a module
		/// </summary>
		public override string StringToSend
		{
			get
			{
				StringBuilder sb = new StringBuilder(1024);

				string p;
				if ((destination != null) && (source != null) && destination.RequirePrefix)
				{
					ModuleClient mcSource = source as ModuleClient;
					if ((mcSource != null) && (mcSource.Alias != mcSource.Name))
						sb.Append(mcSource.Alias);
					else
						sb.Append(source.Name);
					sb.Append(' ');
					//sb.Append(destination.Name);
					//sb.Append(' ');
				}
				sb.Append(command);
				//if (prototype.ParamsRequired && (parameters.Length > 0))
				if (prototype.ParamsRequired || this.HasParams)
				{
					p = parameters;
					ReplaceEnvironmentVars(ref p);
					sb.Append(" \"");
					sb.Append(p);
					sb.Append("\"");
				}
				if (id >= 0)
				{
					sb.Append(" @");
					sb.Append(id);
				}
				return sb.ToString();
			}
		}

		/// <summary>
		/// Gets the time elapsed since the command was created or sent
		/// </summary>
		public TimeSpan Elapsed
		{
			get { return DateTime.Now - sentTime; }
		}

		/// <summary>
		/// Gets the amount of time left to wait for a response
		/// </summary>
		public TimeSpan TimeLeft
		{
			get
			{
				if (prototype.Timeout <= 0) return TimeSpan.MaxValue;
				return (this.sentTime + TimeSpan.FromMilliseconds(prototype.Timeout)) - DateTime.Now;
			}
		}

		/// <summary>
		/// Gets the amount of milliseconds left to wait for a response
		/// </summary>
		public int MillisecondsLeft
		{
			get
			{
				if (prototype.Timeout <= 0) return int.MaxValue;
				return (int)TimeLeft.TotalMilliseconds;
			}
		}

		/// <summary>
		/// Gets or sets the response asociated to this Command object
		/// </summary>
		public Response Response
		{
			get { return response; }
			internal set
			{
				if (response == value) return;
				if (!IsMatch(value)) throw new Exception("Invalid response for command");
				response = value;
				if (response.CommandResponded == null) response.CommandResponded = this;
				if ((id != -1) && (response.Id == -1)) response.Id = id;
			}
		}

		/// <summary>
		/// Gets or sets the response asociated to this Command object
		/// </summary>
		ITextResponse ITextCommand.Response
		{
			get { return response; }
		}

		#endregion

		#region Events
		#endregion

		#region Methods

		/// <summary>
		/// Gets a value indicating if provided Response is a response for current command
		/// </summary>
		/// <param name="response">Response to check</param>
		/// <returns>true if provided Response is a response for command, false otherwise</returns>
		public bool IsMatch(ITextResponse response)
		{
			// This one is if the response destination is known
			if (response.Destination != null)
			{
				//bool matchId = ((this.Id != -1) && (response.Id != -1)) ? (this.Id == response.Id) : true;
				return (this.Source == response.Destination)
					&& (this.Destination == response.Source)
					&& (this.Command == response.Command)
					//&& matchId
					&& (this.SentTime <= response.ArrivalTime);
			}
			// This one is for search for an adequate command-response match
			return false;
		}

		/// <summary>
		/// Asignates the provided Response as Response for the current Command and this Command as CommandResponded for the provided response.
		/// After asignation the Response is sent to its Destination module which must match this Command Source module
		/// </summary>
		/// <param name="response">Response to asign</param>
		/// <returns>true if response was asigned and sent successfully, false otherwise</returns>
		public bool SendResponse(ITextResponse response)
		{
			if (response is Response)
				return SendResponse((Response)response);
			return false;
		}

		/// <summary>
		/// Asignates the provided Response as Response for the current Command and this Command as CommandResponded for the provided response.
		/// After asignation the Response is sent to its Destination module which must match this Command Source module
		/// </summary>
		/// <param name="response">Response to asign</param>
		/// <returns>true if response was asigned and sent successfully, false otherwise</returns>
		public bool SendResponse(Response response)
		{
			if (response.Destination == null) response.Destination = this.source;
			if (!IsMatch(response)) return false;
			if (response.CommandResponded == null) response.CommandResponded = this;
			this.Response = response;
			if ((response.CommandResponded != this) || (this.response != response)) return false;
			return response.Send();
		}

		/// <summary>
		/// Sends itself to its Destination module
		/// </summary>
		/// <returns>true if the command has been sent, false otherwise</returns>
		public override bool Send()
		{
			return Destination.Send(this);
		}

		/// <summary>
		/// Returns a String that represents the current Object. (Inherited from Object.)
		/// </summary>
		/// <returns>String that represents the current Object</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(255);

			if (source.Name == null) sb.Append("Unknown");
			else sb.Append(source.Name);

			sb.Append(' ');

			try
			{
				if ((destination != null) && (destination.Name == null)) sb.Append("Unknown");
				else sb.Append(destination.Name);
			}
			catch { }

			sb.Append(' ');

			sb.Append(command);
			if (prototype.ParamsRequired)
			{
				sb.Append(" \"");
				sb.Append(parameters);
				sb.Append("\"");
			}

			if (id >= 0)
			{
				sb.Append(" @");
				sb.Append(id);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Returns a String that represents the current Object. (Inherited from Object.)
		/// </summary>
		/// <param name="debug">Value indicating if debug information is shown</param>
		/// <returns>String that represents the current Object</returns>
		public string ToString(bool debug)
		{
			if (debug) return this.ToString();

			StringBuilder sb = new StringBuilder(255);

			if (source.Name == null) sb.Append("Unknown");
			else sb.Append(source.Name);

			sb.Append(' ');

			try
			{
				if ((destination != null) && (destination.Name == null)) sb.Append("Unknown");
				else sb.Append(destination.Name);
			}
			catch { }

			sb.Append(' ');

			sb.Append(command);
			if (prototype.ParamsRequired)
			{
				sb.Append(" \"");
				sb.Append(parameters);
				sb.Append("\"");
			}

			if (id >= 0)
			{
				sb.Append(" @");
				sb.Append(id);
			}

			sb.Append(" on ");
			sb.Append(this.SentTime.ToString("yyyy-MM-dd HH:mm:ss"));

			return sb.ToString();
		}

		#endregion

		#region Operators
		/// <summary>
		/// Implicitly converts the Message to a string which can be sent to a module
		/// </summary>
		/// <param name="c">Command to be converted</param>
		/// <returns>A string well formated</returns>
		public static implicit operator string(Command c)
		{
			//string s = c.command;
			//if (c.parameters.Length > 0) s += " \"" + c.parameters + "\"";
			//if (c.id >= 0) s += " @" + c.id.ToString();
			//return s;
			return c.StringToSend;
		}

		#endregion

		#region Static Variables

		private static Regex rxCommandFromBlackboard =
			new Regex(@"^(?<src>[\w\-]+)(\s+(?<dest>[\w\-]+))?\s+(?<cmd>[A-Za-z_]+)(\s+""(?<params>[^""]*)"")?(\s+@(?<id>\d+))?$", RegexOptions.Compiled);
		private static Regex rxCommandFromEndpoint =
			new Regex(@"^((?<src>[\w\-]+)(\s+(?<dest>[\w\-]+))?\s+)?(?<cmd>[A-Za-z_]+)(\s+""(?<params>[^""]*)"")?(\s+@(?<id>\d+))?$", RegexOptions.Compiled);
		private static Regex rxCommandFromSource =
			new Regex(@"((?<dest>[\w\-]+)\s+)?(?<cmd>[A-Za-z_]+)(\s+""(?<params>[^""]*)"")?(\s+@(?<id>\d+))?", RegexOptions.Compiled);
		private static Regex rxCommandFromSrcDest =
			new Regex(@"(?<cmd>[A-Za-z_]+)(\s+""(?<params>[^""]*)"")?(\s+@(?<id>\d+))?", RegexOptions.Compiled);

		#endregion

		#region Static Methods

		#region TryParse

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// </summary>
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="blackboard">A blackboard which contains all information about modules</param>
		/// <param name="command">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string s, Blackboard blackboard, out Command command)
		{
			Exception ex;
			return TryParse(s, blackboard, out command, out ex);
		}

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// </summary>
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="source">The module which sent the command</param>
		/// <param name="command">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string s, ModuleClient source, out Command command)
		{
			Exception ex;
			return TryParse(s, source, out command, out ex);
		}

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// </summary>
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="source">The module which sent the command</param>
		/// <param name="destination">The destination module for the command</param>
		/// <param name="command">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string s, ModuleClient source, ModuleClient destination, out Command command)
		{
			Exception ex;
			return TryParse(s, source, destination, out command, out ex);
		}

		#endregion

		#region TryParse (Code)

#if RegexCommandParse

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// </summary>
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="blackboard">A blackboard which contains all information about modules</param>
		/// <param name="cmd">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="ex">When this method returns, contains the Exception generated during the parse operation,
		/// if the conversion failed, or null if the conversion succeeded. The conversion fails if the s parameter
		/// is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		protected static bool TryParse(string s, Blackboard blackboard, out Command cmd, out Exception ex)
		{
			Regex rx;
			Match m;
			Prototype proto;
			//Module source;
			//Module destination;
			IModule source;
			IModule destination;
			string sCommand;
			string sParams;
			string sSrc;
			string sDest;
			string sId;
			int id;

			cmd = null;
			ex = null;

			// Regular Expresion Generation
			//rx = new Regex(@"((?<src>[\w\-]+)(\s+(?<dest>[\w\-]+))?\s+)?(?<cmd>[A-Za-z_]+)\s+(""(?<params>[^""]*)"")?\s+(@(?<id>\d+))?");
			rx = rxCommandFromBlackboard;
			m = rx.Match(s);
			// Check if input string matchs Regular Expression
			if (!m.Success)
			{
				ex = new ArgumentException("Invalid String", "s");
				return false;
			}
			// Extract Data
			sCommand = m.Result("${cmd}").ToLower();
			sParams = m.Result("${params}");
			sId = m.Result("${id}");
			sSrc = m.Result("${src}");
			sDest = m.Result("${dest}");
			if ((sId == null) || (sId.Length < 1)) id = -1;
			else id = Int32.Parse(sId);
			// Check if source module is specified
			if ((sSrc == null) || (sSrc.Length < 1) || !blackboard.Modules.Contains(sSrc))
			{
				ex = new Exception("No source module found for command provided");
				return false;
			}
			source = blackboard.Modules[sSrc];
			// Browse for destination module and prototype
			if (!blackboard.FindDestinationModule(sCommand, out destination, out proto))
			//throw new Exception("No destination module found for command provided");
			{
				// No destination module found. Must check if destination is availiable
				if ((sDest == null) || (sDest.Length < 1) || !blackboard.Modules.Contains(sDest))
				{
					ex = new Exception("No destination module found for command provided");
					return false;
				}
				destination = blackboard.Modules[sDest];
				proto = null;
			}
			// Check if command matchs a prototype
			if ((proto != null) && proto.ParamsRequired && ((sParams == null) || (sParams.Length < 1)))
			{
				ex = new Exception("Invalid string. The Command requires parameters");
				return false;
			}
			// Create the Command
			cmd = new Command(source, destination, sCommand, sParams, id);
			cmd.prototype = proto;
			cmd.sentTime = DateTime.Now;
			return true;
		}

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// </summary>
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="localEndpoint">The remote endpoint source of the string</param>
		/// <param name="destination">The module which received the command</param>
		/// <param name="cmd">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="ex">When this method returns, contains the Exception generated during the parse operation,
		/// if the conversion failed, or null if the conversion succeeded. The conversion fails if the s parameter
		/// is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>A command object that represents the command contained in s</returns>
		protected static bool TryParse(string s, System.Net.IPEndPoint localEndpoint, ModuleServer destination, out Command cmd, out Exception ex)
		{
			Regex rx;
			Match m;
			Prototype proto;
			//Module source;
			//Module destination;
			IModule src;
			IModule dest;
			string sCommand;
			string sParams;
			string sSrc;
			string sDest;
			string sId;
			int id;

			cmd = null;
			ex = null;

			// Regular Expresion Generation
			rx = rxCommandFromEndpoint;
			m = rx.Match(s);
			// Check if input string matchs Regular Expression
			if (!m.Success)
			{
				ex = new ArgumentException("Invalid String", "s");
				return false;
			}
			// Extract Data
			sCommand = m.Result("${cmd}").ToLower();
			if (!destination.SupportCommand(sCommand))
			{
				ex = new Exception("Unsupported command provided");
				return false;
			}
			sParams = m.Result("${params}");
			sId = m.Result("${id}");
			sSrc = m.Result("${src}");
			sDest = m.Result("${dest}");
			if ((sId == null) || (sId.Length < 1)) id = -1;
			else id = Int32.Parse(sId);
			// When at least one module is specified...
			if ((sSrc != null) && (sSrc.Length > 0))
			{
				// If only one module is specified, there is two options:
				// the specified module is this (and we dont know the source module) or
				// the specified module is the source module
				if ((sDest == null) || (sDest.Length < 1))
				{
					if (sSrc == destination.Name) dest = destination;
					else src = new Module(sSrc, localEndpoint.Address, localEndpoint.Port);
				}

				// If both, source and destination module are specified, since there is no
				// way of get the source module, a new one is created.
				// However its necesary to check if this is the apropiate destination module
				else
				{
					if (sDest != destination.Name) throw new Exception("Invalid destination module");
					dest = destination;
					src = new Module(sSrc, localEndpoint.Address, localEndpoint.Port);
				}
			}
			// If no module is specified, then its set to null
			src = null;
			// Browse for an adequate prototype
			proto = destination.Prototypes[sCommand];
			// Check if command matchs a prototype
			if ((proto != null) && proto.ParamsRequired && ((sParams == null) || (sParams.Length < 1)))
			{
				ex = new Exception("Invalid string. The Command requires parameters");
				return false;
			}
			// Create the Command
			cmd = new Command(src, destination, sCommand, sParams, id);
			cmd.prototype = proto;
			cmd.sentTime = DateTime.Now;
			return true;
		}

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// </summary>
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="source">The module which sent the command</param>
		/// <param name="cmd">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="ex">When this method returns, contains the Exception generated during the parse operation,
		/// if the conversion failed, or null if the conversion succeeded. The conversion fails if the s parameter
		/// is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		protected static bool TryParse(string s, Module source, out Command cmd, out Exception ex)
		{
			Regex rx;
			Match m;
			Prototype proto;
			//Module destination;
			IModule destination;
			string sCommand;
			string sParams;
			string sDest;
			string sId;
			int id;

			cmd = null;
			ex = null;

			// Regular Expresion Generation
			//rx = new Regex(@"((?<src>[\w\-]+)(\s+(?<dest>[\w\-]+))?\s+)?(?<cmd>[A-Za-z_]+)\s+(""(?<params>[^""]*)"")?\s+(@(?<id>\d+))?");
			rx = rxCommandFromSource;
			m = rx.Match(s);
			// Check if input string matchs Regular Expression
			if (!m.Success)
			{
				ex = new ArgumentException("Invalid String", "s");
				return false;
			}
			// Extract Data
			sCommand = m.Result("${cmd}").ToLower();
			sParams = m.Result("${params}");
			sId = m.Result("${id}");
			sDest = m.Result("${dest}");
			if ((sId == null) || (sId.Length < 1)) id = -1;
			else id = Int32.Parse(sId);
			// Browse for destination module and prototype
			if (!source.Parent.FindDestinationModule(sCommand, out destination, out proto))
			{
				// No destination module found. Must check if destination is availiable
				if ((sDest == null) || (sDest.Length < 1) || !source.Parent.Modules.Contains(sDest))
				{
					ex = new Exception("No destination module found for command provided");
					return false;
				}
				destination = source.Parent.Modules[sDest];
				proto = null;
			}
			// Check if command matchs a prototype
			if ((proto != null) && proto.ParamsRequired && ((sParams == null) || (sParams.Length < 1)))
			{
				ex = new Exception("Invalid string. The Command requires parameters");
				return false;
			}
			// Create the Command
			cmd = new Command(source, destination, sCommand, sParams, id);
			cmd.prototype = proto;
			cmd.sentTime = DateTime.Now;
			return true;
		}

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// </summary>
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="source">The module which sent the command</param>
		/// <param name="destination">The destination module for the command</param>
		/// <param name="cmd">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="ex">When this method returns, contains the Exception generated during the parse operation,
		/// if the conversion failed, or null if the conversion succeeded. The conversion fails if the s parameter
		/// is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		protected static bool TryParse(string s, Module source, Module destination, out Command cmd, out Exception ex)
		{
			Regex rx;
			Match m;
			Prototype proto;
			string sCommand;
			string sParams;
			string sId;
			int id;

			cmd = null;
			ex = null;

			// Module Validation
			if (source.Parent != destination.Parent)
			{
				ex = new Exception("Source and destination modules does not belong to the same blackboard");
				return false;
			}
			// Regular Expresion Generation
			rx = rxCommandFromSrcDest;
			m = rx.Match(s);
			// Check if input string matchs Regular Expression
			if (!m.Success)
			{
				ex = new ArgumentException("Invalid String", "s");
				return false;
			}
			// Extract Data
			sCommand = m.Result("${cmd}").ToLower();
			sParams = m.Result("${params}");
			sId = m.Result("${id}");
			if ((sId == null) || (sId.Length < 1)) id = -1;
			else id = Int32.Parse(sId);
			// Browse for an adequate prototype
			proto = null;
			for (int i = 0; i < destination.Prototypes.Count; ++i)
			{
				if (destination.Prototypes[i].Command == sCommand)
				{
					proto = destination.Prototypes[i];
					break;
				}
			}
			// Check if command matchs a prototype. If no prototype found, asume redirection
			if ((proto != null) && proto.ParamsRequired && ((sParams == null) || (sParams.Length < 1)))
			{
				ex = new Exception("Invalid string. The Command requires parameters");
				return false;
			}
			// Create the Command
			cmd = new Command(source, destination, sCommand, sParams, id);
			cmd.prototype = proto;
			cmd.sentTime = DateTime.Now;
			return true;
		}

#else

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// </summary>
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="blackboard">A blackboard which contains all information about modules</param>
		/// <param name="cmd">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="ex">When this method returns, contains the Exception generated during the parse operation,
		/// if the conversion failed, or null if the conversion succeeded. The conversion fails if the s parameter
		/// is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		protected static bool TryParse(string s, Blackboard blackboard, out Command cmd, out Exception ex)
		{
			IPrototype proto;
			//Module source;
			//Module destination;
			IModuleClient source;
			IModuleClient destination;
			string sCommand;
			string sParams;
			string sSrc;
			string sDest;
			int result;
			int id;

			cmd = null;
			ex = null;
			
			// Extract Data
			CommandBase.XtractCommandElements(s, out sSrc, out sDest, out sCommand, out sParams, out result, out id);
			if ((sCommand == null) || (result != -1))
			{
				ex = new ArgumentException("Invalid String", "s");
				return false;
			}

			// Check if source module is specified
			if ((sSrc == null) || (sSrc.Length < 1) || !blackboard.Modules.Contains(sSrc))
			{
				ex = new Exception("No source module found for command provided");
				return false;
			}
			source = blackboard.Modules[sSrc];
			// Browse for destination module and prototype
			if (!blackboard.FindDestinationModule(sCommand, out destination, out proto))
			//throw new Exception("No destination module found for command provided");
			{
				// No destination module found. Must check if destination is availiable
				if ((sDest == null) || (sDest.Length < 1) || !blackboard.Modules.Contains(sDest))
				{
					ex = new Exception("No destination module found for command provided");
					return false;
				}
				destination = blackboard.Modules[sDest];
				proto = null;
			}
			// Check if command matchs a prototype
			if ((proto != null) && proto.ParamsRequired && ((sParams == null) || (sParams.Length < 1)))
			{
				ex = new Exception("Invalid string. The Command requires parameters");
				return false;
			}
			// Create the Command
			cmd = new Command(source, destination, sCommand, sParams, id);
			cmd.prototype = proto;
			cmd.sentTime = DateTime.Now;
			return true;
		}

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// </summary>
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="source">The module which sent the command</param>
		/// <param name="cmd">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="ex">When this method returns, contains the Exception generated during the parse operation,
		/// if the conversion failed, or null if the conversion succeeded. The conversion fails if the s parameter
		/// is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		protected static bool TryParse(string s, ModuleClient source, out Command cmd, out Exception ex)
		{
			IPrototype proto;
			//Module destination;
			IModuleClient destination;
			string sCommand;
			string sParams;
			string sDest;
			int result;
			int id;

			cmd = null;
			ex = null;

			// Extract Data
			CommandBase.XtractCommandElements(s, out sDest, out sCommand, out sParams, out result, out id);
			if ((sCommand == null) || (result != -1))
			{
				ex = new ArgumentException("Invalid String", "s");
				return false;
			}
			
			// Browse for destination module and prototype
			if (source.SupportCommand(sCommand, out proto))
				destination = source;
			else if(!source.Parent.FindDestinationModule(sCommand, out destination, out proto))
			{
				// No destination module found. Must check if destination is availiable
				if ((sDest == null) || (sDest.Length < 1) || !source.Parent.Modules.Contains(sDest))
				{
					ex = new Exception("No destination module found for command provided");
					return false;
				}
				destination = source.Parent.Modules[sDest];
				proto = null;
			}
			// Check if command matchs a prototype
			if (proto == null)
			{
				ex = new Exception("No prototype for provided command was found");
				return false;
			}
			if ((proto != null) && proto.ParamsRequired && ((sParams == null) || (sParams.Length < 1)))
			{
				ex = new Exception("Invalid string. The Command requires parameters");
				return false;
			}
			// Create the Command
			cmd = new Command(source, destination, sCommand, sParams, id);
			cmd.prototype = proto;
			cmd.sentTime = DateTime.Now;
			return true;
		}

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// </summary>
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="source">The module which sent the command</param>
		/// <param name="destination">The destination module for the command</param>
		/// <param name="cmd">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="ex">When this method returns, contains the Exception generated during the parse operation,
		/// if the conversion failed, or null if the conversion succeeded. The conversion fails if the s parameter
		/// is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		protected static bool TryParse(string s, ModuleClient source, ModuleClient destination, out Command cmd, out Exception ex)
		{
			IPrototype proto;
			string sCommand;
			string sParams;
			int result;
			int id;

			cmd = null;
			ex = null;

			// Module Validation
			if (source.Parent != destination.Parent)
			{
				ex = new Exception("Source and destination modules does not belong to the same blackboard");
				return false;
			}
			
			// Extract Data
			CommandBase.XtractCommandElements(s, out sCommand, out sParams, out result, out id);
			if ((sCommand == null) || (result != -1))
			{
				ex = new ArgumentException("Invalid String", "s");
				return false;
			}
			
			// Browse for an adequate prototype
			proto = null;
			for (int i = 0; i < destination.Prototypes.Count; ++i)
			{
				if (destination.Prototypes[i].Command == sCommand)
				{
					proto = destination.Prototypes[i];
					break;
				}
			}
			// Check if command matchs a prototype. If no prototype found, asume redirection
			if ((proto != null) && proto.ParamsRequired && ((sParams == null) || (sParams.Length < 1)))
			{
				ex = new Exception("Invalid string. The Command requires parameters");
				return false;
			}
			// Create the Command
			cmd = new Command(source, destination, sCommand, sParams, id);
			cmd.prototype = proto;
			cmd.sentTime = DateTime.Now;
			return true;
		}

#endif

		#endregion

		#region Parse

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="blackboard">A blackboard which contains all information about modules</param>
		/// </summary>
		/// <returns>A command object that represents the command contained in s</returns>
		public static Command Parse(string s, Blackboard blackboard)
		{
			Exception ex;
			Command command;

			if (!TryParse(s, blackboard, out command, out ex))
				throw ex;
			else return command;
		}

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="source">The module which sent the command</param>
		/// </summary>
		/// <returns>A command object that represents the command contained in s</returns>
		public static Command Parse(string s, ModuleClient source)
		{
			Exception ex;
			Command command;

			if (!TryParse(s, source, out command, out ex))
				throw ex;
			else return command;
		}

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="source">The module which sent the command</param>
		/// <param name="destination">The destination module for the command</param>
		/// </summary>
		/// <returns>A command object that represents the command contained in s</returns>
		public static Command Parse(string s, ModuleClient source, ModuleClient destination)
		{
			Exception ex;
			Command command;

			if (!TryParse(s, source, destination, out command, out ex))
				throw ex;
			else return command;
		}

		#endregion

		/// <summary>
		/// Checks if input string is a command
		/// </summary>
		/// <param name="s">string to analyze</param>
		/// <returns>true if input is command, false otherwise</returns>
		public static bool IsCommand(string s)
		{
			Regex rx;

			rx = new Regex(@"^([\w\-]+(\s+[\w\-]+)?\s+)?[A-Za-z_]+(\s+""[^""]*"")?(\s+@\d+)?$");
			return rx.IsMatch(s);
		}

		/// <summary>
		/// Gets a value indicating if provided Response is a response for provided command and calculates the afinity between them
		/// </summary>
		/// <param name="command">Command to check with</param>
		/// <param name="response">Response to check</param>
		/// <param name="afinity">An integer that represents how much compatible are the command and response provided.
		/// A Zero value indicates the most high afinity between them. This parameter is passed uninitialized.</param>
		/// <returns>true if provided Response is a response for command, false otherwise</returns>
		public static bool IsMatch(Command command, Response response, out int afinity)
		{
			// This one is if the response destination is known
			afinity = 0;
			bool result = false;
			if (response.Destination != null)
			{
				result = (command.Source == response.Destination)
					&& (command.Destination == response.Source)
					&& (command.Command == response.Command)
					&& (command.SentTime <= response.ArrivalTime);
				//if((command.Id != -1) && (response.Id != -1) && (command.Id != response.Id)) ++afinity;
				if (command.Id != response.Id) ++afinity;
				if (command.parameters != response.Parameters) ++afinity;
			}
			else
			{
				++afinity;
				result = (command.Destination == response.Source)
					&& (command.Command == response.Command)
					&& (command.SentTime <= response.ArrivalTime);
				//if((command.Id != -1) && (response.Id != -1) && (command.Id != response.Id)) ++afinity;
				if (command.Id != response.Id) ++afinity;
				if (command.parameters != response.Parameters) ++afinity;
			}
			// This one is for search for an adequate command-response match
			return result;
		}

		#endregion

		#region System Commands

		/*

		/// <summary>
		/// Creates a alive system control command
		/// </summary>
		/// <param name="source">Module which generated the command</param>
		/// <param name="destination">Module to which this command will be sent</param>
		/// <returns>A Command object that represents the alive command</returns>
		public static Command CreateAliveCommand(IModule source, IModule destination)
		{
			Command cmd = new Command();
			cmd.command = "alive";
			cmd.destination = destination;
			cmd.id = -1;
			cmd.originalString = cmd.command;
			cmd.parameters = "";
			cmd.source = source;
			cmd.prototype = Module.PrototypeAlive;
			return cmd;
		}

		/// <summary>
		/// Creates a bin system control command
		/// </summary>
		/// <param name="source">Module which generated the command</param>
		/// <param name="destination">Module to which this command will be sent</param>
		/// <returns>A Command object that represents the bin command</returns>
		public static Command CreateBinCommand(IModule source, IModule destination)
		{
			Command cmd = new Command();
			cmd.command = "bin";
			cmd.destination = destination;
			cmd.id = -1;
			cmd.originalString = cmd.command;
			cmd.parameters = "";
			cmd.source = source;
			cmd.prototype = Module.PrototypeBin;
			return cmd;
		}

		/// <summary>
		/// Creates a busy system control command
		/// </summary>
		/// <param name="source">Module which generated the command</param>
		/// <param name="destination">Module to which this command will be sent</param>
		/// <returns>A Command object that represents the busy command</returns>
		public static Command CreateBusyCommand(IModule source, IModule destination)
		{
			Command cmd = new Command();
			cmd.command = "busy";
			cmd.destination = destination;
			cmd.id = -1;
			cmd.originalString = cmd.command;
			cmd.parameters = "";
			cmd.source = source;
			cmd.prototype = Module.PrototypeBusy;
			return cmd;
		}

		/// <summary>
		/// Creates a connected system control command
		/// </summary>
		/// <param name="source">Module which generated the command</param>
		/// <param name="destination">Module to which this command will be sent</param>
		/// <returns>A Command object that represents the connected command</returns>
		public static Command CreateConnectedCommand(IModule source, IModule destination)
		{
			Command cmd = new Command();
			cmd.command = "connected";
			cmd.destination = destination;
			cmd.id = -1;
			cmd.originalString = cmd.command;
			cmd.parameters = "";
			cmd.source = source;
			cmd.prototype = Module.PrototypeConnected;
			return cmd;
		}

		/// <summary>
		/// Creates a ready system control command
		/// </summary>
		/// <param name="source">Module which generated the command</param>
		/// <param name="destination">Module to which this command will be sent</param>
		/// <returns>A Command object that represents the ready command</returns>
		public static Command CreateReadyCommand(IModule source, IModule destination)
		{
			Command cmd = new Command();
			cmd.command = "ready";
			cmd.destination = destination;
			cmd.id = -1;
			cmd.originalString = cmd.command;
			cmd.parameters = "";
			cmd.source = source;
			cmd.prototype = Module.PrototypeReady;
			return cmd;
		}
		 * 
		*/

		#endregion
	}
}