using System;

namespace Spire
{
	public class UpdateAtEventArgs : EventArgs
	{
		public Cindex At { get; private set; }
		
		public UpdateAtEventArgs(Cindex at)
		{
			At = at;
		}
	}
}