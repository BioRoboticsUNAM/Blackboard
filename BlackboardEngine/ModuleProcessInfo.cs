using System;
using System.Xml.Serialization;
using Blk.Api;

namespace Blk.Engine
{
	/// <summary>
	/// Encapsulates the data required to start a module
	/// </summary>
	[XmlRootAttribute("moduleProcessInfo", Namespace="", IsNullable=false)]
	public class ModuleProcessInfo: IModuleProcessInfo
	{
		#region Variables

		/// <summary>
		/// The name of the process asociated to this module (used for launch operations)
		/// </summary>
		private string processName;

		/// <summary>
		/// The path of the program used to launch this module (used for launch operations)
		/// </summary>
		private string programPath;

		/// <summary>
		/// The arguments used to run this module (used for launch operations)
		/// </summary>
		private string programArgs;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of ModuleExecutableInfo
		/// </summary>
		public ModuleProcessInfo()
		{
			this.processName = null;
			this.programPath = null;
			this.programArgs = null;
		}

		/// <summary>
		/// Initializes a new instance of ModuleExecutableInfo
		/// </summary>
		/// <param name="processName">The name of the process asociated to the module</param>
		/// <param name="programPath">The path of the program used to launch the module</param>
		/// <param name="programArgs">The arguments used to run the module</param>
		public ModuleProcessInfo(string processName, string programPath, string programArgs)
		{
			this.processName = processName;
			this.programPath = programPath;
			this.programArgs = programArgs;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the name of the process asociated to this module (used for launch operations)
		/// </summary>
		[XmlElement("processName")]
		public string ProcessName
		{
			get { return this.processName; }
			set { this.processName = value; }
		}

		/// <summary>
		/// Gets the path of the program used to launch this module (used for launch operations)
		/// </summary>
		[XmlElement("programPath")]
		public string ProgramPath
		{
			get { return this.programPath; }
			set { this.programPath = value; }
		}

		/// <summary>
		/// Gets the arguments used to run this module (used for launch operations)
		/// </summary>
		[XmlElement("programArgs")]
		public string ProgramArgs
		{
			get { return this.programArgs; }
			set { this.programArgs = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Serializes a ModuleProcessInfo object to xml
		/// </summary>
		/// <param name="mpi">Object to serialize</param>
		/// <returns>string containing serialized xml object</returns>
		public static string ToXml(ModuleProcessInfo mpi)
		{
			XmlSerializer serializer;
			System.IO.StringWriter stream;
			System.Text.StringBuilder sb;

			sb = new System.Text.StringBuilder();
			stream = new System.IO.StringWriter(sb);
			serializer = new XmlSerializer(typeof(ModuleProcessInfo));
			serializer.Serialize(stream, mpi, null);
			stream.Flush();
			return sb.ToString();
		}

		/// <summary>
		/// Deserializes a ModuleProcessInfo object from xml
		/// </summary>
		/// <param name="xml">String containing serialized object</param>
		/// <returns>Deserialized object</returns>
		public static ModuleProcessInfo FromXml(string xml)
		{
			XmlSerializer serializer;
			System.IO.StringReader stream;

			stream = new System.IO.StringReader(xml);
			serializer = new XmlSerializer(typeof(ModuleProcessInfo));
			return (ModuleProcessInfo)serializer.Deserialize(stream);
		}

		/// <summary>
		/// Deserializes a ModuleProcessInfo object from xml
		/// </summary>
		/// <param name="reader">Xml stream serialized object</param>
		/// <returns>Deserialized object</returns>
		public static ModuleProcessInfo FromXml(System.Xml.XmlReader reader)
		{
			XmlSerializer serializer;

			serializer = new XmlSerializer(typeof(ModuleProcessInfo));
			return (ModuleProcessInfo)serializer.Deserialize(reader);
		}

		#endregion
	}
}
