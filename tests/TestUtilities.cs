using System;

namespace SpireTest
{
	public static class TestUtilities
	{
	
		public static void Assert(bool test, string errorMessage)
		{
			if(test) return;
			Console.WriteLine(errorMessage + "\n\n");
		}		
	}
}