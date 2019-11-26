using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

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

        public static Type ResolveFromExpr(Expr expr, CodeGenerationState state) =>
            expr switch
            {
                VarExpr v => state.ResolveVariable(v.Name).LocalType,
                TypedVar t => Resolve(t.Type.TypeName, state.ModuleBuilder),
                StrExpr _ => typeof(string),
                IntExpr _ => typeof(int),
                BoolExpr _ => typeof(bool),
                AssignExpr a => ResolveFromExpr(a.Value, state),
                RecordCreationExpr r => Resolve(r.Name, state.ModuleBuilder),
                _ => typeof(object)
            };
    }
}
