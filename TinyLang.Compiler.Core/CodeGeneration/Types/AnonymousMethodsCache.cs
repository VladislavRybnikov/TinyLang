using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace TinyLang.Compiler.Core.CodeGeneration.Types
{
    public class AnonymousMethodsCache
    {
        private Dictionary<string, MethodBuilder> _cache = new Dictionary<string, MethodBuilder>();

        public string New()
        {
            var name = $"TL$<>a:{_cache.Count}";
            _cache.Add(name, null);

            return name;
        }

        public void SetMethod(string name, MethodBuilder builder) => _cache[name] = builder;

        public bool TryGetMethod(string name, out MethodBuilder builder)
        {
            return _cache.TryGetValue(name, out builder);
        }

        public MethodBuilder Peek()
        {
            return _cache.Values.Last();
        }
    }
}

