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

    public class If : ScopedExpr 
    {
        public Expr Predicate { get; }
        public If(Expr predicate)
        {
            Predicate = predicate;
        }

        public override string ToString()
        {
            return $"If(Predicate({Predicate}), {Scope})";
        }
    }
    public class Elif : ScopedExpr 
    {
        public Expr Predicate { get; }
        public Elif(Expr predicate)
        {
            Predicate = predicate;
        }

        public override string ToString()
        {
            return $"Elif(Predicate({Predicate}), {Scope})";
        }
    } 

    public class Else : ScopedExpr 
    {
        public override string ToString() 
        {
            return $"Else({Scope})";
        } 
    }
}
