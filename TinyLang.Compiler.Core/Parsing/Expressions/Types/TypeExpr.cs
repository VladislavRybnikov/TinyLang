using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Types
{
    public class TypeExpr
    {
        public string TypeName { get; }

        public TypeExpr(string typeName)
        {
            TypeName = typeName;
        }

        public override string ToString()
        {
            return $"Type({TypeName})";
        }
    }

    public class TypedVar : Expr
    {
        public TypeExpr Type { get; }

        public GeneralOperations.VarExpr Var { get; }

        public TypedVar(GeneralOperations.VarExpr var, TypeExpr type)
        {
            Var = var;
            Type = type;
        }

        public TypedVar(GeneralOperations.VarExpr var, string typeName)
        {
            Var = var;
            Type = new TypeExpr(typeName);
        }

        public override string ToString()
        {
            return $"{Var}:{Type}";
        }
    }
}
