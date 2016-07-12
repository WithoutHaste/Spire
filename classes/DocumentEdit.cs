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
			documentModel.OnEraseEvent(this, new EraseEventArgs(TextUnit.Character, text.Length));
		}
		
		public override void Redo(DocumentModel documentModel)
		{
			documentModel.CaretPosition = startCindex;
			foreach(char c in text)
			{
				documentModel.OnTextEvent(this, new TextEventArgs(c));
			}
		}
		
		public override bool Concat(DocumentEdit edit)
		{
			if(!(edit is DocumentEdit_AddCharacters)) return false;
			if(this.text.Length >= maxLength) return false;
			if((edit as DocumentEdit_AddCharacters).startCindex != this.startCindex + this.text.Length) return false;
			this.text += (edit as DocumentEdit_AddCharacters).text;
			return true;
		}
	}
}
