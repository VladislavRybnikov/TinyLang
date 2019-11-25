using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Constructions
{
    public class PropExpr
    {
        public string Var;
        public string Prop;

        public PropExpr(string @var, string prop) 
        {
            Var = var;
            Prop = prop;
        }

        public override string ToString() 
        {
            return $"Var({Var}):Prop({Prop})";
        }
    }
}
