using System;

namespace Spire
{
	public class TextEventArgs : EventArgs
	{
		public char Text { get; private set; }
		
		public TextEventArgs(char text)
		{
			Text = text;
		}
	}
}