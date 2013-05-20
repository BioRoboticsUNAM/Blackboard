using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;

namespace Blk.Api.SharedVariables
{
	/// <summary>
	/// Represents a stored shared variable
	/// </summary>
	public interface ISharedVariable
	{
		#region Properties

		/// <summary>
		/// Gets the list of names of modules which are allowed to write to the var
		/// </summary>
		string[] AllowedWriters { get; }

		/// <summary>
		/// Gets the creation time of the shared variable
		/// </summary>
		DateTime CreationTime { get; }

		/// <summary>
		/// Gets a value indicating if the SharedVariable can be dynamically resized
		/// </summary>
		bool Dynamic { get; }

		/// <summary>
		/// Gets a value indicating if the variable is an array
		/// </summary>
		bool IsArray { get; }

		/// <summary>
		/// Gets the name of the SharedVariable
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the IModule object which this SharedVariable object is bound to
		/// </summary>
		IModule Owner { get; }

		/// <summary>
		/// Gets the size of the SharedVariable
		/// </summary>
		int Size { get; }

		/*
		/// <summary>
		/// Gets the collection of subscriptions asociated to this SharedVariable object
		/// </summary>
		public SharedVariableSubscriptionList Subscriptions { get; }
		*/

		/// <summary>
		/// Gets the type of the variable
		/// </summary>
		string Type { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Gets a standard SharedVariableInfo for serialization
		/// </summary>
		/// <returns></returns>
		Robotics.API.SharedVariableInfo GetInfo();

		#endregion

	}
}
