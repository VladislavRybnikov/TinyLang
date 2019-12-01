using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Constructions
{
    public class LambdaExpr : Expr
    {
        public IEnumerable<TypedVar> Args { get; }

        public Expr Expr { get; }

        public LambdaExpr(IEnumerable<TypedVar> args, Expr expr) 
        {
            Args = args;
            Expr = expr;
        }

        public override string ToString()
        {
            return $"Lamda({string.Join(", ", Args)}) => {Expr}";
        }
    }
}
