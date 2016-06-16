using System;
//using System.Collections.Generic;
//using System.Diagnostics;
using System.Drawing;
//using System.Drawing.Drawing2D;
using System.IO;
//using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace Spire
{
	public class Paper : Control
	{
		private System.Timers.Timer caretTimer;
		private bool caretOn = false;
		
		public Paper()
		{
			SetupDoubleBuffer();
			this.GotFocus += new EventHandler(PaperGotFocus);
			this.LostFocus += new EventHandler(PaperLostFocus);
		}
		
		private void SetupDoubleBuffer()
		{
			// Set the value of the double-buffering style bits to true.
			this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			this.UpdateStyles();
		}
		
		private void PaperGotFocus(object sender, EventArgs e)
		{
			caretTimer = new System.Timers.Timer(450/*1000=1second*/);
			caretTimer.Elapsed += new ElapsedEventHandler(CaretTimerElapsed);
			caretTimer.Enabled = true;
		}
		
		private void PaperLostFocus(object sender, EventArgs e)
		{
			if(caretTimer != null)
			{
				caretTimer.Enabled = false;
			}
		}
		
		private void CaretTimerElapsed(object sender, ElapsedEventArgs e)
		{
			caretOn = !caretOn;
			this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Bitmap graphicsBuffer = new Bitmap(this.Width, this.Height);
			Graphics graphics = Graphics.FromImage(graphicsBuffer);
			//graphics.SmoothingMode = SmoothingMode.AntiAlias;

			OnPaint(graphics);

			graphics.Dispose();
			e.Graphics.DrawImageUnscaled(graphicsBuffer, 0, 0);	
		}
		
		private void OnPaint(Graphics graphics)
		{
			graphics.Clear(Color.White);
			
			if(this.Focused && caretOn)
			{
				DrawCaret(graphics);
			}
		}
		
		private void DrawCaret(Graphics graphics)
		{
			Pen pen = new Pen(Color.Black, 0.75f);
			graphics.DrawLine(pen, 10, 20, 10, 30);
		}
	}
}