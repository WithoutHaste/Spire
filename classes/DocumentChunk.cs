using System;
using System.Collections.Generic;

namespace Spire
{
	public class DocumentChunk
	{
		private List<char> _text;
		private int _length;
		
		public DocumentChunk()
		{
			_text = new List<char>();
			_length = 0;
		}
		
		public DocumentChunk(string text)
		{
			_text = new List<char>(text.ToCharArray(0, text.Length));
			_length = _text.Count;
		}
		
		public string Text
		{
			get { return new String(_text.ToArray()); }
		}
		
		public int Length
		{
			get { return _length; }
		}
		
		public void InsertText(string text, int index)
		{
			if(index < 0) throw new Exception("Chunk internal index out of lower bounds.");
			if(index >= _length) throw new Exception("Chunk internal index out of upper bounds.");
			foreach(char c in text)
			{
				InsertChar(c, index);
				index++;
			}
		}
		
		public void InsertText(char text, int index)
		{
			if(index < 0) throw new Exception("Chunk internal index out of lower bounds.");
			if(index >= _length) throw new Exception("Chunk internal index out of upper bounds.");
			InsertChar(text, index);
		}
		
		private void InsertChar(char c, int index)
		{
			_text.Insert(index, c);
			_length++;
		}
	}
}