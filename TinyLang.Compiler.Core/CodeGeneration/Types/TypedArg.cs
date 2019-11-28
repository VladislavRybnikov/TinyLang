using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;

namespace TinyLang.Compiler.Core.CodeGeneration.Types
{
    public class TypedArg
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public Action<ILGenerator> EmitLoad { get; set; }

        public static TypedArg FromVar(TypedVar var, CodeGenerationState state, ICodeGeneratorsFactory factory)
        {
            return new TypedArg
            {
                Name = var.Var.Name,
                Type = TypesResolver.ResolveFromExpr(var, state, factory)
            };
        }

        public TypedArg WithEmitLoad(Action<ILGenerator> action)
        {
            EmitLoad = action;
            return this;
        }
    }
}
