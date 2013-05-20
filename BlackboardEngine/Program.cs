using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Blk.Engine
{
	/*
	class Program
	{
		#region Variables
		//static string modulesFile = "Introduccion.txt";
		//static string scriptFile = "Introduccion.txt";
		#endregion

		#region Main
		static void Main(string[] args)
		{
			Blackboard blackboard;
			ConsoleKeyInfo cki;
			string input;
			List<string> history = new List<string>(1000);
			int historyIndex;

			Console.WriteLine("Loading Blackboard");
			blackboard = Blackboard.FromXML("bb.xml", Console.Out);
			Console.WriteLine("Blackboard loaded");
			Console.TreatControlCAsInput = true;
			Console.WriteLine("Starting...");
			blackboard.Start();
			Console.WriteLine("Waiting for incoming connections");

			while (((cki = Console.ReadKey()).Modifiers != ConsoleModifiers.Control) && (cki.Key != ConsoleKey.C))
			{
				switch(cki.Key)
				{
					case ConsoleKey.Enter:
						history.Add(input);
						historyIndex = history.Count-1;
						break;

					case ConsoleKey.UpArrow:
						
						break;
				}
				if ( == ConsoleKey.Enter)
					input += cki.KeyChar;
				System.Threading.Thread.Sleep(100);
			}
			blackboard.Stop();
			return;
		}

		private static void go()
		{
			string module = @"((?<module>[A-Za-z\-]+)\s+)";
			string fromTo = @"((?<from>[A-Za-z\-]+)\s+(?<to>[A-Za-z\-]+)\s+)";
			string command = @"(?<cmd>[\w]+)";
			string args = @"(\s+""(?<args>[^""]*)""\s+)";
			//string opResult = @"(?<opRes>[01])\s+)";
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
			Regex rx = new Regex("(" + fromTo + "|" + module + ")?" + command + args + id + "?");
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

	}
	 * */
}
