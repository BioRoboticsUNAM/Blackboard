using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Blk.Api;

namespace Blk.Engine.SharedVariables
{
	/// <summary>
	/// Specifies the type of subscription to a SharedVariable object
	/// </summary>
	public enum SharedVariableSubscriptionType
	{
		/// <summary>
		/// Subscribes to the creation of a variable by any module
		/// </summary>
		/// <remarks>After variable creation the subscription is deleted</remarks>
		Creation,
		/// <summary>
		/// Subscribes to the creation of a variable by any module
		/// </summary>
		WriteAny,
		/// <summary>
		/// Subscribes to the writing of a variable by any module different from the subscriber one
		/// </summary>
		/// <remarks>This is the default value</remarks>
		WriteOthers,
		/// <summary>
		/// Subscribes to the writing of a variable by th specified module
		/// </summary>
		WriteModule
	}

	/// <summary>
	/// Specifies how a subscription report of a shared variable change is made
	/// </summary>
	public enum SharedVariableReportType
	{
		/// <summary>
		/// Sends a report that just notifies the change of the content of a shared variable
		/// </summary>
		/// <remarks>This is the default value</remarks>
		Notify,
		/// <summary>
		/// Sends a report that notifies the change of the content of a shared variable sending it's content
		/// </summary>
		SendContent
	}

	/// <summary>
	/// Represents a subscription to a shared variable
	/// </summary>
	public class SharedVariableSubscription :IComparable<SharedVariableSubscription>
	{
		#region Variables

		/// <summary>
		/// The report type choosen by the subscriber
		/// </summary>
		protected readonly SharedVariableReportType reportType;
		
		/// <summary>
		/// The IModule object that subscribes to the shared variable
		/// </summary>
		protected readonly IModuleClient subscriber;

		/// <summary>
		/// The type of subscription to the shared variable
		/// </summary>
		protected readonly SharedVariableSubscriptionType subscriptionType;

		/// <summary>
		/// The shared variable which this SharedVariableSubscription object is binded to
		/// </summary>
		protected SharedVariable variable;

		/// <summary>
		/// The name of the shared variable
		/// </summary>
		protected readonly string variableName;

		/// <summary>
		/// The size the variable must match to send a subscription notification. A -1 value indicates any size
		/// </summary>
		protected readonly int variableSize;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SharedVariableSubscription
		/// </summary>
		/// <param name="variableName">The name of the shared variable</param>
		/// <param name="subscriber">The IModule object that subscribes to the shared variable</param>
		public SharedVariableSubscription(string variableName, IModuleClient subscriber)
			: this(variableName, subscriber, SharedVariableSubscriptionType.WriteOthers, SharedVariableReportType.Notify, -1)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of SharedVariableSubscription
		/// </summary>
		/// <param name="variableName">The name of the shared variable</param>
		/// <param name="subscriber">The IModule object that subscribes to the shared variable</param>
		/// <param name="subscriptionType">The type of subscription to the shared variable</param>
		public SharedVariableSubscription(string variableName, IModuleClient subscriber, SharedVariableSubscriptionType subscriptionType)
			: this(variableName, subscriber, subscriptionType, SharedVariableReportType.Notify, -1)
		{
		}

