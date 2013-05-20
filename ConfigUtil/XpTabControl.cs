using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

public class XpTabControl : TabControl
{

	public XpTabControl()
	{
		this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
			ControlStyles.OptimizedDoubleBuffer, Application.RenderWithVisualStyles && TabRenderer.IsSupported);
		this.UpdateStyles();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		ReDrawTabs(e.Graphics, e.ClipRectangle);
	}

	private void ReDrawTabs(Graphics graphics, Rectangle cliprectangle)
	{
		if (!this.Visible) return;

		// Draw tab page
		TabRenderer.DrawTabPage(graphics, this.ClientRectangle);

		// If there is no tabs to draw, return
		if (this.TabCount == 0)
			return;

		// Check tab alignment
		switch (this.Alignment)
		{
			case TabAlignment.Bottom:
				graphics.RotateTransform(180);
				break;

			case TabAlignment.Left:
				graphics.RotateTransform(90);
				break;

			case TabAlignment.Right:
				graphics.RotateTransform(-90);
				break;

			case TabAlignment.Top:
			default:
				break;
		}

		// Draw tabs
		Rectangle tabRectangle;
		VisualStyleRenderer renderer = new VisualStyleRenderer(VisualStyleElement.Tab.TabItem.Normal);
		//TabItemState state;
		for (int i = 0; i < this.TabCount; ++i )
		{
			tabRectangle = this.GetTabRect(i);
			
			if (i == this.SelectedIndex)
			{
				renderer.SetParameters(VisualStyleElement.Tab.TabItem.Hot);
				//state = TabItemState.Selected;
			}
			else
			{
				renderer.SetParameters(VisualStyleElement.Tab.TabItem.Normal);
				//if(this.TabPages[i].Focused)
				//	state = TabItemState.Hot;
				//else
				//	state = TabItemState.Normal;
			}
			renderer.DrawBackground(graphics, tabRectangle);
			renderer.DrawText(graphics, tabRectangle, this.TabPages[i].Text);
			
			//graphics.FillRectangle(Brushes.Red, tabRectangle);
			//TabRenderer.DrawTabItem(graphics, this.ClientRectangle, state);
		}
	}
}