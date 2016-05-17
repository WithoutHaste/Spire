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

	private static List<Spire.Box> boxes;
	private static Spire.Box hoverBox;
	private static Spire.Box selectedBox;
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
		boxes = new List<Spire.Box>();
		boxes.Add(new Spire.ImageBox() {
			X = 200
			, Y = 100
			, Width = 365
			, Height = 240
			, Filename = "fireengine.gif"
		});
		boxes.Add(new Spire.ImageBox() {
			X = 350
			, Y = 370
			, Width = 134
			, Height = 153
			, Filename = "geisslertubes.gif"
		});
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
		else if(CheckOverSides(e.X, e.Y) || CheckHoverBox(e.X, e.Y))
		{
			if(hoverBox != null)
				selectedBox = hoverBox;
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
		string text = "";
		switch(boxes.Count)
		{
			case 2: text = "4. Logarithmic Casing\n5: Phase Plate"; break;
			case 3: text = "19. Lodusodelta Plate\nEvery seventh conductor being connected by a non-reversible turmy pipe to the differential girdle spring"; break;
			case 4: text = "16-18, 22. Transmission\nSupplies inverse reactive current and automatically synchronizes the caridnal grameters"; break;
			case 5: text = "10. Turbo-encabulator"; break;
			case 6: text = "12. Drawn Reciprocating Dingle Arm"; break;
			case 7: text = "7-9, 14. Girdle Spring\nProduces power by the modial interaction of magneto reluctance and capacitive directance"; break;
			case 8: text = "13. Panametric Fan\nPlaced in-line with the spurving bearings such that side fumbling is minimized"; break;
			case 9: text = "16. Unilateral Phase Detractors"; break;
			case 10: text = "- Charmed\n- Deelated\n- Elucidated\n- Spivy\n- Marzine\n- Hydrocoptic"; break;
		}
		boxes.Add(new Spire.TextBox() {
			X = Math.Min(mouseDownPoint.Value.X, e.X)
			, Y = Math.Min(mouseDownPoint.Value.Y, e.Y)
			, Width = Math.Abs(mouseDownPoint.Value.X - e.X)
			, Height = Math.Abs(mouseDownPoint.Value.Y - e.Y)
			, Text = text
		});
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
			else if(CheckOverSides(e.X, e.Y) || CheckHoverBox(e.X, e.Y))
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
		foreach(Spire.Box box in boxes)
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
		foreach(Spire.Box box in boxes)
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
	
	private static bool CheckHoverBox(int x, int y)
	{
		hoverBox = null;
		foreach(Spire.Box box in boxes)
		{
			if(x >= box.X && x <= box.X+box.Width && y >= box.Y && y <= box.Y+box.Height)
			{
				hoverBox = box;
				return true;
			}
		}
		return false;
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
		foreach(Spire.Box box in boxes)
		{
			if(box is Spire.ImageBox)
				PaintImage(g, box as Spire.ImageBox);
			else if(box is Spire.TextBox)
				PaintTextBox(g, box as Spire.TextBox);
		}
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
		g.Dispose();
		pea.Graphics.DrawImageUnscaled(graphicsBuffer, 0, 0);
	}

	private static void PaintImage(Graphics g, Spire.ImageBox box)
	{
		if(!File.Exists(box.Filename)) return;
			
		using (Image src = Image.FromFile(box.Filename))
		{
			g.DrawImage(src, box.X, box.Y, box.Width, box.Height);
			if(boxes.Count < 11)
			{
				Pen pen = new Pen(Color.Gray, 1);
				g.DrawRectangle(pen, box.X, box.Y, box.Width, box.Height);
			}
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
	
	private static void PaintTextBox(Graphics g, Spire.TextBox box)
	{
		if(boxes.Count < 11)
		{
			Pen pen = new Pen(Color.Gray, 1);
			g.DrawRectangle(pen, box.X, box.Y, box.Width, box.Height);
		}
		else
		{
			Brush brush = new SolidBrush(Color.Black);
			Font font = new Font("Times New Roman", 13);
			int lineHeight = (int)(g.MeasureString("TEST", font).Height);
			int y = box.Y;
			int maxWidth = 0;
			foreach(string line in box.GetLines())
			{
				g.DrawString(line, font, brush, box.X, y);
				y += lineHeight;
				maxWidth = Math.Max(maxWidth, (int)(g.MeasureString(line, font).Width));
			}
			box.Width = maxWidth + 5;
			box.Height = y + 3 - box.Y;
		}
	}
}

namespace Spire
{
	public abstract class Box
	{
		public int X;
		public int Y;
		public int Width;
		public int Height;
	}

	public class ImageBox : Box
	{
		public string Filename;
	}

	public class TextBox : Box
	{
		public string Text;
		private List<string> lines;
		
		public List<string> GetLines()
		{
			if(lines == null)
				lines = SplitLines(Text);
			return lines;
		}
	
		private List<string> SplitLines(string text)
		{
			int minLength = 15;
			int maxLength = 25;
			List<string> lines = new List<string>(text.Split('\n'));
			for(int i=1; i<lines.Count; i++)
			{
				if(lines[i].Length <= maxLength) continue;
				string[] words = lines[i].Split(' ');
				string newLine = "";
				for(int j=0; j<words.Length; j++)
				{
					if(newLine.Length < minLength)
						newLine += " " + words[j];
					else if(newLine.Length + words[j].Length <= maxLength)
						newLine += " " + words[j];
					else
					{
						lines.Insert(i, newLine);
						i++;
						newLine = "" + words[j];
					}
				}
				if(newLine != "")
				{
					lines.Insert(i, newLine);
					i++;
				}
				lines.RemoveAt(i);
			}
			return lines;
		}
	}
}