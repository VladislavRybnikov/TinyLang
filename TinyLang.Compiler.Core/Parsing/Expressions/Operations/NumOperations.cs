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

            public AddExpr(Expr right, Expr left)
            {
                Right = right;
                Left = left;
            }
            public override string ToString()
            {
                return $"Add({Left}, {Right})";
            }
        }

        public class MulExpr : Expr
        {
            public Expr Right { get; }
            public Expr Left { get; }

            public MulExpr(Expr right, Expr left)
            {
                Right = right;
                Left = left;
            }
            public override string ToString()
            {
                return $"Mul({Left}, {Right})";
            }
        }
    }
}
