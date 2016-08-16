using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Spire
{
	public class DocumentModel
	{
		public delegate void UpdateAtEventHandler(object sender, UpdateAtEventArgs e);
		public event UpdateAtEventHandler OnUpdateAtEvent;

		private Cindex _caretPosition;
		private Cindex? _highlightPosition;
		private DocumentChunkCollection chunks;
		private History history;
	
		public DocumentModel()
		{
			chunks = new DocumentChunkCollection();
			CaretPosition = 0;
			history = new History();
		}
		
		public int Length
		{
			get { return chunks.Length; }
		}
		
		public Cindex CaretPosition
		{
			get
			{
				return _caretPosition;
			}
			set 
			{
				if(value == _highlightPosition)
				{
					_highlightPosition = null;
				}
				if(value < 0) value = 0;
				if(value > Length) value = Length;
				_caretPosition = value;
			}
		}
		
		public Cindex HighlightPosition
		{
			get
			{
				if(_highlightPosition == null)
					return _caretPosition;
				return _highlightPosition.Value;
			}
			set
			{
				if(value == _caretPosition)
				{
					_highlightPosition = null;
					return;
				}
				if(value < 0) value = 0;
				if(value > Length) value = Length;
				_highlightPosition = value;
			}
		}
		
		public bool HasHighlight
		{
			get { return (HighlightPosition != CaretPosition); }
		}
		
		public char this[int cindex]
		{
			get { return chunks[cindex]; }
		}
		
		public void ClearHighlight()
		{
			_highlightPosition = null;
		}
		
		public void SetHighlight()
		{
			if(_highlightPosition == null)
				_highlightPosition = _caretPosition;
		}
		
		public string SubString(Cindex from, Cindex to)
		{
			return chunks.SubString(from, to);
		}
		
		public void OnTextEvent(object sender, TextEventArgs e)
		{
			if(HasHighlight)
			{
				ReplaceHighlight(e);
				return;
			}
			if(!e.IsHistoryEvent)
			{
				history.Add(new DocumentEdit_AddCharacters(CaretPosition, e.Text));
			}
			InsertText(e.Text, CaretPosition);
			CaretPosition += e.Text.Length;
		}
		
		private void ReplaceHighlight(TextEventArgs e)
		{
			if(!HasHighlight) 
				throw new Exception("Cannot call ReplaceHighlight when nothing is highlighted.");
			if(!e.IsHistoryEvent)
			{
				history.StartMultiEdit();
			}
			EraseHighlight();
			ClearHighlight();
			OnTextEvent(this, e);
			if(!e.IsHistoryEvent)
			{
				history.EndMultiEdit();
			}
		}
		
		public void OnCaretNavigationHorizontalEvent(object sender, NavigationHorizontalEventArgs e)
		{
			ClearHighlight();
			MoveCaretHorizontal(e);
		}

		public void OnHighlightNavigationHorizontalEvent(object sender, NavigationHorizontalEventArgs e)
		{
			SetHighlight();
			MoveCaretHorizontal(e);
		}
		
		private void MoveCaretHorizontal(NavigationHorizontalEventArgs e)
		{
			int amount = 0;
			switch(e.Direction)
			{
				case HorizontalDirection.Left: amount = -1; break;
				case HorizontalDirection.Right: amount = 1; break;
				default: throw new Exception(String.Format("HorizontalDirection {0} not supported in document navigation", e.Direction));
			}
			switch(e.Unit)
			{
				case TextUnit.Character: CaretPosition += amount; break;
				case TextUnit.Word: throw new Exception("navigation by word not implemented");
				default: throw new Exception(String.Format("Unit {0} not supported in document navigation", e.Unit));
			}
		}

		public void OnEraseEvent(object sender, EraseEventArgs e)
		{
			if(HasHighlight)
			{
				EraseHighlight();
				return;
			}
			switch(e.Unit)
			{
				case TextUnit.Character:
					EraseCharacters(e);
					break;
				case TextUnit.Word:
					throw new Exception("erase whole word not implemented");
				default:
					throw new Exception(String.Format("Unit {0} not supported in document erasures", e.Unit));
			}
		}
		
		private void EraseHighlight()
		{
			EraseCharacters(new EraseEventArgs(TextUnit.Character, _highlightPosition.Value - _caretPosition));
			ClearHighlight();
		}
		
		private void EraseCharacters(EraseEventArgs e)
		{
			int minCindex = CaretPosition;
			int maxCindex = CaretPosition;
			if(e.Amount < 0)
			{
				minCindex = Math.Max(0, CaretPosition+e.Amount);
				maxCindex = Math.Min(CaretPosition-1, Length-1);
				if(!e.IsHistoryEvent && minCindex <= maxCindex)
				{
					history.Add(new DocumentEdit_BackspaceCharacters(CaretPosition, SubString(minCindex,  maxCindex)));
				}
				BackspaceCharacters(Math.Abs(e.Amount));
			}
			else if(e.Amount > 0)
			{
				minCindex = CaretPosition;
				maxCindex = Math.Min(CaretPosition+e.Amount-1, Length-1);
				if(!e.IsHistoryEvent && minCindex <= maxCindex)
				{
					history.Add(new DocumentEdit_DeleteCharacters(CaretPosition, SubString(minCindex, maxCindex)));
				}
				DeleteCharacters(e.Amount);
			}
		}
		
		public void OnUndoEvent(object sender, EventArgs e)
		{
			history.Undo(this);
			ClearHighlight();
		}
		
		public void OnRedoEvent(object sender, EventArgs e)
		{
			history.Redo(this);
			ClearHighlight();
		}
		
		public void OnCopyEvent(object sender, EventArgs e)
		{
			if(!HasHighlight) return;
			Cindex min = Math.Min(CaretPosition, HighlightPosition);
			Cindex max = Math.Max(CaretPosition, HighlightPosition) - 1;
			Clipboard.SetText(SubString(min, max));
		}
		
		public void OnCutEvent(object sender, EventArgs e)
		{
			if(!HasHighlight) return;
			history.AddEditBreak();
			OnCopyEvent(sender, e);
			EraseHighlight();
			history.AddEditBreak();
		}
		
		public void OnPasteEvent(object sender, EventArgs e)
		{
			if(!Clipboard.ContainsText()) return;
			history.AddEditBreak();
			OnTextEvent(sender, new TextEventArgs(Clipboard.GetText()));
			history.AddEditBreak();
		}
		
		private void InsertText(string text, Cindex at)
		{
			chunks.AddAt(text, at);
			RaiseUpdateAtEvent(at);
		}

		private void BackspaceCharacters(int length)
		{
			if(_caretPosition == 0) return;
			int availableLength = Math.Min(length, _caretPosition);
			chunks.RemoveAt(_caretPosition - availableLength, availableLength);
			_caretPosition -= availableLength;
			RaiseUpdateAtEvent(_caretPosition);
		}
		
		private void DeleteCharacters(int length)
		{
			if(_caretPosition >= Length) return;
			int availableLength = Math.Min(length, Length - _caretPosition);
			chunks.RemoveAt(_caretPosition, availableLength);
			RaiseUpdateAtEvent(_caretPosition);
		}
		
		private void RaiseUpdateAtEvent(Cindex at)
		{
			if(OnUpdateAtEvent == null) return;
			OnUpdateAtEvent(this, new UpdateAtEventArgs(at));
		}
		
		public void SaveTXT(StreamWriter stream)
		{
			chunks.SaveTXT(stream);
		}
		
		public void LoadTXT(StreamReader stream)
		{
			chunks.LoadTXT(stream);
			_caretPosition = 0;
		}
	}
}
