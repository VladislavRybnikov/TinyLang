﻿namespace TinyLang.Compiler.Core.Parsing.Expressions.Operations
{
    public static class NumOperations
    {
        public class AddExpr : BinaryExpr
        {
            public AddExpr(Expr left, Expr right) : base(left, right) { }
        }

        public class SubtrExpr : BinaryExpr
        {
            public SubtrExpr(Expr left, Expr right) : base(left, right) { }
        }

        public class MulExpr : BinaryExpr
        {
            public MulExpr(Expr left, Expr right) : base(left, right) { }
        }
        public class DivExpr : BinaryExpr
        {
            public DivExpr(Expr left, Expr right) : base(left, right) { }
        }
    }
}
