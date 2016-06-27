using Spire;
using System;
using System.IO;

namespace SpireTest
{
	public class TestDocumentModel
	{
		private DocumentModel documentModel;
	
		public TestDocumentModel()
		{
			documentModel = new DocumentModel();
			TestAllKeyboardCharacters();
			TestNavigation();
			
			documentModel = new DocumentModel();
			TestLoad();
		}
		
		private void TestAllKeyboardCharacters()
		{
			AddCharacters("TestAllKeyboardCharacters", "abcdefghijklmnopqrstuvwxyz");
			AddCharacters("TestAllKeyboardCharacters", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
			AddCharacters("TestAllKeyboardCharacters", "`1234567890-=[]\\;',./ ");
			AddCharacters("TestAllKeyboardCharacters", "~!@#$%^&*()_+{}|:\"<>?");
		}
		
		private void TestNavigation()
		{
			AddCharacters("TestNavigation", "and the dog jumped over the log");
			MoveCaret("TestNavigation", 2, 0); //move right past end of document
			MoveCaret("TestNavigation", -3, -3); //move left
			AddCharacters("TestNavigation", "old ");
			MoveCaret("TestNavigation", 5, 3); //move right past end of document
			MoveCaret("TestNavigation", -1 * documentModel.Length, -1 * documentModel.Length); //move to beginning
			MoveCaret("TestNavigation", -2, 0); //move past beginning of document
		}
		
		private void TestLoad()
		{
			Console.WriteLine("\n");
			int count = 100000;
			Random random = new Random(1);

			try
			{
				DateTime startTime = DateTime.Now;
				for(int i=0; i<count; i++)
				{
					RaiseTextEvent((char)(random.Next((int)'a', (int)'z')));
				}
				TimeSpan duration = DateTime.Now - startTime;
				Console.WriteLine("Added {0} characters in {1}h {2}m {3}s", count, duration.Hours, duration.Minutes, duration.Seconds);

				TestUtilities.Assert(documentModel.Length == count, String.Format("TestLoad: error adding {0} characters to document model", count));
				
				Console.WriteLine("\nSample: " + documentModel.SubString(count-200, count-1) + "\n");
				
				startTime = DateTime.Now;
				for(int i=0; i<count; i++)
				{
					RaiseNavigationEvent(TextUnit.Character, -1);
				}
				duration = DateTime.Now - startTime;
				Console.WriteLine("Moved to beginning of document in {0}h {1}m {2}s", duration.Hours, duration.Minutes, duration.Seconds);

				TestUtilities.Assert(documentModel.CaretIndex == 0, "TestLoad: error moving to beginning of document");
				
				startTime = DateTime.Now;
				for(int i=0; i<count; i++)
				{
					RaiseEraseEvent(TextUnit.Character, 1);
				}
				duration = DateTime.Now - startTime;
				Console.WriteLine("Deleted document from beginning in {0}h {1}m {2}s", duration.Hours, duration.Minutes, duration.Seconds);
				
				TestUtilities.Assert(documentModel.Length == 0, String.Format("TestLoad: error deleting {0} characters to document model", count));
			}
			catch(Exception e)
			{
				TestUtilities.Assert(false, "TestLoad: "+e.Message);
			}
				
			Console.WriteLine("\n");
		}
		
		private void AddCharacters(string testName, string characters)
		{
			try
			{
				int startIndex = documentModel.CaretIndex;
				int index = startIndex;
				foreach(char c in characters)
				{
					int startLength = documentModel.Length;
					RaiseTextEvent(c);
					TestUtilities.Assert(documentModel.Length == startLength+1, String.Format("{0}: error adding character '{1}' to document model", testName, c));
					index++;
				}
				string observedCharacters = documentModel.SubString(startIndex, index-1);
				TestUtilities.Assert(observedCharacters == characters, String.Format("{0}: error adding text '{1}' to document model.", testName, characters));
			}
			catch(Exception e)
			{
				TestUtilities.Assert(false, testName+": "+e.Message);
			}
		}
		
		private void MoveCaret(string testName, int distance, int expectedChange)
		{
			try
			{
				int currentPosition = documentModel.CaretIndex;
				RaiseNavigationEvent(TextUnit.Character, distance);
				TestUtilities.Assert(currentPosition + expectedChange == documentModel.CaretIndex, String.Format("{0}: error moving caret by {1}", testName, distance));
			}
			catch(Exception e)
			{
				TestUtilities.Assert(false, testName+": "+e.Message);
			}
		}
		
		private void RaiseTextEvent(char text)
		{
			if(documentModel == null) return;
			documentModel.OnTextEvent(this, new TextEventArgs(text));
		}

		private void RaiseNavigationEvent(TextUnit units, int distance)
		{
			if(documentModel == null) return;
			documentModel.OnNavigationEvent(this, new NavigationEventArgs(units, distance));
		}
		
		private void RaiseEraseEvent(TextUnit units, int count)
		{
			if(documentModel == null) return;
			documentModel.OnEraseEvent(this, new EraseEventArgs(units, count));
		}
	}
}