using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Blk.Api;

namespace Blk.Api
{
	/// <summary>
	/// Provides a collection container that enables Modules instances to maintain a list of their Message IPrototypes
	/// </summary>
	public interface IPrototypeCollection : IEnumerable<IPrototype>, ICollection<IPrototype>
	{
		#region Properties

		/// <summary>
		/// Gets the module to which the IPrototypeCollection object belongs
		/// </summary>
		IModuleClient Owner { get; }

		#endregion

		#region Indexers

		/// <summary>
		/// Gets the element at the specified index position
		/// </summary>
		/// <param name="i">The zero based index of the element to get or set</param>
		/// <returns>The module at position i</returns>
		IPrototype this[int i] { get; set; }

		/// <summary>
		/// Gets the element with the specified name
		/// </summary>
		/// <param name="commandName">The name of the module element to get or set</param>
		/// <returns>The module with specified name</returns>
		IPrototype this[string commandName] { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether the specified IPrototype is in the parent Module's IPrototypeCollection object
		/// </summary>
		/// <param name="commandName">The name of the IPrototype to search for in the collection</param>
		/// <returns>true if the specified IPrototype exists in the collection; otherwise, false</returns>
		bool Contains(string commandName);

		/// <summary>
		/// Retrieves the index of a specified IPrototype object in the collection
		/// </summary>
		/// <param name="m">The IPrototype for which the index is returned</param>
		/// <returns>The index of the specified IPrototype. If the IPrototype is not currently a member of the collection, it returns -1</returns>
		int IndexOf(IPrototype m);

		/// <summary>
		/// Retrieves the index of a specified IPrototype object in the collection
		/// </summary>
		/// <param name="commandName">The name of the IPrototype for which the index is returned</param>
		/// <returns>The index of the Movule with the specified name If the IPrototype is not currently a member of the collection, it returns -1</returns>
		int IndexOf(string commandName);

		/// <summary>
		/// Removes a IPrototype, at the specified index location, from the IPrototypeCollection object
		/// </summary>
		/// <param name="index">The ordinal index of the IPrototype to be removed from the collection</param>
		void RemoveAt(int index);

		#endregion
	}
}
