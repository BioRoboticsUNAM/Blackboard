using System;
using System.Xml;
using System.Xml.Serialization;
using Blk.Api;

namespace Blk.Engine.Remote
{
	/// <summary>
	/// Represents a response for a remote check request
	/// </summary>
	[XmlRootAttribute("remoteCheckResponse", Namespace = "", IsNullable = false)]
	public class RemoteCheckResponse : RemoteCheckRequest
	{
		
		#region Variables

		/// <summary>
		/// Stores the number of running instances
		/// </summary>
		protected int instances;

		/// <summary>
		/// Stores custom message
		/// </summary>
		protected string message;

        #endregion

        #region Constructors

		/// <summary>
		/// Initializes a new instance of RemoteShutdownResponse
		/// </summary>
		public RemoteCheckResponse()
			:base()
		{
			this.instances = -1;
			this.message = null;
		}

		/// <summary>
		/// Initializes a new instance of RemoteShutdownResponse
		/// </summary>
		/// <param name="request">RemoteShutdownRequest object to create the response from</param>
		/// <param name="success">Indicates if the request succedeed or not</param>
		/// <param name="message">Custom response in case of failure</param>
		public RemoteCheckResponse(RemoteShutdownRequest request, int instances, string message)
			: base(request.ModuleName, request.ProcessInfo)
		{
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
		public RemoteCheckResponse(string moduleName, ModuleProcessInfo mpi, int instances, string message)
			:base(moduleName, mpi)
		{
			this.instances = instances;
			this.message = message;
		}

        #endregion

        #region Properties
		
		/// <summary>
		/// Gets the number of running instances
		/// </summary>
		[XmlElement("instances")]
		public int Instances
		{
			get { return this.instances; }
			set { this.instances = value; }
		}

		/// <summary>
		/// Gets a value that indicates id the request succedeed or not
		/// </summary>
		[XmlIgnore]
		public bool Success
		{
			get { return this.instances >= 0; }
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
		public static string ToXml(RemoteCheckResponse rsr)
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
			serializer = new XmlSerializer(typeof(RemoteCheckResponse));
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
		public static new RemoteCheckResponse FromXml(string xml)
		{
			XmlSerializer serializer;
			System.IO.StringReader stream;

			stream = new System.IO.StringReader(xml);
			serializer = new XmlSerializer(typeof(RemoteCheckResponse));
			return (RemoteCheckResponse)serializer.Deserialize(stream);
		}

		/// <summary>
		/// Deserializes a RemoteStartupResponse object from xml
		/// </summary>
		/// <param name="reader">Xml stream serialized object</param>
		/// <returns>Deserialized object</returns>
		public static new RemoteCheckResponse FromXml(System.Xml.XmlReader reader)
		{
			XmlSerializer serializer;

			serializer = new XmlSerializer(typeof(RemoteCheckResponse));
			return (RemoteCheckResponse)serializer.Deserialize(reader);
		}

		#endregion
	}
}
