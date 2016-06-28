using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Spire
{
	public class DocumentView
	{
		private DocumentModel documentModel;
		private StringFormat stringFormat;
		
		public DocumentView(DocumentModel model)
		{
			documentModel = model;			
			stringFormat = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };
		}
		
		public int CaretPosition
		{
			get { return documentModel.CaretPosition; }
		}
		
		private string Line(int lineIndex)
		{
			if(documentModel.Length == 0) return "";
			return documentModel.SubString(0, documentModel.Length-1);
		}
		
		public void OnModelUpdateEvent(object sender, UpdateAtEventArgs e)
		{
//			Console.WriteLine("view got model's update message");
		}

		public void Paint(Graphics graphics, bool drawCaret)
		{
			graphics.Clear(Color.White);
			DrawText(graphics);
			if(drawCaret)
			{
				DrawCaret(graphics);
			}
		}
		
		private void DrawText(Graphics graphics)
		{
			Brush brush = new SolidBrush(Color.Black);
			graphics.DrawString(Line(0), Application.GlobalFont, brush, new Point(0, 0), stringFormat);
		}
		
		private void DrawCaret(Graphics graphics)
		{
			Pen pen = new Pen(Color.Black, 0.5f);
			string textToCaret = Line(0).Substring(0, CaretPosition);
			SizeF textSize = graphics.MeasureString(textToCaret, Application.GlobalFont, new PointF(0,0), stringFormat);
			SizeF charSize = graphics.MeasureString("X", Application.GlobalFont, new PointF(0,0), stringFormat);
			graphics.DrawLine(pen, Math.Max(1,textSize.Width), 0, Math.Max(1,textSize.Width), Math.Max(charSize.Height, textSize.Height));
		}
	}
}