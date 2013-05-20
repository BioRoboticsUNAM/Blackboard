using System;
using System.IO;

namespace Blk.Engine
{
	/// <summary>
	/// Represents a class which can load a set of shared variables
	/// </summary>
	public interface ISharedVariableLoader
	{

		/// <summary>
		/// Loads a set of shared variables from a file
		/// </summary>
		/// <param name="filePath">The path of file which contains the shared variable set</param>
		/// <param name="blackboard">The blackboard in which shared variables will be loaded</param>
		void FromFile(string filePath, Blackboard blackboard);

		/// <summary>
		/// Loads a set of shared variables from a stream
		/// </summary>
		/// <param name="stream">The stream which contains the shared variable set</param>
		/// <param name="blackboard">The blackboard in which shared variables will be loaded</param>
		void FromStream(Stream stream, Blackboard blackboard);

		/// <summary>
		/// Loads a set of shared variables from a string
		/// </summary>
		/// <param name="s">The string which contains the shared variable set</param>
		/// <param name="blackboard">The blackboard in which shared variables will be loaded</param>
		void FromString(string s, Blackboard blackboard);
	}
}
