using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Robotics;
using Blk.Api;
using Blk.Engine;
using Blk.Engine.SharedVariables;

namespace Raspboard
{
	/// <summary>
	/// Encapsulates all console commands
	/// </summary>
	public class Kernel
	{
		#region Variables

		/// <summary>
		/// The blackboard itself
		/// </summary>
		private Blackboard blackboard;

		/// <summary>
		/// Command dictionary
		/// </summary>
		private Dictionary<string, StringEventHandler> commands;

		/// <summary>
		/// Stores the list of system prototypes
		/// </summary>
		private Dictionary<string, int> systemPrototypes;

		/// <summary>
		/// Regular expression used to split commands
		/// </summary>
		private Regex rxCommandSplitter;

		/// <summary>
		/// Handles console input
		/// </summary>
		private ConsoleManager consoleManager;

		#endregion

		#region Consutructor

		public Kernel(Blackboard blk)
		{
			this.blackboard = blk;
			this.commands = new Dictionary<string, StringEventHandler>();
			this.rxCommandSplitter = new Regex(@"^(?<cmd>\w+)(\s+(?<par>.*))?$");
			RegisterCommands();
			RegisterSystemPrototypes();
		}

		#endregion

		#region Properties

		/// <summary>
		/// The console manager (used to enable/disable log mode
		/// </summary>
		public ConsoleManager ConsoleManager
		{
			get { return this.consoleManager; }
			set { this.consoleManager = value; }
		}

		#endregion

		#region Methods

		public bool Execute(string command)
		{
			Match m = rxCommandSplitter.Match(command);
			if (!m.Success)
				return false;
			string c = m.Result("${cmd}");
			string p = m.Result("${par}");
			if (!this.commands.ContainsKey(c))
				return false;
			this.commands[c](p);
			return true;
		}

		internal void FillCompletionTree(CompletionTree completionTree)
		{
			foreach (string command in this.commands.Keys)
			{
				completionTree.AddWord(command);
				completionTree.AddWord(String.Format("help {0}", command));
			}

			foreach (IModuleClient m in this.blackboard.Modules)
			{
				if (m == this.blackboard.VirtualModule)
					continue;
				// completionTree.AddWord("list " + m.Name + " commands");
				completionTree.AddWord("info " + m.Name);
				completionTree.AddWord("sim " + m.Name);
				// completionTree.AddWord("put " + m.Name);
				foreach(IPrototype proto in m.Prototypes)
				{
					string par = proto.ParamsRequired ? " \"0\"" : String.Empty;
					//completionTree.AddWord(String.Format("put {0} {1}{2}", m.Name, proto.Command, par));
					completionTree.AddWord(String.Format("put {0}{1}", proto.Command, par));
				}
			}

			completionTree.AddWord("list modules");
			completionTree.AddWord("list commands");
			completionTree.AddWord("list vars");

			foreach (SharedVariable sv in this.blackboard.VirtualModule.SharedVariables)
			{
				completionTree.AddWord("cat " + sv.Name);
				completionTree.AddWord("read " + sv.Name);
			}
		}

		public void RegisterCommands()
		{
			commands.Add("cat",  new StringEventHandler(ReadCommand));
			commands.Add("exit", new StringEventHandler(QuitCommand));
			commands.Add("help", new StringEventHandler(HelpCommand));
			commands.Add("info", new StringEventHandler(InfoCommand));
			commands.Add("list", new StringEventHandler(ListCommand));
			commands.Add("log",  new StringEventHandler(LogCommand));
			commands.Add("put",  new StringEventHandler(PutCommand));
			commands.Add("quit", new StringEventHandler(QuitCommand));
			commands.Add("read", new StringEventHandler(ReadCommand));
			commands.Add("sim",  new StringEventHandler(SimCommand));
		}

		private void RegisterSystemPrototypes()
		{
			this.systemPrototypes = new Dictionary<string, int>();
			foreach (IPrototype p in blackboard.VirtualModule.Prototypes)
				systemPrototypes.Add(p.Command, 0);
		}

