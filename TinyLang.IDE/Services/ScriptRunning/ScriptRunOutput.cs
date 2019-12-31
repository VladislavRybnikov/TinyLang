using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json.Linq;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.IDE.Utils.Extensions;

namespace TinyLang.IDE.Services.ScriptRunning
{
    public interface IScriptRunObserver : IObserver<AST> 
    {

    }

    public class ScriptRunOutput : IScriptRunObserver
    {
        private readonly TextBoxBase _consoleOut;
        private readonly TreeView _astTreeView;
        public ScriptRunOutput(TextBoxBase consoleOut, TreeView astTreeView)
        {
            _consoleOut = consoleOut;
            _astTreeView = astTreeView;
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
            _astTreeView.Items.Clear();
            _astTreeView.Items.Add(astTree);
        }
    }
}
