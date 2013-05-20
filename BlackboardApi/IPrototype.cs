using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Blk.Api
{
	/// <summary>
	/// Represents a prototype for commands
	/// </summary>
	public interface IPrototype : IComparable<IPrototype>
	{
		#region Properties

		/// <summary>
		/// Name of the command that this prototype represent 
		/// </summary>
		string Command { get; }

		/// <summary>
		/// Gets a value indicating if the command has priority over other commands
		/// </summary>
		bool HasPriority { get; }

		/// <summary>
		/// Gets a value indicating if parameters are required
		/// </summary>
		bool ParamsRequired { get; }

		/// <summary>
		/// True if a response is required, false otherwise
		/// </summary>
		bool ResponseRequired { get; }

		/// <summary>
		/// The maximum amount of time to wait for a response in milliseconds
		/// </summary>
		int Timeout { get; }

		/// <summary>
		/// Gets the Module this prototype is bind to
		/// </summary>
		IModule Parent { get; set; }

		#endregion
	}
}
