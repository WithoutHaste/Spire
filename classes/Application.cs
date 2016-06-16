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
		private DocumentModel documentModel;
		private Panel scrollPanel;
		private Paper paper;
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

			paper = BuildPaper();
			paper.Anchor = AnchorStyles.Top;
			paper.Parent = scrollPanel;
			paper.Focus();
			
			documentModel = new DocumentModel();
			paper.OnTextEvent += new Paper.TextEventHandler(documentModel.OnTextEvent);

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

		private Paper BuildPaper()
		{
			Paper paper = new Paper();
			paper.Size = new Size(600, 800);
			paper.Left = 30;
			paper.Top = 20;
			paper.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
			paper.BackColor = Color.White;
			return paper;
		}

	}
}
