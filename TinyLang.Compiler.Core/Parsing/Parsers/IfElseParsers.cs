﻿using LanguageExt;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static TinyLang.Compiler.Core.TinyLanguage;

namespace TinyLang.Compiler.Core.Parsing.Parsers
{
    public static class IfElseParsers
    {
        public static Parser<Expr> IfElse(Parser<Expr> parser)
        {
            var ifParser = IfParser(parser);
            var elifParser = ElifParser(parser);
            var elseParser = ElseParser(parser);
            
            return from @if in ifParser
                from elifs in many(elifParser)
                from @else in optional(elseParser)
                select IfElse(@if, elifs, @else) as Expr;
        }
        public static Parser<Expr> ElseParser(Parser<Expr> parser)
        {
            var elseStrParser = from str in asString(many1(letter)) where str == ReservedNames.Else select str;
            return from s in elseStrParser
                from scope in Scope(parser)
                   select new Else { Scope = scope as Scope } as Expr;
        }

        public static Parser<Expr> ElifParser(Parser<Expr> parser)
        {
            var elifStrParser = from str in asString(many1(letter)) where str == ReservedNames.Elif select str;
            return from s in elifStrParser
                from expr in TokenParser.Parens(parser)
                from scope in Scope(parser)
                   select new Elif(expr) { Scope = scope as Scope } as Expr;
        }

        public static Parser<Expr> IfParser(Parser<Expr> parser)
        {
            var ifStrParser = from str in asString(many1(letter)) where str == ReservedNames.If select str;
            return from s in ifStrParser
                from expr in TokenParser.Parens(parser)
                from scope in Scope(parser)
                   select new If(expr) { Scope = scope as Scope } as Expr;

        }

        private static IfElse IfElse(Expr @if, Seq<Expr> elifs, Option<Expr> @else) =>
            @else.Match(
                Some: some => new IfElse(@if as If, elifs.Cast<Elif>().AsEnumerable(), some as Else),
                None: () => new IfElse(@if as If, elifs.Cast<Elif>().AsEnumerable())
            );
    }
}
