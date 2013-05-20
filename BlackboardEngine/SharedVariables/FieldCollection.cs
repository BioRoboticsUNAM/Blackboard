using System;
using System.Collections.Generic;

namespace Blk.Engine.SharedVariables
{
	public partial class Structure
	{
		/// <summary>
		/// Represents a collection of fields for a structure
		/// </summary>
		public class FieldCollection : IEnumerable<Field>, ICollection<Field>
		{
			#region Variables

			/// <summary>
			/// The Structure object owner of the collection
			/// </summary>
			private Structure owner;

			/// <summary>
			/// The list of fields sorted by name
			/// </summary>
			private List<Field> fields;

			#endregion

			#region Constructor

			/// <summary>
			/// Initializes a new instance of FieldCollection.
			/// </summary>
			/// <param name="owner">The structure to ehich the collection is bound to</param>
			/// <param name="capacity">The number of elements that the new FieldCollection can initially store</param>
			internal FieldCollection(Structure owner, int capacity)
			{
				if (owner == null) throw new ArgumentException("Owner cannot be null");
				this.owner = owner;
				this.fields = new List<Field>(capacity);
			}

			/// <summary>
			/// Initializes a new instance of FieldCollection.
			/// </summary>
			/// <param name="owner">The structure to ehich the collection is bound to</param>
			internal FieldCollection(Structure owner)
				: this(owner, 10)
			{
			}

			#endregion

			#region Properties

			/// <summary>
			/// Gets the structure to which the FieldCollection object belongs
			/// </summary>
			public Structure Owner
			{
				get { return owner; }
			}

			#region ICollection<Field> Members

			/// <summary>
			/// Gets the number of fields in the FieldCollection
			/// </summary>
			public int Count
			{
				get { return fields.Count; }
			}

			/// <summary>
			/// Gets a value indicating whether the FieldCollection object is read-only
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
			/// <returns>The field at position i</returns>
			public Field this[int i]
			{
				get { return fields[i]; }
				set { fields[i] = value; }
			}

			/// <summary>
			/// Gets the element with the specified name
			/// </summary>
			/// <param name="fieldName">The name of the field element to get or set</param>
			/// <returns>The field with specified name</returns>
			public Field this[string fieldName]
			{
				get
				{
					int ix;
					if ((ix = IndexOf(fieldName)) == -1)
						throw new ArgumentException("The field is not currently a member of the collection");
					return fields[ix];
				}
				set
				{
					int ix;
					if ((ix = IndexOf(fieldName)) == -1) throw new ArgumentException("The field is not currently a member of the collection");
					fields[ix] = value;
				}
			}

			#endregion

			#region Methods

			/// <summary>
			/// Determines whether the specified Structure is in the parent Structure's FieldCollection object
			/// </summary>
			/// <param name="fieldName">The name of the Field to search for in the collection</param>
			/// <returns>true if the specified Field exists in the collection; otherwise, false</returns>
			public bool Contains(string fieldName)
			{
				int i;
				for (i = 0; i < fields.Count; ++i)
				{
					if (fields[i].Name == fieldName)
						return true;
				}
				return false;
			}

			/// <summary>
			/// Retrieves the index of a specified Field object in the collection
			/// </summary>
			/// <param name="field">The Field for which the index is returned</param>
			/// <returns>The index of the specified Field. If the Field is not currently a member of the collection, it returns -1</returns>
			public virtual int IndexOf(Field field)
			{
				return fields.IndexOf(field);
			}

			/// <summary>
			/// Retrieves the index of a specified Field object in the collection
			/// </summary>
			/// <param name="fieldName">The name of the Field for which the index is returned</param>
			/// <returns>The index of the Movule with the specified name If the Field is not currently a member of the collection, it returns -1</returns>
			public virtual int IndexOf(string fieldName)
			{
				if (!Contains(fieldName)) return -1;
				for (int i = 0; i < fields.Count; ++i)
				{
					if (fields[i].Name == fieldName)
						return i;
				}
				return -1;
			}

			/// <summary>
			/// Removes a Field, at the specified index location, from the FieldCollection object
			/// </summary>
			/// <param name="index">The ordinal index of the Field to be removed from the collection</param>
			public virtual void RemoveAt(int index)
			{
				fields.RemoveAt(index);
			}

			#region ICollection<Field> Members

			/// <summary>
			/// Adds the specified Field object to the collection.
			/// </summary>
			/// <param name="item">The Field to add to the collection</param>
			public void Add(Field item)
			{
				fields.Add(item);
			}

			/// <summary>
			/// Removes all controls from the current Structure's FieldCollection object
			/// </summary>
			public void Clear()
			{
				fields.Clear();
			}

			/// <summary>
			/// Determines whether the specified Structure is in the parent Structure's FieldCollection object
			/// </summary>
			/// <param name="item">The Field to search for in the collection</param>
			/// <returns>true if the specified Field exists in the collection; otherwise, false</returns>
			public bool Contains(Field item)
			{
				return fields.Contains(item);
			}

			/// <summary>
			/// Copies the child controls stored in the FieldCollection object to an Field array object, beginning at the specified index location in the System.Array
			/// </summary>
			/// <param name="array">The Field array to copy the child controls to.</param>
			/// <param name="arrayIndex">The zero-based relative index in array where copying begins</param>
			public void CopyTo(Field[] array, int arrayIndex)
			{
				fields.CopyTo(array, arrayIndex);
			}

			/// <summary>
			/// Removes the specified Field from the parent Structure's FieldCollection object
			/// </summary>
			/// <param name="item">The Field to be removed</param>
			/// <returns>true if the specified Field exists in the collection; otherwise, false</returns>
			public bool Remove(Field item)
			{
				return fields.Remove(item);
			}

			#endregion

			#region IEnumerable<Field> Members

			/// <summary>
			/// Retrieves an enumerator that can iterate through the FieldCollection object
			/// </summary>
			/// <returns>The enumerator to iterate through the collection.</returns>
			public IEnumerator<Field> GetEnumerator()
			{
				return fields.GetEnumerator();
			}

			#endregion

			#region IEnumerable Members

			/// <summary>
			/// Retrieves an enumerator that can iterate through the FieldCollection object
			/// </summary>
			/// <returns>The enumerator to iterate through the collection.</returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return fields.GetEnumerator();
			}

			#endregion

			#endregion

		}

	}
}