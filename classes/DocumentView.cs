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
			layoutUpdatedTo = -1;
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
			UpdateLayoutFrom(layoutUpdatedTo);
			foreach(DisplayArea displayArea in displayAreas)
			{
				if(displayArea.ContainsCindex(cindex))
					return displayArea;
			}
			throw new Exception(String.Format("Cindex {0} not found in any display area. Layout updated to cindex {1}.", cindex, layoutUpdatedTo));
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
			if(displayArea.IsEmpty)
				return documentModel.Length;
			Graphics graphics = CreateDummyGraphics(displayArea.Width, displayArea.Height);
			Point currentPoint = CindexLocation(graphics, displayArea, currentPosition);
			Line? line = displayArea.GetLine(currentPosition);
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
			DisplayArea displayArea = GetDisplayAreaByCindex(CaretPosition);
			Line line = displayArea.GetLine(CaretPosition).Value;
			documentModel.CaretPosition = line.First;
		}
		
		private void MoveCaretEnd()
		{
			DisplayArea displayArea = GetDisplayAreaByCindex(CaretPosition);
			Line line = displayArea.GetLine(CaretPosition).Value;
			documentModel.CaretPosition = line.Last;
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
			string textToX = "";
			int charCount = 0;
			while(line.First+charCount <= line.Last)
			{
				textToX = documentModel.SubString(line.First, Math.Min(documentModel.Length-1, line.First+charCount));
				if((MeasureString(graphics, textToX)).Width > x)
					break;
				charCount++;
			}
			if(textToX.Length == 0)
			{
				return line.First;
			}
			SizeF currentSize = MeasureString(graphics, textToX);
			SizeF previousSize = MeasureString(graphics, textToX.Substring(0, textToX.Length-1));
			if(Math.Abs(x-currentSize.Width) < Math.Abs(x-previousSize.Width))
				return Math.Min(line.Last, line.First + charCount + 1);
			return Math.Min(line.Last, line.First + charCount);
		}
		
		private void UpdateLayoutFrom(Cindex cindex)
		{
			if(cindex > documentModel.Length) return;
			if(cindex < 0) cindex = 0;
			//cannot call GetDisplayAreaByCindex due to that function calling this one
			DisplayArea displayArea = null;
			foreach(DisplayArea d in displayAreas)
			{
				displayArea = d;
				if(displayArea.ContainsCindex(cindex))
					break;
			}
			if(displayArea == null)
				throw new Exception("No display areas found in UpdateLayoutFrom.");
			displayArea.ClearThroughPreviousLine(cindex);
			while(true)
			{
				UpdateLayout(displayArea);
				if(displayArea.IncludesEndOfDocument)
				{
					displayArea = NextDisplayArea(displayArea);
					while(displayArea != null)
					{
						displayArea.ResetBlank();
						displayArea = NextDisplayArea(displayArea);
					}
					return;
				}
				Cindex previousEndCindex = displayArea.End;
				displayArea = NextDisplayArea(displayArea);
				if(displayArea == null)
				{
					RaiseDocumentTooShortEvent();
					return;
				}
				displayArea.Reset(previousEndCindex+1);
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
					if(lineEnd < documentModel.Length)
					{
						displayArea.AddLineBreak(lineEnd);
						layoutUpdatedTo = Math.Min(documentModel.Length-1, lineEnd);
						lineStart = lineEnd + 1;
					}
					if(lineEnd == documentModel.Length)
					{
						break;
					}
					lineEnd++;
				}
			}
			displayArea.End = lineEnd;
			layoutUpdatedTo = Math.Min(documentModel.Length-1, lineEnd);
			if(lineEnd == documentModel.Length)
			{
				displayArea.IncludesEndOfDocument = true;
			}
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
			return end;
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
				DrawTextLine(graphics, displayArea, displayArea.Y+y, line);
				y += lineHeight;
			}
		}
		
		private void DrawTextLine(Graphics graphics, DisplayArea displayArea, int y, Line line)
		{
			if(line.Length == 0) return;
			DrawHighlightLine(graphics, displayArea, y, line);
			Brush textBrush = new SolidBrush(Color.Black);
			String text = documentModel.SubString(line.First, Math.Min(documentModel.Length-1, line.Last));
			text = text.Replace("\t", Constants.TabEquivalent);
			graphics.DrawString(text, Application.GlobalFont, textBrush, new Point(0, y), stringFormat);
		}
		
		private void DrawHighlightLine(Graphics graphics, DisplayArea displayArea, int y, Line line)
		{
			if(!HighlightOn) return;
			Cindex highlightStart = Math.Min(HighlightPosition, CaretPosition);
			Cindex highlightEnd = Math.Max(HighlightPosition, CaretPosition);
			if(highlightStart > line.Last) return;
			if(highlightEnd < line.First) return;
			Brush highlightBrush = new SolidBrush(Color.FromArgb(255, 205, 255, 255));
			Point start = CindexLocation(graphics, displayArea, Math.Max(highlightStart, line.First));
			Point end = (highlightEnd > line.Last) ? 
				LetterEndLocation(graphics, displayArea, Math.Min(documentModel.Length-1, Math.Min(highlightEnd, line.Last))) :
				CindexLocation(graphics, displayArea, Math.Min(highlightEnd, line.Last));
			int lineHeight = StringHeight(graphics, "X");
			graphics.FillRectangle(highlightBrush, start.X, start.Y, end.X-start.X, lineHeight);
		}
		
		private void DrawCaret(Graphics graphics)
		{
			Pen pen = new Pen(Color.Black, 0.5f);
			int lineHeight = StringHeight(graphics, "X");
			DisplayArea displayArea = GetDisplayAreaByCindex(CaretPosition);
			Point caretLocation = CindexLocation(graphics, displayArea, CaretPosition);
			graphics.DrawLine(pen, caretLocation.X, caretLocation.Y, caretLocation.X, caretLocation.Y + lineHeight);
		}
		
		//returns the top left point of the character at Cindex, ie the space before the character
		private Point CindexLocation(Graphics graphics, DisplayArea displayArea, Cindex cindex)
		{
			if(!displayArea.ContainsCindex(cindex))
				throw new Exception(String.Format("CindexLocation requires displayArea that contains provided cindex {0}.", cindex));
			Line line = displayArea.GetLine(cindex).Value;
			int lineHeight = StringHeight(graphics, "X");
			int y = (displayArea.LineCountToCindex(cindex) - 1) * lineHeight;
			string textToCaret = "";
			if(line.Length > 0 && line.First < cindex)
			{
				textToCaret = documentModel.SubString(line.First, cindex-1);
			}
			SizeF textSize = MeasureString(graphics, textToCaret);
			return new Point((int)Math.Ceiling(textSize.Width), y);
		}
		
		//returns the bottom right point of the character at Cindex
		private Point LetterEndLocation(Graphics graphics, DisplayArea displayArea, Cindex cindex)
		{
			if(!displayArea.ContainsCindex(cindex))
				throw new Exception(String.Format("LetterEndLocation requires displayArea that contains provided cindex {0}.", cindex));
			Line line = displayArea.GetLine(cindex).Value;
			int lineHeight = StringHeight(graphics, "X");
			int y = displayArea.LineCountToCindex(cindex) * lineHeight;
			string textToCaret = "";
			if(line.Length > 0 && line.First < cindex)
			{
				textToCaret = documentModel.SubString(line.First, Math.Min(documentModel.Length-1, cindex));
			}
			SizeF textSize = MeasureString(graphics, textToCaret);
			return new Point((int)Math.Ceiling(textSize.Width), y);
		}
		
		private SizeF MeasureString(Graphics graphics, string text)
		{
			text = text.Replace("\t", Constants.TabEquivalent);
			return graphics.MeasureString(text, Application.GlobalFont, new PointF(0,0), stringFormat);
		}
		
		private int StringHeight(Graphics graphics, string text)
		{
			return (int)Math.Ceiling(MeasureString(graphics, text).Height);
		}
		
	}
}
