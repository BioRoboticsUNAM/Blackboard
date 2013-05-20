using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace BlackboardSatellite
{
	class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1) return;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			FrmSatellite frmSatellite = new FrmSatellite();
			//parseArgs(frmBlackboard, args);
			Application.Run(frmSatellite);
		}
	}
}
