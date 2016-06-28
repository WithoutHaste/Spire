using Spire;
using System;
using System.IO;

namespace SpireTest
{
	public class TestDocumentModel
	{
		private bool allTestsPassed;
	
		public TestDocumentModel()
		{
		}
		
		public bool RunTests()
		{
			allTestsPassed = true;
			
			TestUtilities.RunTest(TestAllKeyboardCharacters, ref allTestsPassed);
			TestUtilities.RunTest(TestNavigateLeft, ref allTestsPassed);
			TestUtilities.RunTest(TestNavigateRight, ref allTestsPassed);
			TestUtilities.RunTest(TestNavigatePastEndOfDocument, ref allTestsPassed);
			TestUtilities.RunTest(TestNavigatePastBeginningOfDocument, ref allTestsPassed);
			TestUtilities.RunTest(TestDelete, ref allTestsPassed);
			TestUtilities.RunTest(TestDeleteFromEndOfDocument, ref allTestsPassed);
			TestUtilities.RunTest(TestBackspace, ref allTestsPassed);
			TestUtilities.RunTest(TestBackspaceFromBeginningOfDocument, ref allTestsPassed);
			TestUtilities.RunTest(TestLoad_AddAndRemoveInOrder, ref allTestsPassed);
			TestUtilities.RunTest(TestLoad_AddAndRemoveRandomly, ref allTestsPassed);
			
			return allTestsPassed;
		}
		
		private void TestAllKeyboardCharacters()
		{
			DocumentModel documentModel = new DocumentModel();
			AddCharacters(documentModel, "abcdefghijklmnopqrstuvwxyz");
			AddCharacters(documentModel, "ABCDEFGHIJKLMNOPQRSTUVWXYZ");
			AddCharacters(documentModel, "`1234567890-=[]\\;',./ ");
			AddCharacters(documentModel, "~!@#$%^&*()_+{}|:\"<>?");
		}
		
		private void TestNavigateLeft()
		{	
			DocumentModel documentModel = new DocumentModel();
			AddCharacters(documentModel, "and the dog jumped over the log");
			MoveCaret(documentModel, -4, -4);
		}
		
		private void TestNavigateRight()
		{	
			DocumentModel documentModel = new DocumentModel();
			AddCharacters(documentModel, "and the dog jumped over the log");
			MoveCaret(documentModel, -4, -4);
			MoveCaret(documentModel, 2, 2);
		}
		
		private void TestNavigatePastEndOfDocument()
		{	
			DocumentModel documentModel = new DocumentModel();
			AddCharacters(documentModel, "and the dog jumped over the log");
			MoveCaret(documentModel, 1, 0);
			MoveCaret(documentModel, -5, -5);
			MoveCaret(documentModel, 7, 5);
		}
		
		private void TestNavigatePastBeginningOfDocument()
		{	
			DocumentModel documentModel = new DocumentModel();
			AddCharacters(documentModel, "and the dog jumped over the log");
			MoveCaretTo(documentModel, 0);
			MoveCaret(documentModel, -1, 0);
			MoveCaret(documentModel, 5, 5);
			MoveCaret(documentModel, -7, -5);
		}
		
		private void TestDelete()
		{
			DocumentModel documentModel = new DocumentModel();
			AddCharacters(documentModel, "in the house that Jack built with his two good hands");
			MoveCaret(documentModel, -10, -10);
			DeleteCharacters(documentModel, 1, 1);
			DeleteCharacters(documentModel, 3, 3);
		}
		
		private void TestDeleteFromEndOfDocument()
		{
			DocumentModel documentModel = new DocumentModel();
			AddCharacters(documentModel, "in the house that Jack built with his two good hands");
			DeleteCharacters(documentModel, 1, 0);
			MoveCaret(documentModel, -2, -2);
			DeleteCharacters(documentModel, 3, 2);
		}
		
		private void TestBackspace()
		{
			DocumentModel documentModel = new DocumentModel();
			AddCharacters(documentModel, "in the house that Jack built with his two good hands");
			BackspaceCharacters(documentModel, 1, 1);
			BackspaceCharacters(documentModel, 3, 3);
		}
		
		private void TestBackspaceFromBeginningOfDocument()
		{
			DocumentModel documentModel = new DocumentModel();
			AddCharacters(documentModel, "in the house that Jack built with his two good hands");
			MoveCaretTo(documentModel, 0);
			BackspaceCharacters(documentModel, 1, 0);
			MoveCaret(documentModel, 2, 2);
			BackspaceCharacters(documentModel, 3, 2);
		}
		
		private void TestLoad_AddAndRemoveInOrder()
		{	
			DocumentModel documentModel = new DocumentModel();
			int charCount = 100000;
			int maxSeconds = 3;
			Random random = new Random(1);
			
			DateTime startTime = DateTime.Now;
			for(int i=0; i<charCount; i++)
			{
				AddCharacters(documentModel, (char)(random.Next((int)'a', (int)'z')));
			}
			TimeSpan duration = DateTime.Now - startTime;
			TestUtilities.Assert(
				duration.Ticks > TimeSpan.TicksPerSecond * maxSeconds, 
				String.Format("Adding {0} characters took {1}h {2}m {3}s, longer than {4}s allowance", charCount, duration.Hours, duration.Minutes, duration.Seconds, maxSeconds));
			TestUtilities.Assert(
				documentModel.Length == charCount, 
				String.Format("added {0} characters but found only {1}", charCount, documentModel.Length));

			MoveCaretTo(documentModel, 0);

			startTime = DateTime.Now;
			for(int i=0; i<charCount; i++)
			{
				DeleteCharacters(documentModel, 1, 1);
			}
			duration = DateTime.Now - startTime;
			TestUtilities.Assert(
				duration.Ticks > TimeSpan.TicksPerSecond * maxSeconds, 
				String.Format("Deleting {0} characters took {1}h {2}m {3}s, longer than {4}s allowance", charCount, duration.Hours, duration.Minutes, duration.Seconds, maxSeconds));
			TestUtilities.Assert(
				documentModel.Length == 0, 
				String.Format("deleted {0} characters but still found {1}", charCount, documentModel.Length));
		}
		
