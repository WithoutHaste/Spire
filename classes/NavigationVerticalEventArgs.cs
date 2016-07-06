using System;

namespace Spire
{
	public class NavigationVerticalEventArgs : EventArgs
	{
		public int Amount { get; private set; }
		
		public NavigationVerticalEventArgs(int amount)
		{
			Amount = amount;
		}
	}
}
