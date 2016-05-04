using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

public class MockupWindow : Form
{
	public static void Main(string[] args)
	{
		Application.Run(new MockupWindow());
	}
	
	private Panel mainPanel;
	
	public MockupWindow()
	{
		this.SuspendLayout();
		
		this.Size = new Size(830,600);
		this.Text = "Spire Mockup";
		this.Icon = new Icon("SpireIcon1.ico");
		
		this.Menu = BuildMenuBar();
		
		Panel scrollPanel = BuildScrollPanel();
		this.Controls.Add(scrollPanel);

		mainPanel = BuildMainPanel();
		mainPanel.Anchor = AnchorStyles.Top;
		mainPanel.Parent = scrollPanel;
		
		ResumeLayout(false);
	}
	
	private Panel BuildScrollPanel()
	{
		Panel scrollPanel = new Panel();
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

		mainMenu.MenuItems.Add(fileMenuItem);
		mainMenu.MenuItems.Add(configMenuItem);
		mainMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Styles", new EventHandler(OpenModalStyles)));
		mainMenu.MenuItems.Add(helpMenuItem);

		return mainMenu;
	}

	private Panel BuildMainPanel()
	{
		System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
		panel.AutoScroll = true;
		panel.Size = new Size(this.Width, this.Height + 500);
		//panel.Location = new Point(10, 10);
		panel.BorderStyle = BorderStyle.Fixed3D;
		panel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
		panel.BackColor = Color.White;
		
//		panel.Paint += new PaintEventHandler(PaintTextLayoutDemo);
//		panel.Paint += new PaintEventHandler(PaintGraphs);

//		SetupFormulaDemo(panel);
		SetupPDFDemo(panel);

		return panel;
	}
	
	private TextBox formulaTextBox;
	
	private void SetupFormulaDemo(Panel parent)
	{
		int margin = 10;
		TextBox textBox = BuildTextInput();
		textBox.Left = margin;
		textBox.Top = margin;
		textBox.Height = parent.Height - 4*margin;
		textBox.Width = parent.Width - 4*margin;
		textBox.Font = new Font(new FontFamily("Times New Roman"), 18);
		textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
		textBox.Parent = parent;
		
		//textBox.Text = "\tThe main winding was of the normal lodusodelta-type plate placed in panendermic semiboloid slots of the stator, every seventh conductor being connected by a non-reversible turmy pipe to the differential girdle spring on the upper end of the grammies.";
		textBox.Text = "\tThe goal is that the user may type continuously to create text and formulas. The mouse should not be required. This can be achieved with configurable replacement text, such as \"pi\" to π, and layout support for complex forms such as integrals.";
		textBox.SelectionStart = textBox.Text.Length;
		
		formulaTextBox = textBox;
	}
	
