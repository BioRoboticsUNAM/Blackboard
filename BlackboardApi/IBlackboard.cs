using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;

using Robotics;
using Robotics.Utilities;

namespace Blk.Api
{
	#region Delegates
	/// <summary>
	/// Represents the method that will handle the StatusChanged event of a Blackboard object.
	/// </summary>
	/// <param name="blackboard">Blackboard which raised the event</param>
	public delegate void BlackboardStatusChangedEH(IBlackboard blackboard);
	/// <summary>
	/// Represents the method that will handle the MessageReceived and MessageSent event of a Blackboard object.
	/// </summary>
	/// <param name="message">Message received/sent</param>
	public delegate void MessageIOEH(ITextMessage message);
	/// <summary>
	/// Represents the method that will handle the ClientConnected and ClientDisconnected event of a Blackboard object.
	/// </summary>
	/// <param name="ip">The ip Address of the client</param>
	public delegate void BlackboardClientCnnEH(IPAddress ip);
	/// <summary>
	/// Represents the method that will handle the ClientConnected and ClientDisconnected event of a Blackboard object.
	/// </summary>
	/// <param name="command">Command tried to be executed</param>
	/// <param name="response">Response to command redirected</param>
	/// <param name="sendResponseSuccess">Indicates if the response was sent successfully</param>
	public delegate void ResponseRedirectedEH(ITextCommand command, ITextResponse response, bool sendResponseSuccess);
	#endregion

	#region Enumerations

	/// <summary>
	/// Enumerates the running status of the blackboard
	/// </summary>
	public enum BlackboardRunningStatus
	{
		/// <summary>
		/// Blackboard is stopped
		/// </summary>
		Stopped,
		/// <summary>
		/// Blackboard is running normally
		/// </summary>
		Running,
		/// <summary>
		/// Blackboard is starting up modules
		/// </summary>
		Starting,
		/// <summary>
		/// Blackboard is stopping off modules
		/// </summary>
		Stopping,
		/// <summary>
		/// Blackboard is restarting modules
		/// </summary>
		Restarting,
		/// <summary>
		/// Blackboard is restarting the test
		/// </summary>
		RestartingTest
	}

	#endregion

	/// <summary>
	/// Implements a blackboard
	/// </summary>
	public interface IBlackboard : IService
	{
		#region Properties

		/// <summary>
		/// Gets or sets the amount of time to wait before stop automatically the blackboard
		/// </summary>
		TimeSpan AutoStopTime { get; set; }

		/// <summary>
		/// Gets the remaining time for AutoStop
		/// </summary>
		TimeSpan AutoStopTimeLeft { get; }

		/// <summary>
		/// Gets the number of clients connected to de Blackboard Server
		/// </summary>
		int ClientsConnected { get; }

		/// <summary>
		/// Gets or sets a textwriter to write in all operations
		/// </summary>
		ILogWriter Log { get; set; }

		/// <summary>
		/// Gets the modules managed by the Blackboard
		/// </summary>
		IModuleCollection Modules { get; }

		/// <summary>
		/// Gets an array of commands pending to send
		/// </summary>
		ITextCommand[] PendingCommands { get; }

		/// <summary>
		/// Gets the plugins which interacts with the Blackboard
		/// </summary>
		IBlackboardPluginManager PluginManager { get; }

		/// <summary>
		/// Gets the port when the blackboard accept incomming connections
		/// </summary>
		int Port { get; set; }

		/// <summary>
		/// Gets the Runnin Status of this blackboard instance
		/// </summary>
		BlackboardRunningStatus RunningStatus { get; }

		/// <summary>
		/// Gets or sets the the virtual module that handles blackboard commands
		/// </summary>
		IModuleBlackboard VirtualModule { get; }

		/// <summary>
		/// Gets or sets the number of attempts while redirecting a response
		/// </summary>
		int SendAttempts { get; set; }

