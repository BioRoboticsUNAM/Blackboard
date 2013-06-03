#define SPEED_UP

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Robotics.Controls;
using Robotics.Utilities;
using Blk.Api;
using Blk.Engine;
using Blk.Engine.Remote;
using Blk.Engine.SharedVariables;

namespace Blk.Gui
{
	public delegate void VoidEventHandler();
	public delegate void AddRedirectionListItemEH(Command command, Response response);

	public partial class FrmBlackboard : Form
	{
		#region Variables

		/// <summary>
		/// The blackboard itself
		/// </summary>
		private Blackboard blackboard;
		/// <summary>
		/// Log manager. Writes to file and textbox
		/// </summary>
		private LogWriter log;
		/// <summary>
		/// Textbox stream writer for log
		/// </summary>
		private XmlTextBoxStreamWriter tbsw;
		/// <summary>
		/// Sorted list containing the module name and the button asociated to a module in the form
		/// </summary>
		private SortedList<string, Button> moduleButtons;
		/// <summary>
		/// Sorted list containing the module name and the button asociated to a module in the small form
		/// </summary>
		private SortedList<string, Button> moduleButtonsClone;
		/// <summary>
		/// Stores the path of the blackboard configuration file
		/// </summary>
		private string bbConfigFile = "";
		/// <summary>
		/// Stores  the Blabkboard log file
		/// </summary>
		private string bbLogFile = "";
		/// <summary>
		/// Flag that indicates if the blackboard must autostart after load
		/// </summary>
		private bool autoStart;
		/// <summary>
		/// Indicates a close has requested. It gives time to the blackboard to shut down
		/// </summary>
		private bool requestClose;
		/// <summary>
		/// Indicates if the redirection history is enabled
		/// </summary>
		private bool redirectionHistoryEnabled;
		/// <summary>
		/// Module selected using one of the buttons
		/// </summary>
		private ModuleClient selectedMC;
		/// <summary>
		/// Represents the UpdateModuleInfo method. Used for async calls
		/// </summary>
		private VoidEventHandler updateModuleInfo;
		/// <summary>
		/// Lost of dispatched commands for fill the RedirectionHistory list
		/// </summary>
		private List<Command> dispatchedCommands;
		/// <summary>
		/// Stores the column used for sort the RedirectionHistory list
		/// </summary>
		private int redirectionHistoryColumn;
		/// <summary>
		/// Stores the relation between MachineStatus and its asociated control
		/// </summary>
		private Dictionary<IPAddress, MachineStatusControl> rmsList;
		/// <summary>
		/// Form used to show data in small screen
		/// </summary>
		private FrmBlackboardSecondaryScreen frmBbss;
		/// <summary>
		/// Battery check overflow counter
		/// </summary>
		private int batCount;
		private ModuleProcessManager processManager;

		/// <summary>
		/// Stores the settings used for the GUI
		/// </summary>
		private GUISettings guiSettings;

		#region Delegates for async calls

		/// <summary>
		/// Represents the SetupBlackboardModule method
		/// </summary>
		private IModuleAddRemoveEH dlgSetupBlackboardModule;
		/// <summary>
		/// Represents the MachineStatusList_MachineStatusAdded method. Used for async calls
		/// </summary>
		private MachineStatusAddRemoveEH dlgMslMachineStatusAdded;
		/// <summary>
		/// Represents the MachineStatusList_MachineStatusRemoved method. Used for async calls
		/// </summary>
		private MachineStatusAddRemoveEH dlgMslMachineStatusRemoved;
		/// <summary>
		/// Represents the MachineStatusList_MachineStatusChanged method. Used for async calls
		/// </summary>
		private MachineStatusElementChangedEH dlgMslMachineStatusChanged;
		/// <summary>
		/// Represents the bbStatusChanged method. Used for async calls
		/// </summary>
		private VoidEventHandler dlgBbStatusChanged;
		/// <summary>
		/// Represents the module_StatusChanged method. Used for async calls
		/// </summary>
		private StatusChangedEH dlgModuleStatusChanged;

#if !SPEED_UP
		/// <summary>
		/// Represents the AddRedirectionListItem method. Used for async calls
		/// </summary>
		private AddRedirectionListItemEH dlgAddRedirectionListItem;
		/// <summary>
		/// Represents the blackboard_ResponseRedirected method. Used for async calls
		/// </summary>
		private ResponseRedirectedEH dlgBlackboardResponseRedirected;
#endif

		/// <summary>
		/// Represents the SharedVariables_SharedVariableAdded method. Used for async calls
		/// </summary>
		private SharedVariableAddedEventHandler dlgShvSharedVariableAdded;

		#endregion

		#endregion

		#region Constructors

		public FrmBlackboard()
		{
			InitializeComponent();

			frmBbss = new FrmBlackboardSecondaryScreen(this);
			this.scTop.Panel2MinSize = 257;
			SetupStartupModeControls();
			//TextBoxStreamWriter tbsw = new TextBoxStreamWriter(txtOutputLog);
			//tbsw.AppendDate = true;
			//log = new LogWriter(tbsw);
			moduleButtons = new SortedList<string, Button>();
			moduleButtonsClone = new SortedList<string, Button>();
			frmBbss.GbBlackboardFiles.Enabled = gbBlackboardFiles.Enabled = true;
			gbBlackboardSettings.Enabled = false;
			frmBbss.GbModuleList.Enabled = gbModuleList.Enabled = false;
			btnStartStop.Enabled = false;
			frmBbss.BtnStartStop.Enabled = false;
			this.Icon = Properties.Resources.star2_48;
			updateModuleInfo = new VoidEventHandler(UpdateModuleInfo);
			dispatchedCommands = new List<Command>(10000);
			rmsList = new Dictionary<IPAddress, MachineStatusControl>();
			this.redirectionHistoryColumn = lvwRedirectionHistory.Columns.IndexOf(chCommandSent);
			tcLog.SelectedTab = tpOutputLog;
			this.chkAutoLog.Checked = true;

			dlgMslMachineStatusAdded = new MachineStatusAddRemoveEH(MachineStatusList_MachineStatusAdded);
			dlgMslMachineStatusRemoved = new MachineStatusAddRemoveEH(MachineStatusList_MachineStatusRemoved);
			dlgMslMachineStatusChanged = new MachineStatusElementChangedEH(MachineStatusList_ElementChanged);
			dlgBbStatusChanged = new VoidEventHandler(bbStatusChanged);
			dlgModuleStatusChanged = new StatusChangedEH(module_StatusChanged);
			dlgShvSharedVariableAdded = new SharedVariableAddedEventHandler(SharedVariables_SharedVariableAdded);
			dlgSetupBlackboardModule = new IModuleAddRemoveEH(SetupBlackboardModule);

#if !SPEED_UP
			dlgBlackboardResponseRedirected = new ResponseRedirectedEH(blackboard_ResponseRedirected);
			dlgAddRedirectionListItem = new AddRedirectionListItemEH(AddRedirectionListItem);
#else
			tcLog.TabPages.Remove(tpMessagePendingList);
			tcLog.TabPages.Remove(tpRedirectionHistory);
#endif				
		}

		#endregion

		#region Properties

		public bool AutoStart
		{
			get { return autoStart; }
			set { autoStart = value; }
		}

		/// <summary>
		/// Gets the blackboard
		/// </summary>
		public Blackboard Blackboard
		{
			get { return this.blackboard; }
		}

		public string ConfigFile
		{
			get { return bbConfigFile; }
			set
			{
				if (String.Compare(bbConfigFile, value, true) == 0)
					return;

				if (!File.Exists(value))
				{
					txtConfigurationFile.BackColor = Color.FromArgb(255,210,210);
					return;
				}

				FileInfo fi = new FileInfo(value);
				bbConfigFile = fi.FullName;
				txtConfigurationFile.Text = fi.Name;
				txtConfigurationFile.Tag = fi.FullName;
				frmBbss.ConfigFile = txtConfigurationFile.Text;
				txtConfigurationFile.BackColor = SystemColors.Window;
				LoadSettings();
			}
		}

		public string LogFile
		{
			get { return bbLogFile; }
			set
			{
				FileInfo fi;

				if (String.Compare(value, bbLogFile, true) == 0)
					return;

				if (!chkAutoLog.Checked)
				{
					txtLogFile.Text = "(Automatically generated)";
					//return;
				}
				
				fi = null;
				try
				{
					fi = new FileInfo(value);
				}
				catch
				{
					txtLogFile.BackColor = Color.FromArgb(255, 200, 200);
					bbLogFile = null;
					return;
				}

				if (fi != null)
				{
					bbLogFile = fi.FullName;
					txtLogFile.Text = fi.FullName;
					txtLogFile.Tag = fi.FullName;
					frmBbss.LogFile = txtLogFile.Text;
					txtLogFile.BackColor = SystemColors.Window;
				}
								
			}
		}

		/// <summary>
		/// Gets or sets the number of attempts while redirecting a response
		/// </summary>
		private int SendAttempts
		{
			get { return blackboard.SendAttempts; }
			set
			{
				if ((blackboard.SendAttempts == value) || (value < 1)) return;
				blackboard.SendAttempts = value;
				try
				{
					nudRetrySendAttempts.Value = value;
				}
				catch { }

			}
		}

		/// <summary>
		/// Enables or disables the controls
		/// </summary>
		protected bool BlackboardControlsEnabled
		{
			//get { }
			set
			{
				if (!value)
					tcLog.TabIndex = 1;
				gbModuleList.Enabled = value;
				gbGeneralActions.Enabled = value;
				gbBlackboardSettings.Enabled = value;
				tpSharedVars.Enabled = value;
				tpModuleInfo.Enabled = value;
				tpRedirectionHistory.Enabled = value;
				tpMessagePendingList.Enabled = value;
				tpDiagnostics.Enabled = value;
			}
		}

		#endregion

		#region Methods

