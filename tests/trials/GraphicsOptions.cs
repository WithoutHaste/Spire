using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Timers;
using System.Windows.Forms;

namespace SpireTest
{
	public class GraphicsOptions : Form
	{
		private Panel scrollPanel;
		private List<Panel> textPanels = new List<Panel>();
		private System.Timers.Timer textTimer;
		private int y = 0;
		public static string DisplayText = "a";
	
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
			StartTimer();

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
		
		private void StartTimer()
		{
			textTimer = new System.Timers.Timer(250/*1000=1second*/);
			textTimer.Elapsed += new ElapsedEventHandler(TextTimerElapsed);
			textTimer.Enabled = true;
		}
		
		private void TextTimerElapsed(object sender, ElapsedEventArgs e)
		{
			if(DisplayText.Length > 45)
			{
				DisplayText = "a";
			}
			else if(DisplayText.Length%2 == 0)
			{
				DisplayText += "f";
			}
			else
			{
				DisplayText += "o";
			}
			foreach(Panel panel in textPanels)
			{
				panel.Invalidate();
			}
		}
		
		private void DisplayOptions()
		{
			List<SmoothingMode> smoothingModes = new List<SmoothingMode>() { SmoothingMode.None, SmoothingMode.AntiAlias, SmoothingMode.HighQuality, SmoothingMode.HighSpeed };
			
			List<TextRenderingHint> textRenderingHints = new List<TextRenderingHint>() { TextRenderingHint.AntiAlias, TextRenderingHint.AntiAliasGridFit, TextRenderingHint.ClearTypeGridFit, TextRenderingHint.SingleBitPerPixel, TextRenderingHint.SingleBitPerPixelGridFit };
			
			//List<StringFormat> stringFormats = new List<StringFormat>() { StringFormat.GenericDefault, StringFormat.GenericTypographic };
			
			foreach(SmoothingMode smoothingMode in smoothingModes)
			{
				foreach(TextRenderingHint textRenderingHint in textRenderingHints)
				{
					DisplayOption(smoothingMode, textRenderingHint, true);
					DisplayOption(smoothingMode, textRenderingHint, false);
				}
			}			
		}
		
		private void DisplayOption(SmoothingMode smoothingMode, TextRenderingHint textRenderingHint, bool defaultStringFormat)
		{
			int height = 75;
		
			Label label = new Label();
			label.Left = 0;
			label.Top = y;
			label.Width = 250;
			label.Height = height;
			label.Text = String.Format("SmoothingMode={0}\nTextRenderingHint={1}\nStringFormat={2}", smoothingMode, textRenderingHint, (defaultStringFormat ? "GenericDefault" : "GenericTypographic"));
			label.TextAlign = ContentAlignment.TopLeft;
			label.Parent = scrollPanel;
			
			PanelWrapper panel = new PanelWrapper() {
				smoothingMode = smoothingMode,
				textRenderingHint = textRenderingHint,
				stringFormat = (defaultStringFormat ? StringFormat.GenericDefault : StringFormat.GenericTypographic)
			};
			panel.Left = label.Width;
			panel.Top = label.Top;
			panel.Width = 400;
			panel.Height = label.Height;
			panel.Parent = scrollPanel;
			textPanels.Add(panel);
			
			y += height;	
		}
	}
	
	public class PanelWrapper : Panel
	{
		public SmoothingMode smoothingMode;
		public TextRenderingHint textRenderingHint;
		public StringFormat stringFormat;

		public PanelWrapper()
		{
		}
		
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = smoothingMode;
			e.Graphics.TextRenderingHint = textRenderingHint;
			e.Graphics.Clear(Color.White);

			Brush brush = new SolidBrush(Color.Black);
			Font font = new Font("Times New Roman", 12);
			e.Graphics.DrawString(GraphicsOptions.DisplayText, font, brush, new Point(5, 5), stringFormat);
		}
	}
}
