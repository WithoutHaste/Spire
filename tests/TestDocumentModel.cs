using Spire;
using System;
using System.IO;

namespace SpireTest
{
	public class TestDocumentModel
	{
		public TestDocumentModel()
		{
		}
		
		public void RunTests(ref bool allTestsPassed)
		{
			TestUtilities.RunTest(TestAllKeyboardCharacters, ref allTestsPassed);
			TestUtilities.RunTest(TestNavigateLeft, ref allTestsPassed);
			TestUtilities.RunTest(TestNavigateRight, ref allTestsPassed);
			TestUtilities.RunTest(TestNavigatePastEndOfDocument, ref allTestsPassed);
			TestUtilities.RunTest(TestNavigatePastBeginningOfDocument, ref allTestsPassed);
			TestUtilities.RunTest(TestDelete, ref allTestsPassed);
			TestUtilities.RunTest(TestDeleteFromEndOfDocument, ref allTestsPassed);
			TestUtilities.RunTest(TestBackspace, ref allTestsPassed);
			TestUtilities.RunTest(TestBackspaceFromBeginningOfDocument, ref allTestsPassed);
			//TestUtilities.RunTest(TestLoad_AddAndRemoveInOrder, ref allTestsPassed);
			//TestUtilities.RunTest(TestLoad_AddAndRemoveRandomly, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoInEmptyDocument, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoOneLetter, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoSeveralLetters, ref allTestsPassed);
			TestUtilities.RunTest(TestUndo100Letters, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoWordWithSpace, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoSeveralWords, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoRemovingSpaceBetweenWords, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoHyphenatedWord, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoReplacingSpaceWithHyphen, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoPuttingHyphenInWord, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoWordWithComma, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoWordWithPeriod, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoWordWithSemiColon, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoWordWithColon, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoWordWithApostrophe, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoWordWithQuote, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoWordWithQuestionMark, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoWordWithExclamationMark, ref allTestsPassed);
		}
		
		private void TestAllKeyboardCharacters()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("abcdefghijklmnopqrstuvwxyz");
			documentModel.AddCharacters("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
			documentModel.AddCharacters("`1234567890-=[]\\;',./ ");
			documentModel.AddCharacters("~!@#$%^&*()_+{}|:\"<>?");
		}
		
		private void TestNavigateLeft()
		{	
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("and the dog jumped over the log");
			documentModel.MoveCaret(-4, -4);
		}
		
		private void TestNavigateRight()
		{	
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("and the dog jumped over the log");
			documentModel.MoveCaret(-4, -4);
			documentModel.MoveCaret(2, 2);
		}
		
		private void TestNavigatePastEndOfDocument()
		{	
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("and the dog jumped over the log");
			documentModel.MoveCaret(1, 0);
			documentModel.MoveCaret(-5, -5);
			documentModel.MoveCaret(7, 5);
		}
		
		private void TestNavigatePastBeginningOfDocument()
		{	
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("and the dog jumped over the log");
			documentModel.MoveCaretTo(0);
			documentModel.MoveCaret(-1, 0);
			documentModel.MoveCaret(5, 5);
			documentModel.MoveCaret(-7, -5);
		}
		
		private void TestDelete()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("in the house that Jack built with his two good hands");
			documentModel.MoveCaret(-10, -10);
			documentModel.DeleteCharacters(1, 1);
			documentModel.DeleteCharacters(3, 3);
		}
		
