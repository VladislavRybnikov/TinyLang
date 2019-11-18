using System;
using System.Reflection.Emit;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public class VarDefinitionGenerator : CodeGenerator<AssignExpr>
    {
        protected internal override CodeGenarationState GenerateInternal(AssignExpr expression, CodeGenarationState state)
        {
            switch (state.State)
            {
                case CodeGenarationStates.Method:
                    {
                        var ilGenerator = state.MethodBuilder.GetILGenerator();

                        (Type type, Action emitLoad) = LoadVar(expression, ilGenerator);

                        var lb = ilGenerator.DeclareLocal(type);
                        emitLoad();
                        ilGenerator.Emit(OpCodes.Stloc, lb);

                        break;
                    }
            }

            return state;
        }

        private (Type type, Action emitLoad) LoadVar(AssignExpr expr, ILGenerator ilGenerator) => expr.Value switch
        {
            StrExpr str => (typeof(string), (Action)(() => ilGenerator.Emit(OpCodes.Ldc_I4, str.Value))),
            IntExpr @int => (typeof(int), () => ilGenerator.Emit(OpCodes.Ldc_I4, @int.Value)),
            BoolExpr @bool => (typeof(bool), () => ilGenerator.Emit(@bool.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0))
        };
    }
}
