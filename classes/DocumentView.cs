using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;

namespace Spire
{
	public class DocumentView
	{
		public event EventHandler OnDocumentTooShortEvent;

		private DocumentModel documentModel;
		private List<DisplayArea> displayAreas;
		private StringFormat stringFormat;
		private Cindex layoutUpdatedTo;
		
		public DocumentView(DocumentModel model)
		{
			documentModel = model;
			displayAreas = new List<DisplayArea>();
			stringFormat = GenerateStringFormat();
			layoutUpdatedTo = 0;
		}
		
		public Cindex CaretPosition
		{
			get { return documentModel.CaretPosition; }
		}
		
		public Cindex HighlightPosition
		{
			get { return documentModel.HighlightPosition; }
		} 
		
		public bool HighlightOn
		{
			get { return (HighlightPosition != CaretPosition); }
		}
		
		public int LineCount
		{
			get { 
				if(layoutUpdatedTo < documentModel.Length-1)
					UpdateLayoutFrom(layoutUpdatedTo);
				return displayAreas.Sum(p=>p.LineCount); 
			}
		}
		
		public int LineNumber /*starts at 1*/
		{
			get {
				int lineNumber = 0;
				foreach(DisplayArea displayArea in displayAreas)
				{
					if(displayArea.ContainsCindex(CaretPosition))
					{
						lineNumber += displayArea.LineCountToCindex(CaretPosition);
						break;
					}
					lineNumber += displayArea.LineCount;
				}
				return lineNumber;
			}
		}
		
		private StringFormat GenerateStringFormat()
		{
			StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };
			return stringFormat;
		}
		
		public void OnModelUpdateEvent(object sender, UpdateAtEventArgs e)
		{
			UpdateLayoutFrom(Math.Min(layoutUpdatedTo, e.At));
		}
		
		public void OnCaretNavigationVerticalEvent(object sender, NavigationVerticalEventArgs e)
		{
			documentModel.ClearHighlight();
			MoveCaretVertical(e);
		}
		
		public void OnCaretNavigationHomeEvent(object sender, EventArgs e)
		{
			documentModel.ClearHighlight();
			MoveCaretHome();
		}
		
		public void OnCaretNavigationEndEvent(object sender, EventArgs e)
		{
			documentModel.ClearHighlight();
			MoveCaretEnd();
		}
		
		public void OnHighlightNavigationVerticalEvent(object sender, NavigationVerticalEventArgs e)
		{
			documentModel.SetHighlight();
			MoveCaretVertical(e);
		}
		
		public void OnHighlightNavigationHomeEvent(object sender, EventArgs e)
		{
			documentModel.SetHighlight();
			MoveCaretHome();
		}
		
		public void OnHighlightNavigationEndEvent(object sender, EventArgs e)
		{
			documentModel.SetHighlight();
			MoveCaretEnd();
		}
		
		private void MoveCaretVertical(NavigationVerticalEventArgs e)
		{
			int amount = 0;
			switch(e.Direction)
			{
				case VerticalDirection.Up:
					amount = -1;
					break;
				case VerticalDirection.Down:
					amount = 1;
					break;
				default: throw new Exception(String.Format("VerticalDirection {0} not supported in document navigation", e.Direction));
			}
			documentModel.CaretPosition = CalculateVerticalMove(CaretPosition, amount);
		}
		
		private DisplayArea GetDisplayAreaByCindex(Cindex cindex)
		{
			foreach(DisplayArea displayArea in displayAreas)
			{
				if(displayArea.ContainsCindex(cindex))
					return displayArea;
			}
			if(cindex <= documentModel.Length && displayAreas.Count > 0)
				return displayAreas.Last();
			return null;
		}
		
		private DisplayArea PreviousDisplayArea(DisplayArea displayArea)
		{
			int index = displayAreas.IndexOf(displayArea);
			if(index == 0)
				return null;
			return displayAreas[index-1];
		}
		
		private DisplayArea NextDisplayArea(DisplayArea displayArea)
		{
			int index = displayAreas.IndexOf(displayArea);
			if(index >= displayAreas.Count-1)
				return null;
			return displayAreas[index+1];
		}
		
