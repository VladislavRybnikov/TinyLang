using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using TinyLang.Compiler.Core.Parsing;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Fluent;
using TinyLang.IDE.Services.ScriptAnalyze;
using TinyLang.IDE.Services.ScriptAnalyze.Models;
using TinyLang.IDE.Services.ScriptRunning;
using TinyLang.IDE.Utils;
using TinyLang.IDE.Utils.SyntaxDefinition;

namespace TinyLang.IDE
{
    public partial class MainWindow : Window
    {
        private TextRange lastTextRange;
        private readonly IScriptRunObservable _runner = new ScriptRunner();
        private readonly IScriptAnalyzer _analyzer; 
        private readonly List<IScriptRunObserver> _runObservers = new List<IScriptRunObserver>();

        public MainWindow()
        {
            InitializeComponent();
            Console.SetOut(new ControlWriter(txtBx2));

            _analyzer = new ScriptAnalyzer();

            _runObservers.Add(new ScriptRunOutput(txtBx2, tv1));
            _runObservers.ForEach(observer => _runner.Subscribe(observer));
            txtBx1.SyntaxHighlighting = new TinyLangDefinition();

            //var sub = Observable.FromEventPattern(txtBx1, nameof(txtBx1.TextChanged))
            //    .Select(_ => txtBx1.Document.FullRange().Text)
            //    .Where(t => t.Length > 2)
            //    .Throttle(TimeSpan.FromSeconds(0.5))
            //    .DistinctUntilChanged()
            //    .Select(async (r) => await _analyzer.AnalyzeForTagsAsync(r))
            //    .Switch()
            //    .SelectMany(t => t)
            //    .Where(x => x != null)
            //    .DistinctUntilChanged()
            //    .Distinct()
            //    .ObserveOnDispatcher()
            //    .Subscribe(HighlightTag);

            btn1.Click += OnBtn1Click;

        }

        private void OnBtn1Click(object sender, RoutedEventArgs args)
           // => _runner.Run(txtBx1.Document.FullRange().Text);
           => _runner.Run(txtBx1.Text);

        //private void HighlightTag(Tag tag)
        //{
        //    var lines = txtBx1.Document.Blocks.ToArray();

        //    var lineNum = tag.Line;

        //    var line = lines[lineNum];

        //    var startOffset = tag.Type == TagType.StringLiteral ? 2 : 0;
        //    var endOffset = tag.Type == TagType.StringLiteral ? 2 : 0;

        //    var s = line.ContentStart.GetPositionAtOffset(tag.Column + 1 + startOffset);
        //    var e = s.GetPositionAtOffset(tag.Value.Length + endOffset);

        //    var range = new TextRange(s, e);
        //    range.ClearAllProperties();
        //    var brush = tag.Type switch
        //    {
        //        TagType.FuncName => Brushes.Blue,
        //        TagType.StringLiteral => Brushes.Orange,
        //        _ => Brushes.Black
        //    };
            
        //    range.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
        //}
    }
}
