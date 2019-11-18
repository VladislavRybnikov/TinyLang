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
        protected internal override CodeGenerationState GenerateInternal(AssignExpr expression, CodeGenerationState state)
        {
            var ilGenerator = state.State switch
            {
                CodeGenerationStates.Method => state.MethodBuilder.GetILGenerator(),
                CodeGenerationStates.Module => state.MainMethodBuilder.GetILGenerator(),
                _ => throw new Exception()
            };

            var lb = DefineVar(expression.Value, ilGenerator, state.ModuleBuilder);

            // TODO: refactor to handle typed vars
            state.MainVariables.Add((expression.Assigment as VarExpr).Name, lb);

            return state;
        }

        private LocalBuilder DefineVar(Expr value, ILGenerator ilGenerator, ModuleBuilder moduleBuilder)
        {

            (Type type, Action emitLoad) = LoadVar(value, ilGenerator, moduleBuilder);

            LocalBuilder lb = ilGenerator.DeclareLocal(type);
            
            emitLoad();
            ilGenerator.Emit(OpCodes.Stloc, lb);

            return lb;
        }

        private (Type type, Action emitLoad) LoadVar(Expr expr, ILGenerator ilGenerator, ModuleBuilder moduleBuilder) => expr switch
        {
            StrExpr str => (typeof(string), (Action)(() => ilGenerator.Emit(OpCodes.Ldc_I4, str.Value))),
            IntExpr @int => (typeof(int), () => ilGenerator.Emit(OpCodes.Ldc_I4, @int.Value)),
            BoolExpr @bool => (typeof(bool), () => ilGenerator.Emit(@bool.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0)),
            RecordCreation record => CreateRecord(record, ilGenerator, moduleBuilder),
            _ => throw new Exception("Unsupported variable type")
        };

        private (Type type, Action emitLoad) CreateRecord
            (RecordCreation expr, ILGenerator ilGenerator, ModuleBuilder moduleBuilder)
        {
            var type = moduleBuilder.GetType(expr.Name);
            var ctor = type.GetConstructors()[0];
            var ctorParams = ctor.GetParameters();

            var providedParams = expr.Props.ToArray();

            if (ctorParams.Length != providedParams.Length)
                throw new IndexOutOfRangeException("Wrong args");

            for (int i = 0; i < ctorParams.Length; i++) 
            {
                LoadVar(providedParams[i], ilGenerator, moduleBuilder).emitLoad();
            }

            ilGenerator.Emit(OpCodes.Newobj, ctor);

            return (type, () => ilGenerator.Emit(OpCodes.Newobj, ctor));
        }
    }
}
