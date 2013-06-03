//#define RegexResponseParse

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Blk.Api;

namespace Blk.Engine
{

	/// <summary>
	/// Represents a command response
	/// </summary>
	public class Response : CommandBase, ITextResponse
	{
		#region Variables

		/// <summary>
		/// Indicates if the command execution was successfull or not
		/// </summary>
		private bool success;
		/// <summary>
		/// Indicates the date and time when the response has arrived
		/// </summary>
		private DateTime arrivalTime;
		/// <summary>
		/// Stores the reason for which the command was not executed generating a failed response
		/// </summary>
		private ResponseFailReason failReason;
		/// <summary>
		/// Stores the command this response is responding
		/// </summary>
		private Command commandResponded;
		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor for further initialization
		/// </summary>
		protected Response()
			: base()
		{
		}

		/*

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the message</param>
		/// <param name="response">Response sent</param>
		/// <param name="param">Param sent</param>
		public Response(Module source, string response, string param) : this(source, null, response, param, -1) { }

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the message</param>
		/// <param name="response">Response sent</param>
		/// <param name="param">Param sent</param>
		/// <param name="id">id of the message</param>
		public Response(Module source, string response, string param, int id) : this(source, null, response, param, id) { }

		*/

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the message</param>
		/// <param name="destination">Module to which this message will be sent</param>
		/// <param name="response">Response sent</param>
		/// <param name="param">Param sent</param>
		/// <param name="success">Value that indicates if the command was executed successfully</param>
		public Response(IModuleClient source, IModuleClient destination, string response, string param, bool success) : this(source, destination, response, param, success, -1) { }

		/// <summary>
		/// Initiates a new instance of Message
		/// </summary>
		/// <param name="source">Module which generated the message</param>
		/// <param name="destination">Module to which this message will be sent</param>
		/// <param name="response">Response sent</param>
		/// <param name="param">Param sent</param>
		/// <param name="id">id of the message</param>
		/// <param name="success">Value that indicates if the command was executed successfully</param>
		public Response(IModuleClient source, IModuleClient destination, string response, string param, bool success, int id)
			: base()
		{
			this.source = source;
			this.destination = destination;
			this.command = response;
			this.parameters = param;
			this.id = id;
			if (this.success = success) failReason = ResponseFailReason.None;
			else failReason = ResponseFailReason.Unknown;
			this.arrivalTime = DateTime.Now;

			if ((destination != null) && (source.Parent != destination.Parent))
				throw new Exception("Source and destination modules does not belong to the same blackboard");
			for (int i = 0; i < source.Prototypes.Count; ++i)
			{
				if (source.Prototypes[i].Command == response)
				{
					this.prototype = source.Prototypes[i];
					break;
				}
			}
			// Check if response matchs a prototype
			if (this.prototype.ParamsRequired && ((param == null) || (param.Length < 1)))
				throw new Exception("Invalid string. The Response requires parameters");
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the date and time when the response has arrived
		/// </summary>
		public DateTime ArrivalTime
		{
			get { return arrivalTime; }
		}

		/// <summary>
		/// Gets the command this Response is responding
		/// </summary>
		public Command CommandResponded
		{
			get { return commandResponded; }
			internal set
			{
				if (commandResponded == value)
				{
					if (commandResponded.Response == null)
						commandResponded.Response = this;
					return;
				}
				if (commandResponded == null)
					commandResponded = value;
				else throw new Exception("Property can be asigned only once");
				if (commandResponded.Response == null)
					commandResponded.Response = this;
			}
		}

		/// <summary>
		/// Gets the command this Response is responding
		/// </summary>
		ITextCommand ITextResponse.CommandResponded
		{
			get { return commandResponded; }
		}

		/// <summary>
		/// Gets the destination module for this response.
		/// Also allows to set when destination module is null
		/// </summary>
		new public IModuleClient Destination
		{
			get
			{
				return base.Destination;
			}
			set
			{
				if (destination != null) throw new ArgumentException("This property is read only");
				if (value == null) throw new ArgumentNullException();
				destination = value;
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

				sb.Append(' ');
				if (success) sb.Append('1');
				else sb.Append('0');

				if (id >= 0)
				{
					sb.Append(" @");
					sb.Append(id);
				}
				return sb.ToString();
			}
		}

		/// <summary>
		/// Gets a value indicating if the command execution was successfull or not
		/// </summary>
		public bool Success
		{
			get { return success; }
		}

		/// <summary>
		/// Gets the reason for which the command was not executed generating a failed response 
		/// </summary>
		public ResponseFailReason FailReason
		{
			get
			{
				if (success) return ResponseFailReason.None;
				return failReason;
			}
			internal set
			{
				if (success) failReason = ResponseFailReason.None;
				else failReason = value;
			}
		}

		#endregion

		#region Events
		#endregion

		#region Methods

		/// <summary>
		/// Sends itself to its Destination module
		/// </summary>
		/// <returns>true if the command response has been sent, false otherwise</returns>
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
			if (prototype.ParamsRequired || this.HasParams)
			{
				sb.Append(" \"");
				sb.Append(parameters);
				sb.Append("\"");
			}

			if (success) sb.Append(" 1");
			else sb.Append(" 0");

			if (id >= 0)
			{
				sb.Append(" @");
				sb.Append(id);
			}

			return sb.ToString();
		}

