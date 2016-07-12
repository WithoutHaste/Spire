using System;
using System.Collections.Generic;
using System.Linq;

namespace Spire
{
	public class History
	{
		private Stack<DocumentEdit> pendingUndos;
		private Stack<DocumentEdit> pendingRedos;
		
		public History()
		{
			pendingUndos = new Stack<DocumentEdit>();
			pendingRedos = new Stack<DocumentEdit>();
		}
		
		public void Add(DocumentEdit edit)
		{
			pendingRedos.Clear();
			if(pendingUndos.Count > 0)
			{
				bool success = pendingUndos.Last().Concat(edit);
				if(success)
					return;
			}
			pendingUndos.Push(edit);
		}
		
		public void Undo(DocumentModel documentModel)
		{
			if(pendingUndos.Count == 0) return;
			
			DocumentEdit edit = pendingUndos.Pop();
			edit.Undo(documentModel);
			pendingRedos.Push(edit);
		}
		
		public void Redo(DocumentModel documentModel)
		{
			if(pendingRedos.Count == 0) return;
			
			DocumentEdit edit = pendingRedos.Pop();
			edit.Redo(documentModel);
			pendingUndos.Push(edit);
		}
	}
}
