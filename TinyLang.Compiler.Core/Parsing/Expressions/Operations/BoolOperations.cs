using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Operations
{
    public static class BoolOperations
    {
        public class AndExpr : Expr
        {
            public Expr Left { get; }
            public Expr Right { get; }
            public AndExpr(Expr left, Expr right)
            {
                Left = left;
                Right = right;
            }

            public override string ToString()
            {
                return $"And({Left}, {Right})";
            }
        }
    }
}
