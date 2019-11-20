using System;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.NumOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.BoolOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;
using System.Collections.Concurrent;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.CompareOperations;

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
                StrExpr str => Res(str.Value),
                VarExpr v => Res(_vars.TryGetValue(v.Name, out var val) ? val : throw new Exception($"'{v.Name}' is not defined")),
                AssignExpr a => a.Assigment switch 
                {
                    VarExpr av => Set(av.Name, Evaluate(a.Value).Value),
                    _ => throw new Exception("Opertaion '=' is not allowed here")
                },
                AddExpr add => Evaluate(add.Left).Value switch
                {
                    string s => Res($"{s}{Evaluate(add.Right).Value}"),
                     object obj => Res((int)obj + (int)Evaluate(add.Right).Value),
                },
                SubtrExpr subtr => Res((int) Evaluate(subtr.Left).Value - (int)Evaluate(subtr.Right).Value),
                DivExpr div => Res((int)Evaluate(div.Left).Value / (int)Evaluate(div.Right).Value),
                MulExpr mult => Res((int) Evaluate(mult.Left).Value * (int) Evaluate(mult.Right).Value),
                AndExpr and => Res((bool) Evaluate(and.Left).Value && (bool) Evaluate(and.Right).Value),
                OrExpr or => Res((bool)Evaluate(or.Left).Value || (bool)Evaluate(or.Right).Value),
                NotExpr not => Res(!(bool)Evaluate(not.Value).Value),
                EqExpr eq => Res(Evaluate(eq.Left).Value.Equals(Evaluate(eq.Right).Value)),
                NotEqExpr notEq => Res(!Evaluate(notEq.Left).Value.Equals(Evaluate(notEq.Right).Value)),
                LessExpr less => Compare(Evaluate(less.Left), Evaluate(less.Right), (l, r) => l < r),
                LessOrEqExpr lessOrEq => Compare(Evaluate(lessOrEq.Left), Evaluate(lessOrEq.Right), (l, r) => l <= r),
                MoreExpr more => Compare(Evaluate(more.Left), Evaluate(more.Right), (l, r) => l > r),
                MoreOrEqExpr moreOrEq => Compare(Evaluate(moreOrEq.Left), Evaluate(moreOrEq.Right), (l, r) => l >= r),
                TernaryIfExpr @if => @if.Then switch 
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

        private static EvaluationResult Compare(EvaluationResult a, EvaluationResult b, Func<int, int, bool> comparer)
        {
            if(a.Value is int aInt && b.Value is int bInt) 
            {
                return Res(comparer(aInt, bInt));
            }

            throw new Exception("Can not compare this objects");
        }
    }
}
