using System;

namespace Spire
{
	public struct Cindex
	{
		private int characterIndex;
		
		public Cindex(int i)
		{
			characterIndex = i;
		}
		
		public int Value
		{
			get { return characterIndex; }
			set { characterIndex = value; }
		}
		
		public static implicit operator Cindex(int i)
		{
			return new Cindex(i);
		}
		
		public static implicit operator int(Cindex c)
		{
			return c.Value;
		}
	}
}