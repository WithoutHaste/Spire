using System;
using System.Collections.Generic;
using System.Drawing;

namespace Spire
{
	public struct DisplayArea
	{
		//private GraphicsUnit _units;
		private int _width;
		private int _height;
		private Cindex _start;
		private List<Cindex> _lineBreaks;
		
		public DisplayArea(int width, int height)
		{
			//_units = GraphicsUnit.Pixel;
			_width = width;
			_height = height;
			_start = -1;
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
			get { return _start; }
			set { _start = value; }
		}
		
		public List<Cindex> LineBreaks
		{
			get { return _lineBreaks; }
			set { _lineBreaks = value; }
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
		
		public void ClearLineBreaksAfter(int lineBreakIndex)
		{
			while(_lineBreaks.Count > lineBreakIndex+1)
				_lineBreaks.RemoveAt(lineBreakIndex+1);
		}
	}
}