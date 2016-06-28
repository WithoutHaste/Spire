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
			TestLoadBulk();
			TestLoadRandom();
			TestDelete();
			TestBackspace();
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
		
		private void TestLoadBulk()
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

				TestUtilities.Assert(documentModel.Length == count, String.Format("TestLoadBulk: error adding {0} characters to document model", count));
				
				//Console.WriteLine("\nSample: " + documentModel.SubString(count-200, count-1) + "\n");
				
				startTime = DateTime.Now;
				for(int i=0; i<count; i++)
				{
					RaiseNavigationEvent(TextUnit.Character, -1);
				}
				duration = DateTime.Now - startTime;
				Console.WriteLine("Moved to beginning of document in {0}h {1}m {2}s", duration.Hours, duration.Minutes, duration.Seconds);

				TestUtilities.Assert(documentModel.CaretIndex == 0, "TestLoadBulk: error moving to beginning of document");
				
				startTime = DateTime.Now;
				for(int i=0; i<count; i++)
				{
					RaiseEraseEvent(TextUnit.Character, 1);
				}
				duration = DateTime.Now - startTime;
				Console.WriteLine("Deleted document from beginning in {0}h {1}m {2}s", duration.Hours, duration.Minutes, duration.Seconds);
				
				TestUtilities.Assert(documentModel.Length == 0, String.Format("TestLoadBulk: error deleting {0} characters to document model", count));
			}
			catch(Exception e)
			{
				TestUtilities.Assert(false, "TestLoadBulk: "+e.Message);
			}
				
			Console.WriteLine("\n");
		}
		
		private void TestLoadRandom()
		{
			Console.WriteLine("\n");
			int count = 100000;
			Random random = new Random(1);

			try
			{
				DateTime startTime = DateTime.Now;
				int countAdds = 0;
				int countDeletes = 0;
				int countBackspaces = 0;
				for(int i=0; i<count; i++)
				{
					int position = random.Next(0, documentModel.Length);
					MoveCaret("TestLoadRandom", position-documentModel.CaretIndex, position-documentModel.CaretIndex);
					if(documentModel.Length > 0 && random.Next(0,2) == 0)
					{
						if(documentModel.CaretIndex > 0 && (documentModel.CaretIndex < documentModel.Length || random.Next(0,2) == 0))
						{
							RaiseEraseEvent(TextUnit.Character, -1);
							countBackspaces++;
						}
						else
						{
							RaiseEraseEvent(TextUnit.Character, 1);
							countDeletes++;
						}
					}
					else
					{
						RaiseTextEvent((char)(random.Next((int)'a', (int)'z')));
						countAdds++;
					}
				}
				TimeSpan duration = DateTime.Now - startTime;
				Console.WriteLine("Randomly added {0} characters, deleted {1}, backspaced {2}, in {3}h {4}m {5}s", countAdds, countDeletes, countBackspaces, duration.Hours, duration.Minutes, duration.Seconds);
			}
			catch(Exception e)
			{
				TestUtilities.Assert(false, "TestLoadRandom: "+e.Message);
			}
				
			Console.WriteLine("\n");
		}
		
		private void TestDelete()
		{
			MoveCaret("TestDelete", documentModel.Length-documentModel.CaretIndex, documentModel.Length-documentModel.CaretIndex);
			AddCharacters("TestDelete", "In the house that Jack build with his two good hands,");
			DeleteCharacters("TestDelete", 1, 0); //delete while at end of document
			DeleteCharacters("TestDelete", 5, 0); //delete while at end of document
			MoveCaret("TestDelete", -8, -8);
			DeleteCharacters("TestDelete", 1, -1); //normal delete
			DeleteCharacters("TestDelete", 3, -3); //normal delete
			DeleteCharacters("TestDelete", 6, -4); //delete through end of document
		}
		
		private void TestBackspace()
		{
			AddCharacters("TestBackspace", "In the house that Jack build with his two good hands,");
			MoveCaret("TestBackspace", -1*documentModel.CaretIndex, -1*documentModel.CaretIndex);
			BackspaceCharacters("TestBackspace", 1, 0); //backspace while at beginning of document
			BackspaceCharacters("TestBackspace", 5, 0); //backspace while at beginning of document
			MoveCaret("TestBackspace", 8, 8);
			BackspaceCharacters("TestBackspace", 1, -1); //normal backspace
			BackspaceCharacters("TestBackspace", 3, -3); //normal backspace
			BackspaceCharacters("TestBackspace", 6, -4); //backspace through beginning of document
		}
		
		private void EraseCharacters(string testName, int count, int expectedLengthChange)
		{
			if(count < 0)
				BackspaceCharacters(testName, Math.Abs(count), expectedLengthChange);
			if(count > 0)
				DeleteCharacters(testName, count, expectedLengthChange);
		}
		
		private void BackspaceCharacters(string testName, int count, int expectedLengthChange)
		{
			try
			{
				int startIndex = documentModel.CaretIndex;
				int startLength = documentModel.Length;
				RaiseEraseEvent(TextUnit.Character, -1*count);
				TestUtilities.Assert(documentModel.Length == startLength-(Math.Min(startIndex, count)), String.Format("{0}: error backspacing {1} times", testName, count));
				TestUtilities.Assert(documentModel.Length == startLength+expectedLengthChange, String.Format("{0}: error backspacing, length does not match expected", testName));
			}
			catch(Exception e)
			{
				TestUtilities.Assert(false, testName+": "+e.Message);
			}
		}
		
		private void DeleteCharacters(string testName, int count, int expectedLengthChange)
		{
			try
			{
				int startIndex = documentModel.CaretIndex;
				int startLength = documentModel.Length;
				RaiseEraseEvent(TextUnit.Character, count);
				TestUtilities.Assert(documentModel.Length == startLength-(Math.Min(startLength-startIndex, count)), String.Format("{0}: error deleting {1} times", testName, count));
				TestUtilities.Assert(documentModel.Length == startLength+expectedLengthChange, String.Format("{0}: error deleting, length does not match expected", testName));
			}
			catch(Exception e)
			{
				TestUtilities.Assert(false, testName+": "+e.Message);
			}
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