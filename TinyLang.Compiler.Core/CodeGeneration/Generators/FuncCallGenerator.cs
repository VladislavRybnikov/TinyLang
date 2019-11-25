using System.Linq;
using System.Reflection.Emit;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;

namespace TinyLang.Compiler.Core.CodeGeneration.Generators
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
                var (type, emitLoad) = ValueLoader(a, il, state);
                emitLoad();
                if(type.IsValueType && expression.Name == "print" || expression.Name ==  "printF") 
                {
                    il.Emit(OpCodes.Box, type);
                }
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
