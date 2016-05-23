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
	private TextBox dataSourceControl;
	private ListBox graphTypeControl;
	private OpenFileDialog openFileDialog;

	private PictureBox sampleGraphA;
	private PictureBox sampleGraphB;
	private PictureBox sampleGraphC;
	private PictureBox sampleGraphD;
	private Dictionary<string, List<Bitmap>> sampleGraphImages;
	
	private Label previewLabel;
	private PictureBox previewGraph;
	private Label plotWithColumnLabel;
	private ComboBox plotWithColumnControl;
	
	public DataGraphicsDialog()
	{
		Width = 800;
		Height = 700;
		Text = "Data Graphics";
		Icon = new Icon("SpireIcon1.ico");

		Label dataSourceLabel = new Label();
		dataSourceLabel.Left = 15;
		dataSourceLabel.Top = 15;
		dataSourceLabel.Width = 100;
		dataSourceLabel.Text = "Data Source";
		dataSourceLabel.TextAlign = ContentAlignment.TopLeft;
		dataSourceLabel.Parent = this;		
		
		dataSourceControl = new TextBox();
		dataSourceControl.Left = dataSourceLabel.Left;
		dataSourceControl.Top = EasyLayout.Below(dataSourceLabel, 0);
		dataSourceControl.Height = 25;
		dataSourceControl.Width = 300;
		dataSourceControl.Parent = this;

		Button selectFileButton = new Button();
		selectFileButton.Text = "Select File";
		selectFileButton.Left = EasyLayout.LeftOf(dataSourceControl, 5);
		selectFileButton.Top = dataSourceControl.Top - 2;
		selectFileButton.Width = 100;
		selectFileButton.Click += new EventHandler(SelectFile);
		selectFileButton.Parent = this;

		Label graphTypeLabel = new Label();
		graphTypeLabel.Left = dataSourceLabel.Left;
		graphTypeLabel.Top = EasyLayout.Below(dataSourceControl, 15);
		graphTypeLabel.Width = 100;
		graphTypeLabel.Text = "Graph Type";
		graphTypeLabel.TextAlign = ContentAlignment.TopLeft;
		graphTypeLabel.Parent = this;		
		
		graphTypeControl = new ListBox();
		graphTypeControl.DataSource = new List<string>() { "Line Graph", "Scatterplot", "Rug Plot", "Geographical Map", "Heat Map", "Bar Graph", "Histogram", "Box Plot", "Stem and Leaf", "Sparkline", "Slant Graph", "Tree Map", "Cosmograph" };
		graphTypeControl.Left = graphTypeLabel.Left;
		graphTypeControl.Top = EasyLayout.Below(graphTypeLabel, 0);
		graphTypeControl.Height = 200;
		graphTypeControl.Width = 100;
		graphTypeControl.SelectedIndexChanged += new EventHandler(GraphTypeChanged);
		graphTypeControl.Parent = this;
		
		int imageBuffer = 15;
		
		sampleGraphA = new PictureBox();
		sampleGraphA.SizeMode = PictureBoxSizeMode.Zoom;
		sampleGraphA.Width = 200;
		sampleGraphA.Height = 150;
		sampleGraphA.Left = EasyLayout.LeftOf(graphTypeControl, imageBuffer);
		sampleGraphA.Top = graphTypeControl.Top + imageBuffer;
		sampleGraphA.Parent = this;
		
		sampleGraphB = new PictureBox();
		sampleGraphB.SizeMode = PictureBoxSizeMode.Zoom;
		sampleGraphB.Width = sampleGraphA.Width;
		sampleGraphB.Height = sampleGraphA.Height;
		sampleGraphB.Left = EasyLayout.LeftOf(sampleGraphA, imageBuffer);
		sampleGraphB.Top = sampleGraphA.Top;
		sampleGraphB.Parent = this;
		
		sampleGraphC = new PictureBox();
		sampleGraphC.SizeMode = PictureBoxSizeMode.Zoom;
		sampleGraphC.Width = sampleGraphA.Width;
		sampleGraphC.Height = sampleGraphA.Height;
		sampleGraphC.Left = sampleGraphA.Left;
		sampleGraphC.Top = EasyLayout.Below(sampleGraphA, imageBuffer);
		sampleGraphC.Parent = this;
		
		sampleGraphD = new PictureBox();
		sampleGraphD.SizeMode = PictureBoxSizeMode.Zoom;
		sampleGraphD.Width = sampleGraphA.Width;
		sampleGraphD.Height = sampleGraphA.Height;
		sampleGraphD.Left = sampleGraphB.Left;
		sampleGraphD.Top = sampleGraphC.Top;
		sampleGraphD.Parent = this;
		
		///////////////////////////////////////////////
		
		Label xAxisColumnLabel = new Label();
		xAxisColumnLabel.Left = graphTypeLabel.Left;
		xAxisColumnLabel.Top = EasyLayout.Below(graphTypeControl, 15);
		xAxisColumnLabel.Width = 115;
		xAxisColumnLabel.Text = "X Axis Data Column";
		xAxisColumnLabel.TextAlign = ContentAlignment.TopLeft;
		xAxisColumnLabel.Parent = this;		
		
		ComboBox xAxisColumnControl = new ComboBox();
		xAxisColumnControl.DataSource = new List<string>() { "", "Month", "Births", "Month Label", "100K Births" };
		xAxisColumnControl.Left = EasyLayout.LeftOf(xAxisColumnLabel, 5);
		xAxisColumnControl.Top = xAxisColumnLabel.Top - 2;
		xAxisColumnControl.Height = 25;
		xAxisColumnControl.Width = 150;
		xAxisColumnControl.Parent = this;

		Label yAxisColumnLabel = new Label();
		yAxisColumnLabel.Left = xAxisColumnLabel.Left;
		yAxisColumnLabel.Top = EasyLayout.Below(xAxisColumnLabel, 5);
		yAxisColumnLabel.Width = 115;
		yAxisColumnLabel.Text = "Y Axis Data Column";
		yAxisColumnLabel.TextAlign = ContentAlignment.TopLeft;
		yAxisColumnLabel.Parent = this;		
		
		ComboBox yAxisColumnControl = new ComboBox();
		yAxisColumnControl.DataSource = new List<string>() { "", "Month", "Births", "Month Label", "100K Births" };
		yAxisColumnControl.Left = EasyLayout.LeftOf(yAxisColumnLabel, 5);
		yAxisColumnControl.Top = yAxisColumnLabel.Top - 2;
		yAxisColumnControl.Height = 25;
		yAxisColumnControl.Width = 150;
		yAxisColumnControl.SelectedIndexChanged += new EventHandler(AxisColumnsSet);
		yAxisColumnControl.Parent = this;

		Label xAxisLabelLabel = new Label();
		xAxisLabelLabel.Left = graphTypeLabel.Left;
		xAxisLabelLabel.Top = EasyLayout.Below(yAxisColumnLabel, 15);
		xAxisLabelLabel.Width = 75;
		xAxisLabelLabel.Text = "X Axis Label";
		xAxisLabelLabel.TextAlign = ContentAlignment.TopLeft;
		xAxisLabelLabel.Parent = this;		
		
		TextBox xAxisLabelControl = new TextBox();
		xAxisLabelControl.Left = EasyLayout.LeftOf(xAxisLabelLabel, 5);
		xAxisLabelControl.Top = xAxisLabelLabel.Top - 2;
		xAxisLabelControl.Height = 25;
		xAxisLabelControl.Width = 150;
		xAxisLabelControl.Parent = this;

		Label yAxisLabelLabel = new Label();
		yAxisLabelLabel.Left = graphTypeLabel.Left;
		yAxisLabelLabel.Top = EasyLayout.Below(xAxisLabelLabel, 15);
		yAxisLabelLabel.Width = 75;
		yAxisLabelLabel.Text = "Y Axis Label";
		yAxisLabelLabel.TextAlign = ContentAlignment.TopLeft;
		yAxisLabelLabel.Parent = this;		
		
		TextBox yAxisLabelControl = new TextBox();
		yAxisLabelControl.Left = EasyLayout.LeftOf(yAxisLabelLabel, 5);
		yAxisLabelControl.Top = yAxisLabelLabel.Top - 2;
		yAxisLabelControl.Height = 25;
		yAxisLabelControl.Width = 150;
		yAxisLabelControl.LostFocus += new EventHandler(AxisLabelsSet);
		yAxisLabelControl.Parent = this;

		previewLabel = new Label();
		previewLabel.Left = EasyLayout.LeftOf(xAxisColumnControl, 20);
		previewLabel.Top = xAxisColumnLabel.Top;
		previewLabel.Width = 100;
		previewLabel.Text = "Graph Preview";
		previewLabel.TextAlign = ContentAlignment.TopLeft;
		previewLabel.Visible = false;
		previewLabel.Parent = this;
		
		previewGraph = new PictureBox();
		previewGraph.SizeMode = PictureBoxSizeMode.Zoom;
		previewGraph.Width = 400;
		previewGraph.Height = 300;
		previewGraph.Left = previewLabel.Left;
		previewGraph.Top = EasyLayout.Below(previewLabel, 5);
		previewGraph.Visible = false;
		previewGraph.Parent = this;

		Label plotWithLabel = new Label();
		plotWithLabel.Left = yAxisLabelLabel.Left;
		plotWithLabel.Top = EasyLayout.Below(yAxisLabelLabel, 5);
		plotWithLabel.Width = 50;
		plotWithLabel.Text = "Plot With";
		plotWithLabel.TextAlign = ContentAlignment.TopLeft;
		plotWithLabel.Parent = this;		
		
		ListBox plotWithControl = new ListBox();
		plotWithControl.DataSource = new List<string>() { "Dot", "Label", "Image" };
		plotWithControl.Left = graphTypeLabel.Left;
		plotWithControl.Top = EasyLayout.Below(plotWithLabel, 0);
		plotWithControl.Height = 50;
		plotWithControl.Width = 100;
		plotWithControl.SelectedIndexChanged += new EventHandler(PlotWithSet);
		plotWithControl.Parent = this;
		
		plotWithColumnLabel = new Label();
		plotWithColumnLabel.Left = EasyLayout.LeftOf(plotWithControl, 5);
		plotWithColumnLabel.Top = plotWithLabel.Top;
		plotWithColumnLabel.Width = 115;
		plotWithColumnLabel.Text = "Plot Label Column";
		plotWithColumnLabel.TextAlign = ContentAlignment.TopLeft;
		plotWithColumnLabel.Visible = false;
		plotWithColumnLabel.Parent = this;		
		
		plotWithColumnControl = new ComboBox();
		plotWithColumnControl.DataSource = new List<string>() { "", "Month", "Births", "Month Label", "100K Births" };
		plotWithColumnControl.Left = plotWithColumnLabel.Left;
		plotWithColumnControl.Top = EasyLayout.Below(plotWithColumnLabel, 0);
		plotWithColumnControl.Height = 25;
		plotWithColumnControl.Width = 150;
		plotWithColumnControl.SelectedIndexChanged += new EventHandler(PlotWithColumnSet);
		plotWithColumnControl.Visible = false;
		plotWithColumnControl.Parent = this;

		Label yAxisRangeLabel = new Label();
		yAxisRangeLabel.Left = plotWithLabel.Left;
		yAxisRangeLabel.Top = EasyLayout.Below(plotWithControl, 5);
		yAxisRangeLabel.Width = 75;
		yAxisRangeLabel.Text = "Y Axis Range";
		yAxisRangeLabel.TextAlign = ContentAlignment.TopLeft;
		yAxisRangeLabel.Parent = this;		
		
		ListBox yAxisRangeControl = new ListBox();
		yAxisRangeControl.DataSource = new List<string>() { "0 - Max", "Min - Max" };
		yAxisRangeControl.Left = yAxisRangeLabel.Left;
		yAxisRangeControl.Top = EasyLayout.Below(yAxisRangeLabel, 0);
		yAxisRangeControl.Height = 40;
		yAxisRangeControl.Width = 75;
		yAxisRangeControl.SelectedIndexChanged += new EventHandler(YAxisRangeSet);
		yAxisRangeControl.Parent = this;

		Label yAxisTicksLabel = new Label();
		yAxisTicksLabel.Left = EasyLayout.LeftOf(yAxisRangeControl, 10);
		yAxisTicksLabel.Top = yAxisRangeLabel.Top;
		yAxisTicksLabel.Width = 75;
		yAxisTicksLabel.Text = "Y Axis Ticks";
		yAxisTicksLabel.TextAlign = ContentAlignment.TopLeft;
		yAxisTicksLabel.Parent = this;		
		
		ListBox yAxisTicksControl = new ListBox();
		yAxisTicksControl.DataSource = new List<string>() { "Interval", "Exact" };
		yAxisTicksControl.Left = yAxisTicksLabel.Left;
		yAxisTicksControl.Top = yAxisRangeControl.Top;
		yAxisTicksControl.Height = 40;
		yAxisTicksControl.Width = 75;
		yAxisTicksControl.SelectedIndexChanged += new EventHandler(YAxisTicksSet);
		yAxisTicksControl.Parent = this;
		
		Label xAxisShowTicksLabel = new Label();
		xAxisShowTicksLabel.Left = yAxisRangeLabel.Left;
		xAxisShowTicksLabel.Top = EasyLayout.Below(yAxisRangeControl, 5);
		xAxisShowTicksLabel.Width = 75;
		xAxisShowTicksLabel.Text = "Show X Ticks";
		xAxisShowTicksLabel.TextAlign = ContentAlignment.TopLeft;
		xAxisShowTicksLabel.Parent = this;		
		
		CheckBox xAxisShowTicksControl = new CheckBox();
		xAxisShowTicksControl.Left = EasyLayout.LeftOf(xAxisShowTicksLabel, 5);
		xAxisShowTicksControl.Top = xAxisShowTicksLabel.Top - 4;
		xAxisShowTicksControl.Width = 10;
		xAxisShowTicksControl.Parent = this;
		
		Label yAxisShowTicksLabel = new Label();
		yAxisShowTicksLabel.Left = xAxisShowTicksLabel.Left;
		yAxisShowTicksLabel.Top = EasyLayout.Below(xAxisShowTicksLabel, 5);
		yAxisShowTicksLabel.Width = 75;
		yAxisShowTicksLabel.Text = "Show Y Ticks";
		yAxisShowTicksLabel.TextAlign = ContentAlignment.TopLeft;
		yAxisShowTicksLabel.Parent = this;		
		
		CheckBox yAxisShowTicksControl = new CheckBox();
		yAxisShowTicksControl.Left = EasyLayout.LeftOf(yAxisShowTicksLabel, 5);
		yAxisShowTicksControl.Top = yAxisShowTicksLabel.Top - 4;
		yAxisShowTicksControl.Width = 10;
		yAxisShowTicksControl.Checked = true;
		yAxisShowTicksControl.CheckedChanged += new EventHandler(YAxisShowTicksSet);
		yAxisShowTicksControl.Parent = this;
		
		Label xAxisShowFrameLabel = new Label();
		xAxisShowFrameLabel.Left = EasyLayout.LeftOf(xAxisShowTicksLabel, 25);
		xAxisShowFrameLabel.Top = xAxisShowTicksLabel.Top;
		xAxisShowFrameLabel.Width = 85;
		xAxisShowFrameLabel.Text = "Show X Frame";
		xAxisShowFrameLabel.TextAlign = ContentAlignment.TopLeft;
		xAxisShowFrameLabel.Parent = this;		
		
		CheckBox xAxisShowFrameControl = new CheckBox();
		xAxisShowFrameControl.Left = EasyLayout.LeftOf(xAxisShowFrameLabel, 5);
		xAxisShowFrameControl.Top = xAxisShowFrameLabel.Top - 4;
		xAxisShowFrameControl.Width = 10;
		xAxisShowFrameControl.Checked = true;
		xAxisShowFrameControl.CheckedChanged += new EventHandler(XAxisShowFrameSet);
		xAxisShowFrameControl.Parent = this;
		
		Label yAxisShowFrameLabel = new Label();
		yAxisShowFrameLabel.Left = xAxisShowFrameLabel.Left;
		yAxisShowFrameLabel.Top = EasyLayout.Below(xAxisShowFrameLabel, 5);
		yAxisShowFrameLabel.Width = 85;
		yAxisShowFrameLabel.Text = "Show Y Frame";
		yAxisShowFrameLabel.TextAlign = ContentAlignment.TopLeft;
		yAxisShowFrameLabel.Parent = this;		
		
		CheckBox yAxisShowFrameControl = new CheckBox();
		yAxisShowFrameControl.Left = EasyLayout.LeftOf(yAxisShowFrameLabel, 5);
		yAxisShowFrameControl.Top = yAxisShowFrameLabel.Top - 4;
		yAxisShowFrameControl.Width = 10;
		yAxisShowFrameControl.Checked = true;
		yAxisShowFrameControl.Parent = this;

		///////////////////////////////////////////////
		
		LoadSampleGraphImages();
		
		this.Load += new EventHandler(GraphTypeChanged);

		ShowDialog();
	}
	
	private void BuildOpenFileDialog()
	{
		if(openFileDialog != null) return;
		openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "Excel files (*.xls)|*.xls|Comma Separated Values (*.csv)|*.csv" ;
		openFileDialog.FilterIndex = 2;
		string defaultDirectory = "E:\\GitHub\\Spire\\Mockup\\";
		if(Directory.Exists(defaultDirectory))
		{
			openFileDialog.InitialDirectory = defaultDirectory;
		}
//		openFileDialog.RestoreDirectory = true;
	}
	
	private void SelectFile(object sender, EventArgs e)
	{
		BuildOpenFileDialog();
		if(openFileDialog.ShowDialog() == DialogResult.OK)
		{
			dataSourceControl.Text = openFileDialog.FileName;
		}
	}
	
	private void LoadSampleGraphImages()
	{
		if(sampleGraphImages != null) return;
		sampleGraphImages = new Dictionary<string, List<Bitmap>>();
		sampleGraphImages["Histogram"] = new List<Bitmap>() { 
			LoadImage("reference\\histogramRange.png") 
		};
		sampleGraphImages["Line Graph"] = new List<Bitmap>() { 
			LoadImage("reference\\line.png") 
			, LoadImage("reference\\lineStacked.png") 
		};
		sampleGraphImages["Geographical Map"] = new List<Bitmap>() { 
			LoadImage("reference\\mapUSA.png") 
			, LoadImage("reference\\mapAfrica.png") 
		};
		sampleGraphImages["Scatterplot"] = new List<Bitmap>() { 
			LoadImage("reference\\scatterPlot.png") 
			, LoadImage("reference\\scatterPlot_bubble.png") 
			, LoadImage("reference\\scatterPlot_dashFrame.png") 
			, LoadImage("reference\\scatterPlot_dashFrame_icon.png") 
			, LoadImage("reference\\scatterPlot_heatMap.png") 
			, LoadImage("reference\\scatterPlot_PhillipsCurve.png") 
		};
		sampleGraphImages["Slant"] = new List<Bitmap>() { 
			LoadImage("reference\\slant.png") 
		};
		sampleGraphImages["Stem and Leaf"] = new List<Bitmap>() { 
			LoadImage("reference\\stemAndLeaf.png") 
			, LoadImage("reference\\stemAndLeaf_Double.png") 
		};
	}
	
	private Bitmap LoadImage(string filename)
	{
		if(File.Exists(filename))
		{
			return new Bitmap(filename);
		}
		Console.WriteLine(filename + " not found");
		return null;
	}
	
	private void GraphTypeChanged(object sender, EventArgs e)
	{
		string selectedGraphType = graphTypeControl.SelectedItem.ToString();
		sampleGraphA.Visible = false;
		sampleGraphB.Visible = false;
		sampleGraphC.Visible = false;
		sampleGraphD.Visible = false;
		if(sampleGraphImages.ContainsKey(selectedGraphType))
		{
		Console.WriteLine("A");
			List<Bitmap> bitmaps = sampleGraphImages[selectedGraphType];
			if(bitmaps.Count > 0 && bitmaps[0] != null)
			{
		Console.WriteLine("B");
				sampleGraphA.Image = bitmaps[0];
				sampleGraphA.Visible = true;
			}
			if(bitmaps.Count > 1 && bitmaps[1] != null)
			{
				sampleGraphB.Image = bitmaps[1];
				sampleGraphB.Visible = true;
			}
			/*
			if(bitmaps.Count > 2 && bitmaps[2] != null)
			{
				sampleGraphC.Image = bitmaps[2];
				sampleGraphC.Visible = true;
			}
			if(bitmaps.Count > 3 && bitmaps[3] != null)
			{
				sampleGraphD.Image = bitmaps[3];
				sampleGraphD.Visible = true;
			}
			*/
		}
		/*
		switch(box.SelectedItem.ToString())
		{
			case "Line Graph":
				break;
			case "Scatterplot":
				break;
			case "Rug Plot":
				break;
			case "Geographical Map":
				break;
			case "Heat Map":
				break;
			case "Bar Graph":
				break;
			case "Histogram":
				break;
			case "Box Plot":
				break;
			case "Stem and Leaf":
				break;
			case "Sparkline":
				break;
			case "Slant Graph":
				break;
			case "Tree Map":
				break;
			case "Cosmograph":
				break;
		}
		*/
	}
	
	private void AxisColumnsSet(object sender, EventArgs e)
	{
		previewGraph.Image = LoadImage("reference\\avgBirthsPerMonthA.png");
		previewLabel.Visible = true;
		previewGraph.Visible = true;
	}
	
	private void AxisLabelsSet(object sender, EventArgs e)
	{
		previewGraph.Image = LoadImage("reference\\avgBirthsPerMonthA2.png");
	}
	
	private void PlotWithSet(object sender, EventArgs e)
	{
		plotWithColumnLabel.Visible = true;
		plotWithColumnControl.Visible = true;
	}
	
	private void PlotWithColumnSet(object sender, EventArgs e)
	{
		previewGraph.Image = LoadImage("reference\\avgBirthsPerMonthB.png");
	}
	
	private void YAxisRangeSet(object sender, EventArgs e)
	{
		previewGraph.Image = LoadImage("reference\\avgBirthsPerMonthC.png");
	}
	
	private void YAxisTicksSet(object sender, EventArgs e)
	{
		previewGraph.Image = LoadImage("reference\\avgBirthsPerMonthD.png");
	}
	
	private void YAxisShowTicksSet(object sender, EventArgs e)
	{
		previewGraph.Image = LoadImage("reference\\avgBirthsPerMonthE.png");
	}
	
	private void XAxisShowFrameSet(object sender, EventArgs e)
	{
		previewGraph.Image = LoadImage("reference\\avgBirthsPerMonthF.png");
	}
}
