using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Constructions
{
    public class While : ScopedExpr
    {
        Expr Predicate { get; }

        public While(Expr predicate)
        {
            Predicate = predicate;
        }

        public override string ToString()
        {
            return $"While(Predicate({Predicate}), {Scope})";
        }
    }

    public class DoWhile : ScopedExpr
    {
        Expr Predicate { get; }

        public DoWhile(Expr predicate)
        {
            Predicate = predicate;
        }

        public override string ToString()
        {
            return $"DoWhile(Predicate({Predicate}), {Scope})";
        }
    }
}
