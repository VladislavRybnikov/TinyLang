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
            var ilGenerator = state.State == CodeGenerationStates.Method
                ? state.MethodBuilder.GetILGenerator() : state.MainMethodBuilder.GetILGenerator();

            CreateRecord(expression, ilGenerator, state).emitLoad();

            return state;
        }
    }
}
