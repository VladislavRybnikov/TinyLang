using LanguageExt;
using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Constructions
{
    public class ForExpr : ScopedExpr
    {
        public ForExpr()
        {
        }

        public Expr Start { get; set; }

        public Expr End { get; set; }

        public Expr Step { get; set; }

        public static Expr Define(Expr start, Expr end, Option<Expr> step, Scope scope)
        {
            return new ForExpr
            {
                Start = start,
                End = end,
                Step = step.IfNone(new IntExpr(1)),
                Scope = scope
            };
        }
    }

    public class ForEachExpr : ScopedExpr
    {
        public ForEachExpr()
        {
        }

        public GeneralOperations.VarExpr Each { get; set; }

        public GeneralOperations.VarExpr Set { get; set; }
    }
}
