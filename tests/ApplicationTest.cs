using System;

namespace SpireTest
{
	public class ApplicationTest
	{
		public ApplicationTest()
		{	
			DateTime startTime = DateTime.Now;
			Console.WriteLine("Starting tests at {0:HH:mm:ss}\n", startTime);

			bool allTestsPassed = true;
			
			(new TestDocumentModel()).RunTests(ref allTestsPassed);
			(new TestDocumentView()).RunTests(ref allTestsPassed);
			
			if(allTestsPassed) Console.WriteLine("All tests passed.\n");
			else Console.WriteLine("Some tests failed.\n");

			DateTime endTime = DateTime.Now;
			Console.WriteLine("\nEnding tests at {0:HH:mm:ss}", endTime);
			TimeSpan duration = endTime - startTime;
			Console.WriteLine("Testing took {0}h {1}m {2}s", duration.Hours, duration.Minutes, duration.Seconds);
		}
	}
}