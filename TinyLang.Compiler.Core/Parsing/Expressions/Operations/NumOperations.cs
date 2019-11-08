using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Operations
{
    public static class NumOperations
    {
        public class AddExpr : Expr
        {
            public Expr Right { get; }
            public Expr Left { get; }

            public AddExpr(Expr left, Expr right)
            {
                Right = right;
                Left = left;
            }
            public override string ToString()
            {
                return $"Add({Left}, {Right})";
            }
        }

        public class SubtrExpr : Expr
        {
            public Expr Left { get; }

            public Expr Right { get; }

            public SubtrExpr(Expr left, Expr right)
            {
                Right = right;
                Left = left;
            }
            public override string ToString()
            {
                return $"Subtr({Left}, {Right})";
            }
        }

        public class MulExpr : Expr
        {
            public Expr Right { get; }
            public Expr Left { get; }

            public MulExpr(Expr left, Expr right)
            {
                Right = right;
                Left = left;
            }
            public override string ToString()
            {
                return $"Mul({Left}, {Right})";
            }
        }
        public class DivExpr : Expr 
        {
            public Expr Right { get; }
            public Expr Left { get; }

            public DivExpr(Expr left, Expr right)
            {
                Right = right;
                Left = left;
            }
            public override string ToString()
            {
                return $"Div({Left}, {Right})";
            }
        }
    }
}
