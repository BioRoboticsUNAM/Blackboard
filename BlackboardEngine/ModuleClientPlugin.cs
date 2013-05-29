using System;
using Blk.Api;
using Robotics.API;

namespace Blk.Engine
{
	/// <summary>
	/// Provides connection fro Plugin modules
	/// </summary>
	public class ModuleClientPlugin : ModuleClient
	{
		#region Variables

		private ModulePlugin modulePlugin;

		#endregion

		#region Constructor

		public ModuleClientPlugin(ModulePlugin modulePlugin)
			: base(modulePlugin.Name)
		{
			this.modulePlugin = modulePlugin;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating if the module is running in the same machine as the blackboard
		/// </summary>
		public override bool IsLocal
		{
			get { return true; }
		}

		/// <summary>
		/// Tells if the connection to the remote module application has been stablished
		/// </summary>
		public override bool IsConnected
		{
			get { return this.modulePlugin != null; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sends a command to the module
		/// </summary>
		/// <param name="command">Response to send</param>
		/// <returns>true if the command has been sent, false otherwise</returns>
		public override bool Send(Command command)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Synchronusly sends a command response to the module
		/// </summary>
		/// <param name="response">Response to send</param>
		/// <returns>true if the response has been sent, false otherwise</returns>
		public override bool Send(Response response)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sends the provided text string through socket client
		/// </summary>
		/// <param name="stringToSend">string to send through socket</param>
		protected override bool Send(string stringToSend)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Does nothing (internal module, no connection required)
		/// </summary>
		protected override void MainThreadDisconnect()
		{
		}

		/// <summary>
		/// Does nothing (internal module, no connection required)
		/// </summary>
		protected override void MainThreadFirstTimeConnect()
		{
		}

		/// <summary>
		/// Does nothing (internal module, no connection required)
		/// </summary>
		protected override void MainThreadLoopAutoConnect()
		{
		}

		#endregion
	}
}
