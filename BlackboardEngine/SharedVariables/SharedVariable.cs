#define SharedVarsStoredAsRawString

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using Blk.Api;
using Blk.Api.SharedVariables;

namespace Blk.Engine.SharedVariables
{
	/// <summary>
	/// Represents the method that will handle the Written event of a SharedVariable object
	/// </summary>
	/// <param name="sv">The SharedVariable object tht rises the Written event</param>
	public delegate void SharedVariableWrittenEventHandler(SharedVariable sv);

	/// <summary>
	/// Represents a stored shared variable
	/// </summary>
	public class SharedVariable : ISharedVariable
	{
		#region Variables

		/// <summary>
		/// Stores the data of the variable
		/// </summary>
#if SharedVarsStoredAsRawString
		protected string data;
#else
		protected byte[] data;
#endif

		/// <summary>
		/// Stores the list of names of modules which are allowed to write to the var
		/// </summary>
		protected List<string> allowedWriters;

		/// <summary>
		/// Flag that specifies if the SharedVariable can be dynamically resized
		/// </summary>
		protected bool dynamic;

		/// <summary>
		/// Indicates if the variable is an array
		/// </summary>
		protected bool isArray;

		/// <summary>
		/// Stores the name of the variable
		/// </summary>
		protected string name;

		/// <summary>
		/// The IModule object which this SharedVariable object is bound to
		/// </summary>
		protected readonly IModuleClient owner;

		/// <summary>
		/// Multiple readers, single writer monitor lock
		/// </summary>
		protected ReaderWriterLock rwLock;

		/// <summary>
		/// Stores the size of the variable
		/// </summary>
		protected int size;

		/// <summary>
		/// Collection of subscriptions asociated to this SharedVariable object
		/// </summary>
		protected SharedVariableSubscriptionList subscriptions;

		/// <summary>
		/// Stores the type of the variable
		/// </summary>
		protected string type;

		/// <summary>
		/// The creation time of the shared variable
		/// </summary>
		private DateTime creationTime;

		#endregion

		#region Static Variables

		/// <summary>
		/// Regular expression used to validate variable types
		/// </summary>
		public static readonly Regex RxVarTypeValidator = new Regex(@"^[A-Z_a-z][0-9A-Z_a-z]*(\[\d*\])?$", RegexOptions.Compiled);

		/// <summary>
		/// Regular expression used to validate variable names
		/// </summary>
		public static readonly Regex RxVarNameValidator = new Regex(@"^[A-Z_a-z][0-9A-Z_a-z]*$", RegexOptions.Compiled);

		/// <summary>
		/// Regular expression used to validate variable content hex representation
		/// </summary>
#if SharedVarsStoredAsRawString
		public static readonly Regex RxVarContentValidator = new Regex(@".*", RegexOptions.Compiled);
#else
		public static readonly Regex RxVarContentValidator = new Regex(@"(0[Xx])?([0-9A-Fa-f]{2})+", RegexOptions.Compiled);
#endif

		/// <summary>
		/// Regular expression used to validate write of SharedVariable objects
		/// </summary>
#if SharedVarsStoredAsRawString
		public static readonly Regex RxWriteSharedVariableValidator = new Regex(@"(\{\s+)?((?<type>[A-Z_a-z][0-9A-Z_a-z]*)(?<array>\[(?<size>\d*)\])?\s+)?(?<name>[A-Z_a-z][0-9A-Z_a-z]*)\s+(?<data>.*)(\s*\})?", RegexOptions.Compiled);
#else
		public static readonly Regex RxSharedVariableValidator = new Regex(@"(?<name>[A-Z_a-z][0-9A-Z_a-z]*)\s*\%\s*(0[Xx])?(?<data>([0-9A-Fa-f]{2})+)", RegexOptions.Compiled);
#endif

		/// <summary>
		/// Regular expression used to validate read of SharedVariable objects
		/// </summary>
		public static readonly Regex RxReadSharedVariableValidator = new Regex(@"(\{\s*)?((?<type>[A-Z_a-z][0-9A-Z_a-z]*)(?<array>\[(?<size>\d*)\])?\s+)?(?<name>[A-Z_a-z][0-9A-Z_a-z]*)(\s*\})?", RegexOptions.Compiled);

		/// <summary>
		/// Regular expression used to validate read of multiple SharedVariable objects
		/// </summary>
		public static readonly Regex RxReadMultipleSharedVariableValidator = new Regex(@"(\{\s*)?(?<names>[A-Z_a-z][0-9A-Z_a-z]*(\s+[A-Z_a-z][0-9A-Z_a-z]*)*)(\s*\})?", RegexOptions.Compiled);

