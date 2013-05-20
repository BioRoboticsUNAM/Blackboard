using System;
using System.Xml;
using System.Xml.Serialization;
using Blk.Api;

namespace Blk.Engine.Remote
{
	/// <summary>
	/// Represents a remote startup request of a module program
	/// </summary>
	[XmlRootAttribute("remoteShutdownRequest", Namespace = "", IsNullable = false)]
	public class RemoteShutdownRequest
	{
		#region Variables

		/// <summary>
		/// Stores the method for shutown modules
		/// </summary>
		protected ModuleShutdownMethod method;

		/// <summary>
		/// Module name
		/// </summary>
		protected string moduleName;

		/// <summary>
		/// Stores data about the program asociated to this module
		/// </summary>
		protected ModuleProcessInfo processInfo;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of ModuleShutdownMethod
		/// </summary>
		public RemoteShutdownRequest()
		{
			this.method = ModuleShutdownMethod.None;
			this.moduleName = null;
			this.processInfo = new ModuleProcessInfo();
		}

		/// <summary>
		/// Initializes a new instance of ModuleShutdownMethod
		/// </summary>
		/// <param name="moduleName">Module name</param>
		/// <param name="method">The method for shutdown the modules</param>
		/// <param name="mpi">Data about the program asociated to this module</param>
		public RemoteShutdownRequest(string moduleName, ModuleShutdownMethod method, IModuleProcessInfo mpi)
		{
			this.method = method;
			this.moduleName = moduleName;
			if(mpi != null)
				this.processInfo = new ModuleProcessInfo(mpi.ProcessName, mpi.ProgramPath, mpi.ProgramArgs);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the method for shutdown the module
		/// </summary>
		[XmlElement("method")]
		public ModuleShutdownMethod Method
		{
			get { return this.method; }
			set { this.method = value; }
		}

		/// <summary>
		/// Gets the name of the Module
		/// </summary>
		[XmlElement("moduleName")]
		public string ModuleName
		{
			get { return moduleName; }
			set { this.moduleName = value; }
		}

		/// <summary>
		/// Gets data about the program asociated to this module
		/// </summary>
		[XmlElement("processInfo")]
		public ModuleProcessInfo ProcessInfo
		{
			get { return this.processInfo; }
			set { this.processInfo = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Serializes a RemoteShutdownRequest object to xml
		/// </summary>
		/// <param name="rsr">Object to serialize</param>
		/// <returns>string containing serialized xml object</returns>
		public static string ToXml(RemoteShutdownRequest rsr)
		{
			XmlSerializer serializer;
			XmlWriter writer;
			XmlWriterSettings settings;
			XmlSerializerNamespaces ns;
			System.Text.StringBuilder sb;

			sb = new System.Text.StringBuilder();
			ns = new XmlSerializerNamespaces();
			ns.Add(String.Empty, String.Empty);
			settings = new XmlWriterSettings();
			settings.Encoding = System.Text.Encoding.UTF8;
			settings.Indent = false;
			writer = XmlWriter.Create(sb, settings);
			serializer = new XmlSerializer(typeof(RemoteShutdownRequest));
			serializer.Serialize(writer, rsr, ns);
			writer.Flush();
			writer.Close();
			return sb.ToString();
		}

		/// <summary>
		/// Deserializes a RemoteShutdownRequest object from xml
		/// </summary>
		/// <param name="xml">String containing serialized object</param>
		/// <returns>Deserialized object</returns>
		public static RemoteShutdownRequest FromXml(string xml)
		{
			XmlSerializer serializer;
			System.IO.StringReader stream;

			stream = new System.IO.StringReader(xml);
			serializer = new XmlSerializer(typeof(RemoteShutdownRequest));
			return (RemoteShutdownRequest)serializer.Deserialize(stream);
		}

		/// <summary>
		/// Deserializes a RemoteShutdownRequest object from xml
		/// </summary>
		/// <param name="reader">Xml stream serialized object</param>
		/// <returns>Deserialized object</returns>
		public static RemoteShutdownRequest FromXml(System.Xml.XmlReader reader)
		{
			XmlSerializer serializer;

			serializer = new XmlSerializer(typeof(RemoteShutdownRequest));
			return (RemoteShutdownRequest)serializer.Deserialize(reader);
		}

		#endregion
	}
}
