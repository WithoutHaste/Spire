using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

public static class FormulasDemo
{
	private static List<char> digitChars = new List<char> { '0','1','2','3','4','5','6','7','8','9' };
	private static List<char> digitSuperscriptChars = new List<char> { '⁰','¹','²','³','⁴','⁵','⁶','⁷','⁸','⁹' };
	private static List<char> digitSubscriptChars = new List<char> { '₀','₁','₂','₃','₄','₅','₆','₇','₈','₉' };
	private static Dictionary<char, char> convertToSuperscript = new Dictionary<char, char>() {
		{'0','⁰'}, {'1','¹'}, {'2','²'}, {'3','³'}, {'4','⁴'}, {'5','⁵'}, {'6','⁶'}, {'7','⁷'}, {'8','⁸'}, {'9','⁹'}
	};
	private static Dictionary<char, char> convertToSubscript = new Dictionary<char, char>() {
		{'0','₀'}, {'1','₁'}, {'2','₂'}, {'3','₃'}, {'4','₄'}, {'5','₅'}, {'6','₆'}, {'7','₇'}, {'8','₈'}, {'9','₉'}
	};
	
	private static TextBox formulaTextBox;
	private static TextBox integralFromTextBox;
	private static TextBox integralToTextBox;
	private static TextBox squareRootTextBox;
	private static Panel squareRootPanel;
	private static bool inSquareRoot = false;
	private static int squareRootStartIndex = 0;
	private static TextBox suffixTextBox;
	private static int formulaStartIndex = 0;
	private static bool inFraction = false;
	private static TextBox fractionOverTextBox;
	private static TextBox fractionUnderTextBox;
	private static int lineNumber = 0;
	
	public static void InitializeDemo(Panel parent)
	{
		int margin = 10;
		formulaTextBox = MockupWindow.BuildTextInput();
		formulaTextBox.Left = margin;
		formulaTextBox.Top = margin;
		formulaTextBox.Height = parent.Height - 4*margin;
		formulaTextBox.Width = parent.Width - 4*margin;
		formulaTextBox.Font = new Font(new FontFamily("Times New Roman"), 18);
		formulaTextBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
		formulaTextBox.Parent = parent;
		
		formulaTextBox.Text = "\tThe goal is that the user may type continuously to create text and formulas. The mouse should not be required. This can be achieved with configurable replacement text, such as \"pi\" to π, and layout support for complex forms such as integrals.";
		formulaTextBox.SelectionStart = formulaTextBox.Text.Length;

		inSquareRoot = false;
		squareRootStartIndex = 0;
		formulaStartIndex = 0;
		inFraction = false;
		lineNumber = 0;
	}
	
	public static void TextBoxKeyUp(object sender, KeyEventArgs e)
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

		if(JustTyped(textBox, "integral "))
		{
			textBox.KeyUp -= TextBoxKeyUp;
			textBox.Text = TextSplice(textBox.Text, endIndex-9, 9, "∫");
			textBox.SelectionStart = endIndex-8;

			integralFromTextBox = MockupWindow.BuildTextInput();
			integralToTextBox = MockupWindow.BuildTextInput();
			
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

			squareRootTextBox = MockupWindow.BuildTextInput();
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

			fractionUnderTextBox = MockupWindow.BuildTextInput();
			//fractionUnderTextBox.BackColor = Color.Yellow;
			fractionUnderTextBox.Left = 220;
			fractionUnderTextBox.Top = 213;
			fractionUnderTextBox.Height = 20;
			fractionUnderTextBox.Width = 47;
			fractionUnderTextBox.Font = new Font(new FontFamily("Times New Roman"), 12);
			fractionUnderTextBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
			fractionUnderTextBox.Parent = textBox.Parent;
			fractionUnderTextBox.BringToFront();
			
			fractionOverTextBox = MockupWindow.BuildTextInput();
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

				suffixTextBox = MockupWindow.BuildTextInput();
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
	
	private static void UpdateSuffixTextBox(TextBox textBox)
	{
		if(suffixTextBox == null)
			return;
		Point xy = XYCharPositionInTextBox(textBox, textBox.GetFirstCharIndexFromLine(textBox.GetLineFromCharIndex(textBox.SelectionStart)), textBox.SelectionStart);
		if(lineNumber == 3)
			suffixTextBox.Left = Math.Max(xy.X + 20, suffixTextBox.Left);
		else
			suffixTextBox.Left = xy.X + 20;
	}
	
	private static void OverlayPanelPaint(object sender, PaintEventArgs e)
	{
		Brush brush = new SolidBrush(Color.FromArgb(0,213,224,243));
		Font font = new Font("Times New Roman", 18);
		e.Graphics.DrawString("##", font, brush, 15, 15);

	}
	
	private static void SquareRootPanelPaint(object sender, PaintEventArgs e)
	{
		Pen pen = new Pen(Color.Black, 1.5F);
		e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
		e.Graphics.DrawLine(pen, 0, 1, (sender as Panel).Width, 1);
	}
	
	private static Point XYCharPositionInTextBox(TextBox textBox, int indexLineStart, int indexChar)
	{
		Graphics g = textBox.CreateGraphics();
		TextMeasure textMeasure = new TextMeasure(g, textBox.Font, textBox.Text.Substring(indexLineStart, indexChar-indexLineStart));
		Point xy = new Point(textBox.Left, textBox.Top);
		xy.X += textMeasure.Width;
		xy.Y += textBox.GetLineFromCharIndex(indexLineStart) * textMeasure.Height;
		return xy;
	}
	
	private static string TextSplice(string text, int index, int length, string insertText)
	{
		return text.Substring(0, index) + insertText + text.Substring(index+length);
	}
	
	private static bool PreviousChar(TextBox textBox, char c)
	{
		int endIndex = textBox.SelectionStart;
		return (textBox.Text.Length > 1 && textBox.Text[endIndex-2] == c);
	}
	
	private static bool PreviousChar(TextBox textBox, List<char> c)
	{
		int endIndex = textBox.SelectionStart;
		return (textBox.Text.Length > 1 && c.Contains(textBox.Text[endIndex-2]));
	}
	
	private static bool JustTyped(TextBox textBox, string searchText)
	{
		int endIndex = textBox.SelectionStart;
		return (textBox.Text.Length >= searchText.Length && textBox.Text.Substring(endIndex-searchText.Length, searchText.Length) == searchText);
	}
	
}

