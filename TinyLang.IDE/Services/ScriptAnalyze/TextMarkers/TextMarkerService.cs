using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace TinyLang.IDE.Services.ScriptAnalyze.TextMarkers
{
	public class TextMarkerService : DocumentColorizingTransformer, ITextMarkerService, ITextViewConnect
    {
		readonly TextSegmentCollection<TextMarker> _markers;
		readonly TextDocument _document;

		public event EventHandler RedrawRequested;

		public KnownLayer Layer => KnownLayer.Selection;

		public TextMarkerService(TextDocument document)
		{
			_document = document ?? throw new ArgumentNullException("document");
			_markers = new TextSegmentCollection<TextMarker>(document);
		}

		public ITextMarker Create(int startOffset, int length)
		{
			if (_markers == null)
				throw new InvalidOperationException("Cannot create a marker when not attached to a document");

			int textLength = _document.TextLength;
			if (startOffset < 0 || startOffset > textLength)
				throw new ArgumentOutOfRangeException("startOffset", startOffset, "Value must be between 0 and " + textLength);
			if (length < 0 || startOffset + length > textLength)
				throw new ArgumentOutOfRangeException("length", length, "length must not be negative and startOffset+length must not be after the end of the document");

			TextMarker m = new TextMarker(this, startOffset, length);
			_markers.Add(m);

			return m;
		}

		public IEnumerable<ITextMarker> GetMarkersAtOffset(int offset)
			=> _markers == null ? Enumerable.Empty<ITextMarker>() : _markers.FindSegmentsContaining(offset);

		public IEnumerable<ITextMarker> TextMarkers => _markers ?? Enumerable.Empty<ITextMarker>();

		public void RemoveAll(Predicate<ITextMarker> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");

			if (_markers != null)
			{
				foreach (TextMarker m in _markers.Where(x => predicate(x)).ToArray())
				{
					Remove(m);
				}
			}
		}

		public void Remove(ITextMarker marker)
		{
			if (marker == null)
				throw new ArgumentNullException("marker");

			TextMarker m = marker as TextMarker;
			if (_markers != null && _markers.Remove(m))
			{
				Redraw(m);
				m.OnDeleted();
			}
		}

		internal void Redraw(ISegment segment)
		{
			foreach (var view in textViews)
			{
				view.Redraw(segment, DispatcherPriority.Normal);
			}

			RedrawRequested?.Invoke(this, EventArgs.Empty);
		}

		protected override void ColorizeLine(DocumentLine line)
		{
			if (_markers == null)
				return;
			int lineStart = line.Offset;
			int lineEnd = lineStart + line.Length;
			foreach (TextMarker marker in _markers.FindOverlappingSegments(lineStart, line.Length))
			{
				Brush foregroundBrush = null;
				if (marker.ForegroundColor != null)
				{
					foregroundBrush = new SolidColorBrush(marker.ForegroundColor.Value);
					foregroundBrush.Freeze();
				}
				ChangeLinePart(
					Math.Max(marker.StartOffset, lineStart),
					Math.Min(marker.EndOffset, lineEnd),
					element => {
						if (foregroundBrush != null)
						{
							element.TextRunProperties.SetForegroundBrush(foregroundBrush);
						}
						Typeface tf = element.TextRunProperties.Typeface;
						element.TextRunProperties.SetTypeface(new Typeface(
							tf.FontFamily,
							marker.FontStyle ?? tf.Style,
							marker.FontWeight ?? tf.Weight,
							tf.Stretch
						));
					}
				);
			}
		}

		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");
			if (drawingContext == null)
				throw new ArgumentNullException("drawingContext");
			if (_markers == null || !textView.VisualLinesValid)
				return;
			var visualLines = textView.VisualLines;
			if (visualLines.Count == 0)
				return;
			int viewStart = visualLines.First().FirstDocumentLine.Offset;
			int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;
			foreach (TextMarker marker in _markers.FindOverlappingSegments(viewStart, viewEnd - viewStart))
			{
				if (marker.BackgroundColor != null)
				{
					BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder();
					geoBuilder.AlignToWholePixels = true;
					geoBuilder.CornerRadius = 3;
					geoBuilder.AddSegment(textView, marker);
					Geometry geometry = geoBuilder.CreateGeometry();
					if (geometry != null)
					{
						Color color = marker.BackgroundColor.Value;
						SolidColorBrush brush = new SolidColorBrush(color);
						brush.Freeze();
						drawingContext.DrawGeometry(brush, null, geometry);
					}
				}
				var underlineMarkerTypes = TextMarkerTypes.SquigglyUnderline | TextMarkerTypes.NormalUnderline | TextMarkerTypes.DottedUnderline;
				if ((marker.MarkerTypes & underlineMarkerTypes) != 0)
				{
					foreach (Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, marker))
					{
						Point startPoint = r.BottomLeft;
						Point endPoint = r.BottomRight;

						Brush usedBrush = new SolidColorBrush(marker.MarkerColor);
						usedBrush.Freeze();
						if ((marker.MarkerTypes & TextMarkerTypes.SquigglyUnderline) != 0)
						{
							double offset = 2.5;

							int count = Math.Max((int)((endPoint.X - startPoint.X) / offset) + 1, 4);

							StreamGeometry geometry = new StreamGeometry();

							using (StreamGeometryContext ctx = geometry.Open())
							{
								ctx.BeginFigure(startPoint, false, false);
								ctx.PolyLineTo(CreatePoints(startPoint, endPoint, offset, count).ToArray(), true, false);
							}

							geometry.Freeze();

							Pen usedPen = new Pen(usedBrush, 1);
							usedPen.Freeze();
							drawingContext.DrawGeometry(Brushes.Transparent, usedPen, geometry);
						}
						if ((marker.MarkerTypes & TextMarkerTypes.NormalUnderline) != 0)
						{
							Pen usedPen = new Pen(usedBrush, 1);
							usedPen.Freeze();
							drawingContext.DrawLine(usedPen, startPoint, endPoint);
						}
						if ((marker.MarkerTypes & TextMarkerTypes.DottedUnderline) != 0)
						{
							Pen usedPen = new Pen(usedBrush, 1);
							usedPen.DashStyle = DashStyles.Dot;
							usedPen.Freeze();
							drawingContext.DrawLine(usedPen, startPoint, endPoint);
						}
					}
				}
			}
		}

		IEnumerable<Point> CreatePoints(Point start, Point end, double offset, int count) 
			=> Enumerable.Range(0, count)
			.Select(i => new Point(start.X + i * offset, start.Y - ((i + 1) % 2 == 0 ? offset : 0)));

		readonly List<TextView> textViews = new List<TextView>();

		void ITextViewConnect.AddToTextView(TextView textView)
		{
			if (textView != null && !textViews.Contains(textView))
			{
				textViews.Add(textView);
			}
		}

		void ITextViewConnect.RemoveFromTextView(TextView textView)
		{
			if (textView != null)
			{
				textViews.Remove(textView);
			}
		}
	}

	public sealed class TextMarker : TextSegment, ITextMarker
	{
		private readonly TextMarkerService _service;

		private Color? _backgroundColor;
		private Color? _foregroundColor;
		private FontWeight? _fontWeight;
		private FontStyle? _fontStyle;
		private TextMarkerTypes _markerTypes;
		private Color _markerColor;

		public TextMarker(TextMarkerService service, int startOffset, int length)
		{
			_service = service ?? throw new ArgumentNullException("service");
			StartOffset = startOffset;
			Length = length;
			_markerTypes = TextMarkerTypes.None;
		}

		public event EventHandler Deleted;

		public bool IsDeleted => !IsConnectedToCollection;

		public void Delete() => _service.Remove(this);

		internal void OnDeleted() => Deleted?.Invoke(this, EventArgs.Empty);

		void Redraw() => _service.Redraw(this);

		public Color? BackgroundColor
		{
			get => _backgroundColor;
			set
			{
				if (_backgroundColor != value)
				{
					_backgroundColor = value;
					Redraw();
				}
			}
		}

		public Color? ForegroundColor
		{
			get => _foregroundColor;
			set
			{
				if (_foregroundColor != value)
				{
					_foregroundColor = value;
					Redraw();
				}
			}
		}

		public FontWeight? FontWeight
		{
			get => _fontWeight;
			set
			{
				if (_fontWeight != value)
				{
					_fontWeight = value;
					Redraw();
				}
			}
		}

		public FontStyle? FontStyle
		{
			get => _fontStyle;
			set
			{
				if (_fontStyle != value)
				{
					_fontStyle = value;
					Redraw();
				}
			}
		}

		public object Tag { get; set; }

		public TextMarkerTypes MarkerTypes
		{
			get => _markerTypes;
			set
			{
				if (_markerTypes != value)
				{
					_markerTypes = value;
					Redraw();
				}
			}
		}

		public Color MarkerColor
		{
			get => _markerColor;
			set
			{
				if (_markerColor != value)
				{
					_markerColor = value;
					Redraw();
				}
			}
		}
	}

}