		private void AddModuleButton(ModuleClient mc)
		{
			Button b = CreateModuleButton(mc);
			flpModuleList.Controls.Add(b);
			moduleButtons.Add(mc.Name, b);
			b.Click += new EventHandler(moduleButton_Click);
		}

		private void AddModuleMenu(ModuleClient mc)
		{
			ToolStripMenuItem mnu = (ToolStripMenuItem)mnuiModules.DropDownItems.Add(mc.Name);
			mnu.DropDownOpening += new EventHandler(ModuleMenu_DropDownOpening);
			mnu.Tag = mc;

			PopulateModuleMenu(mnu);
		}

		#region Populate Module Menus

		private void PopulateModuleMenu(ToolStripMenuItem mnu)
		{
			ModuleClient mc = mnu.Tag as ModuleClient;
			if (mc == null)
				return;
			mnu.DropDownItems.Clear();
			PopulateModuleMenu_Enabled(mnu, mc);
			PopulateModuleMenu_Start(mnu, mc);
			PopulateModuleMenu_Restart(mnu, mc);
			PopulateModuleMenu_Simulation(mnu, mc);
			mnu.DropDownItems.Add(new ToolStripSeparator());
			PopulateModuleMenu_Run(mnu, mc);
			PopulateModuleMenu_CheckAndRun(mnu, mc);
			PopulateModuleMenu_CloseAndKill(mnu, mc);
			mnu.DropDownItems.Add(new ToolStripSeparator());
			PopulateModuleMenu_ZombieCheck(mnu, mc);
		}

		private void PopulateModuleMenu_CheckAndRun(ToolStripMenuItem mnu, ModuleClient mc)
		{
			ToolStripMenuItem mni = (ToolStripMenuItem)mnu.DropDownItems.Add("&Run (if not running)");
			mni.Image = Properties.Resources.ProcessOk16;
			mni.Enabled = mc.Enabled && mc.IsRunning;
			mni.Tag = mc;
			mni.Click += new EventHandler(mnuiModule_Module_CheckAndRun_Click);
		}

		private void PopulateModuleMenu_CloseAndKill(ToolStripMenuItem mnu, ModuleClient mc)
		{
			ToolStripMenuItem mni = (ToolStripMenuItem)mnu.DropDownItems.Add("Close and &Kill");
			mni.Image = Properties.Resources.Skull16;
			mni.Enabled = mc.Enabled;
			mni.Tag = mc;
			mni.Click += new EventHandler(mnuiModule_Module_CloseAndKill_Click);
		}

		private void PopulateModuleMenu_Enabled(ToolStripMenuItem mnu, ModuleClient mc)
		{
			ToolStripMenuItem mni;

			mni = (ToolStripMenuItem)mnu.DropDownItems.Add(mc.Enabled ? "Enabled (click to disable)" : "Disabled (click to enable)");
			mni.Image = mc.Enabled ? Properties.Resources.ExitGreen_16 : Properties.Resources.ExitRed_16;
			mni.Checked = mc.Enabled;
			mni.Tag = mc;
			mni.Click += new EventHandler(mnuiModule_Module_Enabled_Click);
		}

		private void PopulateModuleMenu_Restart(ToolStripMenuItem mnu, ModuleClient mc)
		{
			ToolStripMenuItem mni;

			mni = (ToolStripMenuItem)mnu.DropDownItems.Add("Restart");
			mni.Image = Properties.Resources.refresh16;
			mni.Enabled = mc.IsRunning && selectedMC.Enabled;
			mni.Tag = mc;
			mni.Click += new EventHandler(mnuiModule_Module_Restart_Click);
		}

		private void PopulateModuleMenu_Run(ToolStripMenuItem mnu, ModuleClient mc)
		{
			ToolStripMenuItem mni = (ToolStripMenuItem)mnu.DropDownItems.Add("Run (&unconditionally)");
			mni.Image = Properties.Resources.ProcessWarning16;
			mni.Enabled = mc.Enabled;
			mni.Tag = mc;
			mni.Click += new EventHandler(mnuiModule_Module_Run_Click);
		}

		private void PopulateModuleMenu_Simulation(ToolStripMenuItem mnu, ModuleClient mc)
		{
			ToolStripMenuItem mni = (ToolStripMenuItem)mnu.DropDownItems.Add(mc.Simulation.SimulationEnabled ?
						 "&Simulation enabled" : "&Simulation disabled");
			mni.Image = mc.Simulation.SimulationEnabled ?
				Properties.Resources.CircleGreen_16 :
				Properties.Resources.CircleGray_16;
			mni.Enabled = mc.Enabled;
			mni.Tag = mc;
			mni.Click += new EventHandler(mnuiModule_Module_Simulation_Click);
		}

		private void PopulateModuleMenu_Start(ToolStripMenuItem mnu, ModuleClient mc)
		{
			ToolStripMenuItem mni;

			mni = (ToolStripMenuItem)mnu.DropDownItems.Add(mc.IsRunning ? "Running (click to stop)" : "Stopped (click to start)");
			mni.Image = mc.IsRunning ? Properties.Resources.Stop_16 : Properties.Resources.run16;
			mni.Enabled = mc.Enabled && blackboard.IsRunning;
			mni.Tag = mc;
			mni.Click += new EventHandler(mnuiModule_Module_Start_Click);
		}

		private void PopulateModuleMenu_ZombieCheck(ToolStripMenuItem mnu, ModuleClient mc)
		{
			ToolStripMenuItem mni = (ToolStripMenuItem)mnu.DropDownItems.Add("&Check for zombie processes");
			mni.Image = Properties.Resources.ProcessInfo16;
			mni.Enabled = mc.Enabled && mc.IsRunning;
			mni.Tag = mc;
			mni.Click += new EventHandler(mnuiModule_Module_ZombieCheck_Click);
		}

		#endregion

		private void AddSecondaryModuleButton(ModuleClient mc)
		{
			Button b2 = CreateModuleButton(mc);
			frmBbss.FlpModuleList.Controls.Add(b2);
			moduleButtonsClone.Add(mc.Name, b2);
		}

		private void BlackboardRestarting()
		{
			frmBbss.BtnStartStop.Text = btnStartStop.Text = "Restarting";
			frmBbss.BtnRestartBlackboard.Enabled =
				btnRestartBlackboard.Enabled =
				mnuiBlackboard_Restart.Enabled =
				false;
			frmBbss.BtnRestartTest.Enabled = btnRestartTest.Enabled = false;
			frmBbss.BtnLoad.Enabled =
				btnLoad.Enabled =
				mnuiBlackboard_Load.Enabled =
				mnuiBlackboard_Reload.Enabled =
				true;
		}

		private void BlackboardRestartingTest()
		{
			//frmBbss.BtnRestartBlackboard.Enabled = btnRestartBlackboard.Enabled = false;
			frmBbss.BtnRestartTest.Enabled =
				btnRestartTest.Enabled = 
				mnuiBlackboard_Restart_Test.Enabled =
				false;
			frmBbss.BtnLoad.Enabled =
				btnLoad.Enabled =
				mnuiBlackboard_Load.Enabled =
				mnuiBlackboard_Reload.Enabled =
				true;
		}

		private void BlackboardRunning()
		{
			injectorTool.Blackboard = blackboard;
			frmBbss.BtnStartStop.Text = 
				btnStartStop.Text =
				mnuiBlackboard_StartStop.Text =
				"Stop Blackboard";
			btnStartStop.Image = Properties.Resources.stop16;
			frmBbss.BtnStartStop.Image = Properties.Resources.stop48;
			frmBbss.BtnLoad.Enabled =
				btnLoad.Enabled =
				mnuiBlackboard_Load.Enabled =
				mnuiBlackboard_Reload.Enabled =
				false;
			frmBbss.BtnStartStop.Enabled =
				btnStartStop.Enabled =
				mnuiBlackboard_StartStop.Enabled =
				true;
			frmBbss.BtnRestartBlackboard.Enabled =
				btnRestartBlackboard.Enabled =
				mnuiBlackboard_Restart.Enabled =
				true;
			frmBbss.BtnRestartTest.Enabled = btnRestartTest.Enabled = true;
			frmBbss.GbBlackboardFiles.Enabled = gbBlackboardFiles.Enabled = false;
			timer.Start();
			UpdateSharedVariables();
		}

		private void BlackboardStarting()
		{
			frmBbss.BtnStartStop.Text =
				btnStartStop.Text =
				mnuiBlackboard_StartStop.Text =
				"Starting";
			//frmBbss.BtnRestartBlackboard.Enabled = btnRestartBlackboard.Enabled = false;
			frmBbss.BtnRestartTest.Enabled =
				btnRestartTest.Enabled =
				mnuiBlackboard_Restart_Test.Enabled =
				false;
			frmBbss.BtnLoad.Enabled =
				btnLoad.Enabled =
				mnuiBlackboard_Load.Enabled =
				mnuiBlackboard_Reload.Enabled =				
				true;
		}

		private void BlackboardStopping()
		{
			injectorTool.Blackboard = null;
			frmBbss.BtnStartStop.Text =
				btnStartStop.Text =
				mnuiBlackboard_StartStop.Text =
				"Stopping";
			//frmBbss.BtnRestartBlackboard.Enabled = btnRestartBlackboard.Enabled = false;
			frmBbss.BtnRestartTest.Enabled =
				btnRestartTest.Enabled =
				mnuiBlackboard_Restart_Test.Enabled =
				false;
			frmBbss.BtnLoad.Enabled =
				btnLoad.Enabled =
				mnuiBlackboard_Load.Enabled =
				mnuiBlackboard_Reload.Enabled = 
				true;
		}

