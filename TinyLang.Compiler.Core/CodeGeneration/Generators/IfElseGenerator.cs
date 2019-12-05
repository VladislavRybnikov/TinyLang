using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.Compiler.Core.CodeGeneration.Generators
{
    public class IfElseGenerator : CodeGenerator<IfElseExpr>
    {
        private bool _isTernary = false;

        public IfElseGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }

        protected internal override CodeGenerationState GenerateInternal(IfElseExpr expression, CodeGenerationState state)
        {
            var il = state.State == CodeGenerationStates.Method
                ? state.MethodBuilder.GetILGenerator()
                : state.MainMethodBuilder.GetILGenerator();

            var ifLabel = il.DefineLabel();
            var elseLabel = il.DefineLabel();
            var endLabel = il.DefineLabel();
            
            var labels = new []{ (expression.If.Predicate, expression.If.Scope, ifLabel) }
                .Append(expression.Elifs.Select(x => (x.Predicate, x.Scope, Label: il.DefineLabel()))).ToList(); // elif's labels

            if (expression.Else != null)
            {
                labels.Add((null, expression.Else.Scope, elseLabel));
            }

            labels.Add((null, null, endLabel));
            var labelsArr = labels.ToArray();

            LabelsInfo LabelsInfo(Label current, Label next)
                => new LabelsInfo { Current = current, Next = next, Final = endLabel };

            for (int i = 0; i < labelsArr.Length - 1; i++)
            {
                var (p, s, l) = labelsArr[i];
                var (_, _, nextL) = labelsArr[i + 1];

                var info = LabelsInfo(l, nextL);
                GenerateConditionalBranch(p, s, state, il, info);
            }

            il.MarkLabel(endLabel);
            il.Emit(OpCodes.Nop);

            return state;
        }

        protected override IfElseExpr Typed(Expr expr) 
        {
            return expr switch
            {
                IfElseExpr ifElse => ifElse,
                TernaryIfExpr ternaryIf => FromTernary(ternaryIf),
                _ => null
            };
        }

        private IfElseExpr FromTernary(TernaryIfExpr ternaryIf) 
        {
            _isTernary = true;
            return ternaryIf.ToIfElse();
        }

        private void GenerateConditionalBranch(Expr predicate, Scope scope, CodeGenerationState state, 
            ILGenerator il, LabelsInfo labels)
        {
            il.MarkLabel(labels.Current);
            if (predicate != null)
            {
                var (type, emit) = ValueLoader(predicate, il, state);

                if (type != typeof(bool))
                    throw new Exception("Wrong predicate value");

                emit();
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4_1);
            }

            il.Emit(OpCodes.Brfalse, labels.Next);

            StartScope(scope, il, state, labels.Final);
        }

        private void StartScope(Scope scope, ILGenerator il, CodeGenerationState state, Label final)
        {
            // TODO: refactor
            if (_isTernary)
            {
                ValueLoader(scope.Statements.FirstOrDefault(), il, state).Load();
            }
            else
            {
                LoadScope(scope, state);
            }
            il.Emit(OpCodes.Br, final);
        }

        private class LabelsInfo 
        {
            public Label Current { get; set; }

            public Label Next { get; set; }

            public Label Final { get; set; }
        }
    }
}
