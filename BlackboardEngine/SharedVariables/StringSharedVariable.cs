/*
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Blk.Engine.SharedVariables
{
	public class StringSharedVariable : SharedVariable2
	{
		#region Variables

		/// <summary>
		/// Multiple readers, single writer monitor lock
		/// </summary>
		protected ReaderWriterLock rwLock;

		/// <summary>
		/// Stores the value of the variable
		/// </summary>
		protected string value;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new Instance of SharedVariable
		/// </summary>
		/// <param name="owner">The IModule object which this SharedVariable object is bound to</param>
		/// <param name="name">The name of the variable</param>
		public StringSharedVariable(IModule owner, string name)
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
		public string Read()
		{
			string tmp;
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
			value = null;
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
			string pContent = value;
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
			string pContent = value;
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
		public bool Write(string value)
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
			//if (value is T)
			//    return Write((T)value);
			return false;

		}

		#endregion
	}
}
*/