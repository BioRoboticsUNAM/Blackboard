using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Blk.Gui
{
	public partial class FrmHumanCheckList : Form
	{
		public FrmHumanCheckList()
		{
			InitializeComponent();
			this.btnCancel.Enabled = true;
			this.btnOK.Enabled = false;
			this.DialogResult = DialogResult.Cancel;
			this.cklItemList.CheckOnClick = true;
		}

		private void cklItemList_SelectedValueChanged(object sender, EventArgs e)
		{
			if (this.cklItemList.CheckedIndices.Count == this.cklItemList.Items.Count)
			{
				this.btnOK.Enabled = true;
				this.DialogResult = DialogResult.OK;
				this.Hide();
			}
			else
			{
				this.btnOK.Enabled = false;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Hide();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Hide();
		}

		private void FrmHumanCheckList_Shown(object sender, EventArgs e)
		{
			if (cklItemList.Items.Count < 1)
			{
				this.DialogResult = DialogResult.OK;
				this.Hide();
			}
		}

		public int LoadList(string listPath)
		{
			try
			{
				if (!File.Exists(listPath))
					return 0;

				string[] items = File.ReadAllLines(listPath);
				foreach (string item in items)
				{
					if ((item == null) || (item.Trim().Length < 1))
						continue;
					this.cklItemList.Items.Add(item, false);
				}
				return this.cklItemList.Items.Count;
			}
			catch
			{
				this.cklItemList.Items.Clear();
				return 0;
			}
		}
	}
}
