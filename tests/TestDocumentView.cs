using Spire;
using System;
using System.Drawing;
using System.IO;

namespace SpireTest
{
	public class TestDocumentView
	{
		public TestDocumentView()
		{
		}
		
		public void RunLongTests(ref bool allTestsPassed)
		{
			TestUtilities.RunTest(TestDisplayLongText, ref allTestsPassed);
			TestUtilities.RunTest(TestDisplayTypingInMiddleOfDocument, ref allTestsPassed);
			TestUtilities.RunTest(TestDisplayWordsAlmostTooLongForLine, ref allTestsPassed);
			TestUtilities.RunTest(TestDisplayWordsTooLongForLine, ref allTestsPassed);
			TestUtilities.RunTest(TestDisplayDeletingInMiddleOfDocument, ref allTestsPassed);
		}
		
		public void RunTests(ref bool allTestsPassed)
		{
			TestUtilities.RunTest(TestDisplayNoText, ref allTestsPassed);
			TestUtilities.RunTest(TestDisplayShortText, ref allTestsPassed);
			TestUtilities.RunTest(TestDeletingInFirstWordOfLineTillWordFitsOnPreviousLine, ref allTestsPassed);
			TestUtilities.RunTest(TestMovingUp, ref allTestsPassed);
			TestUtilities.RunTest(TestMovingDown, ref allTestsPassed);
			TestUtilities.RunTest(TestSecondLineShorter, ref allTestsPassed);
			TestUtilities.RunTest(TestFirstLineShorter, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightUp, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightDown, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightUpThenDown, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightDownThenUp, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightUpPastBeginningOfDocument, ref allTestsPassed);
			TestUtilities.RunTest(TestHighlightDownPastEndOfDocument, ref allTestsPassed);
		}
		
		private void TestDisplayNoText()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentView.Display();
		}
		
