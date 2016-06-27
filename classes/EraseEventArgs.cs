using System;

namespace Spire
{
	public class EraseEventArgs : EventArgs
	{
		public enum Units { Character, Word };
		
		public Units Unit { get; private set; }
		public int Amount { get; private set; }
		
		public EraseEventArgs(Units unit, int amount)
		{
			Unit = unit;
			Amount = amount;
		}
	}
}