using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Types
{
    public class ArrayExpr : Expr
    {
        public int Length { get; set; }

        public TypeExpr Type { get; set; }
    }
}
