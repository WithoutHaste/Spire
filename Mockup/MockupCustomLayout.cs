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
	private enum Side { Top, Bottom, Left, Right };

	private static Mode mode = Mode.AddBox;
	private static Panel drawPanel;
	
	private static Point? mouseDownPoint;
	private static Point? mouseDragPoint;

	private static List<Rectangle> boxes;
	private static Rectangle selectedBox;
	private static Point? selectedCorner;
	private static Point? oppositeSelectedCorner;
	private static Side? selectedSide;

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
		drawPanel.MouseDown -= ResizeMouseDown;
		drawPanel.MouseUp -= ResizeMouseUp;
		drawPanel.MouseMove -= ResizeMouseMove;
	}
	
	private static void ApplyMode(Mode m)
	{
		mode = m;
		RemoveListeners();
		switch(mode)
		{
			case Mode.AddBox:
				mouseDownPoint = null;
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
				drawPanel.MouseDown += new MouseEventHandler(ResizeMouseDown);
				drawPanel.MouseUp += new MouseEventHandler(ResizeMouseUp);
				drawPanel.MouseMove += new MouseEventHandler(ResizeMouseMove);
				break;
		}
	}
	
	private static void AddMouseDown(object sender, MouseEventArgs e)
	{
		if(CheckOverCorners(e.X, e.Y))
		{
			ApplyMode(Mode.ResizeBox);
			ResizeMouseDown(sender, e);
		}
		else if(CheckOverSides(e.X, e.Y))
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
			else if(CheckOverSides(e.X, e.Y))
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
	
	private static void ResizeMouseDown(object sender, MouseEventArgs e)
	{
	}
	
	private static void ResizeMouseUp(object sender, MouseEventArgs e)
	{
		selectedBox.X = Math.Min(oppositeSelectedCorner.Value.X, e.X);
		selectedBox.Y = Math.Min(oppositeSelectedCorner.Value.Y, e.Y);
		selectedBox.Width = Math.Abs(oppositeSelectedCorner.Value.X - e.X);
		selectedBox.Height = Math.Abs(oppositeSelectedCorner.Value.Y - e.Y);
		drawPanel.Cursor = Cursors.Arrow;
		ApplyMode(Mode.AddBox);
		drawPanel.Invalidate();
	}
	
	private static void ResizeMouseMove(object sender, MouseEventArgs e)
	{
		mouseDragPoint = new Point(e.X, e.Y);
		drawPanel.Invalidate();
	}
	
	private static void MoveMouseDown(object sender, MouseEventArgs e)
	{
		mouseDownPoint = new Point(e.X, e.Y);
	}
	
	private static void MoveMouseUp(object sender, MouseEventArgs e)
	{
		int deltaX = e.X - mouseDownPoint.Value.X;
		int deltaY = e.Y - mouseDownPoint.Value.Y;
		selectedBox.X += deltaX;
		selectedBox.Y += deltaY;
		drawPanel.Cursor = Cursors.Arrow;
		ApplyMode(Mode.AddBox);
		drawPanel.Invalidate();
	}
	
	private static void MoveMouseMove(object sender, MouseEventArgs e)
	{
		mouseDragPoint = new Point(e.X, e.Y);
		drawPanel.Invalidate();
	}
	
	private static bool CheckOverSides(int x, int y)
	{
		selectedBox = null;
		selectedSide = null;
		foreach(Rectangle box in boxes)
		{
			if(CheckOverSideHorizontal(box.X, box.Y, box.X+box.Width, box.Y, x, y))
			{
				selectedSide = Side.Top;
			}
			else if(CheckOverSideVertical(box.X, box.Y, box.X, box.Y+box.Height, x, y))
			{
				selectedSide = Side.Left;
			}
			else if(CheckOverSideHorizontal(box.X+box.Width, box.Y+box.Height, box.X, box.Y+box.Height, x, y))
			{
				selectedSide = Side.Bottom;
			}
			else if(CheckOverSideVertical(box.X+box.Width, box.Y+box.Height, box.X+box.Width, box.Y, x, y))
			{
				selectedSide = Side.Right;
			}
			
			if(selectedSide != null)
			{
				selectedBox = box;
				break;
			}
		}
			
		return (selectedBox != null);
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
	
	private static bool CheckOverSideHorizontal(int cornerAX, int cornerAY, int cornerBX, int cornerBY, int x, int y)
	{
		int margin = 5;
		return (x >= Math.Min(cornerAX, cornerBX) && x <= Math.Max(cornerAX, cornerBX) && y >= cornerAY-margin && y <= cornerAY+margin);
	}

	private static bool CheckOverSideVertical(int cornerAX, int cornerAY, int cornerBX, int cornerBY, int x, int y)
	{
		int margin = 5;
		return (y >= Math.Min(cornerAY, cornerBY) && y <= Math.Max(cornerAY, cornerBY) && x >= cornerAX-margin && x <= cornerAX+margin);
	}

	public static void Paint(object sender, PaintEventArgs pea)
	{
		Bitmap graphicsBuffer = new Bitmap(drawPanel.Width, drawPanel.Height);
		Graphics g = Graphics.FromImage(graphicsBuffer);
		g.Clear(Color.LightGray);
		//g.SmoothingMode = SmoothingMode.AntiAlias;
		PaintPage(g, new Rectangle(15,15,800-30,550-30));
		if(mode == Mode.AddBox)
		{
			if(mouseDownPoint != null && mouseDragPoint != null)
			{
				PaintDragBox(g, mouseDownPoint.Value, mouseDragPoint.Value);
			}
		}
		else if(mode == Mode.ResizeBox)
		{
			if(mouseDragPoint != null)
			{
				PaintDragBox(g, oppositeSelectedCorner.Value, mouseDragPoint.Value);
			}
		}
		else if(mode == Mode.MoveBox)
		{
			if(mouseDownPoint != null && mouseDragPoint != null)
			{
				int deltaX = mouseDragPoint.Value.X - mouseDownPoint.Value.X;
				int deltaY = mouseDragPoint.Value.Y - mouseDownPoint.Value.Y;
				PaintDragBox(g, selectedBox.X+deltaX, selectedBox.Y+deltaY, selectedBox.Width, selectedBox.Height);
			}
		}
		foreach(Rectangle box in boxes)
		{
			PaintTextBox(g, box);
		}
		PaintImage(g, new Rectangle(300,200,365,240), "fireengine.gif");
		PaintImage(g, new Rectangle(50,100,134,153), "geisslertubes.gif");
		g.Dispose();
		pea.Graphics.DrawImageUnscaled(graphicsBuffer, 0, 0);
	}

	private static void PaintImage(Graphics g, Rectangle rect, string filename)
	{
		if(!File.Exists(filename)) return;
			
		using (Image src = Image.FromFile(filename))
		{
			g.DrawImage(src, rect.X, rect.Y, rect.Width, rect.Height);
		}
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
