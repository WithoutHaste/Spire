using System;
using System.Collections.Generic;

namespace Spire
{
	public class DocumentModel
	{
		public delegate void UpdateAtEventHandler(object sender, UpdateAtEventArgs e);
		public event UpdateAtEventHandler OnUpdateAtEvent;

		private Cindex _caretPosition;
		private List<DocumentChunk> chunks; //list is never left empty
	
		public DocumentModel()
		{
			chunks = new List<DocumentChunk>();
			chunks.Add(new DocumentChunk());
			UpdateChunksIndexesFrom(0);
			CaretPosition = 0;
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
		
		public char this[int cindex]
		{
			get
			{
				DocumentChunk chunk = chunks[FindChunkByCharIndex(cindex)];
				return chunk[cindex];
			}
		}
		
		public string SubString(Cindex from, Cindex to)
		{
			if(from < 0) throw new Exception("Document substring start index out of lower bounds.");
			if(to > LastChunk.End) throw new Exception("Document substring end index out of upper bounds.");
		
			int startChunkIndex = FindChunkByCharIndex(from);
			int endChunkIndex = FindChunkByCharIndex(to);
			
			if(startChunkIndex == endChunkIndex)
			{
				return chunks[startChunkIndex].SubStringByCharIndex(from, to);
			}
			
	//??
			//expected to be 1-2 concats only, if frequently more than 4, use StringBuilder instead
			string subString = chunks[startChunkIndex].SubStringFromCharIndex(from);
			for(int i=startChunkIndex+1; i<endChunkIndex; i++)
			{
				subString += chunks[i].Text;
			}
			subString += chunks[endChunkIndex].SubStringToCharIndex(to);
			return subString;
		}
		
		private DocumentChunk LastChunk
		{
			get { return chunks[chunks.Count-1]; }
		}
		
		public void OnTextEvent(object sender, TextEventArgs e)
		{
			InsertText(new char[] { e.Text }, CaretPosition);
			CaretPosition += 1;
		}
		
		public void OnNavigationEvent(object sender, NavigationEventArgs e)
		{
			switch(e.Unit)
			{
				case TextUnit.Character:
					CaretPosition += e.Amount;
					break;
				case TextUnit.Word:
					throw new Exception("navigation by word not implemented");
				default:
					throw new Exception(String.Format("Unit {0} not supported in document navigation", e.Unit));
			}
		}
		
		public void OnEraseEvent(object sender, EraseEventArgs e)
		{
			switch(e.Unit)
			{
				case TextUnit.Character:
					if(e.Amount < 0) BackspaceCharacters(Math.Abs(e.Amount));
					else if(e.Amount > 0) DeleteCharacters(e.Amount);
					break;
				case TextUnit.Word:
					throw new Exception("erase whole word not implemented");
				default:
					throw new Exception(String.Format("Unit {0} not supported in document erasures", e.Unit));
			}
		}
		
		private void InsertText(char[] text, Cindex at)
		{
			int chunkIndex = FindChunkByCharIndex(at);
			DocumentChunk chunk = chunks[chunkIndex];
			chunk.InsertText(text, at);
			int earliestEditChunkIndex = CheckChunkLength(chunkIndex, chunk);
			UpdateChunksIndexesFrom(earliestEditChunkIndex);
			RaiseUpdateAtEvent(at);
/*			
			Console.WriteLine("------");
			foreach(DocumentChunk c in chunks)
			{
				Console.WriteLine("{0} L={1} ({2}-{3})", c.Text, c.Length, c.Start, c.End);
			}
*/		}

		private void BackspaceCharacters(int count)
		{
			if(_caretPosition == 0) return;
			
			int earliestEditChunkIndex = 0;
			while(count > 0)
			{
				if(_caretPosition == 0) break;
				_caretPosition--;
				int chunkIndex = FindChunkByCharIndex(_caretPosition);
				DocumentChunk chunk = chunks[chunkIndex];
				chunk.RemoveText(_caretPosition, 1);
				earliestEditChunkIndex = CheckChunkLength(chunkIndex, chunk);
				count--;
			}
			UpdateChunksIndexesFrom(earliestEditChunkIndex);
			RaiseUpdateAtEvent(_caretPosition);
		}
		
		private void DeleteCharacters(int count)
		{
			if(_caretPosition >= Length) return;
			
			int earliestEditChunkIndex = 0;
			while(count > 0)
			{
				if(_caretPosition >= Length) break;
				int chunkIndex = FindChunkByCharIndex(_caretPosition);
				DocumentChunk chunk = chunks[chunkIndex];
				chunk.RemoveText(_caretPosition, 1);
				earliestEditChunkIndex = CheckChunkLength(chunkIndex, chunk);
				count--;
			}
			UpdateChunksIndexesFrom(earliestEditChunkIndex);
			RaiseUpdateAtEvent(_caretPosition);
		}
		
		private int CheckChunkLength(int chunkIndex, DocumentChunk chunk)
		{
			if(chunk.IsEmpty)
			{
				if(chunks.Count > 1)
				{
					chunks.RemoveAt(chunkIndex);
				}
				return chunkIndex;
			}
			if(chunk.TooLong)
			{
				SplitChunk(chunkIndex, chunk);
				return chunkIndex;
			}
			if(chunk.TooShort)
			{
				return CombineChunks(chunkIndex, chunk);
			}
			return chunkIndex;
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
				if(chunks[chunkIndex-1].TooShort)
				{
					chunks[chunkIndex-1].Append(chunk);
					chunks.RemoveAt(chunkIndex);
					chunkIndex--;
					chunk = chunks[chunkIndex];
				}
			}
			if(chunkIndex < chunks.Count-1)
			{
				if(chunks[chunkIndex+1].TooShort)
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
		
		private void UpdateChunksIndexesFrom(DocumentChunk chunk)
		{
			UpdateChunksIndexesFrom(chunks.IndexOf(chunk));
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