using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Types
{
    public class IntExpr : Expr
    {
        public int Value { get; set; }
        public IntExpr(int value)
        {
            Value = value;
        }

        public IntExpr()
        {
        }

        public override string ToString()
        {
            return $"Int({Value})";
        }
    }
}