	private void SetupPDFDemo(Panel parent)
	{
		int margin = 10;
		int leftSide = margin;
		int leftWidth = parent.Width/2 - margin;
		int rightSide = leftSide + leftWidth + margin;
		int rightWidth = leftWidth;
				
		Label title = BuildLabel(ContentAlignment.TopCenter);
		title.Left = margin;
		title.Top = margin;
		title.Height = 25;
		title.Width = parent.Width - 4*margin;
		title.Font = new Font(new FontFamily("Times New Roman"), 16);
		title.Text = "Axiomata sive Leges Motus";
		title.Parent = parent;

		Label header1 = BuildLabel(ContentAlignment.TopCenter);
		header1.Left = leftSide;
		header1.Top = title.Top + title.Height + 25;
		header1.Height = 20;
		header1.Width = leftWidth;
		header1.Font = new Font(new FontFamily("Times New Roman"), 14);
		header1.Text = "LEX 1.";
		header1.Parent = parent;

		Label p1A = BuildLabel(ContentAlignment.TopLeft);
		p1A.Left = leftSide;
		p1A.Top = header1.Top + header1.Height + 5;
		p1A.Height = 60;
		p1A.Width = leftWidth;
		p1A.Font = new Font(new FontFamily("Times New Roman"), 12, FontStyle.Italic);
		p1A.Text = "Corpus omne perseverare in statu suo quiescendi vel movendi uniformiter in directum, nisi quatenus illud a viribus impressis cogitur statum suum mutare.";
		p1A.Parent = parent;

		Label p1B = BuildLabel(ContentAlignment.TopLeft);
		p1B.Left = leftSide;
		p1B.Top = p1A.Top + p1A.Height + 5;
		p1B.Height = 60;
		p1B.Width = leftWidth;
		p1B.Font = new Font(new FontFamily("Times New Roman"), 12);
		p1B.Text = "    Corpus omne perseverare in statu suo quiescendi vel movendi uniformiter in directum, nisi quatenus illud a viribus impressis cogitur statum suum mutare.";
		p1B.Parent = parent;

		Label header2 = BuildLabel(ContentAlignment.TopCenter);
		header2.Left = leftSide;
		header2.Top = p1B.Top + p1B.Height + 10;
		header2.Height = 20;
		header2.Width = leftWidth;
		header2.Font = new Font(new FontFamily("Times New Roman"), 14);
		header2.Text = "LEX II.";
		header2.Parent = parent;

		Label p2A = BuildLabel(ContentAlignment.TopLeft);
		p2A.Left = leftSide;
		p2A.Top = header2.Top + header2.Height + 5;
		p2A.Height = 60;
		p2A.Width = leftWidth;
		p2A.Font = new Font(new FontFamily("Times New Roman"), 12, FontStyle.Italic);
		p2A.Text = "Mutationem motus proportionalem esse vi motrici impressa, && fieri secundum lineam rectam qua vis illa imprimitur.";
		p2A.Parent = parent;

		Label p2B = BuildLabel(ContentAlignment.TopLeft);
		p2B.Left = leftSide;
		p2B.Top = p2A.Top + p2A.Height + 5;
		p2B.Height = 150;
		p2B.Width = leftWidth;
		p2B.Font = new Font(new FontFamily("Times New Roman"), 12);
		p2B.Text = "    Si vis aliqua motum quemvis generet; dupla duplum, tripla triplum generabit, sive simul && semel, sive gradatim && successive impressa fuerit. Et hic motus (quoniam in eandem semper plagam cum vi generatrice determinatur) si corpus antea movebatur, motui ejus vel conspiranti additur, vel contrario subducitur, vel obliquo oblique adjicitur, && cum eo secundum utriusque determinationem componitur.";
		p2B.Parent = parent;

		Label header3 = BuildLabel(ContentAlignment.TopCenter);
		header3.Left = leftSide;
		header3.Top = p2B.Top + p2B.Height + 10;
		header3.Height = 20;
		header3.Width = leftWidth;
		header3.Font = new Font(new FontFamily("Times New Roman"), 14);
		header3.Text = "LEX III.";
		header3.Parent = parent;

		Label p3A = BuildLabel(ContentAlignment.TopLeft);
		p3A.Left = leftSide;
		p3A.Top = header3.Top + header3.Height + 5;
		p3A.Height = 60;
		p3A.Width = leftWidth;
		p3A.Font = new Font(new FontFamily("Times New Roman"), 12, FontStyle.Italic);
		p3A.Text = "Actioni contrariam semper && aqualem esse reactionem: sive corporum duorum actiones in se mutuo semper esse aquales && in partes contrarias dirigi.";
		p3A.Parent = parent;

		Label p3B = BuildLabel(ContentAlignment.TopLeft);
		p3B.Left = leftSide;
		p3B.Top = p3A.Top + p3A.Height + 5;
		p3B.Height = 200;
		p3B.Width = leftWidth;
		p3B.Font = new Font(new FontFamily("Times New Roman"), 12);
		p3B.Text = "    Quicquid premit vel trahit alterum, tantundem ab eo premitur vel trahitur. Si quis lapidem digito premit, premitur && hujus digitus a lapide. Si equus lapidem funi alligatum trahit, retrahetur etiam && equus (ut ita dicam) aequaliter in lapidem: nam funis utrinque distentus eodem relaxandi se conatu urgebit equum versus lapidem, ac lapidem versus equum; tantumque impediet progressum unius quantum promovet pregressum alterius. Si corpus aliquod in corpus aliud impingens, motum ejus vi sua quomodocumque mutaverit, idem quoque vicissim in motu ejus vi sua quomodocunque mutaverit, idem quoque vicissim in motu prorio eandem mutationem in partem contratium vi alterius (ob aequalitatem pressionis mutuae) subibit. His actionibus aeqales fiunt mutationes, non velocitatem, sed motuum; scilicet in corporibus non aliunde impeditis. Mutationes enim velocitatum, in contratias itidem partes factae, quia motus aequaliter mutantur, sunt corporibus reciproce proportionales. Obtinet etiam haec lex in attractionibus, ut in scholio proximo probabitur.";
		p3B.Parent = parent;

		Label header4 = BuildLabel(ContentAlignment.TopCenter);
		header4.Left = rightSide;
		header4.Top = title.Top + title.Height + 25;
		header4.Height = 20;
		header4.Width = rightWidth;
		header4.Font = new Font(new FontFamily("Times New Roman"), 14);
		header4.Text = "Corollarium I.";
		header4.Parent = parent;

		Label p4A = BuildLabel(ContentAlignment.TopLeft);
		p4A.Left = rightSide;
		p4A.Top = header4.Top + header4.Height + 5;
		p4A.Height = 45;
		p4A.Width = rightWidth;
		p4A.Font = new Font(new FontFamily("Times New Roman"), 12, FontStyle.Italic);
		p4A.Text = "Corpus viribus conjunctis diagonalem parallelogrammi eodem tempore describere, quo latera separatis.";
		p4A.Parent = parent;

		Button diagramButton = new Button();
		diagramButton.Top = p4A.Top + p4A.Height + 5;
		diagramButton.Height = 110;
		diagramButton.Width = 180;
		diagramButton.Left = rightSide + (rightWidth - diagramButton.Width);
		diagramButton.Image = Image.FromFile("PrincipiaMathematicaCorollariumI.png");
		diagramButton.ImageAlign = ContentAlignment.MiddleCenter;
		diagramButton.FlatStyle = FlatStyle.Flat;
		diagramButton.FlatAppearance.BorderSize = 0;
		diagramButton.Parent = parent;
		diagramButton.BringToFront();

		Label p4B = BuildLabel(ContentAlignment.TopLeft);
		p4B.Left = rightSide;
		p4B.Top = p4A.Top + p4A.Height + 5;
		p4B.Height = 110;
		p4B.Width = rightWidth - diagramButton.Width - 5;
		p4B.Font = new Font(new FontFamily("Times New Roman"), 12);
		p4B.Text = "    Si corpus dato tempore, vi sola M in loco A impressa, ferretur uniformi cum motu ab A ad B, && vi sola N in eodem loco impresa, ferretur ab A ad C: compleatur";
		p4B.Parent = parent;
		
		Label p4C = BuildLabel(ContentAlignment.TopLeft);
		p4C.Left = rightSide;
		p4C.Width = rightWidth;
		p4C.Top = p4B.Top + p4B.Height;
		p4C.Height = 230;
		p4C.Font = new Font(new FontFamily("Times New Roman"), 12);
		p4C.Text = "parallelogrammum ABDC, && vi utraque feretur corpus illud eodem tempore in diagonali ab A ad S. Nam quoniam vis N agit secundum lineam AC ipsi BD parallelam, haec vis per legem II nihil mutabit velocitatem accedendi ad lineam illam BD a vi altera genitam. Accedet igitur corpus eodem tempore ad lineam BD, sive vis N imprimatur, sive non; atque ideo in fine illius temporis reperietur alicubi in linea illa BD. Eodem argumento in fine temporis ejusdem reperietur alicubi in linea CD, && idcrico in utriusque lineae concursu D reperiri necesse est. Perget autem motu rectilineo ab A ad D per legem I.";
		p4C.Parent = parent;

		Label header5 = BuildLabel(ContentAlignment.TopCenter);
		header5.Left = rightSide;
		header5.Top = p4C.Top + p4C.Height + 25;
		header5.Height = 20;
		header5.Width = rightWidth;
		header5.Font = new Font(new FontFamily("Times New Roman"), 14);
		header5.Text = "Corollarium II.";
		header5.Parent = parent;

		Label p5A = BuildLabel(ContentAlignment.TopLeft);
		p5A.Left = rightSide;
		p5A.Top = header5.Top + header5.Height + 5;
		p5A.Height = 90;
		p5A.Width = rightWidth;
		p5A.Font = new Font(new FontFamily("Times New Roman"), 12, FontStyle.Italic);
		p5A.Text = "Et hinc patet compositio vis directae AD ex viribus quisbusvis obliquis AB && BD, && vicissim resolutio vis cujusvis directae AD in obliquas quascunque AB && BD. Quae quidem compositio && resolutio abunde confirmatur ex mechanica.";
		p5A.Parent = parent;

	}
	
