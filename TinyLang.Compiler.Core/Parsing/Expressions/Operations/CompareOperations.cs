namespace TinyLang.Compiler.Core.Parsing.Expressions.Operations
{
    public static class CompareOperations
    {
        public class EqExpr : BinaryExpr
        {
            public EqExpr()
            {
            }

            public EqExpr(Expr left, Expr right) : base(left, right){ }

            public override string Op => "==";
        }

        public class NotEqExpr : BinaryExpr
        {
            public NotEqExpr()
            {
            }

            public NotEqExpr(Expr left, Expr right) : base(left, right){ }

            public override string Op => "!=";
        }

        public class LessExpr : BinaryExpr
        {
            public LessExpr()
            {
            }

            public LessExpr(Expr left, Expr right) : base(left, right){ }

            public override string Op => "<";
        }

        public class LessOrEqExpr : BinaryExpr
        {
            public LessOrEqExpr()
            {
            }

            public LessOrEqExpr(Expr left, Expr right) : base(left, right) { }

            public override string Op => "<=";
        }

        public class MoreExpr : BinaryExpr
        {
            public MoreExpr()
            {
            }

            public MoreExpr(Expr left, Expr right) : base(left, right) { }

            public override string Op => ">";
        }

        public class MoreOrEqExpr : BinaryExpr
        {
            public MoreOrEqExpr()
            {
            }

            public MoreOrEqExpr(Expr left, Expr right) : base(left, right) { }

            public override string Op => ">=";
        }
    }
}
