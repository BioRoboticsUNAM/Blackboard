using System;
using System.Collections.Generic;
using System.Threading;
using Blk.Api.SharedVariables;

namespace Blk.Engine.SharedVariables
{
	/// <summary>
	/// Represents the method that will hanlde the SharedVariableAdded event of a SharedVariableCollection
	/// </summary>
	/// <param name="collection">The SharedVariableCollection to which a new SharedVariable has been added</param>
	/// <param name="variable">The SharedVariable object added to the collection</param>
	public delegate void SharedVariableAddedEventHandler(SharedVariableCollection collection, SharedVariable variable);

	/// <summary>
	/// Provides a collection container that enables ModuleBlackboard instances to maintain a list of their shared variables
	/// allowing multiple readers and single writer on the collection
	/// </summary>
	public class SharedVariableCollection : ISharedVariableCollection, ICollection<SharedVariable>
	{
		#region Variables

		/// <summary>
		/// Stores the shared variables ordered by name
		/// </summary>
		private SortedList<string, SharedVariable> variables;

		/// <summary>
		/// Multiple readers, single writer monitor lock
		/// </summary>
		private ReaderWriterLock rwLock;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SharedVariableCollection
		/// </summary>
		public SharedVariableCollection()
		{
			this.rwLock = new ReaderWriterLock();
			this.variables = new SortedList<string, SharedVariable>(100);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the list of names of the variables in the collection separated by commas
		/// </summary>
		public string NameList
		{
			get
			{
				string[] nameListArray;
				rwLock.AcquireReaderLock(-1);
				nameListArray = new string[variables.Count];
				variables.Keys.CopyTo(nameListArray, 0);
				rwLock.ReleaseReaderLock();
				return String.Join(" ", nameListArray);
			}
		}

		#region ICollection<SharedVariable> Members

		/// <summary>
		/// Gets the number of elements in the SharedVariableCollection object.
		/// </summary>
		public int Count
		{
			get
			{
				int count;
				rwLock.AcquireReaderLock(-1);
				count = variables.Count;
				rwLock.ReleaseReaderLock();
				return count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the SharedVariableCollection object is read-only
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
		/// <param name="index">The zero based index of the element to get</param>
		/// <returns>The module at position i</returns>
		public SharedVariable this[int index]
		{
			get
			{
				SharedVariable shv = null;
				rwLock.AcquireReaderLock(-1);
				if ((index < 0) || (index >= variables.Count))
				{
					rwLock.ReleaseReaderLock();
					throw new ArgumentOutOfRangeException();
				}
				shv = variables.Values[index];
				rwLock.ReleaseReaderLock();
				return shv;
			}
		}

		/// <summary>
		/// Gets the element at the specified index position
		/// </summary>
		/// <param name="index">The zero based index of the element to get</param>
		/// <returns>The module at position i</returns>
		ISharedVariable ISharedVariableCollection.this[int index]
		{
			get { return this[index]; }
		}

		/// <summary>
		/// Gets the element with the specified name
		/// </summary>
		/// <param name="variableName">The name of the element to get</param>
		/// <returns>The variable with specified name</returns>
		public SharedVariable this[string variableName]
		{
			get
			{
				SharedVariable shv = null;
				rwLock.AcquireReaderLock(-1);
				if (!variables.ContainsKey(variableName))
				{
					rwLock.ReleaseReaderLock();
					return null;
				}
				shv = variables[variableName];
				rwLock.ReleaseReaderLock();
				return shv;
			}
		}

		/// <summary>
		/// Gets the element with the specified name
		/// </summary>
		/// <param name="variableName">The name of the element to get</param>
		/// <returns>The variable with specified name</returns>
		ISharedVariable ISharedVariableCollection.this[string variableName]
		{
			get { return this[variableName]; }
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when a shared variable is added to the collection
		/// </summary>
		public event SharedVariableAddedEventHandler SharedVariableAdded;

		#endregion

		#region Methods

		/// <summary>
		/// Adds the specified element object to the collection.
		/// </summary>
		/// <param name="item">The SharedVariable object to add to the collection</param>
		/// <returns>true if the element was added to the collection, false otherwise</returns>
		public bool Add(SharedVariable item)
		{
			bool result = false;
			rwLock.AcquireWriterLock(-1);
			if ((item != null) && !variables.ContainsKey(item.Name))
			{
				variables.Add(item.Name, item);
				result = true;
			}
			rwLock.ReleaseWriterLock();

			if (result)
				OnSharedVariableAdded(item);
			return result;
		}

		/// <summary>
		/// Determines whether the specified element is in the collection
		/// </summary>
		/// <param name="variableName">The name of the shared variable to search for in the collection</param>
		/// <returns>true if the specified SharedVariable exists in the collection; otherwise, false</returns>
		public bool Contains(string variableName)
		{
			bool result;
			rwLock.AcquireReaderLock(-1);
			result = variables.ContainsKey(variableName);
			rwLock.ReleaseReaderLock();
			return result;
		}

		/// <summary>
		/// Returns the list of variable names
		/// </summary>
		/// <returns>The list of variable names</returns>
		public string[] GetVariableNames()
		{
			string[] names;
			rwLock.AcquireReaderLock(-1);
			names = new string[variables.Count];
			variables.Keys.CopyTo(names, 0);
			rwLock.ReleaseReaderLock();
			return names;
		}

		/// <summary>
		/// Rises the SharedVariableAdded event
		/// </summary>
		/// <param name="variable">The SharedVariable object added to the collection</param>
		protected void OnSharedVariableAdded(SharedVariable variable)
		{
			if (this.SharedVariableAdded != null)
			{
				try { SharedVariableAdded(this, variable); }
				catch { }
			}
		}

		#region ICollection<SharedVariable> Members

		/// <summary>
		/// Adds the specified element object to the collection.
		/// </summary>
		/// <param name="item">The SharedVariable object to add to the collection</param>
		void ICollection<SharedVariable>.Add(SharedVariable item)
		{
			Exception ex = null;
			rwLock.AcquireWriterLock(-1);

			if (item == null)
				ex = new ArgumentNullException();
			else if (variables.ContainsKey(item.Name))
				ex = new ArgumentException("An element with the specified key already exists in the SortedList");
			else
				variables.Add(item.Name, item);
			rwLock.ReleaseWriterLock();
			if (ex != null)
				throw ex;
		}

		/// <summary>
		/// Removes all SharedVariable objects from the current collection
		/// </summary>
		public void Clear()
		{
			rwLock.AcquireWriterLock(-1);
			variables.Clear();
			rwLock.ReleaseWriterLock();
		}

		/// <summary>
		/// Determines whether the specified element is in the collection
		/// </summary>
		/// <param name="item">The SharedVariable to search for in the collection</param>
		/// <returns>true if the specified SharedVariable exists in the collection; otherwise, false</returns>
		public bool Contains(SharedVariable item)
		{
			bool result;
			rwLock.AcquireReaderLock(-1);
			result = variables.ContainsValue(item);
			rwLock.ReleaseReaderLock();
			return result;
		}

		/// <summary>
		/// Determines whether the specified element is in the collection
		/// </summary>
		/// <param name="item">The SharedVariable to search for in the collection</param>
		/// <returns>true if the specified SharedVariable exists in the collection; otherwise, false</returns>
		public bool Contains(ISharedVariable item)
		{
			SharedVariable svItem = item as SharedVariable;
			return this.Contains(svItem);
		}

		/// <summary>
		/// Copies the elements stored in the SharedVariableCollection object to an SharedVariable array, beginning at the specified index location in the System.Array
		/// </summary>
		/// <param name="array">The SharedVariable array to copy the child controls to.</param>
		/// <param name="arrayIndex">The zero-based relative index in array where copying begins</param>
		public void CopyTo(SharedVariable[] array, int arrayIndex)
		{
			rwLock.AcquireReaderLock(-1);
			variables.Values.CopyTo(array, arrayIndex);
			rwLock.ReleaseReaderLock();
		}

		/// <summary>
		/// Removes the specified element from the collection
		/// </summary>
		/// <param name="item">The SharedVariable to be removed</param>
		/// <returns>true if the specified SharedVariable exists in the collection; otherwise, false</returns>
		public bool Remove(SharedVariable item)
		{
			bool result;
			rwLock.AcquireWriterLock(-1);
			result = variables.Remove(item.Name);
			rwLock.ReleaseWriterLock();
			return result;
		}

		#endregion

		#region IEnumerable<ISharedVariable> Members

		/// <summary>
		/// Retrieves an enumerator that can iterate through the SharedVariableCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		IEnumerator<ISharedVariable> IEnumerable<ISharedVariable>.GetEnumerator()
		{

			List<ISharedVariable> vars;
			rwLock.AcquireReaderLock(-1);
			vars = new List<ISharedVariable>(variables.Count);
			for (int i = 0; i < vars.Count; ++i)
				vars[i] = variables.Values[i];
			rwLock.ReleaseReaderLock();
			return vars.GetEnumerator();
		}

		#endregion

		#region IEnumerable<SharedVariable> Members

		/// <summary>
		/// Retrieves an enumerator that can iterate through the SharedVariableCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		public IEnumerator<SharedVariable> GetEnumerator()
		{
			IEnumerator<SharedVariable> enumerator;
			rwLock.AcquireReaderLock(-1);
			enumerator = variables.Values.GetEnumerator();
			rwLock.ReleaseReaderLock();
			return enumerator;
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Retrieves an enumerator that can iterate through the SharedVariableCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			IEnumerator<SharedVariable> enumerator;
			rwLock.AcquireReaderLock(-1);
			enumerator = variables.Values.GetEnumerator();
			rwLock.ReleaseReaderLock();
			return enumerator;
		}

		#endregion

		#endregion
	}
}
