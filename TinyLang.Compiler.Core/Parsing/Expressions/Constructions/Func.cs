using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Constructions
{
    public class Func : Expr
    {
        public string Name { get; }

        public IEnumerable<TypedVar> Args { get; }

        public Scope Body { get; }

        public static Expr Define(string name, Option<TypeExpr> type, IEnumerable<TypedVar> args, Scope body) =>
            type.Match(
                some => new TypedFunc(name, some, args, body),
                () => new Func(name, args, body)
            );

        public static Expr Invoke(string name, IEnumerable<Expr> args) => new FuncInvocation(name, args);

        public Func(string name, IEnumerable<TypedVar> args, Scope body)
        {
            Name = name;
            Args = args;
            Body = body;
        }

        public override string ToString()
        {
            return $"Func(\nName({Name}), \nArgs({string.Join(", ", Args)}), \nBody({Body}))";
        }
    }

    public class TypedFunc : Func
    {
        public TypeExpr Type { get; }

        public TypedFunc(string name, TypeExpr type, IEnumerable<TypedVar> args, Scope body) : base(name, args, body)
        {
            Type = type;
        }

        public override string ToString()
        {
            return $"Func:{Type}(\nName({Name}), \nArgs({string.Join(", ", Args)}), \nBody({Body}))";
        }
    }

    public class FuncInvocation : Expr
    {
        public string Name { get; }

        public IEnumerable<Expr> Args { get; }

        public FuncInvocation(string name, IEnumerable<Expr> args)
        {
            Name = name;
            Args = args;
        }

        public override string ToString()
        {
            return $"Invoke(Name({Name}), Args({string.Join(", ", Args)}))";
        }
    }
}
