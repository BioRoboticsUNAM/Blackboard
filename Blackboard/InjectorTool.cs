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
using IModule = Blk.Api.IModule;
using Module = Blk.Engine.ModuleClient;
using Command = Blk.Engine.Command;
using Response = Blk.Engine.Response;

namespace Blk.Gui
{
	public partial class InjectorTool : UserControl
	{
		private Blackboard blackboard;
		private List<Prototype> prototypes;
		private char[] separators = { '\r', '\n'};

		public InjectorTool()
		{
			InitializeComponent();
			prototypes = new List<Prototype>();
		}

		public Blackboard Blackboard
		{
			get { return this.blackboard; }
			set
			{
				//if ((this.blackboard = value) == null)
				//	throw new ArgumentNullException();
				this.blackboard = value;

				prototypes.Clear();
				cbModules.Items.Clear();

				// Load modules, prototypes and populate autocomplete list
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
						txtMessage.AutoCompleteCustomSource.Add(proto.Command + (proto.ParamsRequired ? " \"\"" : String.Empty) + " @0");
					}
				}
			}
		}

		public bool BulkMode
		{
			get { return chkBulk.Checked; }
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
				if (!lstHistory.Items.Contains(token))
					lstHistory.Items.Add(token);
				if (!txtMessage.AutoCompleteCustomSource.Contains(token.StringValue))
					txtMessage.AutoCompleteCustomSource.Add(token.StringValue);
			}
			txtMessage.Clear();

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

			if (String.IsNullOrEmpty(txtMessage.Text))
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