		private Cindex CalculateVerticalMove(Cindex currentPosition, int moveAmount)
		{
			DisplayArea displayArea = GetDisplayAreaByCindex(currentPosition);
			if(displayArea == null)
				return 0;
			if(displayArea.IsEmpty)
				return documentModel.Length;
			Graphics graphics = CreateDummyGraphics(displayArea.Width, displayArea.Height);
			Point currentPoint = CindexLocation(graphics, displayArea, currentPosition);
			Line? line = displayArea.GetLine(currentPosition);
			if(currentPosition == documentModel.Length)
			{
				line = displayArea.GetLine(currentPosition-1);
			}
			while(moveAmount < 0)
			{
				Line nextLine = line.Value;
				line = PreviousLine(displayArea, line.Value);
				if(line == null)
				{
					line = nextLine;
					moveAmount = 0;
					break;
				}
				moveAmount++;
			}
			while(moveAmount > 0)
			{
				Line previousLine = line.Value;
				line = NextLine(displayArea, line.Value);
				if(line == null)
				{
					line = previousLine;
					moveAmount = 0;
					break;
				}
				moveAmount--;
			}
			return FindCindexClosestToX(graphics, line.Value, currentPoint.X);
		}
		
		private Line? PreviousLine(DisplayArea displayArea, Line line)
		{
			Line? previousLine = displayArea.GetLine(line.First-1);
			if(previousLine.HasValue)
				return previousLine;
			do
			{
				displayArea = PreviousDisplayArea(displayArea);
			} while(displayArea != null && displayArea.IsEmpty);
			if(displayArea == null)
				return null;
			return displayArea.LastLine;
		}
		
		private Line? NextLine(DisplayArea displayArea, Line line)
		{
			Line? nextLine = displayArea.GetLine(line.Last+1);
			if(nextLine.HasValue)
				return nextLine;
			do
			{
				displayArea = NextDisplayArea(displayArea);
			} while(displayArea != null && displayArea.IsEmpty);
			if(displayArea == null)
				return null;
			return displayArea.FirstLine;
		}

		public void OnCaretNavigationPointEvent(object sender, NavigationPointEventArgs e)
		{
			documentModel.ClearHighlight();
			MoveCaretPoint(e);
		}

		public void OnHighlightNavigationPointEvent(object sender, NavigationPointEventArgs e)
		{
			documentModel.SetHighlight();
			MoveCaretPoint(e);
		}
		
		private void MoveCaretHome()
		{
			Cindex currentPosition = CaretPosition;
			if(currentPosition == documentModel.Length)
				currentPosition--;
			DisplayArea displayArea = GetDisplayAreaByCindex(currentPosition);
			if(displayArea == null) return;
			Line? line = displayArea.GetLine(currentPosition);
			if(line == null) return;
			documentModel.CaretPosition = line.Value.First;
		}
		
		private void MoveCaretEnd()
		{
			DisplayArea displayArea = GetDisplayAreaByCindex(CaretPosition);
			if(displayArea == null) return;
			Line? line = displayArea.GetLine(CaretPosition);
			if(line == null) return;
			if(line.Value.Last == documentModel.Length-1)
				documentModel.CaretPosition = documentModel.Length;
			else
				documentModel.CaretPosition = line.Value.Last;
		}
		
		private DisplayArea GetDisplayAreaByPoint(Point point)
		{
			foreach(DisplayArea displayArea in displayAreas)
			{
				if(displayArea.ContainsPoint(point))
					return displayArea;
			}
			return null;
		}
		
		private void MoveCaretPoint(NavigationPointEventArgs e)
		{
			DisplayArea displayArea = GetDisplayAreaByPoint(new Point(e.X, e.Y));
			if(displayArea == null)
				return;
			Graphics graphics = CreateDummyGraphics(displayArea.Width, displayArea.Height);
			decimal lineHeight = StringHeight(graphics, "X");
			int lineNumber = (int)Math.Ceiling(e.Y / lineHeight);
			Line? line = displayArea.GetIthLine(lineNumber);
			if(line == null)
				line = displayArea.LastLine;
			if(line == null)
				return;
			documentModel.CaretPosition = FindCindexClosestToX(graphics, line.Value, e.X);
		}
		
		private Cindex FindCindexClosestToX(Graphics graphics, Line line, int x)
		{
			if(documentModel.Length == 0) return 0;
			string textToX = "";
			int charCount = 0;
			Cindex max = (line.Last == documentModel.Length-1) ? (Cindex)documentModel.Length : line.Last;
			while(line.First+charCount <= max)
			{
				textToX = documentModel.SubString(line.First, Math.Min(documentModel.Length-1, line.First+charCount));
				if((MeasureString(graphics, textToX)).Width > x)
					break;
				charCount++;
			}
			SizeF currentSize = MeasureString(graphics, textToX);
			SizeF previousSize = MeasureString(graphics, textToX.Substring(0, textToX.Length-1));
			if(Math.Abs(x-currentSize.Width) < Math.Abs(x-previousSize.Width))
				return Math.Min(max, line.First + charCount + 1);
			return Math.Min(max, line.First + charCount);
		}
		
