using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Blk.Api;

namespace Blk.Engine.Actions
{
	/// <summary>
	/// Represents the method that will handle the ActionAdded and ActionRemoved event of a ActionCollection object.
	/// </summary>
	/// <param name="action"></param>
	public delegate void IActionAddRemoveEH(IAction action);

	/// <summary>
	/// Provides a collection container that enables Module instances to maintain a list of their modules
	/// </summary>
	public class ActionCollection : IEnumerable<IAction>, ICollection<IAction>
	{
		#region Variables

		private IModuleClient owner;
		private List<IAction> actions;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the ActionCollection class for the specified parent Module.
		/// </summary>
		/// <param name="owner">The Module that the action collection is created for</param>
		/// <param name="capacity">The number of elements that the new ActionCollection can initially store</param>
		public ActionCollection(IModuleClient owner, int capacity)
		{
			if (owner == null) throw new ArgumentException("Owner cannot be null");
			this.owner = owner;
			this.actions = new List<IAction>(capacity);
		}

		/// <summary>
		/// Initializes a new instance of the ActionCollection class for the specified parent Module.
		/// </summary>
		/// <param name="owner">The Module that the action collection is created for</param>
		public ActionCollection(IModuleClient owner)
			: this(owner, 10)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the Module to which the ActionCollection object belongs
		/// </summary>
		public IModuleClient Owner
		{
			get { return owner; }
		}

		#region ICollection<Action> Members

		/// <summary>
		/// Gets the number of modules in the ActionCollection object for the specified Module.
		/// </summary>
		public int Count
		{
			get { return actions.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the ActionCollection object is read-only
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
		/// <returns>The action at position i</returns>
		public IAction this[int i]
		{
			get { return actions[i]; }
			set { actions[i] = value; }
		}

		#endregion

		#region Events

		/// <summary>
		/// Raises when a IAction is addedd to the ActionCollection
		/// </summary>
		public event IActionAddRemoveEH ActionAdded;
		/// <summary>
		/// Raises when a IAction is removed from the ActionCollection
		/// </summary>
		public event IActionAddRemoveEH ActionRemoved;

		#endregion

		#region Methods

		/// <summary>
		/// Adds the elements of the specified collection to the end of the ActionCollection's List
		/// </summary>
		/// <param name="collection">The collection whose elements should be added to the end of the ActionCollection's list.
		/// The collection itself cannot be a null reference (Nothing in Visual Basic),
		/// but it can contain elements that are a null reference (Nothing in Visual Basic)</param>
		public void AddRange(IEnumerable<IAction> collection)
		{
			if (collection == null) throw new ArgumentNullException("collection");
			foreach (IAction a in collection)
				if (a != null) Add(a);
			//{
			//	//if (a != null) actions.Add(a);
			//	//a.Parent = this.owner;
			//}
		}

		/// <summary>
		/// Gets an array of elements with the specified type
		/// </summary>
		/// <param name="t">The type of the action element to get</param>
		/// <returns>an array of elements with the specified type</returns>
		public IAction[] GetActionsByType(ActionType t)
		{
			List<IAction> actions = new List<IAction>();
			foreach (IAction ia in this.actions)
				if (ia.Type == t) actions.Add(ia);
			return actions.ToArray();
		}

		/// <summary>
		/// Retrieves the index of a specified Action object in the collection
		/// </summary>
		/// <param name="m">The Action for which the index is returned</param>
		/// <returns>The index of the specified Action. If the Action is not currently a member of the collection, it returns -1</returns>
		public virtual int IndexOf(IAction m)
		{
			return actions.IndexOf(m);
		}

		/// <summary>
		/// Removes a Action, at the specified index location, from the ActionCollection object
		/// </summary>
		/// <param name="index">The ordinal index of the Action to be removed from the collection</param>
		public virtual void RemoveAt(int index)
		{
			if (ActionRemoved != null) ActionRemoved(actions[index]);
			actions.RemoveAt(index);
		}

		#region ICollection<IAction> Members

		/// <summary>
		/// Adds the specified Action object to the collection.
		/// </summary>
		/// <param name="item">The Action to add to the collection</param>
		public void Add(IAction item)
		{
			item.Parent = owner;
			if (ActionAdded != null) ActionAdded(item);
			actions.Add(item);
		}

		/// <summary>
		/// Removes all elements from the current ActionCollection object
		/// </summary>
		public void Clear()
		{
			for (int i = 0; i < actions.Count; ++i)
			{
				if (ActionRemoved != null) ActionRemoved(actions[i]);
				actions[i].Parent = null;
			}
			actions.Clear();
		}

		/// <summary>
		/// Determines whether the specified IAction is in the ActionCollection object
		/// </summary>
		/// <param name="item">The Action to search for in the collection</param>
		/// <returns>true if the specified Action exists in the collection; otherwise, false</returns>
		public bool Contains(IAction item)
		{
			return actions.Contains(item);
		}

		/// <summary>
		/// Copies the child controls stored in the ActionCollection object to an Action array object, beginning at the specified index location in the System.Array
		/// </summary>
		/// <param name="array">The Action array to copy the child controls to.</param>
		/// <param name="arrayIndex">The zero-based relative index in array where copying begins</param>
		public void CopyTo(IAction[] array, int arrayIndex)
		{
			actions.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the specified Action from the parent ActionCollection object
		/// </summary>
		/// <param name="item">The Action to be removed</param>
		/// <returns>true if the specified Action exists in the collection; otherwise, false</returns>
		public bool Remove(IAction item)
		{
			item.Parent = null;
			if (actions.Contains(item) && (ActionRemoved != null)) ActionRemoved(item);
			return actions.Remove(item);
		}

		#endregion

		#region IEnumerable<IAction> Members
		/// <summary>
		/// Retrieves an enumerator that can iterate through the ActionCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		public IEnumerator<IAction> GetEnumerator()
		{
			return actions.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Retrieves an enumerator that can iterate through the ActionCollection object
		/// </summary>
		/// <returns>The enumerator to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return actions.GetEnumerator();
		}

		#endregion

		#endregion
	}

}
