using System;
using System.ComponentModel;

namespace Blk.Engine.SharedVariables
{
	internal class SubscriptionPropertyDescriptor : PropertyDescriptor
	{
		private SharedVariableSubscription subscription;

		public SubscriptionPropertyDescriptor(int index, SharedVariableSubscription subscription)
			: this(index, subscription, null)
		{
		}

		public SubscriptionPropertyDescriptor(int index, SharedVariableSubscription subscription, Attribute[] attrs)
			: base(index.ToString(), attrs)
		{
			this.subscription = subscription;
		}

		public override bool CanResetValue(object component)
		{
			//throw new Exception("The method or operation is not implemented.");
			return false;
		}

		public override Type ComponentType
		{
			get
			{
				//throw new Exception("The method or operation is not implemented.");
				return typeof(SharedVariableSubscription);
			}
		}

		public override object GetValue(object component)
		{
			return subscription.Subscriber.Name;
		}

		public override bool IsReadOnly
		{
			get { return true; }
		}

		public override Type PropertyType
		{
			get { return typeof(string); }
		}

		public override void ResetValue(object component)
		{
			//throw new Exception("The method or operation is not implemented.");
		}

		public override void SetValue(object component, object value)
		{
			//throw new Exception("The method or operation is not implemented.");
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
