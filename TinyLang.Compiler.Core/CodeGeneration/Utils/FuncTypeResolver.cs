using System;
using System.Linq;

namespace TinyLang.Compiler.Core.CodeGeneration.Utils
{
    public static class FuncTypeResolver
    {
        private readonly static Type[] _funcTypes = new[]
            {
                typeof(Func<>),
                typeof(Func<,>),
                typeof(Func<,,>),
                typeof(Func<,,,>),
                typeof(Func<,,,,>),
                typeof(Func<,,,,,>),
                typeof(Func<,,,,,,>),
                typeof(Func<,,,,,,,>),
                typeof(Func<,,,,,,,,>),
                typeof(Func<,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,,,,>),
                typeof(Func<,,,,,,,,,,,,,,,,>)
            };

        public static Type Resolve(Type returnType, params Type[] argsTypes) 
        {
            var types = argsTypes.Append(returnType).ToArray();

            return _funcTypes[argsTypes.Length].MakeGenericType(types);
        }
    }
}