		private void BlackboardStopped()
		{
			tcLog.SelectTab(0);
			frmBbss.BtnStartStop.Text = 
				btnStartStop.Text =
				mnuiBlackboard_StartStop.Text = 
				"Start Blackboard";
			btnStartStop.Image = Properties.Resources.run16;
			frmBbss.BtnStartStop.Image = Properties.Resources.run48;
			frmBbss.GbBlackboardFiles.Enabled = gbBlackboardFiles.Enabled = true;
			frmBbss.BtnStartStop.Enabled = 
				btnStartStop.Enabled =
				mnuiBlackboard_StartStop.Enabled = 
				true;
			frmBbss.BtnRestartBlackboard.Enabled =
				btnRestartBlackboard.Enabled =
				mnuiBlackboard_Restart.Enabled =
				false;
			frmBbss.BtnRestartTest.Enabled = 
				btnRestartTest.Enabled = 
				false;
			frmBbss.BtnLoad.Enabled = 
				btnLoad.Enabled =
				mnuiBlackboard_Load.Enabled =
				mnuiBlackboard_Reload.Enabled = 
				true;
			timer.Stop();
			SaveSettings();
			if (requestClose) Application.Exit();
		}

		internal void BlackboardStartStopToggle()
		{
			if (blackboard == null)
				return;
			if (blackboard.IsRunning)
				Stop();
			else
				Start();
			UpdateModuleInfo();
		}

		/// <summary>
		/// Checks the current machine status and injects a mystat command
		/// </summary>
		private void CheckMachineStatus()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("ip127.0.0.1 bcs");
			sb.Append((100 * SystemInformation.PowerStatus.BatteryLifePercent).ToString("0"));
			if ((SystemInformation.PowerStatus.BatteryChargeStatus & System.Windows.Forms.BatteryChargeStatus.Charging) == System.Windows.Forms.BatteryChargeStatus.Charging)
				sb.Append('c');
			else if (((SystemInformation.PowerStatus.BatteryChargeStatus & System.Windows.Forms.BatteryChargeStatus.NoSystemBattery) == System.Windows.Forms.BatteryChargeStatus.NoSystemBattery) ||
				SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online)
				sb.Append('n');
			else if ((SystemInformation.PowerStatus.BatteryChargeStatus & System.Windows.Forms.BatteryChargeStatus.Unknown) == System.Windows.Forms.BatteryChargeStatus.Unknown)
				sb.Append('u');
			else
				sb.Append('b');

			blackboard.VirtualModule.Send(new Command(blackboard.VirtualModule, blackboard.VirtualModule, "mystat", sb.ToString()));
		}

		private Button CreateModuleButton(ModuleClient mc)
		{
			Button b = new Button();
			b.Name = "btn" + mc.Name;
			b.Text = mc.Name;
			b.Width = flpModuleList.Width - 25;
			b.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			b.ImageAlign = ContentAlignment.MiddleLeft;
			b.TextImageRelation = TextImageRelation.ImageBeforeText;
			b.Image = mc.Enabled ? Properties.Resources.ExitGreen_16 : Properties.Resources.ExitRed_16;

			return b;
		}

		private string GetIPAddresses()
		{
			List<string> ips = new List<string>();
			System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
			foreach (System.Net.IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
				{
					ips.Add(ip.ToString());
				}
			}
			return String.Join(", ", ips.ToArray());
		}

		internal void LoadBlackboard()
		{
			if (!File.Exists(ConfigFile))
				btnExploreConfig.PerformClick();
			if (!LoadBlackboard(ConfigFile))
				MessageBox.Show("Blackboard configuration file does not exist or is in incorrect format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private bool LoadBlackboard(string file)
		{
			FileInfo configFileInfo;
			string bbConfigFileName;
			tcLog.SelectedTab = tpOutputLog;
			frmBbss.GbBlackboardFiles.Enabled = gbBlackboardFiles.Enabled = false;
			try
			{
				SetupBlackboard();
				configFileInfo = new FileInfo(bbConfigFile);
				bbConfigFileName = configFileInfo.FullName;
			}
			catch (Exception ex)
			{
				frmBbss.GbBlackboardFiles.Enabled = gbBlackboardFiles.Enabled = true;
				log.WriteLine("Error loading blackboard");
				log.WriteLine("\t" + ex.Message.Replace("\n", "\n\t"));
				return false;
			}

			//if (bbConfigFileName.Length > (txtConfigurationFile.Width / txtConfigurationFile.Font.SizeInPoints))
			ConfigFile = bbConfigFileName;
			//txtConfigurationFile.ScrollToCaret();
			txtBBInputPort.Text = blackboard.Port.ToString();
			txtBBAddresses.Text = GetIPAddresses();
			frmBbss.GbBlackboardFiles.Enabled = gbBlackboardFiles.Enabled = false;
			gbBlackboardSettings.Enabled = true;
			frmBbss.GbModuleList.Enabled = gbModuleList.Enabled = true;
			mnuiBlackboard_StartStop.Enabled = true;
			//tcLog.Enabled = true;
			BlackboardControlsEnabled = true;
			frmBbss.BtnStartStop.Enabled = btnStartStop.Enabled = true;
			frmBbss.BtnLoad.Text = btnLoad.Text = "Reload blackboard";
			//SendAttempts = 3;
			blackboard.StartupSequence.Method = (ModuleStartupMethod)cbModuleStartupMode.SelectedIndex;
			//blackboard.VerbosityLevel = (int)nudVerbosity.Value;
			nudVerbosity.Value = blackboard.VerbosityLevel;

			return true;
		}

		private void LoadPlugins()
		{
			string pluginsPath;
			pluginsPath = Application.StartupPath + "\\Plugins";
			LoadPlugins(pluginsPath);
		}

		private void LoadPlugins(string pluginsPath)
		{
			try
			{
				if (Directory.Exists(pluginsPath))
					blackboard.PluginManager.LoadPlugins(pluginsPath);
			}
#if DEBUG
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
#else
			catch {}
#endif
			mnuiPlugins.DropDownItems.Clear();
			foreach (IBlackboardPlugin plugin in blackboard.PluginManager)
			{
				mnuiPlugins.DropDownItems.Add(plugin.Name);
			}
		}

		private void LoadSettings()
		{
			if (bbConfigFile == null)
				return;
			GUISettings.LoadFromFile(GUISettings.GetSettingsFileName(bbConfigFile), out guiSettings);
			if ((guiSettings == null) || (guiSettings.ConigurationFileName != bbConfigFile))
			{
				if(blackboard == null)
					return;
				guiSettings = new GUISettings(blackboard, bbConfigFile);
				return;
			}
			
			this.chkAutoLog.Checked = guiSettings.AutoLog;
			this.cbModuleStartupMode.SelectedIndex = (int)guiSettings.ModuleStartupSequenceMode;
			this.nudVerbosity.Value = guiSettings.VerbosityLevel;
			SortedList<string, List<string>> acl = this.interactionTool.AutoCompleteList;
			acl.Clear();
			for (int i = 0; i < guiSettings.InjectedStrings.Count; ++i)
			{
				InjectedString istr = guiSettings.InjectedStrings[i];
				if (!acl.ContainsKey(istr.ModuleName))
					acl.Add(istr.ModuleName, new List<string>(20));
				if (!acl[istr.ModuleName].Contains(istr.Value))
					acl[istr.ModuleName].Add(istr.Value);
			}
		}

		private void ModuleEnableDisable(ModuleClientTcp module)
		{
			if (module == null)
				
				return;
			//bool state;
			//state = btnModuleEnable.Enabled;
			//btnModuleEnable.Enabled = false;
			module.Enabled = !module.Enabled;
			//btnModuleEnable.Enabled = state;
			UpdateModuleInfo();
		}

		private void ModuleProcessRun(ModuleClientTcp module)
		{
			if ((processManager == null) || (module == null))
				return;
			processManager.LaunchModule(module, ModuleStartupMethod.LaunchAlways);
		}

		private void ModuleProcessCheckRun(ModuleClientTcp module)
		{
			if ((blackboard == null) || !blackboard.IsRunning || (module == null) || !module.Enabled)
				return;
			processManager.LaunchModule(module, ModuleStartupMethod.LaunchIfNotRunning);
		}

		private void ModuleProcessKill(ModuleClientTcp module)
		{
			if ((processManager == null) || (module == null))
				return;
			processManager.ShutdownModule(module, ModuleShutdownMethod.CloseThenKill);
		}

		private void ModuleProcessZombieCheck(ModuleClient module)
		{
			if ((processManager == null) || (module == null))
				return;
		}

		private void ModuleRestart(ModuleClient module)
		{
			if ((blackboard == null) || !blackboard.IsRunning || (module == null) || !module.Enabled)
				return;
			module.Restart();
		}

		private void ModuleStart(ModuleClient module)
		{
			module.Start();
			btnModuleStartStopModule.Enabled = false;
			btnModuleStartStopModule.Text = "Starting Module";
			while (!module.IsRunning)
				Application.DoEvents();
			btnModuleStartStopModule.Text = "Stop Module";
			btnModuleStartStopModule.Image = Properties.Resources.Stop_16;
			btnModuleStartStopModule.Enabled = true;
		}

		private void ModuleStartStop(ModuleClient module)
		{
			if ((blackboard == null) || !blackboard.IsRunning || (module == null) || !module.Enabled) return;
			if (module.IsRunning)
				ModuleStop(module);
			else
				ModuleStart(module);
		}

		private void ModuleStop(ModuleClient module)
		{
			module.BeginStop();
			btnModuleStartStopModule.Enabled = false;
			btnModuleStartStopModule.Text = "Stopping Module";
			while (module.IsRunning)
				Application.DoEvents();
			btnModuleStartStopModule.Text = "Start Module";
			btnModuleStartStopModule.Image = Properties.Resources.run16;
			btnModuleStartStopModule.Enabled = true;
		}

		private void OpenConfigFile()
		{
			if(!File.Exists(bbConfigFile))
				return;
			try
			{
				FileInfo fi = new FileInfo(bbConfigFile);
				Process p = new Process();
				ProcessStartInfo psi = new ProcessStartInfo(fi.FullName);
				p.StartInfo = psi;
				p.Start();
			}
			catch{}
		}

		private void OpenLogFile()
		{
			if (!File.Exists(LogFile))
				return;
			try
			{
				FileInfo fi = new FileInfo(bbConfigFile);
				Process p = new Process();
				ProcessStartInfo psi = new ProcessStartInfo(fi.FullName);
				p.StartInfo = psi;
				p.Start();
			}
			catch { }
		}

		private void ReloadBlackboard()
		{
			if (blackboard == null)
			{
				LoadBlackboard();
				return;
			}
			if (blackboard.IsRunning)
			{
				blackboard.BeginStop();
				while (blackboard.RunningStatus != BlackboardRunningStatus.Stopped)
					Application.DoEvents();
			}
			LoadBlackboard();
		}

		private void RestartBlackboard()
		{
			if(blackboard != null)
				blackboard.Restart();
			lstCommandsPending.Items.Clear();
			lstCommandsWaiting.Items.Clear();
			lvwRedirectionHistory.Items.Clear();
		}

		private void RestartBlackboardTimer()
		{
			if(blackboard != null)
				blackboard.Inject("reset \"time\" @1");
		}

		private void RestartBlackboardTest()
		{
			blackboard.RestartTest();
			lstCommandsPending.Items.Clear();
			lstCommandsWaiting.Items.Clear();
			lvwRedirectionHistory.Items.Clear();
		}

		private void SaveSettings()
		{
			InjectedString istr;
			// First the injected string list is updated
			foreach (string moduleName in this.interactionTool.AutoCompleteList.Keys)
			{
				foreach (string injectedString in this.interactionTool.AutoCompleteList[moduleName])
				{
					istr = new InjectedString();
					istr.ModuleName = moduleName;
					istr.Value = injectedString;
					if(!guiSettings.InjectedStrings.Contains(istr))
						guiSettings.InjectedStrings.Add(istr);
				}
			}
			GUISettings.SaveToFile(guiSettings);
		}

		private void SetupStartupModeControls()
		{
			ToolStripItem item;

			this.cbModuleStartupMode.SelectedIndex = (int)ModuleStartupMethod.LaunchAndWaitReady;
			for (int i = 0; i < cbModuleStartupMode.Items.Count; ++i)
			{
				item = mnuStartSequence.Items.Add((string)cbModuleStartupMode.Items[i]);
				item.Click += new EventHandler(tsiStartSequence_Click);
				item.Tag = (ModuleStartupMethod)i;
			}
		}

		private void SetupBlackboard()
		{
			SetupLogfile();
			tbsw = new XmlTextBoxStreamWriter(txtOutputLog, bbLogFile, 1024);
			tbsw.AppendDate = true;
			log = new LogWriter(tbsw);
			processManager = new ModuleProcessManager(log);
			txtOutputLog.Clear();
			lvwRedirectionHistory.Items.Clear();
			txtOutputLog.AppendText("Loading Blackboard\r\n");
			blackboard = Blackboard.FromXML(ConfigFile, log);
			blackboard.VerbosityLevel = 1;
			//blackboard = Blackboard.FromXML("bb.xml");
			LoadPlugins();
			interactionTool.Blackboard = blackboard;
			SetupBlackboardModules();
			SetupBlackboardEvents();
			//tcLog.Enabled = true;
			BlackboardControlsEnabled = true;
			txtOutputLog.AppendText("Blackboard loaded\r\n");
			
			if(chkAutostart.Checked)
			Start();
		}

		private void SetupBlackboardEvents()
		{
			blackboard.Connected += new ModuleConnectionEH(blackboard_Connected);
			blackboard.Disconnected += new ModuleConnectionEH(blackboard_Disconnected);
			blackboard.StatusChanged += new BlackboardStatusChangedEH(blackboard_StatusChanged);
			blackboard.Modules.ModuleAdded += new IModuleAddRemoveEH(BlackboardModuleAdded); ;
#if !SPEED_UP
			blackboard.ResponseRedirected += new ResponseRedirectedEH(blackboard_ResponseRedirected);
#endif

			blackboard.VirtualModule.MachineStatusList.MachineStatusAdded += dlgMslMachineStatusAdded;
			blackboard.VirtualModule.MachineStatusList.MachineStatusRemoved += dlgMslMachineStatusRemoved;
			blackboard.VirtualModule.MachineStatusList.ElementChanged += dlgMslMachineStatusChanged;
			blackboard.VirtualModule.SharedVariables.SharedVariableAdded += dlgShvSharedVariableAdded;
		}

		private void SetupBlackboardModules()
		{
			moduleButtons.Clear();
			moduleButtonsClone.Clear();
			flpModuleList.Controls.Clear();
			frmBbss.FlpModuleList.Controls.Clear();
			mnuiModules.DropDownItems.Clear();
			foreach (IModuleClient im in blackboard.Modules)
				SetupBlackboardModule(im);
		}

		private void SetupBlackboardModule(IModuleClient iModuleClient)
		{
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.Disposing || this.IsDisposed)
					return;
				this.BeginInvoke(dlgSetupBlackboardModule, iModuleClient);
				return;
			}
			ModuleClient mc = iModuleClient as ModuleClient;
			if (mc == null) return;
			mc.StatusChanged += new StatusChangedEH(module_StatusChanged);
			AddModuleButton(mc);
			AddSecondaryModuleButton(mc);
			AddModuleMenu(mc);
		}

