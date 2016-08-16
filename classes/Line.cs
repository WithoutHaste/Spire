using System;

namespace Spire
{
	public struct Line
	{
		public readonly Cindex First;
		public readonly Cindex Last; //last can equal documentModel.Length when including end of document
		public readonly bool IncludesEndOfDocument;
		
		public Line(Cindex first, Cindex last, bool includesEndOfDocument)
		{
			if(first < 0 || last < 0)
				throw new Exception("Line cannot have a cindex less than zero.");
			if(first > last)
				throw new Exception(String.Format("Line cannot end before it starts: first={0} last={1}.", first, last));
			First = first;
			Last = last;
			IncludesEndOfDocument = includesEndOfDocument;
		}
		
		public int Length
		{
			get 
			{
				if(IncludesEndOfDocument)
					return Last - First;
				else
					return Last - First + 1;
			}
		}
		
		public override string ToString()
		{
			return String.Format("[{0} to {1}{2}]", First, Last, (IncludesEndOfDocument ? "\\0" : ""));
		}
	}
}
