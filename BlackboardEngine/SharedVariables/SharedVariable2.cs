using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Blk.Api;

namespace Blk.Engine.SharedVariables
{
	/// <summary>
	/// Implements a base class for shared variable
	/// </summary>
	public abstract class SharedVariable2
	{
		#region Variables

		/// <summary>
		/// Stores the list of names of modules which are allowed to write to the var
		/// </summary>
		protected List<string> allowedWriters;

		/// <summary>
		/// Stores the name of the variable
		/// </summary>
		protected string name;

		/// <summary>
		/// The IModule object which this SharedVariable object is bound to
		/// </summary>
		protected readonly IModule owner;

		/// <summary>
		/// Collection of subscriptions asociated to this SharedVariable object
		/// </summary>
		protected SharedVariableSubscriptionList subscriptions;

		#endregion

		#region Static Variables

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
		/// Regular expression used to validate SharedVariable objects
		/// </summary>
#if SharedVarsStoredAsRawString
		public static readonly Regex RxSharedVariableValidator = new Regex(@"(?<name>[A-Z_a-z][0-9A-Z_a-z]*)\|(?<data>.*)", RegexOptions.Compiled);
#else
		public static readonly Regex RxSharedVariableValidator = new Regex(@"(?<name>[A-Z_a-z][0-9A-Z_a-z]*)\|(0[Xx])?(?<data>([0-9A-Fa-f]{2})+)", RegexOptions.Compiled);
#endif

