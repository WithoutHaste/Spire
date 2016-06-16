using System;

namespace Spire
{
	public class UpdateAtEventArgs : EventArgs
	{
		public int Index { get; private set; }
		
		public UpdateAtEventArgs(int index)
		{
			Index = index;
		}
	}
}