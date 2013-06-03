using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Blk.Api
{
	/// <summary>
	/// Provides a collection container that enables Blackboard instances to maintain a list of their modules
	/// </summary>
	public interface IModuleCollection : IEnumerable<IModuleClient>, ICollection<IModuleClient>
	{
		#region Properties

		/// <summary>
		/// Gets the blackboard to which the ModuleCollection object belongs
		/// </summary>
		IBlackboard Owner { get; }

		#endregion

		#region Indexers

		/// <summary>
		/// Gets the element at the specified index position
		/// </summary>
		/// <param name="i">The zero based index of the element to get or set</param>
		/// <returns>The module at position i</returns>
		IModuleClient this[int i]{get; set;}

		/// <summary>
		/// Gets the element with the specified name
		/// </summary>
		/// <param name="moduleName">The name of the module element to get or set</param>
		/// <returns>The module with specified name</returns>
		IModuleClient this[string moduleName] { get; set; }

		#endregion

		#region Events

		/// <summary>
		/// Raises when a IModule is addedd to the ModuleCollection
		/// </summary>
		event IModuleAddRemoveEH ModuleAdded;

		/// <summary>
		/// Raises when a IModule is removed from the ModuleCollection
		/// </summary>
		event IModuleAddRemoveEH ModuleRemoved;

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether the specified Blackboard is in the parent Blackboard's ModuleCollection object
		/// </summary>
		/// <param name="moduleName">The name of the Module to search for in the collection</param>
		/// <returns>true if the specified Module exists in the collection; otherwise, false</returns>
		bool Contains(string moduleName);

		/// <summary>
		/// Retrieves the index of a specified Module object in the collection
		/// </summary>
		/// <param name="m">The Module for which the index is returned</param>
		/// <returns>The index of the specified Module. If the Module is not currently a member of the collection, it returns -1</returns>
		int IndexOf(IModuleClient m);

		/// <summary>
		/// Retrieves the index of a specified Module object in the collection
		/// </summary>
		/// <param name="moduleName">The name of the Module for which the index is returned</param>
		/// <returns>The index of the Movule with the specified name If the Module is not currently a member of the collection, it returns -1</returns>
		int IndexOf(string moduleName);

		/// <summary>
		/// Removes a Module, at the specified index location, from the ModuleCollection object
		/// </summary>
		/// <param name="index">The ordinal index of the Module to be removed from the collection</param>
		void RemoveAt(int index);

		#endregion
	}

}
