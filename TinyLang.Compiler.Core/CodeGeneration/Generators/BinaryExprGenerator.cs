using System;
using TinyLang.Compiler.Core.CodeGeneration.Types;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;

namespace TinyLang.Compiler.Core.CodeGeneration.Generators
{
    public class BinaryExprGenerator : CodeGenerator<BinaryExpr>
    {
        public BinaryExprGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }

        protected internal override CodeGenerationState GenerateInternal(BinaryExpr expression, CodeGenerationState state)
        {
            TypedLoader.FromBinaryExpr(expression,
                state.Scope == CodeGenerationScope.Method
                    ? state.MethodBuilder.GetILGenerator()
                    : state.MainMethodBuilder.GetILGenerator(),
                state,
                Factory).Load();

            return state;
        }
    }
}
