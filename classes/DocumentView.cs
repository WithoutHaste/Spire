using System;

namespace Spire
{
	public class DocumentView
	{
		private DocumentModel documentModel;
		
		public DocumentView(DocumentModel model)
		{
			documentModel = model;
		}
		
		public int CaretIndex
		{
			get { return documentModel.CaretIndex; }
		}
		
		public string Line(int lineIndex)
		{
			if(documentModel.Length == 0) return "";
			return documentModel.SubString(0, documentModel.Length-1);
		}
		
		public void OnModelUpdateEvent(object sender, UpdateAtEventArgs e)
		{
//			Console.WriteLine("view got model's update message");
		}
	}
}