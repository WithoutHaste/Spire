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
		
		public int CaretPosition
		{
			get { return documentModel.CaretPosition; }
		}
		
		public void OnModelUpdateEvent(object sender, UpdateAtEventArgs e)
		{
			UpdateLayoutFrom(Math.Min(layoutUpdatedTo, e.At));
		}
		
		public void OnNavigationVerticalEvent(object sender, NavigationVerticalEventArgs e)
		{
			DisplayArea displayArea = displayAreas[0];
			Graphics graphics = CreateDummyGraphics(displayArea.Width, displayArea.Height);
			Point caretLocation = CaretLocation(graphics, displayArea);
			int lineBreakIndex = displayArea.GetLineBreakIndexBeforeCharIndex(CaretPosition);
			lineBreakIndex += e.Amount; //assuming only + or - one line
			if(lineBreakIndex < -1)
				return;
			if(lineBreakIndex >= displayArea.LineBreaks.Count)
				return;
			Cindex lineStart = 0;
			if(lineBreakIndex > -1)
				lineStart = displayArea.LineBreaks[lineBreakIndex] + 1;
			documentModel.CaretPosition = FindCindexClosestToX(graphics, lineStart, caretLocation.X);			
		}
		
		private Cindex FindCindexClosestToX(Graphics graphics, Cindex lineStart, int x)
		{
			//is it possible to run off the end of the line here?
			string textToX = "";
			int charCount = 0;
			while(lineStart+charCount < documentModel.Length)
			{
				textToX = documentModel.SubString(lineStart, lineStart+charCount);
				if((MeasureString(graphics, textToX)).Width > x)
					break;
				charCount++;
			}
			SizeF currentSize = MeasureString(graphics, textToX);
			SizeF previousSize = MeasureString(graphics, textToX.Substring(0, textToX.Length-1));
			if(Math.Abs(x-currentSize.Width) < Math.Abs(x-previousSize.Width))
				return lineStart + charCount + 1;
			return lineStart + charCount;
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
			Brush brush = new SolidBrush(Color.Black);
			int lineHeight = StringHeight(graphics, "X");
			DisplayArea displayArea = displayAreas[0];
			int y = 0;
			Cindex lineStart = 0;
			foreach(Cindex lineBreak in displayArea.LineBreaks)
			{
				graphics.DrawString(documentModel.SubString(lineStart, lineBreak), Application.GlobalFont, brush, new Point(0, y), stringFormat);
				y += lineHeight;
				lineStart = lineBreak+1;
			}
			if(lineStart < documentModel.Length)
			{
				graphics.DrawString(documentModel.SubString(lineStart, documentModel.Length-1), Application.GlobalFont, brush, new Point(0, y), stringFormat);
			}
		}
		
		private void DrawCaret(Graphics graphics)
		{
			Pen pen = new Pen(Color.Black, 0.5f);
			int lineHeight = StringHeight(graphics, "X");
			Point caretLocation = CaretLocation(graphics, displayAreas[0]);
			graphics.DrawLine(pen, caretLocation.X, caretLocation.Y, caretLocation.X, caretLocation.Y + lineHeight);
		}
		
		private Point CaretLocation(Graphics graphics, DisplayArea displayArea)
		{
			int lineHeight = StringHeight(graphics, "X");
			Cindex lineStart = 0;
			int y = 0;
			int lineBreakIndex = displayArea.GetLineBreakIndexBeforeCharIndex(CaretPosition);
			if(lineBreakIndex > -1)
			{
				lineStart = displayArea.LineBreaks[lineBreakIndex] + 1;
				y = (lineBreakIndex+1) * lineHeight;
			}
			string textToCaret = "";
			if(lineStart < documentModel.Length && lineStart < CaretPosition)
			{
				textToCaret = documentModel.SubString(lineStart, CaretPosition-1);
			}
			SizeF textSize = MeasureString(graphics, textToCaret);
			return new Point((int)Math.Ceiling(textSize.Width), y);
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
