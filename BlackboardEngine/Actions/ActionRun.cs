using System;
using System.IO;
using System.Diagnostics;
using System.Xml;
using Blk.Api;

namespace Blk.Engine.Actions
{
	/// <summary>
	/// Represents an action that executes a process
	/// </summary>
	class ActionRun : IAction
	{
		#region Variables

		/// <summary>
		/// The Module this ActionRun is bind to
		/// </summary>
		private IModule parent;
		/// <summary>
		/// The name of the file to run
		/// </summary>
		private string exePath;
		/// <summary>
		/// The arguments to provide to the executable
		/// </summary>
		private string arguments;

		#endregion

		#region Constructors

		/// <summary>
		/// Initiates a new instance of ActionRun
		/// </summary>
		/// <param name="exePath">The name of the executable file to run</param>
		/// <param name="arguments"> The arguments to provide to the executable</param>
		public ActionRun(string exePath, string arguments)
		{
			if (File.Exists(exePath))
			{
				this.exePath = (new FileInfo(exePath)).FullName;
			}
			this.arguments = arguments;
		}

		#endregion

		#region Properties


		/// <summary>
		/// Gets the argument string to provide to the executable
		/// </summary>
		public string Arguments
		{
			get { return arguments; }
		}

		/// <summary>
		/// Gets the path of the file to run
		/// </summary>
		public string ExePath
		{
			get { return exePath; }
		}

		/// <summary>
		/// Gets or sets the Module this Action is bind to
		/// </summary>
		public IModule Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		/// <summary>
		/// Gets the type of this IAction instance
		/// </summary>
		public ActionType Type
		{
			get { return ActionType.Check; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Checks if a process is running.
		/// If not running then the process is launched.
		/// </summary>
		/// <returns>true if the action was executed successfully. false otherwise</returns>
		public bool Execute()
		{
			ProcessStartInfo psi;
			Process p;
			bool result = false;
			if (!File.Exists(exePath)) return result;
			try
			{
				psi = new ProcessStartInfo(exePath, arguments);
				psi.WorkingDirectory = (new FileInfo(exePath)).DirectoryName;
				psi.UseShellExecute = true;
				psi.WindowStyle = ProcessWindowStyle.Normal;
				p = new Process();
				p.StartInfo = psi;
				result = p.Start();
				p.WaitForInputIdle(2000);
				return result;
			}
			catch { return result; }
		}

		public override string ToString()
		{
			return "Execute \"" + exePath + "\"";
		}

		#endregion

		#region Static Methods

		#region Parse

		/// <summary>
		/// Converts the XML representation of a action to a ActionCheck object.
		/// <param name="node">A XMLNode containing the action to convert</param>
		/// </summary>
		/// <returns>A ActionCheck object that represents the action contained in s</returns>
		public static ActionRun Parse(XmlNode node)
		{
			string programPath;
			string programArgs;
			//<checkProcess processName="" programPath="" programArgs=""/>
			if (node.Name.ToLower() != "run") throw new ArgumentException("Invalid XML node", "node");
			if((node.Attributes["programPath"] == null) ||
				(node.Attributes["programArgs"] == null)
				) throw new ArgumentException("Insuficient data in XML node", "node");
			programPath = node.Attributes["programPath"].InnerText;
			programArgs = node.Attributes["programArgs"].InnerText;
			
			if (!File.Exists(programPath)) throw new FileNotFoundException("Can't locate program path file", programPath);
			return new ActionRun(programPath, programArgs);
		}

		#endregion

		#region TryParse

		/// <summary>
		/// Converts the XML representation of a action to a ActionRun object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="node">A XMLNode containing the action to convert</param>
		/// <param name="result">When this method returns, contains the ActionRun equivalent to the action
		/// contained in node, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the node parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(XmlNode node, out ActionRun result)
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
		/// Converts the XML representation of a action to a ActionRun object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="node">A XMLNode containing the action to convert</param>
		/// <param name="result">When this method returns, contains the ActionRun equivalent to the action
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
