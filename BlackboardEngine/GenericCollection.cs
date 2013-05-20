using System;
using System.Collections.Generic;
using System.Text;

namespace Blk.Engine
{
	/// <summary>
	/// Provides a collection container that enables Blackboard instances to maintain a list of their collection
	/// </summary>
	public class GenericCollection<T> : IEnumerable<T>, ICollection<T>
	{
		#region Delegates

		/// <summary>
		/// Represents the method that will handle the ElementAdded and ElementRemoved event of a GenericCollection object.
		/// </summary>
		/// <param name="element"></param>
		public delegate void IGenericCollectionAddRemoveEH(T element);

		#endregion

		#region Variables

		private List<T> collection;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the GenericCollection class for the specified parent blackboard.
		/// </summary>
		/// <param name="capacity">The number of elements that the new GenericCollection can initially store</param>
		internal GenericCollection(int capacity)
		{
			this.collection = new List<T>(capacity);
		}

		/// <summary>
		/// Initializes a new instance of the GenericCollection class for the specified parent blackboard.
		/// </summary>
		internal GenericCollection()
			: this(10)
		{
		}

		#endregion

		#region Properties

		#region ICollection<Element> Members

		/// <summary>
		/// Gets the number of elements in the GenericCollection object for the specified Blackboard.
		/// </summary>
		public int Count
		{
			get { return collection.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the GenericCollection object is read-only
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		#endregion

		#endregion

		#region Indexers

		/// <summary>
		/// Gets the element at the specified index position
		/// </summary>
		/// <param name="i">The zero based index of the element to get or set</param>
		/// <returns>The element at position i</returns>
		public T this[int i]
		{
			get { return collection[i]; }
			set { collection[i] = value; }
		}

		#endregion

		#region Events

		/// <summary>
		/// Raises when an element is addedd to the GenericCollection
		/// </summary>
		public event IGenericCollectionAddRemoveEH ElementAdded;
		/// <summary>
		/// Raises when an element is removed from the GenericCollection
		/// </summary>
		public event IGenericCollectionAddRemoveEH ElementRemoved;

		#endregion

		#region Methods

		/// <summary>
		/// Retrieves the index of a specified Element object in the collection
		/// </summary>
		/// <param name="m">The Element for which the index is returned</param>
		/// <returns>The index of the specified Element. If the Element is not currently a member of the collection, it returns -1</returns>
		public virtual int IndexOf(T m)
		{
			return collection.IndexOf(m);
		}

		/// <summary>
		/// Removes a Element, at the specified index location, from the GenericCollection object
		/// </summary>
		/// <param name="index">The ordinal index of the Element to be removed from the collection</param>
		public virtual void RemoveAt(int index)
		{
			if (ElementRemoved != null) ElementRemoved(collection[index]);
			collection.RemoveAt(index);
		}

		#region ICollection<Element> Members

		/// <summary>
		/// Adds the specified Element object to the collection.
		/// </summary>
		/// <param name="item">The Element to add to the collection</param>
		public void Add(T item)
		{
			if (ElementAdded != null) ElementAdded(item);
			collection.Add(item);
			collection.Sort();
		}

		/// <summary>
		/// Removes all controls from the current Blackboard's GenericCollection object
		/// </summary>
		public void Clear()
		{
			for (int i = 0; i < collection.Count; ++i)
			{
				if (ElementRemoved != null) ElementRemoved(collection[i]);
			}
			collection.Clear();
		}

		/// <summary>
		/// Determines whether the specified Blackboard is in the parent Blackboard's GenericCollection object
		/// </summary>
		/// <param name="item">The Element to search for in the collection</param>
		/// <returns>true if the specified Element exists in the collection; otherwise, false</returns>
		public bool Contains(T item)
		{
			return collection.Contains(item);
		}

		/// <summary>
		/// Copies the child controls stored in the GenericCollection object to an Element array object, beginning at the specified index location in the System.Array
		/// </summary>
		/// <param name="array">The Element array to copy the child controls to.</param>
		/// <param name="arrayIndex">The zero-based relative index in array where copying begins</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			collection.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the specified Element from the parent Blackboard's GenericCollection object
		/// </summary>
		/// <param name="item">The Element to be removed</param>
		/// <returns>true if the specified Element exists in the collection; otherwise, false</returns>
		public bool Remove(T item)
		{
			if (collection.Contains(item) && (ElementRemoved != null)) ElementRemoved(item);
			return collection.Remove(item);
		}

		#endregion

		#region IEnumerable<Element> Members
		/// <summary>
		/// Retrieves an enumerator that can iterate through the GenericCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return collection.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Retrieves an enumerator that can iterate through the GenericCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return collection.GetEnumerator();
		}

		#endregion

		#endregion
	}
}
