using System;

namespace Spire
{
	public struct Line
	{
		public readonly Cindex First;
		public readonly Cindex Last;
		
		public Line(Cindex first, Cindex last)
		{
			First = first;
			Last = last;
		}
	}
}
