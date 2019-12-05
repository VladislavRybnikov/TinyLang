using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TinyLang.Compiler.Core.CodeGeneration.Types;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;

namespace TinyLang.Compiler.Core.CodeGeneration.Generators
{
    public class FuncDefinitionGenerator : CodeGenerator<FuncExpr>
    {
        private bool _anonymous;

        protected internal override CodeGenerationState GenerateInternal(FuncExpr expression, CodeGenerationState state)
        {
            var args = expression.Args.Select((x, i) => TypedArg.FromVar(x, state, Factory).WithEmitLoad(il => il?.Emit(OpCodes.Ldarg_S, (sbyte)i))).ToList();
            var argsTypes = args.Select(a => a.Type).ToArray();

            var (name, attr) = GetMethodMetadata(expression.Name, state);

            state.WithGlobalMethod(name, argsTypes, attr);

            args.ForEach(x => state.MethodArgs.Add(x.Name, x));

            LoadScope(expression.Body, state);

            var retExpr = expression.Body.Statements
                              .Select(x => x is RetExpr ret ? ret : null)
                              .FirstOrDefault(x => x != null)?.Expr;

            var retType = TypesResolver.ResolveFromExpr(retExpr, state, Factory);

            state.MethodBuilder.SetReturnType(retType);
            var il = state.MethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ret);

            if (_anonymous)
            {
                state.AnonymousMethodsCache.SetMethod(name, state.MethodBuilder);
            }

            return state.EndGeneration();
        }

        public FuncDefinitionGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }

        protected override FuncExpr Typed(Expr expr)
        {
            return expr switch
            {
                FuncExpr func => func,
                LambdaExpr lambda => FromLambda(lambda),
                _ => null
            };
        }

        private FuncExpr FromLambda(LambdaExpr lambda)
        {
            _anonymous = true;
            return lambda.ToFunc();
        }

        private (string name, MethodAttributes attr) GetMethodMetadata(string originName, CodeGenerationState state)
        {
            return _anonymous
                ? (state.AnonymousMethodsCache.New(),
                    MethodAttributes.Final | MethodAttributes.Private | MethodAttributes.Static)
                : (originName, MethodAttributes.Final | MethodAttributes.Public | MethodAttributes.Static);
        }
    }
}