		private void SetupLogfile()
		{
			if (chkAutoLog.Checked)
			{
				string logPath = Application.StartupPath + "\\Logs\\" + DateTime.Today.ToString("yyyy-MM-dd");
				if (!Directory.Exists(Application.StartupPath + "\\Logs"))
					Directory.CreateDirectory(Application.StartupPath + "\\Logs");
				if (!Directory.Exists(logPath))
					Directory.CreateDirectory(logPath);
				LogFile = logPath + "\\Log_" + DateTime.Now.ToString("HH\\hmm\\mss\\s") + ".log.xml";
			}
			
			if (bbLogFile != (string)txtLogFile.Tag)
			{
				if (tbsw != null)
				{
					tbsw.Close();
				}
				tbsw = new XmlTextBoxStreamWriter(txtOutputLog, bbLogFile, 1024);
				//tbsw.LogFileVerbosityThreshold = (int)nudVerbosityLogFile.Value;
				tbsw.VerbosityThreshold = (int)nudVerbosity.Value;
				log = new LogWriter(tbsw);
				if(blackboard != null)
					blackboard.Log = log;
			}
		}

		private void ShowModuleInfo()
		{
			if (selectedMC == null) return;
			if (selectedMC is ModuleBlackboard)
			{
				ShowModuleBBInfo();
				return;
			}

			if (selectedMC is ModuleClientTcp)
			{
				txtRemoteIPAddress.Text = ((ModuleClientTcp)selectedMC).ServerAddresses.ToString();
				txtOutputPort.Text = ((ModuleClientTcp)selectedMC).Port.ToString();
			}
			txtModuleName.Text = selectedMC.Name;
			btnModuleSimulation.Visible = selectedMC.Enabled;
			btnModuleSimulation.Image = selectedMC.Simulation.SimulationEnabled ?
				Properties.Resources.CircleGreen_16 :
				Properties.Resources.CircleGray_16;
			btnSimulationEnabled.Checked = selectedMC.Simulation.SimulationEnabled;
			btnSimulationDisabled.Checked = !selectedMC.Simulation.SimulationEnabled;

			//lstModuleStartupActions.Items.Clear();
			//lstModuleStartupActions.Items.AddRange(selectedMC.GetStartupActions());

			//lstModuleStopActions.Items.Clear();
			//lstModuleStopActions.Items.AddRange(selectedMC.GetStopActions());

			//lstModuleRestartActions.Items.Clear();
			//lstModuleRestartActions.Items.AddRange(selectedMC.GetRestartActions());

			//lstModuleExecTimeOutActions.Items.Clear();
			//lstModuleExecTimeOutActions.Items.AddRange(selectedMC.GetTestTimeOutActions());

			btnModuleEnable.Enabled = !blackboard.IsRunning;
			btnModuleEnable.Visible = true;
			btnModuleEnable.Image = selectedMC.Enabled ? Properties.Resources.ExitRed_16 : Properties.Resources.ExitGreen_16;
			btnModuleEnable.Text = (selectedMC.Enabled ? "Disable " : "Enable") + " module " + selectedMC.Name;
			btnModuleEnable.ToolTipText = btnModuleEnable.Text;
			btnModuleStartStopModule.Enabled = blackboard.IsRunning && selectedMC.Enabled;
			btnModuleStartStopModule.Visible = selectedMC.Enabled;
			btnModuleStartStopModule.Text = selectedMC.IsRunning ? "Stop Client" : "Start Client";
			btnModuleStartStopModule.Image = selectedMC.IsRunning ? Properties.Resources.Stop_16 : Properties.Resources.run16;
			btnModuleRestartModule.Enabled = selectedMC.IsRunning && selectedMC.Enabled;
			btnModuleRestartModule.Visible = selectedMC.Enabled;

			txtModuleAlias.Text = String.IsNullOrEmpty( selectedMC.Alias) ? "(none)" :selectedMC.Alias;
			txtModuleAuthor.Text = String.IsNullOrEmpty(selectedMC.Author) ? "(none)" : selectedMC.Author;

			lblModuleProcessName.Visible = true;
			lblModuleExeArgs.Visible = true; 
			lblModuleExePath.Visible = true;
			txtModuleProcessName.Visible = true;
			txtModuleExeArgs.Visible = true; 
			txtModuleExePath.Visible = true;

			txtModuleProcessName.Text = String.IsNullOrEmpty(selectedMC.ProcessInfo.ProcessName) ? "(none)" : selectedMC.ProcessInfo.ProcessName;
			txtModuleExeArgs.Text = String.IsNullOrEmpty(selectedMC.ProcessInfo.ProgramArgs) ? "(none)" : selectedMC.ProcessInfo.ProgramArgs;
			txtModuleExePath.Text = String.IsNullOrEmpty(selectedMC.ProcessInfo.ProgramPath) ? "(none)" : selectedMC.ProcessInfo.ProgramPath;

			tsModuleProcess.Enabled =
				tsModuleProcess.Visible = ((selectedMC.ProcessInfo != null) &&
				!String.IsNullOrEmpty(selectedMC.ProcessInfo.ProcessName) &&
				!String.IsNullOrEmpty(selectedMC.ProcessInfo.ProgramPath));
			this.interactionTool.SelectedModule = selectedMC;
			
			//if (selectedMC.IsConnected)
			//{
			//    string suggestion;
			//    Prototype p;
			//    for (i = 0; i < selectedMC.Prototypes.Count; ++i)
			//    {
			//        p = selectedMC.Prototypes[i];
			//        suggestion = p.Command;
			//        if (p.ParamsRequired) suggestion += " \"params\"";
			//        suggestion += " @1";
			//        txtSendToModule.AutoCompleteCustomSource.Add(suggestion);
			//    }
			//}
		}

