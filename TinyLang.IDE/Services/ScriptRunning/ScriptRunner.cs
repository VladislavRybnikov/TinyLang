using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Fluent;

namespace TinyLang.IDE.Services.ScriptRunning
{
    public interface IScriptRunObservable : IObservable<AST>
    {
        void Run(string script);
    }

    public class ScriptRunner : IScriptRunObservable
    {
        private readonly Subject<AST> _subject = new Subject<AST>();

        public void Run(string script)
        {
            try
            {
                using var engine = TinyLangEngine
                    .FromScript(script);

                engine.Execute(out var ast);
                _subject.OnNext(ast);
            }
            catch (Exception ex)
            {
                _subject.OnError(ex);
            }
        }

        public IDisposable Subscribe(IObserver<AST> observer) => _subject.Subscribe(observer);
    }
}