		#endregion

		#region Operators
		/// <summary>
		/// Implicitly converts the Message to a string which can be sent to a module
		/// </summary>
		/// <param name="c">Response to be converted</param>
		/// <returns>A string well formated</returns>
		public static implicit operator string(Response c)
		{
			//string s = c.response;
			//if (c.parameters.Length > 0) s += " \"" + c.parameters + "\"";
			//if (c.id >= 0) s += " @" + c.id.ToString();
			//return s;
			return c.StringToSend;
		}

		#endregion

		#region Static Variables

		private static Regex rxResponseFromBlackboard =
			new Regex(@"(?<src>[\w\-]+)(\s+(?<dest>[\w\-]+))?\s+(?<cmd>[A-Za-z_]+)(\s+""(?<params>[^""]*)"")?\s+(?<result>[10])(\s+@(?<id>\d+))?", RegexOptions.Compiled);
		private static Regex rxResponseFromEndpoint =
			new Regex(@"^((?<src>[\w\-]+)(\s+(?<dest>[\w\-]+))?\s+)?(?<cmd>[A-Za-z_]+)(\s+""(?<params>[^""]*)"")?\s+(?<result>[10])(\s+@(?<id>\d+))?$", RegexOptions.Compiled);
		private static Regex rxResponseFromSource =
			new Regex(@"((?<dest>[\w\-]+)\s+)?(?<cmd>[A-Za-z_]+)(\s+""(?<params>[^""]*)"")?\s+(?<result>[10])(\s+@(?<id>\d+))?", RegexOptions.Compiled);
		private static Regex rxResponseFromSrcDest =
			new Regex(@"(?<cmd>[A-Za-z_]+)(\s+""(?<params>[^""]*)"")?\s+(?<result>[10])(\s+@(?<id>\d+))?", RegexOptions.Compiled);

		#endregion

		#region Static Methods

		#region TryParse

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="blackboard">A blackboard which contains all information about modules</param>
		/// <param name="result">When this method returns, contains the Response equivalent to the response
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string s, Blackboard blackboard, out Response result)
		{
			Exception ex;

			return TryParse(s, blackboard, out result, out ex);
		}

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="source">The module which sent the response</param>
		/// <param name="result">When this method returns, contains the Response equivalent to the response
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string s, ModuleClient source, out Response result)
		{
			Exception ex;

			return TryParse(s, source, out result, out ex);
		}

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="source">The module which sent the response</param>
		/// <param name="destination">The destination module for the response</param>
		/// <param name="result">When this method returns, contains the Response equivalent to the response
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string s, ModuleClient source, ModuleClient destination, out Response result)
		{
			Exception ex;

			return TryParse(s, source, destination, out result, out ex);
		}

		#endregion

		#region TryParse (Code)

