using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public class FuncCallGenerator : CodeGenerator<FuncInvocationExpr>
    {
        protected internal override CodeGenerationState GenerateInternal(FuncInvocationExpr expression, CodeGenerationState state)
        {
            var method = state.DefinedMethods[expression.Name];
            var args = expression.Args.ToArray();

            var il = state.MainMethodBuilder.GetILGenerator();

            var argsTypes = new List<Type>();

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i]) 
                {
                    case VarExpr v: 
                        {
                            var lb = state.MainVariables[v.Name];
                            il.Emit(OpCodes.Ldloc, lb);

                            argsTypes.Add(lb.LocalType);

                            break;
                        }
                }
            }

            il.EmitCall(OpCodes.Call, method, argsTypes.ToArray());

            return state;
        }

        public FuncCallGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }
    }
}
