using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
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

            var argsTypes = args.Select(a =>
            {
                var (type, emitLoad) = LoadVar(a, il, state);
                emitLoad();
                return type;
            }).ToList();

            il.EmitCall(OpCodes.Call, method, argsTypes.ToArray());

            return state;
        }

        public FuncCallGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }
    }
}
