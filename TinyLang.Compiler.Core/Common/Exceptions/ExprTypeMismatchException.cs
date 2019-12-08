﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Common.Exceptions
{
    public class ExprTypeMismatchException : Exception
    {
        public Type Expected { get; }
        public Type Provided { get; }

        public ExprTypeMismatchException(Type expected, Type provided) 
            : base(ComposeMsg(expected, provided))
        {
            Expected = expected;
            Provided = provided;
        }

        private static string ComposeMsg(Type expected, Type provided) 
            => $"Expr type mismatch. Expected {expected}, but provided {provided}.";
    }
}
