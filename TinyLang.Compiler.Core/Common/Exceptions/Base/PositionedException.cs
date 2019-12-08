using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;

namespace TinyLang.Compiler.Core.Common.Exceptions.Base
{
    public abstract class PositionedException : Exception
    {
        public Position Position { get; }

        public PositionedException(Position position, string message) : base(message) 
        {
            Position = position;
        }
    }
}