		private void TestDeleteFromEndOfDocument()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("in the house that Jack built with his two good hands");
			documentModel.DeleteCharacters(1, 0);
			documentModel.MoveCaret(-2, -2);
			documentModel.DeleteCharacters(3, 2);
		}
		
		private void TestBackspace()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("in the house that Jack built with his two good hands");
			documentModel.BackspaceCharacters(1, 1);
			documentModel.BackspaceCharacters(3, 3);
		}
		
		private void TestBackspaceFromBeginningOfDocument()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("in the house that Jack built with his two good hands");
			documentModel.MoveCaretTo(0);
			documentModel.BackspaceCharacters(1, 0);
			documentModel.MoveCaret(2, 2);
			documentModel.BackspaceCharacters(3, 2);
		}
		
		private void TestLoad_AddAndRemoveInOrder()
		{	
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			int charCount = 100000;
			int maxSeconds = 3;
			Random random = new Random(1);
			
			DateTime startTime = DateTime.Now;
			for(int i=0; i<charCount; i++)
			{
				documentModel.AddCharacters((char)(random.Next((int)'a', (int)'z')));
			}
			TimeSpan duration = DateTime.Now - startTime;
			TestUtilities.Assert(
				duration.Ticks > TimeSpan.TicksPerSecond * maxSeconds, 
				String.Format("Adding {0} characters took {1}h {2}m {3}s, longer than {4}s allowance", charCount, duration.Hours, duration.Minutes, duration.Seconds, maxSeconds));
			TestUtilities.Assert(
				documentModel.Length == charCount, 
				String.Format("added {0} characters but found only {1}", charCount, documentModel.Length));

			documentModel.MoveCaretTo(0);

			startTime = DateTime.Now;
			for(int i=0; i<charCount; i++)
			{
				documentModel.DeleteCharacters(1, 1);
			}
			duration = DateTime.Now - startTime;
			TestUtilities.Assert(
				duration.Ticks < TimeSpan.TicksPerSecond * maxSeconds, 
				String.Format("Deleting {0} characters took {1}h {2}m {3}s, longer than {4}s allowance", charCount, duration.Hours, duration.Minutes, duration.Seconds, maxSeconds));
			TestUtilities.Assert(
				documentModel.Length == 0, 
				String.Format("deleted {0} characters but still found {1}", charCount, documentModel.Length));
		}
		
		private void TestLoad_AddAndRemoveRandomly()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			int editCount = 100000;
			int maxSeconds = 3;
			Random random = new Random(1);

			DateTime startTime = DateTime.Now;
			for(int i=0; i<editCount; i++)
			{
				int position = random.Next(0, documentModel.Length+1);
				documentModel.MoveCaretTo(position);
				if(documentModel.Length > 0 && random.Next(0,3) == 0) /* 1/3 chance erase */
				{
					if(random.Next(0,2) == 0) /* 1/2 chance delete or backspace */
					{
						if(documentModel.CaretPosition == 0)
							documentModel.BackspaceCharacters(1, 0);
						else
							documentModel.BackspaceCharacters(1, 1);
					}
					else
					{
						if(documentModel.CaretPosition == documentModel.Length)
							documentModel.DeleteCharacters(1, 0);
						else
							documentModel.DeleteCharacters(1, 1);
					}
				}
				else
				{
					documentModel.AddCharacters((char)(random.Next((int)'a', (int)'z')));
				}
			}
			TimeSpan duration = DateTime.Now - startTime;
			TestUtilities.Assert(
				duration.Ticks < TimeSpan.TicksPerSecond * maxSeconds, 
				String.Format("Random editing {0} times took {1}h {2}m {3}s, longer than {4}s allowance", editCount, duration.Hours, duration.Minutes, duration.Seconds, maxSeconds));
		}
		
		private void TestUndoInEmptyDocument()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.Undo(0, 0);
		}
		
		private void TestUndoOneLetter()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("a");
			documentModel.Undo(0, 0);
		}
		
		private void TestUndoSeveralLetters()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("abcdef");
			documentModel.Undo(0, 0);
		}
		
		private void TestUndo100Letters()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			for(int i=0; i<100; i++)
			{
				documentModel.AddCharacters("x");
			}
			documentModel.Undo();
			TestUtilities.Assert(documentModel.Length > 0 && documentModel.Length < 100, "length wrong");
			TestUtilities.Assert(documentModel.Length == documentModel.CaretPosition, "caret position wrong");
		}
		
		private void TestUndoWordWithSpace()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("one ");
			documentModel.Undo(0, 0);
		}
		
		private void TestUndoSeveralWords()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("one two three");
			documentModel.Undo(8, 8);
			documentModel.Undo(4, 4);
			documentModel.Undo(0, 0);
		}
		
		private void TestUndoRemovingSpaceBetweenWords()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("one two");
			documentModel.MoveCaretTo(3);
			documentModel.DeleteCharacters(1,1);
			documentModel.Undo(7, 3);
			documentModel.Undo(4, 4);
			documentModel.AddCharacters("two");
			documentModel.MoveCaretTo(4);
			documentModel.BackspaceCharacters(1,1);
			documentModel.Undo(7, 4);
			documentModel.Undo(4, 4);
		}
		
		private void TestUndoHyphenatedWord()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("Amelia-Rory");
			documentModel.Undo(0, 0);
		}
		
		private void TestUndoReplacingSpaceWithHyphen()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("stone angels");
			documentModel.MoveCaretTo(5);
			documentModel.DeleteCharacters(1,1);
			documentModel.AddCharacters("-");
			documentModel.Undo(11, 5);
			documentModel.Undo(12, 5);
			documentModel.Undo(6, 6);
		}
		
		private void TestUndoPuttingHyphenInWord()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("abcdefghijklmnopQRSTUVWXYZ");
			documentModel.MoveCaretTo(16);
			documentModel.AddCharacters("-");
			documentModel.Undo(26, 16);
			documentModel.Undo(0, 0);
		}
		
		private void TestUndoWordWithComma()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("and then, th,en");
			documentModel.Undo(10, 10);
			documentModel.Undo(4, 4);
		}
		
		private void TestUndoWordWithPeriod()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("and then. th.en");
			documentModel.Undo(10, 10);
			documentModel.Undo(4, 4);
		}
		
		private void TestUndoWordWithSemiColon()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("and then; th;en");
			documentModel.Undo(10, 10);
			documentModel.Undo(4, 4);
		}
		
		private void TestUndoWordWithColon()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("and then: th:en");
			documentModel.Undo(10, 10);
			documentModel.Undo(4, 4);
		}
		
		private void TestUndoWordWithApostrophe()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("and then' th'en");
			documentModel.Undo(10, 10);
			documentModel.Undo(4, 4);
		}

		private void TestUndoWordWithQuote()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("and then\" th\"en");
			documentModel.Undo(10, 10);
			documentModel.Undo(4, 4);
		}

		private void TestUndoWordWithQuestionMark()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("and then? th?en");
			documentModel.Undo(10, 10);
			documentModel.Undo(4, 4);
		}

		private void TestUndoWordWithExclamationMark()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("and then! th!en");
			documentModel.Undo(10, 10);
			documentModel.Undo(4, 4);
		}
	}
	
	public class DocumentModelWrapper : DocumentModel
	{
		public DocumentModelWrapper()
		{
		}
		
		public void BackspaceCharacters(int charCount, int expectedCharsRemoved)
		{
			int startIndex = this.CaretPosition;
			int startLength = this.Length;
			RaiseEraseEvent(TextUnit.Character, -1*charCount);
			TestUtilities.Assert(this.Length == startLength - (Math.Min(startIndex, charCount)), String.Format("error backspacing {0} times", charCount));
			TestUtilities.Assert(this.Length == startLength - expectedCharsRemoved, String.Format("error backspacing, length does not match expected"));
		}
		
		public void DeleteCharacters(int charCount, int expectedCharsRemoved)
		{
			int startIndex = this.CaretPosition;
			int startLength = this.Length;
			RaiseEraseEvent(TextUnit.Character, charCount);
			TestUtilities.Assert(this.Length == startLength - (Math.Min(startLength-startIndex, charCount)), String.Format("error deleting {0} characters", charCount));
			TestUtilities.Assert(this.Length == startLength - expectedCharsRemoved, String.Format("error deleting, length does not match expected"));
		}
		
		public void AddCharacters(char c)
		{
			AddCharacters(c.ToString());
		}
		
		public void AddCharacters(string characters)
		{
			int startIndex = this.CaretPosition;
			int index = startIndex;
			foreach(char c in characters)
			{
				int startLength = this.Length;
				RaiseTextEvent(c);
				TestUtilities.Assert(this.Length == startLength+1, String.Format("error adding character '{0}' to document model", c));
				index++;
			}
			string observedCharacters = this.SubString(startIndex, index-1);
			TestUtilities.Assert(observedCharacters == characters, String.Format("error adding text '{0}' to document model.", characters));
		}
		
		public void MoveCaretTo(int index)
		{
			MoveCaret(index-this.CaretPosition, index-this.CaretPosition);
			TestUtilities.Assert(this.CaretPosition == index, String.Format("error moving caret to specific index, length {0}, target index {1}, ended up at {2}", this.Length, index, this.CaretPosition));
		}
		
		public void MoveCaret(int distance, int expectedChange)
		{
			int currentPosition = this.CaretPosition;
			RaiseNavigationHorizontalEvent(TextUnit.Character, distance);
			TestUtilities.Assert(currentPosition + expectedChange == this.CaretPosition, String.Format("error moving caret from {0} by {1}, document length {2}", currentPosition, distance, this.Length));
		}
		
		public void Undo()
		{
			RaiseUndoEvent();
		}
		
		public void Undo(int expectedLength, int expectedCaretPosition)
		{
			RaiseUndoEvent();
			TestUtilities.Assert(this.Length == expectedLength, "wrong document length after undo");
			TestUtilities.Assert(this.CaretPosition == expectedCaretPosition, "wrong caret position after undo");
		}
		
		private void RaiseTextEvent(char text)
		{
			this.OnTextEvent(this, new TextEventArgs(text));
		}

		private void RaiseNavigationHorizontalEvent(TextUnit units, int distance)
		{
			this.OnNavigationHorizontalEvent(this, new NavigationHorizontalEventArgs(units, distance));
		}
		
		private void RaiseEraseEvent(TextUnit units, int count)
		{
			this.OnEraseEvent(this, new EraseEventArgs(units, count));
		}
		
		private void RaiseUndoEvent()
		{
			this.OnUndoEvent(this, new EventArgs());
		}
	}
}
