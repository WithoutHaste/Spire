using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public static class StylesDemo
{
	public static List<SpireStyle> Styles;
	private static string fullText = @"The Sphagnopsida, the peat-mosses, comprise the two living genera Ambuchanania and Sphagnum, as well as fossil taxa. However, the genus Sphagnum is a diverse, widespread, and economically important one. These large mosses form extensive acidic bogs in peat swamps. The leaves of Sphagnum have large dead cells alternating with living photosynthetic cells. The dead cells help to store water. Aside from this character, the unique branching, thallose (flat and expanded) protonema, and explosively rupturing sporangium place it apart from other mosses.";
	private static string quoteText = @"Andreaeopsida and Andreaeobryopsida are distinguished by the biseriate (two rows of cells) rhizoids, multiseriate (many rows of cells) protonema, and sporangium that splits along longitudinal lines. Most mosses have capsules that open at the top.";
	private static string secondText = @"Polytrichopsida have leaves with sets of parallel lamellae, flaps of chloroplast-containing cells that look like the fins on a heat sink. These carry out photosynthesis and may help to conserve moisture by partially enclosing the gas exchange surfaces. The Polytrichopsida differ from other mosses in other details of their development and anatomy too, and can also become larger than most other mosses, with e.g. Polytrichum commune forming cushions up to 40 cm (16 in) high. The tallest land moss, a member of the Polytrichidae is probably Dawsonia superba, a native to New Zealand and other parts of Australasia.";
	
	public static void Init()
	{
		if(Styles != null)
			return;
		Styles = new List<SpireStyle>();
		Styles.Add(new SpireStyle() {
			Name = "Header 1", FontFamily = "Times New Roman", FontSize = 12, ForeColor = Color.Black, FontStyle = FontStyle.Regular, Alignment = ContentAlignment.TopCenter, Indent = 0
		});
		Styles.Add(new SpireStyle() {
			Name = "Text", FontFamily = "Times New Roman", FontSize = 12, ForeColor = Color.Black, FontStyle = FontStyle.Regular, Alignment = ContentAlignment.TopCenter, Indent = 0
		});
		Styles.Add(new SpireStyle() {
			Name = "Quotes", FontFamily = "Times New Roman", FontSize = 12, ForeColor = Color.Black, FontStyle = FontStyle.Regular, Alignment = ContentAlignment.TopCenter, Indent = 0
		});
	}

	public static void Paint(object sender, PaintEventArgs pea)
	{
		Bitmap graphicsBuffer = new Bitmap(790, 1500);
		Graphics g = Graphics.FromImage(graphicsBuffer);
		g.Clear(Color.White);
		g.SmoothingMode = SmoothingMode.AntiAlias;
		
		SpireStyle headerStyle = FindStyle("Header 1");
		SpireStyle textStyle = FindStyle("Text");
		SpireStyle quoteStyle = FindStyle("Quotes");
		
		Brush headerBrush = new SolidBrush(headerStyle.ForeColor.Value);
		Brush textBrush = new SolidBrush(textStyle.ForeColor.Value);
		Brush quoteBrush = new SolidBrush(quoteStyle.ForeColor.Value);
		
		Font headerFont = new Font(headerStyle.FontFamily, headerStyle.FontSize.Value, headerStyle.FontStyle.Value);
		Font textFont = new Font(textStyle.FontFamily, textStyle.FontSize.Value, textStyle.FontStyle.Value);
		Font quoteFont = new Font(quoteStyle.FontFamily, quoteStyle.FontSize.Value, quoteStyle.FontStyle.Value);

		int lineHeight = (int)(g.MeasureString("TEST", headerFont).Height);
		int y = 15;
		int x = 15;
		int indent = 30;

		g.DrawString("Classification", headerFont, headerBrush, x+(indent*headerStyle.Indent.Value), y);
		y += lineHeight + 5;

		y = WriteText(g, textFont, textBrush, x+(indent*textStyle.Indent.Value), y, fullText) + 5;
		y = WriteText(g, quoteFont, quoteBrush, x+(indent*quoteStyle.Indent.Value), y, quoteText) + 5;
		y = WriteText(g, textFont, textBrush, x+(indent*textStyle.Indent.Value), y, secondText) + 5;
	
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

	private static SpireStyle FindStyle(string name)
	{
		foreach(SpireStyle style in Styles)
		{
			if(style.Name == name)
				return style;
		}
		return null;
	}
	
}

public class StyleDialog : Form
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
	private Panel previewPanel;
	
	public StyleDialog(Panel previewPanel)
	{
		this.previewPanel = previewPanel;
	
		Width = 400;
		Height = 300;
		Text = "Styles";
		Icon = new Icon("SpireIcon1.ico");
		
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
		
		ShowDialog();
	}
	
	private void StyleSelectedChanged(object sender, EventArgs e)
	{
		DisplayStyle();
	}
	
	private void DisplayStyle()
	{
		SpireStyle style = FindStyle(styleBox.SelectedItem.ToString());
		if(style == null)
			return;
		SetValue(fontFamilyBox, style.FontFamily);
		fontSizeInput.Text = style.FontSize.ToString();
		
		foreColorInput.Text = SpireStyle.ColorToRGB(style.ForeColor.Value);
		foreColorPreview.BackColor = style.ForeColor.Value;
		
		boldCheckBox.Checked = ((style.FontStyle.Value & FontStyle.Bold) == FontStyle.Bold);
		italicCheckBox.Checked = ((style.FontStyle.Value & FontStyle.Italic) == FontStyle.Italic);
		
		indentInput.Text = style.Indent.ToString();
	}
	
	private void FontFamilyChanged(object sender, EventArgs e)
	{
		SpireStyle style = FindStyle(styleBox.SelectedItem.ToString());
		if(style == null)
			throw new Exception("No style selected");
		style.FontFamily = (sender as ComboBox).SelectedItem.ToString();
		previewPanel.Invalidate();
	}
	
	private void FontSizeChanged(object sender, EventArgs e)
	{
		SpireStyle style = FindStyle(styleBox.SelectedItem.ToString());
		if(style == null)
			throw new Exception("No style selected");
		float fontSize = style.FontSize.Value;
		if(Single.TryParse((sender as TextBox).Text, out fontSize))
		{
			style.FontSize = fontSize;
		}
		else
		{
			(sender as TextBox).Text = style.FontSize.ToString();
		}
		previewPanel.Invalidate();
	}
	
	private void ForeColorStyleChanged(object sender, EventArgs e)
	{
		SpireStyle style = FindStyle(styleBox.SelectedItem.ToString());
		if(style == null)
			throw new Exception("No style selected");
		try
		{
			style.ForeColor = SpireStyle.RGBToColor((sender as TextBox).Text);
			foreColorPreview.BackColor = style.ForeColor.Value;
		}
		catch(Exception)
		{
			foreColorInput.Text = SpireStyle.ColorToRGB(style.ForeColor.Value);
		}
		previewPanel.Invalidate();
	}
	
	private void BoldChanged(object sender, EventArgs e)
	{
		SpireStyle style = FindStyle(styleBox.SelectedItem.ToString());
		if(style == null)
			throw new Exception("No style selected");
		if((sender as CheckBox).Checked)
		{
			style.FontStyle = style.FontStyle | FontStyle.Bold;
		}
		else
		{
			style.FontStyle = style.FontStyle & ~FontStyle.Bold;
		}
		previewPanel.Invalidate();
	}
	
	private void ItalicChanged(object sender, EventArgs e)
	{
		SpireStyle style = FindStyle(styleBox.SelectedItem.ToString());
		if(style == null)
			throw new Exception("No style selected");
		if((sender as CheckBox).Checked)
		{
			style.FontStyle = style.FontStyle | FontStyle.Italic;
		}
		else
		{
			style.FontStyle = style.FontStyle & ~FontStyle.Italic;
		}
		previewPanel.Invalidate();
	}
	
	private void IndentChanged(object sender, EventArgs e)
	{
		SpireStyle style = FindStyle(styleBox.SelectedItem.ToString());
		if(style == null)
			throw new Exception("No style selected");
		int indent = style.Indent.Value;
		if(Int32.TryParse((sender as TextBox).Text, out indent))
		{
			style.Indent = indent;
		}
		else
		{
			(sender as TextBox).Text = style.Indent.ToString();
		}
		previewPanel.Invalidate();
	}
	
	private SpireStyle FindStyle(string name)
	{
		foreach(SpireStyle style in StylesDemo.Styles)
		{
			if(style.Name == name)
				return style;
		}
		return null;
	}
	
	private void SetValue(ComboBox box, string value)
	{
		box.SelectedIndex = box.FindString(value);
	}
	
}

