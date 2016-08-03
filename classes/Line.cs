using System;

namespace Spire
{
	public struct Line
	{
		public readonly Cindex First;
		public readonly Cindex Last;
		
		public Line(Cindex first, Cindex last)
		{
			if(first < 0 || last < 0)
				throw new Exception("Line cannot have a cindex less than zero.");
			First = first;
			Last = last;
		}
		
		public override string ToString()
		{
			return String.Format("[{0}-{1}]", First, Last);
		}
	}
}
