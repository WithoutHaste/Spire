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
	
	private static int deltaX = -130;
	
	private static TextBox formulaTextBox;
	private static Label showType;
	private static bool startedFormula = false;
	private static bool inIntegral = false;
	private static TextBox integralFromTextBox;
	private static TextBox integralToTextBox;
	private static TextBox squareRootTextBox;
	private static Panel squareRootPanel;
	private static bool inSquareRoot = false;
	private static int squareRootStartIndex = 0;
	private static TextBox suffixTextBox;
	private static int formulaStartIndex = 0;
	private static bool inFraction = false;
	private static bool afterFraction = false;
	private static TextBox fractionOverTextBox;
	private static TextBox fractionUnderTextBox;
	
	private static int baseFontSize = 28;
	
	public static void InitializeDemo(Panel parent)
	{
		int margin = 10;
		
		formulaTextBox = MockupWindow.BuildTextInput();
		formulaTextBox.Left = margin;
		formulaTextBox.Top = 30;
		formulaTextBox.Height = 50;
		formulaTextBox.Width = parent.Width - 4*margin;
		formulaTextBox.Font = new Font(new FontFamily("Times New Roman"), baseFontSize);
		formulaTextBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
		formulaTextBox.Text = "";
		formulaTextBox.SelectionStart = formulaTextBox.Text.Length;
		formulaTextBox.Parent = parent;
		
		showType = MockupWindow.BuildLabel(ContentAlignment.MiddleLeft);
		showType.Left = formulaTextBox.Left;
		showType.Top = formulaTextBox.Top + formulaTextBox.Height + margin;
		showType.Height = 70;
		showType.Width = parent.Width - 4*margin;
		showType.Font = new Font(new FontFamily("Times New Roman"), baseFontSize, FontStyle.Bold);
		showType.BackColor = Color.LightGray;
		showType.ForeColor = Color.Black;
		showType.Text = "Typed: ";
		showType.Parent = parent;
		showType.BringToFront();
		
		inIntegral = false;
		inSquareRoot = false;
		squareRootStartIndex = 0;
		formulaStartIndex = 0;
		inFraction = false;
		afterFraction = false;
	}
	
	private static void UpdateShowType(KeyEventArgs e)
	{
		if(e.KeyValue == 51 && e.Shift)
			startedFormula = true;
	
		if(!startedFormula)
			return;
	
		if(e.KeyValue == '6' && e.Shift)
			showType.Text += '^';
		else if(e.KeyValue == '9' && e.Shift)
			showType.Text += '(';
		else if(e.KeyValue == '0' && e.Shift)
			showType.Text += ')';
		else if(e.KeyValue == '3' && e.Shift)
			showType.Text += '#';
		else if(e.KeyValue == 187 && e.Shift)
			showType.Text += '+';
		else if(e.KeyValue == 189)
			showType.Text += '-';
		else if(e.KeyValue == '\t' || e.KeyValue == 9)
			showType.Text += " \\t ";
		else
			showType.Text += ((char)e.KeyValue).ToString().ToLower();
		Console.WriteLine(e.KeyValue+" "+e.KeyCode+" "+e.KeyData);
	}
	
	public static void TextBoxKeyUp(object sender, KeyEventArgs e)
	{
		UpdateShowType(e);
	
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
			squareRootPanel.Width = 12*squareRootCharCount;
		}
		if(inFraction)
		{
			if(JustTyped(textBox, "\t"))
			{
				textBox.KeyUp -= TextBoxKeyUp;
				textBox.Text = TextSplice(textBox.Text, endIndex-1, 1, "");
				textBox.SelectionStart = endIndex-1;
				fractionOverTextBox.Width = 55;
				textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
				
				fractionUnderTextBox.Focus();
				fractionUnderTextBox.SelectionStart = 0;

				suffixTextBox.Text = ")##";
				
				return;
			}
			else if(JustTyped(textBox, ")"))
			{
				inFraction = false;
				afterFraction = true;
				textBox.KeyUp -= TextBoxKeyUp;
				textBox.Text = TextSplice(textBox.Text, endIndex-1, 1, "");
				textBox.SelectionStart = endIndex-1;
				textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
				
				suffixTextBox.Text = "##";

				formulaTextBox.Text += "       ";
				formulaTextBox.SelectionStart = formulaTextBox.Text.Length;
				formulaTextBox.Focus();
				
				return;
			}
		}
		if(inIntegral)
		{
			if(JustTyped(textBox, ")"))
			{
				inIntegral = false;
				textBox.KeyUp -= TextBoxKeyUp;
				textBox.Text = TextSplice(textBox.Text, endIndex-1, 1, "");
				textBox.SelectionStart = endIndex-1;
				textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
				
				suffixTextBox.Text = "##";
				
				return;
			}
		}

		if(JustTyped(textBox, "integral("))
		{
			textBox.KeyUp -= TextBoxKeyUp;
			textBox.Text = TextSplice(textBox.Text, endIndex-9, 9, "∫");
			textBox.SelectionStart = endIndex-8;

			integralFromTextBox = MockupWindow.BuildTextInput();
			integralToTextBox = MockupWindow.BuildTextInput();
			
			//integralFromTextBox.BackColor = Color.Yellow;
			integralFromTextBox.Left = 264 + deltaX;
			integralFromTextBox.Top = 50;
			integralFromTextBox.Height = 18;
			integralFromTextBox.Width = 22;
			integralFromTextBox.Font = new Font(new FontFamily("Times New Roman"), 14);
			integralFromTextBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
			integralFromTextBox.Parent = textBox.Parent;
			integralFromTextBox.Focus();
			integralFromTextBox.SelectionStart = 0;
			integralFromTextBox.BringToFront();
		
			//integralToTextBox.BackColor = Color.Green;
			integralToTextBox.Left = integralFromTextBox.Left;
			integralToTextBox.Top = 30;
			integralToTextBox.Height = integralFromTextBox.Height;
			integralToTextBox.Width = integralFromTextBox.Width;
			integralToTextBox.Font = integralFromTextBox.Font;
			integralToTextBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
			integralToTextBox.Parent = textBox.Parent;
			integralToTextBox.BringToFront();
			
			inIntegral = true;
			suffixTextBox.Text = ")##";

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
			squareRootPanel.Left = 273 + deltaX;
			squareRootPanel.Top = 30;
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
			fractionUnderTextBox.Left = 255 + deltaX;
			fractionUnderTextBox.Top = 51;
			fractionUnderTextBox.Height = 25;
			fractionUnderTextBox.Width = 70;
			fractionUnderTextBox.Font = new Font(new FontFamily("Times New Roman"), 18);
			fractionUnderTextBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);
			fractionUnderTextBox.Parent = textBox.Parent;
			fractionUnderTextBox.BringToFront();
			
			fractionOverTextBox = MockupWindow.BuildTextInput();
			//fractionOverTextBox.BackColor = Color.Green;
			fractionOverTextBox.Left = fractionUnderTextBox.Left;
			fractionOverTextBox.Top = fractionUnderTextBox.Top - fractionUnderTextBox.Height;
			fractionOverTextBox.Height = fractionUnderTextBox.Height;
			fractionOverTextBox.Width = 100;
			fractionOverTextBox.Font = new Font(new FontFamily("Times New Roman"), 18, FontStyle.Underline);
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
			afterFraction = false;
			formulaStartIndex = endIndex - 2;
		
			textBox.KeyUp -= TextBoxKeyUp;
			textBox.Text = TextSplice(textBox.Text, endIndex-2, 2, "");
			textBox.SelectionStart = endIndex-2;
			textBox.KeyUp += new KeyEventHandler(TextBoxKeyUp);

			if(suffixTextBox == null)
			{
				Point xy = XYCharPositionInTextBox(textBox, textBox.GetFirstCharIndexFromLine(textBox.GetLineFromCharIndex(textBox.SelectionStart)), textBox.SelectionStart);

				suffixTextBox = MockupWindow.BuildTextInput();
				suffixTextBox.Left = xy.X + 30;
				suffixTextBox.Top = 28;
				suffixTextBox.Width = 150;
				suffixTextBox.Height = 40;
				suffixTextBox.Text = "##";
				suffixTextBox.Font = formulaTextBox.Font;
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
		if(inFraction || afterFraction)
		{
			suffixTextBox.Left = Math.Max(xy.X + 30, suffixTextBox.Left);
		}
		else
		{
			suffixTextBox.Left = xy.X + 30;
		}
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

