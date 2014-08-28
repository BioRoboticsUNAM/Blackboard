using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Blk.Engine;
using Robotics;

namespace ConfigUtil
{
	public partial class ModuleView : UserControl
	{
		private VoidEventHandler dlgFillModuleData;
		private VoidEventHandler dlgClearModule;
		private SortedList<string, Prototype> prototypes;
		private ModuleClient selectedModule;
		private Prototype selectedPrototype;

		public ModuleView()
		{
			dlgFillModuleData = new VoidEventHandler(FillModuleData);
			dlgClearModule = new VoidEventHandler(ClearModule);
			InitializeComponent();

			prototypes = new SortedList<string, Prototype>();
			selectedModule = null;
			FillModuleData();
		}

		public ModuleClient Module
		{
			get { return selectedModule; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				bool update = (selectedModule == value);
				selectedModule = value;
				if (update) FillModuleData();
			}
		}

		private void ChangeModuleName()
		{
			//string modulePreviousName;

			//if (selectedModule == null)
			//    return;

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

			throw new NotImplementedException();

			//if (modules.ContainsKey(selectedModule.Name))
			//{
			//    MessageBox.Show(ex.Message, "ConfigUtil", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    txtModuleName.Focus();
			//    return;
			//}
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
			txtModuleName.Text = "";
			txtModuleAddress.Text = "";
			nudModulePort.Value = nudModulePort.Minimum;
			chkModuleEnabled.Checked = false;
			chkModuleCheckAlive.Checked = false;
			chkModuleRequirePrefix.Checked = false;
			chkModuleSimulate.Checked = false;
			dgvModuleCommands.Rows.Clear();
			dgvModuleCommands.Visible = false;
		}

		private void DeletePrototype(Prototype proto)
		{
			//if (proto == null)
			//    return;

			//prototypes.Remove(proto.CommandName);
			//for (int i = 0; i < Blackboard.Modules.Count; ++i)
			//    Blackboard.Modules[i].Commands.Remove(proto);
			throw new NotImplementedException();
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
			this.Enabled = false;
			txtModuleName.Text = selectedModule.Name;
			if (selectedModule is ModuleClientTcp)
			{
				txtModuleAddress.Text = ((ModuleClientTcp)selectedModule).ServerAddresses.ToString();
				nudModulePort.Value = ((ModuleClientTcp)selectedModule).Port;
			}
			chkModuleEnabled.Checked = selectedModule.Enabled;
			chkModuleCheckAlive.Checked = selectedModule.AliveCheck;
			chkModuleRequirePrefix.Checked = selectedModule.RequirePrefix;
			chkModuleSimulate.Checked = selectedModule.Simulation.SimulationEnabled;
			dgvModuleCommands.Rows.Clear();

			foreach (Blk.Api.IPrototype proto in selectedModule.Prototypes)
			{
				//DataGridViewRow r = dgvModuleCommands.co;
				//r.Cells["colCommandName"].Value = selectedModule.Prototypes[i].Command;
				//r.Cells["colPriority"].Value = selectedModule.Prototypes[i].HasPriority;
				//r.Cells["colAnswer"].Value = selectedModule.Prototypes[i].ResponseRequired;
				//r.Cells["colParameters"].Value = selectedModule.Prototypes[i].ParamsRequired;
				//r.Cells["colTimeout"].Value = selectedModule.Prototypes[i].Timeout;

				int n = dgvModuleCommands.Rows.Add();
				DataGridViewRow r = dgvModuleCommands.Rows[n];

				r.Cells[0].Value = proto.Command;
				r.Cells[3].Value = proto.HasPriority;
				r.Cells[2].Value = proto.ResponseRequired;
				r.Cells[1].Value = proto.ParamsRequired;
				r.Cells[4].Value = proto.Timeout;

				r.Tag = proto;
			}
			dgvModuleCommands.Visible = true;
			this.Enabled = true;
		}

		private void ReplacePrototype(ModuleClient module, Prototype oldPrototype, Prototype newPrototype)
		{
			//if ((module == null) || (newPrototype == null))
			//    return;
			//if (oldPrototype != null)
			//{
			//    prototypes.Remove(oldPrototype.CommandName);
			//    for (int i = 0; i < Blackboard.Modules.Count; ++i)
			//        Blackboard.Modules[i].Commands.Remove(oldPrototype);
			//}
			//prototypes.Add(newPrototype.CommandName, newPrototype);
			//module.Commands.Add(newPrototype);
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

		#region Module Edition Control Event Handlers

		private void txtModuleName_KeyPress(object sender, KeyPressEventArgs e)
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

		private void txtModuleName_Leave(object sender, EventArgs e)
		{
			if (selectedModule == null)
				return;
			ChangeModuleName();
		}

		private void txtModuleName_KeyDown(object sender, KeyEventArgs e)
		{
			if (selectedModule == null)
				return;
			switch (e.KeyCode)
			{
				case Keys.Enter:
					ChangeModuleName();
					if (selectedModule.Name == txtModuleName.Text)
					{
						e.SuppressKeyPress = true;
						txtModuleAddress.Focus();
					}
					break;

				case Keys.Escape:
					txtModuleName.Text = selectedModule.Name;
					break;
			}

		}

		private void txtModuleAddress_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (((e.KeyChar < '0') || (e.KeyChar > '9')) && (e.KeyChar != '.') && (e.KeyChar != '\b'))
				e.Handled = true;
			else if (e.KeyChar == '.')
			{
				int i = 0;
				int count = 0;
				string s = txtModuleAddress.Text;
				for (i = 0; i < s.Length; ++i)
					if (s[i] == '.') ++count;
				if (count >= 3)
					e.Handled = true;
			}
			else if ((e.KeyChar >= '0') && (e.KeyChar <= '9'))
			{
				string s = txtModuleAddress.Text.Substring(0, txtModuleAddress.SelectionStart);
				int dotPos = s.LastIndexOf('.');
				if ((s.Length - dotPos) > 3)
					e.Handled = true;
			}

		}

