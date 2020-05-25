using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TinyLang.Compiler.Core.Common.Exceptions.Base;
using TinyLang.IDE.Services.ScriptAnalyze;
using TinyLang.IDE.Services.ScriptAnalyze.TextMarkers;
using TinyLang.IDE.Services.ScriptRunning;
using TinyLang.IDE.Utils.Extensions;
using TinyLang.IDE.Utils.SyntaxDefinition;

namespace TinyLang.IDE
{

    public partial class MainWindow : Window
    {
        private IScriptAnalyzer _analyzer;
        private ITextMarkerService _textMarkerService;
        private ILineChangeTracker _lineTracker;
        private IScriptRunProcessor _scriptRunProcessor;

        public MainWindow()
        {
            InitializeComponent();
            InitializeRunComponents();
            InitializeAnalyzeComponents();

        }

        private void InitializeRunComponents() 
        {
            _scriptRunProcessor = new ScriptRunProcessor
            {
                OutputTextBox = txtBx2,
                ASTTreeView = tv1
            };

            _scriptRunProcessor.AddTreeViewEnricher(new TreeViewIconEnricher());
            _scriptRunProcessor.CompilationErrorOccured.Subscribe(er 
                => MessageBox.Show(er.Message, "Compilation error", MessageBoxButton.OK, MessageBoxImage.Error));

            var runRequested = Observable.Merge(new []
            {
                Observable.FromEventPattern(RunMenuItem, nameof(RunMenuItem.Click)),
                Observable.FromEventPattern(DebugRunMenuItem, nameof(DebugRunMenuItem.Click)),
                Observable.FromEventPattern(this, nameof(KeyDown))
                    .Where(_ => Keyboard.Modifiers == ModifierKeys.Control && Keyboard.IsKeyDown(Key.F5))
            });

            
            runRequested.Select(_  => txtBx1.Text)
                .SubscribeOnDispatcher()
                .Subscribe(_scriptRunProcessor);
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            (new SettingsForm
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            }).ShowDialog();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            txtBx1.Undo();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            txtBx2.Redo();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tv1.Items.Clear();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            txtBx2.Clear();
        }
    }
}
