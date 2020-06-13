namespace TinyLang.Compiler.Core.Parsing.Expressions.Operations
{
    public static class NumOperations
    {
        public class AddExpr : BinaryExpr
        {
            public AddExpr()
            {
            }

            public AddExpr(Expr left, Expr right) : base(left, right) { }

            public override string Op => "+";
        }

        public class SubtrExpr : BinaryExpr
        {
            public SubtrExpr()
            {
            }

            public SubtrExpr(Expr left, Expr right) : base(left, right) { }

            public override string Op => "-";
        }

        public class MulExpr : BinaryExpr
        {
            public MulExpr()
            {
            }

            public MulExpr(Expr left, Expr right) : base(left, right) { }

            public override string Op => "*";
        }
        public class DivExpr : BinaryExpr
        {
            public DivExpr()
            {
            }

            public DivExpr(Expr left, Expr right) : base(left, right) { }

            public override string Op => "/";
        }
    }
}
