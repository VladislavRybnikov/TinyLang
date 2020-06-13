using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Types
{
    public class FuncTypeExpr : TypeExpr
    {
        public IEnumerable<TypeExpr> ArgsTypes { get; set; }
        public TypeExpr ReturnType { get; set; }

        public FuncTypeExpr(IEnumerable<TypeExpr> argsTypes, TypeExpr returnType)  
            : base(TypeName(argsTypes, returnType))
        {
            ArgsTypes = argsTypes;
            ReturnType = returnType;
        }

        public FuncTypeExpr()
        {
        }

        private static string TypeName(IEnumerable<TypeExpr> argsTypes, TypeExpr returnType) 
            => $"Type(({string.Join(", ", argsTypes.Select(a => a.Name))})->" +
                $"{returnType.Name})";

        public override string ToString()
        {
            return TypeName(ArgsTypes, ReturnType);
        }
    }
}
