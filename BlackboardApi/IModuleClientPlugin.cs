using System;
using System.Collections.Generic;
using System.Text;

namespace Blk.Api
{
	/// <summary>
	/// Represents a ModuleClient for interfacing module applications running as plugins
	/// </summary>
	public interface IModuleClientPlugin : IModuleClient
	{
		/// <summary>
		/// Receives a message and enqueues it for processing and redirection
		/// </summary>
		/// <param name="message">The message to receive</param>
		void Receive(string message);
	}
}
