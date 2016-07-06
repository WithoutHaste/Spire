using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace SpireTest
{
	public class GraphicsOptions : Form
	{
		private Panel scrollPanel;
		private int y = 0;
	
		public static void Main(string[] args)
		{
			System.Windows.Forms.Application.Run(new GraphicsOptions());
		}
	
		public GraphicsOptions()
		{
			SuspendLayout();
			
			Size = new Size(800,600);
			Text = "Graphics Options";
			
			scrollPanel = BuildScrollPanel();
			scrollPanel.Parent = this;

			DisplayOptions();

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
		
		private void DisplayOptions()
		{
			List<SmoothingMode> smoothingModes = new List<SmoothingMode>() { SmoothingMode.None, SmoothingMode.AntiAlias, SmoothingMode.HighQuality, SmoothingMode.HighSpeed };
			
			List<TextRenderingHint> textRenderingHints = new List<TextRenderingHint>() { TextRenderingHint.AntiAlias, TextRenderingHint.AntiAliasGridFit, TextRenderingHint.ClearTypeGridFit, TextRenderingHint.SingleBitPerPixel, TextRenderingHint.SingleBitPerPixelGridFit };
			
			List<StringFormat> stringFormats = new List<StringFormat>() { StringFormat.GenericDefault, StringFormat.GenericTypographic };
			
			foreach(SmoothingMode smoothingMode in smoothingModes)
			{
				foreach(TextRenderingHint textRenderingHint in textRenderingHints)
				{
					foreach(StringFormat stringFormat in stringFormats)
					{
						DisplayOption(smoothingMode, textRenderingHint, stringFormat);
					}
				}
			}			
		}
		
		private void DisplayOption(SmoothingMode smoothingMode, TextRenderingHint textRenderingHint, StringFormat stringFormat)
		{
			int height = 75;
		
			Label label = new Label();
			label.Left = 0;
			label.Top = y;
			label.Width = 200;
			label.Height = height;
			label.Text = String.Format("SmoothingMode={0}\nTextRenderingHint={1}\nStringFormat={2}", smoothingMode, textRenderingHint, (stringFormat == StringFormat.GenericDefault ? "GenericDefault" : "GenericTypographic"));
			label.TextAlign = ContentAlignment.TopLeft;
			label.Parent = scrollPanel;
			
			y += height;	
		}
	}
}