using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class StyleDialog : Form
{
	public List<SpireStyle> Styles = new List<SpireStyle>();
	
	private ListBox styleBox;
	private Label styleHeader;
	private ComboBox fontFamilyBox;
	private TextBox fontSizeInput;
	private TextBox foreColorInput;
	private Button foreColorPreview;
	private CheckBox boldCheckBox;
	private CheckBox italicCheckBox;
	private ComboBox alignmentBox;
	
	public StyleDialog()
	{
		Width = 400;
		Height = 400;
		Text = "Styles";
		
		Styles.Add(new SpireStyle() {
			Name = "Header 1", FontFamily = "Times New Roman", FontSize = 18, ForeColor = Color.Black, FontStyle = FontStyle.Bold, Alignment = ContentAlignment.TopCenter, MarginLeft = 0
		});
		Styles.Add(new SpireStyle() {
			Name = "Text", FontFamily = "Times New Roman", FontSize = 12, ForeColor = Color.Black, FontStyle = FontStyle.Regular, Alignment = ContentAlignment.TopLeft, MarginLeft = 0
		});
		Styles.Add(new SpireStyle() {
			Name = "Quotes", FontFamily = "Arial", FontSize = 12, ForeColor = Color.Gray, FontStyle = FontStyle.Italic, Alignment = ContentAlignment.TopLeft, MarginLeft = 25
		});
		
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
		int leftInputs = leftLabels + 35;
		
		styleHeader = new Label();
		styleHeader.Left = leftInputs;
		styleHeader.Top = styleBox.Top;
		styleHeader.Width = 100;
		styleHeader.Text = "";
		styleHeader.TextAlign = ContentAlignment.TopLeft;
		styleHeader.Parent = this;
		
		Label fontFamilyLabel = new Label();
		fontFamilyLabel.Left = leftLabels;
		fontFamilyLabel.Top = styleHeader.Top + styleHeader.Height + 5;
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
		foreColorInput.Width = 60;
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
		
		Button close = new Button();
		close.Text = "Close";
		close.Left = leftInputs;
		close.Top = addStyle.Top;
		close.Width = 75;
		close.Click += (sender, e) => { this.Close(); };
		close.Parent = this;
		
		DisplayStyle("Header 1");
		
		ShowDialog();
	}
	
	private void StyleSelectedChanged(object sender, EventArgs e)
	{
		DisplayStyle((sender as ListBox).SelectedItem.ToString());
	}
	
	private void DisplayStyle(string name)
	{
		SpireStyle style = FindStyle(name);
		styleHeader.Text = style.Name;
		SetValue(fontFamilyBox, style.FontFamily);
		fontSizeInput.Text = style.FontSize.ToString();
		
		boldCheckBox.Checked = ((style.FontStyle & FontStyle.Bold) == FontStyle.Bold);
		italicCheckBox.Checked = ((style.FontStyle & FontStyle.Italic) == FontStyle.Italic);
		
	}
	
	private SpireStyle FindStyle(string name)
	{
		foreach(SpireStyle style in Styles)
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
	public float FontSize { get; set; }
	public Color ForeColor { get; set; }
	public FontStyle FontStyle { get; set; }
	public ContentAlignment Alignment { get; set; }
	public int MarginLeft { get; set; }
}