		#endregion

		#region General commands

		public void HelpCommand(string s)
		{
			Help.ShowHelp(s, this.commands.Keys);
		}

		public void InfoCommand(string s)
		{
			ConsoleColor color;

			if (!this.blackboard.Modules.Contains(s))
			{
				Console.WriteLine("Unknown module {0}", s);
				return;
			}
			IModuleClient m = this.blackboard.Modules[s];
			WriteModule(m);
			if(!String.IsNullOrEmpty(m.Alias) && (m.Alias != m.Name))
				Console.WriteLine("Alias:  {0}", m.Alias);
			if (!m.Enabled) return;

			if (m.Simulation.SimulationEnabled)
			{
				color = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Simulation enabled");
				Console.ForegroundColor = color;
			}

			IModuleClientTcp tcpModule = m as IModuleClientTcp;
			if (tcpModule != null)
			{
				color = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine();
				Console.WriteLine("TCP information:");
				Console.ForegroundColor = color;
				StringBuilder sb = new StringBuilder(100);
				foreach (System.Net.IPAddress a in tcpModule.ServerAddresses)
				{
					sb.Append(a.ToString());
					sb.Append(", ");
				}
				if(sb.Length > 2)
					sb.Length -= 2;
				Console.WriteLine("TCP Port:  {0}", tcpModule.Port);
				Console.WriteLine("Addresses: {0}", sb.ToString());
			}

			if (m.ProcessInfo != null)
			{
				color = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine();
				Console.WriteLine("Process information:");
				Console.ForegroundColor = color;
				Console.WriteLine("Process Name: {0}", m.ProcessInfo.ProcessName);
				Console.WriteLine("Program Path: {0}", m.ProcessInfo.ProgramPath);
				Console.WriteLine("Program Args: {0}", m.ProcessInfo.ProgramArgs);
			}

			color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine();
			Console.WriteLine("Prototypes (supported commands):");
			Console.ForegroundColor = color;
			WriteModulePrototypes(m);
			Console.WriteLine();
		}

		public void ListCommand(string s)
		{
			if (String.IsNullOrEmpty(s))
			{
				WriteCommandOptions("list", "modules", "vars", "commands", "<module name> commands");
				return;
			}

			switch (s.ToLower())
			{
				case "modules":
					ListModules();
					return;

				case "variables":
				case "vars":
					ListVariables();
					return;

				case "commands":
					ListPrototypes();
					return;
			}
		}

		public void LogCommand(string s)
		{
			if (this.consoleManager == null)
				return;

			if (String.IsNullOrEmpty(s))
			{
				this.consoleManager.EnableLog(3);
				return;
			}

			int verbosity;
			if (!Int32.TryParse(s, out verbosity) || (verbosity < 0) || (verbosity > 9))
			{
				Console.WriteLine("Invalid verbosity level. Values must be between 1 and 9");
				return;
			}
			if (verbosity == 0)
				this.consoleManager.DisableLog();
			else
				this.consoleManager.EnableLog(verbosity);
		}		

		public void PutCommand(string s)
		{
			bool result;
			string src = String.Empty;
			int spacePos = s.IndexOf(' ');
			if (spacePos > 0)
			{
				src = s.Substring(0, spacePos);
				if (!blackboard.Modules.Contains(src))
					src = blackboard.VirtualModule.Name + " ";
				else
					src = String.Empty;
			}
			result = blackboard.Inject(src + s);
			Console.WriteLine("Injection {0}", result ? "succeeded" : "failed");
		}

		public void QuitCommand(string s)
		{
			this.blackboard.Stop();
		}

		public void ReadCommand(string s)
		{
			if (!this.blackboard.VirtualModule.SharedVariables.Contains(s))
			{
				Console.WriteLine("Unknown shared variable {0}", s);
				return;
			}
			SharedVariable sv = this.blackboard.VirtualModule.SharedVariables[s];
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("{0}{1} {2} = ", sv.Type, sv.IsArray ? "[]" : String.Empty, sv.Name);
			Console.ForegroundColor = color;
			Console.WriteLine(sv.ReadStringData());
		}

