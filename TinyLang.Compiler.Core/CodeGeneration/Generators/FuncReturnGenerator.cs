using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;

namespace TinyLang.Compiler.Core.CodeGeneration.Generators
{
    public class FuncReturnGenerator : CodeGenerator<RetExpr>
    {
        public FuncReturnGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }

        protected internal override CodeGenerationState GenerateInternal(RetExpr expression, CodeGenerationState state)
        {
            if (expression.Expr is GeneralOperations.VarExpr v)
            {
                ValueLoader(v, state.MethodBuilder.GetILGenerator(), state).Load();
            }
            else
            {
                Factory.GeneratorFor(expression.Expr.GetType()).Generate(expression.Expr, state);
            }

            return state;
        }
    }
}
