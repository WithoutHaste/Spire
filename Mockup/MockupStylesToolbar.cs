using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public static class StylesToolbarDemo
{
	private static Panel parentPanel;
	private static Panel textPanel;
	private static Panel toolbarPanel;
	
	private static Panel titleButton;
	private static Panel authorsButton;
	private static Panel abstractButton;
	
	private static bool title = false;
	private static bool authors = false;
	private static bool abstract_ = false;
	
	private static Color defaultGray;
	private static Color highlightColor = Color.FromArgb(255,153,255,255); //light cyan
	
	public static void Init(Panel scrollPanel, Panel mainPanel)
	{
		parentPanel = scrollPanel;
		textPanel = mainPanel;
		toolbarPanel = BuildToolbar();
		toolbarPanel.Parent = parentPanel;
		textPanel.Left = toolbarPanel.Width;
		AddTextBox();
	}
	
	public static void ClearDemo()
	{
		if(parentPanel == null) return;
		if(toolbarPanel == null) return;
		textPanel.Left = 0;
		parentPanel.Controls.Remove(toolbarPanel);
	}
	
	public static Panel BuildToolbar()
	{
		Panel panel = new Panel();
		panel.Width = 110;
		panel.Height = parentPanel.Height;
		panel.Left = 0;
		panel.Top = 0;
		
		List<Panel> buttonPanels = new List<Panel>();
		titleButton = BuildToolbarButton("T", "Title", ToggleTitle);
		authorsButton = BuildToolbarButton("A", "Authors", ToggleAuthors);
		abstractButton = BuildToolbarButton("B", "Abstract", ToggleAbstract);
		
		buttonPanels.Add(titleButton);
		buttonPanels.Add(authorsButton);
		buttonPanels.Add(abstractButton);
		buttonPanels.Add(BuildToolbarButton("H", "Header", NoToggle));
		buttonPanels.Add(BuildToolbarButton("S", "Sub Header", NoToggle));
		buttonPanels.Add(BuildToolbarButton("Q", "Quote", NoToggle));
		buttonPanels.Add(BuildToolbarButton("F", "Formula", NoToggle));
		
		int y = 0;
		foreach(Panel buttonPanel in buttonPanels)
		{
			buttonPanel.Top = y;
			buttonPanel.Left = 0;
			buttonPanel.Parent = panel;
			y += buttonPanel.Height;
		}
		
		return panel;
	}
	
	//Func delegates return something
	//Action delegates return void
	private static Panel BuildToolbarButton(string alt, string text, Action<object, EventArgs> click)
	{
		int height = 50;
	
		Label label = new Label();
		label.Font = new Font("Times New Roman", 10);
		label.Text = "Alt-" + alt;
		label.Width = 40;
		label.Height = height;
		label.Top = 0;
		label.Left = 0;
		label.TextAlign = ContentAlignment.MiddleCenter;
	
		Button button = new Button();
		button.Font = new Font("Times New Roman", 12);
		button.Text = text;
		button.Width = 70;
		button.Height = height;
		button.Top = 0;
		button.Left = EasyLayout.LeftOf(label, 0);
		button.Click += new EventHandler(click);
		defaultGray = button.BackColor;
		
		Panel panel = new Panel();
		panel.Width = 110;
		panel.Height = height;
		panel.Controls.Add(label);
		panel.Controls.Add(button);
		
		return panel;
	}
	
	private static void AddTextBox()
	{
		TextBox box = MockupWindow.BuildTextInput();
		box.Left = 30;
		box.Top = Math.Max(15, EasyLayout.Below(textPanel.Controls, 5));
		box.Width = 600;
		box.KeyDown += new KeyEventHandler(TextPanelKeyDown);
		box.KeyPress += new KeyPressEventHandler(TextPanelKeyPress);
		textPanel.Controls.Add(box);
		box.Focus();
	}
	
	private static void UpdateTextBox()
	{
		TextBox box = GetFocusedTextBox();
		if(box == null) return;
		
		if(title)
		{
			box.TextAlign = HorizontalAlignment.Center;
			box.Font = new Font("Times New Roman", 14, FontStyle.Bold);
			box.Height = 25;
		}
		else if(authors)
		{
			box.TextAlign = HorizontalAlignment.Center;
			box.Font = new Font("Times New Roman", 12);
			box.Height = 20;
		}
		else if(abstract_)
		{
			box.TextAlign = HorizontalAlignment.Left;
			box.Font = new Font("Times New Roman", 12, FontStyle.Italic);
			box.Height = 110;
		}
		else
		{
			box.TextAlign = HorizontalAlignment.Left;
			box.Font = new Font("Times New Roman", 11);
			box.Height = 400;
		}
	}
	
	private static TextBox GetFocusedTextBox()
	{
		foreach(Control control in textPanel.Controls)
		{
			if(control.Focused)
				return control as TextBox;
		}
		return null;
	}

	private static void TextPanelKeyDown(object sender, KeyEventArgs e)
	{
		if(e.Alt)
		{
			if(e.KeyValue == 'T') ToggleTitle(titleButton.Controls[1], null);
			if(e.KeyValue == 'A') ToggleAuthors(authorsButton.Controls[1], null);
			if(e.KeyValue == 'B') ToggleAbstract(abstractButton.Controls[1], null);
		}
	}
	
	private static void TextPanelKeyPress(object sender, KeyPressEventArgs e)
	{
		if(e.KeyChar == '\r' || e.KeyChar == '\n')
		{
			if(!title && !authors && !abstract_) return;
			
			e.Handled = true;
			AddTextBox();
		}
	}
	
	private static void ToggleButton(Button button, bool flag)
	{
		if(flag)
		{
			button.BackColor = highlightColor;
		}
		else
		{
			button.BackColor = defaultGray;
		}
	}
	
	private static void NoToggle(object sender, EventArgs e)
	{
	}
	
	private static void ToggleTitle(object sender, EventArgs e)
	{
		title = !title;
		ToggleButton(sender as Button, title);
		UpdateTextBox();
	}
	
	private static void ToggleAuthors(object sender, EventArgs e)
	{
		authors = !authors;
		ToggleButton(sender as Button, authors);
		UpdateTextBox();
	}
	
	private static void ToggleAbstract(object sender, EventArgs e)
	{
		abstract_ = !abstract_;
		ToggleButton(sender as Button, abstract_);
		UpdateTextBox();
	}
	
}
