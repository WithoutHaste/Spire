using System;

namespace SpireTest
{
	public class ApplicationLongTest
	{
		public ApplicationLongTest()
		{	
			DateTime startTime = DateTime.Now;
			Console.WriteLine("Starting long tests at {0:HH:mm:ss}\n", startTime);

			bool allTestsPassed = true;
			
			(new TestDocumentModel()).RunLongTests(ref allTestsPassed);
			(new TestDocumentView()).RunLongTests(ref allTestsPassed);
			
			if(allTestsPassed) Console.WriteLine("All long tests passed.\n");
			else Console.WriteLine("Some long tests failed.\n");

			DateTime endTime = DateTime.Now;
			Console.WriteLine("\nEnding long tests at {0:HH:mm:ss}", endTime);
			TimeSpan duration = endTime - startTime;
			Console.WriteLine("Long testing took {0}h {1}m {2}s", duration.Hours, duration.Minutes, duration.Seconds);
		}
	}
}
