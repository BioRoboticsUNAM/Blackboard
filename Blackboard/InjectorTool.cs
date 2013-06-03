using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Robotics.API;
using Blk.Api;
using Blk.Engine;
using IModule = Blk.Api.IModuleClient;
using Module = Blk.Engine.ModuleClient;
using Command = Blk.Engine.Command;
using Response = Blk.Engine.Response;

namespace Blk.Gui
{
	public partial class InjectorTool : UserControl
	{
		private const string InputTip = "Type command/response here";
		private Blackboard blackboard;
		private List<Prototype> prototypes;
		private List<HistoryToken> history;
		private char[] separators = { '\r', '\n'};

		public InjectorTool()
		{
			InitializeComponent();
			prototypes = new List<Prototype>();
			history = new List<HistoryToken>();
			this.cmbHystoryFilter.Sorted = true;
			this.cmbHystoryFilter.Items.Add("(none)");
			this.cmbHystoryFilter.AutoCompleteSource = AutoCompleteSource.CustomSource;
			DisplayInputTip();
		}

		public Blackboard Blackboard
		{
			get { return this.blackboard; }
			set
			{
				//if ((this.blackboard = value) == null)
				//	throw new ArgumentNullException();
				this.blackboard = value;
				string quickCommand;

				prototypes.Clear();
				cbModules.Items.Clear();
				cmbQuickCommand.Items.Clear();

				// Load modules, prototypes and populate autocomplete list
				this.Enabled = blackboard != null;
				if (blackboard == null)
					return;
				foreach (Module module in blackboard.Modules)
				{
					cbModules.Items.Add(module);
					if (module == blackboard.VirtualModule)
						cbModules.SelectedItem = module;

					foreach (Prototype proto in module.Prototypes)
					{
						prototypes.Add(proto);
						quickCommand = proto.Command + (proto.ParamsRequired ? " \"\"" : String.Empty) + " @0";
						txtMessage.AutoCompleteCustomSource.Add(quickCommand);
						cmbQuickCommand.Items.Add(quickCommand);
						AddFilter(proto.Command);
					}
				}
			}
		}

		public bool BulkMode
		{
			get { return chkBulk.Checked; }
		}

		private void AddHystoryToken(HistoryToken token)
		{
			if(!history.Contains(token))
				history.Add(token);
			//if (!lstHistory.Items.Contains(token))
			//    lstHistory.Items.Add(token);
			if (!txtMessage.AutoCompleteCustomSource.Contains(token.StringValue))
				txtMessage.AutoCompleteCustomSource.Add(token.StringValue);
			Filter();
		}

		private void AddFilter(string filterString)
		{
			if (cmbHystoryFilter.Items.Contains(filterString))
				return;
			cmbHystoryFilter.Items.Add(filterString);
			cmbHystoryFilter.AutoCompleteCustomSource.Add(filterString);
		}

		private void AddQuickCommand()
		{
			
			if (cmbQuickCommand.SelectedIndex == -1)
				return;
			txtMessage.AppendText( cmbQuickCommand.SelectedItem.ToString());
			int ix = txtMessage.Text.LastIndexOf('"');
			if (ix == -1)
				return;
			txtMessage.Focus();
			txtMessage.SelectionStart = ix;
			txtMessage.SelectionLength = 0;
		}

		private void ClearInputTip()
		{
			if (String.Compare(txtMessage.Text, InputTip, true) != 0)
				return;
			txtMessage.ForeColor = Control.DefaultForeColor;
			txtMessage.Clear();
		}

		private void DisplayInputTip()
		{
			if (!String.IsNullOrEmpty(txtMessage.Text))
				return;
			this.txtMessage.Text = InputTip;
			this.txtMessage.ForeColor = Color.LightGray;
		}

