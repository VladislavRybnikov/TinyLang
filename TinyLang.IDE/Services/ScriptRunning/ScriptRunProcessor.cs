using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
    public interface IScriptRunProcessor : IProcessor<string, AST>
    {
        TextBoxBase OutputTextBox { get; set; }
        TreeView ASTTreeView { get; set; }

        void AddTreeViewEnricher(IEnricher<TreeViewItem> enricher);
    }

    public class ScriptRunProcessor : BaseProcessor<string, AST>, IScriptRunProcessor
    {
        private TextBoxBase _outputTextBox;
        private TreeView _astTreeView;
        private readonly List<IEnricher<TreeViewItem>> _treeViewEnrichers = new List<IEnricher<TreeViewItem>>();

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

        public void AddTreeViewEnricher(IEnricher<TreeViewItem> enricher) => _treeViewEnrichers.Add(enricher);

        public override AST Process(string val)
        {
            using var engine = TinyLangEngine
                   .FromScript(val);

            engine.Execute(out var ast);
            return ast;
        }

        private void SetOutputTextBox(TextBoxBase textBoxBase) 
        {
            _outputTextBox = textBoxBase;
            Console.SetOut(new ControlWriter(textBoxBase));
        }

        private void SetASTTreeView(TreeView astTreeView)
        {
            _astTreeView = astTreeView;

            this.Subscribe(value =>
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
