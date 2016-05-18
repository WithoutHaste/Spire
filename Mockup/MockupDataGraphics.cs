using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

public static class DataGraphicsDemo
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
		ApplyMode(Mode.AddBox);
		
		int margin = 50;
		int middleSpacer = 25;
		
		TextBox text1 = MockupWindow.BuildTextInput();
		text1.Font = new Font("Times New Roman", 12);
		text1.Text = "      Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis ornare mauris id venenatis pretium. Quisque ac consequat justo, pretium laoreet mi. Ut pharetra, felis a porttitor porttitor, tortor nisi ultrices nisl, nec vehicula turpis erat ut ante. Donec maximus blandit felis eget cursus. Nunc dignissim feugiat purus, id maximus elit. Fusce eleifend dolor eu ante interdum pellentesque. Quisque malesuada congue ligula, at scelerisque risus convallis vitae. Pellentesque gravida laoreet consequat.";
		text1.Top = margin;
		text1.Left = margin;
		text1.Width = (drawPanel.Width - margin - margin - middleSpacer) / 2;
		text1.Height = 175;
		text1.Parent = drawPanel;

		TextBox text2 = MockupWindow.BuildTextInput();
		text2.Font = text1.Font;
		text2.Text = "      Vivamus rutrum nulla diam, in condimentum lacus mattis a. Donec ante sapien, molestie at convallis vel, tincidunt nec nibh. Vivamus ac commodo odio. Aliquam maximus, lacus sit amet ornare efficitur, dolor justo condimentum risus, nec posuere massa nunc vel tortor. Interdum et malesuada fames ac ante ipsum primis in faucibus. Sed vitae enim fermentum, molestie sem vel, dictum felis. Donec purus risus, volutpat luctus justo non, interdum tristique ante.";
		text2.Top = EasyLayout.Below(text1, 0);
		text2.Left = text1.Left;
		text2.Width = text1.Width;
		text2.Height = 175;
		text2.Parent = drawPanel;

		TextBox text3 = MockupWindow.BuildTextInput();
		text3.Font = text1.Font;
		text3.Text = "      Proin sed dapibus turpis. Cras eget nulla mauris. Aliquam lobortis euismod risus id dapibus. Cras iaculis fermentum malesuada. In interdum erat vel semper auctor. Ut vitae sapien ac nunc elementum hendrerit venenatis eu purus. Aenean efficitur, massa vel ultricies sollicitudin, ante sapien suscipit sapien, eu porta sapien arcu sed turpis. Quisque mollis ultricies arcu, nec pretium nisi auctor sed. Integer quis pulvinar massa. Pellentesque porttitor lorem eget massa mollis pulvinar. Nullam in quam ac risus ultricies accumsan. Sed nec dui sed lorem viverra viverra. Curabitur velit justo, porta vel quam elementum, finibus aliquam ipsum.";
		text3.Top = EasyLayout.Below(text2, 0);
		text3.Left = text1.Left;
		text3.Width = text1.Width;
		text3.Height = 300;
		text3.Parent = drawPanel;
		
		TextBox text4 = MockupWindow.BuildTextInput();
		text4.Font = text1.Font;
		text4.Text = "ligula, in tristique libero tristique a. Nullam eget sapien mi. Aenean pretium, magna sed dictum hendrerit, libero ipsum venenatis lorem, non fringilla metus tortor in nibh. Aenean tempus, justo a sollicitudin tristique, libero dolor faucibus nisi, eget ullamcorper justo tortor ut arcu. Donec quis nisi id turpis feugiat ornare tempus a elit. Vestibulum eu lorem consequat, lacinia velit eu, maximus ex. Quisque dictum nibh ac libero tempor, eu congue sem viverra. Aliquam feugiat, felis quis consequat elementum, erat lacus tincidunt purus, at rhoncus purus justo eget nulla. Donec sed ipsum egestas, aliquet nibh sed, mollis mauris. Sed fermentum sodales velit ut malesuada. Morbi sed est neque.";
		text4.Top = text1.Top + 150;
		text4.Left = EasyLayout.LeftOf(text1, middleSpacer);
		text4.Width = text1.Width - 15;
		text4.Height = 175;
		text4.Parent = drawPanel;
		
		TextBox text5 = MockupWindow.BuildTextInput();
		text5.Font = text1.Font;
		text5.Text = "      Nulla rutrum, odio sed sodales imperdiet, nibh lacus lacinia tortor, eget sagittis nibh magna nec dolor. Duis ultricies arcu id maximus finibus. Mauris porta elit non mi molestie pellentesque sit amet sed neque. Praesent vitae erat tempor, placerat nisl vel, tempor orci. Mauris consectetur lectus nec metus cursus, id tincidunt turpis malesuada. Phasellus at neque orci. Sed accumsan placerat arcu sed sodales. Morbi a urna sem. Pellentesque ultricies dignissim leo eget mattis.";
		text5.Top = EasyLayout.Below(text4, 0);
		text5.Left = text4.Left;
		text5.Width = text4.Width;
		text5.Height = 175;
		text5.Parent = drawPanel;
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
	
	public static ContextMenu BuildContextMenu()
	{
		ContextMenu menu = new ContextMenu();
		MenuItem paste = new MenuItem() { Text="Paste Image" };
		MenuItem image = new MenuItem() { Text="Import Image"};
		MenuItem dataGraphic = new MenuItem() { Text="Create Data Graphic" };
		dataGraphic.Click += new EventHandler(OpenDataGraphicsModal);
		menu.MenuItems.Add(paste);
		menu.MenuItems.Add(image);
		menu.MenuItems.Add(dataGraphic);
		return menu;
	}
	
	private static void OpenDataGraphicsModal(object sender, EventArgs e)
	{
		DataGraphicsDialog dialog = new DataGraphicsDialog();
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
		boxes.Add(new Spire.TextBox() {
			X = Math.Min(mouseDownPoint.Value.X, e.X)
			, Y = Math.Min(mouseDownPoint.Value.Y, e.Y)
			, Width = Math.Abs(mouseDownPoint.Value.X - e.X)
			, Height = Math.Abs(mouseDownPoint.Value.Y - e.Y)
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
		PaintPage(g, new Rectangle(15,15,800-30,600));
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

public class DataGraphicsDialog : Form
{
	private ListBox styleBox;
	private ComboBox fontFamilyBox;
	private TextBox fontSizeInput;
	private TextBox foreColorInput;
	private Button foreColorPreview;
	private CheckBox boldCheckBox;
	private CheckBox italicCheckBox;
	private ComboBox alignmentBox;
	private TextBox indentInput;
	
	public DataGraphicsDialog()
	{
		Width = 700;
		Height = 500;
		Text = "Data Graphics";
		Icon = new Icon("SpireIcon1.ico");
		/*
		styleBox = new ListBox();
		styleBox.DataSource = new List<string>() { "Header 1", "Header 2", "Header 3", "Text", "Section Start", "Paragraph Start", "Quotes", "Footnotes", "Sidenotes", "Formula" };
		styleBox.Left = 15;
		styleBox.Top = 15;
		styleBox.Height = 200;
		styleBox.SelectedIndexChanged += new EventHandler(StyleSelectedChanged);
		styleBox.Parent = this;
		
		Button addStyle = new Button();
		addStyle.Text = "Add Style";
		addStyle.Left = styleBox.Left;
		addStyle.Top = styleBox.Top + styleBox.Height + 5;
		addStyle.Width = styleBox.Width;
		addStyle.Parent = this;
		
		int leftLabels = styleBox.Left + styleBox.Width + 25;
		int leftInputs = leftLabels + 40;
		
		Label fontFamilyLabel = new Label();
		fontFamilyLabel.Left = leftLabels;
		fontFamilyLabel.Top = styleBox.Top;
		fontFamilyLabel.Width = 30;
		fontFamilyLabel.Text = "Font";
		fontFamilyLabel.TextAlign = ContentAlignment.TopLeft;
		fontFamilyLabel.Parent = this;		
		
		fontFamilyBox = new ComboBox();
		fontFamilyBox.DataSource = new List<string>() { "Arial", "Courier", "Times New Roman" };
		fontFamilyBox.Left = leftInputs;
		fontFamilyBox.Top = fontFamilyLabel.Top - 2;
		fontFamilyBox.Height = 25;
		fontFamilyBox.Parent = this;
		fontFamilyBox.SelectedIndexChanged += new EventHandler(FontFamilyChanged);
		
		Label fontSizeLabel = new Label();
		fontSizeLabel.Left = leftLabels;
		fontSizeLabel.Top = fontFamilyLabel.Top + fontFamilyLabel.Height + 5;
		fontSizeLabel.Width = 30;
		fontSizeLabel.Text = "Size";
		fontSizeLabel.TextAlign = ContentAlignment.TopLeft;
		fontSizeLabel.Parent = this;		
		
		fontSizeInput = new TextBox();
		fontSizeInput.Left = leftInputs;
		fontSizeInput.Top = fontSizeLabel.Top - 2;
		fontSizeInput.Height = 25;
		fontSizeInput.Width = 40;
		fontSizeInput.LostFocus += new EventHandler(FontSizeChanged);
		fontSizeInput.Parent = this;
		
		Label foreColorLabel = new Label();
		foreColorLabel.Left = leftLabels;
		foreColorLabel.Top = fontSizeLabel.Top + fontSizeLabel.Height + 5;
		foreColorLabel.Width = 35;
		foreColorLabel.Text = "Color";
		foreColorLabel.TextAlign = ContentAlignment.TopLeft;
		foreColorLabel.Parent = this;		
		
		foreColorInput = new TextBox();
		foreColorInput.Left = leftInputs;
		foreColorInput.Top = foreColorLabel.Top - 2;
		foreColorInput.Height = 25;
		foreColorInput.Width = 120;
		foreColorInput.LostFocus += new EventHandler(ForeColorStyleChanged);
		foreColorInput.Parent = this;
		
		foreColorPreview = new Button();
		foreColorPreview.Left = foreColorInput.Left + foreColorInput.Width + 5;
		foreColorPreview.Top = foreColorInput.Top;
		foreColorPreview.Height = 20;
		foreColorPreview.Width = 40;
		foreColorPreview.FlatStyle = FlatStyle.Flat;
		foreColorPreview.Parent = this;
		
		Label boldLabel = new Label();
		boldLabel.Left = leftLabels;
		boldLabel.Top = foreColorLabel.Top + foreColorLabel.Height + 5;
		boldLabel.Width = 35;
		boldLabel.Text = "Bold";
		boldLabel.TextAlign = ContentAlignment.TopLeft;
		boldLabel.Parent = this;		
		
		boldCheckBox = new CheckBox();
		boldCheckBox.Left = boldLabel.Left + boldLabel.Width + 3;
		boldCheckBox.Top = boldLabel.Top - 4;
		boldCheckBox.Width = 10;
		boldCheckBox.CheckedChanged += new EventHandler(BoldChanged);
		boldCheckBox.Parent = this;
		
		Label italicLabel = new Label();
		italicLabel.Left = boldCheckBox.Left + boldCheckBox.Width + 10;
		italicLabel.Top = boldLabel.Top;
		italicLabel.Width = 35;
		italicLabel.Text = "Italic";
		italicLabel.TextAlign = ContentAlignment.TopLeft;
		italicLabel.Parent = this;		
		
		italicCheckBox = new CheckBox();
		italicCheckBox.Left = italicLabel.Left + italicLabel.Width + 3;
		italicCheckBox.Top = boldCheckBox.Top;
		italicCheckBox.CheckedChanged += new EventHandler(ItalicChanged);
		italicCheckBox.Parent = this;
		
		Label alignmentLabel = new Label();
		alignmentLabel.Left = leftLabels;
		alignmentLabel.Top = boldLabel.Top + boldLabel.Height + 5;
		alignmentLabel.Width = 30;
		alignmentLabel.Text = "Align";
		alignmentLabel.TextAlign = ContentAlignment.TopLeft;
		alignmentLabel.Parent = this;		
		
		alignmentBox = new ComboBox();
		alignmentBox.DataSource = new List<string>() { "Left", "Center", "Right" };
		alignmentBox.Left = leftInputs;
		alignmentBox.Top = alignmentLabel.Top - 2;
		alignmentBox.Height = 25;
		alignmentBox.Parent = this;		
		
		Label indentLabel = new Label();
		indentLabel.Left = leftLabels;
		indentLabel.Top = alignmentLabel.Top + alignmentLabel.Height + 5;
		indentLabel.Width = 40;
		indentLabel.Text = "Indent";
		indentLabel.TextAlign = ContentAlignment.TopLeft;
		indentLabel.Parent = this;		
		
		indentInput = new TextBox();
		indentInput.Left = leftInputs;
		indentInput.Top = indentLabel.Top - 2;
		indentInput.Height = 25;
		indentInput.Width = 30;
		indentInput.LostFocus += new EventHandler(IndentChanged);
		indentInput.Parent = this;		
		
		Button close = new Button();
		close.Text = "Close";
		close.Left = leftInputs;
		close.Top = addStyle.Top;
		close.Width = 75;
		close.Click += (sender, e) => { this.Close(); };
		close.Parent = this;

		this.Load += new EventHandler(StyleSelectedChanged);
	*/	
		ShowDialog();
	}
}
