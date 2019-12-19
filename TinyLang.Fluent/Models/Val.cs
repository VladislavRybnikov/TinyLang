using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;

namespace TinyLang.Fluent.Models
{
    public class Val : IStatement
    {
        Expr _e;

        public Val(string str) => _e = new StrExpr(str);

        public Val(int i) => _e = new IntExpr(i);

        public Val(bool b) => _e = new BoolExpr(b);

        Expr IStatement.GetExpr() => _e;
    }
}
