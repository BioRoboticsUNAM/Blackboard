using System;
using System.IO;
using System.Diagnostics;
using System.Xml;
//using System.Xml.Schema;
//using System.Xml.Serialization;
using Blk.Api;

namespace Blk.Engine.Actions
{
	/// <summary>
	/// Represents an action that checks if a process is running.
	/// If not running then the process is launched.
	/// </summary>
	internal class ActionCheck : IAction
	{
		#region Variables

		/// <summary>
		/// The Module this ActionCheck is bind to
		/// </summary>
		private IModuleClient parent;
		/// <summary>
		/// The name of the process for which check
		/// </summary>
		private string processName;
		/// <summary>
		/// The name of the file to run
		/// </summary>
		private string exePath;
		/// <summary>
		/// The arguments to provide to the executable
		/// </summary>
		private string arguments;
		/// <summary>
		/// The priority to start the process with
		/// </summary>
		private ProcessPriorityClass priority;

		#endregion

		#region Constructors

		/// <summary>
		/// Initiates a new instance of ActionCheck
		/// </summary>
		/// <param name="processName">The name of the process for which check</param>
		/// <param name="exePath">The name of the executable file to run</param>
		/// <param name="arguments"> The arguments to provide to the executable</param>
		public ActionCheck(string processName, string exePath, string arguments)
		{
			this.processName = processName;
			if (File.Exists(exePath))
			{
				this.exePath = (new FileInfo(exePath)).FullName;
			}
			this.arguments = arguments;
		}

		/// <summary>
		/// Initiates a new instance of ActionCheck
		/// </summary>
		/// <param name="processName">The name of the process for which check</param>
		/// <param name="exePath">The name of the executable file to run</param>
		/// <param name="arguments"> The arguments to provide to the executable</param>
		/// <param name="priority">The priority to start the process with</param>
		public ActionCheck(string processName, string exePath, string arguments, ProcessPriorityClass priority) : this(processName, exePath, arguments)
		{
			this.priority = priority;
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
		public IModuleClient Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		/// <summary>
		/// Gets the name of the process to check for
		/// </summary>
		public string ProcessName
		{
			get { return processName; }
		}		

		/// <summary>
		/// Gets the type of this IAction instance
		/// </summary>
		public ActionType Type
		{
			get { return ActionType.Check; }
		}

		/// <summary>
		/// Gets the priority to start the process with
		/// </summary>
		public ProcessPriorityClass Priority
		{
			get { return priority; }
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
			//bool running;
			Process[] processList = Process.GetProcesses();
			foreach (Process p in processList)
			{
				if (p.ProcessName == processName)
					return true;//running = true;
			}
			if (!File.Exists(exePath)) return false;
			try
			{
				Process process;
				ProcessStartInfo psi;
				psi = new ProcessStartInfo(exePath, arguments);
				psi.WorkingDirectory = (new FileInfo(exePath)).DirectoryName;
				process = new Process();
				process.StartInfo = psi;
				//process.PriorityClass = priority;
				process.Start();
				process.PriorityClass = priority;
				process.WaitForInputIdle(2000);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public override string ToString()
		{
			return "Check for process [" + processName + "] if not running execute \"" + exePath + " " + arguments + "\"";
		}
		
		/*

		/// <summary>
		/// Returns null
		/// </summary>
		/// <returns>null</returns>
		public XmlSchema GetSchema()
		{
			return null;
		}
		
		/// <summary>
		/// Generates an object from its XML representation
		/// </summary>
		/// <param name="reader">The XmlReader stream from which the object is deserialized.</param>
		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement("checkprocess");
		}

		/// <summary>
		/// Converts the object into its XML representation
		/// </summary>
		/// <param name="writer">The XmlWriter stream to which the object is serialized.</param>
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("checkprocess");
			writer.WriteAttributeString("processName", this.processName);
			writer.WriteAttributeString("programPath", this.exePath);
			writer.WriteAttributeString("programArgs", this.arguments);
			writer.WriteEndElement();
		}
		*/

		#endregion


		#region Static Methods

		#region Parse

		/// <summary>
		/// Converts the XML representation of a action to a ActionCheck object.
		/// <param name="node">A XMLNode containing the action to convert</param>
		/// </summary>
		/// <returns>A ActionCheck object that represents the action contained in s</returns>
		public static ActionCheck Parse(XmlNode node)
		{
			string processName;
			string programPath;
			string programArgs;
			int priority;
			ProcessPriorityClass processPriority;
			//<checkProcess processName="" programPath="" programArgs=""/>
			if (node.Name.ToLower() != "checkprocess") throw new ArgumentException("Invalid XML node", "node");
			if((node.Attributes["processName"] == null) ||
				(node.Attributes["programPath"] == null)
				) throw new ArgumentException("Insuficient data in XML node", "node");
			processName = node.Attributes["processName"].InnerText;
			programPath = node.Attributes["programPath"].InnerText;
			if (node.Attributes["programArgs"] != null)
				programArgs = node.Attributes["programArgs"].InnerText;
			else programArgs = "";
			if ((node.Attributes["priority"] == null) || Int32.TryParse(node.Attributes["priority"].InnerText, out priority) || (priority < 0) || (priority > 5))
				priority = 3;

			switch (priority)
			{
				case 0: processPriority = ProcessPriorityClass.RealTime; break;
				case 1: processPriority = ProcessPriorityClass.High; break;
				case 2: processPriority = ProcessPriorityClass.BelowNormal; break;
				case 3: processPriority = ProcessPriorityClass.Normal; break;
				case 4: processPriority = ProcessPriorityClass.BelowNormal; break;
				case 5: processPriority = ProcessPriorityClass.Idle; break;
				default: processPriority = ProcessPriorityClass.Normal; break;
			}
			
			if (!File.Exists(programPath)) throw new FileNotFoundException("Can't locate program path file", programPath);
			return new ActionCheck(processName, programPath, programArgs, processPriority);
		}

		#endregion

		#region TryParse

		/// <summary>
		/// Converts the XML representation of a action to a ActionCheck object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="node">A XMLNode containing the action to convert</param>
		/// <param name="result">When this method returns, contains the ActionCheck equivalent to the action
		/// contained in node, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the node parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(XmlNode node, out ActionCheck result)
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
		/// Converts the XML representation of a action to a ActionCheck object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="node">A XMLNode containing the action to convert</param>
		/// <param name="result">When this method returns, contains the ActionCheck equivalent to the action
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
