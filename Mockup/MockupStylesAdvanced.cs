using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public static class StylesDemoAdvanced
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
			Name = "Global", FontFamily = "Times New Roman", FontSize = 12, ForeColor = Color.Black, FontStyle = FontStyle.Regular, Alignment = ContentAlignment.TopCenter, Indent = 0
		});
		Styles.Add(new SpireStyle() {
			Name = "Header 1"
		});
		Styles.Add(new SpireStyle() {
			Name = "Text"
		});
		Styles.Add(new SpireStyle() {
			Name = "Quotes"
		});
	}

	public static void Paint(object sender, PaintEventArgs pea)
	{
		Bitmap graphicsBuffer = new Bitmap(790, 1500);
		Graphics g = Graphics.FromImage(graphicsBuffer);
		g.Clear(Color.White);
		g.SmoothingMode = SmoothingMode.AntiAlias;
		
		SpireStyle headerStyle = LoadStyle("Header 1");
		SpireStyle textStyle = LoadStyle("Text");
		SpireStyle quoteStyle = LoadStyle("Quotes");
		
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

	private static SpireStyle LoadStyle(string name)
	{
		SpireStyle global = FindStyle("Global");
		SpireStyle local = FindStyle(name);
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
	
	private void ApplyStyle(object sender, EventArgs e)
	{
		SpireStyle style = FindSelectedStyle();
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
		SpireStyle style = FindSelectedStyle();
		styleLabel.Text = "Style: ";
		styleControl.Text = "";
		if(style == null)
			return;
		styleLabel.Text += style.Name;
		styleControl.Text = style.ToText();
	}
	
	private SpireStyle FindSelectedStyle()
	{
		return FindStyle(styleListControl.SelectedItem.ToString());
	}
		
	private SpireStyle FindStyle(string name)
	{
		foreach(SpireStyle style in StylesDemoAdvanced.Styles)
		{
			if(style.Name == name)
				return style;
		}
		return null;
	}

}

