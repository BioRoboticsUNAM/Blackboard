using System;
using System.Collections.Generic;
using System.Text;

namespace Blk.Api
{
	/// <summary>
	/// Enumerates the simulations types a Module support
	/// </summary>
	//public enum SimulationOptions { SimulationDisabled, EnabledExact, EnabledWithError };
	public enum SimulationType
	{
		/// <summary>
		/// Simulation is disabled
		/// </summary>
		SimulationDisabled = 0,
		/// <summary>
		/// Simulates and responses match the commands successfully
		/// </summary>
		EnabledExact,
		/// <summary>
		/// Simulates and responses match the commands
		/// </summary>
		EnabledWithError
	};

	/// <summary>
	/// Encapsulates simlation options for a Module
	/// </summary>
	public interface IModuleSimulationOptions
	{
		#region Properties

		/// <summary>
		/// Gets the type of the simulation
		/// </summary>
		SimulationType Type { get; }

		/// <summary>
		/// Gets or sets a value between 0 and 1 that indicates the success ratio of a simulated response. A value greater than 1 disables the simulation.
		/// </summary>
		double SuccessRatio { get; set; }

		/// <summary>
		/// gets a value indicating if the simulation is enabled
		/// </summary>
		bool SimulationEnabled { get; }

		#endregion
	}
}
