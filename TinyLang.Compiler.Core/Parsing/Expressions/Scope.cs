using System.Collections.Generic;
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

    public class FuncDeclaration : ScopedExpr
    {
        public IEnumerable<VarExpr> Args { get; }

        public FuncDeclaration(IEnumerable<VarExpr> args) 
        {
            Args = args;
        }


        public override string ToString()
        {
            return $"Func(Args({string.Join(", ", Args)}), {Scope})";
        }
    }

    public class FuncInvocation : Expr
    {
        public IEnumerable<Expr> Parameters { get; }

        public FuncInvocation(IEnumerable<Expr> parameters) 
        {
            Parameters = parameters;
        }
    }

}
