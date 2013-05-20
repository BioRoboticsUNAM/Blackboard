using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Blk.Api;
using Blk.Engine;
using Robotics.Utilities;

namespace Blk.Engine.Remote
{
	/// <summary>
	/// Manages program execution and termination
	/// </summary>
	public class ProcessManager
	{
		#region Constants

		/// <summary>
		/// Number of attempts to kill a process
		/// </summary>
		public const int KILL_ATTEMPTS = 10;
		#endregion

		#region Variables

		/// <summary>
		/// Object used to dump log
		/// </summary>
		protected readonly ILogWriter log;

		#endregion

		#region Constructors

		/// <summary>
		/// Initialzes a new instance of the ProcessManager object
		/// </summary>
		/// <param name="log">Object used to dump log</param>
		public ProcessManager(ILogWriter log)
		{
			this.log = log;
			//this.oLock = new Object();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the blackboard object to which this instance belongs to
		/// </summary>
		private ILogWriter Log
		{
			get { return this.log; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Request the operating system to close the main window all instances of a process
		/// </summary>
		/// <param name="mpi">The object which contains the information about the process to close</param>
		public bool CloseProcessWindow(IModuleProcessInfo mpi)
		{
			Process[] processesToEnd;
			Stopwatch sw;

			processesToEnd = Process.GetProcessesByName(mpi.ProcessName);
			if (processesToEnd.Length < 1)
				return true;

			// Request close
			Log.WriteLine(6, "Stopping process '" + mpi.ProcessName + "'...");
			foreach (Process p in processesToEnd)
			{
				try
				{
					p.CloseMainWindow();
				}
				catch { }
			}

			// Wait for processes to close
			sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < 1000)
			{
				if (RunningProcessesCount(mpi) == 0)
				{
					Log.WriteLine(6, "Stopping process '" + mpi.ProcessName + "' complete!");
					sw.Stop();
					return true;
				}
				System.Threading.Thread.Sleep(100);
			}
			sw.Stop();
			return false;
		}

		/// <summary>
		/// Closes the main window of all instances of a process. If the window does not close, it Kills the process
		/// </summary>
		/// <param name="mpi">The object which contains the information about the process to kill</param>
		public bool CloseThenKillProcess(IModuleProcessInfo mpi)
		{
			Process[] processesToKill;
			Stopwatch sw;
			int tries = KILL_ATTEMPTS;

			processesToKill = Process.GetProcessesByName(mpi.ProcessName);
			if (processesToKill.Length < 1)
				return true;

			// Request close
			Log.WriteLine(6, "Stopping process '" + mpi.ProcessName + "'...");
			foreach (Process p in processesToKill)
			{
				try
				{
					p.CloseMainWindow();
				}
				catch { }
			}

			// Wait for processes to close
			sw = new Stopwatch();
			sw.Start();
			while (sw.ElapsedMilliseconds < 1000)
			{
				processesToKill = Process.GetProcessesByName(mpi.ProcessName);
				if (processesToKill.Length == 0)
				{
					Log.WriteLine(6, "Stopping process '" + mpi.ProcessName + "' complete!");
					sw.Stop();
					return true;
				}
				System.Threading.Thread.Sleep(100);
			}
			sw.Stop();

			// Kill alive process
			Log.WriteLine(6, "Stopping process '" + mpi.ProcessName + "' failed. Killing the process...");

			do
			{
				foreach (Process p in processesToKill)
				{
					try
					{
						p.Kill();
						p.WaitForExit(500);
					}
					catch
					{
						Log.WriteLine(5, "Kill process '" + mpi.ProcessName + "' failed.");
						return false;
					}
				}
			} while ((RunningProcessesCount(mpi) > 0) && (tries-- > 0));
			return true;
		}

		/// <summary>
		/// Kills all instances of a process
		/// </summary>
		/// <param name="mpi">The object which contains the information about the process to kill</param>
		public bool KillProcess(IModuleProcessInfo mpi)
		{
			Process[] processesToKill;
			int tries = KILL_ATTEMPTS;

			processesToKill = Process.GetProcessesByName(mpi.ProcessName);
			if (processesToKill.Length < 1)
				return true;

			// Kill alive process
			Log.WriteLine(6, "Killing the process '" + mpi.ProcessName + "'...");

			do
			{
				foreach (Process p in processesToKill)
				{
					try
					{
						p.Kill();
						p.WaitForExit(500);
					}
					catch
					{
						Log.WriteLine(5, "Kill process '" + mpi.ProcessName + "' failed.");
						return false;
					}
				}
			} while ((RunningProcessesCount(mpi) > 0) && (tries-- > 0));
			return true;
		}

		/// <summary>
		/// Launches a process
		/// </summary>
		/// <param name="mpi">The object which contains the information about the process to launch</param>
		public bool LaunchProcess(IModuleProcessInfo mpi)
		{
			ProcessStartInfo psi;
			Process p;
			Stopwatch sw;

			if (!File.Exists(mpi.ProgramPath))
			{
				Log.WriteLine(5, "Can not start process '" + mpi.ProcessName + "': File not found.");
				return false;
			}

			Log.WriteLine(6, "Starting process '" + mpi.ProcessName + "'...");
			try
			{
				psi = new ProcessStartInfo(mpi.ProgramPath, mpi.ProgramArgs);
				psi.WorkingDirectory = (new FileInfo(mpi.ProgramPath)).DirectoryName;
				psi.UseShellExecute = true;
				psi.WindowStyle = ProcessWindowStyle.Normal;
				p = new Process();
				p.StartInfo = psi;
				if (!p.Start())
				{
					Log.WriteLine(5, "Can not start process '" + mpi.ProcessName + "': Can not start program.");
					return false;
				}
				sw = new Stopwatch();
				sw.Start();
				while (sw.ElapsedMilliseconds < 10000)
				{
					if (p.WaitForInputIdle(500))
					{
						Log.WriteLine(6, "Process '" + mpi.ProcessName + "' started.");
						sw.Stop();
						return true;
					}
				}
				sw.Stop();
				Log.WriteLine(5, "Starting process '" + mpi.ProcessName + "': timed out.");
			}
			catch (Exception ex)
			{
				Log.WriteLine(5, "Can not start process '" + mpi.ProcessName + "': " + ex.Message);
				return false;
			}
			return false;
		}

		/// <summary>
		/// Launches a process if it is not running
		/// </summary>
		/// <param name="mpi">The object which contains the information about the process to launch</param>
		public bool LaunchProcessIfNotRunning(IModuleProcessInfo mpi)
		{
			//ProcessStartInfo psi;
			//Process p;
			//Stopwatch sw;

			if (this.RunningProcessesCount(mpi) > 0)
			{
				Log.WriteLine(6, "Process '" + mpi.ProcessName + "' is already running");
				return true;
			}

			return this.LaunchProcess(mpi);
		}

		/// <summary>
		/// Gets the number of processes running with the specified module process info
		/// </summary>
		/// <param name="mpi">The object which contains the information about the process to check</param>
		/// <returns>The number of runnin processes</returns>
		public int RunningProcessesCount(IModuleProcessInfo mpi)
		{
			if (mpi == null)
				return 0;
			return Process.GetProcessesByName(mpi.ProcessName).Length;
		}

		#endregion
	}
}
