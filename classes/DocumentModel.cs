using System;
using System.Collections.Generic;

namespace Spire
{
	public class DocumentModel
	{
		private List<DocumentChunk> chunks;
		private static int maxChunkLength = 30;
	
		public DocumentModel()
		{
			chunks = new List<DocumentChunk>();
		}
		
		public void OnTextEvent(object sender, TextEventArgs e)
		{
			//InsertText(e.Text, chunks.Count-1, 0);
			Console.WriteLine("got "+e.Text);
		}
		
		private void InsertText(string text, int chunkIndex, int stringIndex)
		{
			if(chunkIndex < 0) throw new Exception("Chunk index out of lower bounds.");
			if(chunkIndex >= chunks.Count) throw new Exception("Chunk index out of upper bounds.");
			chunks[chunkIndex].InsertText(text, stringIndex);
		}
	}
}