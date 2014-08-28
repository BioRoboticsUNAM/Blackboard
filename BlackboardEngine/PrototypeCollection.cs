using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Blk.Api;

namespace Blk.Engine
{
	/// <summary>
	/// Provides a collection container that enables Modules instances to maintain a list of their Message Prototypes
	/// </summary>
	public class PrototypeCollection : IPrototypeCollection
	{
		#region Variables

		private IModuleClient owner;
		private Dictionary<string, IPrototype> prototypes;
		private ReaderWriterLock rwLock;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the PrototypeCollection class for the specified parent module.
		/// </summary>
		/// <param name="owner">The module that the module collection is created for</param>
		/// <param name="capacity">The number of elements that the new ModuleCollection can initially store</param>
		internal PrototypeCollection(IModuleClient owner, int capacity)
		{
			if (owner == null) throw new ArgumentException("Owner cannot be null");
			this.owner = owner;
			this.prototypes = new Dictionary<string, IPrototype>();
			this.rwLock = new ReaderWriterLock();
		}

		/// <summary>
		/// Initializes a new instance of the PrototypeCollection class for the specified parent module.
		/// </summary>
		/// <param name="owner">The module that the module collection is created for</param>
		internal PrototypeCollection(IModuleClient owner)
			: this(owner, 10)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the module to which the PrototypeCollection object belongs
		/// </summary>
		public IModuleClient Owner
		{
			get { return owner; }
		}

		#region ICollection<IPrototype> Members

