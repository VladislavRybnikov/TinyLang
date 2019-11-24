using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.CodeGeneration.Types
{
    public class TypedLoader
    {
        public Type Type { get; }

        public Action Load { get; }

        public TypedLoader(Type type, Action load)
        {
            Type = type;
            Load = load;
        }

        public void Deconstruct(out Type type, out Action load) 
        {
            type = Type;
            load = Load;
        }

        public static implicit operator TypedLoader((Type type, Action load) tuple)
            => new TypedLoader(tuple.type, tuple.load);
    }
}
