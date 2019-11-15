using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Operations
{
    public static class CompareOperations
    {
        public class EqExpr : Expr
        {
            public Expr Left { get; }
            public Expr Right { get; }
            public EqExpr(Expr left, Expr right)
            {
                Left = left;
                Right = right;
            }

            public override string ToString()
            {
                return $"Eq({Left}, {Right})";
            }
        }

        public class NotEqExpr : Expr
        {
            public Expr Left { get; }
            public Expr Right { get; }
            public NotEqExpr(Expr left, Expr right)
            {
                Left = left;
                Right = right;
            }

            public override string ToString()
            {
                return $"NotEq({Left}, {Right})";
            }
        }

        public class LessExpr : Expr
        {
            public Expr Left { get; }
            public Expr Right { get; }
            public LessExpr(Expr left, Expr right)
            {
                Left = left;
                Right = right;
            }

            public override string ToString()
            {
                return $"Less({Left}, {Right})";
            }
        }

        public class LessOrEqExpr : Expr
        {
            public Expr Left { get; }
            public Expr Right { get; }
            public LessOrEqExpr(Expr left, Expr right)
            {
                Left = left;
                Right = right;
            }

            public override string ToString()
            {
                return $"LessOrEq({Left}, {Right})";
            }
        }

        public class MoreExpr : Expr 
        {
            public Expr Left { get; }
            public Expr Right { get; }
            public MoreExpr(Expr left, Expr right)
            {
                Left = left;
                Right = right;
            }

            public override string ToString()
            {
                return $"More({Left}, {Right})";
            }

        }

        public class MoreOrEqExpr : Expr 
        {
            public Expr Left { get; }
            public Expr Right { get; }
            public MoreOrEqExpr(Expr left, Expr right)
            {
                Left = left;
                Right = right;
            }

            public override string ToString()
            {
                return $"MoreOrEq({Left}, {Right})";
            }
        }
    }
}
