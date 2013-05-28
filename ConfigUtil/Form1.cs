using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Blk.Engine;
using Robotics;

namespace ConfigUtil
{
	public partial class Form1 : Form
	{
		private VoidEventHandler dlgClearAll;
		private VoidEventHandler dlgFillBlackboardData;

		private ConfigurationHelper configurationHelper;
		private SortedList<string, ModuleClient> modules;
		private SortedList<string, Prototype> prototypes;
		private ModuleClient selectedModule;

		public Form1()
		{
			InitializeComponent();
			dlgClearAll = new VoidEventHandler(ClearAll);
			dlgFillBlackboardData = new VoidEventHandler(FillBlackboardData);

			modules = new SortedList<string, ModuleClient>();
			prototypes = new SortedList<string, Prototype>();
			ConfigurationHelper.Blackboard = null;
		}

		#region Properties

		public ConfigurationHelper ConfigurationHelper
		{
			get { return this.configurationHelper; }
			set
			{
				if (value == null)
					ClearAll();
				tpLeftTabs.Enabled = (value != null);
				//gbModule.Enabled = (value != null);
				this.configurationHelper = value;
			}
		}

		#endregion

		#region Methods

		private void AddPrototype(Prototype proto)
		{
			ReplacePrototype(selectedModule, null, proto);
		}

