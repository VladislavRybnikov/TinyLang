﻿using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static TinyLang.Compiler.Core.TinyLanguage;
using TinyLang.Compiler.Core.Parsing.Parsers.Abstract;
using TinyLang.Compiler.Core.Common.Attributes;

namespace TinyLang.Compiler.Core.Parsing.Parsers
{
    [ParserOrder(3)]
    public class WhileParser : IActionParser
    {
        public Parser<Expr> Parse(Parser<Expr> parser) => WhileParsers.While(parser);
    }

    [ParserOrder(4)]
    public class DoWhileParser : IActionParser
    {
        public Parser<Expr> Parse(Parser<Expr> parser) => WhileParsers.DoWhile(parser);
    }

    public static class WhileParsers
    {
        public static Parser<Expr> While(Parser<Expr> parser)
        {
            return from s in StrValue(ReservedNames.While)
                from expr in TokenParser.Parens(parser)
                from scope in Scope(parser)
                select new WhileExpr(expr) { Scope = scope as Scope } as Expr;
        }

        public static Parser<Expr> DoWhile(Parser<Expr> parser)
        {
            return from d in StrValue(ReservedNames.Do)
                from scope in Scope(parser)
                from w in StrValue(ReservedNames.While)
                from expr in TokenParser.Parens(parser)
                select new DoWhileExpr(expr) { Scope = scope as Scope } as Expr;
        }
    }
}
