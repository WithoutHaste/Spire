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
		
		public delegate void CaretNavigationHorizontalEventHandler(object sender, NavigationHorizontalEventArgs e);
		public event CaretNavigationHorizontalEventHandler OnCaretNavigationHorizontalEvent;
		
		public delegate void CaretNavigationVerticalEventHandler(object sender, NavigationVerticalEventArgs e);
		public event CaretNavigationVerticalEventHandler OnCaretNavigationVerticalEvent;
		
		public delegate void HighlightNavigationHorizontalEventHandler(object sender, NavigationHorizontalEventArgs e);
		public event HighlightNavigationHorizontalEventHandler OnHighlightNavigationHorizontalEvent;
		
		public delegate void HighlightNavigationVerticalEventHandler(object sender, NavigationVerticalEventArgs e);
		public event HighlightNavigationVerticalEventHandler OnHighlightNavigationVerticalEvent;
		
		public delegate void CaretNavigationPointEventHandler(object sender, NavigationPointEventArgs e);
		public event CaretNavigationPointEventHandler OnCaretNavigationPointEvent;
		
		public delegate void HighlightNavigationPointEventHandler(object sender, NavigationPointEventArgs e);
		public event HighlightNavigationPointEventHandler OnHighlightNavigationPointEvent;
		
		public delegate void EraseEventHandler(object sender, EraseEventArgs e);
		public event EraseEventHandler OnEraseEvent;

		public event EventHandler OnUndoEvent;
		public event EventHandler OnRedoEvent;
		public event EventHandler OnCaretNavigationHomeEvent;
		public event EventHandler OnCaretNavigationEndEvent;
		public event EventHandler OnHighlightNavigationHomeEvent;
		public event EventHandler OnHighlightNavigationEndEvent;
		public event EventHandler OnCopyEvent;
		public event EventHandler OnCutEvent;
		public event EventHandler OnPasteEvent;
	
		private int pageNumber;
		private DocumentView documentView;
		private System.Timers.Timer caretTimer;
		private System.Timers.Timer caretMovingTimer;
		private bool caretOn = false;
		private bool caretMoving = false;
		
		public Paper(int pageNumber)
		{
			this.pageNumber = pageNumber;
			SetupDoubleBuffer();
			this.Cursor = Cursors.IBeam;
			this.GotFocus += new EventHandler(EnableCaretTimer);
			this.LostFocus += new EventHandler(DisableCaretTimer);
			this.PreviewKeyDown += new PreviewKeyDownEventHandler(PreviewUserKeyDown);
			this.KeyPress += new KeyPressEventHandler(UserKeyPress);
			this.KeyDown += new KeyEventHandler(UserKeyDown);
			this.MouseClick += new MouseEventHandler(UserMouseClick);
			this.MouseDown += new MouseEventHandler(UserMouseDown);
			this.MouseUp += new MouseEventHandler(UserMouseUp);
		}
		
		public void SetModel(DocumentModel documentModel)
		{
			OnTextEvent += new TextEventHandler(documentModel.OnTextEvent);
			OnCaretNavigationHorizontalEvent += new CaretNavigationHorizontalEventHandler(documentModel.OnCaretNavigationHorizontalEvent);
			OnHighlightNavigationHorizontalEvent += new HighlightNavigationHorizontalEventHandler(documentModel.OnHighlightNavigationHorizontalEvent);
			OnEraseEvent += new EraseEventHandler(documentModel.OnEraseEvent);
			OnUndoEvent += new EventHandler(documentModel.OnUndoEvent);
			OnRedoEvent += new EventHandler(documentModel.OnRedoEvent);
			OnCopyEvent += new EventHandler(documentModel.OnCopyEvent);
			OnCutEvent += new EventHandler(documentModel.OnCutEvent);
			OnPasteEvent += new EventHandler(documentModel.OnPasteEvent);
		}
		
		public void SetView(DocumentView view)
		{
			documentView = view;
			documentView.AppendDisplayArea(new DisplayArea(0, 0, this.Width, this.Height, pageNumber));
			OnCaretNavigationVerticalEvent += new CaretNavigationVerticalEventHandler(documentView.OnCaretNavigationVerticalEvent);
			OnHighlightNavigationVerticalEvent += new HighlightNavigationVerticalEventHandler(documentView.OnHighlightNavigationVerticalEvent);
			OnCaretNavigationPointEvent += new CaretNavigationPointEventHandler(documentView.OnCaretNavigationPointEvent);
			OnHighlightNavigationPointEvent += new HighlightNavigationPointEventHandler(documentView.OnHighlightNavigationPointEvent);
			OnCaretNavigationHomeEvent += new EventHandler(documentView.OnCaretNavigationHomeEvent);
			OnCaretNavigationEndEvent += new EventHandler(documentView.OnCaretNavigationEndEvent);
			OnHighlightNavigationHomeEvent += new EventHandler(documentView.OnHighlightNavigationHomeEvent);
			OnHighlightNavigationEndEvent += new EventHandler(documentView.OnHighlightNavigationEndEvent);
		}
				
		private void SetupDoubleBuffer()
		{
			this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			this.UpdateStyles();
		}
		
		private void SetupDragAndDrop()
		{
			this.MouseMove += new MouseEventHandler(UserMouseMove);
		}
		
		private void TeardownDragAndDrop()
		{
			this.MouseMove -= UserMouseMove;
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
				case Keys.Tab:
				case Keys.Up:
					e.IsInputKey = true;
					break;
			}
		}
		
		private void UserKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Control)
			{
				UserControlKeyDown(e);
				return;
			}
			if(e.Shift)
			{
				UserShiftKeyDown(e);
				return;
			}
			switch(e.KeyCode)
			{
				case Keys.Delete:
					RaiseEraseEvent(TextUnit.Character, 1);
					break;
				case Keys.Down:
					RaiseCaretNavigationVerticalEvent(VerticalDirection.Down);
					break;
				case Keys.End:
					RaiseCaretNavigationEndEvent();
					break;
				case Keys.Home:
					RaiseCaretNavigationHomeEvent();
					break;
				case Keys.Left:
					RaiseCaretNavigationHorizontalEvent(TextUnit.Character, HorizontalDirection.Left);
					break;
				case Keys.Right:
					RaiseCaretNavigationHorizontalEvent(TextUnit.Character, HorizontalDirection.Right);
					break;
				case Keys.Up:
					RaiseCaretNavigationVerticalEvent(VerticalDirection.Up);
					break;
				default:
					return;
			}
			e.Handled = true;
			this.Invalidate();
		}
		
		private void UserControlKeyDown(KeyEventArgs e)
		{
			if(!e.Control) 
				throw new Exception("Control key required for function UserControlKeyDown.");
/*			switch(e.KeyCode)
			{
				case Keys.C:
					RaiseCopyEvent();
					break;
				case Keys.V:
					RaisePasteEvent();
					break;
				case Keys.X:
					RaiseCutEvent();
					break;
				case Keys.Y:
					RaiseRedoEvent();
					break;
				case Keys.Z:
					RaiseUndoEvent();
					break;
				default:
					return;
			}
			e.Handled = true;
			this.Invalidate();
*/		}
		
		private void UserShiftKeyDown(KeyEventArgs e)
		{
			if(!e.Shift)
				throw new Exception("Shift key required for function UserShiftKeyDown.");
			switch(e.KeyCode)
			{
				case Keys.Down:
					RaiseHighlightNavigationVerticalEvent(VerticalDirection.Down);
					break;
				case Keys.End:
					RaiseHighlightNavigationEndEvent();
					break;
				case Keys.Home:
					RaiseHighlightNavigationHomeEvent();
					break;
				case Keys.Left:
					RaiseHighlightNavigationHorizontalEvent(TextUnit.Character, HorizontalDirection.Left);
					break;
				case Keys.Right:
					RaiseHighlightNavigationHorizontalEvent(TextUnit.Character, HorizontalDirection.Right);
					break;
				case Keys.Up:
					RaiseHighlightNavigationVerticalEvent(VerticalDirection.Up);
					break;
				default:
					return;
			}
			e.Handled = true;
			this.Invalidate();
		}
		
		private void UserKeyPress(object sender, KeyPressEventArgs e)
		{
			if((Control.ModifierKeys & Keys.Control) == Keys.Control) return;
			else if(e.KeyChar == '\b')
			{
				RaiseEraseEvent(TextUnit.Character, -1);
			}
			else
			{
				RaiseTextEvent(e.KeyChar);
			}
			e.Handled = true;
			this.Invalidate();
		}
		
		private void UserMouseClick(object sender, MouseEventArgs e)
		{
		}
		
		private void UserMouseDown(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right) return;
			SetupDragAndDrop();
			RaiseCaretNavigationPointEvent(e.X, e.Y);
			this.Invalidate();
		}
		
		private void UserMouseMove(object sender, MouseEventArgs e)
		{
			int caretPosition = documentView.CaretPosition;
			RaiseHighlightNavigationPointEvent(e.X, e.Y);
			if(caretPosition != documentView.CaretPosition)
				this.Invalidate();
		}
		
		private void UserMouseUp(object sender, MouseEventArgs e)
		{
			TeardownDragAndDrop();
		}
		
		private void RaiseTextEvent(char text)
		{
			if(OnTextEvent == null) return;
			OnTextEvent(this, new TextEventArgs(text));
		}
		
		private void RaiseCaretNavigationHorizontalEvent(TextUnit unit, HorizontalDirection direction)
		{
			if(OnCaretNavigationHorizontalEvent == null) return;
			EnableCaretMovingTimer();
			OnCaretNavigationHorizontalEvent(this, new NavigationHorizontalEventArgs(unit, direction));
		}
		
		private void RaiseCaretNavigationVerticalEvent(VerticalDirection direction)
		{
			if(OnCaretNavigationVerticalEvent == null) return;
			EnableCaretMovingTimer();
			OnCaretNavigationVerticalEvent(this, new NavigationVerticalEventArgs(direction));
		}
		
		private void RaiseHighlightNavigationHorizontalEvent(TextUnit unit, HorizontalDirection direction)
		{
			if(OnHighlightNavigationHorizontalEvent == null) return;
			OnHighlightNavigationHorizontalEvent(this, new NavigationHorizontalEventArgs(unit, direction));
		}
		
		private void RaiseHighlightNavigationVerticalEvent(VerticalDirection direction)
		{
			if(OnHighlightNavigationVerticalEvent == null) return;
			OnHighlightNavigationVerticalEvent(this, new NavigationVerticalEventArgs(direction));
		}
		
		private void RaiseEraseEvent(TextUnit unit, int amount)
		{
			if(OnEraseEvent == null) return;
			OnEraseEvent(this, new EraseEventArgs(unit, amount));
		}
		
		public void RaiseUndoEvent()
		{
			if(OnUndoEvent == null) return;
			OnUndoEvent(this, new EventArgs());
			this.Invalidate();
		}

		public void RaiseRedoEvent()
		{
			if(OnRedoEvent == null) return;
			OnRedoEvent(this, new EventArgs());
			this.Invalidate();
		}
		
		private void RaiseCaretNavigationPointEvent(int x, int y)
		{
			if(OnCaretNavigationPointEvent == null) return;
			EnableCaretMovingTimer();
			OnCaretNavigationPointEvent(this, new NavigationPointEventArgs(x, y));
		}

		private void RaiseHighlightNavigationPointEvent(int x, int y)
		{
			if(OnHighlightNavigationPointEvent == null) return;
			OnHighlightNavigationPointEvent(this, new NavigationPointEventArgs(x, y));
		}
		
		private void RaiseCaretNavigationHomeEvent()
		{
			if(OnCaretNavigationHomeEvent == null) return;
			OnCaretNavigationHomeEvent(this, new EventArgs());
		}
		
		private void RaiseCaretNavigationEndEvent()
		{
			if(OnCaretNavigationEndEvent == null) return;
			OnCaretNavigationEndEvent(this, new EventArgs());
		}

		private void RaiseHighlightNavigationHomeEvent()
		{
			if(OnHighlightNavigationHomeEvent == null) return;
			OnHighlightNavigationHomeEvent(this, new EventArgs());
		}
		
		private void RaiseHighlightNavigationEndEvent()
		{
			if(OnHighlightNavigationEndEvent == null) return;
			OnHighlightNavigationEndEvent(this, new EventArgs());
		}
		
		public void RaiseCopyEvent()
		{
			if(OnCopyEvent == null) return;
			OnCopyEvent(this, new EventArgs());
		}
		
		public void RaiseCutEvent()
		{
			if(OnCutEvent == null) return;
			OnCutEvent(this, new EventArgs());
			this.Invalidate();
		}
		
		public void RaisePasteEvent()
		{
			if(OnPasteEvent == null) return;
			OnPasteEvent(this, new EventArgs());
			this.Invalidate();
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
