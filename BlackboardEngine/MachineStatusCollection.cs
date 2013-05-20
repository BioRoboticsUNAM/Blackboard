using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Blk.Engine
{
	/// <summary>
	/// Represents the method that will handle the MachineStatusAdded and MachineStatusRemoved event of a MachineStatusCollection object.
	/// </summary>
	/// <param name="machineStatus">MachineStatus object added or removed</param>
	public delegate void MachineStatusAddRemoveEH(MachineStatus machineStatus);
	/// <summary>
	/// Represents the method that will handle the ElementChanged event of a MachineStatusCollection object.
	/// </summary>
	/// <param name="index">index of changed object</param>
	/// <param name="machineStatus">MachineStatus new value</param>
	public delegate void MachineStatusElementChangedEH(int index, MachineStatus machineStatus);

	/// <summary>
	/// Provides a collection container that enables ModuleBlackboard instances to maintain a list of the status of connected machines
	/// </summary>
	public class MachineStatusCollection: IEnumerable<MachineStatus>, ICollection<MachineStatus>
	{
		#region Variables

		private ModuleBlackboard owner;
		private List<MachineStatus> statusList;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the MachineStatusCollection class for the specified parent blackboard.
		/// </summary>
		/// <param name="owner">The blackboard that the machineStatus collection is created for</param>
		/// <param name="capacity">The number of elements that the new MachineStatusCollection can initially store</param>
		internal MachineStatusCollection(ModuleBlackboard owner, int capacity)
		{
			if (owner == null) throw new ArgumentException("Owner cannot be null");
			this.owner = owner;
			this.statusList = new List<MachineStatus>(capacity);
		}

		/// <summary>
		/// Initializes a new instance of the MachineStatusCollection class for the specified parent blackboard.
		/// </summary>
		/// <param name="owner">The blackboard that the machineStatus collection is created for</param>
		internal MachineStatusCollection(ModuleBlackboard owner) : this(owner, 10)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the blackboard to which the MachineStatusCollection object belongs
		/// </summary>
		internal ModuleBlackboard Owner
		{
			get { return owner; }
		}

		#region ICollection<MachineStatus> Members

		/// <summary>
		/// Gets the number of machineStatuss in the MachineStatusCollection object for the specified ModuleBlackboard.
		/// </summary>
		public int Count
		{
			get { return statusList.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the MachineStatusCollection object is read-only
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
		/// <returns>The machineStatus at position i</returns>
		public MachineStatus this[int i]
		{
			get { return statusList[i]; }
			set
			{
				statusList[i] = value;
				if (ElementChanged != null) ElementChanged(i, value);
			}
		}

		/// <summary>
		/// Gets the element with the specified IP Address
		/// </summary>
		/// <param name="machineStatusIPAddress">The IP Address asociated to the MachineStatus element to get or set</param>
		/// <returns>The machineStatus with specified IP Address</returns>
		public MachineStatus this[IPAddress machineStatusIPAddress]
		{
			get
			{
				int ix;
				if((ix = IndexOf(machineStatusIPAddress)) == -1) throw new ArgumentException("The machineStatus is not currently a member of the collection");
				return statusList[ix];
			}
			set
			{
				int ix;
				if ((ix = IndexOf(machineStatusIPAddress)) == -1) throw new ArgumentException("The machineStatus is not currently a member of the collection");
				statusList[ix] = value;
				if (ElementChanged != null) ElementChanged(ix, value);
			}
		}

		#endregion

		#region Events
		/// <summary>
		/// Raises when a MachineStatus element in the MachineStatusCollection collection is changed
		/// </summary>
		public event MachineStatusElementChangedEH ElementChanged;
		/// <summary>
		/// Raises when a MachineStatus is addedd to the MachineStatusCollection
		/// </summary>
		public event MachineStatusAddRemoveEH MachineStatusAdded;
		/// <summary>
		/// Raises when a MachineStatus is removed from the MachineStatusCollection
		/// </summary>
		public event MachineStatusAddRemoveEH MachineStatusRemoved;

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether the specified MachineStatus is in the MachineStatusCollection object
		/// </summary>
		/// <param name="machineStatusIPAddress">The IP Address asociated to the MachineStatus to search for in the collection</param>
		/// <returns>true if the specified MachineStatus exists in the collection; otherwise, false</returns>
		public bool Contains(IPAddress machineStatusIPAddress)
		{
			int i;
			if (machineStatusIPAddress == null)
				return false;
			for (i = 0; i < statusList.Count; ++i)
			{
				if (statusList[i].IPAddress.Equals(machineStatusIPAddress))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Retrieves the index of a specified MachineStatus object in the collection
		/// </summary>
		/// <param name="m">The MachineStatus for which the index is returned</param>
		/// <returns>The index of the specified MachineStatus. If the MachineStatus is not currently a member of the collection, it returns -1</returns>
		public virtual int IndexOf(MachineStatus m)
		{
			return statusList.IndexOf(m);
		}

		/// <summary>
		/// Retrieves the index of a specified MachineStatus object in the collection
		/// </summary>
		/// <param name="machineStatusIPAddress">The IP Address asociated to the MachineStatus for which the index is returned</param>
		/// <returns>The index of the Movule with the specified name If the MachineStatus is not currently a member of the collection, it returns -1</returns>
		public virtual int IndexOf(IPAddress machineStatusIPAddress)
		{
			if (!Contains(machineStatusIPAddress)) return -1;
			for (int i = 0; i < statusList.Count; ++i)
			{
				if (statusList[i].IPAddress.Equals(machineStatusIPAddress))
					return i;
			}
			return -1;
		}

		/// <summary>
		/// Removes a MachineStatus, at the specified index location, from the MachineStatusCollection object
		/// </summary>
		/// <param name="index">The ordinal index of the MachineStatus to be removed from the collection</param>
		public virtual void RemoveAt(int index)
		{
			if (MachineStatusRemoved != null) MachineStatusRemoved(statusList[index]);
			statusList.RemoveAt(index);
		}

		#region ICollection<MachineStatus> Members

		/// <summary>
		/// Adds the specified MachineStatus object to the collection.
		/// </summary>
		/// <param name="item">The MachineStatus to add to the collection</param>
		public void Add(MachineStatus item)
		{
			if (MachineStatusAdded != null)
			{
				MachineStatusAdded(item);
				statusList.Add(item);
			}
			//statusList.Sort();
		}

		/// <summary>
		/// Removes all controls from the current ModuleBlackboard's MachineStatusCollection object
		/// </summary>
		public void Clear()
		{
			for (int i = 0; i < statusList.Count; ++i)
			{
				if (MachineStatusRemoved != null) MachineStatusRemoved(statusList[i]);
			}
			statusList.Clear();
		}

		/// <summary>
		/// Determines whether the specified ModuleBlackboard is in the parent ModuleBlackboard's MachineStatusCollection object
		/// </summary>
		/// <param name="item">The MachineStatus to search for in the collection</param>
		/// <returns>true if the specified MachineStatus exists in the collection; otherwise, false</returns>
		public bool Contains(MachineStatus item)
		{
			return statusList.Contains(item);
		}

		/// <summary>
		/// Copies the child controls stored in the MachineStatusCollection object to an MachineStatus array object, beginning at the specified index location in the System.Array
		/// </summary>
		/// <param name="array">The MachineStatus array to copy the child controls to.</param>
		/// <param name="arrayIndex">The zero-based relative index in array where copying begins</param>
		public void CopyTo(MachineStatus[] array, int arrayIndex)
		{
			statusList.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the specified MachineStatus from the parent ModuleBlackboard's MachineStatusCollection object
		/// </summary>
		/// <param name="item">The MachineStatus to be removed</param>
		/// <returns>true if the specified MachineStatus exists in the collection; otherwise, false</returns>
		public bool Remove(MachineStatus item)
		{
			if (statusList.Contains(item) && (MachineStatusRemoved != null)) MachineStatusRemoved(item);
			return statusList.Remove(item);
		}

		#endregion

		#region IEnumerable<MachineStatus> Members
		/// <summary>
		/// Retrieves an enumerator that can iterate through the MachineStatusCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		public IEnumerator<MachineStatus> GetEnumerator()
		{
			return statusList.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Retrieves an enumerator that can iterate through the MachineStatusCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return statusList.GetEnumerator();
		}

		#endregion

		#endregion		
	}

}