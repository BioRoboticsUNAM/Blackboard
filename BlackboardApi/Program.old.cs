using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Blackboard
{
	class Program
	{
		#region Variables
		static string modulesFile = "Introduccion.txt";
		static string scriptFile = "Introduccion.txt";
		#endregion

		#region Main
		static void Main(string[] args)
		{
			Blackboard blackboard;
			Console.WriteLine("Loading Blackboard");
			blackboard = Blackboard.FromXML("bb.xml", Console.Out);
			Console.WriteLine("Blackboard loaded");
			Console.WriteLine("Waiting for incoming connections");

			go();
			return;

			CommandBase m;
			blackboard.ParseMessage("SP-REC MV-PLN go_to_room \"maindoor\" 1 @1", out m);
			blackboard.ParseMessage("SP-REC MV-PLN go_to_room \"maindoor\" 0", out m);
			blackboard.ParseMessage("SP-REC where is the mother 1.1 @1", out m);
			blackboard.ParseMessage("FACE move no 1 @1", out m);
			blackboard.ParseMessage("FACE show happy 1 @1", out m);

			blackboard.Start();
			Console.ReadKey();
		}

		private static void go()
		{
			string module = @"((?<module>[A-Za-z\-]+)\s+)";
			string fromTo = @"((?<from>[A-Za-z\-]+)\s+(?<to>[A-Za-z\-]+)\s+)";
			string command = @"(?<cmd>[\w]+)";
			string args = @"(\s+""(?<args>[^""]*)""\s+)";
			string opResult = @"(?<opRes>[01])\s+)";
			string id = @"(?<id>@\d+)";

			string[] test =
				{
					"VISION MV-PLN go_to_room \"maindoor\" @1",
					"MV-PLN go_to_room \"maindoor\" 0 @1",
					"SP-REC say \"where is the mother\" @2",
					"SP-REC say \"where is the mother\" 1 @2",
					"FACE move no 1 @1",
					"FACE show happy 1 @1"
				};
			Regex rx = new Regex("(" + fromTo + "|" + module + ")?" + command + args +id + "?");
			Match m;
			foreach (string s in test)
			{
				m = rx.Match(s);
				if (!m.Success)
				{
					Console.WriteLine("Rejected: " + s);
					continue;
				}
				if ((m.Result("${from}") != "") && (m.Result("${to}") != ""))
					Console.WriteLine("Redirect: " + s);
				else if (m.Result("${module}") != "")
					Console.WriteLine("Module Explicit: " + s);
				else
					Console.WriteLine("Single Command: " + s);
			}
			Console.ReadLine();
		}
		#endregion

		#region Static Methods
		static bool setupBlackboard(Blackboard b)
		{
			Console.WriteLine("Loading Modules...");
			if (b.LoadModules(modulesFile))
				Console.WriteLine("Modules loaded: OK");
			else
			{
				Console.WriteLine("Error loading modules");
				return false;
			}
			Console.WriteLine("Loading Script...");
			if (b.LoadScript(scriptFile))
				Console.WriteLine("Script loaded: OK");
			else
			{
				ConsoleKey k;
				do
				{
					Console.WriteLine("Error loading script. Continue? y/N");
					k = Console.ReadKey(true).Key;
					if (k == ConsoleKey.N)
						return false;
					if (k == ConsoleKey.Y)
						break;
				} while ((k != ConsoleKey.Y) && (k != ConsoleKey.N));

			}

			b.MessageReceived += new MessageIOEH(b_MessageReceived);
			//b.MessageRedirect += new MessageRedirectEH(b_MessageRedirect);
			b.MessageSent += new MessageIOEH(b_MessageSent);
			b.ModuleConnectedToBlackBoard += new BlackboardClientCnnEH(b_ModuleConnectedToBlackBoard);
			b.ModuleDisconnectedFromBlackBoard += new BlackboardClientCnnEH(b_ModuleDisconnectedFromBlackBoard);
			b.BlackboardConnectedToModule += new ModuleConnectionEH(b_BlackboardConnectedToModule);
			b.BlackboardDisconnectedFromModule += new ModuleConnectionEH(b_BlackboardDisconnectedFromModule);
			return true;
		}

		#endregion


		#region Event Handlers
		static void b_BlackboardDisconnectedFromModule(ModuleClient m)
		{
			string s;
			if (m == null) s = "Unknown";
			else s = m.ToString();
			Console.WriteLine("Blackboard disconnected from " + s);
		}

		static void b_BlackboardConnectedToModule(ModuleClient m)
		{
			string s;
			if (m == null) s = "Unknown";
			else s = m.ToString();
			Console.WriteLine("Blackboard connected to " + s);
		}

		static void b_ModuleDisconnectedFromBlackBoard(System.Net.IPAddress ip)
		{
			string s;
			if (ip == null) s = "Unknown";
			else s = ip.ToString();
			Console.WriteLine(s + " disconnected from Blackboard");
		}

		static void b_ModuleConnectedToBlackBoard(System.Net.IPAddress ip)
		{
			string s;
			if (ip == null) s = "Unknown";
			else s = ip.ToString();
			Console.WriteLine(s + " connected to Blackboard");
		}

		static void b_MessageSent(CommandBase message)
		{
			string s;
			if (message.Source == null) s = "Unknown";
			else s = message.Source.ToString();
			Console.WriteLine("==> Sending to " + s + ": " + message.OriginalString);
		}

		static void b_MessageRedirect(ModuleClient origin, ModuleClient destination, string message)
		{
			string s1, s2;
			if (origin == null) s1 = "Unknown";
			else s1 = origin.ToString();
			if (destination == null) s2 = "Unknown";
			else s2 = destination.ToString();
			Console.WriteLine("=>> Redirecting from " + s1 + " to " + s2 + ": " + message);
		}

		static void b_MessageReceived(CommandBase message)
		{
			string s;
			if (message.Source == null) s = "Unknown";
			else s = message.Source.ToString();
			Console.WriteLine("<== Received from " + s + ": " + message.OriginalString);
		}
		#endregion
	}
}
