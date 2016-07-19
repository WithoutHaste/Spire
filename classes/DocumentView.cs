using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Spire
{
	public class DocumentView
	{
		private DocumentModel documentModel;
		private List<DisplayArea> displayAreas;
		private StringFormat stringFormat;
		private Cindex layoutUpdatedTo;
		
		public DocumentView(DocumentModel model)
		{
			documentModel = model;
			displayAreas = new List<DisplayArea>();
			stringFormat = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };
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
			get { return displayAreas[0].LineBreaks.Count + 1; }
		}
		
		public void OnModelUpdateEvent(object sender, UpdateAtEventArgs e)
		{
			UpdateLayoutFrom(Math.Min(layoutUpdatedTo, PreviousLineBreak(e.At)));
		}
		
		public void OnCaretNavigationVerticalEvent(object sender, NavigationVerticalEventArgs e)
		{
			documentModel.ClearHighlight();
			MoveCaretVertical(e);
		}
		
		public void OnHighlightNavigationVerticalEvent(object sender, NavigationVerticalEventArgs e)
		{
			documentModel.SetHighlight();
			MoveCaretVertical(e);
		}
		
		private void MoveCaretVertical(NavigationVerticalEventArgs e)
		{
			int amount = 0;
			switch(e.Direction)
			{
				case VerticalDirection.Up: amount = -1; break;
				case VerticalDirection.Down: amount = 1; break;
				default: throw new Exception(String.Format("VerticalDirection {0} not supported in document navigation", e.Direction));
			}
			DisplayArea displayArea = displayAreas[0];
			Graphics graphics = CreateDummyGraphics(displayArea.Width, displayArea.Height);
			documentModel.CaretPosition = CalculateVerticalMove(graphics, displayArea, CaretPosition, amount);
		}
		
		private Cindex CalculateVerticalMove(Graphics graphics, DisplayArea displayArea, Cindex currentPosition, int moveAmount)
		{
			Point currentPoint = CindexLocation(graphics, displayArea, currentPosition);
			int lineBreakIndex = displayArea.GetLineBreakIndexBeforeCharIndex(currentPosition);
			lineBreakIndex += moveAmount;
			if(lineBreakIndex < -1)
				return currentPosition;
			if(lineBreakIndex >= displayArea.LineBreaks.Count)
				return currentPosition;
			Cindex lineStart = 0;
			if(lineBreakIndex > -1)
				lineStart = displayArea.LineBreaks[lineBreakIndex] + 1;
			return FindCindexClosestToX(graphics, lineStart, currentPoint.X);
		}
		
		public void OnNavigationPointEvent(object sender, NavigationPointEventArgs e)
		{
			DisplayArea displayArea = displayAreas[0];
			Graphics graphics = CreateDummyGraphics(displayArea.Width, displayArea.Height);
			decimal lineHeight = StringHeight(graphics, "X");
			int lineIndex = (int)Math.Floor(e.Y / lineHeight);
			int lineBreakIndex = Math.Min(lineIndex - 1, displayArea.LineBreaks.Count - 1);
			int lineStart = (lineBreakIndex >= 0) ? displayArea.LineBreaks[lineBreakIndex] + 1 : 0;
			int cindex = FindCindexClosestToX(graphics, lineStart, e.X);
			documentModel.CaretPosition = cindex;
			documentModel.ClearHighlight();
		}
		
		private Cindex FindCindexClosestToX(Graphics graphics, Cindex lineStart, int x)
		{
			string textToX = "";
			int charCount = 0;
			int max = NextLineBreak(lineStart);
			while(lineStart+charCount <= max)
			{
				textToX = documentModel.SubString(lineStart, Math.Min(documentModel.Length-1, lineStart+charCount));
				if((MeasureString(graphics, textToX)).Width > x)
					break;
				charCount++;
			}
			SizeF currentSize = MeasureString(graphics, textToX);
			SizeF previousSize = MeasureString(graphics, textToX.Substring(0, textToX.Length-1));
			if(Math.Abs(x-currentSize.Width) < Math.Abs(x-previousSize.Width))
				return Math.Min(max, lineStart + charCount + 1);
			return Math.Min(max, lineStart + charCount);
		}
		
		private Cindex PreviousLineBreak(Cindex cindex)
		{
			//assuming one infinite display area to start with
			DisplayArea displayArea = displayAreas[0];
			int lineBreakIndex = displayArea.GetLineBreakIndexBeforeCharIndex(cindex);
			if(lineBreakIndex < 0) return 0;
			return displayArea.LineBreaks[lineBreakIndex];
		}
		
		private Cindex NextLineBreak(Cindex cindex)
		{
			//assuming one infinite display area to start with
			DisplayArea displayArea = displayAreas[0];
			int lineBreakIndex = displayArea.GetLineBreakIndexBeforeCharIndex(cindex);
			lineBreakIndex++;
			if(lineBreakIndex < displayArea.LineBreaks.Count)
				return displayArea.LineBreaks[lineBreakIndex];
			return documentModel.Length;
		}
		
		private void UpdateLayoutFrom(Cindex cindex)
		{
			//assuming one infinite display area to start with
			DisplayArea displayArea = displayAreas[0];
			cindex = displayArea.ClearLineBreaksAfter(cindex);
			using(Graphics graphics = CreateDummyGraphics(displayArea.Width, displayArea.Height))
			{
				int endCindex = cindex;
				while(endCindex < documentModel.Length-1)
				{
					endCindex = FindEndOfLine(displayArea, graphics, cindex);
					if(endCindex < documentModel.Length-1)
					{
						displayArea.LineBreaks.Add(endCindex);
						layoutUpdatedTo = endCindex + 1;
						cindex = endCindex + 1;
					}
					endCindex++;
				}
			}
		}
		
		private Cindex FindEndOfLine(DisplayArea displayArea, Graphics graphics, Cindex start)
		{
			int end = start;
			int? lastSpace = null;
			while(end < documentModel.Length)
			{
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
			displayArea.Start = 0;
			displayAreas.Add(displayArea);
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
			int lineHeight = StringHeight(graphics, "X");
			DisplayArea displayArea = displayAreas[0];
			int y = 0;
			Cindex lineStart = 0;
			foreach(Cindex lineBreak in displayArea.LineBreaks)
			{
				DrawTextLine(graphics, y, lineStart, lineBreak);
				y += lineHeight;
				lineStart = lineBreak+1;
			}
			if(lineStart < documentModel.Length)
			{
				DrawTextLine(graphics, y, lineStart, documentModel.Length-1);
			}
		}
		
		private void DrawTextLine(Graphics graphics, int y, Cindex lineStart, Cindex lineEnd)
		{
			DrawHighlightLine(graphics, y, lineStart, lineEnd);
			Brush textBrush = new SolidBrush(Color.Black);
			graphics.DrawString(documentModel.SubString(lineStart, lineEnd), Application.GlobalFont, textBrush, new Point(0, y), stringFormat);
		}
		
		private void DrawHighlightLine(Graphics graphics, int y, Cindex lineStart, Cindex lineEnd)
		{
			if(!HighlightOn) return;
			Cindex highlightStart = Math.Min(HighlightPosition, CaretPosition);
			Cindex highlightEnd = Math.Max(HighlightPosition, CaretPosition);
			if(highlightStart > lineEnd) return;
			if(highlightEnd < lineStart) return;
			Brush highlightBrush = new SolidBrush(Color.FromArgb(255, 205, 255, 255));
			Point start = CindexLocation(graphics, displayAreas[0], Math.Max(highlightStart, lineStart));
			Point end = (highlightEnd > lineEnd) ? 
				LetterEndLocation(graphics, displayAreas[0], Math.Min(highlightEnd, lineEnd)) :
				CindexLocation(graphics, displayAreas[0], Math.Min(highlightEnd, lineEnd));
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
		
		private Point CindexLocation(Graphics graphics, DisplayArea displayArea, Cindex cindex)
		{
			int lineHeight = StringHeight(graphics, "X");
			Cindex lineStart = 0;
			int y = 0;
			int lineBreakIndex = displayArea.GetLineBreakIndexBeforeCharIndex(cindex);
			if(lineBreakIndex > -1)
			{
				lineStart = displayArea.LineBreaks[lineBreakIndex] + 1;
				y = (lineBreakIndex+1) * lineHeight;
			}
			string textToCaret = "";
			if(lineStart < documentModel.Length && lineStart < cindex)
			{
				textToCaret = documentModel.SubString(lineStart, cindex-1);
			}
			SizeF textSize = MeasureString(graphics, textToCaret);
			return new Point((int)Math.Ceiling(textSize.Width), y);
		}
		
		private Point LetterEndLocation(Graphics graphics, DisplayArea displayArea, Cindex cindex)
		{
			int lineHeight = StringHeight(graphics, "X");
			Cindex lineStart = 0;
			int y = 0;
			int lineBreakIndex = displayArea.GetLineBreakIndexBeforeCharIndex(cindex);
			if(lineBreakIndex > -1)
			{
				lineStart = displayArea.LineBreaks[lineBreakIndex] + 1;
				y = (lineBreakIndex+1) * lineHeight;
			}
			string text = "";
			if(lineStart < documentModel.Length && lineStart < cindex)
			{
				text = documentModel.SubString(lineStart, cindex);
			}
			SizeF textSize = MeasureString(graphics, text);
			return new Point((int)Math.Ceiling(textSize.Width), y+lineHeight);
		}
		
		private SizeF MeasureString(Graphics graphics, string text)
		{
			return graphics.MeasureString(text, Application.GlobalFont, new PointF(0,0), stringFormat);
		}
		
		private int StringHeight(Graphics graphics, string text)
		{
			return (int)Math.Ceiling(MeasureString(graphics, text).Height);
		}
		
	}
}
