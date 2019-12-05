using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.CodeGeneration.Types;
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
            { "int", typeof(int) },
            { "bool", typeof(bool) }
        };

        public static Type Resolve(string name, ModuleBuilder module)
            => _types.TryGetValue(name, out var val) ? val : module.GetType(name);

        public static Type Resolve(TypeExpr expr, ModuleBuilder module) =>
            expr switch
            {
                FuncTypeExpr f => ResolveFuncType(f, module),
                _  => Resolve(expr.TypeName, module)
            };

        public static Type ResolveFromExpr(Expr expr, CodeGenerationState state, ICodeGeneratorsFactory factory) =>
            expr switch
            {
                VarExpr v => state.ResolveVariable(v.Name).LocalType,
                TypedVar t => Resolve(t.Type, state.ModuleBuilder),
                StrExpr _ => typeof(string),
                IntExpr _ => typeof(int),
                BoolExpr _ => typeof(bool),
                AssignExpr a => ResolveFromExpr(a.Value, state, factory),
                RecordCreationExpr r => Resolve(r.Name, state.ModuleBuilder),
                FuncInvocationExpr f => state.ResolveMethod(f.Name).ReturnType,
                Expr e => TypedLoader.FromValue(e, null, state, factory).Type
            };

        private static Type ResolveFuncType(FuncTypeExpr f, ModuleBuilder module)
        {
            var argsTypes = f.ArgsTypes.Select(x => Resolve(x, module))
                .Append(Resolve(f.ReturnType, module)).ToArray();

            return typeof(Func<>).MakeGenericType(argsTypes);
        }
    }
}