		private void ShowModuleBBInfo()
		{
			if (selectedMC == null) return;
			if (!(selectedMC is ModuleBlackboard))
			{
				ShowModuleInfo();
				return;
			}

			txtRemoteIPAddress.Text = "(none)";
			txtOutputPort.Text = "(none)";
			txtModuleName.Text = selectedMC.Name;
			btnModuleSimulation.Visible = false;
			btnModuleSimulation.Image = selectedMC.Simulation.SimulationEnabled ?
				Properties.Resources.CircleGreen_16 :
				Properties.Resources.CircleGray_16;
			btnSimulationEnabled.Checked = selectedMC.Simulation.SimulationEnabled;
			btnSimulationDisabled.Checked = !selectedMC.Simulation.SimulationEnabled;

			btnModuleEnable.Enabled = false;
			btnModuleEnable.Visible = false;
			btnModuleEnable.Image = selectedMC.Enabled ? Properties.Resources.ExitRed_16 : Properties.Resources.ExitGreen_16;
			btnModuleEnable.Text = (selectedMC.Enabled ? "Disable " : "Enable") + " module " + selectedMC.Name;
			btnModuleEnable.ToolTipText = btnModuleEnable.Text;
			btnModuleStartStopModule.Enabled = false;
			btnModuleStartStopModule.Visible = false;
			btnModuleStartStopModule.Text = selectedMC.IsRunning ? "Stop Client" : "Start Client";
			btnModuleStartStopModule.Image = selectedMC.IsRunning ? Properties.Resources.Stop_16 : Properties.Resources.run16;
			btnModuleRestartModule.Enabled = false;
			btnModuleRestartModule.Visible = false;

			txtModuleAlias.Text = "(none)";
			txtModuleAuthor.Text = "(none)";

			lblModuleProcessName.Visible = false;
			lblModuleExeArgs.Visible = false;
			lblModuleExePath.Visible = false;
			txtModuleProcessName.Visible = false;
			txtModuleExeArgs.Visible = false;
			txtModuleExePath.Visible = false;

			txtModuleProcessName.Text = String.Empty;
			txtModuleExeArgs.Text = String.Empty;
			txtModuleExePath.Text = String.Empty;

			tsModuleProcess.Enabled =
				tsModuleProcess.Visible = ((selectedMC.ProcessInfo != null) &&
				!String.IsNullOrEmpty(selectedMC.ProcessInfo.ProcessName) &&
				!String.IsNullOrEmpty(selectedMC.ProcessInfo.ProgramPath));

			//if (selectedMC.IsConnected)
			//{
			//    string suggestion;
			//    Prototype p;
			//    for (i = 0; i < selectedMC.Prototypes.Count; ++i)
			//    {
			//        p = selectedMC.Prototypes[i];
			//        suggestion = p.Command;
			//        if (p.ParamsRequired) suggestion += " \"params\"";
			//        suggestion += " @1";
			//        txtSendToModule.AutoCompleteCustomSource.Add(suggestion);
			//    }
			//}
			this.interactionTool.SelectedModule = selectedMC;
		}

		private void SimulateModule(ModuleClient module, SimulationType simulationType)
		{
			SimulateModule(module, simulationType, 1.0);
		}

		private void SimulateModule(ModuleClient module, SimulationType simulationType, double simulationRatio)
		{
			bool wasRunning;

			if ((module == null) || (blackboard == null) || (selectedMC == blackboard.VirtualModule))
				return;
			tcLog.Enabled = false;
			if (wasRunning = module.IsRunning)
			{
				module.Stop();
				while (module.IsRunning)
					Application.DoEvents();
			}
			if (simulationType == SimulationType.SimulationDisabled)
				module.Simulation.SuccessRatio = 2;
			else
				module.Simulation.SuccessRatio = simulationRatio;

			if (wasRunning)
			{
				module.Start();
				while (!module.IsRunning)
					Application.DoEvents();
			}
			tcLog.Enabled = true;
			ShowModuleInfo();
		}

		private void ShowHumanCheckList()
		{
			FrmHumanCheckList frm = new FrmHumanCheckList();
			FileInfo cfgFileInfo = new FileInfo(this.ConfigFile);
			if (frm.LoadList(cfgFileInfo.DirectoryName + "\\HumanCheckList.txt") < 1)
				return;
			if (frm.ShowDialog(this) == DialogResult.OK)
				return;
			Stop();
		}

		private void Start()
		{
			if (blackboard.IsRunning) return;
			txtOutputLog.AppendText("Starting Blackboard\r\n");
			frmBbss.BtnStartStop.Enabled = btnStartStop.Enabled = false;
			blackboard.BeginStart();
			frmBbss.BtnStartStop.Text = btnStartStop.Text = "Stop Blackboard";
			frmBbss.BtnLoad.Enabled = btnLoad.Enabled = false;
			//btnStartStop.Enabled = true;
			timer.Start();
			ShowHumanCheckList();
		}

		private void Stop()
		{
			if (!blackboard.IsRunning) return;
			txtOutputLog.AppendText("Stopping Blackboard\r\n");
			frmBbss.BtnStartStop.Enabled=btnStartStop.Enabled = false;
			blackboard.BeginStop();
			frmBbss.BtnStartStop.Text = btnStartStop.Text = "Start Blackboard";
			timer.Stop();
			//frmBbss.FlpModuleList.Enabled = flpModuleList.Enabled = false;
			lvSharedVariables.Items.Clear();
			//btnStartStop.Enabled = true;
		}

		private void UpdateModuleInfo()
		{
			if (!this.IsHandleCreated || this.Disposing || this.IsDisposed)
				return;
			if (this.InvokeRequired)
			{
				this.BeginInvoke(updateModuleInfo);
				return;
			}
			ShowModuleInfo();
		}

		private void UpdateSharedVariables()
		{
			ListViewItem lvi;
			string[] varNames;
			
			lvSharedVariables.Items.Clear();
			varNames = blackboard.VirtualModule.SharedVariables.GetVariableNames();

			for (int i = 0; i < varNames.Length; ++i)
			{
				lvi = lvSharedVariables.Items.Add(varNames[i]);
				lvi.Tag = blackboard.VirtualModule.SharedVariables[varNames[i]];
			}
			lvSharedVariables.Refresh();
		}

		#region RedirectionHistory Update and sort

#if !SPEED_UP

		private void AddRedirectionListItem(Command command, Response response)
		{
			if (this.InvokeRequired)
			{
				if (!IsHandleCreated || IsDisposed || Disposing)
					return;
				this.BeginInvoke(dlgAddRedirectionListItem, command, response);
				return;
			}

			lvwRedirectionHistory.Items.Add(CreateRedirectionListItem(command, response));
			lvwRedirectionHistory.Refresh();
		}

		private ListViewItem CreateRedirectionListItem(Command command, Response response)
		{
			ListViewItem item;
			int index;

			if (command == null)
			{
				item = new ListViewItem("#Corrupt#");
				item.SubItems.Add("#Corrupt#");
				item.SubItems.Add("#Corrupt#");
				item.SubItems.Add("#Corrupt#");
				item.SubItems.Add("#Corrupt#");
				item.SubItems.Add("#Corrupt#");
				item.SubItems.Add("#Corrupt#");
				item.SubItems.Add("#Unknown#");
				item.BackColor = Color.Red;
				return item;
			}

			if ((response == null) && (command.Response != null))
				response = command.Response;

			item = new ListViewItem(command.ToString());
			index = lvwRedirectionHistory.Items.Count;
			item.Tag = command;
			if (response != null) item.SubItems.Add(response.ToString());
			else item.SubItems.Add("#Corrupt#");
			item.SubItems.Add(command.Source.Name);
			item.SubItems.Add(command.Destination.Name);
			item.SubItems.Add(command.SentTime.ToString());
			if (response != null)
			{
				item.SubItems.Add(response.ArrivalTime.ToString());
				item.SubItems.Add(response.FailReason.ToString());
				item.SubItems.Add((response.SentStatus == SentStatus.SentSuccessfull ? "Yes" : "No"));

				if (response.FailReason == ResponseFailReason.ExecutedButNotSucceded)
				{
					if (index % 2 == 0) item.BackColor = Color.Lime;
					else item.BackColor = Color.LightGreen;
				}
				else if (response.SentStatus != SentStatus.SentSuccessfull)
				{
					if (index % 2 == 0) item.BackColor = Color.LightYellow;
					else item.BackColor = Color.LightGoldenrodYellow;
				}
				else if (response.FailReason != ResponseFailReason.None)
				{
					if (index % 2 == 0) item.BackColor = Color.LightPink;
					else item.BackColor = Color.LightSalmon;
				}
				else
				{
					if (index % 2 == 0) item.BackColor = Color.White;
					else item.BackColor = Color.WhiteSmoke;
				}
			}
			else
			{
				item.SubItems.Add("#Corrupt#");
				item.SubItems.Add("#Corrupt#");
				item.SubItems.Add("#Unknown#");
			}
			return item;
		}

