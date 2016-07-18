using System;

namespace Spire
{
	public class NavigationHorizontalEventArgs : EventArgs
	{
		public TextUnit Unit { get; private set; }
		public HorizontalDirection Direction { get; private set; }
		
		public NavigationHorizontalEventArgs(TextUnit unit, HorizontalDirection direction)
		{
			Unit = unit;
			Direction = direction;
			if(unit == TextUnit.Line) 
				throw new Exception(String.Format("Invalid TextUnit {0}", unit));
		}
	}
}
