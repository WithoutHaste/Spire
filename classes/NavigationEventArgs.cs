using System;

namespace Spire
{
	public class NavigationEventArgs : EventArgs
	{
		public TextUnit Unit { get; private set; }
		public int Amount { get; private set; }
		
		public NavigationEventArgs(TextUnit unit, int amount)
		{
			Unit = unit;
			Amount = amount;
		}
	}
}