using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.CodeGeneration.Types;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;

namespace TinyLang.Compiler.Core.CodeGeneration
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

            args.ForEach(x => state.MethodArgs.Add(x.Name, x));

            var argsTypes = args.Select(a => a.Type).ToArray();

            state.WithGlobalMethod(expression.Name, retType, argsTypes);

            foreach (var s in expression.Body.Statements)
            {
                Factory.GeneratorFor(s.GetType()).Generate(s, state);
            }


            return state.EndGeneration();
        }

        public FuncDefinitionGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }
    }
}
