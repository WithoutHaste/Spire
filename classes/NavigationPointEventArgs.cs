using System;

namespace Spire
{
	public class NavigationPointEventArgs : EventArgs
	{
		public int X { get; private set; }
		public int Y { get; private set; }
		
		public NavigationPointEventArgs(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
}