		private void UpdateRedirectionList()
		{
			//if (lvwRedirectionHistory.Items.Count == dispatchedCommands.Count) return;

			Command command;
			Response response;
			ListViewItem item;
			Comparison<Command> comparison;
			//int selectedIndex;
			//Command selectedItem;


			// Sort the items
//#region Sort the items
			switch (lvwRedirectionHistory.Columns[redirectionHistoryColumn].Name)
			{
				case "chCommand":
					comparison = new Comparison<Command>(rhComparisonCommand);
					break;

				case "chCommandDestination":
					comparison = new Comparison<Command>(rhComparisonDestination);
					break;

				default:
				case "chCommandSent":
					comparison = new Comparison<Command>(rhComparisonSentTime);
					break;

				case "chCommandSource":
					comparison = new Comparison<Command>(rhComparisonSource);
					break;

				case "chInfo":
					comparison = new Comparison<Command>(rhComparisonInfo);
					break;

				case "chResponse":
					comparison = new Comparison<Command>(rhComparisonResponse);
					break;

				case "chResponseArrival":
					comparison = new Comparison<Command>(rhComparisonArrival);
					break;

				case "chResponseSent":
					comparison = new Comparison<Command>(rhComparisonSentStatus);
					break;
			}
//#endregion
			dispatchedCommands.Sort(comparison);

			//if ((lvwRedirectionHistory.SelectedIndices.Count > 0) && (lvwRedirectionHistory.SelectedIndices[0] != -1))
			//{
			//    selectedIndex = lvwRedirectionHistory.SelectedIndices[0];
			//    selectedItem = (Command)lvwRedirectionHistory.Items[selectedIndex].Tag;
			//    selectedIndex = dispatchedCommands.IndexOf(selectedItem);
			//}
			lvwRedirectionHistory.Items.Clear();
			for (int i = 0; i < dispatchedCommands.Count; ++i)
			{
				command = dispatchedCommands[i];
				response = command.Response;

				item = CreateRedirectionListItem(command, response);
				lvwRedirectionHistory.Items.Add(item);

			}
			//if(selectedIndex != -1)
			//	lvwRedirectionHistory.Items.
		}

		private int rhComparisonCommand(Command c1, Command c2)
		{
			return c1.ToString().CompareTo(c2.ToString());
		}

		private int rhComparisonResponse(Command c1, Command c2)
		{
			return c1.Response.ToString().CompareTo(c2.Response.ToString());
		}

		private int rhComparisonSource(Command c1, Command c2)
		{
			return c1.Source.CompareTo(c2.Source);
		}

		private int rhComparisonDestination(Command c1, Command c2)
		{
			return c1.Destination.CompareTo(c2.Destination);
		}

		private int rhComparisonSentTime(Command c1, Command c2)
		{
			return c1.SentTime.CompareTo(c2.SentTime);
		}

		private int rhComparisonArrival(Command c1, Command c2)
		{
			return c1.Response.ArrivalTime.CompareTo(c2.Response.ArrivalTime);
		}

		private int rhComparisonInfo(Command c1, Command c2)
		{
			return c1.Response.FailReason.CompareTo(c2.Response.FailReason);
		}

		private int rhComparisonSentStatus(Command c1, Command c2)
		{
			return c1.Response.SentStatus.CompareTo(c2.Response.SentStatus);
		}

#endif
		#endregion

		#endregion

		#region Form EventHandler functions

		#region General (form) Controls

		private void btnStartStop_Click(object sender, EventArgs e)
		{
			BlackboardStartStopToggle();
		}

		private void FrmBlackboard_Load(object sender, EventArgs e)
		{
			//this.Width = 1024;
			//this.Height = 700;
			tcLog.SelectedTab = tpOutputLog;
			if (AutoStart && File.Exists(bbConfigFile))
				LoadBlackboard(bbConfigFile);
			frmBbss.Show();

			//LoadPluginsFromManagedDll("Plugins\\PluginExample.dll");
		}

		private void chkAutoLog_CheckedChanged(object sender, EventArgs e)
		{
			this.lblLogFile.Enabled = !chkAutoLog.Checked;
			this.txtLogFile.Text = "(Automatically generated)";
			this.txtLogFile.Enabled = !chkAutoLog.Checked;
			this.btnExploreLog.Enabled = !chkAutoLog.Checked;
		}