#if RegexResponseParse

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="blackboard">A blackboard which contains all information about modules</param>
		/// <param name="response">When this method returns, contains the Response equivalent to the response
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="ex">When this method returns, contains the Exception generated during the parse operation,
		/// if the conversion failed, or null if the conversion succeeded. The conversion fails if the s parameter
		/// is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		protected static bool TryParse(string s, Blackboard blackboard, out Response response, out Exception ex)
		{
			Regex rx;
			Match m;
			Prototype proto;
			//Module source;
			IModule source;
			//Module destination;
			IModule destination;
			string sCommand;
			string sParams;
			string sSrc;
			string sDest;
			string sId;
			string sResult;
			bool result;
			int id;

			ex = null;
			response = null;

			// Regular Expresion Generation
			//rx = new Regex(@"((?<src>[\w\-]+)(\s+(?<dest>[\w\-]+))?\s+)?(?<cmd>[A-Za-z_]+)\s+(""(?<params>[^""]*)"")?\s+(@(?<id>\d+))?");
			rx = rxResponseFromBlackboard;
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
			sResult = m.Result("${result}");
			if ((sResult == null) || ((sResult != "1") && (sResult != "0")))
			{
				ex = new Exception("Invalid string. No suitable result value found");
				return false;
			}
			result = (sResult == "1");
			if ((sId == null) || (sId.Length < 1)) id = -1;
			else id = Int32.Parse(sId);
			// Check if source module is specified
			if ((sSrc == null) || (sSrc.Length < 1) || !blackboard.Modules.Contains(sSrc))
			{
				ex = new Exception("No source module found for response provided");
				return false;
			}
			source = blackboard.Modules[sSrc];
			// Browse for destination module and prototype
			if (!blackboard.FindDestinationModule(sCommand, out source, out proto))
			//Since the response arrived from a module which can't generate it a exception is rised
			{
				ex = new Exception("Sender module and generator module does not match");
				return false;
			}

			// if destination module is specified, then it mus be set
			if ((sDest != null) && (sDest.Length > 0) && source.Parent.Modules.Contains(sDest))
				destination = source.Parent.Modules[sDest];
			// Else, since there is no way at this point to determine which is destination module for response
			// and this is no specified, it is set to null, to allow blackboard to find an apropiate one
			else
				destination = null;
			// Check if response matchs a prototype
			if ((proto != null) && proto.ParamsRequired && ((sParams == null) || (sParams.Length < 1)))
			{
				ex = new Exception("Invalid string. The Response requires parameters");
				return false;
			}
			// Create the Response
			response = new Response(source, destination, sCommand, sParams, result, id);
			response.prototype = proto;
			if (!response.success) response.failReason = ResponseFailReason.ExecutedButNotSucceded;
			return true;
		}

		/// <summary>
		/// Converts the string representation of a response to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="localEndpoint">The remote endpoint source of the string</param>
		/// <param name="destination">The module which received the response</param>
		/// <param name="response">When this method returns, contains the Response equivalent to the response
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="ex">When this method returns, contains the Exception generated during the parse operation,
		/// if the conversion failed, or null if the conversion succeeded. The conversion fails if the s parameter
		/// is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		protected static bool TryParse(string s, System.Net.IPEndPoint localEndpoint, ModuleServer destination, out Response response, out Exception ex)
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
			string sResult;
			bool result;
			int id;

			ex = null;
			response = null;

			// Regular Expresion Generation
			rx = rxResponseFromEndpoint;
			m = rx.Match(s);
			// Check if input string matches Regular Expression
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
			sResult = m.Result("${result}");
			if ((sResult == null) || ((sResult != "1") && (sResult != "0")))
			{
				ex = new Exception("Invalid string. No suitable result value found");
				return false;
			}
			result = (sResult == "1");
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
					if (sDest != destination.Name)
					{
						ex = new Exception("Invalid destination module");
						return false;
					}
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
			response = new Response(src, destination, sCommand, sParams, result, id);
			response.prototype = proto;
			if (!response.success) response.failReason = ResponseFailReason.ExecutedButNotSucceded;
			return true;
		}

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="source">The module which sent the response</param>
		/// <param name="response">When this method returns, contains the Response equivalent to the response
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="ex">When this method returns, contains the Exception generated during the parse operation,
		/// if the conversion failed, or null if the conversion succeeded. The conversion fails if the s parameter
		/// is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		protected static bool TryParse(string s, Module source, out Response response, out Exception ex)
		{
			Regex rx;
			Match m;
			Prototype proto;
			//Module destination;
			IModule destination;
			//Module eSource;
			IModule eSource;
			string sCommand;
			string sParams;
			string sDest;
			string sId;
			string sResult;
			bool result;
			int id;

			ex = null;
			response = null;

			// Regular Expresion Generation
			//rx = new Regex(@"((?<src>[\w\-]+)(\s+(?<dest>[\w\-]+))?\s+)?(?<cmd>[A-Za-z_]+)\s+(""(?<params>[^""]*)"")?\s+(@(?<id>\d+))?");
			rx = rxResponseFromSource;
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
			sResult = m.Result("${result}");
			if ((sResult == null) || ((sResult != "1") && (sResult != "0")))
			{
				ex = new Exception("Invalid string. No suitable result value found");
				return false;
			}
			result = (sResult == "1");
			if ((sId == null) || (sId.Length < 1)) id = -1;
			else id = Int32.Parse(sId);
			// Browse for prototype and validating command
			if (!source.Parent.FindDestinationModule(sCommand, out eSource, out proto) || (eSource != source))
			//Since the response arrived from a module which can't generate it a exception is rised
			{
				ex = new Exception("Sender module and generator module does not match");
				return false;
			}

			// if destination module is specified, then it mus be set
			if ((sDest != null) && (sDest.Length > 0) && source.Parent.Modules.Contains(sDest))
				destination = source.Parent.Modules[sDest];
			// Else, since there is no way at this point to determine which is destination module for response
			// and this is no specified, it is set to null, to allow blackboard to find an apropiate one
			else
				destination = null;
			// Check if response matchs a prototype
			if ((proto != null) && proto.ParamsRequired && ((sParams == null) || (sParams.Length < 1)))
			{
				ex = new Exception("Invalid string. The Response requires parameters");
				return false;
			}
			// Create the Response
			response = new Response(source, destination, sCommand, sParams, result, id);
			response.prototype = proto;
			if (!response.success) response.failReason = ResponseFailReason.ExecutedButNotSucceded;
			return true;
		}

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="source">The module which sent the response</param>
		/// <param name="destination">The destination module for the response</param>
		/// <param name="response">When this method returns, contains the Response equivalent to the response
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="ex">When this method returns, contains the Exception generated during the parse operation,
		/// if the conversion failed, or null if the conversion succeeded. The conversion fails if the s parameter
		/// is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		protected static bool TryParse(string s, Module source, Module destination, out Response response, out Exception ex)
		{
			Regex rx;
			Match m;
			Prototype proto;
			string sCommand;
			string sParams;
			string sId;
			string sResult;
			bool result;
			int id;

			ex = null;
			response = null;

			// Module Validation
			if (source.Parent != destination.Parent)
			{
				ex = new Exception("Source and destination modules does not belong to the same blackboard");
				return false;
			}
			// Regular Expresion Generation
			rx = rxResponseFromSrcDest;
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
			sResult = m.Result("${result}");
			if ((sResult == null) || ((sResult != "1") && (sResult != "0")))
			{
				ex = new Exception("Invalid string. No suitable result value found");
				return false;
			}
			result = (sResult == "1");
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
			// Check if response matchs a prototype. If no prototype found, asume redirection
			if ((proto != null) && proto.ParamsRequired && ((sParams == null) || (sParams.Length < 1)))
			{
				ex = new Exception("Invalid string. The Response requires parameters");
				return false;
			}
			// Create the Response
			response = new Response(source, destination, sCommand, sParams, result, id);
			response.prototype = proto;
			if (!response.success) response.failReason = ResponseFailReason.ExecutedButNotSucceded;
			return true;
		}

