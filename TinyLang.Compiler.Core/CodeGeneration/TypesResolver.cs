using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public static class TypesResolver
    {
        private static Dictionary<string, Type> _types = new Dictionary<string, Type>
        {
            { "str", typeof(string) },
            { "int", typeof(int) }
        };

        public static Type Resolve(string name, ModuleBuilder module)
            => _types.TryGetValue(name, out var val) ? val : module.GetType(name);

        public static Type ResolveFromExpr(Expr expr, ModuleBuilder module) =>
            expr switch
            {
                TypedVar t => Resolve(t.Type.TypeName, module),
                StrExpr _ => typeof(string),
                IntExpr _ => typeof(int),
                BoolExpr _ => typeof(bool),
                GeneralOperations.AssignExpr a => ResolveFromExpr(a.Value, module),
                RecordCreationExpr r => Resolve(r.Name, module),
                _ => typeof(object)
            };
    }
}
