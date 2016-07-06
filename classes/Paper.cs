using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Timers;
using System.Windows.Forms;

namespace Spire
{
	public class Paper : Control
	{
		public delegate void TextEventHandler(object sender, TextEventArgs e);
		public event TextEventHandler OnTextEvent;
		
		public delegate void NavigationHorizontalEventHandler(object sender, NavigationHorizontalEventArgs e);
		public event NavigationHorizontalEventHandler OnNavigationHorizontalEvent;
		
		public delegate void NavigationVerticalEventHandler(object sender, NavigationVerticalEventArgs e);
		public event NavigationVerticalEventHandler OnNavigationVerticalEvent;
		
		public delegate void EraseEventHandler(object sender, EraseEventArgs e);
		public event EraseEventHandler OnEraseEvent;
	
		private DocumentView documentView;
		private System.Timers.Timer caretTimer;
		private System.Timers.Timer caretMovingTimer;
		private bool caretOn = false;
		private bool caretMoving = false;
		
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
			this.OnNavigationVerticalEvent += new Paper.NavigationVerticalEventHandler(documentView.OnNavigationVerticalEvent);
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
		
		private void EnableCaretMovingTimer()
		{
			caretMoving = true;
			if(caretMovingTimer == null)
			{
				CreateCaretMovingTimer();
			}
			else
			{
				ResetCaretMovingTimer();
			}
		}
		
		private void CreateCaretMovingTimer()
		{
			caretMovingTimer = new System.Timers.Timer(250/*1000=1second*/);
			caretMovingTimer.Elapsed += new ElapsedEventHandler(CaretMovingTimerElapsed);
			caretMovingTimer.Enabled = true;
		}
		
		private void ResetCaretMovingTimer()
		{
			caretMovingTimer.Stop();
			caretMovingTimer.Start();
		}
		
		private void CaretMovingTimerElapsed(object sender, ElapsedEventArgs e)
		{
			caretMoving = false;
			caretMovingTimer.Stop();
			this.Invalidate();
		}
		
		private void PreviewUserKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
					e.IsInputKey = true;
					break;
			}
		}
		
		private void UserKeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Delete:
					RaiseEraseEvent(TextUnit.Character, 1);
					break;
				case Keys.Down:
					RaiseNavigationVerticalEvent(1);
					break;
				case Keys.Left:
					RaiseNavigationHorizontalEvent(TextUnit.Character, -1);
					break;
				case Keys.Right:
					RaiseNavigationHorizontalEvent(TextUnit.Character, 1);
					break;
				case Keys.Up:
					RaiseNavigationVerticalEvent(-1);
					break;
				default:
					return;
			}
			e.Handled = true;
			this.Invalidate();
		}
		
		private void UserKeyPress(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == '\b')
			{
				RaiseEraseEvent(TextUnit.Character, -1);
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
		
		private void RaiseNavigationHorizontalEvent(TextUnit unit, int amount)
		{
			if(OnNavigationHorizontalEvent == null) return;
			EnableCaretMovingTimer();
			OnNavigationHorizontalEvent(this, new NavigationHorizontalEventArgs(unit, amount));
		}
		
		private void RaiseNavigationVerticalEvent(int amount)
		{
			if(OnNavigationVerticalEvent == null) return;
			EnableCaretMovingTimer();
			OnNavigationVerticalEvent(this, new NavigationVerticalEventArgs(amount));
		}
		
		private void RaiseEraseEvent(TextUnit unit, int amount)
		{
			if(OnEraseEvent == null) return;
			OnEraseEvent(this, new EraseEventArgs(unit, amount));
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Bitmap graphicsBuffer = new Bitmap(this.Width, this.Height);
			Graphics graphics = Graphics.FromImage(graphicsBuffer);
			graphics.PageUnit = GraphicsUnit.Pixel;

			if(documentView != null)
			{
				bool drawCaret = (this.Focused && (caretOn || caretMoving));
				documentView.Paint(graphics, drawCaret);
			}

			graphics.Dispose();
			e.Graphics.DrawImageUnscaled(graphicsBuffer, 0, 0);	
		}
		
	}
}
