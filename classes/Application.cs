using System;
//using System.Collections.Generic;
//using System.Diagnostics;
using System.Drawing;
//using System.Drawing.Drawing2D;
using System.IO;
//using System.Text;
using System.Windows.Forms;

namespace Spire
{
	public class Application : Form
	{
		private Panel scrollPanel;
		private Paper documentPanel;
		private string iconPath = Path.Combine("images", "SpireIcon1.ico");
		
		public Application()
		{
			SuspendLayout();
			
			Size = new Size(800,600);
			Text = "Spire";
			if(File.Exists(iconPath))
			{
				Icon = new Icon(iconPath);
			}
			
			scrollPanel = BuildScrollPanel();
			scrollPanel.Parent = this;

			documentPanel = BuildDocumentPanel();
			documentPanel.Anchor = AnchorStyles.Top;
			documentPanel.Parent = scrollPanel;

			documentPanel.Invalidate();
			
			ResumeLayout(false);
		}

		private Panel BuildScrollPanel()
		{
			Panel panel = new Panel();
			panel.Width = this.ClientSize.Width;
			panel.Height = this.ClientSize.Height;
			panel.AutoScroll = true;
			return panel;
		}

		private Paper BuildDocumentPanel()
		{
			Paper panel = new Paper();
			panel.AutoScroll = true;
			panel.Size = new Size(600, 800);
			panel.Left = 30;
			panel.Top = 20;
			panel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
			panel.BackColor = Color.White;
			return panel;
		}

	}
}
