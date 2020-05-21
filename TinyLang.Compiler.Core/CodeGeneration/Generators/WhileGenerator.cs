using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;

namespace TinyLang.Compiler.Core.CodeGeneration.Generators
{
    public class WhileGenerator : CodeGenerator<WhileExpr>
    {
        public WhileGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }

        protected internal override CodeGenerationState GenerateInternal(WhileExpr expression, CodeGenerationState state)
        {
            throw new System.NotImplementedException();
        }
    }
}
