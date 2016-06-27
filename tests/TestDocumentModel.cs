using System;
//using System.Diagnostics;
using Spire;

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
	}
}