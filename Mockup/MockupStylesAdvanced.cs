using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public static class StylesDemoAdvanced
{
	public static List<SpireStyleAdvanced> Styles;
	private static string fullText = @"The Sphagnopsida, the peat-mosses, comprise the two living genera Ambuchanania and Sphagnum, as well as fossil taxa. However, the genus Sphagnum is a diverse, widespread, and economically important one. These large mosses form extensive acidic bogs in peat swamps. The leaves of Sphagnum have large dead cells alternating with living photosynthetic cells. The dead cells help to store water. Aside from this character, the unique branching, thallose (flat and expanded) protonema, and explosively rupturing sporangium place it apart from other mosses.";
	private static string quoteText = @"Andreaeopsida and Andreaeobryopsida are distinguished by the biseriate (two rows of cells) rhizoids, multiseriate (many rows of cells) protonema, and sporangium that splits along longitudinal lines. Most mosses have capsules that open at the top.";
	private static string secondText = @"Polytrichopsida have leaves with sets of parallel lamellae, flaps of chloroplast-containing cells that look like the fins on a heat sink. These carry out photosynthesis and may help to conserve moisture by partially enclosing the gas exchange surfaces. The Polytrichopsida differ from other mosses in other details of their development and anatomy too, and can also become larger than most other mosses, with e.g. Polytrichum commune forming cushions up to 40 cm (16 in) high. The tallest land moss, a member of the Polytrichidae is probably Dawsonia superba, a native to New Zealand and other parts of Australasia.";
	
	public static void Init()
	{
		if(Styles != null)
			return;
		Styles = new List<SpireStyleAdvanced>();
		Styles.Add(new SpireStyleAdvanced() {
			Name = "Global", FontFamily = "Times New Roman", FontSize = 12, ForeColor = Color.Black, FontStyle = FontStyle.Regular, Indent = 0
		});
		Styles.Add(new SpireStyleAdvanced() {
			Name = "Header 1"
		});
		Styles.Add(new SpireStyleAdvanced() {
			Name = "Text"
		});
		Styles.Add(new SpireStyleAdvanced() {
			Name = "Quotes"
		});
	}

	public static void Paint(object sender, PaintEventArgs pea)
	{
		Bitmap graphicsBuffer = new Bitmap(790, 1500);
		Graphics g = Graphics.FromImage(graphicsBuffer);
		g.Clear(Color.White);
		g.SmoothingMode = SmoothingMode.AntiAlias;
		
		SpireStyleAdvanced headerStyle = LoadStyle("Header 1");
		SpireStyleAdvanced textStyle = LoadStyle("Text");
		SpireStyleAdvanced quoteStyle = LoadStyle("Quotes");
		
		Brush headerBrush = new SolidBrush(headerStyle.GetForeColor());
		Brush textBrush = new SolidBrush(textStyle.GetForeColor());
		Brush quoteBrush = new SolidBrush(quoteStyle.GetForeColor());
		
		Font headerFont = new Font(headerStyle.GetFontFamily(), headerStyle.GetFontSize(), headerStyle.GetFontStyle());
		Font textFont = new Font(textStyle.GetFontFamily(), textStyle.GetFontSize(), textStyle.GetFontStyle());
		Font quoteFont = new Font(quoteStyle.GetFontFamily(), quoteStyle.GetFontSize(), quoteStyle.GetFontStyle());

		int lineHeight = (int)(g.MeasureString("TEST", headerFont).Height);
		int y = 15;
		int x = 15;
		int indent = 30;

		g.DrawString("Classification", headerFont, headerBrush, x+(indent*headerStyle.GetIndent()), y);
		y += lineHeight + 5;

		y = WriteText(g, textFont, textBrush, x+(indent*textStyle.GetIndent()), y, fullText) + 5;
		y = WriteText(g, quoteFont, quoteBrush, x+(indent*quoteStyle.GetIndent()), y, quoteText) + 5;
		y = WriteText(g, textFont, textBrush, x+(indent*textStyle.GetIndent()), y, secondText) + 5;
	
		g.Dispose();
		pea.Graphics.DrawImageUnscaled(graphicsBuffer, 0, 0);	
	}
	
	private static int WriteText(Graphics g, Font font, Brush brush, int x, int y, string text)
	{
		int lineHeight = (int)(g.MeasureString("TEST", font).Height);
		int index = 0;
		int charsPerLine = 70;
		while(index < text.Length-1)
		{
			int endIndex = index + charsPerLine;
			while(endIndex < text.Length && text[endIndex-1] != ' ')
				endIndex++;
			if(endIndex >= text.Length)
				endIndex = text.Length - 1;
			g.DrawString(text.Substring(index, endIndex-index), font, brush, x, y);
			y += lineHeight;
			index = endIndex;
		}
		return y;
	}

	private static SpireStyleAdvanced FindStyle(string name)
	{
		foreach(SpireStyleAdvanced style in Styles)
		{
			if(style.Name == name)
				return style;
		}
		return null;
	}

	private static SpireStyleAdvanced LoadStyle(string name)
	{
		SpireStyleAdvanced global = FindStyle("Global");
		SpireStyleAdvanced local = FindStyle(name);
		return local.Combine(global);
	}
	
}

