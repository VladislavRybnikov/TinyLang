using LanguageExt.Parsec;
using System;
using System.Collections.Generic;
using System.Text;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Prim;
using static TinyLang.Compiler.Core.TinyLanguage;
using LanguageExt;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using TinyLang.Compiler.Core.Parsing.Parsers.Abstract;
using TinyLang.Compiler.Core.Common.Attributes;

namespace TinyLang.Compiler.Core.Parsing.Parsers
{
    [ParserOrder(2)]
    public class ForParser : IActionParser
    {
        public static Parser<Expr> For(Parser<Expr> parser)
        {
            return from f in StrValue("for")
                   from s in spaces
                   from args in TokenParser.Parens(ForArgs(parser))
                   from body in ScopeOrSingle(parser)
                   select ForExpr.Define(args.Start, args.End, args.Step, body as Scope);
        }

        private static Parser<(Expr Start, Expr End, Option<Expr> Step)> ForArgs(Parser<Expr> parser) 
        {
            return from start in parser
                   from s1 in spaces
                   from terminal1 in StrValue("to")
                   from s2 in spaces
                   from end in parser
                   from step in optional(Step(parser))
                   select (start, end, step);
        }

        private static Parser<Expr> Step(Parser<Expr> parser) 
        {
            return from terminal1 in StrValue("step")
                   from s in spaces
                   from step in parser
                   select step;
        }

        public Parser<Expr> Parse(Parser<Expr> parser) => For(parser);
    }
}
