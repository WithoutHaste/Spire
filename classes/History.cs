using System;
using System.Collections.Generic;
using System.Linq;

namespace Spire
{
	public class History
	{
		private Stack<DocumentEdit> pendingUndos;
		private Stack<DocumentEdit> pendingRedos;
		private int? countMultiEdits;
		
		public History()
		{
			pendingUndos = new Stack<DocumentEdit>();
			pendingRedos = new Stack<DocumentEdit>();
			countMultiEdits = null;
		}
		
		private bool InMultiEdit
		{
			get { return countMultiEdits.HasValue; }
		}
		
		public void Add(DocumentEdit edit)
		{
			pendingRedos.Clear();
			if(pendingUndos.Count > 0)
			{
				bool success = pendingUndos.First().Concat(edit);
				if(success)
					return;
			}
			pendingUndos.Push(edit);
			if(InMultiEdit)
				countMultiEdits++;
		}
		
		public void Undo(DocumentModel documentModel)
		{
			if(pendingUndos.Count == 0) return;
			if(InMultiEdit) throw new Exception("cannot undo within multi edit");
			
			DocumentEdit edit = pendingUndos.Pop();
			edit.Undo(documentModel);
			pendingRedos.Push(edit);
		}
		
		public void Redo(DocumentModel documentModel)
		{
			if(pendingRedos.Count == 0) return;
			if(InMultiEdit) throw new Exception("cannot redo within multi edit");
			
			DocumentEdit edit = pendingRedos.Pop();
			edit.Redo(documentModel);
			pendingUndos.Push(edit);
		}
		
		public void StartMultiEdit()
		{
			countMultiEdits = 0;
		}
		
		public void EndMultiEdit()
		{
			List<DocumentEdit> edits = new List<DocumentEdit>();
			for(int i=0; i<countMultiEdits; i++)
			{
				edits.Insert(0, pendingUndos.Pop());
			}
			pendingUndos.Push(new DocumentEdit_Multiple(edits));
			countMultiEdits = null;
		}
	}
}