		/*
		/// <summary>
		/// Gets the ShutdownSequence object used to terminate the module programs
		/// </summary>
		ShutdownSequenceManager ShutdownSequence { get; }

		/// <summary>
		/// Gets the StartupSequence object used to execute the module programs
		/// </summary>
		StartupSequenceManager StartupSequence { get; }
		*/

		/// <summary>
		/// Gets the remaining time for perform Test-Timeout actions
		/// </summary>
		TimeSpan TestTimeLeft { get; }

		/// <summary>
		/// Gets or sets the amount of time to wait before perform Test-Timeout actions
		/// </summary>
		TimeSpan TestTimeOut { get; set; }

		/// <summary>
		/// Gets or sets the time delay between loads of each module in milliseconds
		/// </summary>
		int ModuleLoadDelay { get; set; }

		/// <summary>
		/// Gets or sets how uch data is sent to the console
		/// </summary>
		int VerbosityLevel { get; set; }

		/// <summary>
		/// Gets an array of commands waiting for response
		/// </summary>
		ITextCommand[] WaitingCommands{ get; }

		#endregion

		#region Events

		/// <summary>
		/// Raises when a client connects to Blackboard TCPServer
		/// </summary>
		event TcpClientConnectedEventHandler ClientConnected;

		/// <summary>
		/// Raises when a client disconnects from Blackboard TCPServer
		/// </summary>
		event TcpClientDisconnectedEventHandler ClientDisconnected;

		/*
		/// <summary>
		/// Raises when a Blackboard Module connects to the remote module
		/// </summary>
		event ModuleConnectionEH Connected;

		/// <summary>
		/// Raises when a Blackboard Module disconnects from the remote module
		/// </summary>
		event ModuleConnectionEH Disconnected;
		*/

		/// <summary>
		/// Raises when this blackboard changes its status
		/// </summary>
		event BlackboardStatusChangedEH StatusChanged;

		/// <summary>
		/// Raises when a Response for a received command is redirected
		/// </summary>
		event ResponseRedirectedEH ResponseRedirected;

		#endregion

		#region Methods

		/// <summary>
		/// Asynchronously starts the Blackboard.
		/// If the Blackboard is running, it has no effect.
		/// </summary>
		/// <returns></returns>
		void BeginStart();

		/// <summary>
		/// Asynchronously stops the Blackboard.
		/// If the Blackboard is not running, it has no effect.
		/// </summary>
		void BeginStop();

		/// <summary>
		/// Restarts the blackboard.
		/// This action restart the timers
		/// </summary>
		void Restart();

		/// <summary>
		/// Requests to restart the test.
		/// This action does not restart the timers
		/// </summary>
		void RestartTest();

		/// <summary>
		/// Sets the startup time to current time
		/// </summary>
		void RestartTimer();

		/// <summary>
		/// Injects a message to the blackboard unparsed message queue
		/// </summary>
		/// <param name="textToInject">Message string to inject</param>
		/// <returns>true if injection was successfull, false otherwise</returns>
		bool Inject(string textToInject);

		#region Blackboard Main Tasks

		/// <summary>
		/// Look for a module in the blackboard that supports specified command
		/// </summary>
		/// <param name="commandName">The name of the command to look for</param>
		/// <param name="destination">When this method returns, contains the Module that supports the command
		/// specified, if the conversion succeeded, or null if no module supports the command.
		/// The search fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if a destination module was found, false otherwise</returns>
		bool FindDestinationModule(string commandName, out IModule destination);

		/// <summary>
		/// Look for a module in the blackboard that supports specified command
		/// </summary>
		/// <param name="commandName">The name of the command to look for</param>
		/// <param name="destination">When this method returns, contains the Module that supports the command
		/// specified, if the conversion succeeded, or null if no module supports the command.
		/// The search fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <param name="prototype">When this method returns, contains the Prototype for the command
		/// specified, if the conversion succeeded, or null if no module supports the command.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// <returns>true if a destination module was found, false otherwise</returns>
		bool FindDestinationModule(string commandName, out IModule destination, out IPrototype prototype);

		#endregion

		#endregion
	}
}
