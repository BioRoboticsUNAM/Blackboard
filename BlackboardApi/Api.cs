using System;
using System.Net;
using Robotics;

namespace Blk.Api
{
	#region	Enumerations

	///	<summary>
	///	Enumerates the running status of the blackboard
	///	</summary>
	public enum BlackboardRunningStatus
	{
		///	<summary>
		///	Blackboard is stopped
		///	</summary>
		Stopped,
		///	<summary>
		///	Blackboard is running normally
		///	</summary>
		Running,
		///	<summary>
		///	Blackboard is starting up modules
		///	</summary>
		Starting,
		///	<summary>
		///	Blackboard is stopping off modules
		///	</summary>
		Stopping,
		///	<summary>
		///	Blackboard is restarting modules
		///	</summary>
		Restarting,
		///	<summary>
		///	Blackboard is restarting the test
		///	</summary>
		RestartingTest
	}

	/// <summary>
	/// Enumerates the reasons for which a command was not executed generating a failed response 
	/// </summary>
	public enum ResponseFailReason
	{
		/// <summary>
		/// The response was succeded
		/// </summary>
		None,
		/// <summary>
		/// The command was executed in the destination module but was not succeded
		/// </summary>
		ExecutedButNotSucceded,
		/// <summary>
		/// Generated failed response due to timeout
		/// </summary>
		TimedOut,
		/// <summary>
		/// The module is disconected
		/// </summary>
		ModuleDisconnected,
		/// <summary>
		/// Blackboard is restarting. Message has not sent
		/// </summary>
		BlackboardRestarting,
		/// <summary>
		/// Blackboard is restarting the test. Message has not sent
		/// </summary>
		BlackboardRestartingTest,
		/// <summary>
		/// Unknown reason
		/// </summary>
		Unknown
	}

	///	<summary>
	///	Sent Status	for	classes	derived	from CommandBase Class
	///	</summary>
	public enum	SentStatus
	{
		///	<summary>
		///	The	Command/Response has not been sent yet
		///	</summary>
		NotSentYet,
		///	<summary>
		///	Send operation succeded
		///	</summary>
		SentSuccessfull,
		///	<summary>
		///	Send operation failed
		///	</summary>
		SentFailed
	}

	/// <summary>
	/// Enumerates the simulations types a Module support
	/// </summary>
	//public enum SimulationOptions { SimulationDisabled, EnabledExact, EnabledWithError };
	public enum SimulationType
	{
		/// <summary>
		/// Simulation is disabled
		/// </summary>
		SimulationDisabled = 0,
		/// <summary>
		/// Simulates and responses match the commands successfully
		/// </summary>
		EnabledExact,
		/// <summary>
		/// Simulates and responses match the commands
		/// </summary>
		EnabledWithError
	}

	#endregion

	#region Delegates

	/// <summary>
	/// Represents the method that will handle the ClientConnected and ClientDisconnected event of a Blackboard object.
	/// </summary>
	/// <param name="command">Command tried to be executed</param>
	/// <param name="response">Response to command redirected</param>
	/// <param name="sendResponseSuccess">Indicates if the response was sent successfully</param>
	public delegate void ResponseRedirectedEH(ITextCommand command, ITextResponse response, bool sendResponseSuccess);

	#endregion
}