		/// <summary>
		/// Initializes a new instance of SharedVariableSubscription
		/// </summary>
		/// <param name="variableName">The name of the shared variable</param>
		/// <param name="subscriber">The IModule object that subscribes to the shared variable</param>
		/// <param name="subscriptionType">The type of subscription to the shared variable</param>
		/// <param name="variableSize">The size the variable must match to send a subscription notification. A -1 value indicates any size</param>
		public SharedVariableSubscription(string variableName, IModuleClient subscriber, SharedVariableSubscriptionType subscriptionType, int variableSize)
			:this(variableName, subscriber, subscriptionType, SharedVariableReportType.Notify, variableSize)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of SharedVariableSubscription
		/// </summary>
		/// <param name="variableName">The name of the shared variable</param>
		/// <param name="subscriber">The IModule object that subscribes to the shared variable</param>
		/// <param name="subscriptionType">The type of subscription to the shared variable</param>
		/// <param name="reportType">The report type choosen by the subscriber</param>
		/// <param name="variableSize">The size the variable must match to send a subscription notification. A -1 value indicates any size</param>
		public SharedVariableSubscription(string variableName, IModuleClient subscriber, SharedVariableSubscriptionType subscriptionType, SharedVariableReportType reportType, int variableSize)
		{
			this.reportType = reportType;
			this.subscriber = subscriber;
			this.subscriptionType = subscriptionType;
			this.variableName = variableName;
			this.variableSize = variableSize;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the report type choosen by the subscriber
		/// </summary>
		public SharedVariableReportType ReportType
		{
			get { return reportType; }
		}
		
		/// <summary>
		/// Gets the IModule object that subscribes to the shared variable
		/// </summary>
		public IModuleClient Subscriber
		{
			get { return subscriber; }
		}

		/// <summary>
		/// Gets the type of subscription to the shared variable
		/// </summary>
		public SharedVariableSubscriptionType SubscriptionType
		{
			get { return subscriptionType; }
		}

		/// <summary>
		/// Gets or Sets the shared variable which this SharedVariableSubscription object is binded to
		/// </summary>
		public SharedVariable Variable
		{
			get { return variable; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				if (value.Name != VariableName)
					throw new ArgumentException("The names of the SharedVariable and this SharedVariableSubscription does not match");
				variable = value;
			}
		}

		/// <summary>
		/// Gets the name of the shared variable
		/// </summary>
		public string VariableName
		{
			get { return variableName; }
		}

		/// <summary>
		/// The size the variable must match to send a subscription notification.
		/// </summary>
		/// <remarks>A -1 value indicates any size</remarks>
		public int VariableSize
		{
			get { return variableSize; }
		}

		#endregion

		#region Methods

		#region IComparable<SharedVariableSubscription> Members

		/// <summary>
		/// Compares to SharedVariableSubscription objects
		/// </summary>
		/// <param name="other">SharedVariableSubscription object to compare to</param>
		/// <returns>Comparison result</returns>
		public int CompareTo(SharedVariableSubscription other)
		{
			//int result;

			//result = this.variableName.CompareTo(other);
			//if(result == 0)
			//{
			//    result = this.variableSize.CompareTo(other.variableSize);
			//    result+= this.subscriptionType.CompareTo(other.subscriptionType);
			//    result+= this.reportType.CompareTo(other.reportType);
			//    if(this.subscriber != null)
			//    result+= this.subscriber.CompareTo(other.subscriber);
			//}
			//return result;
			unchecked
			{
				return this.variableName.CompareTo(other.variableName) + this.subscriber.Name.CompareTo(other.subscriber.Name);
			}
		}

		#endregion

		#endregion

		#region Static Variables

		/// <summary>
		/// Regular expression used to validate subscriptions
		/// </summary>
		public static readonly Regex RxAddSubscriptionValidator = new Regex(@"(?<var>[A-Z_a-z][0-9A-Z_a-z]*)(\s+sub?scribe=(?<sType>(creation|writeany|writeothers|writemodule)))?(\s+report=(?<rType>(notify|content)))?(\s+size=(?<size>(any|-1|\d+)))?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <summary>
		/// Regular expression used to validate unsubscriptions
		/// </summary>
		public static readonly Regex RxRemoveSubscriptionValidator = new Regex(@"(?<var>[A-Z_a-z][0-9A-Z_a-z]*)(\s+sub?scribe=(?<sType>(creation|writeany|writeothers|writemodule)))?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		#endregion

		#region Static Methods

		/// <summary>
		/// Creates a new SharedVariableSubscription object using the parameters included in a command
		/// </summary>
		/// <param name="command">Command which parameters will be parsed</param>
		/// <returns>A SharedVariableSubscription object that represents the subscription request</returns>
		public static SharedVariableSubscription Parse(Command command)
		{
			string var;
			string sType;
			string rType;
			SharedVariableReportType reportType;
			SharedVariableSubscriptionType subscriptionType;
			int size;

			Match m = RxAddSubscriptionValidator.Match(command.Parameters);
			if (!m.Success)
				throw new ArgumentException("Invalid parameters in command", "command");
			
			var = m.Result("${var}");
			sType = m.Result("${sType}");
			rType = m.Result("${rType}");
			if(!Int32.TryParse(m.Result("${size}"), out size))
				size = -1;

			#region Switch Subscription Type

			switch (sType)
			{
				case "creation":
					subscriptionType = SharedVariableSubscriptionType.Creation;
					break;

				case "writeany":
					subscriptionType = SharedVariableSubscriptionType.WriteAny;
					break;

				case "writeothers":
					subscriptionType = SharedVariableSubscriptionType.WriteOthers;
					break;

				case "writemodule":
					subscriptionType = SharedVariableSubscriptionType.WriteModule;
					throw new Exception("Unsupported option");
					//break;

				default:
					subscriptionType = SharedVariableSubscriptionType.WriteOthers;
					break;
			}

			#endregion

			#region Switch Report Type
			switch (rType)
			{
				case "content":
				
					reportType = SharedVariableReportType.SendContent;
					break;

				case "notify":
					reportType = SharedVariableReportType.Notify;
					break;

				default:
					reportType = SharedVariableReportType.Notify;
					break;
			}
			#endregion

			return new SharedVariableSubscription(var, command.Source, subscriptionType, reportType, size);
		}

		/// <summary>
		/// Creates a new SharedVariableSubscription object using the parameters included in a command. A return value indicates whether the operation succeeded.
		/// </summary>
		/// <param name="command">Command which parameters will be parsed</param>
		/// <param name="subscription">When this method returns, contains the SharedVariableSubscription object that represents the subscription request, if the conversion succeeded, or null if the conversion failed. This parameter is passed uninitialized</param>
		/// <returns>true if command parameters was parsed successfully; otherwise, false.</returns>
		public static bool TryParse(Command command, out SharedVariableSubscription subscription)
		{
			subscription = null;
			try { subscription = Parse(command); }
			catch { return false; }
			return true;
		}

		#endregion
	}
}
