using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Blk.Api
{
	/// <summary>
	/// Represents a command
	/// </summary>
	public interface ITextCommand : ITextMessage
	{
		#region Properties

		/// <summary>
		/// Gets or Sets the time when the command was sent.
		/// Initially contains the time when the command was created
		/// </summary>
		DateTime SentTime { get; set; }

		/// <summary>
		/// Gets the time elapsed since the command was created or sent
		/// </summary>
		TimeSpan Elapsed { get; }

		/// <summary>
		/// Gets the amount of time left to wait for a response
		/// </summary>
		TimeSpan TimeLeft { get; }

		/// <summary>
		/// Gets the amount of milliseconds left to wait for a response
		/// </summary>
		int MillisecondsLeft { get; }

		/// <summary>
		/// Gets or sets the response asociated to this Command object
		/// </summary>
		ITextResponse Response { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Gets a value indicating if provided Response is a response for current command
		/// </summary>
		/// <param name="response">Response to check</param>
		/// <returns>true if provided Response is a response for command, false otherwise</returns>
		bool IsMatch(ITextResponse response);

		/// <summary>
		/// Asignates the provided Response as Response for the current Command and this Command as CommandResponded for the provided response.
		/// After asignation the Response is sent to its Destination module which must match this Command Source module
		/// </summary>
		/// <param name="response">Response to asign</param>
		/// <returns>true if response was asigned and sent successfully, false otherwise</returns>
		bool SendResponse(ITextResponse response);

		#endregion
	}
}