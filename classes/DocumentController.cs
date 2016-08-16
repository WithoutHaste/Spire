using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Spire
{
	public class DocumentController
	{
		private DocumentModel documentModel;
		private DocumentView documentView;
		private Panel parentContainer;
		private List<Paper> papers;
		
		public DocumentController(Panel parentContainer)
		{
			this.parentContainer = parentContainer;
			papers = new List<Paper>();
		}
		
		private Paper BuildPaper()
		{
			Paper paper = new Paper(papers.Count + 1);
			//paper.Size = new Size(600, 800);
			paper.Size = new Size(600, 400);
			paper.Left = 30;
			paper.Top = (papers.Count == 0) ? 20 : papers.Last().Top + papers.Last().Height;
			paper.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
			paper.BackColor = Color.White;
			paper.Parent = parentContainer;
			paper.SetModel(documentModel);
			paper.SetView(documentView);
			return paper;
		}
		
		public void OnNewFile(DocumentModel documentModel, DocumentView documentView)
		{
			this.documentModel = documentModel;
			this.documentView = documentView;
			documentView.OnDocumentTooShortEvent += new EventHandler(OnDocumentTooShortEvent);
			parentContainer.Controls.Clear();
			papers.Clear();
			papers.Add(BuildPaper());
			papers.Last().Focus();
		}
		
		public void OnOpenFile()
		{
			//todo: in future, display areas can change when a file is opened
			foreach(Paper paper in papers)
			{
				paper.Invalidate();
			}
		}
		
		public void OnDocumentTooShortEvent(object sender, EventArgs e)
		{
			papers.Add(BuildPaper());
			papers.Last().Focus();
		}
		
		public void RaiseUndoEvent()
		{
			Paper paper = GetPaperWithFocus();
			if(paper == null) return;
			paper.RaiseUndoEvent();
		}
		
		public void RaiseRedoEvent()
		{
			Paper paper = GetPaperWithFocus();
			if(paper == null) return;
			paper.RaiseRedoEvent();
		}
		
		public void RaiseCopyEvent()
		{
			Paper paper = GetPaperWithFocus();
			if(paper == null) return;
			paper.RaiseCopyEvent();
		}
		
		public void RaiseCutEvent()
		{
			Paper paper = GetPaperWithFocus();
			if(paper == null) return;
			paper.RaiseCutEvent();
		}
		
		public void RaisePasteEvent()
		{
			Paper paper = GetPaperWithFocus();
			if(paper == null) return;
			paper.RaisePasteEvent();
		}
		
		private Paper GetPaperWithFocus()
		{
			foreach(Paper paper in papers)
			{
				if(paper.ContainsFocus)
					return paper;
			}
			return null;
		}
		
	}
}
