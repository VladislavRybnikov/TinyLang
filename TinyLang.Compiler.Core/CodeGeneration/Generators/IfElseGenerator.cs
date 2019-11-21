using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;

namespace TinyLang.Compiler.Core.CodeGeneration.Generators
{
    public class IfElseGenerator : CodeGenerator<IfElseExpr>
    {
        private readonly bool _isTernary;

        public IfElseGenerator(ICodeGeneratorsFactory factory, bool isTernary = false) : base(factory)
        {
            _isTernary = isTernary;
        }

        protected internal override CodeGenerationState GenerateInternal(IfElseExpr expression, CodeGenerationState state)
        {
            var il = state.State == CodeGenerationStates.Method
                ? state.MethodBuilder.GetILGenerator()
                : state.MainMethodBuilder.GetILGenerator();

            var elseLabel = il.DefineLabel();

            new[] {(expression.If.Predicate, expression.If.Scope)}
                .Append(expression.Elifs.Select(x => (x.Predicate, x.Scope)))
                .Select(x => (x.Predicate, x.Scope, Label: il.DefineLabel()))
                .Select(x => GenerateConditionalBranch(x.Predicate, x.Scope, state, il, x.Label)).ToList()
                .ForEach(x => x());

            if (expression.Else != null)
            {
                StartScope(expression.Else.Scope, elseLabel, il, state);
            }

            return state;
        }

        private Action GenerateConditionalBranch(Expr predicate, Scope scope, CodeGenerationState state, ILGenerator il, Label label)
        {
            var (type, emit) = LoadVar(predicate, il, state);

            if (type != typeof(bool))
                throw new Exception("Wrong predicate value");

            emit();
            il.Emit(OpCodes.Brtrue_S, label);

            return () => StartScope(scope, label, il, state);
        }

        private void StartScope(Scope scope, Label label, ILGenerator il, CodeGenerationState state)
        {
            il.MarkLabel(label);
            if (_isTernary)
            {
                LoadVar(scope.Statements.FirstOrDefault(), il, state);
            }
            else
            {
                LoadScope(scope, state);
            }
        }
    }
}