		/// <summary>
		/// Regular expression used to validate creation of SharedVariable objects
		/// </summary>
		//public static readonly Regex RxCreateSharedVariableValidator = new Regex(@"^(?<name>[A-Z_a-z][0-9A-Z_a-z]*)(\s*\%\s*(?<size>\d{1,9}))?$", RegexOptions.Compiled);
		public static readonly Regex RxCreateSharedVariableValidator = new Regex(@"^(\{\s*)?(\{\s+)?((?<type>[A-Z_a-z][0-9A-Z_a-z]*)(?<array>\[(?<size>\d*)\])?\s+)?(?<name>[A-Z_a-z][0-9A-Z_a-z]*)(\s*\})?$", RegexOptions.Compiled);

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new Instance of SharedVariable
		/// </summary>
		/// <param name="owner">The IModule object which this SharedVariable object is bound to</param>
		/// <param name="name">The name of the variable</param>
		public SharedVariable(IModuleClient owner, string name)
			: this(owner, "var", name, false, -1)
		{ }

		/// <summary>
		/// Initializes a new Instance of SharedVariable
		/// </summary>
		/// <param name="owner">The IModule object which this SharedVariable object is bound to</param>
		/// <param name="type">The type of the variable</param>
		/// <param name="name">The name of the variable</param>
		/// <param name="isArray">Indicates if the variable is an array</param>
		public SharedVariable(IModuleClient owner, string type, string name, bool isArray)
			: this(owner, type, name, isArray, -1)
		{ }

