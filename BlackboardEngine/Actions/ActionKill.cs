using System;
using System.IO;
using System.Diagnostics;
using System.Xml;
using Blk.Api;

namespace Blk.Engine.Actions
{
	/// <summary>
	/// Represents an action that checks if a process is running and kills it
	/// </summary>
	internal class ActionKill : IAction
	{
		#region Variables

		/// <summary>
		/// The Module this ActionKill is bind to
		/// </summary>
		private IModuleClient parent;
		/// <summary>
		/// The name of the process for which check
		/// </summary>
		private string processName;
		/// <summary>
		/// The amount of time to wait for a process to terminate
		/// </summary>
		private int timeOut;

		#endregion

		#region Constructor

		/// <summary>
		/// Initiates a new instance of ActionKill
		/// </summary>
		/// <param name="processName">The name of the process to kill</param>
		/// <param name="timeOut"> The maximum amount of time to wait for the process to end</param>
		public ActionKill(string processName, int timeOut)
		{
			this.processName = processName;
			this.timeOut = timeOut;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the Module this Action is bind to
		/// </summary>
		public IModuleClient Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		/// <summary>
		/// Gets the name of the process to kill
		/// </summary>
		public string ProcessName
		{
			get { return processName; }
		}

		/// <summary>
		/// Gets the amount of time to wait for a process to terminate
		/// </summary>
		public int TimeOut
		{
			get { return timeOut; }
		}	

		/// <summary>
		/// Gets the type of this IAction instance
		/// </summary>
		public ActionType Type
		{
			get { return ActionType.Kill; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Checks if a process is running and kills it.
		/// </summary>
		/// <returns>true if the action was executed successfully. false otherwise</returns>
		public bool Execute()
		{
			Process[] processesToKill;
			int timeLeft;

			processesToKill = Process.GetProcessesByName(processName);
			if (processesToKill.Length < 1) return false;
			// Request close
			foreach (Process p in processesToKill)
			{
				try
				{
					p.CloseMainWindow();
					//p.Close();
				}
				catch { }
			}
			timeLeft = timeOut;

			// Wait for processes to close
			while (timeLeft > 0)
			{
				processesToKill = Process.GetProcessesByName(processName);
				if (processesToKill.Length == 0) return true;
				System.Threading.Thread.Sleep(10);
				timeLeft -= 10;
			}

			// Kill alive process
			foreach (Process p in processesToKill)
			{
				try
				{
					p.Close();
					System.Threading.Thread.Sleep(10);
					p.Kill();
				}
				catch { }
			}
			processesToKill = Process.GetProcessesByName(processName);
			return (processesToKill.Length == 0);
		}

		public override string ToString()
		{
			return "Kill [" + processName + "]";
		}

		#endregion

		#region Static Methods

		#region Parse

		/// <summary>
		/// Converts the XML representation of a action to a ActionKill object.
		/// <param name="node">A XMLNode containing the action to convert</param>
		/// </summary>
		/// <returns>A ActionKill object that represents the action contained in s</returns>
		public static ActionKill Parse(XmlNode node)
		{
			string processName;
			int timeOut;

			// <killProcess name="" timeout="100" />
			if (node.Name.ToLower() != "killprocess") throw new ArgumentException("Invalid XML node", "node");
			if(node.Attributes["processName"] == null)
				throw new ArgumentException("Insuficient data in XML node", "node");
			processName = node.Attributes["processName"].InnerText;
			if (!Int32.TryParse(node.Attributes["timeout"].InnerText, out timeOut))
				timeOut = 1000;
				//throw new ArgumentException("Invalid data in XML node", "node");
			
			return new ActionKill(processName, timeOut);
		}

		#endregion

		#region TryParse

		/// <summary>
		/// Converts the XML representation of a action to a ActionKill object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="node">A XMLNode containing the action to convert</param>
		/// <param name="result">When this method returns, contains the ActionKill equivalent to the action
		/// contained in node, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the node parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(XmlNode node, out ActionKill result)
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
		/// Converts the XML representation of a action to a ActionKill object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="node">A XMLNode containing the action to convert</param>
		/// <param name="result">When this method returns, contains the ActionKill equivalent to the action
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