		private void TestDisplayShortText()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("And yet, Dom Paulo's own Faith told him that the burden was there, had been there since Adam's time - and the burden imposed by a fiend crying in mockery, \"Man!\" at man.");
			documentView.Display();
		}
		
		private void TestDisplayLongText()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("And yet, Dom Paulo's own Faith told him that the burden was there, had been there since Adam's time - and the burden imposed by a fiend crying in mockery, \"Man!\" at man.");
			documentModel.AddCharacters("And yet, Dom Paulo's own Faith told him that the burden was there, had been there since Adam's time - and the burden imposed by a fiend crying in mockery, \"Man!\" at man.");
			documentModel.AddCharacters("And yet, Dom Paulo's own Faith told him that the burden was there, had been there since Adam's time - and the burden imposed by a fiend crying in mockery, \"Man!\" at man.");
			documentModel.AddCharacters("And yet, Dom Paulo's own Faith told him that the burden was there, had been there since Adam's time - and the burden imposed by a fiend crying in mockery, \"Man!\" at man.");
			documentModel.AddCharacters("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890 ");
			documentModel.AddCharacters("And yet, Dom Paulo's own Faith told him that the burden was there, had been there since Adam's time - and the burden imposed by a fiend crying in mockery, \"Man!\" at man.");
			documentModel.AddCharacters("And yet, Dom Paulo's own Faith told him that the burden was there, had been there since Adam's time - and the burden imposed by a fiend crying in mockery, \"Man!\" at man.");
			documentModel.AddCharacters("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890 ");
			documentModel.AddCharacters("And yet, Dom Paulo's own Faith told him that the burden was there, had been there since Adam's time - and the burden imposed by a fiend crying in mockery, \"Man!\" at man.");
			documentModel.AddCharacters("And yet, Dom Paulo's own Faith told him that the burden was there, had been there since Adam's time - and the burden imposed by a fiend crying in mockery, \"Man!\" at man.");
			documentModel.AddCharacters("And yet, Dom Paulo's own Faith told him that the burden was there, had been there since Adam's time - and the burden imposed by a fiend crying in mockery, \"Man!\" at man.");
			documentModel.AddCharacters("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890 ");
			documentModel.AddCharacters("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890 ");
			documentModel.AddCharacters("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890 ");
			documentModel.AddCharacters("And yet, Dom Paulo's own Faith told him that the burden was there, had been there since Adam's time - and the burden imposed by a fiend crying in mockery, \"Man!\" at man.");
			documentModel.AddCharacters("And yet, Dom Paulo's own Faith told him that the burden was there, had been there since Adam's time - and the burden imposed by a fiend crying in mockery, \"Man!\" at man.");
			documentModel.AddCharacters("And yet, Dom Paulo's own Faith told him that the burden was there, had been there since Adam's time - and the burden imposed by a fiend crying in mockery, \"Man!\" at man.");
			documentModel.AddCharacters("\"Man!\" - calling each to account for the deeds of all since the beginning; a burden impressed upon every generation before the opening of the womb, the burden of the guilt of original sin.");
			documentModel.AddCharacters("Brother Francis was not entirely convinced that they were talking about the donkey. \"Good day to you, sir,\" the monk said pleasantly. \"You may take the ass. Walking will improve my health, I think.\" He smiled again and started away.");
			documentView.Display();
		}
		
		private void TestDisplayTypingInMiddleOfDocument()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("We know little or nothing of Vermeer's personality, and it is dangerous to generalize from the evidence of his paintings. He left behind him a large family and many debts, and his life may have been sordid.");
			documentModel.AddCharacters("But a mind that is troubled may seek a peaceful refuge in art. Vermeer is almost as much a mystery as Shakespeare, but he is perhaps nearer to another British poet, his comtemporary Thomas Traherne (1637-74), whose work was almost lost for centuries and then recovered.");
			documentModel.MoveCaretTo(15);
			documentModel.AddCharacters("Vermeer's ability to convey a sense of absolute tranquility is nowhere more apparent than in this painting, his homage to his native city.");
			documentModel.MoveCaretTo(80);
			documentModel.AddCharacters("Air that can almost be felt, water and cool light are its chief elements.");
			documentView.Display();
		}
		
		private void TestDisplayWordsAlmostTooLongForLine()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("123456789012345678901234567890123456789012345678901234567890 ");
			documentModel.AddCharacters("123456789012345678901234567890123456789012345678901234567890 ");
			documentModel.AddCharacters("123456789012345678901234567890123456789012345678901234567890 ");
			documentView.Display();
		}

		private void TestDisplayWordsTooLongForLine()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890 ");
			documentModel.AddCharacters("123456789012345678901234567890123456789012345678901234567890 ");
			documentModel.AddCharacters("123456789012345678901234567890123456789012345678901234567890 ");
			documentModel.AddCharacters("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890 ");
			documentView.Display();
		}
		
		private void TestDisplayDeletingInMiddleOfDocument()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("We know little or nothing of Vermeer's personality, and it is dangerous to generalize from the evidence of his paintings. He left behind him a large family and many debts, and his life may have been sordid.");
			documentModel.AddCharacters("But a mind that is troubled may seek a peaceful refuge in art. Vermeer is almost as much a mystery as Shakespeare, but he is perhaps nearer to another British poet, his comtemporary Thomas Traherne (1637-74), whose work was almost lost for centuries and then recovered.");
			documentModel.MoveCaretTo(15);
			documentModel.DeleteCharacters(12, 12);
			documentModel.MoveCaretTo(65);
			documentModel.BackspaceCharacters(20, 20);
			documentView.Display();
		}
		
		private void TestDeletingInFirstWordOfLineTillWordFitsOnPreviousLine()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("One Two Threeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
			documentModel.MoveCaretTo(15);
			documentView.MoveCaretUpOrDown(1,1); //verify there are two lines
			documentModel.DeleteCharacters(documentModel.Length-documentModel.CaretPosition, documentModel.Length-documentModel.CaretPosition);
			documentView.MoveCaretUpOrDown(1,0); //there should now be just one line
		}
		
		private void TestMovingUp()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("We know little or nothing of Vermeer's personality, and it is dangerous to generalize from the evidence of his paintings. He left behind him a large family and many debts, and his life may have been sordid.");
			documentModel.AddCharacters("But a mind that is troubled may seek a peaceful refuge in art. Vermeer is almost as much a mystery as Shakespeare, but he is perhaps nearer to another British poet, his comtemporary Thomas Traherne (1637-74), whose work was almost lost for centuries and then recovered.");
			documentView.MoveCaretUpOrDown(-1, -1);
			documentModel.MoveCaretTo(12);
			documentView.MoveCaretUpOrDown(-1, 0);
		}
		
		private void TestMovingDown()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("We know little or nothing of Vermeer's personality, and it is dangerous to generalize from the evidence of his paintings. He left behind him a large family and many debts, and his life may have been sordid.");
			documentModel.AddCharacters("But a mind that is troubled may seek a peaceful refuge in art. Vermeer is almost as much a mystery as Shakespeare, but he is perhaps nearer to another British poet, his comtemporary Thomas Traherne (1637-74), whose work was almost lost for centuries and then recovered.");
			documentView.MoveCaretUpOrDown(1, 0);
			documentModel.MoveCaretTo(12);
			documentView.MoveCaretUpOrDown(1, 1);
		}
		
		private void TestSecondLineShorter()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			while(documentView.LineCount < 2)
			{
				documentModel.AddCharacters("watermelon ");
			}
			documentModel.AddCharacters("mushroom mushroom");
			documentModel.MoveCaretTo(30);
			documentView.MoveCaretUpOrDown(1, 1);
			documentModel.MoveCaret(1,0); //should be at end of document now
		}
		
		private void TestFirstLineShorter()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("One Two Three");
			while(documentView.LineCount < 2)
			{
				documentModel.AddCharacters("eeeee");
			}
			documentView.MoveCaretUpOrDown(-1, -1); //should be at end of first line
			documentModel.MoveCaret(1,1); //move to beginning of second line
			documentView.MoveCaretUpOrDown(1,0); //should be on last line now
		}
		
		private void TestHighlightUp()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("In return for the promise ");
			while(documentView.LineCount < 2)
			{
				documentModel.AddCharacters("of the King to send ");
			}
			documentView.MoveHighlightUpOrDown(-1, -1);
		}
		
		private void TestHighlightDown()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("In return for the promise ");
			while(documentView.LineCount < 2)
			{
				documentModel.AddCharacters("of the King to send ");
			}
			documentModel.MoveCaretTo(0);
			documentView.MoveHighlightUpOrDown(1, 1);
		}
		
		private void TestHighlightUpThenDown()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("In return for the promise ");
			while(documentView.LineCount < 2)
			{
				documentModel.AddCharacters("of the King to send ");
			}
			documentView.MoveHighlightUpOrDown(-1, -1);
			documentView.MoveHighlightUpOrDown(1, 1);
			documentModel.VerifyHighlightedTextEquals("");
		}

		private void TestHighlightDownThenUp()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("In return for the promise ");
			while(documentView.LineCount < 2)
			{
				documentModel.AddCharacters("of the King to send ");
			}
			documentModel.MoveCaretTo(0);
			documentView.MoveHighlightUpOrDown(1, 1);
			documentView.MoveHighlightUpOrDown(-1, -1);
			documentModel.VerifyHighlightedTextEquals("");
		}
		
		private void TestHighlightUpPastBeginningOfDocument()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("In return for the promise ");
			while(documentView.LineCount < 2)
			{
				documentModel.AddCharacters("of the King to send ");
			}
			documentView.MoveHighlightUpOrDown(-1, -1);
			documentView.MoveHighlightUpOrDown(-1, 0);
		}
		
		private void TestHighlightDownPastEndOfDocument()
		{
			DocumentModelWrapper documentModel = new DocumentModelWrapper();
			DocumentViewWrapper documentView = DocumentViewWrapper.Init(documentModel);
			documentModel.AddCharacters("In return for the promise ");
			while(documentView.LineCount < 2)
			{
				documentModel.AddCharacters("of the King to send ");
			}
			documentModel.MoveCaretTo(0);
			documentView.MoveHighlightUpOrDown(1, 1);
			documentView.MoveHighlightUpOrDown(1, 0);
		}
		
	}
	
	public class DocumentViewWrapper : DocumentView
	{
		private static Size pageSize = new Size(500, 500);
		
		public DocumentViewWrapper(DocumentModel documentModel) : base(documentModel)
		{
		}
		
		public static DocumentViewWrapper Init(DocumentModel documentModel)
		{
			DocumentViewWrapper documentView = new DocumentViewWrapper(documentModel);
			documentView.AppendDisplayArea(new DisplayArea(pageSize.Width, pageSize.Height));
			documentModel.OnUpdateAtEvent += new DocumentModel.UpdateAtEventHandler(documentView.OnModelUpdateEvent);
			return documentView;
		}
		
		public void Display()
		{
			Bitmap bitmap = new Bitmap(pageSize.Width, pageSize.Height);
			using(Graphics graphics = Graphics.FromImage(bitmap))
			{
				this.Paint(graphics, true);
			}
		}
		
		public void MoveCaretUpOrDown(int distance, int expectedChange)
		{
			int previousPosition = this.CaretPosition;
			RaiseCaretNavigationVerticalEvent(distance);
			bool success = true;
			if(expectedChange == 0) success = (previousPosition == this.CaretPosition);
			else if(expectedChange < 0) success = (this.CaretPosition < previousPosition);
			else if(expectedChange > 0) success = (this.CaretPosition > previousPosition);
			TestUtilities.Assert(success, String.Format("error moving caret from {0} by {1} line(s), expected change {2}", previousPosition, distance, expectedChange));
		}
		
		public void MoveHighlightUpOrDown(int distance, int expectedChange)
		{
			int previousCaretPosition = this.CaretPosition;
			int previousHighlightPosition = this.HighlightPosition;
			RaiseHighlightNavigationVerticalEvent(distance);
			bool success = true;
			if(expectedChange == 0) success = (previousCaretPosition == this.CaretPosition && previousHighlightPosition == this.HighlightPosition);
			else if(expectedChange < 0) success = (this.CaretPosition < previousCaretPosition && previousHighlightPosition == this.HighlightPosition);
			else if(expectedChange > 0) success = (this.CaretPosition > previousCaretPosition && previousHighlightPosition == this.HighlightPosition);
			TestUtilities.Assert(success, String.Format("error moving highlight from {0} by {1} line(s), expected change {2}", previousCaretPosition, distance, expectedChange));
		}
		
		private void RaiseCaretNavigationVerticalEvent(int distance)
		{
			while(distance < 0)
			{
				this.OnCaretNavigationVerticalEvent(this, new NavigationVerticalEventArgs(VerticalDirection.Up));
				distance++;
			}
			while(distance > 0)
			{
				this.OnCaretNavigationVerticalEvent(this, new NavigationVerticalEventArgs(VerticalDirection.Down));
				distance--;
			}
		}
		
		private void RaiseHighlightNavigationVerticalEvent(int distance)
		{
			while(distance < 0)
			{
				this.OnHighlightNavigationVerticalEvent(this, new NavigationVerticalEventArgs(VerticalDirection.Up));
				distance++;
			}
			while(distance > 0)
			{
				this.OnHighlightNavigationVerticalEvent(this, new NavigationVerticalEventArgs(VerticalDirection.Down));
				distance--;
			}
		}
		
	}
}
