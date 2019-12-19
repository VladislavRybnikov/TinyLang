using System;
using System.Linq;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;

namespace TinyLang.Fluent.Models
{
    public class FuncCall : IStatement
    {
        Expr _e;

        public FuncCall(Func<string, Expr> f, string name, params string[] args)
            => _e = FuncExpr.Invoke(name, args.Select(f));

        Expr IStatement.GetExpr() => _e;
    }
}
