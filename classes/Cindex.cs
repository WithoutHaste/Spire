using System;

namespace Spire
{
	public struct Cindex
	{
		private int _characterIndex;
		
		public Cindex(int i)
		{
			_characterIndex = i;
		}
		
		public int Value
		{
			get { return _characterIndex; }
			set { _characterIndex = value; }
		}
		
		public override string ToString()
		{
			return _characterIndex.ToString();
		}
		
		public static implicit operator Cindex(int i)
		{
			return new Cindex(i);
		}
		
		public static implicit operator int(Cindex c)
		{
			return c.Value;
		}
		
		public static Cindex operator --(Cindex c)
		{
			c._characterIndex--;
			return c;
		}
		
		public static Cindex operator ++(Cindex c)
		{
			c._characterIndex++;
			return c;
		}
	}
}
