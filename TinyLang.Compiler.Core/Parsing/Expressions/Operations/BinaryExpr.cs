using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Operations
{
    public abstract class BinaryExpr : Expr
    {
        public Expr Right { get; }
        public Expr Left { get; }

        public BinaryExpr(Expr left, Expr right)
        {
            Right = right;
            Left = left;
        }

        public override string ToString()
        {
            return $"{GetType().Name.Replace(nameof(Expr), string.Empty)}({Left}, {Right})";
        }
    }
}
