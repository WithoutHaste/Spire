using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

public static class FootnotesDemo
{
	public static void Paint(object sender, PaintEventArgs pea)
	{
		Bitmap graphicsBuffer = new Bitmap(790, 1500);
		Graphics g = Graphics.FromImage(graphicsBuffer);
		g.Clear(Color.White);
		g.SmoothingMode = SmoothingMode.AntiAlias;
		
		Brush brush = new SolidBrush(Color.Black);
		Font normalFont = new Font("Times New Roman", 16);
		int lineHeight = (int)(g.MeasureString("TEST", normalFont).Height);
		int y = 0;
		using(StreamReader reader = new StreamReader("footnotesText1.txt"))
		{
			int lineNumber = 0;
			int charsPerLine = 70;
			while(reader.Peek() >= 0)
			{
				char[] c = new char[charsPerLine];
				reader.Read(c, 0, c.Length);
				g.DrawString(new string(c), normalFont, brush, 15, lineHeight*lineNumber);
				lineNumber++;
				y = lineNumber * lineHeight;
			}
		}
		Pen pen = new Pen(Color.Black, 1.0F);
		y += 15;
		g.DrawLine(pen, 15, y, 650, y);
		y += 10;
		
		Font footnoteFont = new Font("Times New Roman", 13);
		lineHeight = (int)(g.MeasureString("TEST", footnoteFont).Height);
		using(StreamReader reader = new StreamReader("footnotesText2.txt"))
		{
			int lineNumber = 0;
			string line;
			while((line = reader.ReadLine()) != null)
			{
				g.DrawString(line, footnoteFont, brush, 25, y + lineHeight*lineNumber);
				lineNumber++;
			}
		}
	
		g.Dispose();
		pea.Graphics.DrawImageUnscaled(graphicsBuffer, 0, 0);
	}
}

