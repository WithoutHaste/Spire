using System;
//using System.Collections.Generic;
//using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
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
		
		public delegate void NavigationEventHandler(object sender, NavigationEventArgs e);
		public event NavigationEventHandler OnNavigationEvent;
		
		public delegate void EraseEventHandler(object sender, EraseEventArgs e);
		public event EraseEventHandler OnEraseEvent;
	
		private DocumentView documentView;
		private System.Timers.Timer caretTimer;
		private bool caretOn = false;
		
		public Paper()
		{
			SetupDoubleBuffer();
			this.GotFocus += new EventHandler(EnableCaretTimer);
			this.LostFocus += new EventHandler(DisableCaretTimer);
			this.PreviewKeyDown += new PreviewKeyDownEventHandler(PreviewUserKeyDown);
			this.KeyPress += new KeyPressEventHandler(UserKeyPress);
			this.KeyDown += new KeyEventHandler(UserKeyDown);
		}
		
		public void SetView(DocumentView view)
		{
			documentView = view;
		}
				
		private void SetupDoubleBuffer()
		{
			this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			this.UpdateStyles();
		}
		
		private void EnableCaretTimer(object sender, EventArgs e)
		{
			caretOn = true;
			caretTimer = new System.Timers.Timer(350/*1000=1second*/);
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
		
		private void PreviewUserKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Left:
				case Keys.Right:
					e.IsInputKey = true;
					break;
			}
		}
		
		private void UserKeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Delete:
					RaiseEraseEvent(EraseEventArgs.Units.Character, 1);
					break;
				case Keys.Left:
					RaiseNavigationEvent(NavigationEventArgs.Units.Character, -1);
					break;
				case Keys.Right:
					RaiseNavigationEvent(NavigationEventArgs.Units.Character, 1);
					break;
			}
		}
		
		private void UserKeyPress(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == '\b')
			{
				RaiseEraseEvent(EraseEventArgs.Units.Character, -1);
			}
			else
			{
				RaiseTextEvent(e.KeyChar);
			}
			e.Handled = true;
			this.Invalidate();
			//Console.WriteLine("key press: {0}", e.KeyChar);
		}
		
		private void RaiseTextEvent(char text)
		{
			if(OnTextEvent == null) return;
			OnTextEvent(this, new TextEventArgs(text));
		}
		
		private void RaiseNavigationEvent(NavigationEventArgs.Units unit, int amount)
		{
			if(OnNavigationEvent == null) return;
			OnNavigationEvent(this, new NavigationEventArgs(unit, amount));
		}
		
		private void RaiseEraseEvent(EraseEventArgs.Units unit, int amount)
		{
			if(OnEraseEvent == null) return;
			OnEraseEvent(this, new EraseEventArgs(unit, amount));
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Bitmap graphicsBuffer = new Bitmap(this.Width, this.Height);
			Graphics graphics = Graphics.FromImage(graphicsBuffer);
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

			if(documentView != null)
			{
				bool drawCaret = (this.Focused && caretOn);
				documentView.Paint(graphics, drawCaret);
			}

			graphics.Dispose();
			e.Graphics.DrawImageUnscaled(graphicsBuffer, 0, 0);	
		}
		
	}
}