#else

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="blackboard">A blackboard which contains all information about modules</param>
		/// <param name="response">When this method returns, contains the Response equivalent to the response
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="ex">When this method returns, contains the Exception generated during the parse operation,
		/// if the conversion failed, or null if the conversion succeeded. The conversion fails if the s parameter
		/// is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		protected static bool TryParse(string s, Blackboard blackboard, out Response response, out Exception ex)
		{
			IPrototype proto;
			IModuleClient source;
			IModuleClient destination;
			string sCommand;
			string sParams;
			string sSrc;
			string sDest;
			bool result;
			int iResult;
			int id;

			ex = null;
			response = null;

			// Extract Data
			CommandBase.XtractCommandElements(s, out sSrc, out sDest, out sCommand, out sParams, out iResult, out id);
			if ((sCommand == null) || (iResult == -1))
			{
				ex = new ArgumentException("Invalid String", "s");
				return false;
			}
			result = iResult == 1;

			// Check if source module is specified
			if ((sSrc == null) || (sSrc.Length < 1) || !blackboard.Modules.Contains(sSrc))
			{
				ex = new Exception("No source module found for response provided");
				return false;
			}
			source = blackboard.Modules[sSrc];
			// Browse for destination module and prototype
			if (!blackboard.FindDestinationModule(sCommand, out source, out proto))
			//Since the response arrived from a module which can't generate it a exception is rised
			{
				ex = new Exception("Sender module and generator module does not match");
				return false;
			}

			// if destination module is specified, then it mus be set
			if ((sDest != null) && (sDest.Length > 0) && source.Parent.Modules.Contains(sDest))
				destination = source.Parent.Modules[sDest];
			// Else, since there is no way at this point to determine which is destination module for response
			// and this is no specified, it is set to null, to allow blackboard to find an apropiate one
			else
				destination = null;
			// Check if response matchs a prototype
			if ((proto != null) && proto.ParamsRequired && ((sParams == null) || (sParams.Length < 1)))
			{
				ex = new Exception("Invalid string. The Response requires parameters");
				return false;
			}
			// Create the Response
			response = new Response(source, destination, sCommand, sParams, result, id);
			response.prototype = proto;
			if (!response.success) response.failReason = ResponseFailReason.ExecutedButNotSucceded;
			return true;
		}

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="source">The module which sent the response</param>
		/// <param name="response">When this method returns, contains the Response equivalent to the response
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="ex">When this method returns, contains the Exception generated during the parse operation,
		/// if the conversion failed, or null if the conversion succeeded. The conversion fails if the s parameter
		/// is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		protected static bool TryParse(string s, ModuleClient source, out Response response, out Exception ex)
		{
			IPrototype proto;
			//Module destination;
			IModuleClient destination;
			//Module eSource;
			IModuleClient eSource;
			string sCommand;
			string sParams;
			string sDest;
			bool result;
			int iResult;
			int id;

			ex = null;
			response = null;

			// Extract Data
			CommandBase.XtractCommandElements(s, out sDest, out sCommand, out sParams, out iResult, out id);
			if ((sCommand == null) || (iResult == -1))
			{
				ex = new ArgumentException("Invalid String", "s");
				return false;
			}
			result = iResult == 1;
			
			// Browse for prototype and validating command
			if (!source.Parent.FindDestinationModule(sCommand, out eSource, out proto) || (eSource != source))
			//Since the response arrived from a module which can't generate it a exception is rised
			{
				ex = new Exception("Sender module and generator module does not match");
				return false;
			}

			// if destination module is specified, then it mus be set
			if ((sDest != null) && (sDest.Length > 0) && source.Parent.Modules.Contains(sDest))
				destination = source.Parent.Modules[sDest];
			// Else, since there is no way at this point to determine which is destination module for response
			// and this is no specified, it is set to null, to allow blackboard to find an apropiate one
			else
				destination = null;
			// Check if response matchs a prototype
			if ((proto != null) && proto.ParamsRequired && ((sParams == null) || (sParams.Length < 1)))
			{
				ex = new Exception("Invalid string. The Response requires parameters");
				return false;
			}
			// Create the Response
			response = new Response(source, destination, sCommand, sParams, result, id);
			response.prototype = proto;
			if (!response.success) response.failReason = ResponseFailReason.ExecutedButNotSucceded;
			return true;
		}

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="source">The module which sent the response</param>
		/// <param name="destination">The destination module for the response</param>
		/// <param name="response">When this method returns, contains the Response equivalent to the response
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="ex">When this method returns, contains the Exception generated during the parse operation,
		/// if the conversion failed, or null if the conversion succeeded. The conversion fails if the s parameter
		/// is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		protected static bool TryParse(string s, ModuleClient source, ModuleClient destination, out Response response, out Exception ex)
		{
			IPrototype proto;
			string sCommand;
			string sParams;
			bool result;
			int iResult;
			int id;

			ex = null;
			response = null;

			// Module Validation
			if (source.Parent != destination.Parent)
			{
				ex = new Exception("Source and destination modules does not belong to the same blackboard");
				return false;
			}
			// Extract Data
			CommandBase.XtractCommandElements(s, out sCommand, out sParams, out iResult, out id);
			if ((sCommand == null) || (iResult == -1))
			{
				ex = new ArgumentException("Invalid String", "s");
				return false;
			}
			result = iResult == 1;

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
			// Check if response matchs a prototype. If no prototype found, asume redirection
			if ((proto != null) && proto.ParamsRequired && ((sParams == null) || (sParams.Length < 1)))
			{
				ex = new Exception("Invalid string. The Response requires parameters");
				return false;
			}
			// Create the Response
			response = new Response(source, destination, sCommand, sParams, result, id);
			response.prototype = proto;
			if (!response.success) response.failReason = ResponseFailReason.ExecutedButNotSucceded;
			return true;
		}

