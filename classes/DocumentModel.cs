using System;
using System.Collections.Generic;

namespace Spire
{
	public class DocumentModel
	{
		public delegate void UpdateAtEventHandler(object sender, UpdateAtEventArgs e);
		public event UpdateAtEventHandler OnUpdateAtEvent;

		private List<DocumentChunk> chunks;
		//private static int maxChunkLength = 30;
	
		public DocumentModel()
		{
			chunks = new List<DocumentChunk>();
		}
		
		public int Length
		{
			get {
				if(chunks.Count == 0) return 0;
				return LastChunk.EndCharIndex + 1;
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
			get {
				if(chunks.Count == 0) return null;
				return chunks[chunks.Count-1];
			}
		}
		
		public void OnTextEvent(object sender, TextEventArgs e)
		{
			if(chunks.Count == 0)
			{
				chunks.Add(new DocumentChunk(e.Text));
				UpdateChunkStartIndexesFrom(0);
				RaiseUpdateAtEvent(0);
			}
			else
			{
				InsertText(e.Text, chunks.Count-1, 0);
			}
		}
		
		private void InsertText(char text, int chunkIndex, int stringIndex)
		{
			if(chunkIndex < 0) throw new Exception("Chunk index out of lower bounds.");
			if(chunkIndex >= chunks.Count) throw new Exception("Chunk index out of upper bounds.");
			chunks[chunkIndex].InsertText(text, stringIndex);
			UpdateChunkStartIndexesFrom(chunkIndex+1);
			RaiseUpdateAtEvent(chunkIndex);
		}

		private void InsertText(string text, int chunkIndex, int stringIndex)
		{
			if(chunkIndex < 0) throw new Exception("Chunk index out of lower bounds.");
			if(chunkIndex >= chunks.Count) throw new Exception("Chunk index out of upper bounds.");
			chunks[chunkIndex].InsertText(text, stringIndex);
			UpdateChunkStartIndexesFrom(chunkIndex+1);
			RaiseUpdateAtEvent(chunkIndex);
		}
		
		private void RaiseUpdateAtEvent(int chunkIndex)
		{
			if(OnUpdateAtEvent == null) return;
			OnUpdateAtEvent(this, new UpdateAtEventArgs(chunkIndex));
		}
		
		private void UpdateChunkStartIndexesFrom(int chunkIndex)
		{
			if(chunks.Count == 0) return;
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
	}
}