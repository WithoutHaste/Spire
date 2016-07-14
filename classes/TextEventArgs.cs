using System;

namespace Spire
{
	public class TextEventArgs : EventArgs
	{
		public string Text { get; private set; }
		public bool IsHistoryEvent { get; private set; }
		
		public TextEventArgs(char text)
		{
			Text = text.ToString();
			IsHistoryEvent = false;
		}
		
		public TextEventArgs(string text)
		{
			Text = text;
			IsHistoryEvent = false;
		}
		
		public TextEventArgs(string text, bool isHistoryEvent)
		{
			Text = text;
			IsHistoryEvent = true;
		}
	}
}