public class StyleDialogAdvanced : Form
{
	private TextBox variablesControl;
	private ListBox styleListControl;
	private Label styleLabel;
	private TextBox styleControl;
	private Panel previewPanel;
	
	public StyleDialogAdvanced(Panel previewPanel)
	{
		this.previewPanel = previewPanel;
	
		Width = 600;
		Height = 350;
		Text = "Styles";
		Icon = new Icon("SpireIcon1.ico");
		
		int buffer = 15;
		
		Label variablesLabel = new Label();
		variablesLabel.Left = buffer;
		variablesLabel.Top = buffer;
		variablesLabel.Width = 60;
		variablesLabel.Text = "Variables";
		variablesLabel.TextAlign = ContentAlignment.TopLeft;
		variablesLabel.Parent = this;		
		
		variablesControl = new TextBox();
		variablesControl.AcceptsReturn = true;
		variablesControl.AcceptsTab = true;
		variablesControl.Multiline = true;
		variablesControl.Left = variablesLabel.Left;
		variablesControl.Top = EasyLayout.Below(variablesLabel, 5);
		variablesControl.Height = 200;
		variablesControl.Width = 200;
		variablesControl.Parent = this;
		
		Button applyVariables = new Button();
		applyVariables.Text = "Apply Variables";
		applyVariables.Left = variablesControl.Left;
		applyVariables.Top = EasyLayout.Below(variablesControl, 5);
		applyVariables.Width = variablesControl.Width;
		applyVariables.Click += new EventHandler(ApplyVariables);
		applyVariables.Parent = this;
		
		Label styleListLabel = new Label();
		styleListLabel.Left = EasyLayout.LeftOf(variablesControl, buffer);
		styleListLabel.Top = variablesLabel.Top;
		styleListLabel.Width = 50;
		styleListLabel.Text = "Styles";
		styleListLabel.TextAlign = ContentAlignment.TopLeft;
		styleListLabel.Parent = this;		
		
		styleListControl = new ListBox();
		styleListControl.DataSource = new List<string>() { "Global", "Header 1", "Header 2", "Header 3", "Text", "Section Start", "Paragraph Start", "Quotes", "Footnotes", "Sidenotes", "Formula" };
		styleListControl.Left = styleListLabel.Left;
		styleListControl.Top = EasyLayout.Below(styleListLabel, 5);
		styleListControl.Height = 200;
		styleListControl.SelectedIndexChanged += new EventHandler(StyleSelectedChanged);
		styleListControl.Parent = this;
		
		Button addStyle = new Button();
		addStyle.Text = "Add Style";
		addStyle.Left = styleListControl.Left;
		addStyle.Top = EasyLayout.Below(styleListControl, 5);
		addStyle.Width = styleListControl.Width;
		addStyle.Parent = this;
		
		styleLabel = new Label();
		styleLabel.Left = EasyLayout.LeftOf(styleListControl, buffer);
		styleLabel.Top = styleListLabel.Top;
		styleLabel.Width = 100;
		styleLabel.Text = "Style: ";
		styleLabel.TextAlign = ContentAlignment.TopLeft;
		styleLabel.Parent = this;		
		
		styleControl = new TextBox();
		styleControl.AcceptsReturn = true;
		styleControl.AcceptsTab = true;
		styleControl.Multiline = true;
		styleControl.Left = styleLabel.Left;
		styleControl.Top = EasyLayout.Below(styleLabel, 5);
		styleControl.Height = 200;
		styleControl.Width = 200;
		styleControl.Parent = this;
		
		Button applyStyle = new Button();
		applyStyle.Text = "Apply Style";
		applyStyle.Left = styleControl.Left;
		applyStyle.Top = EasyLayout.Below(styleControl, 5);
		applyStyle.Width = styleControl.Width;
		applyStyle.Click += new EventHandler(ApplyStyle);
		applyStyle.Parent = this;
		
		Button showHelp = new Button();
		showHelp.Text = "?";
		showHelp.Width = 30;
		showHelp.Height = styleLabel.Height;
		showHelp.Left = styleControl.Left + styleControl.Width - showHelp.Width;
		showHelp.Top = styleLabel.Top;
		showHelp.Click += new EventHandler(ShowHelp);
		showHelp.Parent = this;
		
		Button close = new Button();
		close.Text = "Close";
		close.Left = applyStyle.Left;
		close.Top = EasyLayout.Below(applyStyle, buffer);
		close.Width = applyStyle.Width;
		close.Click += (sender, e) => { this.Close(); };
		close.Parent = this;

		this.Load += new EventHandler(StyleSelectedChanged);
		
		ShowDialog();
	}
	
