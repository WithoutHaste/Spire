using System;

namespace Spire
{
	public class UpdateAtEventArgs : EventArgs
	{
		public int CharIndex { get; private set; }
		
		public UpdateAtEventArgs(int charIndex)
		{
			CharIndex = charIndex;
		}
	}
}