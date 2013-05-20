using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Blk.Api;

namespace Blk.Engine
{
	/// <summary>
	/// Manages several IP addresses to connect to a IP address
	/// </summary>
	public class ServerAddressCollection : IServerAddressCollection
	{
		#region Variables

		private IModule owner;
		private List<IPAddress> addresses;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the ServerAddressCollection class for the specified parent IModule.
		/// </summary>
		/// <param name="owner">The module that the IP address collection is created for</param>
		/// <param name="capacity">The number of elements that the new ServerAddressCollection can initially store</param>
		internal ServerAddressCollection(IModule owner, int capacity)
		{
			if (owner == null) throw new ArgumentException("Owner cannot be null");
			this.owner = owner;
			this.addresses = new List<IPAddress>(capacity);
		}

		/// <summary>
		/// Initializes a new instance of the ServerAddressCollection class for the specified parent IModule.
		/// </summary>
		/// <param name="owner">The module that the IP address collection is created for</param>
		internal ServerAddressCollection(IModule owner)
			: this(owner, 10)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the module to which the ServerAddressCollection object belongs
		/// </summary>
		public IModule Owner
		{
			get { return owner; }
		}

		#region ICollection<IPAddress> Members

		/// <summary>
		/// Gets the number of modules in the ServerAddressCollection object for the specified IModule.
		/// </summary>
		public int Count
		{
			get { return addresses.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the ServerAddressCollection object is read-only
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
		/// <returns>The IP address at position i</returns>
		public IPAddress this[int i]
		{
			get {
				return (i < addresses.Count) ? this.addresses[i] : null;
			}
			//set { addresses[i] = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds the elements of the specified collection to the end of the ServerAddressCollection skipping duplicates
		/// </summary>
		/// <param name="collection">The collection whose elements should be added to the end of the ServerAddressCollection</param>
		public virtual void AddRange(IEnumerable<IPAddress> collection)
		{
			if(collection == null)
				return;
			foreach (IPAddress ip in collection)
			{
				this.Add(ip);
			}
		}

		/// <summary>
		/// Retrieves the index of a specified IP address object in the collection
		/// </summary>
		/// <param name="address">The IP address for which the index is returned</param>
		/// <returns>The index of the specified IP address. If the IP address is not currently a member of the collection, it returns -1</returns>
		public virtual int IndexOf(IPAddress address)
		{
			return addresses.IndexOf(address);
		}

		/// <summary>
		/// Removes a IP address, at the specified index location, from the ServerAddressCollection object
		/// </summary>
		/// <param name="index">The ordinal index of the IP address to be removed from the collection</param>
		public virtual void RemoveAt(int index)
		{
			addresses.RemoveAt(index);
		}

		/// <summary>
		/// Copies the elements of the collection to a new array
		/// </summary>
		public IPAddress[] ToArray()
		{
			return this.addresses.ToArray();
		}

		/// <summary>
		/// Returns a String that represents the current Object.
		/// </summary>
		/// <returns>A String that represents the current Object.</returns>
		public override string ToString()
		{
			int i;
			string s = String.Empty;

			for (i = 0; i < this.addresses.Count - 1; ++i)
				s += this.addresses[i] + ", ";
			s += this.addresses[i];
			return s;
		}

		#region ICollection<IPAddress> Members

		/// <summary>
		/// Adds the specified IP address object to the collection.
		/// </summary>
		/// <param name="item">The IP address to add to the collection</param>
		public void Add(IPAddress item)
		{
			if (addresses.Contains(item))
				return;
			addresses.Add(item);
		}

		/// <summary>
		/// Removes all controls from the current IModule's ServerAddressCollection object
		/// </summary>
		public void Clear()
		{
			addresses.Clear();
		}

		/// <summary>
		/// Determines whether the specified IPAddress is in the parent IModule's ServerAddressCollection object
		/// </summary>
		/// <param name="item">The IP address to search for in the collection</param>
		/// <returns>true if the specified IP address exists in the collection; otherwise, false</returns>
		public bool Contains(IPAddress item)
		{
			return addresses.Contains(item);
		}

		/// <summary>
		/// Copies the child controls stored in the ServerAddressCollection object to an IP address array object, beginning at the specified index location in the System.Array
		/// </summary>
		/// <param name="array">The IP address array to copy the child controls to.</param>
		/// <param name="arrayIndex">The zero-based relative index in array where copying begins</param>
		public void CopyTo(IPAddress[] array, int arrayIndex)
		{
			addresses.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the specified IP address from the parent IModule ServerAddressCollection object
		/// </summary>
		/// <param name="item">The IP address to be removed</param>
		/// <returns>true if the specified IP address exists in the collection; otherwise, false</returns>
		public bool Remove(IPAddress item)
		{
			return addresses.Remove(item);
		}

		#endregion

		#region IEnumerable<IPAddress> Members
		/// <summary>
		/// Retrieves an enumerator that can iterate through the ServerAddressCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		public IEnumerator<IPAddress> GetEnumerator()
		{
			return addresses.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Retrieves an enumerator that can iterate through the ServerAddressCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return addresses.GetEnumerator();
		}

		#endregion

		#endregion

		#region Operators

		/*

		public static implicit operator IPAddress(ServerAddressCollection collection)
		{
			if (collection == null)
				return null;
			return collection.addresses.Count < 1 ? null : collection[0];
		}

		public static implicit operator ServerAddressCollection(IPAddress address)
		{
			return null;
		}

		*/

		#endregion
	}
}