	private void ShowHelp(object sender, EventArgs e)
	{
		
	}
	
	private void ApplyVariables(object sender, EventArgs e)
	{
		foreach(string line in variablesControl.Text.Split('\n'))
		{
			string[] fields = line.Split('=');
			if(fields.Length < 2) continue;
			string fieldName = Regex.Replace(fields[0], @"[\s\t]+", "");
			fields[0] = "";
			SpireStyleAdvanced.Variables[fieldName] = (String.Join(" ", fields)).Trim();
		}
		variablesControl.Text = FormatVariables();
		previewPanel.Invalidate();
	}
	
	private string FormatVariables()
	{
		List<string> lines = new List<string>();
		foreach(KeyValuePair<string, string> pair in SpireStyleAdvanced.Variables)
		{
			lines.Add(String.Format("{0} = {1}", pair.Key, pair.Value));
		}
		return String.Join(Environment.NewLine, lines.ToArray());
	}
	
	private void ApplyStyle(object sender, EventArgs e)
	{
		SpireStyleAdvanced style = FindSelectedStyle();
		foreach(string line in styleControl.Text.Split('\n'))
		{
			string[] fields = line.Split('=');
			if(fields.Length < 2) continue;
			string fieldName = Regex.Replace(fields[0], @"[\s\t]+", "");
			fields[0] = "";
			style.ApplyField(fieldName, String.Join(" ", fields));
		}
		DisplayStyle();		
		previewPanel.Invalidate();
	}
	
	private void StyleSelectedChanged(object sender, EventArgs e)
	{
		DisplayStyle();
	}
	
	private void DisplayStyle()
	{
		SpireStyleAdvanced style = FindSelectedStyle();
		styleLabel.Text = "Style: ";
		styleControl.Text = "";
		if(style == null)
			return;
		styleLabel.Text += style.Name;
		styleControl.Text = style.ToText();
	}
	
	private SpireStyleAdvanced FindSelectedStyle()
	{
		return FindStyle(styleListControl.SelectedItem.ToString());
	}
		
	private SpireStyleAdvanced FindStyle(string name)
	{
		foreach(SpireStyleAdvanced style in StylesDemoAdvanced.Styles)
		{
			if(style.Name == name)
				return style;
		}
		return null;
	}

}


public class SpireStyleAdvanced
{
	public SpireStyleAdvanced()
	{
	}
	
	public static Dictionary<string, string> Variables = new Dictionary<string, string>();
	
	public string Name { get; set; }
	
	public string FontFamily { get; set; }
	public float? FontSize { get; set; }
	public Color? ForeColor { get; set; }
	public FontStyle? FontStyle { get; set; }
	public int? Indent { get; set; }

	public string FontFamilyVariable { get; set; }
	public string FontSizeVariable { get; set; }
	public string ForeColorVariable { get; set; }
	public string FontStyleVariable { get; set; }
	public string IndentVariable { get; set; }
	
	private static Dictionary<string, Color> Colors = new Dictionary<string, Color>() {
		{"red", Color.Red}, {"blue", Color.Blue}, {"yellow", Color.Yellow}, {"green", Color.Green}, {"purple", Color.Purple}, {"orange",Color.Orange}, {"white", Color.White}, {"gray", Color.Gray}, {"grey", Color.Gray}, {"black", Color.Black}
	};

