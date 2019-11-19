using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public interface ICodeGenerator
    {
        CodeGenerationState Generate(Expr expression, CodeGenerationState state);
    }

    public interface ICodeGeneratorsFactory
    {
        ICodeGenerator GeneratorFor(Type type);
        ICodeGenerator GeneratorFor<T>() where T : Expr;
    }

    public class CodeGeneratorsFactory : ICodeGeneratorsFactory
    {
        private static CodeGeneratorsFactory _instance;

        private IDictionary<Type, ICodeGenerator> _genartors;

        private CodeGeneratorsFactory() { }

        public ICodeGenerator GeneratorFor(Type type) => _genartors[type];
        public ICodeGenerator GeneratorFor<T>() where T : Expr => _genartors[typeof(T)];

        public static ICodeGeneratorsFactory Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = new CodeGeneratorsFactory();
                _instance._genartors = new Dictionary<Type, ICodeGenerator>
                {
                    { typeof(AssignExpr), new VarDefinitionGenerator(_instance) },
                    { typeof(RecordExpr), new RecordDefinitionGenerator(_instance) },
                    { typeof(FuncInvocationExpr), new FuncCallGenerator(_instance) }
                };

                return _instance;
            }
        }
    }

    public abstract class CodeGenerator<TExpr> : ICodeGenerator where TExpr : Expr
    {
        protected readonly ICodeGeneratorsFactory Factory;
        protected CodeGenerator(ICodeGeneratorsFactory factory)
        {
            Factory = factory;
        }

        public CodeGenerationState Generate(Expr expression, CodeGenerationState state)
        {
            return GenerateInternal(expression as TExpr, state);
        }

        protected internal abstract CodeGenerationState GenerateInternal(TExpr expression, CodeGenerationState state);
    }
}
