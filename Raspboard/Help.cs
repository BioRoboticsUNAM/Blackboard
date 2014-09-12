using System;
using System.Collections.Generic;
using System.Text;

namespace Raspboard
{
	public class Help
	{
		public static void ShowHelp(string command, IEnumerable<string> commands)
		{
			switch (command)
			{
				case "cat": Cat(); break;
				case "exit": Quit(); break;
				case "help": HelpOnHelp(); break;
				case "info": Info(); break;
				case "list": List(); break;
				case "put": Put(); break;
				case "proc": Proc(); break;
				case "quit": Quit(); break;
				case "read": Read(); break;
				case "sim": Sim(); break;
				case "trace": Trace(); break;

				default:
					MainHelp(commands);
					break;
			}
			Console.WriteLine();
		}

		private static void Cat()
		{
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("cat command is an alias for read command");
			Console.ForegroundColor = color;
			Console.WriteLine();
			Read();
		}

		private static void HelpOnHelp()
		{
			WriteCommandHelp("help", "Displays the list of available commands");
			WriteCommandHelp("help <command>", "Displays help for the specifiec command");
		}

		private static void Info()
		{
			WriteCommandHelp("info <module> ", "Displays detailed information for the specified module ");
		}

		private static void List()
		{
			WriteCommandHelp("list modules", "Displays the list of all modules and it's status");
			WriteCommandHelp("list commands", "Displays the list of all commands registered in the system, sorted by module ");
			WriteCommandHelp("list <module> commands", "Displays the list of all commands registered for the specified module ");
			WriteCommandHelp("list vars", "Displays the list of all the shared variables");
		}

		private static void Put()
		{
			Console.WriteLine("Injects a command or response (message)");
			WriteCommandHelp("put <message>", "Injects message into the system via the virtual module");
			WriteCommandHelp("put <module> <message>", "Injects message into the system via the specified module");
		}

		private static void Proc()
		{
			WriteCommandHelp("proc check <module>", "Checks if the process corresponding to the specified module is running");
			WriteCommandHelp("proc close <module>", "Attempts to close the process corresponding to the specified module");
			WriteCommandHelp("proc kill <module>", "Attempts to close the process corresponding to the specified module");
			WriteCommandHelp("proc launch <module>", "Checks if the process corresponding to the specified module is running. If the process is not running, is started");
			WriteCommandHelp("proc restart <module>", "Checks if the process corresponding to the specified module is running. If the process is running, attempts to kill it and start it again");
			WriteCommandHelp("proc start <module>", "Attempts to start the process corresponding to the specified module");
		}

		private static void Quit()
		{
			WriteCommandHelp("quit", "Shuts down the blackboard and exits");
		}

		private static void Read()
		{
			WriteCommandHelp("read <variable>", "Displays the content of a shared variable");
		}

		private static void Sim()
		{
			WriteCommandHelp("sim <module>", "Toggles simulation for the specified module");
		}

		private static void Trace()
		{
			WriteCommandHelp("trace <variable>", "Toggles notifications for the specified shared variable. When enabled, displays the content every time the variable is written");
		}

		private static void WriteCommandHelp(string command, string text)
		{
			WriteCommandHelp(command, 25, text);
		}

		private static void WriteCommandHelp(string command, int commandSize, string text)
		{
			//string[] parts;
			//int textMaxSize = Console.BufferWidth - commandSize;
			//string padding = String.Empty.PadRight(commandSize, ' ');
			//command = command.PadRight(commandSize, ' ');
			//if (text.Length < textMaxSize)
			//    parts = new String[] { text };
			//else{
			//    parts = new String[1 + text.Length / textMaxSize];
			//    for (int i = 0; i < parts.Length; ++i)
			//        parts[i] = text.Substring(i * textMaxSize, Math.Min(textMaxSize, text.Length - i * textMaxSize));
			//}
			//Console.Write("{0}{1}", command, parts[0]);
			//for(int i = 1; i < parts.Length; ++i)
			//    Console.Write("{0}{1}", padding, parts[i]);
			//Console.WriteLine();
			string[] parts = text.Split(' ');
			string padding = String.Empty.PadRight(commandSize, ' ');
			command = command.PadRight(commandSize, ' ');
			Console.Write("{0}{1} ", command, parts[0]);
			for (int i = 1; i < parts.Length; ++i)
			{
				if ((Console.CursorLeft + parts[i].Length) >= Console.BufferWidth)
				{
					Console.WriteLine();
					Console.Write(padding);
				}
				Console.Write("{0} ", parts[i]);
			}
			Console.WriteLine();

		}

		private static void MainHelp(IEnumerable<string> commands)
		{

			List<string> cmdList = new List<string>(commands);
			cmdList.Sort();
			Console.WriteLine("Type help <command> for detailed help about a specific command.");
			Console.WriteLine("List of supported commands:");
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.White;
			foreach (string cmd in cmdList)
				Console.WriteLine("  {0}", cmd);
			Console.ForegroundColor = color;
		}
	}
}