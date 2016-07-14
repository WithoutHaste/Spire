using System;

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
			documentModel.CaretPosition = startCindex;
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
