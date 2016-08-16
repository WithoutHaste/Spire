using System;
//using System.Collections.Generic;
//using System.Diagnostics;
using System.Drawing;
//using System.Drawing.Drawing2D;
using System.IO;
//using System.Text;
using System.Windows.Forms;

namespace Spire
{
	public class Application : Form
	{
		public static Font GlobalFont = new Font("Times New Roman", 12);
		public static int DocumentWidth = 600;
	
		private DocumentController documentController;
		private DocumentModel documentModel;
		private DocumentView documentView;
		private Panel scrollPanel;
		private string iconPath = Path.Combine("images", "SpireIcon1.ico");
		private string saveAsFilename;
		
		public Application()
		{
			SuspendLayout();
			
			Size = new Size(800,600);
			Text = "Spire";
			SetIcon();
			Menu = BuildMainMenu();
			
			scrollPanel = BuildScrollPanel();
			scrollPanel.Parent = this;
			
			documentController = new DocumentController(scrollPanel);
			OnNewFile();

			ResumeLayout(false);
		}
		
		private void SetIcon()
		{
			if(File.Exists(iconPath))
			{
				this.Icon = new Icon(iconPath);
			}
		}

		private Panel BuildScrollPanel()
		{
			Panel panel = new Panel();
			panel.Width = this.ClientSize.Width;
			panel.Height = this.ClientSize.Height;
			panel.AutoScroll = true;
			return panel;
		}

		private MainMenu BuildMainMenu()
		{
			MainMenu mainMenu = new MainMenu();
			mainMenu.MenuItems.Add(BuildFileMenu());
			mainMenu.MenuItems.Add(BuildEditMenu());
			return mainMenu;
		}
		
		private MenuItem BuildFileMenu()
		{
			MenuItem menu = new MenuItem("File");
			menu.MenuItems.Add(BuildMenuItem("New", new EventHandler(OnNewFile), Shortcut.CtrlN));
			menu.MenuItems.Add(BuildMenuItem("Open", new EventHandler(OnOpenFile), Shortcut.CtrlO));
			menu.MenuItems.Add(BuildMenuItem("Save", new EventHandler(OnSaveFile), Shortcut.CtrlS));
			menu.MenuItems.Add(BuildMenuItem("Save As", new EventHandler(OnSaveAsFile), Shortcut.None));
			return menu;
		}
		
		private MenuItem BuildEditMenu()
		{
			MenuItem menu = new MenuItem("Edit");
			menu.MenuItems.Add(BuildMenuItem("Undo", new EventHandler(OnUndo), Shortcut.CtrlZ));
			menu.MenuItems.Add(BuildMenuItem("Redo", new EventHandler(OnRedo), Shortcut.CtrlY));
			menu.MenuItems.Add(BuildMenuItem("Copy", new EventHandler(OnCopy), Shortcut.CtrlC));
			menu.MenuItems.Add(BuildMenuItem("Cut", new EventHandler(OnCut), Shortcut.CtrlX));
			menu.MenuItems.Add(BuildMenuItem("Paste", new EventHandler(OnPaste), Shortcut.CtrlV));
			return menu;
		}
		
		private MenuItem BuildMenuItem(string text, EventHandler eventHandler, Shortcut shortcut)
		{
			MenuItem menuItem = new MenuItem(text);
			menuItem.Shortcut = shortcut;
			menuItem.Click += eventHandler;
			menuItem.ShowShortcut = true;
			return menuItem;
		}
		
		private void OnUndo(object sender, EventArgs e)
		{
			documentController.RaiseUndoEvent();
		}
		
		private void OnRedo(object sender, EventArgs e)
		{
			documentController.RaiseRedoEvent();
		}
		
		private void OnCopy(object sender, EventArgs e)
		{
			documentController.RaiseCopyEvent();
		}
		
		private void OnCut(object sender, EventArgs e)
		{
			documentController.RaiseCutEvent();
		}
		
		private void OnPaste(object sender, EventArgs e)
		{
			documentController.RaisePasteEvent();
		}
		
		private void OnNewFile(object sender, EventArgs e)
		{
			OnNewFile();
		}
		
		private void OnNewFile()
		{
			documentModel = new DocumentModel();
			documentView = new DocumentView(documentModel);
			documentModel.OnUpdateAtEvent += new DocumentModel.UpdateAtEventHandler(documentView.OnModelUpdateEvent);
			documentController.OnNewFile(documentModel, documentView);
		}
		
		private void OnOpenFile(object sender, EventArgs e)
		{
			using(OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.Filter = "txt (*.txt) |*.txt";
				if(dialog.ShowDialog() == DialogResult.OK)
				{
					saveAsFilename = dialog.FileName;
					using(StreamReader stream = new StreamReader(dialog.OpenFile()))
					{
						documentModel.LoadTXT(stream);
					}
					documentController.OnOpenFile();
				}
			}
		}
		
		private void OnSaveFile(object sender, EventArgs e)
		{
			if(String.IsNullOrEmpty(saveAsFilename))
			{
				OnSaveAsFile(sender, e);
				return;
			}
			using(StreamWriter stream = new StreamWriter(File.Open(saveAsFilename, FileMode.Open)))
			{
				documentModel.SaveTXT(stream);
			}
		}
		
		private void OnSaveAsFile(object sender, EventArgs e)
		{
			using(SaveFileDialog dialog = new SaveFileDialog())
			{
				dialog.Filter = "txt (*.txt) |*.txt";
				if(dialog.ShowDialog() == DialogResult.OK)
				{
					saveAsFilename = dialog.FileName;
					using(StreamWriter stream = new StreamWriter(dialog.OpenFile()))
					{
						documentModel.SaveTXT(stream);
					}
				}
			}
		}

	}
}
