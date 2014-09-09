using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

using Robotics.Utilities;
using Blk.Api;
using Blk.Engine;

namespace Raspboard
{
	class Program
	{
		
		/// <summary>
		/// Stores the XML configuration file path
		/// </summary>
		private string configFile;
		
		/// <summary>
		/// Stores the path of the log file
		/// </summary>
		private string logFile;

		/// <summary>
		/// The blackboard itself
		/// </summary>
		private Blackboard blackboard;
		
		/// <summary>
		/// Blackboard log object
		/// </summary>
		private LogWriter log;

		/// <summary>
		/// Log manager. Writes to file and textbox
		/// </summary>
		private ConsoleLogWriter cLog;

		/// <summary>
		/// Handles console input
		/// </summary>
		private ConsoleManager consoleManager;

		/// <summary>
		/// Object that contains all the console commands
		/// </summary>
		private Kernel kernel;

		/// <summary>
		/// Ctor
		/// </summary>
		private Program()
		{
			this.logFile = null;
			this.configFile = "bb.xml";
		}
		
		/// <summary>
		/// Loads the Blackboard from the config file
		/// </summary>
		/// <returns>true if the Blackboard was leaded successfully, false otherwise</returns>
		private bool LoadBlackboard()
		{
			FileInfo fi;
			try
			{
				fi = new FileInfo(this.configFile);
				this.configFile = fi.FullName;

				SetupLogfile();
				log.WriteLine("Loading Blackboard");
				this.blackboard = Blackboard.FromXML(this.configFile, log);
				this.blackboard.StartupSequence.Method = Blk.Engine.Remote.ModuleStartupMethod.None;
				blackboard.VerbosityLevel = 1;
				
				SetupBlackboardModules();
				log.WriteLine("Blackboard loaded");
			}
			catch (Exception ex)
			{
				log.WriteLine("Error loading blackboard");
				log.WriteLine("\t" + ex.Message.Replace("\n", "\n\t"));
				return false;
			}
			return true;
			
		}

		/// <summary>
		/// Parses the input arguments and configures the blackboard
		/// </summary>
		/// <param name="args">Arguments passed to the application</param>
		/// <returns>true if arguments weew successfully parsed, false otherwise</returns>
		private bool ParseArgs(string[] args)
		{
			if ((args.Length == 0) || !File.Exists(args[0])) return false;

			this.configFile = args[0];

			for (int i = 1; i < args.Length; ++i)
			{

				switch (args[i].ToLower())
				{

					case "-h":
					case "--h":
					case "-help":
					case "--help":
					case "/h":
						return false;

					case "-log":
						if (++i > args.Length) return false;
						this.logFile = args[i];
						break;
				}
			}
			return true;
		}

		/// <summary>
		/// Keep running the application
		/// </summary>
		private void Run()
		{
			if (!LoadBlackboard())
				return;
			this.log.WriteLine("Starting Blackboard");
			this.blackboard.Start();
			Program.running = true;

			this.kernel = new Kernel(blackboard);
			this.consoleManager = new ConsoleManager(blackboard, kernel, cLog);
			this.consoleManager.DisableLog();
			kernel.ListModules();
			kernel.Execute("help");
			consoleManager.WritePrompt();
			while (this.blackboard.IsRunning)
			{
				consoleManager.Poll();
			}
			Console.WriteLine("Good bye!");
			Console.WriteLine();
		}

		/// <summary>
		/// Adds event handlers for a change in the status of the modules.
		/// </summary>
		private void SetupBlackboardModules()
		{
			//foreach (IModuleClient im in blackboard.Modules)
			//{
			//    im.StatusChanged += new StatusChangedEH(module_StatusChanged);
			//}
		}

		/// <summary>
		/// Configures the Log file
		/// </summary>
		private void SetupLogfile()
		{
			Console.Clear();

			if (String.IsNullOrEmpty(this.logFile))
			{
				this.logFile = "log " + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + ".xml";
				if (!Directory.Exists("logs"))
				{
					try { Directory.CreateDirectory("logs"); }
					catch { }
				}
				if (Directory.Exists("logs"))
					this.logFile = Path.Combine("logs", this.logFile);
			}

			FileInfo fi = new FileInfo(this.logFile);
			this.logFile = fi.FullName;

			cLog = new ConsoleLogWriter(this.logFile);
			cLog.AppendDate = true;
			cLog.DefaultPriority = 1;
			cLog.ConsoleVerbosityThreshold = 5;
			cLog.FileVerbosityThreshold = 5;

			this.log = new LogWriter(cLog);
			this.log.DefaultVerbosity = 1;
			this.log.VerbosityTreshold = 5;
			if (this.blackboard != null)
				this.blackboard.Log = this.log;
		}

		/// <summary>
		/// Prints help
		/// </summary>
		private static void ShowHelp()
		{
			Console.WriteLine("Raspboard Help");
			Console.WriteLine("Use {0} configFile [-log logFile]", Process.GetCurrentProcess().ProcessName);
			Console.WriteLine("configFile\tBlackboard configuration file");
			Console.WriteLine("logFile\tBlackboard log file");
		}

		/// <summary>
		/// Stops the Raspboard execution
		/// </summary>
		private void Stop()
		{
			Console.WriteLine();
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Stopping Blackboard");
			Console.ForegroundColor = color;
			Program.running = false;
			this.blackboard.Stop();
		}

		#region Static Members

		/// <summary>
		/// Flag used to keep the program running
		/// </summary>
		private static bool running;

		/// <summary>
		/// Program application
		/// </summary>
		private static Program program;

		/// <summary>
		/// Main entry point
		/// </summary>
		/// <param name="args">Arguments passed to the application</param>
		public static void Main(string[] args)
		{
			if (!IsFirstInstance){
				Console.WriteLine("Another instance of {0} is already running. Aborted.", Process.GetCurrentProcess().ProcessName);
				return;
			}

			program = new Program();
			if (!program.ParseArgs(args))
			{
				ShowHelp();
				return;
			}
			Console.CancelKeyPress += new ConsoleCancelEventHandler(OnCtrlC);
			program.Run();
		}

		/// <summary>
		/// Validates that there is only one instance of the Raspboard running
		/// </summary>
		private static bool IsFirstInstance
		{
			get
			{
				string pName = Process.GetCurrentProcess().ProcessName;
				Process[] p = Process.GetProcessesByName(pName);
#if DEBUG
				return true;
#else
			return Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length == 1;
#endif
			}
		}

		/// <summary>
		/// Handles de CTRL+C event to safely abort the program
		/// </summary>
		/// <param name="sender">Not used</param>
		/// <param name="e">Handling args</param>
		private static void OnCtrlC(object sender, ConsoleCancelEventArgs e)
		{
			if (running)
			{
				e.Cancel = true;
				program.Stop();
			}
		}

		#endregion
	}
}
