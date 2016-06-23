using System;

namespace Spire
{
	public class NavigationEventArgs : EventArgs
	{
		public enum Units { Character, Word };
		
		public Units Unit { get; private set; }
		public int Amount { get; private set; }
		
		public NavigationEventArgs(Units unit, int amount)
		{
			Unit = unit;
			Amount = amount;
		}
	}
}