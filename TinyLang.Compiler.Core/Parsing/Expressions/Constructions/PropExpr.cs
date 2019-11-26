using System;
using System.Collections.Generic;
using System.Text;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Constructions
{
    public class PropExpr : Expr
    {
        public Expr Expr;
        public string Prop;

        public PropExpr(Expr expr, string prop) 
        {
            Expr = expr;
            Prop = prop;
        }

        public override string ToString() 
        {
            return $"({Expr}):Prop({Prop})";
        }
    }
}
