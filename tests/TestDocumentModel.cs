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
		}
		
		private void TestAllKeyboardCharacters()
		{
			AddCharacters("abcdefghijklmnopqrstuvwxyz");
			AddCharacters("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
			AddCharacters("`1234567890-=[]\\;',./ ");
			AddCharacters("~!@#$%^&*()_+{}|:\"<>?");
		}
		
		private void AddCharacters(string characters)
		{
			try
			{
				int startIndex = documentModel.Length;
				int index = documentModel.Length;
				foreach(char c in characters)
				{
					RaiseTextEvent(c);
					TestUtilities.Assert(documentModel.Length == index+1, String.Format("Added char '{0}' to model, length should be {1}, observed {2}.", c, index+1, documentModel.Length));
					index++;
				}
				string observedCharacters = documentModel.SubString(startIndex, index-1);
				TestUtilities.Assert(observedCharacters == characters, String.Format("Expected model to equal '{0}', observed '{1}'.", characters, observedCharacters));
			}
			catch(Exception e)
			{
				TestUtilities.Assert(false, e.Message);
			}
		}
		
		private void RaiseTextEvent(char text)
		{
			if(documentModel == null) return;
			documentModel.OnTextEvent(this, new TextEventArgs(text));
		}

	}
}