	public string GetFontFamily()
	{
		if(!String.IsNullOrEmpty(FontFamily))
			return FontFamily;
		if(!String.IsNullOrEmpty(FontFamilyVariable))
			return Variables[FontFamilyVariable];
		return null;
	}
	
	public float GetFontSize()
	{
		if(FontSize != null)
			return FontSize.Value;
		if(!String.IsNullOrEmpty(FontSizeVariable))
			return ParseFloat(Variables[FontSizeVariable]).Value;
		return 0;
	}
	
	public Color GetForeColor()
	{
		if(ForeColor != null)
			return ForeColor.Value;
		if(!String.IsNullOrEmpty(ForeColorVariable))
			return ParseColor(Variables[ForeColorVariable]).Value;
		return Color.White;
	}
	
	public FontStyle GetFontStyle()
	{
		if(FontStyle != null)
			return FontStyle.Value;
		if(!String.IsNullOrEmpty(FontStyleVariable))
			return ParseFontStyle(Variables[FontStyleVariable]).Value;
		return System.Drawing.FontStyle.Regular;
	}
	
	public int GetIndent()
	{
		if(Indent != null)
			return Indent.Value;
		if(!String.IsNullOrEmpty(IndentVariable))
			return ParseInt(Variables[IndentVariable]).Value;
		return 0;
	}
		
	public SpireStyleAdvanced Combine(SpireStyleAdvanced baseStyle)
	{
		SpireStyleAdvanced combined = Clone();
		if(String.IsNullOrEmpty(FontFamily) && String.IsNullOrEmpty(FontFamilyVariable))
		{
			combined.FontFamily = baseStyle.FontFamily;
			combined.FontFamilyVariable = baseStyle.FontFamilyVariable;
		}
		if(FontSize == null && String.IsNullOrEmpty(FontSizeVariable))
		{
			combined.FontSize = baseStyle.FontSize;
			combined.FontSizeVariable = baseStyle.FontSizeVariable;
		}
		if(ForeColor == null && String.IsNullOrEmpty(ForeColorVariable))
		{
			combined.ForeColor = baseStyle.ForeColor;
			combined.ForeColorVariable = baseStyle.ForeColorVariable;
		}
		if(FontStyle == null && String.IsNullOrEmpty(FontStyleVariable))
		{
			combined.FontStyle = baseStyle.FontStyle;
			combined.FontStyleVariable = baseStyle.FontStyleVariable;
		}
		if(Indent == null && String.IsNullOrEmpty(IndentVariable))
		{
			combined.Indent = baseStyle.Indent;
			combined.IndentVariable = baseStyle.IndentVariable;
		}

		return combined;
	}
	
	public SpireStyleAdvanced Clone()
	{
		return new SpireStyleAdvanced() {
			Name = Name
			, FontFamily = FontFamily
			, FontSize = FontSize
			, ForeColor = ForeColor
			, FontStyle = FontStyle
			, Indent = Indent
			, FontFamilyVariable = FontFamilyVariable
			, FontSizeVariable = FontSizeVariable
			, ForeColorVariable = ForeColorVariable
			, FontStyleVariable = FontStyleVariable
			, IndentVariable = IndentVariable
		};
	}
	
	public string ToText()
	{
		List<string> lines = new List<string>();
		if(!String.IsNullOrEmpty(FontFamily))
			lines.Add("Font = "+FontFamily);
		else if(!String.IsNullOrEmpty(FontFamilyVariable))
			lines.Add("Font = "+FontFamilyVariable);
			
		if(FontSize != null)
			lines.Add("FontSize = "+FontSize);
		else if(!String.IsNullOrEmpty(FontSizeVariable))
			lines.Add("FontSize = "+FontSizeVariable);

		if(ForeColor != null)
			lines.Add("FontColor = "+SpireStyleAdvanced.ColorToRGB(ForeColor.Value));
		else if(!String.IsNullOrEmpty(ForeColorVariable))
			lines.Add("FontColor = "+ForeColorVariable);

		if(FontStyle != null)
			lines.Add("FontStyle = "+SpireStyleAdvanced.FontStyleToString(FontStyle.Value));
		else if(!String.IsNullOrEmpty(FontStyleVariable))
			lines.Add("FontStyle = "+FontStyleVariable);
			
		if(Indent != null)
			lines.Add("Indent = "+Indent);
		else if(!String.IsNullOrEmpty(IndentVariable))
			lines.Add("Indent = "+IndentVariable);
			
		return String.Join(Environment.NewLine, lines.ToArray());
	}
	
