using System;
using System.Collections.Generic;
using System.Text;

namespace Spire
{
	public class DocumentModel
	{
		public delegate void UpdateAtEventHandler(object sender, UpdateAtEventArgs e);
		public event UpdateAtEventHandler OnUpdateAtEvent;

		private Cindex _caretPosition;
		private Cindex? _highlightPosition;
		private List<DocumentChunk> chunks;
		private History history;
	
		public DocumentModel()
		{
			chunks = new List<DocumentChunk>();
			chunks.Add(new DocumentChunk()); //list is never left empty
			UpdateChunksIndexesFrom(0);
			CaretPosition = 0;
			history = new History();
		}
		
		public int Length
		{
			get { return LastChunk.End + 1; }
		}
		
		public Cindex CaretPosition
		{
			get
			{
				return _caretPosition;
			}
			set 
			{
				if(value < 0) value = 0;
				if(value > Length) value = Length;
				_caretPosition = value;
			}
		}
		
		public Cindex HighlightPosition
		{
			get
			{
				if(_highlightPosition == null)
					return _caretPosition;
				return _highlightPosition.Value;
			}
			set
			{
				if(value < 0) value = 0;
				if(value > Length) value = Length;
				_highlightPosition = value;
			}
		}
		
		public bool HasHighlight
		{
			get { return (HighlightPosition != CaretPosition); }
		}
		
		public char this[int cindex]
		{
			get
			{
				int chunkIndex = FindChunkByCharIndex(cindex);
				DocumentChunk chunk = chunks[chunkIndex];
				return chunk[cindex];
			}
		}
		
		public void ClearHighlight()
		{
			_highlightPosition = null;
		}
		
		public void SetHighlight()
		{
			if(_highlightPosition == null)
				_highlightPosition = _caretPosition;
		}
		
		public string SubString(Cindex from, Cindex to)
		{
			if(from < 0) throw new Exception("Document substring start index out of lower bounds.");
			if(to > LastChunk.End) throw new Exception("Document substring end index out of upper bounds.");
			if(to < from) throw new Exception("Document 'to' is less than 'from'.");
		
			int startChunkIndex = FindChunkByCharIndex(from);
			int endChunkIndex = FindChunkByCharIndex(to);
			
			if(startChunkIndex == endChunkIndex)
			{
				return chunks[startChunkIndex].SubStringByCharIndex(from, to);
			}
			
			if(endChunkIndex - startChunkIndex < 3)
			{
				return SubStringWithConcat(startChunkIndex, endChunkIndex, from, to);
			}
			else
			{
				return SubStringWithStringBuilder(startChunkIndex, endChunkIndex, from, to);
			}
		}
		
		private string SubStringWithConcat(int startChunkIndex, int endChunkIndex, Cindex from, Cindex to)
		{
			string subString = chunks[startChunkIndex].SubStringFromCharIndex(from);
			for(int i=startChunkIndex+1; i<endChunkIndex; i++)
			{
				subString += chunks[i].Text;
			}
			subString += chunks[endChunkIndex].SubStringToCharIndex(to);
			return subString;
		}
		
		private string SubStringWithStringBuilder(int startChunkIndex, int endChunkIndex, Cindex from, Cindex to)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(chunks[startChunkIndex].SubStringFromCharIndex(from));
			for(int i=startChunkIndex+1; i<endChunkIndex; i++)
			{
				stringBuilder.Append(chunks[i].Text);
			}
			stringBuilder.Append(chunks[endChunkIndex].SubStringToCharIndex(to));
			return stringBuilder.ToString();
		}
		
		private DocumentChunk LastChunk
		{
			get { return chunks[chunks.Count-1]; }
		}
		
		public void OnTextEvent(object sender, TextEventArgs e)
		{
			if(!e.IsHistoryEvent)
			{
				history.Add(new DocumentEdit_AddCharacters(CaretPosition, e.Text));
			}
			InsertText(e.Text, CaretPosition);
			CaretPosition += e.Text.Length;
			ClearHighlight();
		}
		
		public void OnCaretNavigationHorizontalEvent(object sender, NavigationHorizontalEventArgs e)
		{
			ClearHighlight();
			MoveCaretHorizontal(e);
		}

		public void OnHighlightNavigationHorizontalEvent(object sender, NavigationHorizontalEventArgs e)
		{
			SetHighlight();
			MoveCaretHorizontal(e);
		}
		
		private void MoveCaretHorizontal(NavigationHorizontalEventArgs e)
		{
			int amount = 0;
			switch(e.Direction)
			{
				case HorizontalDirection.Left: amount = -1; break;
				case HorizontalDirection.Right: amount = 1; break;
				default: throw new Exception(String.Format("HorizontalDirection {0} not supported in document navigation", e.Direction));
			}
			switch(e.Unit)
			{
				case TextUnit.Character: CaretPosition += amount; break;
				case TextUnit.Word: throw new Exception("navigation by word not implemented");
				default: throw new Exception(String.Format("Unit {0} not supported in document navigation", e.Unit));
			}
		}

		public void OnEraseEvent(object sender, EraseEventArgs e)
		{
			if(HasHighlight)
			{
				EraseHighlight();
				return;
			}
			switch(e.Unit)
			{
				case TextUnit.Character:
					EraseCharacters(e);
					break;
				case TextUnit.Word:
					throw new Exception("erase whole word not implemented");
				default:
					throw new Exception(String.Format("Unit {0} not supported in document erasures", e.Unit));
			}
		}
		
