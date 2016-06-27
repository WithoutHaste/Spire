using System;

namespace Spire
{
	public class EraseEventArgs : EventArgs
	{
		public TextUnit Unit { get; private set; }
		public int Amount { get; private set; }
		
		public EraseEventArgs(TextUnit unit, int amount)
		{
			Unit = unit;
			Amount = amount;
		}
	}
}