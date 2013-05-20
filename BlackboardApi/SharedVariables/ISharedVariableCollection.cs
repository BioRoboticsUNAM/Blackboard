using System;
using System.Collections.Generic;
using System.Threading;

namespace Blk.Api.SharedVariables
{

	/// <summary>
	/// Provides a collection container that enables ModuleBlackboard instances to maintain a list of their shared variables
	/// allowing multiple readers and single writer on the collection
	/// </summary>
	public interface ISharedVariableCollection : IEnumerable<ISharedVariable>
	{
		#region Properties

		/// <summary>
		/// Gets the list of names of the variables in the collection separated by commas
		/// </summary>
		string NameList { get; }

		/// <summary>
		/// Gets the number of elements in the ISharedVariableCollection object.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets a value indicating whether the ISharedVariableCollection object is read-only
		/// </summary>
		bool IsReadOnly { get; }

		#endregion

		#region Indexers

		/// <summary>
		/// Gets the element at the specified index position
		/// </summary>
		/// <param name="index">The zero based index of the element to get</param>
		/// <returns>The module at position i</returns>
		ISharedVariable this[int index] { get; }

		/// <summary>
		/// Gets the element with the specified name
		/// </summary>
		/// <param name="variableName">The name of the element to get</param>
		/// <returns>The variable with specified name</returns>
		ISharedVariable this[string variableName] { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether the specified element is in the collection
		/// </summary>
		/// <param name="item">The ISharedVariable to search for in the collection</param>
		/// <returns>true if the specified ISharedVariable exists in the collection; otherwise, false</returns>
		bool Contains(ISharedVariable item);

		/// <summary>
		/// Determines whether the specified element is in the collection
		/// </summary>
		/// <param name="variableName">The name of the shared variable to search for in the collection</param>
		/// <returns>true if the specified SharedVariable exists in the collection; otherwise, false</returns>
		bool Contains(string variableName);

		/// <summary>
		/// Returns the list of variable names
		/// </summary>
		/// <returns>The list of variable names</returns>
		string[] GetVariableNames();

		#endregion


	}
}
