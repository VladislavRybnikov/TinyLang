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

        public class OrExpr : Expr 
        {
            public Expr Left { get; }
            public Expr Right { get; }

            public OrExpr(Expr left, Expr right)
            {
                Left = left;
                Right = right;
            }

            public override string ToString()
            {
                return $"Or({Left}, {Right})";
            }
        }

        public class NotExpr : Expr 
        {
            public Expr Value { get; }

            public NotExpr(Expr value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return $"Not({Value})";
            }
        }
    }
}
