using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Spire
{
	public class DisplayArea
	{
		private int _width;
		private int _height;
		private int _x;
		private int _y;
		private List<Cindex> _lineBreaks;
		
		public DisplayArea(int x, int y, int width, int height)
		{
			_x = x;
			_y = y;
			_width = width;
			_height = height;
			Start = -1;
			End = -1;
			_lineBreaks = new List<Cindex>();
		}
		
		public int X
		{
			get { return _x; }
		}
		
		public int Y
		{
			get { return _y; }
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
		
		public Cindex LastLineStart
		{
			get
			{
				if(_lineBreaks.Count == 0) return Start;
				return _lineBreaks.Last() + 1;
			}
		}
		
		public Line? LastLine
		{
			get
			{
				if(IsEmpty) return null;
				if(_lineBreaks.Count == 0) return new Line(Start, End);
				return new Line(_lineBreaks.Last() + 1, End);
			}
		}
		
		public Line? FirstLine
		{
			get
			{
				if(IsEmpty) return null;
				if(_lineBreaks.Count == 0) return new Line(Start, End);
				return new Line(Start, _lineBreaks[0]);
			}
		}
		
		public bool ContainsCindex(Cindex cindex)
		{
			return (cindex >= Start && cindex <= End);
		}
		
		public bool ContainsPoint(Point point)
		{
			if(point.X < X || point.X > X+Width) return false;
			if(point.Y < Y || point.Y > Y+Height) return false;
			return true;
		}
		
		public Line? GetLine(Cindex cindex)
		{
			if(!ContainsCindex(cindex)) return null;
			if(_lineBreaks.Count == 0) return new Line(Start, End);
			for(int i=0; i<_lineBreaks.Count; i++)
			{
				if(_lineBreaks[i] >= cindex)
				{
					if(i == 0) return new Line(Start, _lineBreaks[0]);
					return new Line(_lineBreaks[i-1]+1, _lineBreaks[i]);
				}
			}
			return new Line(_lineBreaks.Last()+1, End);
		}
		
		public Line? GetIthLine(int lineNumber)
		{
			List<Line> lines = GetLines();
			if(lines.Count < lineNumber) return null;
			return lines[lineNumber-1];
		}
		
		public List<Line> GetLines()
		{
			List<Line> lines = new List<Line>();
			Cindex start = Start;
			foreach(Cindex lineBreak in _lineBreaks)
			{
				lines.Add(new Line(start, lineBreak));
				start = lineBreak + 1;
			}
			if(!IsEmpty && start <= End)
			{
				lines.Add(new Line(start, End));
			}
			return lines;
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
		
		public Cindex? PreviousLineBreak(Cindex cindex)
		{
			Cindex? previousLineBreak = null;
			foreach(Cindex lineBreak in _lineBreaks)
			{
				if(lineBreak >= cindex)
					break;
				previousLineBreak = lineBreak;
			}
			return previousLineBreak;
		}
		
		public Cindex? NextLineBreak(Cindex cindex)
		{
			foreach(Cindex lineBreak in _lineBreaks)
			{
				if(lineBreak > cindex)
					return lineBreak;
			}
			return null;
		}
		
		public void ClearThroughPreviousLine(Cindex cindex)
		{
			End = -1;
			while(_lineBreaks.Count > 0 && _lineBreaks.Last() > cindex)
			{
				_lineBreaks.RemoveAt(_lineBreaks.Count-1);
			}
			if(_lineBreaks.Count > 0)
			{
				_lineBreaks.RemoveAt(_lineBreaks.Count-1);
			}
		}
		
		public void Reset(Cindex start)
		{
			End = -1;
			Start = start;
			_lineBreaks.Clear();
		}
		
		public void AddLineBreak(Cindex cindex)
		{
			if(cindex < Start) throw new Exception("Cannot add line break less than start.");
			if(_lineBreaks.Count > 0 && _lineBreaks.Last() >= cindex) throw new Exception("Cannot add line break less than or equal to previous line break.");
			_lineBreaks.Add(cindex);
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
