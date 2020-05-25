using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Common.Exceptions.Base;
using TinyLang.Compiler.Core.Parsing.Expressions;

namespace TinyLang.Compiler.Core.Common.Exceptions
{
    public class ExprTypeMismatchException : PositionedException
    {
        public Type Expected { get; }
        public Type Provided { get; }

        public ExprTypeMismatchException(Type expected, Type provided, Position pos) 
            : base(pos, ComposeMsg(expected, provided))
        {
            Expected = expected;
            Provided = provided;
        }

        private static string ComposeMsg(Type expected, Type provided) 
            => $"Expr type mismatch. Expected {expected}, but provided {provided}.";
    }
}
