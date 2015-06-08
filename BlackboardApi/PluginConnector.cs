using System;
using Robotics.API;

namespace Blk.Api
{
	/// <summary>
	/// Implements message communication with blackboard via plugin interface
	/// </summary>
	public class PluginConnector : MessageParser<ITextMessage>
	{
		#region Variables

		/// <summary>
		/// The IModuleClient intance with which this instance communicates
		/// </summary>
		private readonly IModuleClientPlugin moduleClient;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of PluginConnector
		/// </summary>
		/// <param name="moduleClient">The module client in the blackboard to connect with</param>
		public PluginConnector(IModuleClientPlugin moduleClient)
		{
			if (moduleClient == null)
				throw new ArgumentNullException();
			this.moduleClient = moduleClient;
			moduleClient.ResponseReceived += new ResponseReceivedEH(ModuleClient_ResponseReceived);
			moduleClient.CommandReceived += new CommandReceivedEH(ModuleClient_CommandReceived);
		}

		private void ModuleClient_CommandReceived(IModuleClient sender, ITextCommand c)
		{
			base.ProduceData(c);
		}

		private void ModuleClient_ResponseReceived(IModuleClient sender, ITextResponse r)
		{
			base.ProduceData(r);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the IModuleClient instance with which this instance communicates
		/// </summary>
		public IModuleClientPlugin ModuleClient
		{
			get { return this.moduleClient; }
		}

		/// <summary>
		/// Gets a value indicating if the connector is connected to the message source
		/// </summary>
		public override bool IsConnected
		{
			get { return (this.moduleClient != null) && this.moduleClient.IsRunning; }
		}

		/// <summary>
		/// Returns the ModuleName property of the asociated IConnectionManager object
		/// </summary>
		public override string ModuleName
		{
			get { return this.moduleClient.Name; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Parses a received message
		/// </summary>
		/// <param name="message">String received</param>
		protected override void Parse(ITextMessage message)
		{
			if (message is ITextCommand)
			{
				ITextCommand cmd = (ITextCommand)message;
				OnCommandReceived(new Command(this, cmd.Command, cmd.Parameters, cmd.Id));
			}
			else if (message is ITextResponse)
			{
				ITextResponse rsp = (ITextResponse)message;
				OnResponseReceived(new Response(this, rsp.Command, rsp.Parameters, rsp.Success, rsp.Id));
			}
		}

		/// <summary>
		/// Redirects a received response from the command manager
		/// </summary>
		/// <param name="response">The received response</param>
		public override void ReceiveResponse(Response response)
		{
			if (response.MessageSource != this)
				return;

		}

		/// <summary>
		/// Sends a command to the Blackboard
		/// </summary>
		/// <param name="command">Command to be sent</param>
		/// <returns>true if the command was sent, false otherwise</returns>
		public override bool Send(Command command)
		{
			if (moduleClient == null)
				return false;

			moduleClient.Receive(command.StringToSend);
			return true;
		}

		/// <summary>
		/// Sends a response to the Blackboard
		/// </summary>
		/// <param name="response">Response to be sent</param>
		/// <returns>true if the response was sent, false otherwise</returns>
		public override bool Send(Response response)
		{
			if (moduleClient == null)
				return false;
			moduleClient.Receive(response.StringToSend);
			return true;
		}

		/// <summary>
		/// Starts the parser
		/// </summary>
		public override void Start()
		{
			if (moduleClient == null)
				return;
			base.Start();
		}

		/// <summary>
		/// Stops the parser
		/// </summary>
		public override void Stop()
		{
			base.Stop();
		}

		#endregion
	}
}
