using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.Common.Exceptions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;

namespace TinyLang.Compiler.Core.CodeGeneration.Generators
{
    public class ForGenerator : CodeGenerator<ForExpr>
    {
        public ForGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {

        }

        protected internal override CodeGenerationState GenerateInternal(ForExpr expression, CodeGenerationState state)
        {
            var il = state.Scope == CodeGenerationScope.Method
                ? state.MethodBuilder.GetILGenerator()
                : state.MainMethodBuilder.GetILGenerator();

            var endLabel = il.DefineLabel();
            var startLabel = il.DefineLabel();
            var checkLabel = il.DefineLabel();

            var (startType, startEmitLoad) = ValueLoader(expression.Start, il, state);
            var (endType, endEmitLoad) = ValueLoader(expression.End, il, state);
            var (stepType, stepEmitLoad) = ValueLoader(expression.Step, il, state);

            if (!(startType.Equals(typeof(int)) && endType.Equals(typeof(int)) && stepType.Equals(typeof(int)))) 
            {
                throw new ExprTypeCastException(startType, typeof(int), expression.Pos);
            }
            LocalBuilder lb = il.DeclareLocal(typeof(int));

            startEmitLoad();
            il.Emit(OpCodes.Stloc, lb);

            il.MarkLabel(checkLabel);
            il.Emit(OpCodes.Ldloc, lb);
            endEmitLoad();
            il.Emit(OpCodes.Clt);
            il.Emit(OpCodes.Brfalse, endLabel);

            il.MarkLabel(startLabel);
            state.StartLoop(lb);
            LoadScope(expression.Scope, state);
            il.Emit(OpCodes.Ldloc, lb);
            stepEmitLoad();
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc, lb);
            il.Emit(OpCodes.Br, checkLabel);

            il.MarkLabel(endLabel);
            il.Emit(OpCodes.Nop);
            state.EndLoop();

            return state;
        }
    }
}