		private void UpdateLayoutFrom(Cindex cindex)
		{
			DisplayArea displayArea = GetDisplayAreaByCindex(cindex);
			if(displayArea == null)
			{
				return;
			}
			displayArea.ClearThroughPreviousLine(cindex);
			Cindex? cindexBeforeGeneratingNewDisplayArea = null;
			while(true)
			{
				UpdateLayout(displayArea);
				int end = displayArea.End;
				displayArea = NextDisplayArea(displayArea);
				if(displayArea == null)
				{
					if(end == documentModel.Length-1)
					{
						return;
					}
					if(cindexBeforeGeneratingNewDisplayArea.HasValue && cindexBeforeGeneratingNewDisplayArea == end)
					{
						return;
					}
					RaiseDocumentTooShortEvent();
					return;
				}
				displayArea.Reset(end+1);
			}
		}
		
		private void UpdateLayout(DisplayArea displayArea)
		{
			Cindex lineStart = displayArea.LastLineStart;
			Cindex lineEnd = lineStart;
			using(Graphics graphics = CreateDummyGraphics(displayArea.Width, displayArea.Height))
			{
				int lineHeight = StringHeight(graphics, "X");
				while(lineEnd < documentModel.Length && (displayArea.LineCount + 1)*lineHeight < displayArea.Height)
				{
					lineEnd = FindEndOfLine(displayArea, graphics, lineEnd);
					if(lineEnd < documentModel.Length-1 || (lineEnd < documentModel.Length && documentModel[lineEnd] == Constants.EndLineCharacter))
					{
						displayArea.AddLineBreak(lineEnd);
						layoutUpdatedTo = lineEnd + 1;
						lineStart = lineEnd + 1;
					}
					lineEnd++;
				}
			}
			displayArea.End = lineEnd - 1;
		}
		
		private Cindex FindEndOfLine(DisplayArea displayArea, Graphics graphics, Cindex start)
		{
			int end = start;
			int? lastSpace = null;
			while(end < documentModel.Length)
			{
				if(documentModel[end] == Constants.EndLineCharacter)
					return end;			
				if(documentModel[end] == ' ')
					lastSpace = end;
				SizeF textSize = graphics.MeasureString(documentModel.SubString(start, end), Application.GlobalFont, new PointF(0,0), stringFormat);
				if(textSize.Width > displayArea.Width)
				{
					if(lastSpace.HasValue && lastSpace != end)
						return lastSpace.Value;
					return end-1;
				}
				end++;
			}
			return end-1;
		}

		public void AppendDisplayArea(DisplayArea displayArea)
		{
			if(displayAreas.Count == 0)
				displayArea.Start = 0;
			else if(!displayAreas.Last().IsEmpty)
				displayArea.Start = displayAreas.Last().End + 1;
			else if(displayAreas.Last().Start >= 0)
				displayArea.Start = displayAreas.Last().Start;
			displayAreas.Add(displayArea);
		}
		
		public void RaiseDocumentTooShortEvent()
		{
			if(OnDocumentTooShortEvent == null) return;
			OnDocumentTooShortEvent(this, new EventArgs());
		}
		
		public void Paint(Graphics graphics, bool drawCaret)
		{
			SetupGraphics(graphics);
			graphics.Clear(Color.White);
			DrawText(graphics);
			if(drawCaret)
			{
				DrawCaret(graphics);
			}
		}
		
		private void SetupGraphics(Graphics graphics)
		{
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
		}
		
		private Graphics CreateDummyGraphics(int width, int height)
		{
			Bitmap graphicsBuffer = new Bitmap(width, height);
			Graphics graphics = Graphics.FromImage(graphicsBuffer);
			graphics.PageUnit = GraphicsUnit.Pixel;
			return graphics;
		}
		
		private void DrawText(Graphics graphics)
		{
			foreach(DisplayArea displayArea in displayAreas)
			{
				DrawText(graphics, displayArea);
			}
		}
		
		private void DrawText(Graphics graphics, DisplayArea displayArea)
		{
			int lineHeight = StringHeight(graphics, "X");
			int y = 0;
			foreach(Line line in displayArea.GetLines())
			{
				DrawTextLine(graphics, displayArea.Y+y, line);
				y += lineHeight;
			}
		}
		
