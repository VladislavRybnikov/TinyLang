using System;
using System.Collections.Generic;
using System.Text;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Types
{
    public class FuncExpr : Expr
    {
        public FuncExpr(VarExpr args)
        {
            Args = args;
        }

        public VarExpr Args { get; }

        public Scope Scope { get; private set; }

        public FuncExpr WithBody(Scope bodyScope)
        {
            Scope = bodyScope;
            return this;
        }
    }
}
