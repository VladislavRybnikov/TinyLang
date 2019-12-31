using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;

namespace TinyLang.Compiler.Core.Common.Exceptions.Base
{
    public class PositionedException : Exception
    {
        public Position Position { get; }

        public int Length { get; set; }

        public PositionedException(Position position, string message) : base(message) 
        {
            Position = position;
        }
    }
}
