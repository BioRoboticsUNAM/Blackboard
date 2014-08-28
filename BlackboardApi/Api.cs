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
	/// <param name="ip">The ip Address of the client</param>
	public delegate void BlackboardClientCnnEH(IPAddress ip);

	/// <summary>
	/// Represents the method that will handle the StatusChanged event of a Blackboard object.
	/// </summary>
	/// <param name="blackboard">Blackboard which raised the event</param>
	public delegate void BlackboardStatusChangedEH(IBlackboard blackboard);

	/// <summary>
	/// Represents the method that will handle the PrototypeCollectionStatusChanged event of a IPrototypeCollection object.
	/// </summary>
	/// <param name="collection">Collection which raised the event</param>
	public delegate void PrototypeCollectionStatusChangedEH(IPrototypeCollection collection);

	/// <summary>
	/// Represents the method that will handle the CommandReceived event of a IModule object.
	/// </summary>
	/// <param name="sender">The module where the command has arrived</param>
	/// <param name="c">Command received</param>
	public delegate void CommandReceivedEH(IModuleClient sender, ITextCommand c);

	/// <summary>
	/// Represents the method that will handle the ModuleAdded and ModuleRemoved event of a ModuleCollection object.
	/// </summary>
	/// <param name="module"></param>
	public delegate void IModuleAddRemoveEH(IModuleClient module);

	/// <summary>
	/// Represents the method that will handle the MessageReceived and MessageSent event of a Blackboard object.
	/// </summary>
	/// <param name="message">Message received/sent</param>
	public delegate void MessageIOEH(ITextMessage message);

	/// <summary>
	/// Represents the method that will handle the ClientConnected and ClientDisconnected event of a Blackboard object.
	/// </summary>
	/// <param name="command">Command tried to be executed</param>
	/// <param name="response">Response to command redirected</param>
	/// <param name="sendResponseSuccess">Indicates if the response was sent successfully</param>
	public delegate void ResponseRedirectedEH(ITextCommand command, ITextResponse response, bool sendResponseSuccess);

	/// <summary>
	/// Represents the method that will handle the ResponseReceived event of a IModule object.
	/// </summary>
	/// <param name="sender">The module where the response has arrived</param>
	/// <param name="r">Response received</param>
	public delegate void ResponseReceivedEH(IModuleClient sender, ITextResponse r);

	/// <summary>
	/// Represent the method that will handle the change of the status of a IModule object.
	/// </summary>
	/// <param name="sender">The IModule which status has changed</param>
	public delegate void StatusChangedEH(IModuleClient sender);

	#endregion
}