using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Blk.Api
{
	/// <summary>
	/// Manages several IP addresses to connect to a IP address
	/// </summary>
	public interface IServerAddressCollection : ICollection<IPAddress>
	{
		#region Properties

		/// <summary>
		/// Gets the module to which the ServerAddressCollection object belongs
		/// </summary>
		IModule Owner { get; }

		#endregion

		#region Indexers

		/// <summary>
		/// Gets the element at the specified index position
		/// </summary>
		/// <param name="i">The zero based index of the element to get or set</param>
		/// <returns>The IP address at position i</returns>
		IPAddress this[int i] { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Adds the elements of the specified collection to the end of the ServerAddressCollection skipping duplicates
		/// </summary>
		/// <param name="collection">The collection whose elements should be added to the end of the ServerAddressCollection</param>
		void AddRange(IEnumerable<IPAddress> collection);

		/// <summary>
		/// Retrieves the index of a specified IP address object in the collection
		/// </summary>
		/// <param name="address">The IP address for which the index is returned</param>
		/// <returns>The index of the specified IP address. If the IP address is not currently a member of the collection, it returns -1</returns>
		int IndexOf(IPAddress address);

		/// <summary>
		/// Removes a IP address, at the specified index location, from the ServerAddressCollection object
		/// </summary>
		/// <param name="index">The ordinal index of the IP address to be removed from the collection</param>
		void RemoveAt(int index);

		/// <summary>
		/// Copies the elements of the collection to a new array
		/// </summary>
		IPAddress[] ToArray();

		#endregion
	}
}