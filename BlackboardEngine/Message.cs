using System;
using System.Collections.Generic;
using System.Text;
using Blk.Api;

namespace Blk.Engine
{

	/// <summary>
	/// Represents a message
	/// </summary>
	public interface IMessage
	{

		#region Properties

		/*

		/// <summary>
		/// Gets the command name
		/// </summary>
		public virtual string Command
		{
			get { return command; }
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
		public virtual Prototype Prototype
		{
			get { return prototype; }
		}

		*/

		/// <summary>
		/// Gets the number of send attempts for this CommandBase Instance
		/// </summary>
		int SendAttempts { get; /* set; */ }

		/// <summary>
		/// Stores the last sent attempt result for this CommandBase instance
		/// </summary>
		SentStatus SentStatus { get; /* set;*/ }

		/// <summary>
		/// Gets the destination Module of this command
		/// </summary>
		IModule Destination { get; }
		
		/// <summary>
		/// Gets the source Module of this command
		/// </summary>
		IModule Source { get; }
		
		#endregion

		#region Methods

		/// <summary>
		/// Sends itself trough its Destination IModule
		/// </summary>
		/// <returns>true if the command has been sent, false otherwise</returns>
		bool Send();

		#endregion
	}
}
