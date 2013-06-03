using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Blk.Api;

namespace Blk.Engine
{

	/// <summary>
	/// Provides a collection container that enables Blackboard instances to maintain a list of their modules
	/// </summary>
	public class ModuleCollection: IModuleCollection
	{
		#region Variables

		private Blackboard owner;
		private List<IModuleClient> modules;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the ModuleCollection class for the specified parent blackboard.
		/// </summary>
		/// <param name="owner">The blackboard that the module collection is created for</param>
		/// <param name="capacity">The number of elements that the new ModuleCollection can initially store</param>
		internal ModuleCollection(Blackboard owner, int capacity)
		{
			if (owner == null) throw new ArgumentException("Owner cannot be null");
			this.owner = owner;
			this.modules = new List<IModuleClient>(capacity);
		}

		/// <summary>
		/// Initializes a new instance of the ModuleCollection class for the specified parent blackboard.
		/// </summary>
		/// <param name="owner">The blackboard that the module collection is created for</param>
		internal ModuleCollection(Blackboard owner) : this(owner, 10)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the blackboard to which the ModuleCollection object belongs
		/// </summary>
		public Blackboard Owner
		{
			get { return owner; }
		}

		/// <summary>
		/// Gets the blackboard to which the ModuleCollection object belongs
		/// </summary>
		IBlackboard IModuleCollection.Owner
		{
			get { return owner; }
		}

		#region ICollection<Module> Members

		/// <summary>
		/// Gets the number of modules in the ModuleCollection object for the specified Blackboard.
		/// </summary>
		public int Count
		{
			get { return modules.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the ModuleCollection object is read-only
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
		public IModuleClient this[int i]
		{
			get { return modules[i]; }
			set { modules[i] = value; }
		}

		/// <summary>
		/// Gets the element with the specified name
		/// </summary>
		/// <param name="moduleName">The name of the module element to get or set</param>
		/// <returns>The module with specified name</returns>
		public IModuleClient this[string moduleName]
		{
			get
			{
				int ix;
				if((ix = IndexOf(moduleName.ToUpper())) == -1)
					throw new ArgumentException("The module is not currently a member of the collection");
				return modules[ix];
			}
			set
			{
				int ix;
				if ((ix = IndexOf(moduleName)) == -1) throw new ArgumentException("The module is not currently a member of the collection");
				modules[ix] = value;
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Raises when a IModule is addedd to the ModuleCollection
		/// </summary>
		public event IModuleAddRemoveEH ModuleAdded;
		/// <summary>
		/// Raises when a IModule is removed from the ModuleCollection
		/// </summary>
		public event IModuleAddRemoveEH ModuleRemoved;

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether the specified Blackboard is in the parent Blackboard's ModuleCollection object
		/// </summary>
		/// <param name="moduleName">The name of the Module to search for in the collection</param>
		/// <returns>true if the specified Module exists in the collection; otherwise, false</returns>
		public bool Contains(string moduleName)
		{
			int i;
			for (i = 0; i < modules.Count; ++i)
			{
				if (modules[i].Name.ToUpper() == moduleName.ToUpper())
					return true;
			}
			return false;
		}

		/// <summary>
		/// Retrieves the index of a specified Module object in the collection
		/// </summary>
		/// <param name="m">The Module for which the index is returned</param>
		/// <returns>The index of the specified Module. If the Module is not currently a member of the collection, it returns -1</returns>
		public virtual int IndexOf(IModuleClient m)
		{
			return modules.IndexOf(m);
		}

		/// <summary>
		/// Retrieves the index of a specified Module object in the collection
		/// </summary>
		/// <param name="moduleName">The name of the Module for which the index is returned</param>
		/// <returns>The index of the Movule with the specified name If the Module is not currently a member of the collection, it returns -1</returns>
		public virtual int IndexOf(string moduleName)
		{
			if (!Contains(moduleName)) return -1;
			for (int i = 0; i < modules.Count; ++i)
			{
				if (modules[i].Name.ToUpper() == moduleName.ToUpper())
					return i;
			}
			return -1;
		}

		/// <summary>
		/// Removes a Module, at the specified index location, from the ModuleCollection object
		/// </summary>
		/// <param name="index">The ordinal index of the Module to be removed from the collection</param>
		public virtual void RemoveAt(int index)
		{
			if (ModuleRemoved != null) ModuleRemoved(modules[index]);
			modules.RemoveAt(index);
		}

		/// <summary>
		/// Returns the string representation of the object
		/// </summary>
		/// <returns>A string representation of the object</returns>
		public override string ToString()
		{
			int i = 0;
			string s = string.Empty;

			for (i = 0; i < this.modules.Count - 1; ++i)
			{
				s += this.modules[i].Name;
				if (!modules[i].Enabled)
					s += " (disabled)";
				s += ", ";
			}
			s += this.modules[i].Name;
			if (!modules[i].Enabled)
				s += " (disabled)";
			return s;
		}

		#region ICollection<Module> Members

		/// <summary>
		/// Adds the specified Module object to the collection.
		/// </summary>
		/// <param name="item">The Module to add to the collection</param>
		public void Add(IModuleClient item)
		{
			item.Parent = owner;
			if (ModuleAdded != null) ModuleAdded(item);
			modules.Add(item);
			modules.Sort();
		}

		/// <summary>
		/// Removes all controls from the current Blackboard's ModuleCollection object
		/// </summary>
		public void Clear()
		{
			for (int i = 0; i < modules.Count; ++i)
			{
				if (ModuleRemoved != null) ModuleRemoved(modules[i]);
				modules[i].Parent = null;
			}
			modules.Clear();
		}

		/// <summary>
		/// Determines whether the specified Blackboard is in the parent Blackboard's ModuleCollection object
		/// </summary>
		/// <param name="item">The Module to search for in the collection</param>
		/// <returns>true if the specified Module exists in the collection; otherwise, false</returns>
		public bool Contains(IModuleClient item)
		{
			return modules.Contains(item);
		}

		/// <summary>
		/// Copies the child controls stored in the ModuleCollection object to an Module array object, beginning at the specified index location in the System.Array
		/// </summary>
		/// <param name="array">The Module array to copy the child controls to.</param>
		/// <param name="arrayIndex">The zero-based relative index in array where copying begins</param>
		public void CopyTo(IModuleClient[] array, int arrayIndex)
		{
			modules.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the specified Module from the parent Blackboard's ModuleCollection object
		/// </summary>
		/// <param name="item">The Module to be removed</param>
		/// <returns>true if the specified Module exists in the collection; otherwise, false</returns>
		public bool Remove(IModuleClient item)
		{
			item.Parent = null;
			if (modules.Contains(item) && (ModuleRemoved != null)) ModuleRemoved(item);
			return modules.Remove(item);
		}

		#endregion

		#region IEnumerable<Module> Members
		/// <summary>
		/// Retrieves an enumerator that can iterate through the ModuleCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		public IEnumerator<IModuleClient> GetEnumerator()
		{
			return modules.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Retrieves an enumerator that can iterate through the ModuleCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return modules.GetEnumerator();
		}

		#endregion

		#endregion
	}

}
