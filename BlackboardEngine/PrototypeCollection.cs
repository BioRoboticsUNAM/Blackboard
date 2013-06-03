using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
		private List<IPrototype> prototypes;

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
			this.prototypes = new List<IPrototype>();
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
			get { return prototypes.Count; }
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
		/// Gets the element at the specified index position
		/// </summary>
		/// <param name="i">The zero based index of the element to get or set</param>
		/// <returns>The module at position i</returns>
		public IPrototype this[int i]
		{
			get { return prototypes[i]; }
			set { prototypes[i] = value; }
		}

		/// <summary>
		/// Gets the element with the specified name
		/// </summary>
		/// <param name="commandName">The name of the module element to get or set</param>
		/// <returns>The module with specified name</returns>
		public IPrototype this[string commandName]
		{
			get
			{
				int ix;
				if ((ix = IndexOf(commandName)) == -1) throw new ArgumentException("The prototype is not currently a member of the collection");
				return prototypes[ix];
			}
			set
			{
				int ix;
				if ((ix = IndexOf(commandName)) == -1) throw new ArgumentException("The prototype is not currently a member of the collection");
				prototypes[ix] = value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether the specified Prototype is in the parent Module's PrototypeCollection object
		/// </summary>
		/// <param name="commandName">The name of the Prototype to search for in the collection</param>
		/// <returns>true if the specified Prototype exists in the collection; otherwise, false</returns>
		public bool Contains(string commandName)
		{
			int i;
			for (i = 0; i < prototypes.Count; ++i)
			{
				if (prototypes[i].Command.ToUpper() == commandName.ToUpper())
					return true;
			}
			return false;
		}

		/// <summary>
		/// Retrieves the index of a specified Prototype object in the collection
		/// </summary>
		/// <param name="m">The Prototype for which the index is returned</param>
		/// <returns>The index of the specified Prototype. If the Prototype is not currently a member of the collection, it returns -1</returns>
		public virtual int IndexOf(IPrototype m)
		{
			return prototypes.IndexOf(m);
		}

		/// <summary>
		/// Retrieves the index of a specified Prototype object in the collection
		/// </summary>
		/// <param name="commandName">The name of the Prototype for which the index is returned</param>
		/// <returns>The index of the Movule with the specified name If the Prototype is not currently a member of the collection, it returns -1</returns>
		public virtual int IndexOf(string commandName)
		{
			if (!Contains(commandName)) return -1;
			for (int i = 0; i < prototypes.Count; ++i)
			{
				if (prototypes[i].Command.ToUpper() == commandName.ToUpper())
					return i;
			}
			return -1;
		}

		/// <summary>
		/// Removes a Prototype, at the specified index location, from the PrototypeCollection object
		/// </summary>
		/// <param name="index">The ordinal index of the Prototype to be removed from the collection</param>
		public virtual void RemoveAt(int index)
		{
			prototypes.RemoveAt(index);
		}

		#region ICollection<IPrototype> Members

		/// <summary>
		/// Adds the specified Prototype object to the collection.
		/// </summary>
		/// <param name="item">The Prototype to add to the collection</param>
		public void Add(IPrototype item)
		{
			item.Parent = owner;
			prototypes.Add(item);
			prototypes.Sort();
		}

		/// <summary>
		/// Removes all controls from the current Module's PrototypeCollection object
		/// </summary>
		public void Clear()
		{
			for (int i = 0; i < prototypes.Count; ++i)
				prototypes[i].Parent = null;
			prototypes.Clear();
		}

		/// <summary>
		/// Determines whether the specified Module is in the parent Module's PrototypeCollection object
		/// </summary>
		/// <param name="item">The Prototype to search for in the collection</param>
		/// <returns>true if the specified Prototype exists in the collection; otherwise, false</returns>
		public bool Contains(IPrototype item)
		{
			return prototypes.Contains(item);
		}

		/// <summary>
		/// Copies the child controls stored in the PrototypeCollection object to an Prototype array object, beginning at the specified index location in the System.Array
		/// </summary>
		/// <param name="array">The Prototype array to copy the child controls to.</param>
		/// <param name="arrayIndex">The zero-based relative index in array where copying begins</param>
		public void CopyTo(IPrototype[] array, int arrayIndex)
		{
			prototypes.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the specified Prototype from the parent Module's PrototypeCollection object
		/// </summary>
		/// <param name="item">The Prototype to be removed</param>
		/// <returns>true if the specified Prototype exists in the collection; otherwise, false</returns>
		public bool Remove(IPrototype item)
		{
			item.Parent = null;
			return prototypes.Remove(item);
		}

		#endregion

		#region IEnumerable<IPrototype> Members
		/// <summary>
		/// Retrieves an enumerator that can iterate through the PrototypeCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		public IEnumerator<IPrototype> GetEnumerator()
		{
			return prototypes.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Retrieves an enumerator that can iterate through the PrototypeCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return prototypes.GetEnumerator();
		}

		#endregion

		#endregion		
	}
}
