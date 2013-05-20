using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigUtil
{
	class ModuleClient
	{
		#region Constants

		/// <summary>
		/// Stores the Regular Expression used to match the known system control commands
		/// </summary>
		protected const string RxControlCommandNames = "bin|busy|alive|ready|restart_test";

		/// <summary>
		/// Maximum check interval for alive, busy and ready checkup messages in milliseconds
		/// </summary>
		public const int MaxCheckInterval = 60000;

		/// <summary>
		/// Minimum check interval for alive, busy and ready checkup messages in milliseconds
		/// </summary>
		public const int MinCheckInterval = 1000;

		#endregion

		#region Static Variables

		/// <summary>
		/// Default interval time for alive, busy and ready checkup messages in milliseconds
		/// </summary>
		private static int globalCheckInterval = 10000;

		#endregion

		#region Static Properties

		/// <summary>
		/// Gets or sets the default interval time for alive, busy and ready checkup messages in milliseconds
		/// </summary>
		public static int GlobalCheckInterval
		{
			get { return globalCheckInterval; }
			set
			{
				if ((value < MinCheckInterval) || (value > MaxCheckInterval))
					throw new ArgumentOutOfRangeException("GlobalCheckInterval must be between 1000 and 60000 milliseconds");
				globalCheckInterval = value;
			}
		}

		#endregion
	}
}
