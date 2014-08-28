using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Robotics;
using Blk.Engine;
using Blk.Engine.SharedVariables;
using Robotics.API;

using SharedVariable = Blk.Engine.SharedVariables.SharedVariable;
using Module = Blk.Engine.ModuleClient;
using Blk.Api;

namespace ConfigUtil
{
	public partial class FrmConfigUtil : Form
	{
		#region Variables

		private ConfigurationHelper configurationHelper;
		private VoidEventHandler dlgClearAll;
		private VoidEventHandler dlgFillBlackboardData;
		private VoidEventHandler dlgFillModuleData;
		private VoidEventHandler dlgFillVariableData;
		private VoidEventHandler dlgClearModule;
		private VoidEventHandler dlgClearVariable;
		private SortedList<string, Module> modules;
		private SortedList<string, Prototype> prototypes;
		private Module selectedModule;
		private SharedVariable selectedVar;
		//private Prototype selectedPrototype;

		#endregion

		#region Constructor

		public FrmConfigUtil()
		{
			InitializeComponent();
			dlgClearAll = new VoidEventHandler(ClearAll);
			dlgFillBlackboardData = new VoidEventHandler(FillBlackboardData);
			dlgFillModuleData = new VoidEventHandler(FillModuleData);
			dlgFillVariableData = new VoidEventHandler(FillVariableData);
			dlgClearModule = new VoidEventHandler(ClearModule);
			dlgClearVariable = new VoidEventHandler(ClearVariable);

			modules = new SortedList<string, Module>();
			prototypes = new SortedList<string, Prototype>();
			this.configurationHelper = new ConfigurationHelper();
			ConfigurationHelper.Blackboard = null;
		}

		#endregion

		#region Properties

		public ConfigurationHelper ConfigurationHelper
		{
			get { return this.configurationHelper; }
			set {
				if (value == null)
					ClearAll();
				this.configurationHelper = value;
				
				tpLeftTabs.Enabled = (this.configurationHelper != null);
				tsmiSaveAs.Enabled = (this.configurationHelper != null);
				tsmiSave.Enabled = (this.configurationHelper != null);
				saveToolStripButton.Enabled = (this.configurationHelper != null);
				tsbNewModule.Enabled = (this.configurationHelper != null);
				
				tsbDeleteModule.Enabled = false;
			}
		}

		#endregion

		#region Methods

		private Prototype AddPrototype(Module module, string commandName, bool responseRequired, bool paramsRequired, bool hasPriority, int timeout)
		{
			if(module == null)
			return null;
			Prototype proto = new Prototype(commandName, paramsRequired, responseRequired, timeout, hasPriority);
			module.Prototypes.Add(proto);
			return proto;
		}

		private void ChangeModuleName()
		{
			//string modulePreviousName;

			if (selectedModule == null)
				return;

			//if (txtModuleName.Text.Length < 3)
			//{
			//    selectedModule.Name = "MODULE";
			//    txtModuleName.Text = selectedModule.Name;
			//    return;
			//}

			//if (selectedModule.Name == txtModuleName.Text)
			//    return;
			//try
			//{
			//    modulePreviousName = selectedModule.Name;
			//    selectedModule.Name = txtModuleName.Text;
			//    modules.Remove(modulePreviousName);
			//    modules.Add(selectedModule.Name, selectedModule);
			//    UpdateListBoxItem(lstBBModules, selectedModule);
			//}
			//catch (Exception ex)
			//{
			//    MessageBox.Show(ex.Message, "ConfigUtil", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    txtModuleName.Focus();
			//    return;
			//}
			//if (modules.ContainsKey(selectedModule.Name))
			//{
			//    MessageBox.Show(ex.Message, "ConfigUtil", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    txtModuleName.Focus();
			//    return;
			//}
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
			//txtBBName.Text = "";
			//nudBBPort.Value = nudBBPort.Minimum;
			//nudBBAutoStop.Value = nudBBAutoStop.Minimum;
			//nudTestTimeOut.Value = nudTestTimeOut.Minimum;
			modules.Clear();
			prototypes.Clear();
			lstBBModules.Items.Clear();
			ClearModule();
		}

