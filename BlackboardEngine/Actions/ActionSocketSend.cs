using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml;
using Blk.Api;

namespace Blk.Engine.Actions
{
	/// <summary>
	/// Represents an action that sends a string through a socket.
	/// </summary>
	class ActionSocketSend : IAction
	{
		
		#region Variables
		/// <summary>
		/// Delay before send the command
		/// </summary>
		private int delay;
		/// <summary>
		/// The text string to send through socket
		/// </summary>
		private string textToSend;
		/// <summary>
		/// Destinatary machine IP Address
		/// </summary>
		private IPAddress address;
		/// <summary>
		/// Socket connection port connection 
		/// </summary>
		private int port;
		/// <summary>
		/// The Module this ActionSend is bind to
		/// </summary>
		private IModule parent;

		#endregion

		#region Constructors

		/// <summary>
		/// Initiates a new instance of ActionSocketSend
		/// </summary>
		/// <param name="text">String to send throuch socket</param>
		/// <param name="address">IP Address of the destinatary machine</param>
		/// <param name="port">Connection port</param>
		/// <param name="delay">Delay before send the command</param>
		public ActionSocketSend(string text, IPAddress address, int port, int delay)
		{
			this.textToSend = text;
			this.address = address;
			this.port = port;
			this.delay = delay;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the delay before send the command
		/// </summary>
		public int Delay
		{
			get { return delay; }
		}

		/// <summary>
		/// Gets the string to be sent
		/// </summary>
		public string TextToSend
		{
			get {
				return textToSend;
			}
		}

		/// <summary>
		/// Gets the IP Address of the destinatary machine
		/// </summary>
		public IPAddress Address
		{
			get { return address; }
		}

		/// <summary>
		/// Gets the connection port
		/// </summary>
		public int Port
		{
			get { return port; }
		}

		/// <summary>
		/// Gets or sets the Module this Action is bind to
		/// </summary>
		public IModule Parent
		{
			get { return parent; }
			set
			{
				parent = value;
			}
		}	

		/// <summary>
		/// Gets the type of this IAction instance
		/// </summary>
		public ActionType Type
		{
			get { return ActionType.SocketSend; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sends the command to the module.
		/// </summary>
		/// <returns>true if the action was executed successfully. false otherwise</returns>
		public bool Execute()
		{
			return ExecuteAsync();
		}

		/// <summary>
		/// Synchronously sends the command to the module.
		/// </summary>
		/// <returns>true if the action was executed successfully. false otherwise</returns>
		private bool ExecuteSync()
		{
			SocketTcpClient client;
			try
			{
				client = new SocketTcpClient(address, port);
				if (!client.TryConnect()) return false;
				client.Send(textToSend);
				client.Disconnect();
				return true;
			}
			catch
			{
				return false;
			}
		}
		
		/// <summary>
		/// Aynchronously sends the command to the module.
		/// </summary>
		/// <returns>always true</returns>
		private bool ExecuteAsync()
		{
			Thread executionThread;
			executionThread = new Thread(new ThreadStart(delegate() { Thread.Sleep(delay); ExecuteSync(); }));
			executionThread.IsBackground = true;
			executionThread.Priority = ThreadPriority.BelowNormal;
			executionThread.Start();
			return true;
		}


		public override string ToString()
		{
			return "Send to [" + address.ToString() + ":" + port.ToString() + "]: \"" + textToSend + "\" after " + delay.ToString() + " seconds";
		}

		#endregion

		#region Static Methods

		#region Parse

		/// <summary>
		/// Converts the XML representation of a action to a ActionSend object.
		/// <param name="node">A XMLNode containing the action to convert</param>
		/// </summary>
		/// <returns>A ActionSend object that represents the action contained in s</returns>
		public static ActionSocketSend Parse(XmlNode node)
		{
			IPAddress address;
			string textToSend;
			int delay;
			int port;

			if (node.Name.ToLower() != "socketsend") throw new ArgumentException("Invalid XML node", "node");

			if ((node.Attributes["address"] == null) || !IPAddress.TryParse(node.Attributes["address"].InnerText.Trim(), out address))
				throw new ArgumentException("Invalid IP Address in XML node", "node");

			if ((node.Attributes["port"] == null) || !Int32.TryParse(node.Attributes["port"].InnerText.Trim(), out port) || (port > 65535) || (port < 1))
				throw new ArgumentException("Invalid port in XML node", "node");

			if ((node.Attributes["delay"] == null) || !Int32.TryParse(node.Attributes["delay"].InnerText.Trim(), out delay))
				delay = 0;

			if (node.Attributes["textToSend"] != null)
				textToSend = node.Attributes["textToSend"].InnerText;
			else if (node.Attributes["string"] != null)
				textToSend = node.Attributes["string"].InnerText;
			else
				textToSend = node.InnerText;
			

			return new ActionSocketSend(textToSend, address, port, delay);
		}

		#endregion

		#region TryParse

		/// <summary>
		/// Converts the XML representation of a action to a ActionSend object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="node">A XMLNode containing the action to convert</param>
		/// <param name="result">When this method returns, contains the ActionCheck equivalent to the action
		/// contained in node, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the node parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(XmlNode node, out ActionSocketSend result)
		{
			
			try
			{
				result = Parse(node);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		/// <summary>
		/// Converts the XML representation of a action to a ActionSend object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="node">A XMLNode containing the action to convert</param>
		/// <param name="result">When this method returns, contains the ActionSend equivalent to the action
		/// contained in node, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the node parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(XmlNode node, out IAction result)
		{

			try
			{
				result = (IAction)Parse(node);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		#endregion

		#endregion
	}
}
