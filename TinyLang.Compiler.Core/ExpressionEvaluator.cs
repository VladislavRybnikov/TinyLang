using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.NumOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.BoolOperations;

namespace TinyLang.Compiler.Core
{
    public class EvaluationResult
    {
        public object Value { get; set; }
    }

    public static class ExpressionEvaluator
    {
        static EvaluationResult Res(object val) => new EvaluationResult { Value = val };

        public static EvaluationResult Evaluate(Expr expr) =>
            expr switch
            {
                IntExpr i => Res(i.Value),
                BoolExpr b => Res(b.Value),
                AddExpr add => Res((int) Evaluate(add.Left).Value + (int) Evaluate(add.Right).Value),
                MulExpr mult => Res((int) Evaluate(mult.Left).Value * (int) Evaluate(mult.Right).Value),
                AndExpr and => Res((bool) Evaluate(and.Left).Value && (bool) Evaluate(and.Right).Value),
                _ => throw new Exception("unsupported")

            };
    }
}