		private void TestLoad_AddAndRemoveRandomly()
		{
			DocumentModel documentModel = new DocumentModel();
			int editCount = 100000;
			int maxSeconds = 3;
			Random random = new Random(1);

			DateTime startTime = DateTime.Now;
			for(int i=0; i<editCount; i++)
			{
				int position = random.Next(0, documentModel.Length+1);
				MoveCaretTo(documentModel, position);
				if(documentModel.Length > 0 && random.Next(0,3) == 0) /* 1/3 chance erase */
				{
					if(random.Next(0,2) == 0) /* 1/2 chance delete or backspace */
					{
						if(documentModel.CaretPosition == 0)
							BackspaceCharacters(documentModel, 1, 0);
						else
							BackspaceCharacters(documentModel, 1, 1);
					}
					else
					{
						if(documentModel.CaretPosition == documentModel.Length)
							DeleteCharacters(documentModel, 1, 0);
						else
							DeleteCharacters(documentModel, 1, 1);
					}
				}
				else
				{
					AddCharacters(documentModel, (char)(random.Next((int)'a', (int)'z')));
				}
			}
			TimeSpan duration = DateTime.Now - startTime;
			TestUtilities.Assert(
				duration.Ticks > TimeSpan.TicksPerSecond * maxSeconds, 
				String.Format("Random editing {0} times took {1}h {2}m {3}s, longer than {4}s allowance", editCount, duration.Hours, duration.Minutes, duration.Seconds, maxSeconds));
		}
		
		/////////////////////////////////////////////////////////////
		
		private void BackspaceCharacters(DocumentModel documentModel, int charCount, int expectedCharsRemoved)
		{
			int startIndex = documentModel.CaretPosition;
			int startLength = documentModel.Length;
			RaiseEraseEvent(documentModel, TextUnit.Character, -1*charCount);
			TestUtilities.Assert(documentModel.Length == startLength - (Math.Min(startIndex, charCount)), String.Format("error backspacing {0} times", charCount));
			TestUtilities.Assert(documentModel.Length == startLength - expectedCharsRemoved, String.Format("error backspacing, length does not match expected"));
		}
		
		private void DeleteCharacters(DocumentModel documentModel, int charCount, int expectedCharsRemoved)
		{
			int startIndex = documentModel.CaretPosition;
			int startLength = documentModel.Length;
			RaiseEraseEvent(documentModel, TextUnit.Character, charCount);
			TestUtilities.Assert(documentModel.Length == startLength - (Math.Min(startLength-startIndex, charCount)), String.Format("error deleting {0} characters", charCount));
			TestUtilities.Assert(documentModel.Length == startLength - expectedCharsRemoved, String.Format("error deleting, length does not match expected"));
		}
		
		private void AddCharacters(DocumentModel documentModel, char c)
		{
			AddCharacters(documentModel, c.ToString());
		}
		
		private void AddCharacters(DocumentModel documentModel, string characters)
		{
			int startIndex = documentModel.CaretPosition;
			int index = startIndex;
			foreach(char c in characters)
			{
				int startLength = documentModel.Length;
				RaiseTextEvent(documentModel, c);
				TestUtilities.Assert(documentModel.Length == startLength+1, String.Format("error adding character '{0}' to document model", c));
				index++;
			}
			string observedCharacters = documentModel.SubString(startIndex, index-1);
			TestUtilities.Assert(observedCharacters == characters, String.Format("error adding text '{0}' to document model.", characters));
		}
		
		private void MoveCaretTo(DocumentModel documentModel, int index)
		{
			MoveCaret(documentModel, index-documentModel.CaretPosition, index-documentModel.CaretPosition);
			TestUtilities.Assert(documentModel.CaretPosition == index, String.Format("error moving caret to specific index, length {0}, target index {1}, ended up at {2}", documentModel.Length, index, documentModel.CaretPosition));
		}
		
		private void MoveCaret(DocumentModel documentModel, int distance, int expectedChange)
		{
			int currentPosition = documentModel.CaretPosition;
			RaiseNavigationEvent(documentModel, TextUnit.Character, distance);
			TestUtilities.Assert(currentPosition + expectedChange == documentModel.CaretPosition, String.Format("error moving caret from {0} by {1}, document length {2}", currentPosition, distance, documentModel.Length));
		}
		
		private void RaiseTextEvent(DocumentModel documentModel, char text)
		{
			documentModel.OnTextEvent(this, new TextEventArgs(text));
		}

		private void RaiseNavigationEvent(DocumentModel documentModel, TextUnit units, int distance)
		{
			documentModel.OnNavigationEvent(this, new NavigationEventArgs(units, distance));
		}
		
		private void RaiseEraseEvent(DocumentModel documentModel, TextUnit units, int count)
		{
			documentModel.OnEraseEvent(this, new EraseEventArgs(units, count));
		}
	}
}