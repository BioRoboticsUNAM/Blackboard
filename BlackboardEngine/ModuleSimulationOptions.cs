using System;
using System.Collections.Generic;
using System.Text;
using Blk.Api;

namespace Blk.Engine
{
	/// <summary>
	/// Encapsulates simlation options for a Module
	/// </summary>
	public class ModuleSimulationOptions : IModuleSimulationOptions
	{
		#region Variables

		/// <summary>
		/// The type of the simulation
		/// </summary>
		protected SimulationType type;

		/// <summary>
		/// Value between 0 and 1 that indicates the success ratio of a simulated response
		/// </summary>
		protected double successRatio;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of ModuleClientSimulationOptions
		/// </summary>
		public ModuleSimulationOptions()
		{
			type = SimulationType.SimulationDisabled;
		}

		/// <summary>
		/// Initializes a new instance of ModuleClientSimulationOptions
		/// </summary>
		/// <param name="simulated">Indicates if the simulation is enabled</param>
		public ModuleSimulationOptions(bool simulated)
		{
			type = simulated ? SimulationType.EnabledExact : SimulationType.SimulationDisabled;
		}

		/// <summary>
		/// Initializes a new instance of ModuleClientSimulationOptions
		/// </summary>
		/// <param name="successRatio">Value between 0 and 1 that indicates the success ratio of a simulated response. A value greater than one disables the simulation.</param>
		public ModuleSimulationOptions(double successRatio)
		{
			this.SuccessRatio = successRatio;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the type of the simulation
		/// </summary>
		public SimulationType Type
		{
			get { return this.type; }
		}

		/// <summary>
		/// Gets or sets a value between 0 and 1 that indicates the success ratio of a simulated response. A value greater than 1 disables the simulation.
		/// </summary>
		public double SuccessRatio
		{
			get { return this.successRatio; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException();
				else if (value < 1)
				{
					type = SimulationType.EnabledWithError;
					this.successRatio = value;
				}
				else if (value == 1)
				{
					type = SimulationType.EnabledExact;
					this.successRatio = 1;
				}
				else //if(value > 1)
				{
					type = SimulationType.SimulationDisabled;
					this.successRatio = 1;
				}
			}
		}

		/// <summary>
		/// gets a value indicating if the simulation is enabled
		/// </summary>
		public bool SimulationEnabled
		{
			get { return (int)type != 0; }
		}

		/*
		/// <summary>
		/// A ModuleClientSimulationOptions that represents a Simulation Disabled option
		/// </summary>
		private static ModuleClientSimulationOptions simulationDisabled = new ModuleClientSimulationOptions(false);

		/// <summary>
		/// A ModuleClientSimulationOptions that represents a Simulation Enabled with no errors option
		/// </summary>
		private static ModuleClientSimulationOptions simulationWitoutErrors = new ModuleClientSimulationOptions(true);
		*/

		/// <summary>
		/// Gets a ModuleClientSimulationOptions that represents a Simulation Disabled option
		/// </summary>
		public static ModuleSimulationOptions SimulationDisabled
		{
			get { return new ModuleSimulationOptions(false); }
		}

		/// <summary>
		/// Gets a ModuleClientSimulationOptions that represents a Simulation Enabled with no errors option
		/// </summary>
		public static ModuleSimulationOptions SimulationWitoutErrors
		{
			get { return new ModuleSimulationOptions(true); }
		}

		#endregion
	}
}
