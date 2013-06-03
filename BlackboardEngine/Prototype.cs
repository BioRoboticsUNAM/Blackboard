using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Blk.Api;

namespace Blk.Engine
{
	/// <summary>
	/// A Command Prototype
	/// </summary>
	public class Prototype : IPrototype, IComparable<Prototype>
	{

		#region Constants
		/// <summary>
		/// A constant used to specify a infinite waiting period. This field is constant.
		/// </summary>
		public const int InfiniteTimeout = 0;
		#endregion

		#region Variables
		/// <summary>
		/// Contains name of the command that this prototype represent 
		/// </summary>
		private string command;
		/// <summary>
		/// True if command has priority over other commands
		/// </summary>
		private bool hasPriority;
		/// <summary>
		/// True if a parameters are required, false otherwise
		/// </summary>
		private bool parametersRequired;
		/// <summary>
		/// The Module this prototype is bind to
		/// </summary>
		internal IModuleClient parent;
		/// <summary>
		/// True if a response is required, false otherwise
		/// </summary>
		private bool responseRequired;
		/// <summary>
		/// When responseRequired is set, represents the maximum amount of time to wait for a response
		/// </summary>
		private int timeout;
		
		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of Prototype
		/// </summary>
		/// <param name="command">The name of the command that this prototype will represent</param>
		/// <param name="responseRequired">True if the command need a response, false otherwise</param>
		public Prototype(string command, bool responseRequired) : 
			this(command, true, responseRequired, 0, false) { }

		/// <summary>
		/// Creates a new instance of Prototype
		/// </summary>
		/// <param name="command">The name of the command that this prototype will represent</param>
		/// <param name="paramsRequired">True if the command need at least one parameter, false otherwise</param>
		/// <param name="responseRequired">True if the command need a response, false otherwise</param>
		public Prototype(string command, bool paramsRequired, bool responseRequired) :
			this(command, paramsRequired, responseRequired, 0, false) { }

		/// <summary>
		/// Creates a new instance of Prototype
		/// </summary>
		/// <param name="command">The name of the command that this prototype will represent</param>
		/// <param name="responseRequired">True if the command need a response, false otherwise</param>
		/// <param name="paramsRequired">True if the command need at least one parameter, false otherwise</param>
		/// <param name="timeOut">If a response is required, the amount of time in miliseconds to wait for receive the response</param>
		public Prototype(string command, bool paramsRequired, bool responseRequired, int timeOut) :
			this(command, paramsRequired, responseRequired, 0, false) { }

		/// <summary>
		/// Creates a new instance of Prototype
		/// </summary>
		/// <param name="command">The name of the command that this prototype will represent</param>
		/// <param name="responseRequired">True if the command need a response, false otherwise</param>
		/// <param name="paramsRequired">True if the command need at least one parameter, false otherwise</param>
		/// <param name="timeOut">If a response is required, the amount of time in miliseconds to wait for receive the response</param>
		/// <param name="hasPriority">True if command has priority over other commands</param>
		public Prototype(string command, bool paramsRequired, bool responseRequired, int timeOut, bool hasPriority)
		{
			this.command = command.Trim().ToLower();
			this.parametersRequired = paramsRequired;
			if (this.responseRequired = responseRequired)
				this.timeout = timeOut;
			else this.timeout = 0;
			this.hasPriority = hasPriority;
		}

		#endregion

		#region Properties
		/// <summary>
		/// Name of the command that this prototype represent 
		/// </summary>
		public string Command{get {return command;}}
		/// <summary>
		/// Gets a value indicating if the command has priority over other commands
		/// </summary>
		public bool HasPriority { get { return hasPriority; } }

		/// <summary>
		/// Gets a value indicating if parameters are required
		/// </summary>
		public bool ParamsRequired { get { return parametersRequired; } }
		/// <summary>
		/// True if a response is required, false otherwise
		/// </summary>
		public bool ResponseRequired{get {return responseRequired;}}
		/// <summary>
		/// The maximum amount of time to wait for a response in milliseconds
		/// </summary>
		public int Timeout{get {return timeout;}}
		/// <summary>
		/// Gets the Module this prototype is bind to
		/// </summary>
		public IModuleClient Parent
		{
			get { return parent; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				this.parent = value;
			}
		}

		/// <summary>
		/// Overrided. String representation of the prototype
		/// </summary>
		/// <returns>String representation of the prototype</returns>
		public override string ToString()
		{
			string s = this.Command;

			if(parent != null) s+= "@" + parent.Name + " ";
			if (this.hasPriority) s += ", Has Priority";
			if (this.parametersRequired) s += ", Requires Params";
			if (this.responseRequired) s += ", Requires response before " + this.timeout + "msec";
			return s;
		}

		#endregion

		#region Static Variables

		/// <summary>
		/// Gets a Regex object which can be used to check if a string is a valid command name
		/// </summary>
		private static System.Text.RegularExpressions.Regex rxCommandName = new System.Text.RegularExpressions.Regex(@"^[a-z][a-z_]{2,128}$");

		#endregion

		#region Properties estaticas

		/// <summary>
		/// Gets a Regex object which can be used to check if a string is a valid command name
		/// </summary>
		public static System.Text.RegularExpressions.Regex RxCommandName
		{
			get { return rxCommandName; }
		}

		/// <summary>
		/// A generic prototype for unrecognized commands
		/// </summary>
		public static Prototype Unrecognized
		{
			get { return new Prototype("", true, true, 0, false); }
		}


		#endregion

		#region IComparable<Prototype> Members

		int IComparable<Prototype>.CompareTo(Prototype other)
		{
			return this.command.CompareTo(other.command);
		}

		#endregion

		#region IComparable<IPrototype> Members

		int IComparable<IPrototype>.CompareTo(IPrototype other)
		{
			return this.command.CompareTo(other.Command);
		}

		#endregion
	}
}
