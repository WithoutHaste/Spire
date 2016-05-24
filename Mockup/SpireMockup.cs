using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

public class MockupWindow : Form
{
	[STAThread] //required to use OpenFileDialog - research more
	public static void Main(string[] args)
	{
		Application.Run(new MockupWindow());
	}
	
	private DoubleBufferedPanel scrollPanel;
	private DoubleBufferedPanel mainPanel;
	
	public MockupWindow()
	{
		this.SuspendLayout();
		
		this.Size = new Size(830,600);
		this.Text = "Spire Mockup";
		this.Icon = new Icon("SpireIcon1.ico");
		
		this.Menu = BuildMenuBar();
		
		scrollPanel = BuildScrollPanel();
		this.Controls.Add(scrollPanel);

		mainPanel = BuildMainPanel();
		mainPanel.Anchor = AnchorStyles.Top;
		mainPanel.Parent = scrollPanel;
		
		ResumeLayout(false);
	}
	
	private DoubleBufferedPanel BuildScrollPanel()
	{
		DoubleBufferedPanel scrollPanel = new DoubleBufferedPanel();
		scrollPanel.Width = this.Width - 20;
		scrollPanel.Height = this.Height;
		scrollPanel.AutoScroll = true;
		return scrollPanel;
	}
	
	private MainMenu BuildMenuBar()
	{
		MainMenu mainMenu = new MainMenu();

		System.Windows.Forms.MenuItem fileMenuItem = new System.Windows.Forms.MenuItem("File");
		System.Windows.Forms.MenuItem configMenuItem = new System.Windows.Forms.MenuItem("Config");
		System.Windows.Forms.MenuItem demosMenuItem = new System.Windows.Forms.MenuItem("Demos");
		System.Windows.Forms.MenuItem helpMenuItem = new System.Windows.Forms.MenuItem("Help");

		fileMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("&New"));
		fileMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("&Open"));
		fileMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Close"));
		fileMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("&Save"));
		fileMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Save As..."));
		fileMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Export PDF", new EventHandler(ExportPDFClick)));
		fileMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("&Print"));
		fileMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Exit"));

		configMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Formula"));
		configMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Plugins"));
		configMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Default Styles"));
		configMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Language"));

		demosMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Text Layout", new EventHandler(SetupTextLayoutDemo)));
		demosMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Custom Layout", new EventHandler(SetupCustomLayoutDemo)));
		demosMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Styles Basic", new EventHandler(SetupStylesDemo)));
		demosMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Styles Advanced", new EventHandler(SetupStylesDemoAdvanced)));
		demosMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Formulas", new EventHandler(SetupFormulasDemo)));
		demosMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Footnotes", new EventHandler(SetupFootnotesDemo)));
		demosMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("PDF", new EventHandler(SetupPDFDemo)));
		demosMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Graphs", new EventHandler(SetupGraphsDemo)));
		demosMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Data Graphics", new EventHandler(SetupDataGraphicsDemo)));
		demosMenuItem.MenuItems.Add(new System.Windows.Forms.MenuItem("Data Annotations", new EventHandler(SetupDataAnnotationsDemo)));

		mainMenu.MenuItems.Add(fileMenuItem);
		mainMenu.MenuItems.Add(configMenuItem);
