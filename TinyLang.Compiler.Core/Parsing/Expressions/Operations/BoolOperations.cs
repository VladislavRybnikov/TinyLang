namespace TinyLang.Compiler.Core.Parsing.Expressions.Operations
{
    public static class BoolOperations
    {
        public class AndExpr : BinaryExpr
        {
            public AndExpr()
            {
            }

            public AndExpr(Expr left, Expr right) : base(left, right){ }

            public override string Op => "&&";
        }

        public class OrExpr : BinaryExpr
        {
            public OrExpr()
            {
            }

            public OrExpr(Expr left, Expr right) : base(left, right){ }

            public override string Op => "||";
        }

        public class NotExpr : Expr 
        {
            public string Op => "!";

            public Expr Value { get; set; }

            public NotExpr(Expr value)
            {
                Value = value;
            }

            public NotExpr()
            {
            }

            public override string ToString()
            {
                return $"Not({Value})";
            }
        }
    }
}
