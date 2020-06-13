using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Constructions
{
    public class WhileExpr : ScopedExpr
    {
        Expr Predicate { get; set; }

        public WhileExpr(Expr predicate)
        {
            Predicate = predicate;
        }

        public WhileExpr()
        {
        }

        public override string ToString()
        {
            return $"While(Predicate({Predicate}), {Scope})";
        }
    }

    public class DoWhileExpr : ScopedExpr
    {
        Expr Predicate { get; set; }

        public DoWhileExpr(Expr predicate)
        {
            Predicate = predicate;
        }

        public DoWhileExpr()
        {
        }

        public override string ToString()
        {
            return $"DoWhile(Predicate({Predicate}), {Scope})";
        }
    }
}
