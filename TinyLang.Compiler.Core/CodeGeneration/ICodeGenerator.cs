using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;
using FuncInvocation = TinyLang.Compiler.Core.Parsing.Expressions.Constructions.FuncInvocation;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public interface ICodeGenerator
    {
        CodeGenerationState Generate(Expr expression, CodeGenerationState state);
    }

    public static class CodeGeneratorFactory 
    {
        private static IDictionary<Type, ICodeGenerator> _genartors = new Dictionary<Type, ICodeGenerator>
        {
            { typeof(AssignExpr), new VarDefinitionGenerator() },
            { typeof(Record), new RecordDefinitionGenerator() },
            { typeof(FuncInvocation), new MethodCallGenerator() }
        };

        public static ICodeGenerator For(Type type) => _genartors[type];
    }

    public abstract class CodeGenerator<TExpr> : ICodeGenerator where TExpr : Expr
    {
        public CodeGenerationState Generate(Expr expression, CodeGenerationState state)
        {
            return GenerateInternal(expression as TExpr, state);
        }

        protected internal abstract CodeGenerationState GenerateInternal(TExpr expression, CodeGenerationState state);
    }
}
