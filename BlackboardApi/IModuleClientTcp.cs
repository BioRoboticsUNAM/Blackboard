using System;
using System.Collections.Generic;
using System.Net;

namespace Blk.Api
{
	/// <summary>
	/// Represents a blackboard module interface which connects to a remote module via TCP/IP
	/// </summary>
	public interface IModuleClientTcp : IModuleClient
	{
		#region Properties

		/// <summary>
		/// Gets the port where to connect to the Application Module
		/// </summary>
		int Port { get; }

		/// <summary>
		/// Gets the IP Address of Application Module's computer
		/// </summary>
		IPAddress ServerAddress { get; }

		/// <summary>
		/// Gets a collection of IP Addresses of the computers where the Application Module's can be running
		/// </summary>
		IServerAddressCollection ServerAddresses { get; }

		#endregion

		#region Methods
		#endregion
	}
}
