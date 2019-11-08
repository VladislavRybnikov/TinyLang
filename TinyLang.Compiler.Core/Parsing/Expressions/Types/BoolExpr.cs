using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Types
{
    public class BoolExpr : Expr
    {
        public bool Value { get; }
        public BoolExpr(bool value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"Bool({Value})";
        }
    }
}
