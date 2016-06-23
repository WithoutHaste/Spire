using System;
using System.Collections.Generic;

namespace Spire
{
	public class DocumentModel
	{
		public delegate void UpdateAtEventHandler(object sender, UpdateAtEventArgs e);
		public event UpdateAtEventHandler OnUpdateAtEvent;

		private int _caretIndex;
		private List<DocumentChunk> chunks; //list is never left empty
		//private static int maxChunkLength = 30;
	
		public DocumentModel()
		{
			chunks = new List<DocumentChunk>();
			chunks.Add(new DocumentChunk());
			UpdateChunksIndexesFrom(0);
			CaretIndex = 0;
		}
		
		public int Length
		{
			get { return LastChunk.EndCharIndex + 1; }
		}
		
		public int CaretIndex
		{
			get
			{
				return _caretIndex;
			}
			set 
			{
				if(value < 0) value = 0;
				if(value > Length) value = Length;
				_caretIndex = value;
			}
		}
		
		public string SubString(int startCharIndex, int endCharIndex)
		{
			if(chunks.Count == 0) return "";
			if(startCharIndex < 0) throw new Exception("Document substring start index out of lower bounds.");
			if(endCharIndex > LastChunk.EndCharIndex) throw new Exception("Document substring end index out of upper bounds.");
		
			string subString = "";
			int startChunkIndex = 0;
			while(startChunkIndex < chunks.Count && chunks[startChunkIndex].EndCharIndex < startCharIndex)
			{
				startChunkIndex++;
			}
			int endChunkIndex = startChunkIndex;
			while(endChunkIndex < chunks.Count && chunks[endChunkIndex].EndCharIndex < endCharIndex)
			{
				endCharIndex++;
			}
			
			if(startChunkIndex == endChunkIndex)
			{
				return chunks[startChunkIndex].SubStringByCharIndex(startCharIndex, endCharIndex);
			}
			
			//expected to be 1-2 concats only, if frequently more than 4, use StringBuilder instead
			subString += chunks[startChunkIndex].SubStringFromCharIndex(startCharIndex);
			for(int i=startChunkIndex+1; i<endChunkIndex; i++)
			{
				subString += chunks[i].Text;
			}
			subString += chunks[endChunkIndex].SubStringToCharIndex(endCharIndex);
			return subString;
		}
		
		private DocumentChunk LastChunk
		{
			get { return chunks[chunks.Count-1]; }
		}
		
		public void OnTextEvent(object sender, TextEventArgs e)
		{
			InsertText(new char[] { e.Text }, CaretIndex);
			CaretIndex += 1;
		}
		
		public void OnNavigationEvent(object sender, NavigationEventArgs e)
		{
			switch(e.Unit)
			{
				case NavigationEventArgs.Units.Character:
					CaretIndex += e.Amount;
					break;
				case NavigationEventArgs.Units.Word:
					throw new Exception("navigation by word not implemented");
			}
		}
		
		private void InsertText(char[] text, int charIndex)
		{
			DocumentChunk chunk = FindChunkByCharIndex(charIndex);
			chunk.InsertText(text, charIndex);
			UpdateChunksIndexesFrom(chunk);
			RaiseUpdateAtEvent(charIndex);
		}
		
		private void RaiseUpdateAtEvent(int charIndex)
		{
			if(OnUpdateAtEvent == null) return;
			OnUpdateAtEvent(this, new UpdateAtEventArgs(charIndex));
		}
		
		private void UpdateChunksIndexesFrom(DocumentChunk chunk)
		{
			UpdateChunksIndexesFrom(chunks.IndexOf(chunk));
		}
		
		private void UpdateChunksIndexesFrom(int chunkIndex)
		{
			if(chunkIndex == 0)
			{
				chunks[chunkIndex].StartCharIndex = 0;
				chunkIndex++;
			}
			while(chunkIndex < chunks.Count)
			{
				chunks[chunkIndex].StartCharIndex = chunks[chunkIndex-1].EndCharIndex + 1;
				chunkIndex++;
			}
		}
		
		private DocumentChunk FindChunkByCharIndex(int charIndex)
		{
			int chunkIndex = 0;
			while(chunkIndex < chunks.Count && chunks[chunkIndex].EndCharIndex != -1 && chunks[chunkIndex].EndCharIndex < charIndex)
			{
				chunkIndex++;
			}
			if(chunkIndex >= chunks.Count)
			{
				chunkIndex--; //insert at end of last chunk
			}
			return chunks[chunkIndex];
		}
	}
}