#endif

		#endregion

		#region Parse

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="blackboard">A blackboard which contains all information about modules</param>
		/// </summary>
		/// <returns>A response object that represents the response contained in s</returns>
		public static Response Parse(string s, Blackboard blackboard)
		{
			Exception ex;
			Response response;

			if (!TryParse(s, blackboard, out response, out ex))
				throw ex;
			else return response;
		}

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="source">The module which sent the response</param>
		/// </summary>
		/// <returns>A response object that represents the response contained in s</returns>
		public static Response Parse(string s, ModuleClient source)
		{
			Exception ex;
			Response response;

			if (!TryParse(s, source, out response, out ex))
				throw ex;
			else return response;
		}

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="source">The module which sent the response</param>
		/// <param name="destination">The destination module for the response</param>
		/// </summary>
		/// <returns>A response object that represents the response contained in s</returns>
		public static Response Parse(string s, ModuleClient source, ModuleClient destination)
		{
			Exception ex;
			Response response;

			if (!TryParse(s, source, destination, out response, out ex))
				throw ex;
			else return response;
		}

		#endregion

		/// <summary>
		/// Creates a response from a command data
		/// </summary>
		/// <param name="command">The command to use as base for the response</param>
		/// <param name="result">true if command succeded, false otherwise</param>
		/// <returns>A generic response for the command with same parameters</returns>
		public static Response CreateFromCommand(Command command, bool result)
		{
			Response response = new Response();
			response.commandResponded = command;
			response.source = command.Destination;
			response.destination = command.Source;
			response.command = command.Command;
			response.parameters = command.Parameters;
			response.id = command.Id;
			if (response.success = result) response.failReason = ResponseFailReason.None;
			else response.failReason = ResponseFailReason.Unknown;
			response.prototype = command.Prototype;
			if (command.SentTime >= DateTime.Now)
				response.arrivalTime = command.SentTime.AddMilliseconds(5);
			response.arrivalTime = DateTime.Now;
			return response;
		}

		/// <summary>
		/// Creates a response from a command data
		/// </summary>
		/// <param name="command">The command to use as base for the response</param>
		/// <param name="failReason">the reason for which command has failed</param>
		/// <returns>A generic response for the command with same parameters</returns>
		public static Response CreateFromCommand(Command command, ResponseFailReason failReason)
		{
			Response response = new Response();
			response.commandResponded = command;
			response.source = command.Destination;
			response.destination = command.Source;
			response.command = command.Command;
			response.parameters = command.Parameters;
			response.id = command.Id;
			response.failReason = failReason;
			response.success = (failReason == ResponseFailReason.None);

			response.prototype = command.Prototype;
			if (command.SentTime >= DateTime.Now)
				response.arrivalTime = command.SentTime.AddMilliseconds(5);
			response.arrivalTime = DateTime.Now;
			return response;
		}

		/// <summary>
		/// Checks if input string is a command response
		/// </summary>
		/// <param name="s">string to analyze</param>
		/// <returns>true if input is command response, false otherwise</returns>
		public static bool IsResponse(string s)
		{
			Regex rx;

			rx = new Regex(@"^([\w\-]+(\s+[\w\-]+)?\s+)?[A-Za-z_]+(\s+""[^""]*"")?\s+[10](\s+@\d+)?$");
			return rx.IsMatch(s);
		}

		#endregion
	}
}