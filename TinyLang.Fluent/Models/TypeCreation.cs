using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;

namespace TinyLang.Fluent.Models
{
    public class TypeCreation : IStatement
    {
        public TypeCreation(string name, params IStatement[] args) 
        {
        }
        public TypeCreation(string name, params string[] args)
        {
        }

        Expr IStatement.GetExpr()
        {
            throw new NotImplementedException();
        }
    }
}
