using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public static class TypesResolver
    {
        private static Dictionary<string, Type> _types = new Dictionary<string, Type>
        {
            { "str", typeof(string) },
            { "int", typeof(int) }
        };

        public static Type Resolve(string name, ModuleBuilder module)
            => _types.TryGetValue(name, out var val) ? val : module.GetType("name");
    }
}
