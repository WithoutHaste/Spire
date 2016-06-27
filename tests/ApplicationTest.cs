using System;

namespace SpireTest
{
	public class ApplicationTest
	{
		public ApplicationTest()
		{	
			DateTime startTime = DateTime.Now;
			Console.WriteLine("Starting tests at {0:HH:mm:ss}\n", startTime);

			new TestDocumentModel();

			DateTime endTime = DateTime.Now;
			Console.WriteLine("\nEnding tests at {0:HH:mm:ss}", endTime);
			TimeSpan duration = endTime - startTime;
			Console.WriteLine("Testing took {0}h {1}m {2}s", duration.Hours, duration.Minutes, duration.Seconds);
		}
	}
}