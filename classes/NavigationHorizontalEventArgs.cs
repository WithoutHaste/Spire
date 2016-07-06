using System;

namespace Spire
{
	public class NavigationHorizontalEventArgs : EventArgs
	{
		public TextUnit Unit { get; private set; }
		public int Amount { get; private set; }
		
		public NavigationHorizontalEventArgs(TextUnit unit, int amount)
		{
			Unit = unit;
			Amount = amount;
		}
	}
}