		/// <summary>
		/// Gets the number of prototypes in the PrototypeCollection object for the specified Module.
		/// </summary>
		public int Count
		{
			get
			{
				int count;
				this.rwLock.AcquireReaderLock(-1);
				count = prototypes.Count;
				this.rwLock.ReleaseReaderLock();
				return count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the PrototypeCollection object is read-only
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		#endregion

		#endregion

		#region Indexers

		/// <summary>
		/// Gets the element with the specified name
		/// </summary>
		/// <param name="commandName">The name of the module element to get or set</param>
		/// <returns>The module with specified name</returns>
		public IPrototype this[string commandName]
		{
			get
			{
				this.rwLock.AcquireReaderLock(-1);
				if (!prototypes.ContainsKey(commandName))
				{
					this.rwLock.ReleaseReaderLock();
					throw new ArgumentException("The prototype is not currently a member of the collection");
				}
				IPrototype proto = this.prototypes[commandName];
				this.rwLock.ReleaseReaderLock();
				return proto;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				this.rwLock.AcquireWriterLock(-1);
				if (value.Parent != owner)
					value.Parent = owner;
				if (!prototypes.ContainsKey(commandName))
					prototypes.Add(value.Command, value);
				else
					prototypes[commandName] = value;
				this.rwLock.ReleaseWriterLock();
				OnPrototypeCollectionStatusChanged();
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the content of the collection changes
		/// </summary>
		public event PrototypeCollectionStatusChangedEH PrototypeCollectionStatusChanged;

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether the specified Prototype is in the parent Module's PrototypeCollection object
		/// </summary>
		/// <param name="commandName">The name of the Prototype to search for in the collection</param>
		/// <returns>true if the specified Prototype exists in the collection; otherwise, false</returns>
		public bool Contains(string commandName)
		{
			this.rwLock.AcquireReaderLock(-1);
			bool result = this.prototypes.ContainsKey(commandName);
			this.rwLock.ReleaseReaderLock();
			return result; 
		}

		/// <summary>
		/// Rises the PrototypeCollectionStatusChanged event
		/// </summary>
		protected void OnPrototypeCollectionStatusChanged()
		{
			if (this.PrototypeCollectionStatusChanged != null)
				this.PrototypeCollectionStatusChanged(this);
		}

		#region ICollection<IPrototype> Members

		/// <summary>
		/// Adds the specified Prototype object to the collection.
		/// </summary>
		/// <param name="item">The Prototype to add to the collection</param>
		public void Add(IPrototype item)
		{
			this.rwLock.AcquireWriterLock(-1);
			item.Parent = owner;
			if (prototypes.ContainsKey(item.Command))
			{
				if (prototypes[item.Command] != item)
					prototypes[item.Command] = item;
			}
			else
				prototypes.Add(item.Command, item);
			this.rwLock.ReleaseWriterLock();
			OnPrototypeCollectionStatusChanged();
		}

		/// <summary>
		/// Removes all controls from the current Module's PrototypeCollection object
		/// </summary>
		public void Clear()
		{
			this.rwLock.AcquireWriterLock(-1);
			foreach (IPrototype proto in prototypes.Values)
				proto.Parent = null;
			prototypes.Clear();
			this.rwLock.ReleaseWriterLock();
			OnPrototypeCollectionStatusChanged();
		}

		/// <summary>
		/// Determines whether the specified Module is in the parent Module's PrototypeCollection object
		/// </summary>
		/// <param name="item">The Prototype to search for in the collection</param>
		/// <returns>true if the specified Prototype exists in the collection; otherwise, false</returns>
		public bool Contains(IPrototype item)
		{
			if (item == null) return false;
			this.rwLock.AcquireReaderLock(-1);
			bool result = this.prototypes.ContainsKey(item.Command);
			this.rwLock.ReleaseReaderLock();
			return result; 
		}

		/// <summary>
		/// Copies the child controls stored in the PrototypeCollection object to an Prototype array object, beginning at the specified index location in the System.Array
		/// </summary>
		/// <param name="array">The Prototype array to copy the child controls to.</param>
		/// <param name="arrayIndex">The zero-based relative index in array where copying begins</param>
		public void CopyTo(IPrototype[] array, int arrayIndex)
		{
			this.rwLock.AcquireReaderLock(-1);
			foreach (IPrototype proto in prototypes.Values)
			{
				if(arrayIndex < array.Length)
					array[arrayIndex++] = proto;
			}
			this.rwLock.ReleaseReaderLock();
		}

		/// <summary>
		/// Removes the specified Prototype from the parent Module's PrototypeCollection object
		/// </summary>
		/// <param name="item">The Prototype to be removed</param>
		/// <returns>true if the specified Prototype exists in the collection; otherwise, false</returns>
		public bool Remove(IPrototype item)
		{
			if (item == null)
				return false;
			return Remove(item.Command);
		}

		/// <summary>
		/// Removes the specified Prototype from the parent Module's PrototypeCollection object
		/// </summary>
		/// <param name="commandName">The Prototype's name to be removed</param>
		/// <returns>true if the specified Prototype exists in the collection; otherwise, false</returns>
		public bool Remove(string commandName)
		{
			if (String.IsNullOrEmpty(commandName))
				return false;
			this.rwLock.AcquireWriterLock(-1);
			if (!this.prototypes.ContainsKey(commandName))
			{
				this.rwLock.ReleaseWriterLock();
				return false;
			}
			bool result = this.prototypes.Remove(commandName);
			this.rwLock.ReleaseWriterLock();
			OnPrototypeCollectionStatusChanged();
			return result;
		}
		
		/// <summary>
		/// Copies the content of the collection to a fixed length array
		/// </summary>
		/// <returns>A copy of the content of the collection</returns>
		public IPrototype[] ToArray()
		{
			this.rwLock.AcquireReaderLock(-1);
			IPrototype[] pArray = new IPrototype[this.prototypes.Count];
			int ix = 0;
			foreach (IPrototype p in this.prototypes.Values)
				pArray[ix++] = p;
			this.rwLock.ReleaseReaderLock();
			Array.Sort(pArray);
			return pArray;
		}

		/// <summary>
		/// Copies the content of the collection to a fixed length array
		/// </summary>
		/// <param name="includeSystemPrototypes">Indicates if system prototypes will be included or excluded</param>
		/// <returns>A copy of the content of the collection</returns>
		public IPrototype[] ToArray(bool includeSystemPrototypes)
		{
			if(includeSystemPrototypes)
				return this.ToArray();

			this.rwLock.AcquireReaderLock(-1);
			int count = 0;
			foreach (IPrototype p in this.prototypes.Values)
			{
				if ((p is Prototype) && ((Prototype)p).IsSystem)
					continue;
				++count;
			}
			IPrototype[] pArray = new IPrototype[count];
			int ix = 0;
			foreach (IPrototype p in this.prototypes.Values)
			{
				if ((p is Prototype) && ((Prototype)p).IsSystem)
					continue;
				pArray[ix++] = p;
			}
			this.rwLock.ReleaseReaderLock();
			Array.Sort(pArray);
			return pArray;
		}

		#endregion

		#region IEnumerable<IPrototype> Members
		/// <summary>
		/// Retrieves an enumerator that can iterate through the PrototypeCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		public IEnumerator<IPrototype> GetEnumerator()
		{
			this.rwLock.AcquireReaderLock(-1);
			IEnumerator<IPrototype> e = this.prototypes.Values.GetEnumerator();
			this.rwLock.ReleaseReaderLock();
			return e;
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Retrieves an enumerator that can iterate through the PrototypeCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			this.rwLock.AcquireReaderLock(-1);
			IEnumerator e = this.prototypes.Values.GetEnumerator();
			this.rwLock.ReleaseReaderLock();
			return e;
		}

		#endregion

		#endregion		
	}
}
