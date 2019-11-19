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

        private (Type type, Action emitLoad) LoadVar(Expr expr, ILGenerator ilGenerator, CodeGenerationState state) => expr switch
        {
            VarExpr v => LoadFromMethodScope(v, ilGenerator, state),
            StrExpr str => (typeof(string), (Action)(() => ilGenerator.Emit(OpCodes.Ldstr, str.Value))),
            IntExpr @int => (typeof(int), () => ilGenerator.Emit(OpCodes.Ldc_I4, @int.Value)),
            BoolExpr @bool => (typeof(bool), () => ilGenerator.Emit(@bool.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0)),
            RecordCreationExpr record => CreateRecord(record, ilGenerator, state),
            _ => throw new Exception("Unsupported variable type")
        };

        private (Type type, Action emitLoad) LoadFromMethodScope(VarExpr expr, ILGenerator ilGenerator,
            CodeGenerationState state)
        {
            if (state.State != CodeGenerationStates.Method) 
                throw new Exception("Can not resolve variable");
            
            if (state.MethodArgs.TryGetValue(expr.Name, out var arg))
            {
                return (arg.Type, () => arg.EmitLoad(ilGenerator));
            }

            if (state.MethodVariables.TryGetValue(expr.Name, out var v))
            {
                return (v.LocalType, () => ilGenerator.Emit(OpCodes.Ldloc, v));
            }

            throw new Exception("Can not resolve variable");
        }

        private (Type type, Action emitLoad) CreateRecord
            (RecordCreationExpr expr, ILGenerator ilGenerator, CodeGenerationState state)
        {
            var type = state.ModuleBuilder.GetType(expr.Name);
            var ctor = type.GetConstructors()[0];
            var ctorParams = ctor.GetParameters();

            var providedParams = expr.Props.ToArray();

            if (ctorParams.Length != providedParams.Length)
                throw new IndexOutOfRangeException("Wrong args");

            for (int i = 0; i < ctorParams.Length; i++)
            {
                //ilGenerator.Emit(OpCodes.Stloc_0);
                //ilGenerator.Emit(OpCodes.Ldarg_0);
                LoadVar(providedParams[i], ilGenerator, state).emitLoad();
            }

            //ilGenerator.Emit(OpCodes.Newobj, ctor);

            return (type, () => ilGenerator.Emit(OpCodes.Newobj, ctor));
        }
    }
}
