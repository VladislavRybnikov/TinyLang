using System.Linq;
using System.Reflection.Emit;
using TinyLang.Compiler.Core.CodeGeneration.Types;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;

namespace TinyLang.Compiler.Core.CodeGeneration.Generators
{
    public class FuncDefinitionGenerator : CodeGenerator<FuncExpr>
    {
        protected internal override CodeGenerationState GenerateInternal(FuncExpr expression, CodeGenerationState state)
        {
            var retExpr = expression.Body.Statements
                              .Select(x => x is RetExpr ret ? ret : null)
                              .FirstOrDefault(x => x != null)?.Expr;

            var retType = TypesResolver.ResolveFromExpr(retExpr, state.ModuleBuilder);

            var args = expression.Args.Select((x, i) => TypedArg.FromVar(x, state).WithEmitLoad(il => il.Emit(OpCodes.Ldarg_S, (sbyte)i))).ToList();
            var argsTypes = args.Select(a => a.Type).ToArray();

            state.WithGlobalMethod(expression.Name, retType, argsTypes);

            args.ForEach(x => state.MethodArgs.Add(x.Name, x));

            LoadScope(expression.Body, state);

            var il = state.MethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ret);

            return state.EndGeneration();
        }

        public FuncDefinitionGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }
    }
}
