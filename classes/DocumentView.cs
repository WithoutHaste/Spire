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
		
		private void UpdateLayoutFrom(Cindex cindex)
		{
			//assuming one infinite display area to start with
			DisplayArea displayArea = displayAreas[0];
			int lineBreakIndex = displayArea.GetLineBreakIndexBeforeCharIndex(cindex);
			if(lineBreakIndex > -1)
			{
				cindex = displayArea.LineBreaks[lineBreakIndex];
			}
			else
			{
				cindex = 0;
			}
			displayArea.ClearLineBreaksAfter(lineBreakIndex);
			using(Graphics graphics = CreateDummyGraphics(displayArea.Width, displayArea.Height))
			{
				while(cindex < documentModel.Length)
				{
					//find full line
					int endCindex = cindex;
					while(endCindex < documentModel.Length)
					{
						SizeF textSize = graphics.MeasureString(documentModel.SubString(cindex, endCindex), Application.GlobalFont, new PointF(0,0), stringFormat);
						if(textSize.Width > displayArea.Width)
						{
							endCindex--;
							displayArea.LineBreaks.Add(endCindex);
							break;
						}
						endCindex++;
					}
					layoutUpdatedTo = endCindex;
					cindex = endCindex+1;
				}
			}
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
			SizeF charSize = graphics.MeasureString("X", Application.GlobalFont, new PointF(0,0), stringFormat);
			DisplayArea displayArea = displayAreas[0];
			int y = 0;
			Cindex lineStart = 0;
			foreach(Cindex lineBreak in displayArea.LineBreaks)
			{
				graphics.DrawString(documentModel.SubString(lineStart, lineBreak), Application.GlobalFont, brush, new Point(0, y), stringFormat);
				y += (int)Math.Ceiling(charSize.Height);
				lineStart = lineBreak+1;
			}
			if(documentModel.Length-1 >= lineStart)
			{
				graphics.DrawString(documentModel.SubString(lineStart, documentModel.Length-1), Application.GlobalFont, brush, new Point(0, y), stringFormat);
			}
		}
		
		private void DrawCaret(Graphics graphics)
		{
			Pen pen = new Pen(Color.Black, 0.5f);
			SizeF charSize = graphics.MeasureString("X", Application.GlobalFont, new PointF(0,0), stringFormat);
			DisplayArea displayArea = displayAreas[0];
			Cindex lineStart = 0;
			int y = 0;
			int lineBreakIndex = displayArea.GetLineBreakIndexBeforeCharIndex(CaretPosition);
			if(lineBreakIndex > -1)
			{
				lineStart = displayArea.LineBreaks[lineBreakIndex] + 1;
				y = (lineBreakIndex+1) * (int)Math.Ceiling(charSize.Height);
			}
			string textToCaret = "";
			if(documentModel.Length-1 >= lineStart && lineStart <= CaretPosition-1)
			{
				textToCaret = documentModel.SubString(lineStart, CaretPosition-1);
			}
			SizeF textSize = graphics.MeasureString(textToCaret, Application.GlobalFont, new PointF(0,0), stringFormat);
			graphics.DrawLine(pen, Math.Max(1,textSize.Width), y, Math.Max(1,textSize.Width), y + Math.Max(charSize.Height, textSize.Height));
		}
		
	}
}
