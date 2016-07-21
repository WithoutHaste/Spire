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
		
		public void RunLongTests(ref bool allTestsPassed)
		{
			TestUtilities.RunTest(TestLoad_AddAndRemoveInOrder, ref allTestsPassed);
			TestUtilities.RunTest(TestLoad_AddAndRemoveRandomly, ref allTestsPassed);
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
			TestUtilities.RunTest(TestUndoInNewDocument, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoMoreThanAvailable, ref allTestsPassed);
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
			TestUtilities.RunTest(TestUndoContinuingWordAfterMoving, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoContinuingBackspaceAfterMoving, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoContinuingDeleteAfterMoving, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoContinuingWordAfterHighlighting, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoContinuingBackspaceAfterHighlighting, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoContinuingDeleteAfterHighlighting, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoContinuingWordAfterTypingElsewhere, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoContinuingBackspaceAfterEditingElsewhere, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoContinuingDeleteAfterEditingElsewhere, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoEditWordAfterTypingElsewhere, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoDigit, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoDigits, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoDigitsInWord, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoDigitsEndingWord, ref allTestsPassed);
			TestUtilities.RunTest(TestUndoDigitsStartingWord, ref allTestsPassed);
			TestUtilities.RunTest(TestRedoInNewDocument, ref allTestsPassed);
			TestUtilities.RunTest(TestRedoTyping, ref allTestsPassed);
			TestUtilities.RunTest(TestRedoBackspace, ref allTestsPassed);
			TestUtilities.RunTest(TestRedoDelete, ref allTestsPassed);
			TestUtilities.RunTest(TestRedoMoreThanAvailable, ref allTestsPassed);
			TestUtilities.RunTest(TestRedoSeveralTimes, ref allTestsPassed);
			TestUtilities.RunTest(TestRedoListClearsOnEdit, ref allTestsPassed);
			TestUtilities.RunTest(TestNoHighlight, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightLeft, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightRight, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightLeftThenRight, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightRightThenLeft, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightThenUnhighlight, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightPastBeginningOfDocument, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightPastEndOfDocument, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightIsClearedAfterUndo, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightIsClearedAfterRedo, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightingMovesCaret, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightGreaterThanCaretThenDelete, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightLessThanCaretThenDelete, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightGreaterThanCaretThenBackspace, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightLessThanCaretThenBackspace, ref allTestsPassed);
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
		
		private void TestUndoInNewDocument()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.Undo(0, 0);
		}
		
		private void TestUndoMoreThanAvailable()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("abc def ghi");
			documentModel.Undo(8, 8);
			documentModel.Undo(4, 4);
			documentModel.Undo(0, 0);
			documentModel.Undo(0, 0);
			documentModel.Undo(0, 0);
		}
		
		private void TestUndoOneLetter()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("a");
			documentModel.BackspaceCharacters(1,1);
			documentModel.Undo(1, 1);
			documentModel.VerifyTextEquals("a");
			documentModel.MoveCaret(-1, -1);
			documentModel.DeleteCharacters(1, 1);
			documentModel.Undo(1, 0);
			documentModel.VerifyTextEquals("a");
			documentModel.Undo(0, 0);
		}
		
		private void TestUndoSeveralLetters()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("abcdef");
			documentModel.BackspaceCharacters(3, 3);
			documentModel.Undo(6, 6);
			documentModel.VerifyTextEquals("abcdef");
			documentModel.MoveCaret(-3, -3);
			documentModel.DeleteCharacters(3, 3);
			documentModel.Undo(6, 3);
			documentModel.VerifyTextEquals("abcdef");
			documentModel.Undo(0, 0);
		}
		
		private void TestUndo100Letters()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			int count = 100;
			for(int i=0; i<count; i++)
			{
				documentModel.AddCharacters("x");
			}
			string fullText = documentModel.SubString(0, count-1);
			documentModel.BackspaceCharacters(count, count);
			while(documentModel.Length < count)
			{
				documentModel.Undo();
			}
			documentModel.VerifyTextEquals(fullText);
			documentModel.MoveCaretTo(0);
			documentModel.DeleteCharacters(count, count);
			while(documentModel.Length < count)
			{
				documentModel.Undo();
			}
			documentModel.VerifyTextEquals(fullText);
			documentModel.Undo();
			TestUtilities.Assert(documentModel.Length > 0 && documentModel.Length < count, "length wrong");
			TestUtilities.Assert(documentModel.Length == documentModel.CaretPosition, "caret position wrong");
		}
		
		private void TestUndoWordWithSpace()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("one ");
			documentModel.BackspaceCharacters(4, 4);
			documentModel.Undo(4, 4);
			documentModel.VerifyTextEquals("one ");
			documentModel.MoveCaretTo(0);
			documentModel.DeleteCharacters(4, 4);
			documentModel.Undo(4, 0);
			documentModel.VerifyTextEquals("one ");
			documentModel.Undo(0, 0);
		}
		
		private void TestUndoSeveralWords()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("one two three");
			documentModel.BackspaceCharacters(13, 13);
			documentModel.Undo(13, 13);
			documentModel.VerifyTextEquals("one two three");
			documentModel.MoveCaretTo(0);
			documentModel.DeleteCharacters(13, 13);
			documentModel.Undo(13, 0);
			documentModel.VerifyTextEquals("one two three");
			documentModel.Undo(8, 8);
			documentModel.VerifyTextEquals("one two ");
			documentModel.Undo(4, 4);
			documentModel.VerifyTextEquals("one ");
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
			documentModel.BackspaceCharacters(11, 11);
			documentModel.Undo(11, 11);
			documentModel.VerifyTextEquals("Amelia-Rory");
			documentModel.MoveCaretTo(0);
			documentModel.DeleteCharacters(11, 11);
			documentModel.Undo(11, 0);
			documentModel.VerifyTextEquals("Amelia-Rory");
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
		
		private void TestUndoPunctuationAsCharacter(char punctuation)
		{
			string fullText = String.Format("and then{0} th{0}en {0}then", punctuation);
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters(fullText);
			documentModel.BackspaceCharacters(21, 21);
			documentModel.Undo(21, 21);
			documentModel.VerifyTextEquals(fullText);
			documentModel.MoveCaretTo(0);
			documentModel.DeleteCharacters(21, 21);
			documentModel.Undo(21, 0);
			documentModel.VerifyTextEquals(fullText);
			documentModel.Undo(16, 16);
			documentModel.Undo(10, 10);
			documentModel.Undo(4, 4);
		}
		
		private void TestUndoWordWithComma()
		{
			TestUndoPunctuationAsCharacter(',');
		}
		
		private void TestUndoWordWithPeriod()
		{
			TestUndoPunctuationAsCharacter('.');
		}
		
		private void TestUndoWordWithSemiColon()
		{
			TestUndoPunctuationAsCharacter(';');
		}
		
		private void TestUndoWordWithColon()
		{
			TestUndoPunctuationAsCharacter(':');
		}
		
		private void TestUndoWordWithApostrophe()
		{
			TestUndoPunctuationAsCharacter('\'');
		}

		private void TestUndoWordWithQuote()
		{
			TestUndoPunctuationAsCharacter('"');
		}

		private void TestUndoWordWithQuestionMark()
		{
			TestUndoPunctuationAsCharacter('?');
		}

		private void TestUndoWordWithExclamationMark()
		{
			TestUndoPunctuationAsCharacter('!');
		}
		
		private void TestUndoContinuingWordAfterMoving()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("chubby bunny");
			documentModel.MoveCaret(-3, -3);
			documentModel.MoveCaret(3, 3);
			documentModel.AddCharacters("Easter");
			documentModel.Undo(7, 7);
			documentModel.Undo(0, 0);
		}
		
		private void TestUndoContinuingBackspaceAfterMoving()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("whale in pail");
			documentModel.BackspaceCharacters(5, 5);
			documentModel.MoveCaret(-2, -2);
			documentModel.MoveCaret(2, 2);
			documentModel.BackspaceCharacters(3, 3);
			documentModel.Undo(13, 13);
			documentModel.VerifyTextEquals("whale in pail");
		}
		
		private void TestUndoContinuingDeleteAfterMoving()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("whale in pail");
			documentModel.MoveCaretTo(0);
			documentModel.DeleteCharacters(5, 5);
			documentModel.MoveCaret(2, 2);
			documentModel.MoveCaret(-2, -2);
			documentModel.DeleteCharacters(3, 3);
			documentModel.Undo(13, 0);
			documentModel.VerifyTextEquals("whale in pail");
		}
		
		private void TestUndoContinuingWordAfterHighlighting()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("agreed to attempt the nearly impossible ");
			documentModel.MoveHighlight(-3, -3);
			documentModel.MoveHighlight(3, 3);
			documentModel.AddCharacters("feat");
			documentModel.Undo(40, 40);
			documentModel.Undo(29, 29);
		}
		
		private void TestUndoContinuingBackspaceAfterHighlighting()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("agreed to attempt the nearly impossible ");
			documentModel.BackspaceCharacters(5, 5);
			documentModel.MoveHighlight(-3, -3);
			documentModel.MoveHighlight(3, 3);
			documentModel.BackspaceCharacters(3, 3);
			documentModel.Undo(40, 40);
			documentModel.VerifyTextEquals("agreed to attempt the nearly impossible ");
		}
		
		private void TestUndoContinuingDeleteAfterHighlighting()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("agreed to attempt the nearly impossible ");
			documentModel.MoveCaretTo(0);
			documentModel.DeleteCharacters(5, 5);
			documentModel.MoveHighlight(3, 3);
			documentModel.MoveHighlight(-3, -3);
			documentModel.DeleteCharacters(3, 3);
			documentModel.Undo(40, 0);
			documentModel.VerifyTextEquals("agreed to attempt the nearly impossible ");
		}
		
		private void TestUndoContinuingWordAfterTypingElsewhere()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("big red ball");
			documentModel.MoveCaretTo(4);
			documentModel.AddCharacters("soft");
			documentModel.MoveCaretTo(16);
			documentModel.AddCharacters("bearing");
			documentModel.Undo(16, 16);
			documentModel.Undo(12, 4);
			documentModel.Undo(8, 8);
			documentModel.Undo(4, 4);
			documentModel.Undo(0, 0);
		}
		
		private void TestUndoContinuingBackspaceAfterEditingElsewhere()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("whale in pail");
			documentModel.BackspaceCharacters(5, 5); //whale in//
			documentModel.MoveCaret(-2, -2);
			documentModel.BackspaceCharacters(1, 1); //whalein//
			documentModel.MoveCaret(2, 2);
			documentModel.BackspaceCharacters(3, 3); //whal//
			documentModel.VerifyTextEquals("whal");
			documentModel.Undo(7, 7);
			documentModel.VerifyTextEquals("whalein");
			documentModel.Undo(8, 6);
			documentModel.VerifyTextEquals("whale in");
			documentModel.Undo(13, 13);
			documentModel.VerifyTextEquals("whale in pail");
		}
		
		private void TestUndoContinuingDeleteAfterEditingElsewhere()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("whale in pail");
			documentModel.MoveCaretTo(0);
			documentModel.DeleteCharacters(5, 5); // in pail//
			documentModel.MoveCaret(2, 2);
			documentModel.DeleteCharacters(1, 1); // i pail//
			documentModel.MoveCaret(-2, -2);
			documentModel.DeleteCharacters(3, 3); //pail//
			documentModel.VerifyTextEquals("pail");
			documentModel.Undo(7, 0);
			documentModel.VerifyTextEquals(" i pail");
			documentModel.Undo(8, 2);
			documentModel.VerifyTextEquals(" in pail");
			documentModel.Undo(13, 0);
			documentModel.VerifyTextEquals("whale in pail");
		}
		
		private void TestUndoEditWordAfterTypingElsewhere()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("big red ball");
			documentModel.MoveCaretTo(4);
			documentModel.AddCharacters("soft");
			documentModel.MoveCaretTo(14);
			documentModel.AddCharacters("llalla");
			documentModel.Undo(16, 14);
			documentModel.Undo(12, 4);
			documentModel.Undo(8, 8);
		}
		
		private void TestUndoDigit()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("one 1 two");
			documentModel.BackspaceCharacters(9, 9);
			documentModel.Undo(9, 9);
			documentModel.VerifyTextEquals("one 1 two");
			documentModel.MoveCaretTo(0);
			documentModel.DeleteCharacters(9, 9);
			documentModel.Undo(9, 0);
			documentModel.VerifyTextEquals("one 1 two");
			documentModel.Undo(6, 6);
			documentModel.Undo(4, 4);
		}
		
		private void TestUndoDigits()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("one 1134 two");
			documentModel.BackspaceCharacters(12, 12);
			documentModel.Undo(12, 12);
			documentModel.VerifyTextEquals("one 1134 two");
			documentModel.MoveCaretTo(0);
			documentModel.DeleteCharacters(12, 12);
			documentModel.Undo(12, 0);
			documentModel.VerifyTextEquals("one 1134 two");
			documentModel.Undo(9, 9);
			documentModel.Undo(4, 4);
		}
		
		private void TestUndoDigitsInWord()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("one1134two");
			documentModel.BackspaceCharacters(10, 10);
			documentModel.Undo(10, 10);
			documentModel.VerifyTextEquals("one1134two");
			documentModel.MoveCaretTo(0);
			documentModel.DeleteCharacters(10, 10);
			documentModel.Undo(10, 0);
			documentModel.VerifyTextEquals("one1134two");
			documentModel.Undo(0, 0);
		}
		
		private void TestUndoDigitsEndingWord()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("one1134");
			documentModel.BackspaceCharacters(7, 7);
			documentModel.Undo(7, 7);
			documentModel.VerifyTextEquals("one1134");
			documentModel.MoveCaretTo(0);
			documentModel.DeleteCharacters(7, 7);
			documentModel.Undo(7, 0);
			documentModel.VerifyTextEquals("one1134");
			documentModel.Undo(0, 0);
		}
		
		private void TestUndoDigitsStartingWord()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("1134two");
			documentModel.BackspaceCharacters(7, 7);
			documentModel.Undo(7, 7);
			documentModel.VerifyTextEquals("1134two");
			documentModel.MoveCaretTo(0);
			documentModel.DeleteCharacters(7, 7);
			documentModel.Undo(7, 0);
			documentModel.VerifyTextEquals("1134two");
			documentModel.Undo(0, 0);
		}

		private void TestRedoInNewDocument()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.Redo(0, 0);
		}

		private void TestRedoTyping()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("one two three");
			documentModel.Undo(8, 8);
			documentModel.VerifyTextEquals("one two ");
			documentModel.Redo(13, 13);
			documentModel.VerifyTextEquals("one two three");
		}

		private void TestRedoBackspace()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("one two three");
			documentModel.BackspaceCharacters(7, 7);
			documentModel.Undo(13, 13);
			documentModel.VerifyTextEquals("one two three");
			documentModel.Redo(6, 6);
			documentModel.VerifyTextEquals("one tw");
		}

		private void TestRedoDelete()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("one two three");
			documentModel.MoveCaretTo(0);
			documentModel.DeleteCharacters(7, 7);
			documentModel.Undo(13, 0);
			documentModel.VerifyTextEquals("one two three");
			documentModel.Redo(6, 0);
			documentModel.VerifyTextEquals(" three");
		}
		
		private void TestRedoMoreThanAvailable()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("abc def ghi");
			documentModel.Undo(8, 8);
			documentModel.Undo(4, 4);
			documentModel.Undo(0, 0);
			documentModel.Redo(4, 4);
			documentModel.Redo(8, 8);
			documentModel.Redo(11, 11);
			documentModel.Redo(11, 11);
			documentModel.Redo(11, 11);
		}
		
		private void TestRedoSeveralTimes()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("The brown fox");
			documentModel.Undo(10, 10);
			documentModel.Undo(4, 4);
			documentModel.Undo(0, 0);
			documentModel.Redo(4, 4);
			documentModel.Redo(10, 10);
			documentModel.Redo(13, 13);
		}
		
		private void TestRedoListClearsOnEdit()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("The brown fox");
			documentModel.Undo(10, 10);
			documentModel.Undo(4, 4);
			documentModel.Undo(0, 0);
			documentModel.Redo(4, 4);
			documentModel.AddCharacters("chubby bunny");
			documentModel.Redo(16, 16);
			documentModel.Redo(16, 16);
		}
		
		private void TestNoHighlight()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("When the Black Dragon seized the Deep of Ylferdun,");
			documentModel.VerifyHighlightedTextEquals("");
		}
		
		private void TestHighlightLeft()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("When the Black Dragon seized the Deep of Ylferdun,");
			documentModel.MoveHighlight(-4, -4);
			documentModel.VerifyHighlightedTextEquals("dun,");
		}
		
		private void TestHighlightRight()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("When the Black Dragon seized the Deep of Ylferdun,");
			documentModel.MoveCaretTo(11);
			documentModel.MoveHighlight(4, 4);
			documentModel.VerifyHighlightedTextEquals("ack ");
		}
		
		private void TestHighlightLeftThenRight()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("young Gareth braved the far Winterlands to find John Aversin,");
			documentModel.MoveHighlight(-10, -10);
			documentModel.VerifyHighlightedTextEquals("n Aversin,");
			documentModel.MoveHighlight(5, 5);
			documentModel.VerifyHighlightedTextEquals("rsin,");
		}
		
		private void TestHighlightRightThenLeft()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("young Gareth braved the far Winterlands to find John Aversin,");
			documentModel.MoveCaretTo(11);
			documentModel.MoveHighlight(10, 10);
			documentModel.VerifyHighlightedTextEquals("h braved t");
			documentModel.MoveHighlight(-5, -5);
			documentModel.VerifyHighlightedTextEquals("h bra");
		}
		
		private void TestHighlightThenUnhighlight()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("young Gareth braved the far Winterlands to find John Aversin,");
			documentModel.MoveCaretTo(11);
			documentModel.MoveHighlight(10, 10);
			documentModel.VerifyHighlightedTextEquals("h braved t");
			documentModel.MoveHighlight(-10, -10);
			documentModel.VerifyHighlightedTextEquals("");
		}
		
		private void TestHighlightPastBeginningOfDocument()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("Dragonsbane - the only living man ever to slay a dragon.");
			documentModel.MoveHighlight(-1 * (documentModel.Length + 5), -1 * documentModel.Length);
			documentModel.VerifyHighlightedTextEquals("Dragonsbane - the only living man ever to slay a dragon.");
		}
		
		private void TestHighlightPastEndOfDocument()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("Dragonsbane - the only living man ever to slay a dragon.");
			documentModel.MoveCaretTo(0);
			documentModel.MoveHighlight(documentModel.Length + 5, documentModel.Length);
			documentModel.VerifyHighlightedTextEquals("Dragonsbane - the only living man ever to slay a dragon.");
		}
		
		private void TestHighlightIsClearedAfterUndo()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("Dragonsbane - the only living man ever to slay a dragon.");
			documentModel.MoveHighlight(-10, -10);
			documentModel.Undo(49, 49);
			documentModel.VerifyHighlightedTextEquals("");
		}
		
		private void TestHighlightIsClearedAfterRedo()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("Dragonsbane - the only living man ever to slay a dragon.");
			documentModel.Undo(49, 49);
			documentModel.MoveHighlight(-10, -10);
			documentModel.Redo(56, 56);
			documentModel.VerifyHighlightedTextEquals("");
		}
		
		private void TestHighlightingMovesCaret()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("Dragonsbane - the only living man ever to slay a dragon.");
			documentModel.MoveHighlight(-10, -10);
			TestUtilities.Assert(documentModel.CaretPosition == 46, String.Format("caret position after highlight = {0}", documentModel.CaretPosition));
		}
		
		private void TestHighlightGreaterThanCaretThenDelete()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("Act now and enjoy");
			documentModel.MoveHighlight(-5, -5);
			TestUtilities.Assert(documentModel.HighlightPosition > documentModel.CaretPosition, "test not setup right");
			documentModel.DeleteCharacters(1, 5);
			documentModel.VerifyTextEquals("Act now and ");
			documentModel.VerifyHighlightedTextEquals("");
		}

		private void TestHighlightLessThanCaretThenDelete()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("Act now and enjoy");
			documentModel.MoveCaretTo(3);
			documentModel.MoveHighlight(5, 5);
			TestUtilities.Assert(documentModel.HighlightPosition < documentModel.CaretPosition, "test not setup right");
			documentModel.DeleteCharacters(1, 5);
			documentModel.VerifyTextEquals("Actand enjoy");
			documentModel.VerifyHighlightedTextEquals("");
		}
		
		private void TestHighlightGreaterThanCaretThenBackspace()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("Act now and enjoy");
			documentModel.MoveHighlight(-5, -5);
			TestUtilities.Assert(documentModel.HighlightPosition > documentModel.CaretPosition, "test not setup right");
			documentModel.BackspaceCharacters(1, 5);
			documentModel.VerifyTextEquals("Act now and ");
			documentModel.VerifyHighlightedTextEquals("");
		}
		
		private void TestHighlightLessThanCaretThenBackspace()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			documentModel.AddCharacters("Act now and enjoy");
			documentModel.MoveCaretTo(3);
			documentModel.MoveHighlight(5, 5);
			TestUtilities.Assert(documentModel.HighlightPosition < documentModel.CaretPosition, "test not setup right");
			documentModel.BackspaceCharacters(1, 5);
			documentModel.VerifyTextEquals("Actand enjoy");
			documentModel.VerifyHighlightedTextEquals("");
		}
	}
	
	public class DocumentModelWrapper : DocumentModel
	{
		public DocumentModelWrapper()
		{
		}
		
		public void BackspaceCharacters(int charCount, int expectedCharsRemoved)
		{
			int startLength = this.Length;
			RaiseEraseEvent(TextUnit.Character, -1*charCount);
			TestUtilities.Assert(this.Length == startLength - expectedCharsRemoved, String.Format("error backspacing, length does not match expected"));
		}
		
		public void DeleteCharacters(int charCount, int expectedCharsRemoved)
		{
			int startLength = this.Length;
			RaiseEraseEvent(TextUnit.Character, charCount);
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
			RaiseCaretNavigationHorizontalEvent(TextUnit.Character, distance);
			TestUtilities.Assert(currentPosition + expectedChange == this.CaretPosition, String.Format("error moving caret from {0} by {1}, document length {2}", currentPosition, distance, this.Length));
		}
		
		public void MoveHighlightTo(int index)
		{
			Cindex currentHighlightPosition = this.HighlightPosition;
			MoveHighlight(index-this.CaretPosition, index-this.CaretPosition);
			TestUtilities.Assert(this.CaretPosition == index, String.Format("error moving highlight to specific index, caret position incorrect"));
			TestUtilities.Assert(this.HighlightPosition == currentHighlightPosition, String.Format("error moving highlight to specific index, highlight position incorrect"));
		}
		
		public void MoveHighlight(int distance, int expectedChange)
		{
			int currentHighlightPosition = this.HighlightPosition;
			int currentCaretPosition = this.CaretPosition;
			RaiseHighlightNavigationHorizontalEvent(TextUnit.Character, distance);
			TestUtilities.Assert(currentCaretPosition + expectedChange == this.CaretPosition, String.Format("caret error moving highlight from {0} by {1}, document length {2}", currentCaretPosition, distance, this.Length));
			TestUtilities.Assert(currentHighlightPosition == this.HighlightPosition, String.Format("highlight error moving highlight from {0} by {1}, document length {2}", currentCaretPosition, distance, this.Length));
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
		
		public void Redo()
		{
			RaiseRedoEvent();
		}
		
		public void Redo(int expectedLength, int expectedCaretPosition)
		{
			RaiseRedoEvent();
			TestUtilities.Assert(this.Length == expectedLength, "wrong document length after redo");
			TestUtilities.Assert(this.CaretPosition == expectedCaretPosition, "wrong caret position after redo");
		}
		
		public void VerifyTextEquals(string text)
		{
			if(this.Length == 0)
			{
				TestUtilities.Assert(text == "", "transcription error");
				return;
			}
			TestUtilities.Assert(this.SubString(0, this.Length-1) == text, "transcription error");
		}
		
		public void VerifyHighlightedTextEquals(string text)
		{
			if(this.Length == 0 || this.CaretPosition == this.HighlightPosition)
			{
				TestUtilities.Assert(text == "", "highlighted text error");
				return;
			}
			Cindex min = Math.Min(this.CaretPosition, this.HighlightPosition);
			Cindex max = Math.Max(this.CaretPosition, this.HighlightPosition);
			string substring = this.SubString(min, max-1);
			TestUtilities.Assert(substring == text, String.Format("highlighted text error: cindex {0} to {1} = '{2}', not '{3}'", min, max, substring, text));
		}
		
		private void RaiseTextEvent(char text)
		{
			this.OnTextEvent(this, new TextEventArgs(text));
		}

		private void RaiseCaretNavigationHorizontalEvent(TextUnit units, int distance)
		{
			while(distance < 0)
			{
				this.OnCaretNavigationHorizontalEvent(this, new NavigationHorizontalEventArgs(units, HorizontalDirection.Left));
				distance++;
			}
			while(distance > 0)
			{
				this.OnCaretNavigationHorizontalEvent(this, new NavigationHorizontalEventArgs(units, HorizontalDirection.Right));
				distance--;
			}
		}

		private void RaiseHighlightNavigationHorizontalEvent(TextUnit units, int distance)
		{
			while(distance < 0)
			{
				this.OnHighlightNavigationHorizontalEvent(this, new NavigationHorizontalEventArgs(units, HorizontalDirection.Left));
				distance++;
			}
			while(distance > 0)
			{
				this.OnHighlightNavigationHorizontalEvent(this, new NavigationHorizontalEventArgs(units, HorizontalDirection.Right));
				distance--;
			}
		}
		
		private void RaiseEraseEvent(TextUnit units, int count)
		{
			this.OnEraseEvent(this, new EraseEventArgs(units, count));
		}
		
		private void RaiseUndoEvent()
		{
			this.OnUndoEvent(this, new EventArgs());
		}
		
		private void RaiseRedoEvent()
		{
			this.OnRedoEvent(this, new EventArgs());
		}
	}
}
