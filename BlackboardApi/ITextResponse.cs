using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Blk.Api
{
	/// <summary>
	/// Represents a command response
	/// </summary>
	public interface ITextResponse : ITextMessage
	{
		#region Properties

		/// <summary>
		/// Gets the date and time when the response has arrived
		/// </summary>
		DateTime ArrivalTime { get; }

		/// <summary>
		/// Gets the command this Response is responding
		/// </summary>
		ITextCommand CommandResponded { get; }

		/// <summary>
		/// Gets the destination module for this response.
		/// Also allows to set when destination module is null
		/// </summary>
		new IModuleClient Destination { get; set; }

		/// <summary>
		/// Gets a value indicating if the command execution was successfull or not
		/// </summary>
		bool Success { get; }

		/// <summary>
		/// Gets the reason for which the command was not executed generating a failed response 
		/// </summary>
		ResponseFailReason FailReason { get; }

		#endregion
	}
}