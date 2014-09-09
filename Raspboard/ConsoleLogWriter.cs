using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;
using Robotics;

namespace Raspboard
{
	/// <summary>
	/// Implements a TextWriter that dumps its contents to both a System.Windows.Forms.TexBox and a xml file
	/// </summary>
	public class ConsoleLogWriter : TextWriter, IDisposable
	{
		#region Variables

		private StreamWriter logFile;

		private ProducerConsumer<StringToken> pending;

		private Thread writerThread;

		private bool running;

		/// <summary>
		/// Stores a value that indicates if the time and date must be appended to each log entry
		/// </summary>
		protected bool appendDate;

		private StringBuilder waitingHadle;

		/// <summary>
		/// The default priority for messages
		/// </summary>
		private int defaultPriority;

		/// <summary>
		/// The verbosity treshold for the console. Only messages with a priority equal or higher than the treshold will be shown
		/// </summary>
		private int consoleVerbosityThreshold;

		/// <summary>
		/// The verbosity treshold for the file. Only messages with a priority equal or higher than the treshold will be written
		/// </summary>
		private int fileVerbosityThreshold;

		/// <summary>
		/// Gets a value indicating whether the component is being disposed
		/// </summary>
		private bool disposing;

		/// <summary>
		/// Gets a value indicating whether the component is disposed
		/// </summary>
		private bool disposed;

		/// <summary>
		/// Stores an Id for the messages of the log session
		/// </summary>
		private int recordId;
		private bool logRecordTagOpen;
		private bool logTagOpen;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the TextBoxStreamWriter class
		/// </summary>
		/// <param name="logFile">The path of the file to which dump the contents to</param>
		public ConsoleLogWriter(string logFile)
		{
			this.pending = new ProducerConsumer<StringToken>(100);
			this.waitingHadle = new StringBuilder(1024);
			this.OpenLogFileStream(logFile, out this.logFile);

			this.appendDate = false;
			this.consoleVerbosityThreshold = 5;
			this.defaultPriority = 5;
			this.disposed = false;
			this.disposing = false;
			this.recordId = 0;
			this.logRecordTagOpen = false;
			this.logTagOpen = false;
			SetupThread();
		}

		#endregion

		#region Destructor

