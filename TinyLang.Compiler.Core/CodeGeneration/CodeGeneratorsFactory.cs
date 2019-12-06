using System;
using System.Collections.Generic;
using TinyLang.Compiler.Core.CodeGeneration.Generators;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.Compiler.Core.CodeGeneration
{
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

        public ICodeGenerator GeneratorFor(Type type)
        {
            if (_genartors.TryGetValue(type, out var val))
            {
                return val;
            }

            return type != typeof(object) ? GeneratorFor(type.BaseType) : throw new Exception("Can not resolve Generator");
        }

        public ICodeGenerator GeneratorFor<T>() where T : Expr => GeneratorFor(typeof(T));

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
                    { typeof(FuncInvocationExpr), new FuncCallGenerator(_instance) },
                    { typeof(FuncExpr), new FuncDefinitionGenerator(_instance) },
                    { typeof(RetExpr), new FuncReturnGenerator(_instance) },
                    { typeof(RecordCreationExpr), new RecordCreationGenerator(_instance) },
                    { typeof(IfElseExpr), new IfElseGenerator(_instance) },
                    { typeof(TernaryIfExpr), new IfElseGenerator(_instance) },
                    { typeof(LambdaExpr), new FuncDefinitionGenerator(_instance) },
                    { typeof(Expr), new SingleExprGenerator(_instance) }
                };

                return _instance;
            }
        }
    }

}
