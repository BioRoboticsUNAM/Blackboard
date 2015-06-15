using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Robotics;
using Robotics.API.Parsers;
using Robotics.Sockets;

namespace Blk.Engine
{
	/// <summary>
	/// Asynchronously parses incoming TcpPackets extracting command and responses
	/// </summary>
	public partial class TcpPacketParser : TcpPacketParserEngine
	{
		#region Variables

		/// <summary>
		/// Stores a reference to the IService object which handles this parser
		/// </summary>
		private readonly IService owner;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of TcpPacketParser
		/// </summary>
		public TcpPacketParser(IService owner)
		{
			this.owner = owner;
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when a string is parsed from the TcpPacket
		/// </summary>
		public event Action<string> StringReceived;

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating wether the ParserEngine is running.
		/// </summary>
		public override bool IsRunning { get { return this.owner.IsRunning; } }

		#endregion

		#region Methods

		/// <summary>
		/// When overriden in a derived class, creates a new Task of the type required
		/// for this TcpPacketParserEngine
		/// </summary>
		/// <param name="ep">The endPoint to which the task will be bound</param>
		/// <returns>A parser task</returns>
		public override AsyncTask CreateTask(IPEndPoint ep)
		{
			return new Task(ep, this);
		}

		/// <summary>
		/// Raises the StringReceived event
		/// </summary>
		/// <param name="s">The received string</param>
		protected void OnStringReceived(string s)
		{
			try
			{
				if (this.StringReceived != null)
					this.StringReceived(s);
			}
			catch { }
		}

		#endregion

		public class Task : TcpPacketParserEngine.AsyncTask
		{
			/// <summary>
			/// Required for appending valid chars
			/// </summary>
			private readonly StringBuilder sb;

			/// <summary>
			/// The TcpPacketParser object owner of this task
			/// </summary>
			private readonly TcpPacketParser parser;

			/// <summary>
			/// Initializes a new instance of AsyncTask
			/// </summary>
			/// <param name="endpoint">The endpoint this Task will be bounded to</param>
			/// <param name="parser">The TcpPacketParser object owner of this task</param>
			public Task(IPEndPoint endpoint, TcpPacketParser parser)
				: base(endpoint)
			{
				this.sb = new StringBuilder(1024);
				this.parser = parser;
			}

			/// <summary>
			/// Parses data into Command and Rresponse objects
			/// </summary>
			protected override void ParseNext()
			{
				int next = ReadUTF8();
				if (next > 0)
				{
					sb.Append((char)next);
				}
				else if (sb.Length > 0)
				{
					parser.OnStringReceived(sb.ToString());
					sb.Length = 0;
				}
			}

		}
	}
}