//		mainMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Styles", new EventHandler(OpenModalStyles)));
		mainMenu.MenuItems.Add(demosMenuItem);
		mainMenu.MenuItems.Add(helpMenuItem);

		return mainMenu;
	}

	private DoubleBufferedPanel BuildMainPanel()
	{
		DoubleBufferedPanel panel = new DoubleBufferedPanel();
		panel.AutoScroll = true;
		panel.Size = new Size(this.Width, this.Height + 800);
		//panel.Location = new Point(10, 10);
		panel.BorderStyle = BorderStyle.Fixed3D;
		panel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
		panel.BackColor = Color.White;

		return panel;
	}
	
	private void RemoveAllDemos()
	{
		mainPanel.Paint -= FootnotesDemo.Paint;
		mainPanel.Paint -= TextLayoutDemo.Paint;
		mainPanel.Paint -= CustomLayoutDemo.Paint;
		mainPanel.Paint -= StylesDemo.Paint;
		mainPanel.Paint -= StylesDemoAdvanced.Paint;
		mainPanel.Paint -= DataGraphicsDemo.Paint;
		mainPanel.Paint -= DataAnnotationsDemo.Paint;
		CustomLayoutDemo.RemovePanel();
		mainPanel.Controls.Clear();
	}
	
	private void SetupFootnotesDemo(object sender, EventArgs e)
	{
		RemoveAllDemos();
		mainPanel.Paint += new PaintEventHandler(FootnotesDemo.Paint);
		mainPanel.Invalidate();
	}
	
	private void SetupFormulasDemo(object sender, EventArgs e)
	{
		RemoveAllDemos();
		FormulasDemo.InitializeDemo(mainPanel);
		mainPanel.Invalidate();
	}

	private void SetupPDFDemo(object sender, EventArgs e)
	{
		RemoveAllDemos();
		PDFDemo.InitializeDemo(mainPanel);
		mainPanel.Invalidate();
	}
	
	private void SetupTextLayoutDemo(object sender, EventArgs e)
	{
		RemoveAllDemos();
		mainPanel.Paint += new PaintEventHandler(TextLayoutDemo.Paint);
		mainPanel.Invalidate();
	}
	
	private void SetupCustomLayoutDemo(object sender, EventArgs e)
	{
		RemoveAllDemos();
		this.Width = 1000;
		scrollPanel.Width = this.Width - 20;
		mainPanel.Width = this.Width;
		mainPanel.Left = 0;
		CustomLayoutDemo.SetPanel(mainPanel);
		mainPanel.Paint += new PaintEventHandler(CustomLayoutDemo.Paint);
		mainPanel.Invalidate();
	}
	
	private void SetupDataGraphicsDemo(object sender, EventArgs e)
	{
		RemoveAllDemos();
		DataGraphicsDemo.SetPanel(mainPanel);
		mainPanel.Paint += new PaintEventHandler(DataGraphicsDemo.Paint);
		mainPanel.Invalidate();
		
		this.ContextMenu = DataGraphicsDemo.BuildContextMenu();
	}
	
	private void SetupDataAnnotationsDemo(object sender, EventArgs e)
	{
		RemoveAllDemos();
		DataAnnotationsDemo.SetPanel(mainPanel);
		mainPanel.Paint += new PaintEventHandler(DataAnnotationsDemo.Paint);
		mainPanel.Invalidate();
		
		this.ContextMenu = DataAnnotationsDemo.BuildContextMenu();
	}
	
	private void SetupGraphsDemo(object sender, EventArgs e)
	{
		RemoveAllDemos();
		mainPanel.Paint += new PaintEventHandler(GraphsDemo.Paint);
		mainPanel.Invalidate();
	}
	
	private void SetupStylesDemo(object sender, EventArgs e)
	{
		RemoveAllDemos();
		StylesDemo.Init();
		mainPanel.Paint += new PaintEventHandler(StylesDemo.Paint);
		mainPanel.Invalidate();
		StyleDialog dialog = new StyleDialog(mainPanel);
	}

	private void SetupStylesDemoAdvanced(object sender, EventArgs e)
	{
		RemoveAllDemos();
		StylesDemoAdvanced.Init();
		mainPanel.Paint += new PaintEventHandler(StylesDemoAdvanced.Paint);
		mainPanel.Invalidate();
		StyleDialogAdvanced dialog = new StyleDialogAdvanced(mainPanel);
	}

	public static TextBox BuildTextInput()
	{
		TextBox textBox = new TextBox();
		textBox.AcceptsReturn = true;
		textBox.AcceptsTab = true;
		textBox.Multiline = true;
		textBox.BorderStyle = BorderStyle.None;
		return textBox;
	}
	
	public static Label BuildLabel(ContentAlignment align)
	{
		Label label = new Label();
		label.BorderStyle = BorderStyle.None;
		label.TextAlign = align;
		return label;
	}
	
	private void ExportPDFClick(object sender, EventArgs e)
	{
		Process.Start(Environment.CurrentDirectory + "\\demoPDF.pdf");
	}
			
}

public class Rectangle
{
	public Rectangle(int x, int y, int width, int height)
	{
		X = x;
		Y = y;
		Width = width;
		Height = height;
	}
	
	public int X { get; set; }
	public int Y { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
}
