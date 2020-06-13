using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TinyLang.Compiler.Core.CodeGeneration.Generators;
using TinyLang.Compiler.Core.Common.Attributes;
using TinyLang.Compiler.Core.Common.Exceptions;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public interface ICodeGeneratorsFactory
    {
        ICodeGenerator GeneratorFor(Type type, Type originType = null);
        ICodeGenerator GeneratorFor<T>() where T : Expr;
    }

    public class CodeGeneratorsFactory : ICodeGeneratorsFactory
    {
        private static CodeGeneratorsFactory _instance;

        private IDictionary<Type, ICodeGenerator> _genartors;

        private CodeGeneratorsFactory() { }

        public ICodeGenerator GeneratorFor(Type type, Type originType = null)
        {
            originType ??= type;

            if (_genartors.TryGetValue(type, out var val))
            {
                return val;
            }

            return type != typeof(object) ? GeneratorFor(type.BaseType, originType) 
                : throw new MissedGeneratorException(originType);
        }

        public ICodeGenerator GeneratorFor<T>() where T : Expr => GeneratorFor(typeof(T));

        public static ICodeGeneratorsFactory Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = new CodeGeneratorsFactory();

                _instance._genartors = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => typeof(ICodeGenerator).IsAssignableFrom(t) && !t.IsAbstract)
                    .SelectMany(t => GetGeneratorMappings(t))
                    .ToDictionary(key => key.Item1, value => value.Item2);

                return _instance;
            }
        }

        private static IEnumerable<(Type, ICodeGenerator)> GetGeneratorMappings(Type generatorType) 
        {
            if (generatorType.GetCustomAttributes<SkipGeneratorAttribute>().Any()) yield break; 

            var genericAttrType = generatorType.BaseType.GetGenericArguments()[0];

            yield return (genericAttrType, New(generatorType));

            foreach (var type in generatorType
                .GetCustomAttributes<ApplicableForAttribute>()
                .Select(attr => attr.Type)) 
            {
                if (type != genericAttrType) yield return (type, New(generatorType));
            }
        }

        private static ICodeGenerator New(Type generatorType) => 
            generatorType.GetConstructor(new Type[] { typeof(ICodeGeneratorsFactory) })
                    .Invoke(new object[] { _instance }) as ICodeGenerator;
    }

}
