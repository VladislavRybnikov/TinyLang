using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Types
{
    public class StrExpr : Expr
    {
        public string Value { get; }

        public StrExpr(string Value)
        {
            this.Value = Value;
        }

        public override string ToString()
        {
            return $"Str({Value})";
        }
    }
}