	private TextBox BuildTextInput()
	{
		TextBox textBox = new TextBox();
		textBox.AcceptsReturn = true;
		textBox.AcceptsTab = true;
		textBox.Multiline = true;
		textBox.BorderStyle = BorderStyle.None;
		return textBox;
	}
	
	private Label BuildLabel(ContentAlignment align)
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
	
	private List<char> digitChars = new List<char> { '0','1','2','3','4','5','6','7','8','9' };
	private List<char> digitSuperscriptChars = new List<char> { '⁰','¹','²','³','⁴','⁵','⁶','⁷','⁸','⁹' };
	private List<char> digitSubscriptChars = new List<char> { '₀','₁','₂','₃','₄','₅','₆','₇','₈','₉' };
	private Dictionary<char, char> convertToSuperscript = new Dictionary<char, char>() {
		{'0','⁰'}, {'1','¹'}, {'2','²'}, {'3','³'}, {'4','⁴'}, {'5','⁵'}, {'6','⁶'}, {'7','⁷'}, {'8','⁸'}, {'9','⁹'}
	};
	private Dictionary<char, char> convertToSubscript = new Dictionary<char, char>() {
		{'0','₀'}, {'1','₁'}, {'2','₂'}, {'3','₃'}, {'4','₄'}, {'5','₅'}, {'6','₆'}, {'7','₇'}, {'8','₈'}, {'9','₉'}
	};
	
