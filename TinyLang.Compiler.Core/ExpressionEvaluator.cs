using System;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.NumOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.BoolOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;
using System.Collections.Concurrent;

namespace TinyLang.Compiler.Core
{
    public class EvaluationResult
    {
        public bool IsEmpty { get; set; }
        public object Value { get; set; }

        public EvaluationResult(object value) 
        {
            Value = value;
            IsEmpty = false;
        }

        public EvaluationResult() 
        {
            IsEmpty = true;
        }
    }

    public static class ExpressionEvaluator
    {
        private static readonly ConcurrentDictionary<string, object> _vars = new ConcurrentDictionary<string, object>();

        static EvaluationResult Res(object val) => new EvaluationResult(val);
        static EvaluationResult Empty => new EvaluationResult();

        public static EvaluationResult Evaluate(Expr expr) =>
            expr switch
            {
                IntExpr i => Res(i.Value),
                BoolExpr b => Res(b.Value),
                VarExpr v => Res(_vars.TryGetValue(v.Name, out var val) ? val : throw new Exception($"'{v.Name}' is not defined")),
                AssignExpr a => a.Assigment switch 
                {
                    VarExpr av => Set(av.Name, Evaluate(a.Value).Value),
                    _ => throw new Exception("Opertaion '=' is not allowed here")
                },
                AddExpr add => Res((int) Evaluate(add.Left).Value + (int) Evaluate(add.Right).Value),
                SubtrExpr subtr => Res((int) Evaluate(subtr.Left).Value - (int)Evaluate(subtr.Right).Value),
                DivExpr div => Res((int)Evaluate(div.Left).Value / (int)Evaluate(div.Right).Value),
                MulExpr mult => Res((int) Evaluate(mult.Left).Value * (int) Evaluate(mult.Right).Value),
                AndExpr and => Res((bool) Evaluate(and.Left).Value && (bool) Evaluate(and.Right).Value),
                OrExpr or => Res((bool)Evaluate(or.Left).Value || (bool)Evaluate(or.Right).Value),
                NotExpr not => Res(!(bool)Evaluate(not.Value).Value),
                EqualsExpr eq => Res(Evaluate(eq.Left).Value.Equals(Evaluate(eq.Right).Value)),
                IfExpr @if => @if.Then switch 
                {
                    ChooseExpr ch => (bool)Evaluate(@if.Condition).Value ? Evaluate(ch.Left) : Evaluate(ch.Right),
                    _ => throw new Exception("Expected ':' operator")
                },
                _ => throw new Exception("unsupported")

            };

        private static EvaluationResult Set(string name, object value) 
        {
            _vars[name] = value;
            return Empty;
        }
    }
}
