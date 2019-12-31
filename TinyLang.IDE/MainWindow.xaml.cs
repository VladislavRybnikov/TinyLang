using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TinyLang.Compiler.Core.Common.Exceptions.Base;
using TinyLang.IDE.Services.ScriptAnalyze;
using TinyLang.IDE.Services.ScriptAnalyze.TextMarkers;
using TinyLang.IDE.Services.ScriptRunning;
using TinyLang.IDE.Utils;
using TinyLang.IDE.Utils.Extensions;
using TinyLang.IDE.Utils.SyntaxDefinition;

namespace TinyLang.IDE
{

    public partial class MainWindow : Window
    {
        private readonly IScriptRunObservable _runner = new ScriptRunner();
        private IScriptAnalyzer _analyzer;
        private ITextMarkerService _textMarkerService;
        private ILineChangeTracker _lineTracker;
        private readonly List<IScriptRunObserver> _runObservers = new List<IScriptRunObserver>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeRunComponents();
            InitializeAnalyzeComponents();
        }

        private void InitializeRunComponents() 
        {
            Console.SetOut(new ControlWriter(txtBx2));

            _runObservers.Add(new ScriptRunOutput(txtBx2, tv1));
            _runObservers.ForEach(observer => _runner.Subscribe(observer));

            btn1.Click += OnBtn1Click;
        }

        private void InitializeAnalyzeComponents()
        {
            _analyzer = new ScriptAnalyzer();
            _textMarkerService = new TextMarkerService(txtBx1.Document);
            _lineTracker = new LineTracker();

            txtBx1.SyntaxHighlighting = new TinyLangDefinition();

            _textMarkerService.AddTo(txtBx1.TextArea.TextView,
                v => v.BackgroundRenderers,
                v => v.LineTransformers);

            _lineTracker.AddTo(txtBx1.Document.LineTrackers);

            _lineTracker.DocumentLineChanges.Subscribe(OnLineChange);
            ParseErrors.Subscribe(OnParseError);

        }

        private void OnBtn1Click(object sender, RoutedEventArgs args)
           => _runner.Run(txtBx1.Text);

        private IObservable<PositionedException> ParseErrors => Observable.FromEventPattern(txtBx1, nameof(txtBx1.TextChanged))
                .Select(_ => txtBx1.Text)
                .Where(t => t.Length > 2)
                .Throttle(TimeSpan.FromSeconds(0.5))
                .Select(async (t) => await _analyzer.AnalyzeForExceptions(t))
                .Switch()
                .Where(x => x != null)
                .ObserveOnDispatcher()
                .SelectMany(x => x);

        private void OnParseError(PositionedException ex)
        {
            try
            {
                var line = txtBx1.Document.GetLineByNumber(ex.Position.Line + 1);
                var l = line.Length - ex.Position.Column;
                var marker = _textMarkerService.Create(line.Offset + ex.Position.Column, l);
                marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                marker.MarkerColor = Colors.Red;
            }
            catch { }
        }

        private void OnLineChange(DocumentLineChange change)
        {
            _textMarkerService.RemoveAll(m => change.Offset >= m.StartOffset
                       && m.StartOffset + m.Length <= change.Offset + change.ChangedLine.Length);
            lnCountValueLbl.Content = _lineTracker.LinesCount + 1;
        }
    }
}
