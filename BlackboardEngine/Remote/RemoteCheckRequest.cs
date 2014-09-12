using System;
using System.Xml;
using System.Xml.Serialization;
using Blk.Api;

namespace Blk.Engine.Remote
{
	/// <summary>
	/// Represents a remote check request of a module program
	/// </summary>
	[XmlRootAttribute("remoteCheckRequest", Namespace = "", IsNullable = false)]
	public class RemoteCheckRequest
	{
		#region Variables

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
		/// Initializes a new instance of RemoteCheckRequest
		/// </summary>
		public RemoteCheckRequest()
		{
			this.moduleName = null;
			this.processInfo = new ModuleProcessInfo();
		}

		/// <summary>
		/// Initializes a new instance of RemoteCheckRequest
		/// </summary>
		/// <param name="moduleName">Module name</param>
		/// <param name="method">The method for shutdown the modules</param>
		/// <param name="mpi">Data about the program asociated to this module</param>
		public RemoteCheckRequest(string moduleName, IModuleProcessInfo mpi)
		{
			this.moduleName = moduleName;
			if(mpi != null)
				this.processInfo = new ModuleProcessInfo(mpi.ProcessName, mpi.ProgramPath, mpi.ProgramArgs);
		}

		#endregion

		#region Properties

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
		/// Serializes a RemoteCheckRequest object to xml
		/// </summary>
		/// <param name="rsr">Object to serialize</param>
		/// <returns>string containing serialized xml object</returns>
		public static string ToXml(RemoteCheckRequest rsr)
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
		/// Deserializes a RemoteCheckRequest object from xml
		/// </summary>
		/// <param name="xml">String containing serialized object</param>
		/// <returns>Deserialized object</returns>
		public static RemoteCheckRequest FromXml(string xml)
		{
			XmlSerializer serializer;
			System.IO.StringReader stream;

			stream = new System.IO.StringReader(xml);
			serializer = new XmlSerializer(typeof(RemoteCheckRequest));
			return (RemoteCheckRequest)serializer.Deserialize(stream);
		}

		/// <summary>
		/// Deserializes a RemoteCheckRequest object from xml
		/// </summary>
		/// <param name="reader">Xml stream serialized object</param>
		/// <returns>Deserialized object</returns>
		public static RemoteCheckRequest FromXml(System.Xml.XmlReader reader)
		{
			XmlSerializer serializer;

			serializer = new XmlSerializer(typeof(RemoteCheckRequest));
			return (RemoteCheckRequest)serializer.Deserialize(reader);
		}

		#endregion
	}
}
