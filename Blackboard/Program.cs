using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace Blk.Gui
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			string pName = Process.GetCurrentProcess().ProcessName;
			Process[] p = Process.GetProcessesByName(pName);
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1) return;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
				FrmBlackboard frmBlackboard = new FrmBlackboard();
				parseArgs(frmBlackboard, args);
				Application.Run(frmBlackboard);
		}

		private static void parseArgs(FrmBlackboard frmBlackboard, string[] args)
		{
			if (File.Exists(Application.StartupPath + "\\bb.xml")) frmBlackboard.ConfigFile = Application.StartupPath + "\\bb.xml";
			if (frmBlackboard.LogFile == "") frmBlackboard.LogFile = Application.StartupPath + "\\log.log.xml";
			if ((args.Length < 1) || !File.Exists(args[0])) return;
			frmBlackboard.ConfigFile = args[0];			

			for (int i = 1; i < args.Length; ++i)
			{

				switch (args[i].ToLower())
				{
					case "autostart":
						frmBlackboard.AutoStart = true;
						break;

					case "-h":
					case "--h":
					case "-help":
					case "--help":
					case "/h":
						showHelp();
						break;

					case "-log":
						if (++i > args.Length) return;
						if (File.Exists(args[i])) frmBlackboard.LogFile = args[i];
						else frmBlackboard.LogFile = args[i];
						break;
				}
			}
		}

		private static void showHelp()
		{
			// -a 127.0.0.1 -r 2001 -w 2000 -sim
			Console.WriteLine("Blackboard Help");
			Console.WriteLine("Use blackboard.exe [configFile] [-log logFile] [-autostart]");
			Console.WriteLine("configFile\tBlackboard configuration file");
			Console.WriteLine("logFile\tBlackboard log file");
			Console.WriteLine("autostart\tAutoatically starts blackboard");
		}
	}
}