using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Fluent;
using TinyLang.IDE.Core;
using TinyLang.IDE.Utils;
using TinyLang.IDE.Utils.Extensions;

namespace TinyLang.IDE.Services.ScriptRunning
{
    public enum SyntaxType { TinyLang, Json }

    public class Code
    {
        public SyntaxType SyntaxType { get; set; }

        public string Value { get; set; }
    }

    public class CompilationError : EventArgs 
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
    }

    public interface IScriptRunProcessor : IProcessor<Code, AST>
    {
        TextBoxBase OutputTextBox { get; set; }
        TreeView ASTTreeView { get; set; }
        AST AST { get; set; }

        void AddTreeViewEnricher(IEnricher<TreeViewItem> enricher);

        IObservable<CompilationError> CompilationErrorOccured { get; }
    }

    public class ScriptRunProcessor : BaseProcessor<Code, AST>, IScriptRunProcessor
    {
        private TextBoxBase _outputTextBox;
        private TreeView _astTreeView;
        private readonly List<IEnricher<TreeViewItem>> _treeViewEnrichers = new List<IEnricher<TreeViewItem>>();

        private Subject<CompilationError> _compilationErrors = new Subject<CompilationError>();

        public IObservable<CompilationError> CompilationErrorOccured => _compilationErrors;

        public TextBoxBase OutputTextBox 
        {
            get => _outputTextBox;
            set => SetOutputTextBox(value); 
        }
        public TreeView ASTTreeView
        {
            get => _astTreeView;
            set => SetASTTreeView(value); 
        }

        public AST AST { get; set; }

        public void AddTreeViewEnricher(IEnricher<TreeViewItem> enricher) => _treeViewEnrichers.Add(enricher);

        public override AST Process(Code val)
        {
            try
            {
                using var engine = val.SyntaxType == SyntaxType.TinyLang 
                    ? TinyLangEngine.FromScript(val.Value) 
                    : TinyLangEngine.FromAST(val.Value);

                engine.Execute(out var ast);
                AST = ast;
                return ast;
            }
            catch (Exception ex)
            {
                _compilationErrors.OnNext(new CompilationError
                {
                    Message = ex.Message
                });
            }

            return null;
        }

        private void SetOutputTextBox(TextBoxBase textBoxBase) 
        {
            _outputTextBox = textBoxBase;
            Console.SetOut(new ControlWriter(textBoxBase));
        }

        private void SetASTTreeView(TreeView astTreeView)
        {
            _astTreeView = astTreeView;

            this.Where(value => !(value is null)).Subscribe(value =>
            {
                var astTree = JToken.Parse(value.ToString()).ToTreeViewItem("AST");

                Enrich(astTree);

                _astTreeView.Items.Clear();
                _astTreeView.Items.Add(astTree);
            });
        }

        private void Enrich(TreeViewItem item)
           => _treeViewEnrichers.ForEach(enricher => enricher.Enrich(item));
    }
}
