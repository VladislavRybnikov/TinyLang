namespace TinyLang.Compiler.Core.Parsing.Expressions.Operations
{
    public static class BoolOperations
    {
        public class AndExpr : BinaryExpr
        {
            public AndExpr(Expr left, Expr right) : base(left, right){ }
        }

        public class OrExpr : BinaryExpr
        {
            public OrExpr(Expr left, Expr right) : base(left, right){ }
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
