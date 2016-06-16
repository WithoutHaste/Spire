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
		public delegate void TextEventHandler(object sender, TextEventArgs e);
		public event TextEventHandler OnTextEvent;
	
		private System.Timers.Timer caretTimer;
		private bool caretOn = false;
		
		public Paper()
		{
			SetupDoubleBuffer();
			this.GotFocus += new EventHandler(EnableCaretTimer);
			this.LostFocus += new EventHandler(DisableCaretTimer);
			this.KeyPress += new KeyPressEventHandler(UserKeyPress);
		}
		
		private void SetupDoubleBuffer()
		{
			this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			this.UpdateStyles();
		}
		
		private void EnableCaretTimer(object sender, EventArgs e)
		{
			caretTimer = new System.Timers.Timer(450/*1000=1second*/);
			caretTimer.Elapsed += new ElapsedEventHandler(CaretTimerElapsed);
			caretTimer.Enabled = true;
		}
		
		private void DisableCaretTimer(object sender, EventArgs e)
		{
			if(caretTimer == null) return;
			caretTimer.Enabled = false;
		}
		
		private void CaretTimerElapsed(object sender, ElapsedEventArgs e)
		{
			caretOn = !caretOn;
			this.Invalidate();
		}
		
		private void UserKeyPress(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar >= 'a' && e.KeyChar <= 'z')
			{
				RaiseTextEvent(e.KeyChar);
				e.Handled = true;
			}
			else if(e.KeyChar >= 'A' && e.KeyChar <= 'Z')
			{
				RaiseTextEvent(e.KeyChar);
				e.Handled = true;
			}
			else if(e.KeyChar >= '0' && e.KeyChar <= '9')
			{
				RaiseTextEvent(e.KeyChar);
				e.Handled = true;
			}
			Console.WriteLine("Key Press: "+e.KeyChar+" = "+(int)e.KeyChar);
		}
		
		private void RaiseTextEvent(char text)
		{
			if(OnTextEvent == null) return;
			OnTextEvent(this, new TextEventArgs(text));
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
			DrawCaret(graphics);
		}
		
		private void DrawCaret(Graphics graphics)
		{
			if(!this.Focused) return;
			if(!caretOn) return;
			Pen pen = new Pen(Color.LightBlue, 0.75f);
			graphics.DrawLine(pen, 10, 20, 10, 30);
		}
	}
}