using System;
using System.Text.RegularExpressions;
using Robotics.API;

namespace Blk.Engine
{
	/// <summary>
	/// Implements validation methods for names and other blackboard elements
	/// </summary>
	public static class Validator
	{
		#region Variables

		/// <summary>
		/// Regular expression for command name validation
		/// </summary>
		public static readonly Regex RxCommandNameValidator = new Regex(@"^[a-z_][a-z_\d]{1,254}$", RegexOptions.Compiled);
		/// <summary>
		/// Regular expression for command name extraction
		/// </summary>
		public static readonly Regex RxCommandNameXtractor = new Regex(@"(?<commandName>[a-z_][a-z_\d]{1,254})", RegexOptions.Compiled);
		/// <summary>
		/// Regular expression for module name validation
		/// </summary>
		public static readonly Regex RxModuleNameValidator = new Regex(@"^[A-Z][A-Z\d\-]{0,62}[A-Z\d]$", RegexOptions.Compiled);
		/// <summary>
		/// Regular expression for module name extraction
		/// </summary>
		public static readonly Regex RxModuleNameXtractor = new Regex(@"(?<moduleName>[A-Z][A-Z\d\-]{0,62}[A-Z\d])", RegexOptions.Compiled);
		/// <summary>
		/// Regular expression for shared variable name validation
		/// </summary>
		public static readonly Regex RxVariableNameValidator = new Regex(@"^[A-Za-z_][0-9A-Za-z_]{0,63}$", RegexOptions.Compiled);
		/// <summary>
		/// Regular expression for shared variable name extraction
		/// </summary>
		public static readonly Regex RxVariableNameXtractor = new Regex(@"(?<variableName>[A-Za-z_][0-9A-Za-z_]{0,63})", RegexOptions.Compiled);

		#endregion

		#region Methods

		/// <summary>
		/// Gets a value indicating if the provided string is a valid command name
		/// </summary>
		/// <param name="commandName">The provided string to validate</param>
		/// <returns>true if the provided string is a valid command name, false otherwise</returns>
		public static bool IsValidCommandName(string commandName)
		{
			return RxCommandNameValidator.IsMatch(commandName);
		}

		/// <summary>
		/// Gets a value indicating if the provided string is a valid command name
		/// </summary>
		/// <param name="commandName">The provided string to validate</param>
		/// <returns>true if the provided string is a valid module name, false otherwise</returns>
		public static bool IsValidModuleName(string commandName)
		{
			return RxModuleNameValidator.IsMatch(commandName);
		}

		/// <summary>
		/// Gets a value indicating if the provided string is a valid shared variable name
		/// </summary>
		/// <param name="variableName">The provided string to validate</param>
		/// <returns>true if the provided string is a valid shared variable name, false otherwise</returns>
		public static bool IsValidVariableName(string variableName)
		{
			return RxVariableNameValidator.IsMatch(variableName);
		}

		#endregion
	}
}
