using System;

namespace Spire
{
	public class NavigationVerticalEventArgs : EventArgs
	{
		public VerticalDirection Direction { get; private set; }
		
		public NavigationVerticalEventArgs(VerticalDirection direction)
		{
			Direction = direction;
		}
	}
}
