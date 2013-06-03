using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Blk.Api
{
	/// <summary>
	/// Sent Status for classes derived from CommandBase Class
	/// </summary>
	public enum SentStatus
	{
		/// <summary>
		/// The Command/Response has not been sent yet
		/// </summary>
		NotSentYet,
		/// <summary>
		/// Send operation succeded
		/// </summary>
		SentSuccessfull,
		/// <summary>
		/// Send operation failed
		/// </summary>
		SentFailed
	};

	/// <summary>
	/// Implements a command between Blackboard and Modules
	/// </summary>
	public interface ITextMessage : IComparable<ITextMessage>
	{
		#region Properties

		/// <summary>
		/// Gets the command name
		/// </summary>
		string Command { get; }

		/// <summary>
		/// Gets the destination Module of this ITextMessage
		/// </summary>
		IModuleClient Destination { get; }

		/// <summary>
		/// Gets the id of the ITextMessage
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Gets a value indicating id the ITextMessage contains params
		/// </summary>
		bool HasParams { get; }

		/// <summary>
		/// Gets the string from where this ITextMessage was build
		/// </summary>
		string OriginalString { get; }

		/// <summary>
		/// Gets the parameters of this ITextMessage
		/// </summary>
		string Parameters { get; }

		/// <summary>
		/// Prototype asociated to this ITextMessage
		/// </summary>
		IPrototype Prototype { get; }

		/// <summary>
		/// Gets the number of send attempts for this ITextMessage Instance
		/// </summary>
		int SendAttempts { get; }

		/// <summary>
		/// Stores the last sent attempt result for this ITextMessage instance
		/// </summary>
		SentStatus SentStatus { get; }

		/// <summary>
		/// Gets the source Module of this ITextMessage
		/// </summary>
		IModuleClient Source { get; }

		/// <summary>
		/// Gets a string which can be sent to a module
		/// </summary>
		string StringToSend { get; }

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