using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using Blk.Engine;
using Blk.Engine.Remote;
using Robotics;
using Robotics.Controls;
using Robotics.Utilities;

namespace BlackboardSatellite
{
	public partial class FrmSatellite : Form
	{
		#region Variables

		private LogWriter log;
		private ProducerConsumer<TcpPacket> dataReceived;
		private Thread mainThread;
		private bool running;
		private SocketTcpServer server;
		private ProcessManager processManager;
		private bool closeFlag;

		#endregion

		#region Constructor

		public FrmSatellite()
		{
			InitializeComponent();
			InitializeSockets();
			InitializeThreads();
			InitializeLog();
			this.Icon = Properties.Resources.Repair_Drone;
			this.notifyIcon.Visible = true;
			this.Visible = false;
			this.ShowInTaskbar = false;
			this.Hide();
			this.processManager = new ProcessManager(log);
			this.closeFlag = false;
		}

		#endregion

		#region Properties


		#endregion

		#region Methods

		private void InitializeLog()
		{
			TextBoxStreamWriter tsw;

			tsw = new TextBoxStreamWriter(txtLog, Application.ExecutablePath + ".log", 512);
			this.log = new LogWriter(tsw);
		}

		private void InitializeSockets()
		{
			this.server = new SocketTcpServer(2300);
			this.server.DataReceived += new TcpDataReceivedEventHandler(server_DataReceived);
			this.server.ClientConnected += new TcpClientConnectedEventHandler(server_ClientConnected);
			this.server.ClientDisconnected += new TcpClientDisconnectedEventHandler(server_ClientDisconnected);
			this.server.Start();
		}

		private void InitializeThreads()
		{
			this.mainThread = new Thread(new ThreadStart(this.MainThreadTask));
			this.mainThread.IsBackground = true;
			this.mainThread.Start();
		}

		private void MainThreadTask()
		{
			this.dataReceived = new ProducerConsumer<TcpPacket>();
			TcpPacket packet;

			running = true;
			while (running)
			{
				packet = this.dataReceived.Consume(100);
				if (packet == null)
					continue;
				foreach (string s in packet.DataStrings)
					Parse(s, packet.RemoteEndPoint);
			}
		}

		private void Parse(string s, IPEndPoint remoteEndPoint)
		{
			RemoteStartupRequest request;
			RemoteStartupResponse response;
			string serialized;

			try
			{
				request = RemoteStartupRequest.FromXml(s);
			}
			catch { request = null; }
			if (request == null)
				//response = new RemoteStartupResponse(request, false, "Invalid request");
				return;
			else
				response = Execute(request);
			serialized = RemoteStartupResponse.ToXml(response);
			server.SendTo(remoteEndPoint, serialized);
		}

		private RemoteStartupResponse Execute(RemoteStartupRequest request)
		{
			bool success;

			switch (request.Method)
			{
				case ModuleStartupMethod.LaunchAlways:
					success = this.processManager.LaunchProcess(request.ProcessInfo);
					break;

				case ModuleStartupMethod.LaunchAndWaitReady:
				case ModuleStartupMethod.KillThenLaunch:
					success = this.processManager.CloseThenKillProcess(request.ProcessInfo) &&
						this.processManager.LaunchProcess(request.ProcessInfo);
					break;

				case ModuleStartupMethod.KillOnly:
					success = this.processManager.CloseThenKillProcess(request.ProcessInfo);
					break;

				default:
					success = true;
					break;
			}
			return new RemoteStartupResponse(request, success, null);
		}

		#endregion

		#region Event Handlers

		private void FrmSatellite_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!closeFlag)
			{
				this.WindowState = FormWindowState.Minimized;
				this.Hide();
				this.Visible = false;
				return;
			}
			running = false;
			this.Enabled = false;
			this.notifyIcon.Visible = false;
			this.Visible = false;
			
			this.server.Stop();
			this.mainThread.Join();
		}

		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Normal;
		}

		private void FrmSatellite_Resize(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized)
			{
				this.Hide();
				this.Visible = false;
				//this.ShowInTaskbar = false;
			}
			else
			{
				this.Visible = true;
				this.Activate();
				//this.ShowInTaskbar = true;
			}
		}

		private void showToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Normal;
			this.Show();
			this.Visible = true;
			this.Activate();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.closeFlag = true;
			this.Close();
		}

		#region Socket event handlers

		private void server_ClientDisconnected(System.Net.EndPoint ep)
		{
			log.WriteLine("Disconnected client: " + ((ep != null) ? ep.ToString() : String.Empty));
		}

		private void server_ClientConnected(Socket s)
		{
			log.WriteLine("Connected client: " + s.RemoteEndPoint.ToString());
		}

		private void server_DataReceived(TcpPacket p)
		{
			dataReceived.Produce(p);
			
		}

		#endregion

		#endregion
	}
}
