using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

public static class DataAnnotationsDemo
{
	private static Panel drawPanel;
	private static List<Spire.Box> boxes;
	private static TextBox text1;

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
		
		text1 = MockupWindow.BuildTextInput();
		text1.Font = new Font("Times New Roman", 12);
		text1.Text = "      Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis ornare mauris id venenatis pretium. Quisque ac consequat justo, pretium laoreet mi. Ut pharetra, felis a porttitor porttitor, tortor nisi ultrices nisl, nec vehicula turpis erat ut ante. Donec maximus blandit felis eget cursus. Nunc dignissim feugiat purus, id maximus elit. Fusce eleifend dolor eu ante interdum pellentesque. Quisque malesuada congue ligula, at scelerisque risus convallis vitae. Pellentesque gravida laoreet consequat.";
		text1.Top = margin;
		text1.Left = margin;
		text1.Width = (drawPanel.Width - margin - margin - middleSpacer) / 2;
		text1.Height = 175;
		text1.Parent = drawPanel;

		boxes.Add(new Spire.ImageBox() {
			X = text1.Left + 20
			, Y = EasyLayout.Below(text1, 5)
			, Width = 300
			, Height = 200
			, Filename = "reference\\dataAnnotationModel.png"
		});

		TextBox text2 = MockupWindow.BuildTextInput();
		text2.Font = text1.Font;
		text2.Text = "      Vivamus rutrum nulla diam, in condimentum lacus mattis a. Donec ante sapien, molestie at convallis vel, tincidunt nec nibh. Vivamus ac commodo odio. Aliquam maximus, lacus sit amet ornare efficitur, dolor justo condimentum risus, nec posuere massa nunc vel tortor. Interdum et malesuada fames ac ante ipsum primis in faucibus. Sed vitae enim fermentum, molestie sem vel, dictum felis. Donec purus risus, volutpat luctus justo non, interdum tristique ante.";
		text2.Top = boxes[0].Y + boxes[0].Height + 5;
		text2.Left = text1.Left;
		text2.Width = text1.Width;
		text2.Height = 175;
		text2.Parent = drawPanel;

		TextBox text4 = MockupWindow.BuildTextInput();
		text4.Font = text1.Font;
		text4.Text = "ligula, in tristique libero tristique a. Nullam eget sapien mi. Aenean pretium, magna sed dictum hendrerit, libero ipsum venenatis lorem, non fringilla metus tortor in nibh. Aenean tempus, justo a sollicitudin tristique, libero dolor faucibus nisi, eget ullamcorper justo tortor ut arcu. Donec quis nisi id turpis feugiat ornare tempus a elit. Vestibulum eu lorem consequat, lacinia velit eu, maximus ex. Quisque dictum nibh ac libero tempor, eu congue sem viverra. Aliquam feugiat, felis quis consequat elementum, erat lacus tincidunt purus, at rhoncus purus justo eget nulla. Donec sed ipsum egestas, aliquet nibh sed, mollis mauris. Sed fermentum sodales velit ut malesuada. Morbi sed est neque.";
		text4.Top = text1.Top;
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

		TextBox text3 = MockupWindow.BuildTextInput();
		text3.Font = text1.Font;
		text3.Text = "      Proin sed dapibus turpis. Cras eget nulla mauris. Aliquam lobortis euismod risus id dapibus. Cras iaculis fermentum malesuada. In interdum erat vel semper auctor. Ut vitae sapien ac nunc elementum hendrerit venenatis eu purus. Aenean efficitur, massa vel ultricies sollicitudin, ante sapien suscipit sapien, eu porta sapien arcu sed turpis. Quisque mollis ultricies arcu, nec pretium nisi auctor sed. Integer quis pulvinar massa. Pellentesque porttitor lorem eget massa mollis pulvinar. Nullam in quam ac risus ultricies accumsan. Sed nec dui sed lorem viverra viverra. Curabitur velit justo, porta vel quam elementum, finibus aliquam ipsum.";
		text3.Top = EasyLayout.Below(text5, 0);
		text3.Left = text5.Left;
		text3.Width = text1.Width;
		text3.Height = 300;
		text3.Parent = drawPanel;
		
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
	
	public static ContextMenu BuildContextMenu()
	{
		ContextMenu menu = new ContextMenu();
		MenuItem paste = new MenuItem() { Text="Paste Image" };
		MenuItem image = new MenuItem() { Text="Import Image"};
		MenuItem dataGraphic = new MenuItem() { Text="Edit Data Graphic" };
		MenuItem dataAnnotation = new MenuItem() { Text="Data Annotations" };
		dataAnnotation.Click += new EventHandler(OpenDataAnnotations);
		menu.MenuItems.Add(paste);
		menu.MenuItems.Add(image);
		menu.MenuItems.Add(dataGraphic);
		menu.MenuItems.Add(dataAnnotation);
		return menu;
	}
	
	private static void OpenDataAnnotations(object sender, EventArgs e)
	{
		DataAnnotationsDialog dialog = new DataAnnotationsDialog();
		dialog.FormClosed += new FormClosedEventHandler(DialogClosed);
	}
	
