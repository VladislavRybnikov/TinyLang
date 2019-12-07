﻿namespace TinyLang.Compiler.Core.Parsing.Expressions.Operations
{
    public static class CompareOperations
    {
        public class EqExpr : BinaryExpr
        {
            public EqExpr(Expr left, Expr right) : base(left, right){ }

            public override string Op => "==";
        }

        public class NotEqExpr : BinaryExpr
        {
            public NotEqExpr(Expr left, Expr right) : base(left, right){ }

            public override string Op => "!=";
        }

        public class LessExpr : BinaryExpr
        {
            public LessExpr(Expr left, Expr right) : base(left, right){ }

            public override string Op => "<";
        }

        public class LessOrEqExpr : BinaryExpr
        {
            public LessOrEqExpr(Expr left, Expr right) : base(left, right) { }

            public override string Op => "<=";
        }

        public class MoreExpr : BinaryExpr
        {
            public MoreExpr(Expr left, Expr right) : base(left, right) { }

            public override string Op => ">";
        }

        public class MoreOrEqExpr : BinaryExpr
        {
            public MoreOrEqExpr(Expr left, Expr right) : base(left, right) { }

            public override string Op => ">=";
        }
    }
}
