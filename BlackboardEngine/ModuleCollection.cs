using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
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
		private Dictionary<string, int> modulesDicc;
		private ReaderWriterLock rwLock;

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
			this.modulesDicc = new Dictionary<string, int>(capacity);
			this.rwLock = new ReaderWriterLock();
		}

		/// <summary>
		/// Initializes a new instance of the ModuleCollection class for the specified parent blackboard.
		/// </summary>
		/// <param name="owner">The blackboard that the module collection is created for</param>
		internal ModuleCollection(Blackboard owner) : this(owner, 20)
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
			get
			{
				this.rwLock.AcquireReaderLock(-1);
				if ((i < 0) || (i >= this.modules.Count))
				{
					this.rwLock.ReleaseReaderLock();
					throw new ArgumentOutOfRangeException();
				}
				IModuleClient mc = this.modules[i];
				this.rwLock.ReleaseReaderLock();
				return mc;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				this.rwLock.AcquireWriterLock(-1);
				IModuleClient old = modules[i];
				if (old == value)
				{
					this.rwLock.ReleaseWriterLock();
					return;
				}
				this.modules[i] = value;
				this.rwLock.ReleaseWriterLock();
			}
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
				this.rwLock.AcquireReaderLock(-1);
				if (!this.modulesDicc.ContainsKey(moduleName))
				{
					this.rwLock.ReleaseReaderLock();
					throw new ArgumentException("The module is not currently a member of the collection");
				}
				IModuleClient mc = this.modules[this.modulesDicc[moduleName]];
				this.rwLock.ReleaseReaderLock();
				return mc;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				this.rwLock.AcquireWriterLock(-1);
				if (!this.modulesDicc.ContainsKey(moduleName))
				{
					this.rwLock.ReleaseWriterLock();
					throw new ArgumentException("The module is not currently a member of the collection");
				}
				this.modules[this.modulesDicc[moduleName]] = value;
				this.rwLock.ReleaseWriterLock();
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
			this.rwLock.AcquireReaderLock(-1);
			bool result = this.modulesDicc.ContainsKey(moduleName);
			this.rwLock.ReleaseReaderLock();
			return result;
		}

		/// <summary>
		/// Retrieves the index of a specified Module object in the collection
		/// </summary>
		/// <param name="m">The Module for which the index is returned</param>
		/// <returns>The index of the specified Module. If the Module is not currently a member of the collection, it returns -1</returns>
		public virtual int IndexOf(IModuleClient m)
		{
			this.rwLock.AcquireReaderLock(-1);
			if ((m == null) || !this.modulesDicc.ContainsKey(m.Name))
			{
				this.rwLock.ReleaseReaderLock();
				return -1;
			}
			int result = this.modulesDicc[m.Name];
			this.rwLock.ReleaseReaderLock();
			return result;
		}

		/// <summary>
		/// Retrieves the index of a specified Module object in the collection
		/// </summary>
		/// <param name="moduleName">The name of the Module for which the index is returned</param>
		/// <returns>The index of the Movule with the specified name If the Module is not currently a member of the collection, it returns -1</returns>
		public virtual int IndexOf(string moduleName)
		{
			this.rwLock.AcquireReaderLock(-1);
			if (!this.modulesDicc.ContainsKey(moduleName))
			{
				this.rwLock.ReleaseReaderLock();
				return -1;
			}
			int result = this.modulesDicc[moduleName];
			this.rwLock.ReleaseReaderLock();
			return result;
		}

		/// <summary>
		/// Removes a Module, at the specified index location, from the ModuleCollection object
		/// </summary>
		/// <param name="index">The ordinal index of the Module to be removed from the collection</param>
		public virtual void RemoveAt(int index)
		{
			this.rwLock.AcquireWriterLock(-1);
			if ((index < 0) || (index >= this.modules.Count))
			{
				this.rwLock.ReleaseWriterLock();
				throw new ArgumentOutOfRangeException();
			}
			IModuleClient mc = this.modules[index];
			this.modulesDicc.Remove(mc.Name);
			this.modules.RemoveAt(index);
			for (int i = index; i < this.modules.Count; ++i)
				this.modulesDicc[this.modules[i].Name] = i;
			this.rwLock.ReleaseWriterLock();
			if (ModuleRemoved != null) ModuleRemoved(mc);
		}

		/// <summary>
		/// Returns the string representation of the object
		/// </summary>
		/// <returns>A string representation of the object</returns>
		public override string ToString()
		{
			int i = 0;
			string s = string.Empty;
			this.rwLock.AcquireReaderLock(-1);
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
			this.rwLock.ReleaseReaderLock();
			return s;
		}

		#region ICollection<Module> Members

		/// <summary>
		/// Adds the specified Module object to the collection.
		/// </summary>
		/// <param name="item">The Module to add to the collection</param>
		public void Add(IModuleClient item)
		{
			if (item == null)
				throw new ArgumentNullException();

			this.rwLock.AcquireWriterLock(-1);
			if (this.modulesDicc.ContainsKey(item.Name))
			{
				this.rwLock.ReleaseWriterLock();
				throw new ArgumentException("Another element with the same name exists in the collection");
			}
			item.Parent = owner;
			this.modules.Add(item);
			this.modulesDicc.Add(item.Name, 0);
			modules.Sort();
			for (int i = 0; i < this.modules.Count; ++i)
				this.modulesDicc[this.modules[i].Name] = i;
			this.rwLock.ReleaseWriterLock();
			if (ModuleAdded != null) ModuleAdded(item);
		}

		/// <summary>
		/// Removes all controls from the current Blackboard's ModuleCollection object
		/// </summary>
		public void Clear()
		{
			IModuleClient[] mca;

			this.rwLock.AcquireWriterLock(-1);
			mca = this.modules.ToArray();
			this.modules.Clear();
			this.modulesDicc.Clear();
			this.rwLock.ReleaseWriterLock();
			for (int i = 0; i < mca.Length; ++i)
			{
				mca[i].Parent = null;
				if (ModuleRemoved != null) ModuleRemoved(mca[i]);
			}
		}

		/// <summary>
		/// Determines whether the specified Blackboard is in the parent Blackboard's ModuleCollection object
		/// </summary>
		/// <param name="item">The Module to search for in the collection</param>
		/// <returns>true if the specified Module exists in the collection; otherwise, false</returns>
		public bool Contains(IModuleClient item)
		{
			if (item == null)
				return false;
			this.rwLock.AcquireReaderLock(-1);
			bool result = this.modulesDicc.ContainsKey(item.Name);
			this.rwLock.ReleaseReaderLock();
			return result;
		}

		/// <summary>
		/// Copies the child controls stored in the ModuleCollection object to an Module array object, beginning at the specified index location in the System.Array
		/// </summary>
		/// <param name="array">The Module array to copy the child controls to.</param>
		/// <param name="arrayIndex">The zero-based relative index in array where copying begins</param>
		public void CopyTo(IModuleClient[] array, int arrayIndex)
		{
			this.rwLock.AcquireReaderLock(-1);
			this.modules.CopyTo(array, arrayIndex);
			this.rwLock.ReleaseReaderLock();
		}

		/// <summary>
		/// Removes the specified Module from the parent Blackboard's ModuleCollection object
		/// </summary>
		/// <param name="item">The Module to be removed</param>
		/// <returns>true if the specified Module exists in the collection; otherwise, false</returns>
		public bool Remove(IModuleClient item)
		{
			if (item == null) return false;
			this.rwLock.AcquireWriterLock(-1);
			if (!this.modulesDicc.ContainsKey(item.Name))
			{
				this.rwLock.ReleaseWriterLock(); 
				return false;
		}
			item.Parent = null;
			int index = this.modulesDicc[item.Name];
			this.modulesDicc.Remove(item.Name);
			this.modules.RemoveAt(index);
			for (int i = index; i < this.modules.Count; ++i)
				this.modulesDicc[this.modules[i].Name] = i;
			this.rwLock.ReleaseWriterLock();
			if (ModuleRemoved != null) ModuleRemoved(item);
			return true;
		}

		#endregion

		#region IEnumerable<Module> Members
		/// <summary>
		/// Retrieves an enumerator that can iterate through the ModuleCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		public IEnumerator<IModuleClient> GetEnumerator()
		{
			this.rwLock.AcquireReaderLock(-1);
			IEnumerator<IModuleClient> e = this.modules.GetEnumerator();
			this.rwLock.ReleaseReaderLock();
			return e;
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Retrieves an enumerator that can iterate through the ModuleCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			this.rwLock.AcquireReaderLock(-1);
			IEnumerator e = this.modules.GetEnumerator();
			this.rwLock.ReleaseReaderLock();
			return e;
		}

		#endregion

		#endregion
	}

}