	public static void DialogClosed(object sender, FormClosedEventArgs e)
	{
		if(boxes.Count == 0) return;
		text1.SelectionStart = 0;
		text1.SelectionLength = 0;
		(boxes[0] as Spire.ImageBox).Filename = "reference\\dataAnnotationModelB.png";
		drawPanel.Invalidate();
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
		g.Dispose();
		pea.Graphics.DrawImageUnscaled(graphicsBuffer, 0, 0);
	}

	private static void PaintImage(Graphics g, Spire.ImageBox box)
	{
		if(!File.Exists(box.Filename))
		{
			Pen pen = new Pen(Color.Gray, 1);
			g.DrawRectangle(pen, box.X, box.Y, box.Width, box.Height);
			return;
		}
			
		using (Image src = Image.FromFile(box.Filename))
		{
			g.DrawImage(src, box.X, box.Y, 300, 225);
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

public class DataAnnotationsDialog : Form
{
	private enum Mode { Arrow, Text };
	private Mode mode = Mode.Arrow;

	public DataAnnotationsDialog()
	{
		Width = 600;
		Height = 500;
		Text = "Data Annotations";
		Icon = new Icon("SpireIcon1.ico");
		StartPosition = FormStartPosition.CenterParent;
		
		Button circleButton = new Button();
		circleButton.Width = 21;
		circleButton.Height = 21;
		circleButton.BackgroundImage = LoadImage("reference\\iconCircle.png");
		circleButton.Left = 20;
		circleButton.Top = 20;
		circleButton.Parent = this;

		Button circleFilledButton = new Button();
		circleFilledButton.Width = circleButton.Width;
		circleFilledButton.Height = circleButton.Height;
		circleFilledButton.BackgroundImage = LoadImage("reference\\iconCircleFilled.png");
		circleFilledButton.Left = EasyLayout.LeftOf(circleButton, 0);
		circleFilledButton.Top = circleButton.Top;
		circleFilledButton.Parent = this;

		Button squareButton = new Button();
		squareButton.Width = circleButton.Width;
		squareButton.Height = circleButton.Height;
		squareButton.BackgroundImage = LoadImage("reference\\iconSquare.png");
		squareButton.Left = circleButton.Left;
		squareButton.Top = EasyLayout.Below(circleButton, 0);
		squareButton.Parent = this;

		Button squareFilledButton = new Button();
		squareFilledButton.Width = circleButton.Width;
		squareFilledButton.Height = circleButton.Height;
		squareFilledButton.BackgroundImage = LoadImage("reference\\iconSquareFilled.png");
		squareFilledButton.Left = EasyLayout.LeftOf(squareButton, 0);
		squareFilledButton.Top = squareButton.Top;
		squareFilledButton.Parent = this;

		Button lineButton = new Button();
		lineButton.Width = circleButton.Width;
		lineButton.Height = circleButton.Height;
		lineButton.BackgroundImage = LoadImage("reference\\iconLine.png");
		lineButton.Left = circleButton.Left;
		lineButton.Top = EasyLayout.Below(squareButton, 0);
		lineButton.Parent = this;

		Button arrowButton = new Button();
		arrowButton.Width = circleButton.Width;
		arrowButton.Height = circleButton.Height;
		arrowButton.BackgroundImage = LoadImage("reference\\iconArrow.png");
		arrowButton.Left = EasyLayout.LeftOf(lineButton, 0);
		arrowButton.Top = lineButton.Top;
		arrowButton.Parent = this;

		Button textButton = new Button();
		textButton.Width = circleButton.Width;
		textButton.Height = circleButton.Height;
		textButton.BackgroundImage = LoadImage("reference\\iconText.png");
		textButton.Left = arrowButton.Left;
		textButton.Top = EasyLayout.Below(arrowButton, 0);
		textButton.Click += (sender, e) => { mode = Mode.Text; };
		textButton.Parent = this;

		Button doneButton = new Button();
		doneButton.Width = circleButton.Width * 2;
		doneButton.Height = circleButton.Height;
		doneButton.Text = "Done";
		doneButton.Left = circleButton.Left;
		doneButton.Top = EasyLayout.Below(textButton, 10);
		doneButton.Click += (sender, e) => { DataAnnotationsDemo.DialogClosed(null, null); this.Close(); };
		doneButton.Parent = this;

		PictureBox graph = new PictureBox();
		graph.SizeMode = PictureBoxSizeMode.Zoom;
		graph.Width = 400;
		graph.Height = 300;
		graph.Left = EasyLayout.LeftOf(circleFilledButton, 10);
		graph.Top = circleButton.Top;
		graph.Image = LoadImage("reference\\dataAnnotationModel.png");
		graph.Parent = this;

		this.MouseDown += new MouseEventHandler(MouseDown);
		this.MouseUp += new MouseEventHandler(MouseUp);
		this.MouseMove += new MouseEventHandler(MouseMove);
		
		ShowDialog();
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

	private static void MouseDown(object sender, MouseEventArgs e)
	{
	}
	
	private static void MouseUp(object sender, MouseEventArgs e)
	{
	}
	
	private static void MouseMove(object sender, MouseEventArgs e)
	{
	}
	
		
}