	private TextBox integralFromTextBox;
	private TextBox integralToTextBox;
	private TextBox squareRootTextBox;
	private Panel squareRootPanel;
	private bool inSquareRoot = false;
	private int squareRootStartIndex = 0;
	private TextBox suffixTextBox;
	private int formulaStartIndex = 0;
	private bool inFraction = false;
	private TextBox fractionOverTextBox;
	private TextBox fractionUnderTextBox;
	private int lineNumber = 0;
	
	private void TextBoxKeyUp(object sender, KeyEventArgs e)
	{
		TextBox textBox = (sender as TextBox);
		int endIndex = textBox.SelectionStart;
		
		if(sender == integralFromTextBox && e.KeyValue == ' ')
		{
			integralToTextBox.Focus();
			integralToTextBox.SelectionStart = 0;
			return;
		}
		if(sender == integralToTextBox && e.KeyValue == ' ')
		{
			formulaTextBox.Text += "   ";
			formulaTextBox.SelectionStart = formulaTextBox.Text.Length;
			formulaTextBox.Focus();
			return;
		}
		
		if(inSquareRoot)
		{
			if(JustTyped(textBox, ")"))
			{
				inSquareRoot = false;
				textBox.KeyUp -= TextBoxKeyUp;
				textBox.Text = TextSplice(textBox.Text, endIndex-1, 1, "");
				textBox.SelectionStart = endIndex-1;
				textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
				
				suffixTextBox.Text = "##";
				
				return;
			}
			int squareRootCharCount = formulaTextBox.Text.Substring(squareRootStartIndex).Length;
			squareRootPanel.Width = 8*squareRootCharCount;
		}
		if(inFraction)
		{
			if(JustTyped(textBox, "\t"))
			{
				textBox.KeyUp -= TextBoxKeyUp;
				textBox.Text = TextSplice(textBox.Text, endIndex-1, 1, "");
				textBox.SelectionStart = endIndex-1;
				fractionOverTextBox.Width = 47;
				textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
				
				fractionUnderTextBox.Focus();
				fractionUnderTextBox.SelectionStart = 0;

				suffixTextBox.Text = ")##";
				
				return;
			}
			else if(JustTyped(textBox, ")"))
			{
				inFraction = false;
				textBox.KeyUp -= TextBoxKeyUp;
				textBox.Text = TextSplice(textBox.Text, endIndex-1, 1, "");
				textBox.SelectionStart = endIndex-1;
				textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
				
				suffixTextBox.Text = "##";

				formulaTextBox.Text += "      ";
				formulaTextBox.SelectionStart = formulaTextBox.Text.Length;
				formulaTextBox.Focus();
				
				return;
			}
		}

		if(JustTyped(textBox, "integral"))
		{
			textBox.KeyUp -= TextBoxKeyUp;
			textBox.Text = TextSplice(textBox.Text, endIndex-8, 8, "∫");
			textBox.SelectionStart = endIndex-7;

			integralFromTextBox = BuildTextInput();
			integralToTextBox = BuildTextInput();
			
			//integralFromTextBox.BackColor = Color.Yellow;
			integralFromTextBox.Left = 232;
			integralFromTextBox.Top = 158;
			integralFromTextBox.Height = 12;
			integralFromTextBox.Width = 15;
			integralFromTextBox.Font = new Font(new FontFamily("Times New Roman"), 10);
			integralFromTextBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
			integralFromTextBox.Parent = textBox.Parent;
			integralFromTextBox.Focus();
			integralFromTextBox.SelectionStart = 0;
			integralFromTextBox.BringToFront();
		
			//integralToTextBox.BackColor = Color.Yellow;
			integralToTextBox.Left = 232;
			integralToTextBox.Top = 140;
			integralToTextBox.Height = 15;
			integralToTextBox.Width = 15;
			integralToTextBox.Font = new Font(new FontFamily("Times New Roman"), 12);
			integralToTextBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
			integralToTextBox.Parent = textBox.Parent;
			integralToTextBox.BringToFront();
			
			textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
		}
		else if(JustTyped(textBox, "sqrt("))
		{
			textBox.KeyUp -= TextBoxKeyUp;
			textBox.Text = TextSplice(textBox.Text, endIndex-5, 5, "√");
			textBox.SelectionStart = endIndex-4;

			squareRootTextBox = BuildTextInput();
			squareRootTextBox.Font = new Font(new FontFamily("Times New Roman"), 18);
			
			squareRootPanel = new Panel();
			squareRootPanel.Left = 236;
			squareRootPanel.Top = 171;
			squareRootPanel.Height = 5;
			squareRootPanel.Width = 3;
			squareRootPanel.Parent = textBox.Parent;
			squareRootPanel.Paint += new PaintEventHandler(SquareRootPanelPaint);
			squareRootPanel.BringToFront();
			
			inSquareRoot = true;
			squareRootStartIndex = textBox.SelectionStart;
			
			suffixTextBox.Text = ")##";

			textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
		}
		else if(JustTyped(textBox, "frac("))
		{
			textBox.KeyUp -= TextBoxKeyUp;
			textBox.Text = TextSplice(textBox.Text, endIndex-5, 5, "");
			textBox.SelectionStart = endIndex-5;

			fractionUnderTextBox = BuildTextInput();
			//fractionUnderTextBox.BackColor = Color.Yellow;
			fractionUnderTextBox.Left = 220;
			fractionUnderTextBox.Top = 213;
			fractionUnderTextBox.Height = 20;
			fractionUnderTextBox.Width = 47;
			fractionUnderTextBox.Font = new Font(new FontFamily("Times New Roman"), 12);
			fractionUnderTextBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
			fractionUnderTextBox.Parent = textBox.Parent;
			fractionUnderTextBox.BringToFront();
			
			fractionOverTextBox = BuildTextInput();
			//fractionOverTextBox.BackColor = Color.Green;
			fractionOverTextBox.Left = 220;
			fractionOverTextBox.Top = 195;
			fractionOverTextBox.Height = 20;
			fractionOverTextBox.Width = 100;
			fractionOverTextBox.Font = new Font(new FontFamily("Times New Roman"), 12, FontStyle.Underline);
			fractionOverTextBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
			fractionOverTextBox.Parent = textBox.Parent;
			fractionOverTextBox.Focus();
			fractionOverTextBox.SelectionStart = 0;
			fractionOverTextBox.BringToFront();
		
			inFraction = true;
			
			suffixTextBox.Text = "\\t)##";
			suffixTextBox.BringToFront();

			textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
		}
		else if(JustTyped(textBox, "inf"))
		{
			textBox.KeyUp -= TextBoxKeyUp;
			textBox.Text = TextSplice(textBox.Text, endIndex-3, 3, "∞");
			textBox.SelectionStart = endIndex-2;
			textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
		}
		else if(JustTyped(textBox, "pi"))
		{
			textBox.KeyUp -= TextBoxKeyUp;
			textBox.Text = TextSplice(textBox.Text, endIndex-2, 2, "π");
			textBox.SelectionStart = endIndex-1;
			textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
		}
		else if(JustTyped(textBox, "##"))
		{
			formulaStartIndex = endIndex - 2;
		
			textBox.KeyUp -= TextBoxKeyUp;
			textBox.Text = TextSplice(textBox.Text, endIndex-2, 2, "");
			textBox.SelectionStart = endIndex-2;
			textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);

			if(suffixTextBox == null)
			{
				lineNumber++;
			
				Point xy = XYCharPositionInTextBox(textBox, textBox.GetFirstCharIndexFromLine(textBox.GetLineFromCharIndex(textBox.SelectionStart)), textBox.SelectionStart);

				suffixTextBox = BuildTextInput();
				suffixTextBox.Left = xy.X + 25;
					switch(lineNumber)
					{
						case 1: suffixTextBox.Top = 146; break;
						case 2: suffixTextBox.Top = 172; break;
						case 3: suffixTextBox.Top = 199; break;
					}				
				suffixTextBox.Width = 150;
				suffixTextBox.Height = 25;
				suffixTextBox.Text = "##";
				suffixTextBox.Font = new Font("Times New Roman", 18);
				suffixTextBox.ForeColor = Color.LightGray;
				suffixTextBox.Parent = textBox.Parent;
				suffixTextBox.BringToFront();
			}
			else
			{
				suffixTextBox.Text = "";
				suffixTextBox = null;
			}
		}
		else if(digitChars.Contains((char)e.KeyValue) && e.Modifiers != Keys.Shift)
		{
			if(PreviousChar(textBox, '^'))
			{
				textBox.KeyUp -= TextBoxKeyUp;
				textBox.Text = TextSplice(textBox.Text, endIndex-2, 2, convertToSuperscript[(char)e.KeyValue].ToString());
				textBox.SelectionStart = endIndex-1;
				textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
			}
			else if(PreviousChar(textBox, digitSuperscriptChars))
			{
				textBox.KeyUp -= TextBoxKeyUp;
				textBox.Text = TextSplice(textBox.Text, endIndex-1, 1, convertToSuperscript[(char)e.KeyValue].ToString());
				textBox.SelectionStart = endIndex;
				textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
			}
			else if(PreviousChar(textBox, digitSubscriptChars))
			{
				textBox.KeyUp -= TextBoxKeyUp;
				textBox.Text = TextSplice(textBox.Text, endIndex-1, 1, convertToSubscript[(char)e.KeyValue].ToString());
				textBox.SelectionStart = endIndex;
				textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
			}
		}
		
