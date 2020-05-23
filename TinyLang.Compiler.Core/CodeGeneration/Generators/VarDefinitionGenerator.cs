using System;
using System.Reflection.Emit;
using TinyLang.Compiler.Core.Parsing.Expressions;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.Compiler.Core.CodeGeneration.Generators
{
    public class VarDefinitionGenerator : CodeGenerator<AssignExpr>
    {
        public VarDefinitionGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }

        protected internal override CodeGenerationState GenerateInternal(AssignExpr expression, CodeGenerationState state)
        {
            var ilGenerator = state.Scope switch
            {
                CodeGenerationScope.Method => state.MethodBuilder.GetILGenerator(),
                CodeGenerationScope.Module => state.MainMethodBuilder.GetILGenerator(),
                _ => throw new Exception()
            };

            // TODO: refactor to handle typed vars
            var name = (expression.Assigment as VarExpr).Name;

            if (TryGetVar(name, state, out var lb))
            {
                AssignVar(expression.Value, ilGenerator, state, lb);
            }
            else
            {
                lb = DefineVar(expression.Value, ilGenerator, state);
                state.AddVariable(name, lb);
            }

            return state;
        }

        private LocalBuilder DefineVar(Expr value, ILGenerator ilGenerator, CodeGenerationState state)
        {
            (Type type, Action emitLoad) = ValueLoader(value, ilGenerator, state);

            LocalBuilder lb = ilGenerator.DeclareLocal(type);
            
            emitLoad();
            ilGenerator.Emit(OpCodes.Stloc, lb);

            return lb;
        }

        private void AssignVar(Expr value, ILGenerator ilGenerator, CodeGenerationState state, LocalBuilder lb)
        {
            var (_, emitLoad) = ValueLoader(value, ilGenerator, state);

            emitLoad();
            ilGenerator.Emit(OpCodes.Stloc, lb);
        }

        private bool TryGetVar(string name, CodeGenerationState state, out LocalBuilder lb)
        {
            lb = null;
            if (name == "index" && state.Scope == CodeGenerationScope.Loop) 
            {
                lb = state.LoopIndex;
                return true;
            }

            var res = (state.MainVariables?.TryGetValue(name, out lb) ?? false)
                      || (state.MethodVariables?.TryGetValue(name, out lb) ?? false);

            return res;
        }
    }
}