		private void Inject()
		{
			Module source;
			Command cmd;
			Response rsp;
			string[] inputs;
			HistoryToken token;

			if ((source = (Module)cbModules.SelectedItem) == null)
				return;

			inputs = BulkMode ?
				txtMessage.Text.Split(separators, StringSplitOptions.RemoveEmptyEntries) :
				new string[] { txtMessage.Text.Trim() };
			
			foreach (string input in inputs)
			{
				//blackboard.Inject(source.Name + " " + input);
				token = null;
				if (Response.IsResponse(input) && Response.TryParse(input, source, out rsp))
				{
					token = new HistoryToken(rsp);	
				}
				else if (Command.IsCommand(input) && Command.TryParse(input, source, out cmd))
				{
					token = new HistoryToken(cmd);
				}
				if (token == null)
				{
					blackboard.Inject(source.Name + " " + input);
					continue;
				}
				
				blackboard.Inject(token.StringToInject);
				AddHystoryToken(token);
			}
			txtMessage.Clear();

		}

		private void Filter()
		{
			if ((cmbHystoryFilter.SelectedIndex == 0) && (cmbHystoryFilter.Text == (string)cmbHystoryFilter.Items[0]))
			{
				Filter(null);
				return;
			}

			if (!String.IsNullOrEmpty(cmbHystoryFilter.Text) && !cmbHystoryFilter.Items.Contains(cmbHystoryFilter.Text))
				AddFilter(cmbHystoryFilter.Text);
			Filter(cmbHystoryFilter.Text);
		}

		private void Filter(string filterString)
		{
			lstHistory.SuspendLayout();
			lstHistory.Items.Clear();

			for (int i = 0; i < history.Count; ++i)
			{
				if(String.IsNullOrEmpty(filterString) || history[i].StringValue.Contains(filterString))
					lstHistory.Items.Add(history[i]);
			}
			lstHistory.ResumeLayout();
		}

		private bool IsValidInput(Module source, string s)
		{
			Command cmd;
			Response rsp;

			if ((Response.IsResponse(s) && Response.TryParse(s, source, out rsp)) ||
			Command.IsCommand(s) && Command.TryParse(s, source, out cmd))
				return true;
			return false;
		}

		private void txtMessage_TextChanged(object sender, EventArgs e)
		{
			Module source;
			string[] inputs;

			if (String.IsNullOrEmpty(txtMessage.Text) || (String.Compare(txtMessage.Text, InputTip, true) == 0))
			{
				txtMessage.BackColor = SystemColors.Window;
				return;
			}

			inputs = BulkMode ?
				txtMessage.Text.Split(separators, StringSplitOptions.RemoveEmptyEntries) :
				new string[] { txtMessage.Text.Trim() };

			if ((source = (Module)cbModules.SelectedItem) == null)
				return;
		
			foreach (string input in inputs)
			{
				if (!IsValidInput(source, input))
				{
					btnInject.Enabled = false;
					txtMessage.BackColor = Color.FromArgb(255, 210, 210);
					return;
				}
			}
			btnInject.Enabled = true;
			//txtMessage.BackColor = SystemColors.Window;
			txtMessage.BackColor = Color.FromArgb(210, 255, 210);
		}

		private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
		{
			if ((e.KeyChar == 0x0D) && !BulkMode)
			{
				e.Handled = true;
				btnInject.PerformClick();
			}
		}

		private void btnInject_Click(object sender, EventArgs e)
		{
			btnInject.Enabled = false;
			Inject();
			btnInject.Enabled = true;
		}

		private void lstHistory_DoubleClick(object sender, EventArgs e)
		{
			if (lstHistory.SelectedIndex == -1)
				return;
			blackboard.Inject(lstHistory.SelectedItem.ToString());
		}

		private void miCut_Click(object sender, EventArgs e)
		{
			if (contextMenu.SourceControl != txtMessage)
				return;
			txtMessage.Cut();
		}

		private void miCopy_Click(object sender, EventArgs e)
		{
			if (contextMenu.SourceControl == txtMessage)
			{
				txtMessage.Copy();
			}
			else if (contextMenu.SourceControl == lstHistory)
			{
				if (lstHistory.SelectedIndex == -1)
					return;
				Clipboard.SetText(((HistoryToken)lstHistory.SelectedItem).StringValue);
			}
			else
				return;
			
		}

		private void miDelete_Click(object sender, EventArgs e)
		{
			if (contextMenu.SourceControl == txtMessage)
			{
				txtMessage.SelectedText = String.Empty;
			}
			else if (contextMenu.SourceControl == lstHistory)
			{
				if (lstHistory.SelectedIndex == -1)
					return;
				lstHistory.Items.RemoveAt(lstHistory.SelectedIndex);
			}
			else
				return;
		}

