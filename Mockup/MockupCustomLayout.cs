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
	private static Panel drawPanel;
	private static Point? mouseDownPoint;
	private static Point? mouseDragPoint;
	private static List<Rectangle> boxes;

	public static void AddListeners(Panel panel)
	{
		drawPanel = panel;
		boxes = new List<Rectangle>();
		panel.MouseDown += new MouseEventHandler(MouseDown);
		panel.MouseUp += new MouseEventHandler(MouseUp);
		panel.MouseMove += new MouseEventHandler(MouseMove);
	}
	
	public static void RemoveListeners(Panel panel)
	{
		drawPanel = null;
		panel.MouseDown -= MouseDown;
		panel.MouseUp -= MouseUp;
		panel.MouseMove -= MouseMove;
	}
	
	private static void MouseDown(object sender, MouseEventArgs e)
	{
		mouseDownPoint = new Point(e.X, e.Y);
	}
	
	private static void MouseUp(object sender, MouseEventArgs e)
	{
		boxes.Add(new Rectangle(
			Math.Min(mouseDownPoint.Value.X, e.X)
			, Math.Min(mouseDownPoint.Value.Y, e.Y)
			, Math.Abs(mouseDownPoint.Value.X - e.X)
			, Math.Abs(mouseDownPoint.Value.Y - e.Y)
		));
		mouseDownPoint = null;
		drawPanel.Invalidate();
	}
	
	private static void MouseMove(object sender, MouseEventArgs e)
	{
		if(mouseDownPoint == null) 
		{
			CheckOverCorners(e.X, e.Y);
			return;
		}
		mouseDragPoint = new Point(e.X, e.Y);
		drawPanel.Invalidate();
	}
	
	private static Rectangle CheckOverCorners(int x, int y)
	{
		bool overCorner = false;
		Rectangle overBox = null;
		foreach(Rectangle box in boxes)
		{
			if(CheckOverCorner(box.X, box.Y, x, y))
				overCorner = true;
			else if(CheckOverCorner(box.X+box.Width, box.Y, x, y))
				overCorner = true;
			else if(CheckOverCorner(box.X, box.Y+box.Height, x, y))
				overCorner = true;
			else if(CheckOverCorner(box.X+box.Width, box.Y+box.Height, x, y))
				overCorner = true;
				
			if(overCorner)
			{
				overBox = box;
				break;
			}
		}
		if(overCorner)
		{
			drawPanel.Cursor = Cursors.Hand;
		}
		else
		{
			drawPanel.Cursor = Cursors.Arrow;
		}
		return overBox;
	}
	
	private static bool CheckOverCorner(int cornerX, int cornerY, int x, int y)
	{
		int margin = 5;
		return (x >= cornerX-margin && x <= cornerX+margin && y >= cornerY-margin && y <= cornerY+margin);
	}

	private static bool InDrag {
		get {
			return (mouseDownPoint != null);
		}
	}
	
	public static void Paint(object sender, PaintEventArgs pea)
	{
		Bitmap graphicsBuffer = new Bitmap(800, 800);
		Graphics g = Graphics.FromImage(graphicsBuffer);
		g.Clear(Color.LightGray);
		//g.SmoothingMode = SmoothingMode.AntiAlias;
		PaintPage(g, new Rectangle(15,15,600-30,graphicsBuffer.Height-30));
		if(mouseDownPoint != null && mouseDragPoint != null)
		{
			PaintDragBox(g, mouseDownPoint.Value, mouseDragPoint.Value);
		}
		foreach(Rectangle box in boxes)
		{
			PaintTextBox(g, box);
		}
		g.Dispose();
		pea.Graphics.DrawImageUnscaled(graphicsBuffer, 0, 0);
	}

	private static void PaintPage(Graphics g, Rectangle rect)
	{
		Pen pen = new Pen(Color.Gray, 1);
		Brush brush = new SolidBrush(Color.White);
		
		g.FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
		g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
	}
	
	private static void PaintDragBox(Graphics g, Point a, Point b)
	{
		Pen pen = new Pen(Color.LightBlue, 1);
		g.DrawRectangle(pen, Math.Min(a.X,b.X), Math.Min(a.Y,b.Y), Math.Abs(a.X-b.X), Math.Abs(a.Y-b.Y));
	}
	
	private static void PaintTextBox(Graphics g, Rectangle rect)
	{
		Pen pen = new Pen(Color.DarkGray, 1);
		g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
	}
}
