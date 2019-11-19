using System;
using System.Linq;
using System.Reflection.Emit;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public class VarDefinitionGenerator : CodeGenerator<AssignExpr>
    {
        public VarDefinitionGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }

        protected internal override CodeGenerationState GenerateInternal(AssignExpr expression, CodeGenerationState state)
        {
            var ilGenerator = state.State switch
            {
                CodeGenerationStates.Method => state.MethodBuilder.GetILGenerator(),
                CodeGenerationStates.Module => state.MainMethodBuilder.GetILGenerator(),
                _ => throw new Exception()
            };

            var lb = DefineVar(expression.Value, ilGenerator, state);

            // TODO: refactor to handle typed vars
            state.AddVariable((expression.Assigment as VarExpr).Name, lb);

            return state;
        }

        private LocalBuilder DefineVar(Expr value, ILGenerator ilGenerator, CodeGenerationState state)
        {

            (Type type, Action emitLoad) = LoadVar(value, ilGenerator, state);

            LocalBuilder lb = ilGenerator.DeclareLocal(type);
            
            emitLoad();
            ilGenerator.Emit(OpCodes.Stloc, lb);

            return lb;
        }
    }
}
