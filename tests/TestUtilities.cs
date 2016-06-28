using System;

namespace SpireTest
{
	public static class TestUtilities
	{
		public static void RunTest(Action test, ref bool allTestsPassed)
		{
			try
			{
				test();
			}
			catch(Exception e)
			{
				Console.WriteLine("{0}: {1}\n", test.Method.Name, e.Message);
				allTestsPassed = false;
			}
		}
	
		public static void Assert(bool test, string errorMessage)
		{
			if(test) return;
			throw new Exception(errorMessage);
		}		
	}
}