		private void miPaste_Click(object sender, EventArgs e)
		{
			if (contextMenu.SourceControl != txtMessage)
				return;
			txtMessage.Paste();
		}

		private void miSendToMessage_Click(object sender, EventArgs e)
		{
			if ((contextMenu.SourceControl != lstHistory) ||(lstHistory.SelectedIndex == -1))
				return;
			if (!String.IsNullOrEmpty(txtMessage.Text))
				txtMessage.AppendText("\r\n");
			txtMessage.AppendText(((HistoryToken)lstHistory.SelectedItem).StringValue);
		}

		private void miSelectAll_Click(object sender, EventArgs e)
		{
			if (contextMenu.SourceControl != txtMessage)
				return;
			txtMessage.SelectAll();
		}

		private void miClear_Click(object sender, EventArgs e)
		{
			if (contextMenu.SourceControl != txtMessage)
				return;
			txtMessage.Clear();
		}

		private void miUndo_Click(object sender, EventArgs e)
		{
			if (contextMenu.SourceControl != txtMessage)
				return;
			txtMessage.Undo();
		}

		private void miRedo_Click(object sender, EventArgs e)
		{
			if (contextMenu.SourceControl != txtMessage)
				return;
			
		}

		private void contextMenu_Opening(object sender, CancelEventArgs e)
		{
			if (contextMenu.SourceControl == txtMessage)
			{
				miUndo.Visible =true;
				miUndo.Enabled =txtMessage.CanUndo;
				miRedo.Visible =true;
				miRedo.Enabled = false;
				miSeparator1.Visible = true;
				miCut.Visible = true;
				miCut.Enabled = (txtMessage.SelectionLength > 0);
				miCopy.Visible = true;
				miCopy.Enabled = (txtMessage.SelectionLength > 0);
				miPaste.Visible = true;
				miSeparator2.Visible = true;
				miDelete.Visible = true;
				miDelete.Enabled = (txtMessage.SelectionLength > 0);
				miClear.Visible = true;
				miClear.Enabled = (txtMessage.Text.Length > 0);
				miSelectAll.Visible = true;
				miSendToMessage.Visible = false;
			}
			else if (contextMenu.SourceControl == lstHistory)
			{
				miUndo.Visible = false;
				miRedo.Visible = false;
				miSeparator1.Visible = false;
				miCut.Visible = false;
				miCopy.Visible = true;
				miCopy.Enabled = true;
				miPaste.Visible = false;
				miDelete.Visible = true;
				miDelete.Enabled = true;
				miSeparator2.Visible = true;
				miClear.Visible = false;
				miSelectAll.Visible = false;
				miSendToMessage.Visible = true;
			}
			else
			{
				e.Cancel = true;
				return;
			}
		}

		private void cmbHystoryFilter_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
				Filter();
		}

		private void btnHystoryFilter_Click(object sender, EventArgs e)
		{
			Filter();
		}

		private void cmbHystoryFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			Filter();
		}

		private void btnQuickCommand_Click(object sender, EventArgs e)
		{
			AddQuickCommand();
		}

		private void cmbQuickCommand_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cmbQuickCommand.SelectedIndex == -1)
				return;
			AddQuickCommand();
		}

		private void txtMessage_Enter(object sender, EventArgs e)
		{
			ClearInputTip();
		}

		private void txtMessage_Leave(object sender, EventArgs e)
		{
			DisplayInputTip();
		}

		protected class HistoryToken :  IComparable<HistoryToken>
		{
			private readonly CommandBase message;

			public HistoryToken(CommandBase message)
			{
				this.message = message;
			}

			public CommandBase Message { get { return message; } }

			public IModule Source
			{
				get { return message.Source; }
			}

			public string StringValue
			{
				get { return message.StringToSend; }
			}

			public string StringToInject
			{
				get { return message.Source.Name + " " + message.StringToSend; }
			}

			public override string ToString()
			{
				return message.ToString();
			}

			#region IComparable<HistoryToken> Members

			public int CompareTo(HistoryToken other)
			{
				if (other == null)
					return -1;
				return String.Compare(this.StringValue, other.StringValue, true);
			}

			#endregion
		}

	}
}
