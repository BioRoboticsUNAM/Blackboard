using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace ConfigUtil
{
	class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1) return;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			FrmConfigUtil frmConfigUtil = new FrmConfigUtil();
			Application.Run(frmConfigUtil);
		}
	}
}
