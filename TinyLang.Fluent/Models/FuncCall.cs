using System;
using System.Linq;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;

namespace TinyLang.Fluent.Models
{
    public class FuncCall : IStatement
    {
        Expr _e;

        public FuncCall(string name, params IStatement[] args)
            => _e = FuncExpr.Invoke(name, args.Select(x => x.GetExpr()));

        Expr IStatement.GetExpr() => _e;
    }
}
