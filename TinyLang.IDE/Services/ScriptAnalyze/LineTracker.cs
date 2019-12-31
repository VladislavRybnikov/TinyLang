using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace TinyLang.IDE.Services.ScriptAnalyze
{
	public interface ILineChangeTracker : ILineTracker 
	{
		int LinesCount { get; }
		IObservable<DocumentLineChange> DocumentLineChanges { get; }
	}

	public class DocumentLineChange 
	{
		public DocumentLine ChangedLine { get; set; }
		public int Offset { get; set; }
	}

    public class LineTracker : ILineChangeTracker
	{
		public int LinesCount { get; private set; }

		private Subject<DocumentChangeEventArgs> _docSubject = new Subject<DocumentChangeEventArgs>();
		private Subject<DocumentLine> _lineSubject = new Subject<DocumentLine>();

		public IObservable<DocumentLineChange> DocumentLineChanges
			=> _docSubject.CombineLatest(_lineSubject, (d, l) => new DocumentLineChange 
			{
				ChangedLine = l,
				Offset = d.Offset
			});

		public void BeforeRemoveLine(DocumentLine line)
		{
			LinesCount--;
		}

		public void ChangeComplete(DocumentChangeEventArgs e)
		{
			_docSubject.OnNext(e);
		}

		public void LineInserted(DocumentLine insertionPos, DocumentLine newLine)
		{
			LinesCount++;
		}

		public void RebuildDocument()
		{
		}

		public void SetLineLength(DocumentLine line, int newTotalLength)
		{
			_lineSubject.OnNext(line);
		}
	}
}