	public void ApplyField(string fieldName, string fieldValue)
	{
		fieldName = fieldName.ToLower();
		fieldValue = fieldValue.Trim();
		
		switch(fieldName)
		{
			case "font": 
			case "fontfamily": 
				if(Variables.ContainsKey(fieldValue))
				{
					FontFamilyVariable = fieldValue;
					FontFamily = null;
				}
				else
				{
					FontFamilyVariable = null;
					FontFamily = fieldValue;
				}
				break;
			case "size":
			case "fontsize":
				if(Variables.ContainsKey(fieldValue))
				{
					FontSizeVariable = fieldValue;
					FontSize = null;
				}
				else
				{
					float? fontSize = ParseFloat(fieldValue);
					if(fontSize != null)
					{
						FontSizeVariable = null;
						FontSize = fontSize;
					}
				}
				break;
			case "color":
			case "fontcolor":
			case "textcolor":
				if(Variables.ContainsKey(fieldValue))
				{
					ForeColorVariable = fieldValue;
					ForeColor = null;
				}
				else
				{
					Color? color = ParseColor(fieldValue);
					if(color != null)
					{
						ForeColor = color;
						ForeColorVariable = null;
					}
				}
				break;
			case "style":
			case "fontstyle":
				if(Variables.ContainsKey(fieldValue))
				{
					FontStyleVariable = fieldValue;
					FontStyle = null;
				}
				else
				{
					FontStyle? fontStyle = ParseFontStyle(fieldValue);
					if(fontStyle != null)
					{
						FontStyle = fontStyle;
						FontStyleVariable = null;
					}
				}
				break;
			case "indent":
				if(Variables.ContainsKey(fieldValue))
				{
					IndentVariable = fieldValue;
					Indent = null;
				}
				else
				{
					int? indent = ParseInt(fieldValue);
					if(indent != null)
					{
						Indent = indent;
						IndentVariable = null;
					}
				}
				break;	
		}
	}
	
	public static string FontStyleToString(FontStyle style)
	{
		List<string> terms = new List<string>();
		if((style & System.Drawing.FontStyle.Bold) == System.Drawing.FontStyle.Bold)
			terms.Add("bold");
		if((style & System.Drawing.FontStyle.Italic) == System.Drawing.FontStyle.Italic)
			terms.Add("italic");
		if(terms.Count == 0)
			terms.Add("none");
		return String.Join(" ", terms.ToArray());
	}

	public static string ColorToRGB(Color color)
	{
		return String.Format("{0} {1} {2}", color.R, color.G, color.B);
	}

	public static Color RGBToColor(string s)
	{
		string[] rgb = s.Split(new char[] {' ',',','.','-'});
		if(rgb.Length == 3)
			return Color.FromArgb(255, Int32.Parse(rgb[0]), Int32.Parse(rgb[1]), Int32.Parse(rgb[2]));
		throw new Exception("String is not formatted as a color");
	}
	
	private int? ParseInt(string value)
	{
		int i;
		if(Int32.TryParse(value, out i))
		{
			return i;
		}
		return null;
	}
	
	private float? ParseFloat(string value)
	{
		float f;
		if(Single.TryParse(value, out f))
		{
			return f;
		}
		return null;
	}
	
	private Color? ParseColor(string value)
	{
		Color? c = null;
		try
		{
			c = SpireStyleAdvanced.RGBToColor(value);
		}
		catch(Exception)
		{
			value = value.ToLower();
			if(Colors.ContainsKey(value))
			{	
				c = Colors[value];
			}
		}
		return c;
	}
	
	private FontStyle? ParseFontStyle(string value)
	{
		FontStyle s = System.Drawing.FontStyle.Regular;
		value = value.ToLower();
		if(value.IndexOf("bold") > -1)
			s = s | System.Drawing.FontStyle.Bold;
		if(value.IndexOf("italic") > -1)
			s = s | System.Drawing.FontStyle.Italic;
		if(value.IndexOf("none") > -1 || value.IndexOf("regular") > -1)
			s = System.Drawing.FontStyle.Regular;
		return s;
	}

}
