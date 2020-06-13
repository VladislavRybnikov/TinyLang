using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Types
{
    public class BoolExpr : Expr
    {
        public bool Value { get; set; }
        public BoolExpr(bool value)
        {
            Value = value;
        }

        public BoolExpr()
        {
        }

        public override string ToString()
        {
            return $"Bool({Value})";
        }
    }
}
