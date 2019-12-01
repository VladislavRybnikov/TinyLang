using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Types
{
    public class FuncTypeExpr : TypeExpr
    {
        public IEnumerable<TypeExpr> ArgsTypes { get; }
        public TypeExpr ReturnType { get; }

        public FuncTypeExpr(IEnumerable<TypeExpr> argsTypes, TypeExpr returnType)  
            : base(TypeName(argsTypes, returnType))
        {
            ArgsTypes = argsTypes;
            ReturnType = returnType;
        }

        private static string TypeName(IEnumerable<TypeExpr> argsTypes, TypeExpr returnType) 
            => $"Type(({string.Join(", ", argsTypes.Select(a => a.TypeName))})->" +
                $"{returnType.TypeName})";

        public override string ToString()
        {
            return TypeName(ArgsTypes, ReturnType);
        }
    }
}
