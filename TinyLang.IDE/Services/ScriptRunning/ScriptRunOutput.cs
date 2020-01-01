using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json.Linq;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.IDE.Utils.Extensions;
using System.Windows.Documents;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using TinyLang.Compiler.Core.Parsing;
using System.Text.RegularExpressions;
using TinyLang.IDE.Core;

namespace TinyLang.IDE.Services.ScriptRunning
{
    public interface IScriptRunObserver : IObserver<AST> 
    {
        void AddTreeViewEnricher(IEnricher<TreeViewItem> enricher);
    }

    public class ScriptRunOutput : IScriptRunObserver
    {
        private readonly TextBoxBase _consoleOut;
        private readonly TreeView _astTreeView;
        private readonly List<IEnricher<TreeViewItem>> _treeViewEnrichers;

        public ScriptRunOutput(TextBoxBase consoleOut, TreeView astTreeView)
        {
            _consoleOut = consoleOut;
            _astTreeView = astTreeView;
            _treeViewEnrichers = new List<IEnricher<TreeViewItem>>();
        }

        public void AddTreeViewEnricher(IEnricher<TreeViewItem> enricher)
        {
            _treeViewEnrichers.Add(enricher);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(AST value)
        {
            var astTree = JToken.Parse(value.ToString()).ToTreeViewItem("AST");

            Enrich(astTree);

            _astTreeView.Items.Clear();
            _astTreeView.Items.Add(astTree);
        }

        private void Enrich(TreeViewItem item) 
            => _treeViewEnrichers.ForEach(enricher => enricher.Enrich(item));
    }   
}
