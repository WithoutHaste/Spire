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
		public static Font GlobalFont = new Font("Times New Roman", 12);
		public static int DocumentWidth = 600;
	
		private DocumentModel documentModel;
		private DocumentView documentView;
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
			paper.OnNavigationHorizontalEvent += new Paper.NavigationHorizontalEventHandler(documentModel.OnNavigationHorizontalEvent);
			paper.OnEraseEvent += new Paper.EraseEventHandler(documentModel.OnEraseEvent);
			paper.OnUndoEvent += new EventHandler(documentModel.OnUndoEvent);
			
			documentView = new DocumentView(documentModel);
			documentView.AppendDisplayArea(new DisplayArea(paper.Width, paper.Height));
			documentModel.OnUpdateAtEvent += new DocumentModel.UpdateAtEventHandler(documentView.OnModelUpdateEvent);
			paper.SetView(documentView);

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
