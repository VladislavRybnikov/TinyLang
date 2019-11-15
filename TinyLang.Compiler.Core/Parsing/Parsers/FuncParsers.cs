﻿using System.Linq;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using static LanguageExt.Parsec.Char;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Prim;
using static TinyLang.Compiler.Core.TinyLanguage;
using Func = TinyLang.Compiler.Core.Parsing.Expressions.Constructions.Func;

namespace TinyLang.Compiler.Core.Parsing.Parsers
{
    public static class FuncParsers
    {
        public static Parser<Expr> FuncDefinition(Parser<Expr> parser)
        {
            var argsParser = from x in TokenParser.ParensCommaSep(TypedVarParser)
                select x.AsEnumerable().Cast<TypedVar>();

            return from f in StrValue("func")
                from s in spaces
                from name in TokenParser.Identifier
                from args in argsParser
                from type in optional(TypeAssignParser)
                from body in Scope(parser)
                where !LanguageDef.ReservedOpNames.Contains(name)
                select Func.Define(name, type, args, body as Scope);
        }

        public static Parser<Expr> FuncInvocation(Parser<Expr> parser)
        {
            return from name in TokenParser.Identifier
                from args in TokenParser.ParensCommaSep(parser)
                where !LanguageDef.ReservedOpNames.Contains(name)
                select Func.Invoke(name, args);
        }
    }
}
