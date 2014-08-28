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

		/*
		/// <summary>
		/// Gets the element at the specified index position
		/// </summary>
		/// <param name="i">The zero based index of the element to get or set</param>
		/// <returns>The module at position i</returns>
		IPrototype this[int i] { get; set; }
		*/

		/// <summary>
		/// Gets the element with the specified name
		/// </summary>
		/// <param name="commandName">The name of the module element to get or set</param>
		/// <returns>The module with specified name</returns>
		IPrototype this[string commandName] { get; set; }

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the content of the collection changes
		/// </summary>
		event PrototypeCollectionStatusChangedEH PrototypeCollectionStatusChanged;

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether the specified IPrototype is in the parent Module's IPrototypeCollection object
		/// </summary>
		/// <param name="commandName">The name of the IPrototype to search for in the collection</param>
		/// <returns>true if the specified IPrototype exists in the collection; otherwise, false</returns>
		bool Contains(string commandName);

		/// <summary>
		/// Copies the content of the collection to a fixed length array
		/// </summary>
		/// <returns>A copy of the content of the collection</returns>
		IPrototype[] ToArray();
		
		/// <summary>
		/// Removes the specified Prototype from the parent Module's PrototypeCollection object
		/// </summary>
		/// <param name="commandName">The Prototype's name to be removed</param>
		/// <returns>true if the specified Prototype exists in the collection; otherwise, false</returns>
		bool Remove(string commandName);

		#endregion
	}
}
