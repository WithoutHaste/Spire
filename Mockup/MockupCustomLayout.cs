using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

public static class CustomLayoutDemo
{
	public static void Paint(object sender, PaintEventArgs pea)
	{
		Bitmap graphicsBuffer = new Bitmap(790, 1000);
		Graphics g = Graphics.FromImage(graphicsBuffer);
		g.Clear(Color.White);
		g.SmoothingMode = SmoothingMode.AntiAlias;
		PaintTextBox(g, new Rectangle(10,10,350,390), null);
		PaintTextBox(g, new Rectangle(375,10,350,390), null);
		PaintTextBox(g, new Rectangle(10,415,715,200), null);
		PaintTextBox(g, new Rectangle(10,630,350,310), null);
		PaintTextBox(g, new Rectangle(375,630,350,310), null);
		g.Dispose();
		pea.Graphics.DrawImageUnscaled(graphicsBuffer, 0, 0);
	}

	private static void PaintTextBox(Graphics g, Rectangle rect, string text)
	{
		int borderPadding = 4;
		int linePadding = 4;
		int lineHeight = 12;
		Pen pen = new Pen(Color.LightBlue, 2);
		
		g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
		
		int fillerX = rect.X;
		int fillerY = rect.Y + borderPadding;
		while(fillerY < rect.Y + rect.Height - lineHeight)
		{
			g.FillRectangle(Brushes.LightGray, fillerX + borderPadding, fillerY, rect.Width + rect.X - fillerX - (borderPadding*2), lineHeight);
			fillerX = rect.X;
			fillerY += lineHeight + linePadding;
		}
	}
		
}