		/// <summary>
		/// Regular expression used to validate creation of SharedVariable objects
		/// </summary>
		public static readonly Regex RxCreateSharedVariableValidator = new Regex(@"^(?<name>[A-Z_a-z][0-9A-Z_a-z]*)\|(?<size>\d{1,9})$", RegexOptions.Compiled);

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new Instance of SharedVariable
		/// </summary>
		/// <param name="owner">The IModule object which this SharedVariable object is bound to</param>
		/// <param name="name">The name of the variable</param>
		public SharedVariable2(IModule owner, string name)
		{
			if (owner == null) throw new ArgumentNullException("owner");
			if (String.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			if (!RxVarNameValidator.IsMatch(name)) throw new ArgumentException("Invalid variable name", "name");
			this.name = name;
			this.allowedWriters = new List<string>();
			//subscriptions = new SharedVariableSubscriptionList(this);
			subscriptions.SubscriptionAdded += new SubscriptionAddedRemovedEventHandler(subscriptions_SubscriptionAdded);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the list of names of modules which are allowed to write to the var
		/// </summary>
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

		/// <summary>
		/// Gets the name of the SharedVariable
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Gets the IModule object which this SharedVariable object is bound to
		/// </summary>
		public IModule Owner
		{
			get { return owner; }
		}

		/// <summary>
		/// Gets the collection of subscriptions asociated to this SharedVariable object
		/// </summary>
		public SharedVariableSubscriptionList Subscriptions
		{
			get
			{
				lock (subscriptions)
				{
					return subscriptions;
				}
			}
		}


		#endregion

		#region Events
		#endregion

		#region Methods

		/// <summary>
		/// Reads the content of the variable
		/// </summary>
		/// <param name="value">When this method returns contains the content of the variable</param>
		public abstract void Read(out ISVSerializable value);

		/// <summary>
		/// Sends the variable creation notification to the modules which subscribed for this action
		/// </summary>
		protected abstract void ReportCreationSubscribers();

		/// <summary>
		/// Sends all subscription messages to the subscribers
		/// </summary>
		/// <param name="writer">The IModule object which requested the write operation</param>
		protected abstract void ReportSubscribers(IModule writer);

		/// <summary>
		/// Gets the string representation of the SharedVariable
		/// </summary>
		/// <returns>The string representation of the SharedVariable</returns>
		public override string ToString()
		{
			return base.ToString();
		}

		/// <summary>
		/// Writes new content to the variable
		/// </summary>
		/// <param name="value">Content to be written to the variable</param>
		/// <returns>true if variable was written successfully, false otherwise</returns>
		public abstract bool Write(ISVSerializable value);

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

		#region Static Methods

		/*
		public static bool SharedVariableCreationParser(string input, IModule owner, out SharedVariable variable)
		{
			string id1;
			string id2;
			bool isArray;
			int arraySize;
			int cc;

			isArray = false;
			variable = null;
			cc = 0;
			if (!Parser.XtractIdentifier(input, ref cc, out id1))
				return false;
			if (input[cc] == '[')
			{
				Parser.XtractInt32(input, ref cc, out arraySize);
				if ((input[cc] != ']') || (arraySize < 0))
					return false;
				isArray = true;

			}
			Parser.XtractIdentifier(input, ref cc, out id2);

			if (String.IsNullOrEmpty(id2))
			{
				//variable = new SharedVariable<SerializableString>(owner, id1);
				return true;
			}
			else if (isArray)
			{
				return false;
			}
			return false;
		}
		*/

		#endregion

	}

	/*

	public class SerializableString : ISVSerializable
	{

		#region ISVSerializable Members

		public bool DeserializeFromByteArray(byte[] input)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool DeserializeFromHexString(string s)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool DeserializeFromRawText(string s)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool DeserializeFromXml(string s)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public byte[] SerializeToByteArray()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string SerializeToHexString()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string SerializeToRawText()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public string SerializeToXml()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}

	*/

	/// <summary>
	/// Implements storage for typed shared variables
	/// </summary>
	/// <typeparam name="T">Type of the shared variable</typeparam>
	public class SharedVariable<T> : SharedVariable2
		where T : ISVSerializable, new()
	{
		#region Variables

		/// <summary>
		/// Multiple readers, single writer monitor lock
		/// </summary>
		protected ReaderWriterLock rwLock;

		/// <summary>
		/// Stores the value of the variable
		/// </summary>
		protected T value;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new Instance of SharedVariable
		/// </summary>
		/// <param name="owner">The IModule object which this SharedVariable object is bound to</param>
		/// <param name="name">The name of the variable</param>
		public SharedVariable(IModule owner, string name)
			: base(owner, name)
		{
			rwLock = new ReaderWriterLock();
		}

		#endregion

		#region Properties
		#endregion

		#region Events
		#endregion

		#region Methods

		/// <summary>
		/// Reads the content of the variable
		/// </summary>
		/// <returns>The content of the variable</returns>
		public T Read()
		{
			T tmp;
			rwLock.AcquireReaderLock(-1);
			tmp = value;
			rwLock.ReleaseReaderLock();
			return tmp;
		}

		/// <summary>
		/// Reads the content of the variable
		/// </summary>
		/// <param name="value">When this method returns contains the content of the variable</param>
		/// <returns>true if variable was readed successfully, false otherwise</returns>
		public override void Read(out ISVSerializable value)
		{
			value = Read();
		}

		/// <summary>
		/// Sends the variable creation notification to the modules which subscribed for this action
		/// </summary>
		protected override void ReportCreationSubscribers()
		{
			// Generate parameters for notify subscription type
			//string pNotify = name + " " + Data.Length.ToString();
			string pNotify = name + "|";
			// Generate parameters for content subscription type
			string pContent = value.SerializeToRawText();
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
		protected override void ReportSubscribers(IModule writer)
		{
			// Generate parameters for notify subscription type
			//string pNotify = name + " " + Data.Length.ToString();
			string pNotify = name;
			// Generate parameters for content subscription type
			string pContent = value.SerializeToRawText();
			// The chosen parameters
			string param;
			// The response object
			Response response;

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

					param += "|" + subscriptions[i].ReportType.ToString();
					param += "|" + subscriptions[i].SubscriptionType.ToString();
					param += "|" + writer.Name;
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
		/// Writes new content to the variable
		/// </summary>
		/// <param name="value">Content to be written to the variable</param>
		/// <returns>true if variable was written successfully, false otherwise</returns>
		public bool Write(T value)
		{
			rwLock.AcquireWriterLock(-1);
			this.value = value;
			rwLock.ReleaseWriterLock();
			return true;
		}

		/// <summary>
		/// Writes new content to the variable
		/// </summary>
		/// <param name="value">Content to be written to the variable</param>
		/// <returns>true if variable was written successfully, false otherwise</returns>
		public override bool Write(ISVSerializable value)
		{
			if (value is T)
				return Write((T)value);
			return false;

		}

		#endregion
	}
}