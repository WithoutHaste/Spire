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
	private enum Mode { AddBox, MoveBox, ResizeBox };

	private static Mode mode = Mode.AddBox;
	private static Panel drawPanel;
	
	private static Point? mouseDownPoint;
	private static Point? mouseDragPoint;

	private static List<Rectangle> boxes;
	private static Rectangle selectedBox;
	private static Point? selectedCorner;
	private static Point? oppositeSelectedCorner;

	public static void SetPanel(Panel panel)
	{
		if(drawPanel != null)
		{
			RemovePanel();
		}
		drawPanel = panel;
		boxes = new List<Rectangle>();
		ApplyMode(Mode.AddBox);
	}
	
	public static void RemovePanel()
	{
		if(drawPanel == null)
			return;
		RemoveListeners();
		drawPanel = null;
	}
	
	private static void RemoveListeners()
	{
		drawPanel.MouseDown -= AddMouseDown;
		drawPanel.MouseUp -= AddMouseUp;
		drawPanel.MouseMove -= AddMouseMove;
		drawPanel.MouseDown -= MoveMouseDown;
		drawPanel.MouseUp -= MoveMouseUp;
		drawPanel.MouseMove -= MoveMouseMove;
	}
	
	private static void ApplyMode(Mode m)
	{
		mode = m;
		RemoveListeners();
		switch(mode)
		{
			case Mode.AddBox:
				drawPanel.MouseDown += new MouseEventHandler(AddMouseDown);
				drawPanel.MouseUp += new MouseEventHandler(AddMouseUp);
				drawPanel.MouseMove += new MouseEventHandler(AddMouseMove);
				break;
			case Mode.MoveBox:
				drawPanel.MouseDown += new MouseEventHandler(MoveMouseDown);
				drawPanel.MouseUp += new MouseEventHandler(MoveMouseUp);
				drawPanel.MouseMove += new MouseEventHandler(MoveMouseMove);
				break;
			case Mode.ResizeBox:
				break;
		}
	}
	
	private static void AddMouseDown(object sender, MouseEventArgs e)
	{
		if(CheckOverCorners(e.X, e.Y))
		{
			ApplyMode(Mode.MoveBox);
			MoveMouseDown(sender, e);
		}
		else
		{
			mouseDownPoint = new Point(e.X, e.Y);
		}
	}
	
	private static void AddMouseUp(object sender, MouseEventArgs e)
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
	
	private static void AddMouseMove(object sender, MouseEventArgs e)
	{
		if(mouseDownPoint == null) 
		{
			if(CheckOverCorners(e.X, e.Y))
			{
				if(selectedCorner.Value.X == selectedBox.X && selectedCorner.Value.Y == selectedBox.Y)
					drawPanel.Cursor = Cursors.SizeNWSE;
				else if(selectedCorner.Value.X == selectedBox.X+selectedBox.Width && selectedCorner.Value.Y == selectedBox.Y)
					drawPanel.Cursor = Cursors.SizeNESW;
				else if(selectedCorner.Value.X == selectedBox.X && selectedCorner.Value.Y == selectedBox.Y+selectedBox.Height)
					drawPanel.Cursor = Cursors.SizeNESW;		
				else if(selectedCorner.Value.X == selectedBox.X+selectedBox.Width && selectedCorner.Value.Y == selectedBox.Y+selectedBox.Height)
					drawPanel.Cursor = Cursors.SizeNWSE;
			}
			else if(CheckOverSide(e.X, e.Y)
			{
				drawPanel.Cursor = Cursors.Hand;
			}
			else
			{
				drawPanel.Cursor = Cursors.Arrow;
			}
			return;
		}
		mouseDragPoint = new Point(e.X, e.Y);
		drawPanel.Invalidate();
	}
	
	private static void MoveMouseDown(object sender, MouseEventArgs e)
	{
	}
	
	private static void MoveMouseUp(object sender, MouseEventArgs e)
	{
		selectedBox.X = Math.Min(oppositeSelectedCorner.Value.X, e.X);
		selectedBox.Y = Math.Min(oppositeSelectedCorner.Value.Y, e.Y);
		selectedBox.Width = Math.Abs(oppositeSelectedCorner.Value.X - e.X);
		selectedBox.Height = Math.Abs(oppositeSelectedCorner.Value.Y - e.Y);
		drawPanel.Cursor = Cursors.Arrow;
		ApplyMode(Mode.AddBox);
		drawPanel.Invalidate();
	}
	
	private static void MoveMouseMove(object sender, MouseEventArgs e)
	{
		mouseDragPoint = new Point(e.X, e.Y);
		drawPanel.Invalidate();
	}
	
	private static bool CheckOverSide(int x, int y)
	{
		return false;
	}
	
	private static bool CheckOverCorners(int x, int y)
	{
		selectedBox = null;
		selectedCorner = null;
		oppositeSelectedCorner = null;
		foreach(Rectangle box in boxes)
		{
			if(CheckOverCorner(box.X, box.Y, x, y))
			{
				selectedCorner = new Point(box.X, box.Y);
				oppositeSelectedCorner = new Point(box.X+box.Width, box.Y+box.Height);
			}
			else if(CheckOverCorner(box.X+box.Width, box.Y, x, y))
			{
				selectedCorner = new Point(box.X+box.Width, box.Y);
				oppositeSelectedCorner = new Point(box.X, box.Y+box.Height);
			}
			else if(CheckOverCorner(box.X, box.Y+box.Height, x, y))
			{
				selectedCorner = new Point(box.X, box.Y+box.Height);
				oppositeSelectedCorner = new Point(box.X+box.Width, box.Y);
			}
			else if(CheckOverCorner(box.X+box.Width, box.Y+box.Height, x, y))
			{
				selectedCorner = new Point(box.X+box.Width, box.Y+box.Height);
				oppositeSelectedCorner = new Point(box.X, box.Y);
			}
				
			if(selectedCorner != null)
			{
				selectedBox = box;
				break;
			}
		}
		return (selectedBox != null);
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
		if(mode == Mode.AddBox)
		{
			if(mouseDownPoint != null && mouseDragPoint != null)
			{
				PaintDragBox(g, mouseDownPoint.Value, mouseDragPoint.Value);
			}
		}
		else if(mode == Mode.MoveBox)
		{
			if(mouseDragPoint != null)
			{
				PaintDragBox(g, oppositeSelectedCorner.Value, mouseDragPoint.Value);
			}
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
	
	private static void PaintDragBox(Graphics g, int x, int y, int width, int height)
	{
		PaintDragBox(g, new Point(x, y), new Point(x+width, y+height));
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
