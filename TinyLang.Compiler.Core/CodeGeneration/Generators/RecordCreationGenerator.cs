using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;

namespace TinyLang.Compiler.Core.CodeGeneration.Generators
{
    public class RecordCreationGenerator : CodeGenerator<RecordCreationExpr>
    {
        public RecordCreationGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {

        }

        protected internal override CodeGenerationState GenerateInternal(RecordCreationExpr expression, CodeGenerationState state)
        {
            var ilGenerator = state.Scope == CodeGenerationScope.Method
                ? state.MethodBuilder.GetILGenerator() : state.MainMethodBuilder.GetILGenerator();

            RecordLoader(expression, ilGenerator, state).Load();

            return state;
        }
    }
}
