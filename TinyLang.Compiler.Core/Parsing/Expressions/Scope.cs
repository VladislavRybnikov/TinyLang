using System.Collections.Generic;
using System.Linq;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.Compiler.Core.Parsing.Expressions
{
    public class Scope : Expr
    {
        public List<Expr> Statements { get; }

        public Scope(List<Expr> statements) 
        {
            Statements = statements;
        }

        public Scope(params Expr[] statements)
        {
            Statements = statements.ToList();
        }

        public Scope() 
        {
            Statements = new List<Expr>();
        }

        public override string ToString() 
        {
            return $"Scope({string.Join(", ", Statements)})";
        }

        public class EndOfScope : Expr
        {

        }

        public class StartOfScope : Expr 
        {

        }
    }

    public class ScopedExpr : Expr 
    {
        public Scope Scope { get; set; }
    }

}