		private void ClearAll()
		{
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.IsDisposed || this.Disposing)
					return;
				this.BeginInvoke(dlgClearAll);
				return;
			}
			txtBBName.Text = "";
			nudBBPort.Value = nudBBPort.Minimum;
			nudBBAutoStop.Value = nudBBAutoStop.Minimum;
			nudTestTimeOut.Value = nudTestTimeOut.Minimum;
			modules.Clear();
			prototypes.Clear();
			lstBBModules.Items.Clear();
			//ClearModule();
		}

		private void ChangeModuleName()
		{
			throw new NotImplementedException();
		}

		private void ClearModule()
		{
			throw new NotImplementedException();
		}

		private void DeleteModule()
		{
			if (selectedModule == null)
				return;
			ModuleClient module = selectedModule;
			selectedModule = null;
			//gbModule.Enabled = false;
			ClearModule();
			if (modules.ContainsKey(module.Name))
				modules.Remove(module.Name);
			if (ConfigurationHelper.Blackboard.Modules.Contains(module))
				ConfigurationHelper.Blackboard.Modules.Remove(module);
			if (lstBBModules.Items.Contains(module))
				lstBBModules.Items.Remove(module);
			for (int i = 0; i < module.Prototypes.Count; ++i)
			{
				if (prototypes.ContainsKey(module.Prototypes[i].Command))
					prototypes.Remove(module.Prototypes[i].Command);
			}
		}

		private void DeletePrototype(Prototype proto)
		{
			if (proto == null)
				return;

			prototypes.Remove(proto.Command);
			for (int i = 0; i < ConfigurationHelper.Blackboard.Modules.Count; ++i)
				ConfigurationHelper.Blackboard.Modules[i].Prototypes.Remove(proto);
		}

		private void FillBlackboardData()
		{
			if (ConfigurationHelper.Blackboard == null)
				return;
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.IsDisposed || this.Disposing)
					return;
				this.BeginInvoke(dlgFillBlackboardData);
				return;
			}

			tpLeftTabs.Enabled = false;
			txtBBName.Text = ConfigurationHelper.Blackboard.VirtualModule.Name;
			nudBBPort.Value = ConfigurationHelper.Blackboard.Port;
			//nudBBAutoStop.Value = Blackboard.AutoStop;
			nudTestTimeOut.Value = (int)ConfigurationHelper.Blackboard.TestTimeOut.TotalMilliseconds;
			for (int i = 0; i < ConfigurationHelper.Blackboard.Modules.Count; ++i)
				lstBBModules.Items.Add(ConfigurationHelper.Blackboard.Modules[i]);
			lstBBModules.SelectedIndex = -1;
			tpLeftTabs.Enabled = true;
		}

		private void LoadConfigurationFile()
		{
			if (ofdOpen.ShowDialog() != DialogResult.OK)
				return;
			try
			{
				this.ConfigurationHelper.Blackboard = Blackboard.FromXML(ofdOpen.FileName);
			}
			catch
			{
				this.ConfigurationHelper.Blackboard = null;
			}
			if (ConfigurationHelper.Blackboard != null)
				FillBlackboardData();
		}

		private void NewBlackboard()
		{
			ClearAll();
			ConfigurationHelper.Blackboard = new Blackboard(2300);
			FillBlackboardData();
			SelectModule(null);
		}

		private void NewModule()
		{
			string name;
			int i = 1;
			name = "MODULE";
			while (modules.ContainsKey(name + i.ToString().PadLeft(2, '0')) && (i < 99))
			{
				++i;
			}
			if (modules.ContainsKey(name))
				return;

			ModuleClientTcp module = new ModuleClientTcp(name, IPAddress.Loopback, 2000);
			modules.Add(module.Name, module);
			ConfigurationHelper.Blackboard.Modules.Add(module);
			lstBBModules.Items.Add(module);
			lstBBModules.SelectedItem = module;
		}

		private void ReplacePrototype(ModuleClient module, Prototype oldPrototype, Prototype newPrototype)
		{
			if ((module == null) || (newPrototype == null))
				return;
			if (oldPrototype != null)
			{
				prototypes.Remove(oldPrototype.Command);
				for (int i = 0; i < ConfigurationHelper.Blackboard.Modules.Count; ++i)
					ConfigurationHelper.Blackboard.Modules[i].Prototypes.Remove(oldPrototype);
			}
			prototypes.Add(newPrototype.Command, newPrototype);
			module.Prototypes.Add(newPrototype);
		}

		private void SaveConfigurationFile()
		{
			if (ConfigurationHelper.Blackboard == null)
				return;
			if ((ConfigurationHelper.FilePath == null) || !File.Exists(ConfigurationHelper.FilePath))
			{
				SaveConfigurationFileAs();
				return;
			}
			try
			{
				ConfigurationHelper.SerializeToXml();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error saving file", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void SaveConfigurationFileAs()
		{
			if (sfdSave.ShowDialog(this) != DialogResult.OK)
				return;
			try
			{
				ConfigurationHelper.FilePath = sfdSave.FileName;
				ConfigurationHelper.SerializeToXml();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error saving file", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void SelectModule(ModuleClient selectedModule)
		{
			//if (selectedModule == null)
			//{
			//    gbModule.Enabled = false;
			//    ClearModule();
			//    return;
			//}

			//if (this.selectedModule == selectedModule)
			//    return;
			//this.selectedModule = selectedModule;
			//ClearModule();
			//FillModuleData();
			//txtModuleName.Focus();
			throw new NotImplementedException();
		}

		private void UpdateListBoxItem(ListBox lb, object item)
		{
			int index = lb.Items.IndexOf(item);
			int currentIndex = lb.SelectedIndex;
			lb.BeginUpdate();
			try
			{
				lb.ClearSelected();
				lb.Items[index] = item;
				lb.SelectedIndex = currentIndex;
			}
			finally
			{
				lb.EndUpdate();
			}
		}

		#endregion

		#region Blackboard Edition Control Event Handlers

		private void txtBBName_KeyPress(object sender, KeyPressEventArgs e)
		{
			if ((e.KeyChar >= 'a') && (e.KeyChar <= 'z'))
				unchecked
				{
					e.KeyChar -= 'a';
					e.KeyChar += 'A';
				}
			if (((e.KeyChar < 'A') || (e.KeyChar > 'Z')) && (e.KeyChar != '-') && (e.KeyChar != '\b'))
				e.Handled = true;
		}

		private void txtBBName_Leave(object sender, EventArgs e)
		{
			if (ConfigurationHelper.Blackboard == null)
				return;
			if (txtBBName.Text.Length < 3)
			{
				ConfigurationHelper.Blackboard.VirtualModule.Name = "BLK";
				txtBBName.Text = ConfigurationHelper.Blackboard.VirtualModule.Name;
				return;
			}
			try
			{
				ConfigurationHelper.Blackboard.VirtualModule.Name = txtBBName.Text;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "ConfigUtil", MessageBoxButtons.OK, MessageBoxIcon.Error);
				txtBBName.Focus();
			}
		}

		private void txtBBName_TextChanged(object sender, EventArgs e)
		{
			if ((ConfigurationHelper.Blackboard == null) || (txtBBName.Text.Length < 3))
				return;
			try
			{
				ConfigurationHelper.Blackboard.VirtualModule.Name = txtBBName.Text;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "ConfigUtil", MessageBoxButtons.OK, MessageBoxIcon.Error);
				txtBBName.Focus();
			}
		}

		#endregion

		#region Module Edition Control Event Handlers

		#endregion

		#region Menu and Toolbar Control EventHandlers

		private void newToolStripButton_Click(object sender, EventArgs e)
		{
			NewBlackboard();
		}

		private void lstBBModules_SelectedIndexChanged(object sender, EventArgs e)
		{
			//if (lstBBModules.SelectedIndex == -1)
			//    SelectModule(null);
			//else if (selectedModule == (Module)lstBBModules.SelectedItem)
			//{
			//    if (!gbModule.Enabled)
			//        FillModuleData();
			//    return;
			//}
			//else
			//    SelectModule((Module)lstBBModules.SelectedItem);
			throw new NotImplementedException();
		}

		private void newModuletoolStripButton_Click(object sender, EventArgs e)
		{
			NewModule();
		}

		private void saveToolStripButton_Click(object sender, EventArgs e)
		{
			SaveConfigurationFile();
		}

		private void tsmiSave_Click(object sender, EventArgs e)
		{
			SaveConfigurationFile();
		}

		private void tsmiSaveAs_Click(object sender, EventArgs e)
		{
			SaveConfigurationFileAs();
		}

		private void openToolStripButton_Click(object sender, EventArgs e)
		{
			LoadConfigurationFile();
		}

		private void tsbDeleteModule_Click(object sender, EventArgs e)
		{
			if (selectedModule == null)
				return;
			DeleteModule();
		}

		#endregion

		private void tpLeftTabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			moduleToolBar.Visible = ((tpLeftTabs.SelectedTab != null) && (tpLeftTabs.SelectedTab == tpModules));
		}
	}
}