		/// <summary>
		/// Releases all resources used by the TextBoxStreamWriter object
		/// </summary>
		~ConsoleLogWriter()
		{
			this.running = false;
			Dispose(false);
			if ((writerThread != null) && writerThread.IsAlive)
			{
				writerThread.Join(100);
				if (writerThread.IsAlive)
					writerThread.Abort();
			}
			this.logFile = null;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating if the date and time of write operation must be appended to the writed data
		/// </summary>
		public bool AppendDate
		{
			get { return this.appendDate; }
			set { this.appendDate = value; }
		}

		/// <summary>
		/// Gets or sets the default priority for messages
		/// </summary>
		public int DefaultPriority
		{
			get { return this.defaultPriority; }
			set
			{
				if ((value < 0) || (value > 9))
					throw new ArgumentOutOfRangeException("Value must be between 0 and 9");
				this.defaultPriority = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the component is being disposed
		/// </summary>
		public bool Disposing
		{
			get { return disposing; }
			protected set
			{
				if (disposing && !value)
					disposed = true;
				disposing = value;
			}
		}

		/// <summary>
		/// When overridden in a derived class, returns the Encoding in which the output is written
		/// </summary>
		public override System.Text.Encoding Encoding
		{
			get { return System.Text.Encoding.UTF8; }
		}

		/// <summary>
		/// Gets a value indicating whether the component is disposed
		/// </summary>
		public bool IsDisposed
		{
			get { return disposed; }
			protected set
			{
				if (disposed) return;
				disposed = value;
			}
		}

		/// <summary>
		/// Gets the stream used to write to the file where the log is dumped
		/// </summary>
		protected StreamWriter LogFile { get { return this.logFile; } }

		/// <summary>
		/// Gets or sets the verbosity treshold for the console. Only messages with a priority equal or higher than the treshold will be shown
		/// </summary>
		public int ConsoleVerbosityThreshold
		{
			get { return this.consoleVerbosityThreshold; }
			set
			{
				if ((value < 0) || (value > 9))
					throw new ArgumentOutOfRangeException("Value must be between 0 and 9");
				this.consoleVerbosityThreshold = value;
			}
		}

		/// <summary>
		/// Gets or sets the verbosity treshold for the file. Only messages with a priority equal or higher than the treshold will be written
		/// </summary>
		public int FileVerbosityThreshold
		{
			get { return this.fileVerbosityThreshold; }
			set
			{
				if ((value < 0) || (value > 9))
					throw new ArgumentOutOfRangeException("Value must be between 0 and 9");
				this.fileVerbosityThreshold = value;
			}
		}

		#endregion

		#region Methods

		#region Overloads

		#region Prioritized overloads

		#region Write overload

		/// <summary>
		/// Writes the text representation of a Boolean value to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The Boolean to write</param>
		public virtual void Write(int priority, bool value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes a character to the stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The character to write to the text stream</param>
		public virtual void Write(int priority, char value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="buffer">The character array to write to the text stream</param>
		public virtual void Write(int priority, char[] buffer)
		{
			pending.Produce(new StringToken(priority, new String(buffer)));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="buffer">The character array to write to the text stream</param>
		/// <param name="index">The start index at which begin the write</param>
		/// <param name="count">The number of characters to write</param>
		public virtual void Write(int priority, char[] buffer, int index, int count)
		{
			pending.Produce(new StringToken(priority, new String(buffer, index, count)));
		}

		/// <summary>
		/// Writes the text representation of a decimal value to the text stream. 
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The decimal value to write</param>
		public virtual void Write(int priority, decimal value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte floating-point value to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The double value to write</param>
		public virtual void Write(int priority, double value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte floating-point value to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The float value to write</param>
		public virtual void Write(int priority, float value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte signed integer to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The int value to write</param>
		public virtual void Write(int priority, int value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte signed integer to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The long value to write</param>
		public virtual void Write(int priority, long value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an object to the text stream by calling ToString on that object
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The object to write.</param>
		public virtual void Write(int priority, object value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes a string to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The string value to write</param>
		public virtual void Write(int priority, string value)
		{
			pending.Produce(new StringToken(priority, value));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte unsigned integer to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The uint value to write</param>
		public virtual void Write(int priority, uint value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte unsigned integer to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The ulong value to write</param>
		public virtual void Write(int priority, ulong value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		#endregion

		#region WriteLine Overload

		/// <summary>
		/// Writes the text representation of a Boolean value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The Boolean to write</param>
		public virtual void WriteLine(int priority, bool value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes a character to the stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The character to write followed by a line terminator to the text stream.</param>
		public virtual void WriteLine(int priority, char value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="buffer">The character array to write followed by a line terminator to the text stream.</param>
		public virtual void WriteLine(int priority, char[] buffer)
		{
			pending.Produce(new StringToken(priority, new String(buffer) + base.NewLine));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="buffer">The character array to write followed by a line terminator to the text stream.</param>
		/// <param name="index">The start index at which begin the write</param>
		/// <param name="count">The number of characters to write</param>
		public virtual void WriteLine(int priority, char[] buffer, int index, int count)
		{
			pending.Produce(new StringToken(priority, new String(buffer, index, count) + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a decimal value followed by a line terminator to the text stream.. 
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The decimal value to write</param>
		public virtual void WriteLine(int priority, decimal value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte floating-point value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The double value to write</param>
		public virtual void WriteLine(int priority, double value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte floating-point value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The float value to write</param>
		public virtual void WriteLine(int priority, float value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte signed integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The int value to write</param>
		public virtual void WriteLine(int priority, int value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte signed integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The long value to write</param>
		public virtual void WriteLine(int priority, long value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an object followed by a line terminator to the text stream. by calling ToString on that object
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The object to write.</param>
		public virtual void WriteLine(int priority, object value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes a string followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The string value to write</param>
		public virtual void WriteLine(int priority, string value)
		{
			pending.Produce(new StringToken(priority, value + base.NewLine));
		}


		/// <summary>
		/// Writes the text representation of a 4-byte unsigned integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The uint value to write</param>
		public void WriteLine(int priority, uint value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte unsigned integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The ulong value to write</param>
		public void WriteLine(int priority, ulong value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		#endregion

		#endregion

		#region Unprioritized Overloads

		#region Write overload

		/// <summary>
		/// Writes the text representation of a Boolean value to the text stream
		/// </summary>
		/// <param name="value">The Boolean to write</param>
		public override void Write(bool value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes a character to the stream
		/// </summary>
		/// <param name="value">The character to write to the text stream</param>
		public override void Write(char value)
		{
			//base.Write(value));
			//if (logFile != null) logFile.Write(value));
			//output.Invoke(AppendStringEH, new string[] { value.ToString() }));
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="buffer">The character array to write to the text stream</param>
		public override void Write(char[] buffer)
		{
			pending.Produce(new StringToken(defaultPriority, new String(buffer)));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="buffer">The character array to write to the text stream</param>
		/// <param name="index">The start index at which begin the write</param>
		/// <param name="count">The number of characters to write</param>
		public override void Write(char[] buffer, int index, int count)
		{
			pending.Produce(new StringToken(defaultPriority, new String(buffer, index, count)));
		}

		/// <summary>
		/// Writes the text representation of a decimal value to the text stream. 
		/// </summary>
		/// <param name="value">The decimal value to write</param>
		public override void Write(decimal value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte floating-point value to the text stream
		/// </summary>
		/// <param name="value">The double value to write</param>
		public override void Write(double value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte floating-point value to the text stream
		/// </summary>
		/// <param name="value">The float value to write</param>
		public override void Write(float value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte signed integer to the text stream
		/// </summary>
		/// <param name="value">The int value to write</param>
		public override void Write(int value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte signed integer to the text stream
		/// </summary>
		/// <param name="value">The long value to write</param>
		public override void Write(long value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an object to the text stream by calling ToString on that object
		/// </summary>
		/// <param name="value">The object to write.</param>
		public override void Write(object value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes a string to the text stream
		/// </summary>
		/// <param name="value">The string value to write</param>
		public override void Write(string value)
		{
			pending.Produce(new StringToken(defaultPriority, value));
			//base.Write(value));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte unsigned integer to the text stream
		/// </summary>
		/// <param name="value">The uint value to write</param>
		public override void Write(uint value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte unsigned integer to the text stream
		/// </summary>
		/// <param name="value">The ulong value to write</param>
		public override void Write(ulong value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		#endregion

		#region WriteLine Overload

		/// <summary>
		/// Writes a line terminator to the text stream
		/// </summary>
		public override void WriteLine()
		{
			pending.Produce(new StringToken(defaultPriority, base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a Boolean value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The Boolean to write</param>
		public override void WriteLine(bool value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes a character to the stream
		/// </summary>
		/// <param name="value">The character to write followed by a line terminator to the text stream.</param>
		public override void WriteLine(char value)
		{
			//base.Write(value));
			//if (logFile != null) logFile.Write(value));
			//output.Invoke(AppendStringEH, new string[] { value.ToString() }));
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="buffer">The character array to write followed by a line terminator to the text stream.</param>
		public override void WriteLine(char[] buffer)
		{
			pending.Produce(new StringToken(defaultPriority, new String(buffer) + base.NewLine));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="buffer">The character array to write followed by a line terminator to the text stream.</param>
		/// <param name="index">The start index at which begin the write</param>
		/// <param name="count">The number of characters to write</param>
		public override void WriteLine(char[] buffer, int index, int count)
		{
			pending.Produce(new StringToken(defaultPriority, new String(buffer, index, count) + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a decimal value followed by a line terminator to the text stream.. 
		/// </summary>
		/// <param name="value">The decimal value to write</param>
		public override void WriteLine(decimal value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte floating-point value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The double value to write</param>
		public override void WriteLine(double value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte floating-point value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The float value to write</param>
		public override void WriteLine(float value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte signed integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The int value to write</param>
		public override void WriteLine(int value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte signed integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The long value to write</param>
		public override void WriteLine(long value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an object followed by a line terminator to the text stream. by calling ToString on that object
		/// </summary>
		/// <param name="value">The object to write.</param>
		public override void WriteLine(object value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes a string followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The string value to write</param>
		public override void WriteLine(string value)
		{
			pending.Produce(new StringToken(defaultPriority, value + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte unsigned integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The uint value to write</param>
		public override void WriteLine(uint value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte unsigned integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The ulong value to write</param>
		public override void WriteLine(ulong value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		#endregion

		#endregion

		/// <summary>
		/// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device
		/// </summary>
		public override void Flush()
		{
			//doFlush = true;
			if (LogFile != null)
			{
				lock (LogFile)
				{
					if (LogFile.BaseStream.CanWrite)
						LogFile.Flush();
				}
			}

			base.Flush();
		}

		#endregion

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			this.running = false;

			if (this.disposed || this.disposing)
				return;
			this.disposing = true;

			if (this.LogFile != null)
			{
				try { this.LogFile.Flush(); }
				catch { }
				try { this.LogFile.Close(); }
				catch { }
				try { LogFile.Dispose(); }
				catch { }
			}

			base.Dispose(disposing);

			this.disposing = false;
			this.disposed = true;
		}

		/// <summary>
		/// Initializes the thread for asynchronous write in the file and TextBox logs
		/// </summary>
		protected virtual void SetupThread()
		{
			if (writerThread != null)
				return;
			writerThread = new Thread(new ThreadStart(WriterThread_Task));
			writerThread.IsBackground = true;
			writerThread.Priority = ThreadPriority.BelowNormal;
			this.running = true;
			writerThread.Start();
		}

		private void WriterThread_Task()
		{
			StringToken token;

			OnThreadStarted();
			while (this.running && !this.disposing && !this.disposed)
			{

				try
				{
					token = pending.Consume(100);
					if ((token == null) || String.IsNullOrEmpty(token.Value))
						continue;
					TokenToFile(token);
					TokenToConsole(token);
				}
				catch (ThreadAbortException)
				{
					OnThreadAborted();
					running = false;
					OnThreadStopped();
					return;
				}
				catch
				{
					Thread.Sleep(100);
					continue;
				}
			}
			OnThreadStopped();
		}

		private void CloseLogTag()
		{
			try
			{
				if (logTagOpen && (LogFile != null))
					LogFile.WriteLine("</log>");
				LogFile.Flush();
			}
			catch { }
			this.logTagOpen = false;
		}
		
		/// <summary>
		/// When overriden in a derived class, it allows to perform actions where the thread is aborted
		/// </summary>
		protected void OnThreadAborted()
		{
		}

		/// <summary>
		/// When overriden in a derived class, it allows to perform actions where the thread is started
		/// </summary>
		protected void OnThreadStarted()
		{
			if (LogFile == null)
				return;
			LogFile.WriteLine();
			LogFile.WriteLine("<log startTime=\"" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\">");
			logTagOpen = true;
		}

		/// <summary>
		/// When overriden in a derived class, it allows to perform actions where the thread finish its execution
		/// </summary>
		protected void OnThreadStopped()
		{
			CloseLogTag();
		}

		/// <summary>
		/// Opens a stream for the log file
		/// </summary>
		/// <param name="filePath">The path to the log file to create/open</param>
		/// <param name="stream">When this method returns contains a StreamWriter which allows to write in the specified file,
		/// or null if the file could not be created o could not be oppened</param>
		protected void OpenLogFileStream(string filePath, out StreamWriter stream)
		{
			try
			{
				if (!File.Exists(filePath))
				{
					stream = File.CreateText(filePath);
					LogFile.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
				}
				else stream = new StreamWriter(filePath, true, Encoding.UTF8);					
			}
			catch
			{
				stream = null;
			}
		}

		/// <summary>
		/// Returns a string to be appended to the TextBox log
		/// </summary>
		/// <param name="token">The token object used to generate the string to append</param>
		/// <returns>A string to be appended to the TextBox log</returns>
		protected void TokenToConsole(StringToken token)
		{
			if (token.Priority > ConsoleVerbosityThreshold)
				return;
			Console.Write("{0}{1}", appendDate ? token.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff ") : String.Empty, token.Value);
		}

		/// <summary>
		/// Returns a string to be appended to the log file
		/// </summary>
		/// <param name="token">The token object used to generate the string to append</param>
		/// <returns>A string to be appended to the log file</returns>
		protected void TokenToFile(StringToken token)
		{
			if ((LogFile == null) || !LogFile.BaseStream.CanWrite || (token.Priority > FileVerbosityThreshold))
				return;

			string s = String.Empty;
			string value;

			if (!logRecordTagOpen)
			{
				s = "\t<logRecord id=\"" + recordId.ToString() + "\" time=\"" +
					token.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff") +
					"\" priority=\"" + token.Priority.ToString() + "\"" + ">";
				++recordId;
				logRecordTagOpen = true;
			}

			value= token.Value.Replace("<", "&lt;").Replace(">", "&gt;");
			if (!string.IsNullOrEmpty(LogFile.NewLine) && token.Value.EndsWith(LogFile.NewLine))
			{
				s += value.Substring(0, value.Length - LogFile.NewLine.Length) + "</logRecord>" + LogFile.NewLine;
				logRecordTagOpen = false;
			}
			else
				s += value;
			LogFile.Write(s);
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Releases all resources used by the TextBoxStreamWriter object
		/// </summary>
		void IDisposable.Dispose()
		{
			OnThreadStopped();
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		/*

		#region Static Variables
		/// <summary>
		/// TextBoxStreamWriter with output to a file with the same name as the executable
		/// </summary>
		private static ConsoleLogWriter defaultLog;

		#endregion

		#region Static Contructor

		static ConsoleLogWriter()
		{
			ConsoleLogWriter.defaultLog = new ConsoleLogWriter("log.xml");
		}

		#endregion

		#region Static Properties

		/// <summary>
		/// Gets a TextBoxStreamWriter with output to a file with the same name as the executable
		/// </summary>
		public static ConsoleLogWriter DefaultLog
		{
			get { return ConsoleLogWriter.defaultLog; }
			set { if (value != null)defaultLog = value; }
		}

		#endregion

		*/

		/// <summary>
		/// Encapsulates log messages with its priority and the creation time
		/// </summary>
		protected struct StringToken
		{
			/// <summary>
			/// The message string to write to the log
			/// </summary>
			private string value;
			/// <summary>
			/// The creation time of the message
			/// </summary>
			private DateTime ct;
			/// <summary>
			/// The priority of the message
			/// </summary>
			private int priority;

			/// <summary>
			/// Initializes a new instance of StringToken
			/// </summary>
			/// <param name="priority">The priority of the message</param>
			/// <param name="value">The message string to write to the log</param>
			public StringToken(int priority, string value)
			{
				this.value = value;
				this.ct = DateTime.Now;
				this.priority = priority;
			}

			/// <summary>
			/// Gets the message string to write to the log
			/// </summary>
			public string Value { get { return value; } }

			/// <summary>
			/// Gets the creation time of the message
			/// </summary>
			public DateTime CreationTime { get { return ct; } }

			/// <summary>
			/// Gets the priority of the message
			/// </summary>
			public int Priority { get { return this.priority; } }

			/// <summary>
			/// Implicitely converts a StringToken object to a string
			/// </summary>
			/// <param name="st">The StringToken object to convert</param>
			/// <returns>The message string to write to the log stored in the StringToken object</returns>
			public static implicit operator string(StringToken st)
			{
				return st.value;
			}

			/// <summary>
			/// Implicitely converts a string to a StringToken object with a priority of 5
			/// </summary>
			/// <param name="s">The message string to write to the log</param>
			/// <returns>A StringToken object with a priority of 5 with the input string as value</returns>
			public static implicit operator StringToken(string s)
			{
				return new StringToken(5, s);
			}

			public override string ToString()
			{
				return CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff ") + value;
			}
		}
	}
}
