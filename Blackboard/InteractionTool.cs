using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Blk.Api;
using Blk.Engine;

namespace Blk.Gui
{
	public partial class InteractionTool : UserControl
	{
		#region Variables

		/// <summary>
		/// Stores the autocomplete lists for injection/send textbox of each module
		/// </summary>
		private SortedList<string, List<string>> stmAutoCompleteList;

		private Blackboard blackboard;

		private ModuleClient selectedModule;

		#endregion

		#region Constructor

		public InteractionTool()
		{
			InitializeComponent();
			stmAutoCompleteList = new SortedList<string, List<string>>();
		}

		#endregion

		#region Properties

		public ModuleClient SelectedModule
		{
			get { return this.selectedModule; }
			set
			{
				if (this.selectedModule == value)
				{
					Actualize();
					return;
				}

				if ((this.selectedModule = value) == null)
					return;

				lblLastSendToModuleResult.Text = "";
				Actualize();
				
			}
		}

		public Blackboard Blackboard
		{
			get { return this.blackboard; }
			set
			{
				if(this.blackboard == value)
					return;

				stmAutoCompleteList.Clear();
				if((this.blackboard = value) == null)
					return;
				FillAutocompleteList();
			}
		}

		/// <summary>
		/// Gets the autocomplete lists for injection/send textbox of each module
		/// </summary>
		public SortedList<string, List<string>> AutoCompleteList
		{
			get { return this.stmAutoCompleteList; }
		}

		#endregion

		#region Methods

		public void Actualize()
		{

			txtSendToModule.AutoCompleteCustomSource.Clear();
			if (this.selectedModule == null)
			{
				gbModuleInteraction.Enabled = false;
				txtSendToModule.Enabled = false;
				btnSendToModule.Enabled = false;
				return;
			}
			
			gbModuleInteraction.Enabled = selectedModule.IsConnected;
			txtSendToModule.Enabled = selectedModule.IsConnected;
			btnSendToModule.Enabled = selectedModule.IsConnected;
			string[] acs= new string[0];
			if (this.AutoCompleteList.ContainsKey(selectedModule.Name))
			{
				string suggestion;
				List<string> acl = this.AutoCompleteList[selectedModule.Name];
				IPrototype[] aproto = selectedModule.Prototypes.ToArray();
				foreach (IPrototype p in aproto)
				{
					suggestion = p.Command;
					if (p.ParamsRequired) suggestion += " \"params\"";
					suggestion += " @1";
					if (!acl.Contains(suggestion))
						acl.Add(suggestion);
				}
				acs = acl.ToArray();
			}
			txtSendToModule.AutoCompleteCustomSource.AddRange(acs);
			txtSendToModule.Clear();
			txtSendToModule.Focus();
		}

		private void AddInputToACSL()
		{
			if (!txtSendToModule.AutoCompleteCustomSource.Contains(txtSendToModule.Text))
				txtSendToModule.AutoCompleteCustomSource.Add(txtSendToModule.Text);
			if (!stmAutoCompleteList[SelectedModule.Name].Contains(txtSendToModule.Text))
				stmAutoCompleteList[SelectedModule.Name].Add(txtSendToModule.Text);
			txtSendToModule.Clear();
		}

		private bool InjectUserMessage(ModuleClient mc, string message)
		{
			return blackboard.Inject(mc.Name + " " + message);
		}

		private void FillAutocompleteList()
		{
			if (this.blackboard == null)
				return;

			foreach (IModuleClient im in blackboard.Modules)
			{
				string suggestion;
				List<string> moduleAutoCompleteList = new List<string>(im.Prototypes.Count);
				stmAutoCompleteList.Add(im.Name, moduleAutoCompleteList);
				foreach (IPrototype p in im.Prototypes)
				{
					suggestion = p.Command;
					if (p.ParamsRequired) suggestion += " \"params\"";
					suggestion += " @1";
					moduleAutoCompleteList.Add(suggestion);
				}
			}
		}

		private void InjectUserMessage()
		{
			if (SelectedModule == null) return;
			try
			{
				if (!InjectUserMessage(SelectedModule, txtSendToModule.Text))
				{
					lblLastSendToModuleResult.Text = "Injection Failed";
					return;
				}
				AddInputToACSL();
				lblLastSendToModuleResult.Text = "Injection Succeded!";
			}
			catch
			{
				lblLastSendToModuleResult.Text = "Injection Failed";
			}
		}

		private bool IsValidInput(string s)
		{
			Command cmd;
			Response rsp;

			if (this.selectedModule == null)
				return false;

			if (Response.TryParse(s, selectedModule, out rsp) || Command.TryParse(s, selectedModule, out cmd))
				return true;
			return false;
		}

		private void Log(string s)
		{
			if (this.blackboard == null)
				return;
			this.blackboard.Log.WriteLine(s);
		}

		private bool SendUserMessage(ModuleClient mc, string message)
		{
			Command c;
			Response r;
			bool success;
			if (Command.TryParse(message, mc, out c))
			{
				success = mc.Send(c);
				Log("# Send user command [" + message + "] " + (success ? "success!" : "failed"));
				lblLastSendToModuleResult.Text = "Send user command: " + (success ? "success!" : "failed");
				return true;
			}

			if (Response.TryParse(message, mc, out r))
			{
				success = mc.Send(r);
				Log("# Send user response [" + message + "] " + (success ? "success!" : "failed"));
				lblLastSendToModuleResult.Text = "Send user response: " + (success ? "success!" : "failed");
				return true;
			}
			return false;
		}

		private void SendUserMessage()
		{
			if (SelectedModule == null) return;
			try
			{
				if (!SendUserMessage(SelectedModule, txtSendToModule.Text))
				{
					lblLastSendToModuleResult.Text = "Operation Failed";
					return;
				}
				AddInputToACSL();
				txtSendToModule.Clear();
			}
			catch
			{
				lblLastSendToModuleResult.Text = "Operation Failed";
			}
		}

		private void UpdateAutoCompleteList()
		{
			if ((selectedModule == null) || !selectedModule.IsConnected)
				return;
			List<string> moduleAutoCompleteList = stmAutoCompleteList[selectedModule.Name];
			for (int i = 0; i < moduleAutoCompleteList.Count; ++i)
				txtSendToModule.AutoCompleteCustomSource.Add(moduleAutoCompleteList[i]);
		}

		#endregion

		#region Event handlers

		private void btnSendToModule_Click(object sender, EventArgs e)
		{
			SendUserMessage();
		}

		private void btnModuleInject_Click(object sender, EventArgs e)
		{
			InjectUserMessage();
		}

		private void txtSendToModule_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Return:
					InjectUserMessage();
					//SendUserMessage();
					break;
			}
		}

		private void txtSendToModule_TextChanged(object sender, EventArgs e)
		{
			bool isValid;
			if (String.IsNullOrEmpty(txtSendToModule.Text))
			{
				txtSendToModule.BackColor = SystemColors.Window;
				return;
			}

			isValid = IsValidInput(txtSendToModule.Text);
			txtSendToModule.BackColor = isValid ? Color.FromArgb(210, 255, 210) : Color.FromArgb(255, 210, 210);
			//btnModuleInject.Enabled =isValid;
			//btnSendToModule.Enabled = isValid;


		}

		#endregion
	}
}
