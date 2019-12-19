using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;

namespace TinyLang.Fluent.Models
{
    public interface IStatement
    {
        internal Expr GetExpr();
    }
}
