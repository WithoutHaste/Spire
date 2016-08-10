using System;
using System.Collections.Generic;
using System.Linq;

namespace Spire
{
	class DocumentChunkCollection
	{
		private List<DocumentChunk> chunks; //list is never left empty
		private int updatedToChunkIndex;
		
		public DocumentChunkCollection()
		{
			Initialize();
		}
		
		public int Length
		{
			get {
				UpdateAllChunks();
				return chunks.Last().End;
			}
		}
		
		public void Initialize()
		{
			if(chunks == null)
			{
				chunks = new List<DocumentChunk>();
			}
			else
			{
				chunks.Clear();
			}
			chunks.Add(new DocumentChunk());
			updatedToChunkIndex = -1;
		}
		
		public void AddAt(string text, Cindex cindex)
		{
			int chunkIndex = GetChunkIndexByCindex(cindex);
			DocumentChunk chunk = chunks[chunkIndex];
			chunk.InsertText(text, cindex);
			updatedToChunkIndex = chunkIndex;
			CheckChunkLength(chunkIndex, chunk);
		}
		
		public void RemoveAt(Cindex cindex, int length)
		{
			while(length > 0)
			{
				length = RemoveAtPartial(cindex, length);
			}
		}
		
		private int RemoveAtPartial(Cindex cindex, int length)
		{
			int chunkIndex = GetChunkIndexByCindex(cindex);
			DocumentChunk chunk = chunks[chunkIndex];
			int availableLength = Math.Min(length, chunk.End + 1 - cindex);
			chunk.RemoveText(cindex, availableLength);
			updatedToChunkIndex = chunkIndex;
			CheckChunkLength(chunkIndex, chunk);
			return length - availableLength;
		}
		
		private int GetChunkIndexByCindex(Cindex cindex)
		{
			UpdateChunksToCindex(cindex);
			for(int chunkIndex = 0; chunkIndex < chunks.Count; chunkIndex++)
			{
				if(chunks[chunkIndex].Start >= cindex && chunks[chunkIndex].End <= cindex)
					return chunkIndex;
			}
			if(cindex == Length)
				return chunks.Count - 1;
			throw new Exception(String.Format("Cindex {0} not found in DocumentChunkCollection of length {1}.", cindex, Length));
		}
		
		private DocumentChunk GetChunkByCindex(Cindex cindex)
		{
			UpdateChunksToCindex(cindex);
			foreach(DocumentChunk chunk in chunks)
			{
				if(chunk.Start >= cindex && chunk.End <= cindex)
					return chunk;
			}
			if(cindex == Length)
				return chunks.Last();
			throw new Exception(String.Format("Cindex {0} not found in DocumentChunkCollection of length {1}.", cindex, Length));
		}

		private void CheckChunkLength(int chunkIndex, DocumentChunk chunk)
		{
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
				CombineChunks(chunkIndex, chunk);
			}
		}
		
		private void SplitChunk(int chunkIndex, DocumentChunk chunk)
		{
			while(chunk.IsTooLong)
			{
				DocumentChunk secondChunk = chunk.SplitEnd();
				chunks.Insert(chunkIndex+1, secondChunk);
			}
		}
		
		private void CombineChunks(int chunkIndex, DocumentChunk chunk)
		{
			if(chunkIndex > 0)
			{
				if(chunks[chunkIndex-1].IsTooShort)
				{
					chunks[chunkIndex-1].Append(chunk);
					chunks.RemoveAt(chunkIndex);
					chunkIndex--;
					chunk = chunks[chunkIndex];
					updatedToChunkIndex = Math.Min(updatedToChunkIndex, chunkIndex);
				}
			}
			if(!chunk.IsTooShort)
				return;
			if(chunkIndex < chunks.Count-1)
			{
				if(chunks[chunkIndex+1].IsTooShort)
				{
					chunk.Append(chunks[chunkIndex+1]);
					chunks.RemoveAt(chunkIndex+1);
				}
			}
		}
		
		private void UpdateAllChunks()
		{
			for(int chunkIndex = updatedToChunkIndex + 1; chunkIndex < chunks.Count; chunkIndex++)
			{
				if(chunkIndex == 0)
					chunks[chunkIndex].Start = 0;
				else
					chunks[chunkIndex].Start = chunks[chunkIndex-1].End + 1;
				updatedToChunkIndex = chunkIndex;
			}
		}
		
		private void UpdateChunksToCindex(Cindex cindex)
		{
			for(int chunkIndex = updatedToChunkIndex + 1; chunkIndex < chunks.Count; chunkIndex++)
			{
				if(chunkIndex == 0)
					chunks[chunkIndex].Start = 0;
				else
					chunks[chunkIndex].Start = chunks[chunkIndex-1].End + 1;
				updatedToChunkIndex = chunkIndex;
				if(chunks[chunkIndex].End >= cindex)
					return;
			}
		}
	}
}