public class SpireStyle
{
	public SpireStyle()
	{
	}
	
	public string Name { get; set; }
	public string FontFamily { get; set; }
	public float? FontSize { get; set; }
	public Color? ForeColor { get; set; }
	public FontStyle? FontStyle { get; set; }
	public ContentAlignment? Alignment { get; set; }
	public int? Indent { get; set; }

	public SpireStyle Combine(SpireStyle baseStyle)
	{
		SpireStyle combined = Clone();
		if(String.IsNullOrEmpty(FontFamily))
			combined.FontFamily = baseStyle.FontFamily;
		if(FontSize == null)
			combined.FontSize = baseStyle.FontSize;
		if(ForeColor == null)
			combined.ForeColor = baseStyle.ForeColor;
		if(FontStyle == null)
			combined.FontStyle = baseStyle.FontStyle;
		if(Alignment == null)
			combined.Alignment = baseStyle.Alignment;
		if(Indent == null)
			combined.Indent = baseStyle.Indent;
		return combined;
	}
	
	public SpireStyle Clone()
	{
		return new SpireStyle() {
			Name = Name
			, FontFamily = FontFamily
			, FontSize = FontSize
			, ForeColor = ForeColor
			, FontStyle = FontStyle
			, Alignment = Alignment
			, Indent = Indent
		};
	}
	
