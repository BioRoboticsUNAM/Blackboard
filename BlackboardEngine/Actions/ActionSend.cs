using System;
using System.Xml;
using Blk.Api;

namespace Blk.Engine.Actions
{
	/// <summary>
	/// Represents an action that sends a recognized command to the module.
	/// </summary>
	class ActionSend : IAction
	{

		#region Variables

		/// <summary>
		/// The command to be sent
		/// </summary>
		private Command command;
		/// <summary>
		/// The Command Name (if command has not been created)
		/// </summary>
		private string commandName;
		/// <summary>
		/// The Command Params (if command has not been created)
		/// </summary>
		private string commandParams;
		/// <summary>
		/// The Command Id (if command has not been created)
		/// </summary>
		private int commandId;
		/// <summary>
		/// The Module this ActionSend is bind to
		/// </summary>
		private IModule parent;

		#endregion

		#region Constructors

		/// <summary>
		/// Initiates a new instance of ActionSend
		/// </summary>
		/// <param name="command">The command to be sent</param>
		public ActionSend(Command command)
		{
			this.command = command;
		}

		/// <summary>
		/// Initiates a new instance of ActionSend
		/// </summary>
		/// <param name="command">Command name</param>
		/// <param name="param">Parameters of the command</param>
		/// <param name="id">id of the command</param>
		public ActionSend(string command, string param, int id)
		{
			this.commandName = command;
			this.commandParams = param;
			this.commandId = id;
			//this.command = new Command(parent, parent, this.commandName, this.commandParams, this.commandId);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the command to be sent
		/// </summary>
		public Command Command
		{
			get {
				try
				{
					if ((parent != null) && (command == null))
					{
						IModule source;
						if ((parent.Parent != null) && (parent.Parent.VirtualModule != null))
							source = parent.Parent.VirtualModule;
						else source = parent;
						this.command = new Command(source, parent, this.commandName, this.commandParams, this.commandId);
					}
				}
				catch { return null; }
				return command;
			}
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
			get { return ActionType.Send; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sends the command to the module.
		/// </summary>
		/// <returns>true if the action was executed successfully. false otherwise</returns>
		public bool Execute()
		{
			//Prototype proto;
			try
			{
				if ((parent == null) || (Command == null))
					return false;
				return Command.Send();
			}
			catch
			{
				return false;
			}
		}

		public override string ToString()
		{
			if(parent == null)
				return "Send to [UNKNOWN???] command: \"" + commandName + "\" params: \"" + commandParams + "\"";
			else if(Command == null)
				return "Send to [" + parent.ToString() + "] command: \"" + commandName + "\" params: \"" + commandParams + "\"";
			else
				return "Send to [" + parent.ToString() + "] command: \"" + Command.ToString() + "\"";
		}

		#endregion

		#region Static Methods

		#region Parse

		/// <summary>
		/// Converts the XML representation of a action to a ActionSend object.
		/// <param name="node">A XMLNode containing the action to convert</param>
		/// </summary>
		/// <returns>A ActionSend object that represents the action contained in s</returns>
		public static ActionSend Parse(XmlNode node)
		{
			string commandName;
			string commandParams;
			int commandId = -1;
			//<checkProcess processName="" programPath="" programArgs=""/>
			if (node.Name.ToLower() != "send") throw new ArgumentException("Invalid XML node", "node");

			// Full commands (require params and id). Deprecated
			//if ((node.Attributes["command"] == null) || (node.Attributes["command"].InnerText.Trim().Length < 3) ||
			//	(node.Attributes["params"] == null) || (node.Attributes["params"].InnerText.Trim().Length < 3)
			//	) throw new ArgumentException("Insuficient data in XML node", "node");

			if ((node.Attributes["command"] == null) || (node.Attributes["command"].InnerText.Trim().Length < 3))
				throw new ArgumentException("Insuficient data in XML node", "node");

			commandName = node.Attributes["command"].InnerText;
			if ((node.Attributes["params"] == null) || (node.Attributes["params"].InnerText.Trim().Length < 1))
				commandParams = "";
			else
				commandParams = node.Attributes["params"].InnerText;

			if (node.Attributes["id"] == null || !Int32.TryParse(node.Attributes["id"].InnerText, out commandId))
				commandId = -1;

			return new ActionSend(commandName, commandParams, commandId);
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
		public static bool TryParse(XmlNode node, out ActionSend result)
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
