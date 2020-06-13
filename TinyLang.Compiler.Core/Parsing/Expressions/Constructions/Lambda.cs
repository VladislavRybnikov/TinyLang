using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Constructions
{
    public class LambdaExpr : Expr
    {
        public IEnumerable<TypedVar> Args { get; }

        public Expr Expr { get; set; }

        public LambdaExpr(IEnumerable<TypedVar> args, Expr expr) 
        {
            Args = args;
            Expr = expr;
        }

        public LambdaExpr()
        {
        }

        public FuncExpr ToFunc()
        {
            var scope = new Scope(new RetExpr(Expr));

            return new FuncExpr(null, Args, scope);
        }

        public override string ToString()
        {
            return $"Lambda({string.Join(", ", Args)}) => {Expr}";
        }
    }
}
