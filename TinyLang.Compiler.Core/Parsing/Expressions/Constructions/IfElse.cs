using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Constructions
{
    public class IfElseExpr : Expr
    {
        public IfExpr If { get; }
        public IEnumerable<ElifExpr> Elifs { get; }
        public ElseExpr Else { get; }

        public IfElseExpr(IfExpr @if, IEnumerable<ElifExpr> elifs = null, ElseExpr @else = null)
        {
            If = @if;
            Elifs = elifs ?? Enumerable.Empty<ElifExpr>();
            Else = @else;
        }

        public override string ToString()
        {
            var elifs = Elifs != null && Elifs.Any() ? $"\n{string.Join("\n", Elifs)}" : string.Empty;
            var @else = Else != null ? $"\n{Else}" : string.Empty;

            return $"{If}{elifs}{@else}";
        }
    }
    public class IfExpr : ScopedExpr
    {
        public Expr Predicate { get; }
        public IfExpr(Expr predicate)
        {
            Predicate = predicate;
        }

        public override string ToString()
        {
            return $"If(Predicate({Predicate}), {Scope})";
        }
    }
    public class ElifExpr : ScopedExpr
    {
        public Expr Predicate { get; }
        public ElifExpr(Expr predicate)
        {
            Predicate = predicate;
        }

        public override string ToString()
        {
            return $"Elif(Predicate({Predicate}), {Scope})";
        }
    }

    public class ElseExpr : ScopedExpr
    {
        public override string ToString()
        {
            return $"Else({Scope})";
        }
    }
}
