using System;
using System.Collections.Generic;

namespace Spire
{
	public class DocumentChunk
	{
		private static int lowerChunkLength = 15;
		private static int upperChunkLength = 75;

		private List<char> _text;
		private int _length;
		
		//how does List<> implement Count? is it recalculated every time, or saved. if saved, I don't need _length
		
		public DocumentChunk()
		{
			_text = new List<char>();
			_length = 0;
		}
		
		public DocumentChunk(char text)
		{
			_text = new List<char>();
			_text.Add(text);
			_length = 1;
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
		
		public int StartCharIndex
		{
			get;
			set;
		}
		
		public int EndCharIndex
		{
			get { return StartCharIndex + Length - 1; }
		}
		
		public bool TooShort
		{
			get { return (_length < lowerChunkLength); }
		}
		
		public bool TooLong
		{
			get { return (_length > upperChunkLength); }
		}
		
		public bool IsEmpty
		{
			get { return (_length == 0); }
		}
		
		public void InsertText(char[] text, int charIndex)
		{
			int localIndex = LocalIndex(charIndex);
			if(localIndex < 0) throw new Exception("Chunk internal index out of lower bounds.");
			if(localIndex > _length) throw new Exception("Chunk internal index out of upper bounds.");
			foreach(char c in text)
			{
				InsertChar(c, localIndex);
				localIndex++;
			}
		}
		
		public void RemoveText(int charIndex, int length)
		{
			int localIndex = LocalIndex(charIndex);
			if(localIndex < 0) throw new Exception("Chunk internal index out of lower bounds.");
			if(localIndex > _length) throw new Exception("Chunk internal index out of upper bounds.");
			for(int i=0; i<length; i++)
			{
				if(localIndex > _length) throw new Exception("Chunk internal index out of upper bounds.");
				RemoveChar(localIndex);
			}
		}
		
		private void InsertChar(char c, int localIndex)
		{
			_text.Insert(localIndex, c);
			_length++;
		}
		
		private void AppendChar(char c)
		{
			_text.Insert(_length, c);
			_length++;
		}
		
		private void RemoveChar(int localIndex)
		{
			_text.RemoveAt(localIndex);
			_length--;
		}
		
		private int LocalIndex(int charIndex)
		{
			return charIndex - StartCharIndex;
		}
		
		public string SubStringByCharIndex(int startIndex, int endIndex)
		{
			return SubString(LocalIndex(startIndex), LocalIndex(endIndex));
		}
		
		public string SubStringFromCharIndex(int startIndex)
		{
			return SubString(LocalIndex(startIndex), _length-1);
		}

		public string SubStringToCharIndex(int endIndex)
		{
			return SubString(0, LocalIndex(endIndex));
		}
		
		private string SubString(int localStartIndex, int localEndIndex)
		{
			if(localStartIndex < 0) throw new Exception("Chunk internal index out of lower bounds.");
			if(localStartIndex >= _text.Count) throw new Exception("Chunk internal index out of upper bounds.");
			if(localStartIndex > localEndIndex) throw new Exception("Chunk internal start index greater than end index.");
			if(localEndIndex >= _text.Count) throw new Exception("Chunk internal index out of upper bounds.");
			
			return new String(_text.GetRange(localStartIndex, localEndIndex-localStartIndex+1).ToArray());
		}
		
		public DocumentChunk Halve()
		{
			DocumentChunk secondChunk = new DocumentChunk();
			if(_length <= 1) return secondChunk;
			int halfIndex = (int)Math.Floor(_length / 2D);
			while(halfIndex < _length)
			{
				secondChunk.AppendChar(this._text[halfIndex]);
				this.RemoveChar(halfIndex);
			}
			return secondChunk;
		}
		
		public void Append(DocumentChunk secondChunk)
		{
			this._text.AddRange(secondChunk._text);
			this._length += secondChunk._length;
		}
		
	}
}