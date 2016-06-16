using System;

namespace Spire
{
	public class DocumentView
	{
		private DocumentModel documentModel;
		
		public DocumentView(DocumentModel model)
		{
			documentModel = model;
			CaretIndex = 0;
		}
		
		public int CaretIndex
		{
			get;
			set;
		}
		
		public string Line(int lineIndex)
		{
			return documentModel.SubString(0, documentModel.Length-1);
		}
		
		public void OnModelUpdateEvent(object sender, UpdateAtEventArgs e)
		{
//			Console.WriteLine("view got model's update message");
		}
	}
}