	public string ToText()
	{
		List<string> lines = new List<string>();
		if(!String.IsNullOrEmpty(FontFamily))
			lines.Add("Font="+FontFamily);
		if(FontSize != null)
			lines.Add("FontSize="+FontSize);
		if(ForeColor != null)
			lines.Add("TextColor="+SpireStyle.ColorToRGB(ForeColor.Value));
		if(FontStyle != null)
			lines.Add("FontStyle="+SpireStyle.FontStyleToString(FontStyle.Value));
		if(Indent != null)
			lines.Add("Indent="+Indent);
		return String.Join("\n", lines.ToArray());
	}
	
	public void ApplyField(string fieldName, string fieldValue)
	{
		fieldName = fieldName.ToLower();
		fieldValue = fieldValue.Trim();
		switch(fieldName)
		{
			case "fontfamily": 
				FontFamily = fieldValue;
				break;
			case "fontsize":
				float fontSize;
				if(Single.TryParse(fieldValue, out fontSize))
					FontSize = fontSize;
				else
					FontSize = null;
				break;	
			case "fontcolor":
			case "textcolor": 
				try
				{
					ForeColor = SpireStyle.RGBToColor(fieldValue);
				}
				catch(Exception)
				{
					ForeColor = null;
				}
				break;
			case "fontstyle":
				FontStyle = System.Drawing.FontStyle.Regular;
				fieldValue = fieldValue.ToLower();
				if(fieldValue.IndexOf("bold") > -1)
					FontStyle = FontStyle | System.Drawing.FontStyle.Bold;
				if(fieldValue.IndexOf("italic") > -1)
					FontStyle = FontStyle | System.Drawing.FontStyle.Italic;
				if(fieldValue.IndexOf("none") > -1 || fieldValue.IndexOf("regular") > -1)
					FontStyle = System.Drawing.FontStyle.Regular;
				break;
			case "indent":
				int indent;
				if(Int32.TryParse(fieldValue, out indent))
					Indent = indent;
				else
					Indent = null;
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

}