		/// <summary>
		/// Initializes a new Instance of SharedVariable
		/// </summary>
		/// <param name="owner">The IModule object which this SharedVariable object is bound to</param>
		/// <param name="type">The type of the variable</param>
		/// <param name="name">The name of the variable</param>
		/// <param name="isArray">Indicates if the variable is an array</param>
		/// <param name="arraySize">If the variable is an array, the size of the array or -1 if the variable is not an array. Also a -1 value can be used to indicate dynamic size arrays</param>
		public SharedVariable(IModuleClient owner, string type, string name, bool isArray, int arraySize)
		{
			if (owner == null) throw new ArgumentNullException("owner");
			if (name == null) throw new ArgumentNullException("name");
			if (arraySize < -1) throw new ArgumentOutOfRangeException("arraySize");
			if ((type == null) || type.StartsWith("var[")) type = "var";
			if (!RxVarNameValidator.IsMatch(name)) throw new ArgumentException("Invalid variable name", "name");
			this.owner = owner;
			this.type = type;
			this.name = name;
			this.size = -1;
			if (type == "var")
			{
				this.isArray = false;
				this.size = -1;
			}
			else
			{
				if (this.isArray = isArray)
					this.size = arraySize;
				else
					this.size = -1;
			}
			this.allowedWriters = new List<string>();
#if SharedVarsStoredAsRawString
			data = String.Empty;
#else
			data = new byte[size];
#endif

			subscriptions = new SharedVariableSubscriptionList(this);
			subscriptions.SubscriptionAdded += new SubscriptionAddedRemovedEventHandler(subscriptions_SubscriptionAdded);

			rwLock = new ReaderWriterLock();
			this.creationTime = DateTime.Now;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the list of names of modules which are allowed to write to the var
		/// </summary>
		[ReadOnlyAttribute(true)]
		[Browsable(true)]
		[TypeConverter(typeof(WritersConverter))]
		public virtual List<string> AllowedWriters
		{
			get { return allowedWriters; }
			internal set
			{
				if (value == null)
					throw new ArgumentNullException();
				allowedWriters = value;
			}
		}

		[Browsable(false)]
		string[] ISharedVariable.AllowedWriters
		{
			get { return allowedWriters.ToArray(); }
		}

		/// <summary>
		/// Gets or sets the data of the SharedVariable
		/// </summary>
#if SharedVarsStoredAsRawString

		internal virtual string Data
		{
			get
			{
				string tmp;
				rwLock.AcquireReaderLock(-1);
				tmp = this.data;
				rwLock.ReleaseReaderLock();
				return tmp;
			}
			set
			{
				rwLock.AcquireWriterLock(-1);
				this.data = value;
				rwLock.ReleaseWriterLock();
			}
		}
#else
		internal byte[] Data
		{
			get
			{
				//byte[] tmpData = new byte[size];
				//lock (data)
				//{
				//    data.CopyTo(tmpData, 0);
				//}
				//return tmpData;
				
				byte[] tmp;
				rwLock.AcquireReaderLock(-1);
				tmp = this.data;
				rwLock.ReleaseReaderLock();
				return tmp;
			}
			set
			{
				rwLock.AcquireWriterLock(-1);
				if (!dynamic && (value.Length != size))
				{
					rwLock.ReleaseWriterLock();
					throw new ArgumentException("Data size mismatch", "value");
				}
				data = value;
				rwLock.ReleaseWriterLock();

				//lock (data)
				//{
				//    if (dynamic)
				//    {
				//        //lock (value)
				//        //{
				//        if (value.Length != size)
				//        {
				//            size = value.Length;
				//            data = new byte[size];
				//        }
				//        if (size > 0)
				//            value.CopyTo(data, 0);
				//        //}
				//    }
				//    else
				//    {
				//        if (value.Length != size) throw new ArgumentException("Data size mismatch", "value");
				//        //lock (value)
				//        //{
				//        value.CopyTo(data, 0);
				//        //}
				//    }
				//}
			}
		}
#endif

		/// <summary>
		/// Gets the creation time of the shared variable
		/// </summary>
		[CategoryAttribute("Information")]
		[DescriptionAttribute("Indicates the creation time of the shared variable")]
		public DateTime CreationTime
		{
			get { return this.creationTime; }
		}

		/// <summary>
		/// Gets a value indicating if the SharedVariable can be dynamically resized
		/// </summary>
		[Browsable(false)]
		public bool Dynamic
		{
			get { return dynamic; }
		}

		/// <summary>
		/// Gets a value indicating if the variable is an array
		/// </summary>
		[CategoryAttribute("Information")]
		[DescriptionAttribute("Indicates if the variable is an array")]
		public bool IsArray
		{
			get { return this.isArray; }
		}

		/// <summary>
		/// Gets the name of the SharedVariable
		/// </summary>
		[ParenthesizePropertyName(true)]
		[CategoryAttribute("Information")]
		[DescriptionAttribute("The name of the Shared Variable")]
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Gets the IModule object which this SharedVariable object is bound to
		/// </summary>
		[CategoryAttribute("Information")]
		[DescriptionAttribute("The module to which this Shared Variable object is bound to")]
		public IModuleClient Owner
		{
			get { return owner; }
		}

#if SharedVarsStoredAsRawString
		/// <summary>
		/// Gets a human readable representation of the content of th shared variable
		/// </summary>
		[DisplayName("Value")]
		[CategoryAttribute("Information")]
		[DescriptionAttribute("The (serialized) value contained in the shared variable")]
		public string StringData
		{
			get
			{
				string value = "[Read Error]";
				try
				{
					rwLock.AcquireReaderLock(50);
					value = this.data;
					rwLock.ReleaseReaderLock();
				}
				catch (ApplicationException)
				{
					rwLock.ReleaseReaderLock();
					value = "[Read Error]";
				}
				return value;
			}
		}
#endif

		/// <summary>
		/// Gets the size of the SharedVariable
		/// </summary>
		[Browsable(false)]
		public int Size
		{
			get { return size; }
		}

		/// <summary>
		/// Gets the collection of subscriptions asociated to this SharedVariable object
		/// </summary>
		[ReadOnlyAttribute(true)]
		[CategoryAttribute("Subscriptions")]
		[DescriptionAttribute("The collection of subscriptor modules to this Shared Variable")]
		[TypeConverter(typeof(SubscriptionCollectionConverter))]
		public SharedVariableSubscriptionList Subscriptions
		{
			get
			{
				return subscriptions;
			}
		}

		/// <summary>
		/// Gets the string to send this SharedVariable object as parameter in a text command
		/// </summary>
		[Browsable(false)]
		public string StringToSend
		{
			get
			{

#if SharedVarsStoredAsRawString
				string sts;
				sts = "{ " + type;
				if (isArray)
				{
					sts += "[";
					if (size != -1) sts += size.ToString();
					sts += "]";
				}
				sts += " " + name;
				rwLock.AcquireReaderLock(-1);
				if (!String.IsNullOrEmpty(Data)) sts += " " + Data;
				rwLock.ReleaseReaderLock();
				sts += " }";
				return sts;
#else
				StringBuilder sb;
				rwLock.AcquireReaderLock(-1);
				sb = new StringBuilder(name.Length + 2 * Data.Length + 1);
				sb.Append(type);
				sb.Append(' ');
				sb.Append(name);
				sb.Append(' ');
				for (int i = 0; i < Data.Length; ++i)
					sb.Append(Data[i].ToString("X2"));
				rwLock.ReleaseReaderLock();
				return sb.ToString();
#endif
			}
		}

		/// <summary>
		/// Gets the type of the variable
		/// </summary>
		[CategoryAttribute("Information")]
		[DescriptionAttribute("The type of the variable")]
		public string Type
		{
			get { return this.type; }
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the value of the shared variable is updated (the shared variable is written)
		/// </summary>
		public event SharedVariableWrittenEventHandler Written;

		#endregion

		#region Methods

		/// <summary>
		/// Gets a standard SharedVariableInfo for serialization
		/// </summary>
		/// <returns></returns>
		public Robotics.API.SharedVariableInfo GetInfo()
		{
			Robotics.API.SharedVariableInfo info;
			Robotics.API.SubscriptionInfo[] subscriptions;
			Robotics.API.SharedVariableSubscriptionType sType;
			Robotics.API.SharedVariableReportType rType;

			info = new Robotics.API.SharedVariableInfo(this.Type, this.Name, this.IsArray, this.Size);
			info.CreationTime = this.creationTime;
			if (this.AllowedWriters.Count > 0)
				info.AllowedWriters = this.AllowedWriters.ToArray();

			if (this.Subscriptions.Count < 1)
				return info;
			subscriptions = new Robotics.API.SubscriptionInfo[this.Subscriptions.Count];
			for (int i = 0; i < this.Subscriptions.Count; ++i)
			{
				switch (this.Subscriptions[i].SubscriptionType)
				{
					case SharedVariableSubscriptionType.Creation:
						sType = Robotics.API.SharedVariableSubscriptionType.Creation;
						break;

					case SharedVariableSubscriptionType.WriteAny:
						sType = Robotics.API.SharedVariableSubscriptionType.WriteAny;
						break;

					case SharedVariableSubscriptionType.WriteModule:
						sType = Robotics.API.SharedVariableSubscriptionType.WriteModule;
						break;

					default:
					case SharedVariableSubscriptionType.WriteOthers:
						sType = Robotics.API.SharedVariableSubscriptionType.WriteOthers;
						break;
				}

				switch (this.Subscriptions[i].ReportType)
				{
					case SharedVariableReportType.Notify:
						rType = Robotics.API.SharedVariableReportType.Notify;
						break;

					default:
					case SharedVariableReportType.SendContent:
						rType = Robotics.API.SharedVariableReportType.SendContent;
						break;
				}
				subscriptions[i] = new Robotics.API.SubscriptionInfo(
					info,
					this.Subscriptions[i].Subscriber.Name,
					sType,
					rType
					);
				//if (this.Subscriptions[i].SubscriptionType == SharedVariableSubscriptionType.WriteModule)
				//	subscriptions[i].WriterModule = this.Subscriptions[i].WriterModule;
			}
			return info;
		}

		/// <summary>
		/// Reads the content of the SharedVariable as an hexadecimal string representation
		/// </summary>
		/// <returns>the content of the SharedVariable in hexadecimal format as string</returns>
		public virtual string ReadStringData()
		{
#if SharedVarsStoredAsRawString
			return Data;
#else
			StringBuilder sb;
			
			rwLock.AcquireReaderLock(-1);
			sb = new StringBuilder(data.Length * 2);
			for (int i = 0; i < data.Length; ++i)
				sb.Append(data[i].ToString("X2"));
			rwLock.ReleaseReaderLock();
			
			return sb.ToString();
#endif
		}

		/// <summary>
		/// Writes the provided data in hexadecimal string representation to the variable
		/// </summary>
		/// <param name="writer">The IModule object which requested the write operation</param>
		/// <param name="dataType">The received type of the data to update the variable</param>
		/// <param name="arraySize">Specifies the size of the array if the variable was created as an array. If the variable is not an array must be -1</param>
		/// <param name="content">The data in hexadecimal string representation</param>
		/// <returns>true if the write succeded, false otherwise</returns>
		public virtual bool WriteStringData(IModuleClient writer, string dataType, int arraySize, string content)
		{
			if (!String.IsNullOrEmpty(content) && (this.type != "var") && (this.type != dataType))
				return false;
			/*
			 * a = isArray
			 * b = arraySize != -1
			 * 
			 * a b r
			 * 0 0 0 
			 * 0 1 1
			 * 1 0 1
			 * 1 1 0
			*/
			if ((this.size != -1) && (this.size != arraySize))
				return false;
			if ((this.type != "var") && (this.size != -1) && (this.isArray ^ (arraySize != -1)))
				return false;
			if (!allowedWriters.Contains("*") &&
				((writer == null) || ((writer != null) && !allowedWriters.Contains(writer.Name))))
				return false;

#if SharedVarsStoredAsRawString
			rwLock.AcquireWriterLock(-1);
			this.data = content;
			rwLock.ReleaseWriterLock();
#else
			if (!RxVarContentValidator.IsMatch(content))
				return false;

			int nSize = content.Length / 2;

			if (!dynamic && (nSize != size))
				return false;
			byte[] nData;

			nData = new byte[nSize];
			int i = 0;
			int j = 0;
			for (i = 0; i < nSize; ++i, j += 2)
			{
				if (!Byte.TryParse(
					content.Substring(j, 2),
					System.Globalization.NumberStyles.HexNumber,
					System.Globalization.NumberFormatInfo.InvariantInfo,
					out nData[i]))
					return false;
			}


			rwLock.AcquireWriterLock(-1);
			this.size = nSize;
			this.data = nData;
			rwLock.ReleaseWriterLock();
#endif
			OnWritten();
			ReportSubscribers(writer);

			return true;
		}

		/// <summary>
		/// Sends the variable creation notification to the modules which subscribed for this action
		/// </summary>
		protected void ReportCreationSubscribers()
		{
			// Generate parameters for notify subscription type
			//string pNotify = name + " " + Data.Length.ToString();
			string pNotify = name + " % ";
			// Generate parameters for content subscription type
			string pContent = StringToSend;
			// The chosen parameters
			string param;
			// The response object
			Response response;
			int i = 0;

			lock (subscriptions)
			{

				while (i < subscriptions.Count)
				{
					if (subscriptions[i].SubscriptionType == SharedVariableSubscriptionType.Creation)
					{
						switch (subscriptions[i].ReportType)
						{
							case SharedVariableReportType.Notify:
								param = pNotify;
								break;

							case SharedVariableReportType.SendContent:
								param = pContent;
								break;

							default:
								param = pNotify;
								break;
						}
						response = new Response(this.Owner, subscriptions[i].Subscriber, "read_var", param, true);
						subscriptions.RemoveAt(i);
						response.Send();
					}
					else
						++i;

				}
			}
		}

		/// <summary>
		/// Sends all subscription messages to the subscribers
		/// </summary>
		/// <param name="writer">The IModule object which requested the write operation</param>
		protected internal void ReportSubscribers(IModuleClient writer)
		{
			// Generate parameters for notify subscription type
			//string pNotify = name + " " + Data.Length.ToString();
			string pNotify = "{ " + type + (isArray ? "[" + ((size != -1) ? size.ToString() : "") + "] " : " ") + name + " } ";
			// Generate parameters for content subscription type
			string pContent = StringToSend + " ";
			// The chosen parameters
			string param;
			// The response object
			Response response;

			OnWritten();
			lock (subscriptions)
			{

				for (int i = 0; i < subscriptions.Count; ++i)
				{
					switch (subscriptions[i].ReportType)
					{
						case SharedVariableReportType.Notify:
							param = pNotify;
							break;

						case SharedVariableReportType.SendContent:
							param = pContent;
							break;

						default:
							param = pNotify;
							break;
					}

					param += subscriptions[i].ReportType.ToString();
					param += " % " + subscriptions[i].SubscriptionType.ToString();
					param += " % " + writer.Name;
					response = new Response(this.Owner, subscriptions[i].Subscriber, "read_var", param, true);

					switch (subscriptions[i].SubscriptionType)
					{
						case SharedVariableSubscriptionType.Creation:
							break;

						case SharedVariableSubscriptionType.WriteAny:
							response.Send();
							break;

						case SharedVariableSubscriptionType.WriteModule:
							break;

						case SharedVariableSubscriptionType.WriteOthers:
						default:
							if (writer != subscriptions[i].Subscriber)
								response.Send();
							break;
					}
				}
			}
		}

		/// <summary>
		/// Gets the string representation of the SharedVariable
		/// </summary>
		/// <returns>The string representation of the SharedVariable</returns>
		public override string ToString()
		{
			return base.ToString();
		}

		/// <summary>
		/// Rises the Written event
		/// </summary>
		protected virtual void OnWritten()
		{
			if (this.Written != null)
			{
				try
				{
					this.Written(this);
				}
				catch { }
			}
		}

		#region EventHandlers

		/// <summary>
		/// Handles the SubscriptionAdded event of the suscrptions collection
		/// </summary>
		/// <param name="collection">The suscrptions collection</param>
		/// <param name="subscription">The subscription added</param>
		protected virtual void subscriptions_SubscriptionAdded(SharedVariableSubscriptionList collection, SharedVariableSubscription subscription)
		{
			if (subscription.SubscriptionType == SharedVariableSubscriptionType.Creation)
				ReportCreationSubscribers();
		}

		#endregion

		#endregion

	}	
}
