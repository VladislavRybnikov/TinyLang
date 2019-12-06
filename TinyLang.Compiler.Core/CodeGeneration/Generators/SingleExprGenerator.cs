using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.CodeGeneration.Types;
using TinyLang.Compiler.Core.Parsing.Expressions;

namespace TinyLang.Compiler.Core.CodeGeneration.Generators
{
    public class SingleExprGenerator : CodeGenerator<Expr>
    {
        public SingleExprGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }

        protected internal override CodeGenerationState GenerateInternal(Expr expression, CodeGenerationState state)
        {
            TypedLoader.FromValue(expression, state.State == CodeGenerationStates.Method
                    ? state.MethodBuilder.GetILGenerator()
                    : state.MainMethodBuilder.GetILGenerator(),
                state,
                Factory).Load();

            return state;
        }
    }
}