		private void EraseHighlight()
		{
			EraseCharacters(new EraseEventArgs(TextUnit.Character, _highlightPosition.Value - _caretPosition));
			ClearHighlight();
		}
		
		private void EraseCharacters(EraseEventArgs e)
		{
			int minCindex = CaretPosition;
			int maxCindex = CaretPosition;
			if(e.Amount < 0)
			{
				minCindex = Math.Max(0, CaretPosition+e.Amount);
				maxCindex = Math.Min(CaretPosition-1, Length-1);
				if(!e.IsHistoryEvent && minCindex <= maxCindex)
				{
					history.Add(new DocumentEdit_BackspaceCharacters(CaretPosition, SubString(minCindex,  maxCindex)));
				}
				BackspaceCharacters(Math.Abs(e.Amount));
			}
			else if(e.Amount > 0)
			{
				minCindex = CaretPosition;
				maxCindex = Math.Min(CaretPosition+e.Amount-1, Length-1);
				if(!e.IsHistoryEvent && minCindex <= maxCindex)
				{
					history.Add(new DocumentEdit_DeleteCharacters(CaretPosition, SubString(minCindex, maxCindex)));
				}
				DeleteCharacters(e.Amount);
			}
		}
		
		public void OnUndoEvent(object sender, EventArgs e)
		{
			history.Undo(this);
			ClearHighlight();
		}
		
		public void OnRedoEvent(object sender, EventArgs e)
		{
			history.Redo(this);
			ClearHighlight();
		}
		
		private void InsertText(string text, Cindex at)
		{
			int chunkIndex = FindChunkByCharIndex(at);
			DocumentChunk chunk = chunks[chunkIndex];
			chunk.InsertText(text, at);
			CheckChunkLength(chunkIndex, chunk);
			RaiseUpdateAtEvent(at);
		}

		private void BackspaceCharacters(int count)
		{
			if(_caretPosition == 0) return;
			
			while(count > 0)
			{
				if(_caretPosition == 0) break;
				_caretPosition--;
				int chunkIndex = FindChunkByCharIndex(_caretPosition);
				DocumentChunk chunk = chunks[chunkIndex];
				chunk.RemoveText(_caretPosition, 1);
				CheckChunkLength(chunkIndex, chunk);
				count--;
			}
			RaiseUpdateAtEvent(_caretPosition);
		}
		
		private void DeleteCharacters(int count)
		{
			if(_caretPosition >= Length) return;

			while(count > 0)
			{
				if(_caretPosition >= Length) break;
				int chunkIndex = FindChunkByCharIndex(_caretPosition);
				DocumentChunk chunk = chunks[chunkIndex];
				chunk.RemoveText(_caretPosition, 1);
				CheckChunkLength(chunkIndex, chunk);
				count--;
			}
			RaiseUpdateAtEvent(_caretPosition);
		}
		
		private void CheckChunkLength(int chunkIndex, DocumentChunk chunk)
		{
			int updateFromChunkIndex = chunkIndex;
			if(chunk.IsEmpty)
			{
				if(chunks.Count > 1)
				{
					chunks.RemoveAt(chunkIndex);
				}
			}
			else if(chunk.IsTooLong)
			{
				SplitChunk(chunkIndex, chunk);
			}
			else if(chunk.IsTooShort)
			{
				updateFromChunkIndex = CombineChunks(chunkIndex, chunk);
			}
			UpdateChunksIndexesFrom(updateFromChunkIndex);
		}
		
		private void SplitChunk(int chunkIndex, DocumentChunk chunk)
		{
			DocumentChunk secondChunk = chunk.Halve();
			chunks.Insert(chunkIndex+1, secondChunk);
		}
		
		private int CombineChunks(int chunkIndex, DocumentChunk chunk)
		{
			if(chunkIndex > 0)
			{
				if(chunks[chunkIndex-1].IsTooShort)
				{
					chunks[chunkIndex-1].Append(chunk);
					chunks.RemoveAt(chunkIndex);
					chunkIndex--;
					chunk = chunks[chunkIndex];
				}
			}
			if(chunkIndex < chunks.Count-1)
			{
				if(chunks[chunkIndex+1].IsTooShort)
				{
					chunk.Append(chunks[chunkIndex+1]);
					chunks.RemoveAt(chunkIndex+1);
				}
			}
			return chunkIndex;
		}
		
		private void RaiseUpdateAtEvent(Cindex at)
		{
			if(OnUpdateAtEvent == null) return;
			OnUpdateAtEvent(this, new UpdateAtEventArgs(at));
		}
		
		private void UpdateChunksIndexesFrom(int chunkIndex)
		{
			if(chunkIndex == 0)
			{
				chunks[chunkIndex].Start = 0;
				chunkIndex++;
			}
			while(chunkIndex < chunks.Count)
			{
				chunks[chunkIndex].Start = chunks[chunkIndex-1].End + 1;
				chunkIndex++;
			}
		}
		
		private int FindChunkByCharIndex(Cindex cindex)
		{
			int chunkIndex = 0;
			while(chunkIndex < chunks.Count && chunks[chunkIndex].End != -1 && chunks[chunkIndex].End < cindex)
			{
				chunkIndex++;
			}
			if(chunkIndex >= chunks.Count)
			{
				chunkIndex--; //insert at end of last chunk
			}		
			return chunkIndex;
		}
	}
}
