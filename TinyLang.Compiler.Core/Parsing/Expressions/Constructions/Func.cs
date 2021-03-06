﻿using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Constructions
{
    public class FuncExpr : Expr
    {
        public string Name { get; set; }

        public IEnumerable<TypedVar> Args { get; set; }

        public Scope Body { get; set; }

        public static Expr Define(string name, Option<TypeExpr> type, IEnumerable<TypedVar> args, Scope body) =>
            type.Match(
                some => new TypedFuncExpr(name, some, args, body),
                () => new FuncExpr(name, args, body)
            );

        public static Expr Invoke(string name, IEnumerable<Expr> args) => new FuncInvocationExpr(name, args);

        public static Expr Return(Expr expr) => new RetExpr(expr);

        public FuncExpr(string name, IEnumerable<TypedVar> args, Scope body)
        {
            Name = name;
            Args = args;
            Body = body;
        }

        public FuncExpr()
        {
        }

        public override string ToString()
        {
            return $"Func(\nName({Name}), \nArgs({string.Join(", ", Args)}), \nBody({Body}))";
        }
    }

    public class TypedFuncExpr : FuncExpr
    {
        public TypeExpr Type { get; set; }

        public TypedFuncExpr(string name, TypeExpr type, IEnumerable<TypedVar> args, Scope body) : base(name, args, body)
        {
            Type = type;
        }

        public TypedFuncExpr()
        {
        }

        public override string ToString()
        {
            return $"Func:{Type}(\nName({Name}), \nArgs({string.Join(", ", Args)}), \nBody({Body}))";
        }
    }

    public class FuncInvocationExpr : Expr
    {
        public string Name { get; set; }

        public IEnumerable<Expr> Args { get; set; }

        public FuncInvocationExpr(string name, IEnumerable<Expr> args)
        {
            Name = name;
            Args = args;
        }

        public override string ToString()
        {
            return $"Invoke(Name({Name}), Args({string.Join(", ", Args)}))";
        }
    }

    public class RetExpr : Expr
    {
        public Expr Expr { get; set; }

        public RetExpr(Expr expr)
        {
            Expr = expr;
        }

        public RetExpr()
        {
        }

        public override string ToString()
        {
            return $"Ret({Expr})";
        }
    }
}