		private void ClearModule()
		{
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.IsDisposed || this.Disposing)
					return;
				this.BeginInvoke(dlgClearModule);
				return;
			}
			pgModuleProperties.Enabled = false;
			pgModuleProperties.SelectedObject = null;
			dgvModuleCommands.Enabled = false;
			dgvModuleCommands.Visible = false;
			dgvModuleCommands.ClearSelection();
			dgvModuleCommands.Rows.Clear();
			
		}

		private void ClearVariable()
		{
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.IsDisposed || this.Disposing)
					return;
				this.BeginInvoke(dlgClearVariable);
				return;
			}
			pgVariableProperties.SelectedObject = null;
			dgvSharedVariables.Rows.Clear();
			dgvSharedVariables.Visible = false;
		}

		private void DeleteModule()
		{
			if (selectedModule == null)
				return;
			Module module = selectedModule;
			selectedModule = null;
			dgvModuleCommands.Enabled = false;
			ClearModule();
			if (modules.ContainsKey(module.Name))
				modules.Remove(module.Name);
			if (ConfigurationHelper.Blackboard.Modules.Contains(module))
				ConfigurationHelper.Blackboard.Modules.Remove(module);
			if (lstBBModules.Items.Contains(module))
				lstBBModules.Items.Remove(module);
			foreach (IPrototype proto in module.Prototypes)
			{
				if (prototypes.ContainsKey(proto.Command))
					prototypes.Remove(proto.Command);
			}
		}

		private void DeletePrototype(Prototype proto)
		{
			if (proto == null)
				return;

			prototypes.Remove(proto.Command);
			//for (int i = 0; i < Blackboard.Modules.Count; ++i)
			//	Blackboard.Modules[i].Commands.Remove(proto);
			selectedModule.Prototypes.Remove(proto);
		}

		private void DeleteWriter(string writer)
		{
			if (writer == null)
				return;

			selectedVar.AllowedWriters.Remove(writer);
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
			pgBlackboardProperties.Enabled = true;
			pgBlackboardProperties.SelectedObject = new BlackboardWrapper( ConfigurationHelper.Blackboard);

			//txtBBName.Text = Blackboard.Name;
			//nudBBPort.Value = Blackboard.Port;
			////nudBBAutoStop.Value = Blackboard.AutoStop;
			//nudTestTimeOut.Value = Blackboard.TestTimeOut;
			for (int i = 0; i < ConfigurationHelper.Blackboard.Modules.Count; ++i)
				lstBBModules.Items.Add(ConfigurationHelper.Blackboard.Modules[i]);
			foreach(SharedVariable sv in ConfigurationHelper.Blackboard.VirtualModule.SharedVariables)
				lstBBVars.Items.Add(sv);
			lstBBModules.SelectedIndex = -1;
			lstBBVars.SelectedIndex = -1;
			tpLeftTabs.Enabled = true;
		}

		private void FillModuleData()
		{
			if (selectedModule == null)
				return;
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.IsDisposed || this.Disposing)
					return;
				this.BeginInvoke(dlgFillModuleData);
				return;
			}
			dgvModuleCommands.Visible = (selectedModule != null);
			gbModule.Text = "Module Configuration: " + ((selectedModule != null) ? selectedModule.Name : "none");
			pgModuleProperties.Enabled = false;
			pgModuleProperties.SelectedObject = new ModuleWrapper(selectedModule);
			pgModuleProperties.Enabled = true;

			dgvModuleCommands.Visible = false;
			dgvModuleCommands.Enabled = false;
			dgvModuleCommands.Rows.Clear();
			//selectedModule.Prototypes.Sort();

			foreach (IPrototype proto in selectedModule.Prototypes)
			{
				int n = dgvModuleCommands.Rows.Add();
				DataGridViewRow r = dgvModuleCommands.Rows[n];

				r.Cells["colCommandName"].Value = proto.Command;
				r.Cells["colPriority"].Value = proto.HasPriority;
				r.Cells["colAnswer"].Value = proto.ResponseRequired;
				r.Cells["colParameters"].Value = proto.ParamsRequired;
				r.Cells["colTimeout"].Value = proto.Timeout;

				r.Tag = proto;
			}
			dgvModuleCommands.Visible = true;
			dgvModuleCommands.Enabled = true;
		}

		private void FillVariableData()
		{
			if (selectedVar == null)
				return;
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.IsDisposed || this.Disposing)
					return;
				this.BeginInvoke(dlgFillVariableData);
				return;
			}
			dgvSharedVariables.Visible = (selectedVar != null);
			gbSharedVariables.Text = "Shared Variable Configuration: " + ((selectedVar != null) ? selectedVar.Name : "none");
			pgVariableProperties.Enabled = false;
			pgVariableProperties.SelectedObject = selectedVar;
			pgVariableProperties.Enabled = true;
			dgvSharedVariables.Rows.Clear();
			selectedVar.AllowedWriters.Sort();

			for (int i = 0; i < selectedVar.AllowedWriters.Count; ++i)
			{
				int n = dgvSharedVariables.Rows.Add();
				DataGridViewRow r = dgvSharedVariables.Rows[n];

				r.Cells[0].Value = selectedVar.AllowedWriters[i];
				r.Tag = selectedVar.AllowedWriters[i];
			}
			dgvModuleCommands.Visible = true;
		}

		private void LoadConfigurationFile()
		{
			pgBlackboardProperties.Enabled = false;
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

		private void ReplacePrototype(Module module, Prototype oldPrototype, Prototype newPrototype)
		{
			if ((module == null)||(newPrototype == null))
				return;
			if (oldPrototype != null)
			{
				prototypes.Remove(oldPrototype.Command);
				for (int i = 0; i < ConfigurationHelper.Blackboard.Modules.Count; ++i)
					ConfigurationHelper.Blackboard.Modules[i].Prototypes.Remove(oldPrototype);
			}
			//prototypes.Add(newPrototype.CommandName, newPrototype);
			
		}

		private void ReplaceWriter(SharedVariable var, string oldWriter, string newWriter)
		{
			if ((var == null) || (newWriter == null))
				return;
			if (oldWriter != null)
			{
				var.AllowedWriters.Remove(oldWriter);
			}
			var.AllowedWriters.Remove(newWriter);
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
			catch(Exception ex) 
			{
				MessageBox.Show(ex.Message, "Error saving file", MessageBoxButtons.OK, MessageBoxIcon.Error); 
			}
		}

		private void SaveConfigurationFileAs()
		{
			if(sfdSave.ShowDialog(this) != DialogResult.OK)
				return;
			try
			{
				ConfigurationHelper.FilePath = sfdSave.FileName;
				ConfigurationHelper.SerializeToXml();
			}
			catch(Exception ex) 
			{
				MessageBox.Show(ex.Message, "Error saving file", MessageBoxButtons.OK, MessageBoxIcon.Error); 
			}
		}

		private void SelectModule(Module selectedModule)
		{
			if (selectedModule == null)
			{
				dgvModuleCommands.Enabled = false;
				pgModuleProperties.Enabled = false;
				//ClearModule();
				return;
			}
			
			if (this.selectedModule == selectedModule)
				return;
			this.selectedModule = selectedModule;
			pgModuleProperties.Enabled = true;
			ClearModule();
			FillModuleData();
			pgModuleProperties.Focus();
			dgvModuleCommands.Sort(dgvModuleCommands.Columns["colCommandName"], ListSortDirection.Ascending);
		}

		private void SelectVariable(SharedVariable sharedVar)
		{
			if (sharedVar == null)
			{
				dgvSharedVariables.Enabled = false;
				pgVariableProperties.Enabled = false;
				return;
			}

			if (this.selectedVar == sharedVar)
				return;
			this.selectedVar = sharedVar;
			pgVariableProperties.Enabled = true;
			ClearVariable();
			FillVariableData();
			pgVariableProperties.Focus();
		}

		private void ShowConfiguration()
		{
			gbBlackboard.Visible = (tpLeftTabs.SelectedIndex == 0);
			gbBlackboard.Dock = gbBlackboard.Visible ? DockStyle.Fill : DockStyle.None;
			gbModule.Visible = (tpLeftTabs.SelectedIndex == 1);
			gbModule.Dock = gbModule.Visible ? DockStyle.Fill : DockStyle.None;
			gbSharedVariables.Visible = (tpLeftTabs.SelectedIndex == 2);
			
			gbSharedVariables.Dock = gbSharedVariables.Visible ? DockStyle.Fill : DockStyle.None;

			dgvSharedVariables.Visible = (selectedVar != null);
			dgvModuleCommands.Visible = (selectedModule != null);
			
			gbModule.Text = "Module Configuration: " + ((selectedModule != null) ? selectedModule.Name : "none");
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

		private void ValidateCommandRows()
		{
			
		}

		private void ValidateWriters()
		{

		}

		private void ValidateWriterRows()
		{
			bool allModulesCanWrite;
			string moduleName;
			if (selectedVar == null)
				return;

			allModulesCanWrite = selectedVar.AllowedWriters.Contains("*");
			foreach (DataGridViewRow row in dgvSharedVariables.Rows)
			{
				row.ErrorText = "";
				moduleName =row.Cells["colModuleWriter"].FormattedValue.ToString();
				if ((moduleName.Length == 0) || (moduleName == "*"))
					continue;
				if (allModulesCanWrite)
					row.ErrorText = "Duplicated writer module name";
				if (!ConfigurationHelper.Blackboard.Modules.Contains(moduleName))
				{
					if (row.ErrorText.Length != 0)
						row.ErrorText += ". ";
					row.ErrorText+= "Module does not exist";
				}
			}
		}

		#endregion

		#region Command Edition Control EventHandlers

		private void dgvModuleCommands_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			string headerText = dgvModuleCommands.Columns[e.ColumnIndex].HeaderText;

			// If cell is in the Command Name column.
			if (headerText == "Command Name")
			{
				string commandName = e.FormattedValue.ToString();
				dgvModuleCommands.Rows[e.RowIndex].ErrorText = "";

				if (!Prototype.RxCommandName.IsMatch(commandName))
				{
					if (dgvModuleCommands.Rows[e.RowIndex].IsNewRow)
					{
						dgvModuleCommands.CurrentCell.Value = "";
						return;
					}
					dgvModuleCommands.Rows[e.RowIndex].ErrorText = "Invalid command name";
					e.Cancel = true;
				}
			}
			// If cell is in the Timeout column.
			else if (headerText == "Timeout")
			{
				dgvModuleCommands.Rows[e.RowIndex].ErrorText = "";
				string sTimeout = e.FormattedValue.ToString();
				int timeOut;
				if (!Int32.TryParse(sTimeout, out timeOut) || (timeOut < 0))
				{
					if (dgvModuleCommands.Rows[e.RowIndex].IsNewRow)
					{
						dgvModuleCommands.CurrentCell.Value = "";
						return;
					}
					dgvModuleCommands.Rows[e.RowIndex].ErrorText = "Invalid timeout";

					e.Cancel = true;
				}
			}
		}

		private void dgvModuleCommands_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			int index = dgvModuleCommands.CurrentCell.ColumnIndex;
			if ((index == 0) || (index == 4))
			{
				DataGridViewTextBoxEditingControl tbec = e.Control as DataGridViewTextBoxEditingControl;

				if (tbec != null)
				{
					tbec.CharacterCasing = CharacterCasing.Lower;
					tbec.KeyPress += new KeyPressEventHandler(dgvModuleCommands_KeyPress);
				}
			}

			//e.Control.KeyPress += new KeyPressEventHandler(dataGridViewTextBox_KeyPress);
		}

		private void dgvModuleCommands_KeyPress(object sender, KeyPressEventArgs e)
		{
			switch (dgvModuleCommands.CurrentCell.ColumnIndex)
			{
				case 0:
					//if (!((e.KeyChar >= 'a') && (e.KeyChar <= 'z')) && !((e.KeyChar >= '0') && (e.KeyChar <= '9')) && (e.KeyChar != '_') && (e.KeyChar != '\b'))
					if (!((e.KeyChar >= 'a') && (e.KeyChar <= 'z')) && (e.KeyChar != '_') && (e.KeyChar != '\b'))
						e.Handled = true;
					break;

				case 4:
					if (!((e.KeyChar >= '0') && (e.KeyChar <= '9')) && (e.KeyChar != '\b'))
						e.Handled = true;
					break;

				default:
					break;
			}
		}

		private void dgvModuleCommands_RowEnter(object sender, DataGridViewCellEventArgs e)
		{

		}

		private void dgvModuleCommands_RowValidated(object sender, DataGridViewCellEventArgs e)
		{
			if ((e.RowIndex == -1) || (e.RowIndex >= dgvModuleCommands.Rows.Count))
				return;
			Prototype proto = dgvModuleCommands.Rows[e.RowIndex].Tag as Prototype;

			string commandName = (string)dgvModuleCommands.Rows[e.RowIndex].Cells["colCommandName"].FormattedValue;
			bool responseRequired = (bool)dgvModuleCommands.Rows[e.RowIndex].Cells["colAnswer"].FormattedValue;
			bool paramsRequired = (bool)dgvModuleCommands.Rows[e.RowIndex].Cells["colParameters"].FormattedValue;
			bool hasPriority = (bool)dgvModuleCommands.Rows[e.RowIndex].Cells["colPriority"].FormattedValue;
			int timeout;
			if (!Int32.TryParse(dgvModuleCommands.Rows[e.RowIndex].Cells["colTimeout"].FormattedValue.ToString(), out timeout))
			{
				timeout = 500;
				dgvModuleCommands.Rows[e.RowIndex].Cells["colTimeout"].Value = 500;
			}
			// If it is a new prototype, add it
			if (proto == null)
			{
				if (!Validator.IsValidCommandName(commandName))
					return;
				dgvModuleCommands.Rows[e.RowIndex].Tag = AddPrototype(selectedModule, commandName, responseRequired, paramsRequired, hasPriority, timeout);
				dgvModuleCommands.Sort(dgvModuleCommands.Columns["colCommandName"], ListSortDirection.Ascending);
				return;
			}

			// If it is the same prototype do nothing
			if (
				(proto.Command == commandName) &&
				(proto.HasPriority == hasPriority) &&
				(proto.ParamsRequired == paramsRequired) &&
				(proto.ResponseRequired == responseRequired) &&
				(proto.Timeout == timeout))
				return;
			
			// Else, replace prototype
			proto = new Prototype(commandName, paramsRequired, responseRequired, timeout, hasPriority);
			dgvModuleCommands.Rows[e.RowIndex].Tag = proto;
			dgvModuleCommands.Sort(dgvModuleCommands.Columns["colCommandName"], ListSortDirection.Ascending);

			//if (!Prototype.RxCommandName.IsMatch(commandName))
			//{
			//    if (oldProto != null)
			//        DeletePrototype(oldProto);
			//    return;
			//}
			
			//proto = new Prototype();
			//proto.module = selectedModule;
			//proto.CommandName = commandName;
			//proto.ParamsRequired = paramsRequired;
			//proto.ResponseRequired = responseRequired;
			//proto.Timeout = timeout;
			//proto.HasPriority = hasPriority;

			//if (Blackboard.SupportsCommand(commandName))
			//    dgvModuleCommands.Rows[e.RowIndex].ErrorText = "Duplicated command";
			//ReplacePrototype(selectedModule, oldProto, proto);
			//dgvModuleCommands.Rows[e.RowIndex].Tag = proto;
			//selectedPrototype = proto;
		}

		private void dgvModuleCommands_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
		{
			Prototype proto = e.Row.Tag as Prototype;
			if (proto == null)
				return;
			DeletePrototype(proto);
		}

		#endregion

		#region Writer Edition Control EventHandlers

		private void dgvSharedVariables_CellValidated(object sender, DataGridViewCellEventArgs e)
		{
			string oldWriter = dgvSharedVariables.Rows[e.RowIndex].Tag as string;

			string moduleWriter = (string)dgvSharedVariables.Rows[e.RowIndex].Cells["colModuleWriter"].FormattedValue;
			if (!Validator.IsValidModuleName(moduleWriter) && (moduleWriter != "*"))
			{
				if (oldWriter != null)
					DeleteWriter(oldWriter);
				return;
			}

			ReplaceWriter(selectedVar, oldWriter, moduleWriter);
			dgvSharedVariables.Rows[e.RowIndex].Tag = moduleWriter;
			ValidateWriterRows();
		}

		private void dgvSharedVariables_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			string moduleWriter = e.FormattedValue.ToString();
			dgvSharedVariables.Rows[e.RowIndex].ErrorText = "";

			if (!Validator.IsValidModuleName(moduleWriter) && (moduleWriter != "*"))
			{
				if (dgvSharedVariables.Rows[e.RowIndex].IsNewRow)
				{
					dgvSharedVariables.CurrentCell.Value = "";
					return;
				}
				dgvSharedVariables.Rows[e.RowIndex].ErrorText = "Invalid writer module name";
				e.Cancel = true;
			}
		}

		private void dgvSharedVariables_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			DataGridViewTextBoxEditingControl tbec = e.Control as DataGridViewTextBoxEditingControl;
			if (tbec != null)
			{
				tbec.CharacterCasing = CharacterCasing.Upper;
				tbec.KeyPress += new KeyPressEventHandler(dgvSharedVariables_KeyPress);
			}
		}

		private void dgvSharedVariables_KeyPress(object sender, KeyPressEventArgs e)
		{
			DataGridViewTextBoxEditingControl tbec = sender as DataGridViewTextBoxEditingControl;
			if (tbec == null) return;
			if (e.KeyChar == '\b')
				return;
			if (tbec.Text == "*")
				e.Handled = true;
			else if ((tbec.Text.Length == 0) && (e.KeyChar != '*') && ((e.KeyChar < 'A') || (e.KeyChar > 'Z')) && ((e.KeyChar < 'a') || (e.KeyChar > 'z')))
				e.Handled = true;
			else if ((tbec.Text.Length > 0) && (e.KeyChar != '-') && ((e.KeyChar < 'A') || (e.KeyChar > 'Z')) && ((e.KeyChar < 'a') || (e.KeyChar > 'z')) && ((e.KeyChar < '0') || (e.KeyChar > '9')))
				e.Handled = true;
		}

		private void dgvSharedVariables_RowEnter(object sender, DataGridViewCellEventArgs e)
		{

		}

		private void dgvSharedVariables_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
		{
			string writer = e.Row.Tag as string;
			if (writer == null)
				return;
			DeleteWriter(writer);
		}

		#endregion

		#region Menu and Toolbar Control EventHandlers

		private void newToolStripButton_Click(object sender, EventArgs e)
		{
			NewBlackboard();
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

		}

		#endregion

		private void lstBBModules_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstBBModules.SelectedIndex == -1)
				SelectModule(null);
			else if (selectedModule == (Module)lstBBModules.SelectedItem)
			{
				FillModuleData();
			}
			else
				SelectModule((Module)lstBBModules.SelectedItem);
		}

		private void lstBBVars_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstBBVars.SelectedIndex == -1)
				SelectVariable(null);
			else if (selectedVar == (SharedVariable)lstBBVars.SelectedItem)
			{
				FillVariableData();
			}
			else
				SelectVariable((SharedVariable)lstBBVars.SelectedItem);
		}

		private void tpBBConfig_Click(object sender, EventArgs e)
		{

		}

		private void tpLeftTabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			ShowConfiguration();
		}

		private void FrmConfigUtil_Load(object sender, EventArgs e)
		{
			ShowConfiguration();
		}
	}
}