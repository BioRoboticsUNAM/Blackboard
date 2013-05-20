using System;
using System.Xml.Serialization;

namespace Blk.Api
{
	/// <summary>
	/// Encapsulates the data required to start a module
	/// </summary>
	public interface IModuleProcessInfo
	{
		#region Properties

		/// <summary>
		/// Gets the name of the process asociated to this module (used for launch operations)
		/// </summary>
		string ProcessName { get; set; }

		/// <summary>
		/// Gets the path of the program used to launch this module (used for launch operations)
		/// </summary>
		string ProgramPath { get; set; }

		/// <summary>
		/// Gets the arguments used to run this module (used for launch operations)
		/// </summary>
		string ProgramArgs { get; set; }

		#endregion
	}
}
