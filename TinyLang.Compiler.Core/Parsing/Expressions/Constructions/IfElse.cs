using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Constructions
{
    public class IfElse : Expr
    {
        public If If { get; }
        public IEnumerable<Elif> Elifs { get; }
        public Else Else { get; }

        public IfElse(If @if, IEnumerable<Elif> elifs, Else @else = null)
        {
            If = @if;
            Elifs = elifs;
            Else = @else;
        }

        public override string ToString()
        {
            var elifs = Elifs != null && Elifs.Any() ? $"\n{string.Join("\n", Elifs)}" : string.Empty;
            var @else = Else != null ? $"\n{Else}" : string.Empty;

            return $"{If}{elifs}{@else}";
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
