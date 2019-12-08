using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Common.Exceptions.Base;
using TinyLang.Compiler.Core.Parsing.Expressions;

namespace TinyLang.Compiler.Core.Common.Exceptions
{
    public class ExprTypeCastException : PositionedException
    {
        public Type Expected { get; }
        public Type Provided { get; }

        public ExprTypeCastException(Type expected, Type provided, Position position) 
            : base(position, ComposeMsg(expected, provided, position))
        {
            Expected = expected;
            Provided = provided;
        }

        public static string ComposeMsg(Type expected, Type provided, Position position)
            => $"Can not cast expression of type {provided} to {expected} at {position}"; 
    }
}
