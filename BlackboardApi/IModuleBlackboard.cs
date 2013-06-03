using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Text;
using System.Xml;
using Robotics;
using Blk.Api.SharedVariables;

namespace Blk.Api
{
	/// <summary>
	/// Implements a ModuleBlackboard
	/// </summary>
	public interface IModuleBlackboard : IModuleClient
	{
		#region Properties

		/// <summary>
		/// Gets the list of shared variables
		/// </summary>
		ISharedVariableCollection SharedVariables { get; }

		/*
		/// <summary>
		/// Gets the collection of status of each connected machine.
		/// </summary>
		MachineStatusCollection MachineStatusList { get; }
		*/

		#endregion
	}
}

