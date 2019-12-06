using System;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;

namespace TinyLang.Compiler.Core.CodeGeneration.Generators
{
    public class FuncCallGenerator : CodeGenerator<FuncInvocationExpr>
    {
        protected internal override CodeGenerationState GenerateInternal(FuncInvocationExpr expression, CodeGenerationState state)
        {
            var il = state.State == CodeGenerationStates.Method ? state.MethodBuilder.GetILGenerator()
                : state.MainMethodBuilder.GetILGenerator();

            var args = expression.Args.ToArray();

            Action<Type[]> call;

            if (state.DefinedMethods.TryGetValue(expression.Name, out var m))
            {
                call = CreateMethodCall(m, il);
            }
            else if (state.TryResolveVariable(expression.Name, out var lb))
            {
                call = CreateDelegateCall(lb, il);
            }
            else
            {
                throw new Exception("Can not resolve func name");
            }

            var argsTypes = args.Select(a =>
            {
                var (type, emitLoad) = ValueLoader(a, il, state);
                emitLoad();
                if (type.IsValueType && expression.Name == "print" || expression.Name == "printF")
                {
                    il.Emit(OpCodes.Box, type);
                }
                return type;
            }).ToArray();

            call(argsTypes);

            return state;
        }

        public FuncCallGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }

        private Action<Type[]> CreateMethodCall(MethodBuilder method, ILGenerator il)
        {
            return argsTypes => il.EmitCall(OpCodes.Call, method, argsTypes);
        }

        private Action<Type[]> CreateDelegateCall(LocalBuilder lb, ILGenerator il)
        {
            var method = lb.LocalType?.GetMethod("Invoke");

            il.Emit(OpCodes.Ldloc, lb);
            return argsTypes => il.EmitCall(OpCodes.Callvirt, method, null);
        }
    }
}
