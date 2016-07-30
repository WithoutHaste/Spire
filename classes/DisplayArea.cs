using System;
using System.Collections.Generic;
using System.Linq;

namespace Spire
{
	public class DisplayArea
	{
		private int _width;
		private int _height;
		private List<Cindex> _lineBreaks;
		
		public DisplayArea(int width, int height)
		{
			_width = width;
			_height = height;
			Start = -1;
			End = -1;
			_lineBreaks = new List<Cindex>();
		}
		
		public int Width
		{
			get { return _width; }
		}
		
		public int Height
		{
			get { return _height; }
		}
		
		public Cindex Start
		{
			get;
			set;
		}
		
		public Cindex End
		{
			get;
			set;
		}
		
		public bool IsEmpty
		{
			get { return (Start == -1 || End == -1); }
		}
		
		public int LineCount
		{
			get
			{
				if(IsEmpty)
					return 0;
				return _lineBreaks.Count + 1;
			}
		}
		
		public bool ContainsCindex(Cindex cindex)
		{
			return (cindex >= Start && cindex <= End);
		}
		
		public List<Cindex> LineBreaks
		{
			get { return _lineBreaks; }
			set { _lineBreaks = value; }
		}
		
		public int LineCountToCindex(Cindex cindex)
		{
			if(cindex < Start) return 0;
			if(cindex > End) return LineCount;
			int lineCount = 1;
			foreach(Cindex lineBreak in _lineBreaks)
			{
				if(cindex <= lineBreak) break;
				lineCount++;
			}
			return lineCount;
		}
		
		public int GetLineBreakIndexBeforeCharIndex(Cindex cindex)
		{
			int lineBreakIndex = -1;
			while(lineBreakIndex+1 < _lineBreaks.Count && cindex > _lineBreaks[lineBreakIndex+1])
			{
				lineBreakIndex++;
			}
			return lineBreakIndex;
		}
		
		public Cindex ClearLineBreaksAfter(Cindex cindex)
		{
			while(_lineBreaks.Count > 0 && _lineBreaks.Last() >= cindex)
			{
				_lineBreaks.RemoveAt(_lineBreaks.Count-1);
			}
			if(_lineBreaks.Count == 0) return 0;
			return _lineBreaks.Last() + 1;
		}
		
		public override string ToString()
		{
			return "LineBreaks: " + String.Join(", ", _lineBreaks.Select(p=>p.ToString()).ToArray());
		}
	}
}
