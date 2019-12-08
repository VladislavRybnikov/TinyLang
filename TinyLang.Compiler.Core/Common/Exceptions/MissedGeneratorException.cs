using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Common.Exceptions
{
    public class MissedGeneratorException : Exception
    {
        public Type TypeFor { get; }

        public MissedGeneratorException(Type forType) : base(ComposeMsg(forType))
        {
            TypeFor = forType;
        }

        public static string ComposeMsg(Type type) => $"Missed generator for type {type}.";
    }
}
