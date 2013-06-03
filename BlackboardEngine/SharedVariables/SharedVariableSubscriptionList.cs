using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
//using System.Text;
using Blk.Api;

namespace Blk.Engine.SharedVariables
{

	/// <summary>
	/// Represents the method that will handle the SubscriptionAdded event and SubscriptionRemoved event of a SharedVariableSubscriptionList object
	/// </summary>
	/// <param name="List">The List which raises the event</param>
	/// <param name="subscription">The subscription added to List</param>
	public delegate void SubscriptionAddedRemovedEventHandler(SharedVariableSubscriptionList List, SharedVariableSubscription subscription);

	/// <summary>
	/// Represents a List of SharedVariableSubscription objects
	/// </summary>
	public class SharedVariableSubscriptionList : IList<SharedVariableSubscription>
	{
		#region Variables

		/// <summary>
		/// The list of subscriptions
		/// </summary>
		protected List<SharedVariableSubscription> subscriptions;

		/// <summary>
		/// The SharedVariable object this List is bound to
		/// </summary>
		protected readonly SharedVariable owner;

		/// <summary>
		/// Multiple readers, single writer monitor lock
		/// </summary>
		private ReaderWriterLock rwLock;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SharedVariableSubscriptionList
		/// </summary>
		/// <param name="variable">The SharedVariable object this List will be bound to</param>
		public SharedVariableSubscriptionList(SharedVariable variable)
		{
			if (variable == null) throw new ArgumentNullException();
			this.owner = variable;
			this.subscriptions = new List<SharedVariableSubscription>();
			this.rwLock = new ReaderWriterLock();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of elements actually contained in the SharedVariableSubscriptionList. 
		/// </summary>
		public int Count
		{
			get
			{
				int count;
				rwLock.AcquireReaderLock(-1);
				count = subscriptions.Count;
				rwLock.ReleaseReaderLock();
				return count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the IList is read-only. Always returns false.
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the SharedVariable object this List is bound to
		/// </summary>
		public SharedVariable Owner
		{
			get { return owner; }
		}

		/// <summary>
		/// Gets the element at the specified index
		/// </summary>
		/// <param name="index">The zero-based index of the element to get</param>
		/// <returns>The element at the specified index</returns>
		public SharedVariableSubscription this[int index]
		{
			get
			{
				SharedVariableSubscription subscription;
				rwLock.AcquireReaderLock(-1);
				if ((index < 0) || (index >= subscriptions.Count))
				{
					rwLock.ReleaseReaderLock();
					throw new IndexOutOfRangeException();
				}
				subscription = subscriptions[index];
				rwLock.ReleaseReaderLock();
				return subscription;
			}
			set
			{
				//if (value == null)
				//    throw new ArgumentNullException();
				//value.Variable = this.owner;

				//rwLock.AcquireWriterLock(-1);
				//if (!subscriptions.Contains(value))
				//{
				//    subscriptions[index] = value;
				//    rwLock.ReleaseWriterLock();
				//    if (SubscriptionAdded != null) SubscriptionAdded(this, value);
				//}
				//else
				//{
				//    rwLock.ReleaseWriterLock();
				//    throw new Exception("The same subscription exists in the collection");
				//}

				if (value == null)
					throw new ArgumentNullException();
				value.Variable = this.owner;

				rwLock.AcquireWriterLock(-1);
				if ((index < 0) || (index >= subscriptions.Count))
				{
					rwLock.ReleaseWriterLock();
					throw new IndexOutOfRangeException();
				}
				// Remove duplicates
				for (int i = 0; i < subscriptions.Count; ++i)
				{
					if (subscriptions[i].Subscriber != value.Subscriber)
						continue;
					if (i <= index) --index;
					subscriptions.RemoveAt(i--);
				}
				subscriptions[index] = value;
				rwLock.ReleaseWriterLock();
				if (SubscriptionAdded != null)
					SubscriptionAdded(this, value);
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when a SharedVariableSubscription object is added to the SharedVariableSubscriptionList
		/// </summary>
		public event SubscriptionAddedRemovedEventHandler SubscriptionAdded;

		/// <summary>
		/// Occurs when a SharedVariableSubscription object is removed from the SharedVariableSubscriptionList
		/// </summary>
		public event SubscriptionAddedRemovedEventHandler SubscriptionRemoved;

		#endregion

		#region Methods

		/// <summary>
		/// Rises the SubscriptionRemoved evet
		/// </summary>
		/// <param name="subscription">The removed SharedVariableSubscription object</param>
		protected void OnSubscriptionRemoved(SharedVariableSubscription subscription)
		{
			if (SubscriptionRemoved != null)
				SubscriptionRemoved(this, subscription);
		}

		/// <summary>
		/// Removes the all subscriptions asiciated to the specified IModule from the SharedVariableSubscriptionList.
		/// </summary>
		/// <param name="module">The IModule used to remove the subscriptions.</param>
		/// <returns>true if items was successfully removed from the SharedVariableSubscriptionList; otherwise, false. This method also returns false if item is not found in the original SharedVariableSubscriptionList</returns>
		public bool Remove(IModuleClient module)
		{
			bool result;
			Queue<SharedVariableSubscription> removed;

			result = false;
			rwLock.AcquireWriterLock(-1);
			removed = new Queue<SharedVariableSubscription>(subscriptions.Capacity);
			for (int i = 0; i < subscriptions.Count; ++i)
			{
				if (subscriptions[i].Subscriber == module)
				{
					removed.Enqueue(subscriptions[i]);
					subscriptions.RemoveAt(i);
					--i;
					result = true;
				}
			}
			rwLock.ReleaseWriterLock();

			foreach (SharedVariableSubscription subscription in removed)
				OnSubscriptionRemoved(subscription);
			
			return result;
		}

		/// <summary>
		/// Removes the all subscriptions asiciated to the specified IModule from the SharedVariableSubscriptionList.
		/// </summary>
		/// <param name="moduleName">The name of the subscriber module.</param>
		/// <returns>true if items was successfully removed from the SharedVariableSubscriptionList; otherwise, false. This method also returns false if item is not found in the original SharedVariableSubscriptionList</returns>
		public bool Remove(string moduleName)
		{
			bool result;
			Queue<SharedVariableSubscription> removed;

			result = false;
			rwLock.AcquireWriterLock(-1);
			removed = new Queue<SharedVariableSubscription>(subscriptions.Capacity);
			for (int i = 0; i < subscriptions.Count; ++i)
			{
				if (subscriptions[i].Subscriber.Name == moduleName)
				{
					removed.Enqueue(subscriptions[i]);
					subscriptions.RemoveAt(i);
					--i;
					result = true;
				}
			}
			rwLock.ReleaseWriterLock();

			foreach (SharedVariableSubscription subscription in removed)
				OnSubscriptionRemoved(subscription);

			return result;
		}

		#region IList<SharedVariableSubscription> Members

		/// <summary>
		/// Adds an item to the SharedVariableSubscriptionList.
		/// </summary>
		/// <param name="item">The SharedVariableSubscription object to add to the SharedVariableSubscriptionList.</param>
		public void Add(SharedVariableSubscription item)
		{
			if (item == null)
				throw new ArgumentNullException();
			item.Variable = this.owner;

			//if (!subscriptions.Contains(item))
			//{
			//    subscriptions.Add(item);
			//    if (SubscriptionAdded != null) SubscriptionAdded(this, item);
			//}

			// Remove duplicates
			rwLock.AcquireWriterLock(-1);
			for (int i = 0; i < subscriptions.Count; ++i)
			{
				if (subscriptions[i].Subscriber != item.Subscriber)
					continue;
				subscriptions.RemoveAt(i--);
			}

			// Add subscription
			subscriptions.Add(item);
			rwLock.ReleaseWriterLock();
			if (SubscriptionAdded != null) SubscriptionAdded(this, item);

		}

		/// <summary>
		/// Removes all items from the SharedVariableSubscriptionList.
		/// </summary>
		public void Clear()
		{
			rwLock.AcquireWriterLock(-1);
			if (SubscriptionRemoved != null)
			{
				SharedVariableSubscription item;
				while (subscriptions.Count > 0)
				{
					item = subscriptions[0];
					subscriptions.RemoveAt(0);
					SubscriptionRemoved(this, item);
				}
			}
			else
				subscriptions.Clear();
			rwLock.ReleaseWriterLock();

		}

		/// <summary>
		/// Determines whether the SharedVariableSubscriptionList contains a specific value.
		/// </summary>
		/// <param name="item">The SharedVariableSubscription object to locate in the SharedVariableSubscriptionList.</param>
		/// <returns>true if item is found in the IList; otherwise, false</returns>
		public bool Contains(SharedVariableSubscription item)
		{
			bool result;
			rwLock.AcquireReaderLock(-1);
			result = subscriptions.Contains(item);
			rwLock.ReleaseReaderLock();
			return result;
		}

		/// <summary>
		/// Determines the index of a specific item in the SharedVariableSubscriptionList
		/// </summary>
		/// <param name="item">The object to locate in the SharedVariableSubscriptionList.</param>
		/// <returns>The index of item if found in the list; otherwise, -1</returns>
		public int IndexOf(SharedVariableSubscription item)
		{
			int result;
			rwLock.AcquireReaderLock(-1);
			result = subscriptions.IndexOf(item);
			rwLock.ReleaseReaderLock();
			return result;
		}

		/// <summary>
		/// Inserts an item to the SharedVariableSubscriptionList at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted</param>
		/// <param name="item">The SharedVariableSubscription object to insert into the SharedVariableSubscriptionList</param>
		public void Insert(int index, SharedVariableSubscription item)
		{
			//if (item == null)
			//    throw new ArgumentNullException();

			//item.Variable = this.owner;
			//if (!subscriptions.Contains(item))
			//{
			//    subscriptions.Insert(index, item);
			//    if (SubscriptionAdded != null) SubscriptionAdded(this, item);
			//}
			//else
			//    throw new Exception("The same subscription exists in the collection");
			if (item == null)
				throw new ArgumentNullException();
			item.Variable = this.owner;

			rwLock.AcquireWriterLock(-1);
			if ((index < 0) || (index > subscriptions.Count))
			{
				rwLock.ReleaseWriterLock();
				throw new IndexOutOfRangeException();
			}
			// Remove duplicates
			for (int i = 0; i < subscriptions.Count; ++i)
			{
				if (subscriptions[i].Subscriber != item.Subscriber)
					continue;
				if (i <= index) --index;
				subscriptions.RemoveAt(i--);
			}
			subscriptions.Insert(index, item);
			rwLock.ReleaseWriterLock();
			if (SubscriptionAdded != null)
				SubscriptionAdded(this, item);
		}

		/// <summary>
		/// Removes the item at the specified index
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		public void RemoveAt(int index)
		{
			rwLock.AcquireWriterLock(-1);
			if ((index < 0) || (index >= subscriptions.Count))
			{
				rwLock.ReleaseWriterLock();
				throw new IndexOutOfRangeException();
			}
			subscriptions.RemoveAt(index);
			rwLock.ReleaseWriterLock();
		}

		/// <summary>
		/// Copies the elements of the SharedVariableSubscriptionList to an Array, starting at a particular Array index.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from SharedVariableSubscriptionList. The Array must have zero-based indexing</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins</param>
		public void CopyTo(SharedVariableSubscription[] array, int arrayIndex)
		{
			rwLock.AcquireReaderLock(-1);
			subscriptions.CopyTo(array, arrayIndex);
			rwLock.ReleaseReaderLock();
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the SharedVariableSubscriptionList.
		/// </summary>
		/// <param name="item">The SharedVariableSubscription object to remove from the SharedVariableSubscriptionList.</param>
		/// <returns>true if item was successfully removed from the SharedVariableSubscriptionList; otherwise, false. This method also returns false if item is not found in the original SharedVariableSubscriptionList</returns>
		public bool Remove(SharedVariableSubscription item)
		{
			bool result;
			rwLock.AcquireWriterLock(-1);
			result = subscriptions.Remove(item);
			rwLock.ReleaseWriterLock();
			if (result && (SubscriptionRemoved != null))
				SubscriptionRemoved(this, item);
			return result;
		}

		#endregion

		#region IEnumerable<SharedVariableSubscription> Members

		/// <summary>
		/// Returns an enumerator that iterates through the List
		/// </summary>
		/// <returns>A IEnumerator that can be used to iterate through the List</returns>
		public IEnumerator<SharedVariableSubscription> GetEnumerator()
		{
			IEnumerator<SharedVariableSubscription> enumerator;
			rwLock.AcquireReaderLock(-1);
			enumerator = subscriptions.GetEnumerator();
			rwLock.ReleaseReaderLock();
			return enumerator;
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that iterates through the List
		/// </summary>
		/// <returns>A IEnumerator that can be used to iterate through the List</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			IEnumerator<SharedVariableSubscription> enumerator;
			rwLock.AcquireReaderLock(-1);
			enumerator = subscriptions.GetEnumerator();
			rwLock.ReleaseReaderLock();
			return enumerator;
		}

		#endregion

		#endregion
	}
}