		private void FrmBlackboard_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (requestClose && (blackboard.RunningStatus != BlackboardRunningStatus.Stopped) && (e.CloseReason == CloseReason.UserClosing))
			{
				if (MessageBox.Show("Blackboard is still closing. You can kill the process but\r\nis recommended wait until shutdown process is complete.\r\n\r\nKill Blackboard?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) Application.Exit();
				else
				{
					e.Cancel = true;
					return;
				}
			}
			if ((blackboard != null) && (blackboard.IsRunning))
			{
				//this.ControlBox = false;
				this.Text = this.Text += " - Closing...";
				//this.Visible = false;
				requestClose = true;
				Stop();
				//e.Cancel = true;
				while (blackboard.RunningStatus != BlackboardRunningStatus.Stopped) Application.DoEvents();
			}
		}

		private void FrmBlackboard_FormClosed(object sender, FormClosedEventArgs e)
		{
			frmBbss.Hide();
			frmBbss.Close();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			try
			{
				txtTimeLeft.Text = blackboard.AutoStopTimeLeft.TotalHours.ToString("00") + ":" +
					blackboard.AutoStopTimeLeft.Minutes.ToString("00") + ":" +
					blackboard.AutoStopTimeLeft.Seconds.ToString("00");
				lstCommandsPending.Items.Clear();
				lstCommandsWaiting.Items.Clear();
				lstCommandsPending.Items.AddRange(blackboard.PendingCommands);
				lstCommandsWaiting.Items.AddRange(blackboard.WaitingCommands);
				txtClientsConnected.Text = blackboard.ClientsConnected.ToString();
				log.Flush();
				//UpdateRedirectionList();
				//for (int i = 0; i < blackboard.Modules.Count; ++i)
				//{
				//    if (blackboard.Modules[i].Busy)
				//    {
				//        moduleButtons[blackboard.Modules[i].Name].Image = Properties.Resources.busy16;
				//        moduleButtonsClone[blackboard.Modules[i].Name].Image = Properties.Resources.busy16;
				//        continue;
				//    }
				//    if (!blackboard.Modules[i].IsConnected)
				//    {
				//        moduleButtons[blackboard.Modules[i].Name].Image = Properties.Resources.err16;
				//        moduleButtonsClone[blackboard.Modules[i].Name].Image = Properties.Resources.err16;
				//        continue;
				//    }
				//    if (!blackboard.Modules[i].IsAlive || !blackboard.Modules[i].Ready)
				//    {
				//        moduleButtons[blackboard.Modules[i].Name].Image = Properties.Resources.wrn16;
				//        moduleButtonsClone[blackboard.Modules[i].Name].Image = Properties.Resources.wrn16;
				//    }
				//    else
				//    {
				//        moduleButtons[blackboard.Modules[i].Name].Image = Properties.Resources.ok16;
				//        moduleButtonsClone[blackboard.Modules[i].Name].Image = Properties.Resources.ok16;
				//    }
				//}
				if(batCount > 10)
					CheckMachineStatus();
			}
			catch { }
		}

		private void moduleButton_Click(object sender, EventArgs e)
		{
			ModuleClient mc;
			try
			{
				if (!(sender is Button)) return;
				if (!blackboard.Modules.Contains(((Button)sender).Text)) return;
				if (!(blackboard.Modules[((Button)sender).Text] is ModuleClient)) return;
				mc = (ModuleClient)blackboard.Modules[((Button)sender).Text];
				selectedMC = mc;
				ShowModuleInfo();
				tcLog.SelectTab(tpModuleInfo);
			}
			catch { }
		}

		internal void btnRestartTest_Click(object sender, EventArgs e)
		{
			RestartBlackboardTest();
		}

		internal void btnRestartBlackboard_Click(object sender, EventArgs e)
		{
			RestartBlackboard();
		}

		internal void btnRestartTimer_Click(object sender, EventArgs e)
		{
			RestartBlackboardTimer();
		}

		private void nudRetrySendAttempts_ValueChanged(object sender, EventArgs e)
		{
			SendAttempts = (int)nudRetrySendAttempts.Value;
		}

		private void txtConfigurationFile_TextChanged(object sender, EventArgs e)
		{
			ConfigFile = txtConfigurationFile.Text;
		}

		private void txtLogFile_TextChanged(object sender, EventArgs e)
		{
			LogFile = txtLogFile.Text;
		}

		private void nudVerbosity_ValueChanged(object sender, EventArgs e)
		{
			//tbsw.TextBoxVerbosityThreshold = (int)nudVerbosity.Value;
			if(blackboard != null)
				blackboard.VerbosityLevel = (int)nudVerbosity.Value;
		}

		private void nudVerbosityLogFile_ValueChanged(object sender, EventArgs e)
		{
			//tbsw.LogFileVerbosityThreshold = (int)nudVerbosityLogFile.Value;
		}

		private void chkRedirectionHistory_CheckedChanged(object sender, EventArgs e)
		{
			redirectionHistoryEnabled = chkRedirectionHistory.Checked;
		}

		private void lvSharedVariables_SelectedIndexChanged(object sender, EventArgs e)
		{
			SharedVariable shVar;
			if ((lvSharedVariables.SelectedItems == null) || (lvSharedVariables.SelectedItems.Count < 1))
			{
				shVar = pgSelectedSharedVar.SelectedObject as SharedVariable;
				if (shVar != null)
				{
				}
				pgSelectedSharedVar.SelectedObject = null;
				pgSelectedSharedVar.Refresh();
				return;
			}
			shVar = lvSharedVariables.SelectedItems[0].Tag as SharedVariable;
			if (shVar == null) return;
			pgSelectedSharedVar.SelectedObject = shVar;
			pgSelectedSharedVar.Refresh();
		}

		#endregion

		#region Module Toolbar

		private void btnModuleEnable_Click(object sender, EventArgs e)
		{
			ModuleClientTcp selectedMC = this.selectedMC as ModuleClientTcp;
			if ((blackboard == null) || (selectedMC == null))
				return;
			ModuleEnableDisable(selectedMC);
		}

		private void btnStartSequence_Click(object sender, EventArgs e)
		{
			mnuStartSequence.Show(
				btnStartSequence,
				new Point(btnStartSequence.Width - 5, 0),
				ToolStripDropDownDirection.Right);
		}

		private void tsiStartSequence_Click(object sender, EventArgs e)
		{
			ToolStripItem item;

			item = sender as ToolStripItem;
			if ((blackboard == null) || (item == null) || !(item.Tag is ModuleStartupMethod))
				return;
			blackboard.StartupSequence.StartModulesAsync((ModuleStartupMethod)item.Tag);
		}

		private void btnSimulationEnabled_Click(object sender, EventArgs e)
		{
			SimulateModule(selectedMC, SimulationType.EnabledExact);
		}

		private void btnSimulationDisabled_Click(object sender, EventArgs e)
		{
			SimulateModule(selectedMC, SimulationType.SimulationDisabled);
		}

		private void btnShowModuleActions_Click(object sender, EventArgs e)
		{
			FrmModuleActions frmActions;

			if (selectedMC == null)
				return;
			frmActions = new FrmModuleActions();
			frmActions.Module = selectedMC;
			frmActions.ShowDialog(this);
		}

		private void btnModuleProcessRun_Click(object sender, EventArgs e)
		{
			ModuleClientTcp selectedMC = this.selectedMC as ModuleClientTcp;
			if ((processManager == null) || (selectedMC == null))
				return;
			processManager.LaunchModule(selectedMC, ModuleStartupMethod.LaunchAlways);
		}

		private void btnModuleProcessCheckRun_Click(object sender, EventArgs e)
		{
			ModuleClientTcp selectedMC = this.selectedMC as ModuleClientTcp;
			if ((processManager == null) || (selectedMC == null))
				return;
			processManager.LaunchModule(selectedMC, ModuleStartupMethod.LaunchIfNotRunning);
		}

		private void btnModuleProcessKill_Click(object sender, EventArgs e)
		{
			ModuleClientTcp selectedMC = this.selectedMC as ModuleClientTcp;
			if ((processManager == null) || (selectedMC == null))
				return;
			processManager.ShutdownModule(selectedMC, ModuleShutdownMethod.CloseThenKill);
		}

		private void btnModuleProcessZombieCheck_Click(object sender, EventArgs e)
		{
			if ((processManager == null) || (selectedMC == null))
				return;
		}

		#endregion

		#region Blackboard Menu

		private void mnuiBlackboard_Exit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void mnuiBlackboard_Load_Click(object sender, EventArgs e)
		{
			LoadBlackboard();
		}

		private void mnuiBlackboard_OpenCfgFile_Click(object sender, EventArgs e)
		{
			OpenConfigFile();
		}

		private void mnuiBlackboard_OpenLog_Click(object sender, EventArgs e)
		{
			OpenLogFile();
		}

		private void mnuiBlackboard_Reload_Click(object sender, EventArgs e)
		{
			ReloadBlackboard();
		}

		private void mnuiBlackboard_Restart_Blackboard_Click(object sender, EventArgs e)
		{
			RestartBlackboard();
		}

		private void mnuiBlackboard_Restart_Test_Click(object sender, EventArgs e)
		{
			RestartBlackboardTest();
		}

		private void mnuiBlackboard_Restart_Timer_Click(object sender, EventArgs e)
		{
			RestartBlackboardTimer();
		}

		private void mnuiBlackboard_StartStop_Click(object sender, EventArgs e)
		{
			BlackboardStartStopToggle();
		}

		#endregion

		#region Module menu

		private void ModuleMenu_DropDownOpening(object sender, EventArgs e)
		{
			ToolStripMenuItem mnu = sender as ToolStripMenuItem;
			if (mnu == null)
				return;
			PopulateModuleMenu(mnu);
		}

		private void mnuiModule_Module_CheckAndRun_Click(object sender, EventArgs e)
		{
			ModuleClientTcp mc;
			if (!(sender is ToolStripItem) || ((mc = ((ToolStripItem)sender).Tag as ModuleClientTcp) == null))
				return;
			ModuleProcessCheckRun(mc);
		}

		private void mnuiModule_Module_CloseAndKill_Click(object sender, EventArgs e)
		{
			ModuleClientTcp mc;
			if (!(sender is ToolStripItem) || ((mc = ((ToolStripItem)sender).Tag as ModuleClientTcp) == null))
				return;
			ModuleProcessKill(mc);
		}

		private void mnuiModule_Module_Enabled_Click(object sender, EventArgs e)
		{
			ModuleClientTcp mc;
			if (!(sender is ToolStripItem) || ((mc = ((ToolStripItem)sender).Tag as ModuleClientTcp) == null))
				return;
			ModuleEnableDisable(mc);
		}

		private void mnuiModule_Module_Restart_Click(object sender, EventArgs e)
		{
			ModuleClient mc;
			if (!(sender is ToolStripItem) || ((mc = ((ToolStripItem)sender).Tag as ModuleClient) == null))
				return;
			ModuleRestart(mc);
		}

		private void mnuiModule_Module_Run_Click(object sender, EventArgs e)
		{
			ModuleClientTcp mc;
			if (!(sender is ToolStripItem) || ((mc = ((ToolStripItem)sender).Tag as ModuleClientTcp) == null))
				return;
			ModuleProcessRun(mc);
		}

		private void mnuiModule_Module_Simulation_Click(object sender, EventArgs e)
		{
			ModuleClient mc;
			if (!(sender is ToolStripItem) || ((mc = ((ToolStripItem)sender).Tag as ModuleClient) == null))
				return;
			if(mc.Simulation.SimulationEnabled)
				SimulateModule(mc, SimulationType.SimulationDisabled);
			else 
				SimulateModule(mc, SimulationType.EnabledExact);
		}

		private void mnuiModule_Module_Start_Click(object sender, EventArgs e)
		{
			ModuleClient mc;
			if (!(sender is ToolStripItem) || ((mc = ((ToolStripItem)sender).Tag as ModuleClient) == null))
				return;
			ModuleStartStop(mc);
		}

		private void mnuiModule_Module_ZombieCheck_Click(object sender, EventArgs e)
		{
			ModuleClient mc;
			if (!(sender is ToolStripItem) || ((mc = ((ToolStripItem)sender).Tag as ModuleClient) == null))
				return;
			ModuleProcessZombieCheck(mc);
		}

		#endregion

		#region RedirectionHistory EventHandlers

		private void lvwRedirectionHistory_ColumnClick(object sender, ColumnClickEventArgs e)
		{
#if !SPEED_UP
			redirectionHistoryColumn = e.Column;
			UpdateRedirectionList();
#endif
		}

		#endregion

		#region TabControl controls

		private void btnClearOutputLog_Click(object sender, EventArgs e)
		{
			txtOutputLog.Clear();
		}

		private void ChangeToLogTab(object sender, EventArgs e)
		{
			tcLog.SelectTab(0);
		}

		private void btnSaveOutputLog_Click(object sender, EventArgs e)
		{
			if (dlgSaveLog.FileName == "") dlgSaveLog.FileName = DateTime.Now.ToString("yyyy-MM-dd hhmm") + ".log.xml";
			if (dlgSaveLog.ShowDialog() != DialogResult.OK) return;
			try
			{
				File.WriteAllText(dlgSaveLog.FileName, txtOutputLog.Text);
			}
			catch
			{
				MessageBox.Show("Cannot save to output log file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void tcLog_Selecting(object sender, TabControlCancelEventArgs e)
		{
			if ((e.TabPage == tpModuleInfo) && (selectedMC == null))
			{
				if (blackboard == null)
				{
					e.Cancel = true;
					return;
				}
				selectedMC = blackboard.VirtualModule;
				//e.Cancel = true;
			}
			else if ((e.TabPage == tpSharedVars) && (blackboard == null))
			{
				e.Cancel = true;
				return;
			}
			else if (((e.TabPage == tpDiagnostics) || (e.TabPage == tpInjector)) && ((blackboard == null) || !blackboard.IsRunning))
			{
				e.Cancel = true;
				return;
			}
		}

		private void txtSendToModule_Enter(object sender, EventArgs e)
		{
			//txtSendToModule.AutoCompleteCustomSource.
		}

		private void btnModuleRestartModule_Click(object sender, EventArgs e)
		{
			ModuleRestart(selectedMC);
		}

		private void btnModuleStartStopModule_Click(object sender, EventArgs e)
		{
			ModuleClient module;
			if ((module = selectedMC) == null)
				return;
			ModuleStartStop(module);
		}

		private void btnModuleStartStopClient_Click(object sender, EventArgs e)
		{

		}

		private void btnModuleRestartClient_Click(object sender, EventArgs e)
		{

		}

		#endregion

		#region Group BlackboardSettingsFile controls

		internal void btnExploreConfig_Click(object sender, EventArgs e)
		{
			if (dlgOpenSettingsFile.ShowDialog() != DialogResult.OK) return;
			ConfigFile = dlgOpenSettingsFile.FileName;
		}

		internal void btnExploreLog_Click(object sender, EventArgs e)
		{
			if (dlgSaveLog.ShowDialog() != DialogResult.OK) return;
			LogFile = dlgSaveLog.FileName;
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{
			LoadBlackboard();
		}

		#endregion

		#endregion

		#region Blackboard EventHandler functions

		private void blackboard_Connected(ModuleClient m)
		{
			try
			{
				if (!this.IsHandleCreated || this.Disposing || this.IsDisposed)
					return;
				moduleButtons[m.Name].Image = Properties.Resources.ok16;
				moduleButtonsClone[m.Name].Image = Properties.Resources.ok16;
				if (selectedMC == null) selectedMC = m;
				if (selectedMC == m)
					UpdateModuleInfo();
			}
			catch { }
		}

		private void blackboard_Disconnected(ModuleClient m)
		{
			try
			{
				if (!this.IsHandleCreated || this.Disposing || this.IsDisposed)
					return;
				moduleButtons[m.Name].Image = m.Enabled? Properties.Resources.err16 : Properties.Resources.ExitRed_16;
				moduleButtonsClone[m.Name].Image = m.Enabled ? Properties.Resources.err16 : Properties.Resources.ExitRed_16;
				if (selectedMC == m)
					UpdateModuleInfo();
			}
			catch { }
		}

		void BlackboardModuleAdded(IModuleClient module)
		{
			SetupBlackboardModule(module);
		}

		private void module_StatusChanged(IModuleClient sender)
		{
			try
			{
				if (this.InvokeRequired)
				{
					if (!this.IsHandleCreated || this.Disposing || this.IsDisposed)
						return;
					this.BeginInvoke(dlgModuleStatusChanged, sender);
					return;
				}
				//moduleButtons[sender.Name].Enabled = sender.IsRunning;
				//moduleButtonsClone[sender.Name].Enabled = sender.IsRunning;

				if (sender == selectedMC)
				{
					if (sender.IsRunning)
					{
						btnModuleStartStopModule.Text = "Stop Module";
						btnModuleStartStopModule.Image = Properties.Resources.Stop_16;
						btnModuleStartStopModule.Enabled = true;
					}
					else
					{
						btnModuleStartStopModule.Text = "Start Module";
						btnModuleStartStopModule.Image = Properties.Resources.run16;
						btnModuleStartStopModule.Enabled = true;
					}
				}

				if (!sender.Enabled)
				{
					moduleButtons[sender.Name].Image = Properties.Resources.ExitRed_16;
					moduleButtonsClone[sender.Name].Image = Properties.Resources.ExitRed_16;
					return;
				}

				if (!sender.IsRunning)
				{
					moduleButtons[sender.Name].Image = Properties.Resources.ExitGreen_16;
					moduleButtonsClone[sender.Name].Image = Properties.Resources.ExitGreen_16;
					return;
				}
				if (sender.Busy)
				{
					moduleButtons[sender.Name].Image = Properties.Resources.busy16;
					moduleButtonsClone[sender.Name].Image = Properties.Resources.busy16;
					return;
				}
				if (!sender.IsConnected)
				{
					moduleButtons[sender.Name].Image = Properties.Resources.err16;
					moduleButtonsClone[sender.Name].Image = Properties.Resources.err16;
					return;
				}
				if (!sender.IsAlive || !sender.Ready)
				{
					moduleButtons[sender.Name].Image = Properties.Resources.wrn16;
					moduleButtonsClone[sender.Name].Image = Properties.Resources.wrn16;
				}
				else
				{
					moduleButtons[sender.Name].Image = Properties.Resources.ok16;
					moduleButtonsClone[sender.Name].Image = Properties.Resources.ok16;
				}
			}
			catch { }
		}

		private void blackboard_StatusChanged(IBlackboard blackboard)
		{
			bbStatusChanged();
		}

		private void bbStatusChanged()
		{
			try
			{
				if (((blackboard.RunningStatus == BlackboardRunningStatus.Stopped) || (blackboard.RunningStatus == BlackboardRunningStatus.Stopping)) && requestClose)
				{
					timer.Stop();
					Application.Exit();
					return;
				}
				if (this.InvokeRequired)
				{
					if (!this.IsHandleCreated || this.Disposing || this.IsDisposed)
						return;

					this.BeginInvoke(dlgBbStatusChanged);
					return;
				}

				//frmBbss.FlpModuleList.Enabled = flpModuleList.Enabled = (blackboard.RunningStatus == BlackboardRunningStatus.Running);
				//BlackboardControlsEnabled = (blackboard.RunningStatus != BlackboardRunningStatus.Stopped);
				//frmBbss.GbGeneralActions.Enabled = gbGeneralActions.Enabled = (blackboard.RunningStatus == BlackboardRunningStatus.Running);
				switch (blackboard.RunningStatus)
				{
					case BlackboardRunningStatus.Restarting:
						BlackboardRestarting();
						break;

					case BlackboardRunningStatus.RestartingTest:
						BlackboardRestartingTest();
						break;

					case BlackboardRunningStatus.Starting:
						BlackboardStarting();
						break;

					case BlackboardRunningStatus.Stopping:
						BlackboardStopping();
						break;

					case BlackboardRunningStatus.Running:
						BlackboardRunning();
						break;
					case BlackboardRunningStatus.Stopped:
						BlackboardStopped();
						break;
				}
			}
			catch { }
		}

#if !SPEED_UP

		private void blackboard_ResponseRedirected(Command command, Response response, bool sendResponseSuccess)
		{
			if (!redirectionHistoryEnabled)
				return;
			try
			{
				if (this.InvokeRequired)
				{
					if (!this.IsHandleCreated || this.Disposing || this.IsDisposed)
						return;
					this.BeginInvoke(dlgBlackboardResponseRedirected, command, response, sendResponseSuccess);
					return;
				}

				if (command == null)
					return;
				//if ((command.Response == null) && (response != null))
					//command.Response = null;
				dispatchedCommands.Add(command);
				AddRedirectionListItem(command, response);
			}
			catch { }
		}

#endif

		private void SharedVariables_SharedVariableAdded(SharedVariableCollection collection, SharedVariable variable)
		{
			if (variable == null)
				return;
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.Disposing || this.IsDisposed)
					return;
				this.BeginInvoke(dlgShvSharedVariableAdded, collection, variable);
				return;
			}

			if(lvSharedVariables.Items.Count < (collection.Count -1))
			{
				UpdateSharedVariables();
				return;
			}

			ListViewItem lvi = lvSharedVariables.Items.Add(variable.Name);
			lvi.Tag = variable;
			lvSharedVariables.Refresh();
		}

		#region Status and reporting management

		private void MachineStatusList_ElementChanged(int index, MachineStatus machineStatus)
		{
			#region Async Call Management
			try
			{
				if (!IsHandleCreated || IsDisposed || Disposing) return;
				if (InvokeRequired)
				{
					this.BeginInvoke(dlgMslMachineStatusChanged, index, machineStatus);
					return;
				}
			}
			catch { return; }
			#endregion

			if (rmsList.ContainsKey(machineStatus.IPAddress))
			{
				MachineStatusControl msc = rmsList[machineStatus.IPAddress];
				msc.MachineStatus = machineStatus;
			}
		}

		private void MachineStatusList_MachineStatusRemoved(MachineStatus machineStatus)
		{
			#region Async Call Management
			try
			{
				if (!IsHandleCreated || IsDisposed || Disposing) return;
				if (InvokeRequired)
				{
					this.BeginInvoke(dlgMslMachineStatusRemoved, machineStatus);
					return;
				}
			}
			catch { return; }
			#endregion

			if (rmsList.ContainsKey(machineStatus.IPAddress))
			{
				MachineStatusControl msc = rmsList[machineStatus.IPAddress];
				rmsList.Remove(machineStatus.IPAddress);
				if (flpRemoteMachineStatus.Contains(msc))
					flpRemoteMachineStatus.Controls.Remove(msc);
				msc.Dispose();
			}
		}

		private void MachineStatusList_MachineStatusAdded(MachineStatus machineStatus)
		{
			#region Async Call Management
			try
			{
				if (!IsHandleCreated || IsDisposed || Disposing) return;
				if (InvokeRequired)
				{
					this.BeginInvoke(dlgMslMachineStatusAdded, machineStatus);
					return;
				}
			}
			catch { return; }
			#endregion

			if (!rmsList.ContainsKey(machineStatus.IPAddress))
			{
				MachineStatusControl msc = new MachineStatusControl(machineStatus);
				rmsList.Add(machineStatus.IPAddress, msc);
				if (!flpRemoteMachineStatus.Contains(msc))
					flpRemoteMachineStatus.Controls.Add(msc);
			}
		}

		#endregion

		#endregion


		/// <summary>
		/// Loads plugins from a managed Dll file
		/// </summary>
		/// <param name="dll">A DllInfo object which contains information about the file which contains the plugins to load</param>
		protected virtual void LoadPluginsFromManagedDll(string path)
		{
			System.Reflection.Assembly assembly;
			Type[] types;
			List<IBlackboardPlugin> pluginList = new List<IBlackboardPlugin>();
			IBlackboardPlugin instance;

			//assembly = System.Reflection.Assembly.LoadFrom(path);
			assembly = System.Reflection.Assembly.LoadFile(Application.StartupPath + "\\"+ path);

			if (assembly.ManifestModule.Name == "Robotics.dll")
				return;
			types = assembly.GetTypes();

			foreach (Type type in types)
			{
				if ((type.GetInterface("IBlackboardPlugin") == null) || type.IsAbstract)
					continue;

				//try
				//{
				//instance = (Plugin)Activator.CreateInstance(type);
				instance = (IBlackboardPlugin)assembly.CreateInstance(type.FullName);
				//instance = (Plugin) AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(dll.FilePath, type.FullName);
				pluginList.Add(instance);
				//}
				//catch { continue; }
			}
		}
	}
}