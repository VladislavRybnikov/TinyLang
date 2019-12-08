using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Common.Exceptions.Base;
using TinyLang.Compiler.Core.Parsing.Expressions;

namespace TinyLang.Compiler.Core.Common.Exceptions
{
    public class NameResolveException : PositionedException
    {
        public string Name { get; }

        public NameResolveException(string name, Position position) 
            : base(position, ComposeMsg(name, position))
        {
            Name = name;
        }

        public static string ComposeMsg(string name, Position position) 
            => $"Can not resolve name '{name}' at : {position}.";
    }
}
