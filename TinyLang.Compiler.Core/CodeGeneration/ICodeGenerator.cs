using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public interface ICodeGenerator
    {
        CodeGenarationState Generate(Expr expression, CodeGenarationState state);
    }

    public abstract class CodeGenerator<TExpr> : ICodeGenerator where TExpr : Expr
    {
        private static IDictionary<Type, ICodeGenerator> _genartors = new Dictionary<Type, ICodeGenerator>
        {
            { typeof(AssignExpr), new VarDefinitionGenerator() }
        };

        public static ICodeGenerator For<T>(T expr) where T : Expr => _genartors[typeof(T)];

        public CodeGenarationState Generate(Expr expression, CodeGenarationState state)
        {
            return GenerateInternal(expression as TExpr, state);
        }

        protected internal abstract CodeGenarationState GenerateInternal(TExpr expression, CodeGenarationState state);
    }
}
