using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Types
{
    public class TypeExpr : Expr
    {
        public string Name { get; set; }

        public TypeExpr(string typeName)
        {
            Name = typeName;
        }

        public TypeExpr()
        {
        }

        public override string ToString()
        {
            return $"Type({Name})";
        }
    }

    public class TypedVar : Expr
    {
        public TypeExpr Type { get; set; }

        public GeneralOperations.VarExpr Var { get; set; }

        public TypedVar(string name, string type) : this(new GeneralOperations.VarExpr(name), type)
        {

        }

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

        public TypedVar()
        {
        }

        public override string ToString()
        {
            return $"{Var}:{Type}";
        }
    }
}