		public void SimCommand(string s)
		{
			if (!this.blackboard.Modules.Contains(s))
			{
				Console.WriteLine("Unknown module {0}", s);
				return;
			}
			IModuleClient m = this.blackboard.Modules[s];
			m.Stop();
			m.Simulation.SuccessRatio = m.Simulation.SimulationEnabled ? 2 : 1;
			m.Start();
			WriteModule(m);
			if (m.Simulation.SimulationEnabled)
			{
				ConsoleColor color = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Simulation enabled");
				Console.ForegroundColor = color;
			}
		}

		#endregion

		#region List Commands

		public void ListModules()
		{
			foreach (IModuleClient m in this.blackboard.Modules)
				WriteModule(m);

			Console.WriteLine();
		}

		private void ListPrototypes()
		{
			foreach (IModuleClient m in this.blackboard.Modules)
			{
				if (!m.Enabled) continue;
				if (m == blackboard.VirtualModule)
					continue;
				WriteModule(m);
				WriteModulePrototypes(m);
				Console.WriteLine();
			}
		}

		public void ListVariables()
		{
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			foreach (SharedVariable sv in this.blackboard.VirtualModule.SharedVariables)
			{
				Console.Write("{0}{1}", sv.Type, sv.IsArray ? "[]" : String.Empty);
				Console.CursorLeft = 15;
				Console.Write(sv.Name);
				Console.CursorLeft = 35;
				Console.WriteLine("{0} subscribers.", sv.Subscriptions.Count);
			}
			Console.ForegroundColor = color;
			Console.WriteLine();
		}

		#endregion

		#region Write

		private void WriteCommandOptions(string command, params string[] options)
		{
			ConsoleColor color = Console.ForegroundColor;
			foreach (string opt in options)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write("\tlist ");
				Console.ForegroundColor = color;
				Console.WriteLine(opt);
			}
		}

		private void WriteModule(IModuleClient m)
		{
			string status;
			ConsoleColor color = Console.ForegroundColor;
			if (!m.Enabled)
			{
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine("{0} Disabled", m.Name.PadRight(20, ' '));
				Console.ForegroundColor = color;
				return;
			}
			else if (m.Simulation.SimulationEnabled)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("{0} OK (Simulated)", m.Name.PadRight(20, ' '));
				Console.ForegroundColor = color;
				return;
			}
			else if (!m.IsConnected)
			{
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine("{0} Disconnected", m.Name.PadRight(20, ' '));
				Console.ForegroundColor = color;
				return;
			}
			else if (!m.IsAlive || !m.Ready)
			{
				status = String.Empty;
				if (!m.Ready) status = "Not ready";
				if (!m.IsAlive) status = (String.IsNullOrEmpty(status) ? "Not responding" : status + ", not responding");
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("{0} {1}", m.Name.PadRight(20, ' '), status);
				Console.ForegroundColor = color;
				return;
			}

			status = "OK";
			if (m.Busy) status = "Busy";
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("{0} {1}", m.Name.PadRight(20, ' '), status);
			Console.ForegroundColor = color;
		}

		private void WriteModulePrototypes(IModuleClient m)
		{
			foreach (IPrototype p in m.Prototypes)
			{
				if (systemPrototypes.ContainsKey(p.Command))
					continue;
				Console.Write("\t");
				WritePrototype(p);
			}
		}

		private void WritePrototype(IPrototype p)
		{
			Console.Write(p.Command.PadRight(20, ' '));
			Console.Write("{0} ", p.HasPriority ? "H" : " ");
			Console.Write("{0} ", p.ResponseRequired ? "R" : " ");
			Console.Write("{0} ", p.ParamsRequired ? "P" : " ");
			Console.WriteLine("\ttimeout: {0}ms", p.Timeout.ToString().PadLeft(6, ' '));
		}

		#endregion
	}
}