		private void txtModuleAddress_Leave(object sender, EventArgs e)
		{
			//if (selectedModule == null)
			//    return;
			//if (txtModuleAddress.Text.Length < 7)
			//{
			//    selectedModule.Address = IPAddress.Parse("127.0.0.1");
			//    txtModuleAddress.Text = selectedModule.Address.ToString();
			//    return;
			//}
			//try
			//{
			//    selectedModule.Address = IPAddress.Parse(txtModuleAddress.Text);
			//    UpdateListBoxItem(lstBBModules, selectedModule);
			//}
			//catch (Exception ex)
			//{
			//    MessageBox.Show(ex.Message, "ConfigUtil", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    txtModuleAddress.Focus();
			//}
			throw new NotImplementedException();
		}

		private void nudModulePort_ValueChanged(object sender, EventArgs e)
		{
			//if (selectedModule == null)
			//    return;
			//try
			//{
			//    selectedModule.Port = (int)nudModulePort.Value;
			//    UpdateListBoxItem(lstBBModules, selectedModule);
			//}
			//catch (Exception ex)
			//{
			//    MessageBox.Show(ex.Message, "ConfigUtil", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    nudModulePort.Focus();
			//}
			throw new NotImplementedException();
		}

		private void chkModuleEnabled_CheckedChanged(object sender, EventArgs e)
		{
			if (selectedModule == null)
				return;
			selectedModule.Enabled = chkModuleEnabled.Checked;
		}

		private void chkModuleCheckAlive_CheckedChanged(object sender, EventArgs e)
		{
			if (selectedModule == null)
				return;
			selectedModule.AliveCheck = chkModuleCheckAlive.Checked;
		}

		private void chkModuleRequirePrefix_CheckedChanged(object sender, EventArgs e)
		{
			if (selectedModule == null)
				return;
			selectedModule.RequirePrefix = chkModuleRequirePrefix.Checked;
		}

		private void chkModuleSimulate_CheckedChanged(object sender, EventArgs e)
		{
			if (selectedModule == null)
				return;
			selectedModule.Simulation.SuccessRatio = chkModuleSimulate.Checked ? 1 : 2;
		}

		#endregion

		#region Command Edition Control EventHandlers

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

		private void dgvModuleCommands_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
		{
			Prototype proto = e.Row.Tag as Prototype;
			if (proto == null)
				return;
			DeletePrototype(proto);

		}

		private void dgvModuleCommands_RowEnter(object sender, DataGridViewCellEventArgs e)
		{

		}

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

		private void dgvModuleCommands_CellValidated(object sender, DataGridViewCellEventArgs e)
		{
			Prototype proto;
			Prototype oldProto = dgvModuleCommands.Rows[e.RowIndex].Tag as Prototype;

			string commandName = (string)dgvModuleCommands.Rows[e.RowIndex].Cells["colCommandName"].FormattedValue;
			if (!Prototype.RxCommandName.IsMatch(commandName))
			{
				if (oldProto != null)
					DeletePrototype(oldProto);
				return;
			}
			bool responseRequired = (bool)dgvModuleCommands.Rows[e.RowIndex].Cells["colAnswer"].FormattedValue;
			bool paramsRequired = (bool)dgvModuleCommands.Rows[e.RowIndex].Cells["colParameters"].FormattedValue;
			bool hasPriority = (bool)dgvModuleCommands.Rows[e.RowIndex].Cells["colPriority"].FormattedValue;
			int timeout;
			if (!Int32.TryParse(dgvModuleCommands.Rows[e.RowIndex].Cells["colTimeout"].FormattedValue.ToString(), out timeout))
				timeout = 500;

			proto = new Prototype(commandName, paramsRequired, responseRequired, timeout, hasPriority);
			selectedModule.Prototypes.Add(proto);

			ReplacePrototype(selectedModule, oldProto, proto);
			dgvModuleCommands.Rows[e.RowIndex].Tag = proto;
			selectedPrototype = proto;
		}

		#endregion
	}
}
