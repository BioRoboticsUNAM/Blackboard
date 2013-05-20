using System;
using System.Xml;
using System.Xml.Serialization;

namespace Blk.Engine.Remote
{
	/// <summary>
	/// Represents a response for a remote shutdown request
	/// </summary>
	[XmlRootAttribute("remoteShutdownResponse", Namespace = "", IsNullable = false)]
	public class RemoteShutdownResponse : RemoteShutdownRequest
	{
		
		#region Variables

		/// <summary>
		/// Stores the response
		/// </summary>
		protected bool success;

		/// <summary>
		/// Stores custom message
		/// </summary>
		protected string message;

        #endregion

        #region Constructors

		/// <summary>
		/// Initializes a new instance of RemoteShutdownResponse
		/// </summary>
		public RemoteShutdownResponse()
			:base()
		{
			this.success = false;
			this.message = null;
		}

		/// <summary>
		/// Initializes a new instance of RemoteShutdownResponse
		/// </summary>
		/// <param name="request">RemoteShutdownRequest object to create the response from</param>
		/// <param name="success">Indicates if the request succedeed or not</param>
		/// <param name="message">Custom response in case of failure</param>
		public RemoteShutdownResponse(RemoteShutdownRequest request, bool success, string message)
			: base(request.ModuleName, request.Method, request.ProcessInfo)
		{
			this.success = success;
			this.message = message;
		}

		/// <summary>
		/// Initializes a new instance of RemoteShutdownResponse
		/// </summary>
		/// <param name="moduleName">Module name</param>
		/// <param name="method">The method of startup sequence for modules</param>
		/// <param name="mpi">Data about the program asociated to this module</param>
		/// <param name="success">Indicates if the request succedeed or not</param>
		/// <param name="message">Custom response in case of failure</param>
		public RemoteShutdownResponse(string moduleName, ModuleShutdownMethod method, ModuleProcessInfo mpi, bool success, string message)
			:base(moduleName, method, mpi)
		{
			this.success = success;
			this.message = message;
		}

        #endregion

        #region Properties
		
		/// <summary>
		/// Gets a value that indicates id the request succedeed or not
		/// </summary>
		[XmlElement("success")]
		public bool Success
		{
			get { return this.success; }
			set { this.success = value; }
		}

		/// <summary>
		/// Gets the custom response in case of failure
		/// </summary>
		[XmlElement("message")]
		public string Message
		{
			get { return this.message; }
			set{this.message = value;}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Serializes a RemoteShutdownResponse object to xml
		/// </summary>
		/// <param name="rsr">Object to serialize</param>
		/// <returns>string containing serialized xml object</returns>
		public static string ToXml(RemoteShutdownResponse rsr)
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
			serializer = new XmlSerializer(typeof(RemoteShutdownResponse));
			serializer.Serialize(writer, rsr, ns);
			writer.Flush();
			writer.Close();
			return sb.ToString();
		}

		/// <summary>
		/// Deserializes a RemoteShutdownResponse object from xml
		/// </summary>
		/// <param name="xml">String containing serialized object</param>
		/// <returns>Deserialized object</returns>
		public static new RemoteShutdownResponse FromXml(string xml)
		{
			XmlSerializer serializer;
			System.IO.StringReader stream;

			stream = new System.IO.StringReader(xml);
			serializer = new XmlSerializer(typeof(RemoteShutdownResponse));
			return (RemoteShutdownResponse)serializer.Deserialize(stream);
		}

		/// <summary>
		/// Deserializes a RemoteStartupResponse object from xml
		/// </summary>
		/// <param name="reader">Xml stream serialized object</param>
		/// <returns>Deserialized object</returns>
		public static new RemoteShutdownResponse FromXml(System.Xml.XmlReader reader)
		{
			XmlSerializer serializer;

			serializer = new XmlSerializer(typeof(RemoteShutdownResponse));
			return (RemoteShutdownResponse)serializer.Deserialize(reader);
		}

		#endregion
	}
}
