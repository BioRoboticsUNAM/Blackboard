using System;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PerformanceTester
{
	class Program
	{
		private delegate void VoidMethod();

		private static Regex rxCommandFromSource =
			new Regex(@"((?<dest>[\w\-]+)\s+)?(?<cmd>[A-Za-z_]+)(\s+""(?<params>([^\\""]|\\|\"")*)"")?(\s+@(?<id>\d+))?", RegexOptions.Compiled);
		private string command = @"ra_pos ""3.1416 3.1416 3.1416 3.1416 \"" \"" 3.1416 3.1416 3.1416 3.1416"" @1";


		static void Main(string[] args)
		{
			Console.SetBufferSize(Console.BufferWidth, 5 * Console.BufferHeight);
			FileStream fs = File.Open("C:\\Console.txt", System.IO.FileMode.Create);
			StreamWriter writer = new StreamWriter(fs);
			Console.SetOut(writer);
			Program p = new Program();
			for (int i = 0; i < 100; ++i)
			{
				Console.WriteLine("Executing benchmark " + (i + 1));
				p.RunBenchmark(1000,
					new VoidMethod(p.Method1),
					new VoidMethod(p.Method2),
					new VoidMethod(p.Method3));
				Console.WriteLine();
			}
			Console.ReadLine();
			writer.Flush();
			fs.Close();
		}

		private void RunBenchmark(int numberOfExecutions, params VoidMethod[] methods)
		{
			Stopwatch sw = new Stopwatch();
			int i, n;

			for (i = 0; i < methods.Length; ++i)
			{
				Console.WriteLine("Executing benchmark for Method " + (i + 1));
				sw.Start();
				for (n = 0; n < numberOfExecutions; ++n)
					methods[i]();
				sw.Stop();
				Console.WriteLine("Method " + (i+1) + ". Elapsed average time: " + (sw.ElapsedMilliseconds / (double)numberOfExecutions));
				sw.Reset();
			}
		}

		public void Method1()
		{
			Regex rxCommandFromSource =
			new Regex(@"((?<dest>[\w\-]+)\s+)?(?<cmd>[A-Za-z_]+)(\s+""(?<params>([^\\""]|\\|\"")*)"")?(\s+@(?<id>\d+))?", RegexOptions.Compiled);
			Match m = rxCommandFromSource.Match(command);
			if (!m.Success)
				return;
			string dest = m.Result("${dest}");
			string cmd = m.Result("${cmd}");
			string parameters = m.Result("${params}");
			int id;
			Int32.TryParse(m.Result("${id}"), out id);
		}

		public void Method2()
		{
			Match m = rxCommandFromSource.Match(command);
			if (!m.Success)
				return;
			string dest = m.Result("${dest}");
			string cmd = m.Result("${cmd}");
			string parameters = m.Result("${params}");
			int id;
			Int32.TryParse(m.Result("${id}"), out id);
		}

		public void Method3()
		{
			string dest;
			string cmd;
			string parameters;
			int result;
			int id;
			Blk.Engine.CommandBase.XtractCommandElements(command, out dest, out cmd, out parameters, out result, out id);
		}
	}
}
