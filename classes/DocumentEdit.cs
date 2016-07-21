using System;
using System.Collections.Generic;
using System.Linq;

namespace Spire
{
	public abstract class DocumentEdit
	{
		public DocumentEdit()
		{
		}
		
		public abstract void Undo(DocumentModel documentModel);
		public abstract void Redo(DocumentModel documentModel);
		public abstract bool Concat(DocumentEdit documentEdit);
		
		public override string ToString()
		{
			return "no override for ToString";
		}
	}
	
	public class DocumentEdit_Multiple : DocumentEdit
	{
		private List<DocumentEdit> edits;
		
		public DocumentEdit_Multiple(params DocumentEdit[] edits)
		{
			if(edits.Length <= 1)
				throw new Exception("Multiple edits required for DocumentEdit_Multiple");
			this.edits = edits.ToList();
		}
		
		public DocumentEdit_Multiple(List<DocumentEdit> edits)
		{
			this.edits = new List<DocumentEdit>(edits);
		}
		
		public override void Undo(DocumentModel documentModel)
		{
			for(int i=edits.Count-1; i>=0; i--)
			{
				edits[i].Undo(documentModel);
			}
		}
		
		public override void Redo(DocumentModel documentModel)
		{
			for(int i=0; i<edits.Count; i++)
			{
				edits[i].Redo(documentModel);
			}
		}
		
		public override bool Concat(DocumentEdit edit)
		{
			return false;
		}
		
		public override string ToString()
		{
			return String.Format("Multiple edits: {0}", String.Join(" | ", edits.Select(p=>p.ToString()).ToArray()));
		}
	}
	
	public class DocumentEdit_AddCharacters : DocumentEdit
	{
		private static int maxLength = 70;
		private Cindex startCindex;
		private string text;
		
		public DocumentEdit_AddCharacters(Cindex cindex, string text)
		{
			this.startCindex = cindex;
			this.text = text;
		}
		
		public override void Undo(DocumentModel documentModel)
		{
			documentModel.CaretPosition = startCindex;
			documentModel.OnEraseEvent(this, new EraseEventArgs(TextUnit.Character, text.Length, true));
		}
		
		public override void Redo(DocumentModel documentModel)
		{
			documentModel.CaretPosition = startCindex;
			documentModel.OnTextEvent(this, new TextEventArgs(text, true));
		}
		
		public override bool Concat(DocumentEdit edit)
		{
			if(!(edit is DocumentEdit_AddCharacters)) return false;
			if(this.text.Length >= maxLength) return false;
			if((edit as DocumentEdit_AddCharacters).startCindex != this.startCindex + this.text.Length) return false;
			if(this.text.EndsWith(" ")) return false;
			this.text += (edit as DocumentEdit_AddCharacters).text;
			return true;
		}
		
		public override string ToString()
		{
			return String.Format("AddCharacters '{0}' at {1}", text, startCindex);
		}
	}
	
	public class DocumentEdit_BackspaceCharacters : DocumentEdit
	{
		private static int maxLength = 70;
		private Cindex startCindex;
		private string text;
		
		public DocumentEdit_BackspaceCharacters(Cindex cindex, string text)
		{
			this.startCindex = cindex;
			this.text = text;
		}
		
		public override void Undo(DocumentModel documentModel)
		{
			documentModel.CaretPosition = startCindex - text.Length;
			documentModel.OnTextEvent(this, new TextEventArgs(text, true));
		}
		
		public override void Redo(DocumentModel documentModel)
		{
			documentModel.CaretPosition = startCindex;
			documentModel.OnEraseEvent(this, new EraseEventArgs(TextUnit.Character, -1*text.Length, true));
		}
		
		public override bool Concat(DocumentEdit edit)
		{
			if(!(edit is DocumentEdit_BackspaceCharacters)) return false;
			if(this.text.Length >= maxLength) return false;
			if((edit as DocumentEdit_BackspaceCharacters).startCindex != this.startCindex - this.text.Length) return false;
			this.text = (edit as DocumentEdit_BackspaceCharacters).text + this.text;
			return true;
		}
		
		public override string ToString()
		{
			return String.Format("BackspaceCharacters '{0}' at {1}", text, startCindex);
		}
	}	
	
	public class DocumentEdit_DeleteCharacters : DocumentEdit
	{
		private static int maxLength = 70;
		private Cindex startCindex;
		private string text;
		
		public DocumentEdit_DeleteCharacters(Cindex cindex, string text)
		{
			this.startCindex = cindex;
			this.text = text;
		}
		
		public override void Undo(DocumentModel documentModel)
		{
			documentModel.CaretPosition = startCindex;
			documentModel.OnTextEvent(this, new TextEventArgs(text, true));
			documentModel.CaretPosition = startCindex;
		}
		
		public override void Redo(DocumentModel documentModel)
		{
			documentModel.CaretPosition = startCindex;
			documentModel.OnEraseEvent(this, new EraseEventArgs(TextUnit.Character, text.Length, true));
		}
		
		public override bool Concat(DocumentEdit edit)
		{
			if(!(edit is DocumentEdit_DeleteCharacters)) return false;
			if(this.text.Length >= maxLength) return false;
			if((edit as DocumentEdit_DeleteCharacters).startCindex != this.startCindex) return false;
			this.text += (edit as DocumentEdit_DeleteCharacters).text;
			return true;
		}
		
		public override string ToString()
		{
			return String.Format("DeleteCharacters '{0}' at {1}", text, startCindex);
		}
	}	
}