		UpdateSuffixTextBox(textBox);
	}
	
	private void UpdateSuffixTextBox(TextBox textBox)
	{
		if(suffixTextBox == null)
			return;
		Point xy = XYCharPositionInTextBox(textBox, textBox.GetFirstCharIndexFromLine(textBox.GetLineFromCharIndex(textBox.SelectionStart)), textBox.SelectionStart);
		if(lineNumber == 3)
			suffixTextBox.Left = Math.Max(xy.X + 20, suffixTextBox.Left);
		else
			suffixTextBox.Left = xy.X + 20;
	}
	
	private void OverlayPanelPaint(object sender, PaintEventArgs e)
	{
		Brush brush = new SolidBrush(Color.FromArgb(0,213,224,243));
		Font font = new Font("Times New Roman", 18);
		e.Graphics.DrawString("##", font, brush, 15, 15);

	}
	
	private void SquareRootPanelPaint(object sender, PaintEventArgs e)
	{
		Pen pen = new Pen(Color.Black, 1.5F);
		e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
		e.Graphics.DrawLine(pen, 0, 1, (sender as Panel).Width, 1);
	}
	
	private Point XYCharPositionInTextBox(TextBox textBox, int indexLineStart, int indexChar)
	{
		Graphics g = textBox.CreateGraphics();
		TextMeasure textMeasure = new TextMeasure(g, textBox.Font, textBox.Text.Substring(indexLineStart, indexChar-indexLineStart));
		Point xy = new Point(textBox.Left, textBox.Top);
		xy.X += textMeasure.Width;
		xy.Y += textBox.GetLineFromCharIndex(indexLineStart) * textMeasure.Height;
		return xy;
	}
	
	private string TextSplice(string text, int index, int length, string insertText)
	{
		return text.Substring(0, index) + insertText + text.Substring(index+length);
	}
	
	private bool PreviousChar(TextBox textBox, char c)
	{
		int endIndex = textBox.SelectionStart;
		return (textBox.Text.Length > 1 && textBox.Text[endIndex-2] == c);
	}
	
	private bool PreviousChar(TextBox textBox, List<char> c)
	{
		int endIndex = textBox.SelectionStart;
		return (textBox.Text.Length > 1 && c.Contains(textBox.Text[endIndex-2]));
	}
	
	private bool JustTyped(TextBox textBox, string searchText)
	{
		int endIndex = textBox.SelectionStart;
		return (textBox.Text.Length >= searchText.Length && textBox.Text.Substring(endIndex-searchText.Length, searchText.Length) == searchText);
	}
	
	private void OpenModalStyles(object sender, EventArgs e)
	{
		Console.WriteLine("todo: styles modal");
	}
	
	private void PaintTextLayoutDemo(object sender, PaintEventArgs pea)
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

	private void PaintTextBox(Graphics g, Rectangle rect, string text)
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
	
	private void PaintGraphs(object sender, PaintEventArgs pea)
	{
		Bitmap graphicsBuffer = new Bitmap(400, 600);
		Graphics g = Graphics.FromImage(graphicsBuffer);
//		(new ScatterPlot()).Draw(g, graphicsBuffer.Width, graphicsBuffer.Height);
		PaintImage(g, new Rectangle(0,0,400,300), "PieChartSmallMultiples.png");
		PaintImage(g, new Rectangle(0,300,400,300), "PieChartSmallMultiples_grayscale.png");
		g.Dispose();
		pea.Graphics.DrawImageUnscaled(graphicsBuffer, 10, 10);
	}
	
	private void PaintImage(Graphics g, Rectangle rect, string filename)
	{
		if(!File.Exists(filename)) return;
			
		using (Image src = Image.FromFile(filename))
		{
			g.DrawImage(src, rect.X, rect.Y, rect.Width, rect.Height);
		}
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