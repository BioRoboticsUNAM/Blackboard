using System;
using System.Collections.Generic;
using System.Net;

namespace Blk.Api
{
	#region Delegates

	/// <summary>
	/// Represents the method that will handle the CommandReceived event of a IModule object.
	/// </summary>
	/// <param name="sender">The module where the command has arrived</param>
	/// <param name="c">Command received</param>
	public delegate void CommandReceivedEH(IModuleClient sender, ITextCommand c);

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

	/// <summary>
	/// Provides the base connection interface for modules.
	/// </summary>
	public interface IModuleClient : IComparable<IModuleClient>
	{
		#region Properties

		/// <summary>
		/// Gets or sets an alias for the module
		/// </summary>
		string Alias { get; set; }

		/// <summary>
		/// Gets or sets a value indicating if the current IModule supports binnary commands
		/// </summary>
		bool Bin { get; }

		/// <summary>
		/// Gets a value indicating if the current module is busy (waiting for a response)
		/// </summary>
		bool Busy { get; }

		/// <summary>
		/// Gets or sets a value indicating if the module is enabled.
		/// If module is disabled can not be started and will be ignored by the Blackboard.
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// Gets the idle time of the module
		/// </summary>
		/// <remarks>The idle time of a module is the amount of time elapsed since the last activity 
		/// of the module. Tipucally this time reflects the time elapsed since the last received command/response
		/// If the module is busy the idle time is TimeSpan.Zero</remarks>
		TimeSpan IdleTime { get; }

		/// <summary>
		/// Tells if the IModule is responding and working
		/// </summary>
		bool IsAlive { get; }

		/// <summary>
		/// Tells if the connection to the IModule has been stablished or started
		/// </summary>
		bool IsConnected { get; }

		/// <summary>
		/// Gets a value indicating if the module is running in the same machine as the blackboard
		/// </summary>
		bool IsLocal { get; }

		/// <summary>
		/// Tells if the the IModule is running
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// Gets the name of the Module
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the blackboard this IModule is bind to
		/// </summary>
		IBlackboard Parent { get; set; }

		/// <summary>
		/// Gets data about the program asociated to this module
		/// </summary>
		IModuleProcessInfo ProcessInfo { get; }

		/// <summary>
		/// Gets the prototypes managed by the Module
		/// </summary>
		IPrototypeCollection Prototypes { get; }

		/// <summary>
		/// Tells if the the IModule is ready for normal operation
		/// </summary>
		bool Ready { get; }

		/// <summary>
		/// Gets a value ingicating if the Module require the "SENDER DESTINATION" prefix before the command name
		/// </summary>
		bool RequirePrefix { get; }

		/// <summary>
		/// Gets or sets the delay between send operations in milliseconds
		/// </summary>
		/// <remarks>
		/// A negative value disables the delay
		/// A zero value postpones the send operation untill the next execution of the thread
		/// The maximum value is 300
		/// </remarks>
		int SendDelay { get; set; }

		/// <summary>
		/// Gets or sets a value indicating which mode of simulation is active
		/// </summary>
		IModuleSimulationOptions Simulation { get; }

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the IsAlive property of a IModule object changes its value
		/// </summary>
		event StatusChangedEH AliveChanged;

		/// <summary>
		/// Occurs when the Busy property of a IModule object changes its value
		/// </summary>
		event StatusChangedEH BusyChanged;

		/// <summary>
		/// Occurs when a Command is received trough socket
		/// </summary>
		event CommandReceivedEH CommandReceived;

		/// <summary>
		/// Occurs when a Response is received trough socket
		/// </summary>
		event ResponseReceivedEH ResponseReceived;

		/// <summary>
		/// Occurs when the Ready property of a IModule object changes its value
		/// </summary>
		event StatusChangedEH ReadyChanged;

		/// <summary>
		/// Occurs when the status of a IModule object changes
		/// </summary>
		event StatusChangedEH StatusChanged;

		/// <summary>
		/// Occurs when the status of a IModule object starts working
		/// </summary>
		event StatusChangedEH Started;

		/// <summary>
		/// Occurs when the status of a IModule object stops working
		/// </summary>
		event StatusChangedEH Stopped;

		#endregion

		#region Methods

		/// <summary>
		/// Asynchronously stops the socket connection and command management system.
		/// If the IModuleConnector is not running, it has no effect.
		/// </summary>
		void BeginStop();

		/// <summary>
		/// Starts the socket connection and command management system.
		/// If the IModuleConnector is already running, it has no effect.
		/// </summary>
		void Start();

		/// <summary>
		/// Stops the socket connection and command management system.
		/// If the IModuleConnector is not running, it has no effect.
		/// </summary>
		void Stop();

		/// <summary>
		/// Sends a command to the module
		/// </summary>
		/// <param name="command">Response to send</param>
		/// <returns>true if the command has been sent, false otherwise</returns>
		bool Send(ITextCommand command);

		/// <summary>
		/// Synchronusly sends a command response to the module
		/// </summary>
		/// <param name="response">Response to send</param>
		/// <returns>true if the response has been sent, false otherwise</returns>
		bool Send(ITextResponse response);

		/// <summary>
		/// Blocks the thread call untill the module becomes ready
		/// </summary>
		void WaitReady();

		/// <summary>
		/// Blocks the thread call untill the module becomes ready or the specified time elapses
		/// </summary>
		/// <param name="timeout">The amount of time in milliseconds to wait the module become ready</param>
		void WaitReady(int timeout);

		#endregion
	}
}