		private void DrawTextLine(Graphics graphics, int y, Line line)
		{
			DrawHighlightLine(graphics, y, line);
			Brush textBrush = new SolidBrush(Color.Black);
			graphics.DrawString(documentModel.SubString(line.First, line.Last).Replace("\t", Constants.TabEquivalent), Application.GlobalFont, textBrush, new Point(0, y), stringFormat);
		}
		
		private void DrawHighlightLine(Graphics graphics, int y, Line line)
		{
			if(!HighlightOn) return;
			Cindex highlightStart = Math.Min(HighlightPosition, CaretPosition);
			Cindex highlightEnd = Math.Max(HighlightPosition, CaretPosition);
			if(highlightStart > line.Last) return;
			if(highlightEnd < line.First) return;
			Brush highlightBrush = new SolidBrush(Color.FromArgb(255, 205, 255, 255));
			Point start = CindexLocation(graphics, displayAreas[0], Math.Max(highlightStart, line.First));
			Point end = (highlightEnd > line.Last) ? 
				LetterEndLocation(graphics, displayAreas[0], Math.Min(highlightEnd, line.Last)) :
				CindexLocation(graphics, displayAreas[0], Math.Min(highlightEnd, line.Last));
			int lineHeight = StringHeight(graphics, "X");
			graphics.FillRectangle(highlightBrush, start.X, start.Y, end.X-start.X, lineHeight);
		}
		
		private void DrawCaret(Graphics graphics)
		{
			Pen pen = new Pen(Color.Black, 0.5f);
			int lineHeight = StringHeight(graphics, "X");
			Point caretLocation = CindexLocation(graphics, displayAreas[0], CaretPosition);
			graphics.DrawLine(pen, caretLocation.X, caretLocation.Y, caretLocation.X, caretLocation.Y + lineHeight);
		}
		
		//returns the top left point of the character at Cindex, ie the space before the character
		private Point CindexLocation(Graphics graphics, DisplayArea displayArea, Cindex cindex)
		{
			Line? line = displayArea.GetLine(cindex);
			if(line == null && IsLastDisplayArea(displayArea))
			{
				line = displayArea.GetLine(cindex-1);
				if(line == null)
				{
					return new Point(0,0);
				}
			}
			int lineHeight = StringHeight(graphics, "X");
			int y = (displayArea.LineCountToCindex(cindex) - 1) * lineHeight;
			string textToCaret = "";
			if(line.Value.First < documentModel.Length && line.Value.First < cindex)
			{
				textToCaret = documentModel.SubString(line.Value.First, cindex-1);
			}
			SizeF textSize = MeasureString(graphics, textToCaret);
			return new Point((int)Math.Ceiling(textSize.Width), y);
		}
		
		//returns the bottom right point of the character at Cindex
		private Point LetterEndLocation(Graphics graphics, DisplayArea displayArea, Cindex cindex)
		{
			Line? line = displayArea.GetLine(cindex);
			if(line == null)
			{
				return new Point(0,0);
			}
			int lineHeight = StringHeight(graphics, "X");
			int y = displayArea.LineCountToCindex(cindex) * lineHeight;
			string textToCaret = "";
			if(line.Value.First < documentModel.Length && line.Value.First < cindex)
			{
				textToCaret = documentModel.SubString(line.Value.First, cindex);
			}
			SizeF textSize = MeasureString(graphics, textToCaret);
			return new Point((int)Math.Ceiling(textSize.Width), y);
		}
		
		private SizeF MeasureString(Graphics graphics, string text)
		{
			return graphics.MeasureString(text.Replace("\t", Constants.TabEquivalent), Application.GlobalFont, new PointF(0,0), stringFormat);
		}
		
		private int StringHeight(Graphics graphics, string text)
		{
			return (int)Math.Ceiling(MeasureString(graphics, text).Height);
		}

		private bool IsLastDisplayArea(DisplayArea displayArea)
		{
			if(displayAreas.Count == 0) throw new Exception("No display areas found in DocumentView.");
			DisplayArea lastDisplayArea = displayAreas[0];
			foreach(DisplayArea	nextDisplayArea in displayAreas)
			{
				if(!nextDisplayArea.IsEmpty)
					lastDisplayArea = nextDisplayArea;
			}
			return (displayArea == lastDisplayArea);
		}
		
	}
}
