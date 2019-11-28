using LanguageExt;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static TinyLang.Compiler.Core.TinyLanguage;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;

namespace TinyLang.Compiler.Core.Parsing.Parsers
{
    public static class IfElseParsers
    {
        public static Parser<Expr> Ternary(Parser<Expr> parser) 
        {
            var chooseParser = from ifOp in TokenParser.Colon
                               from then in parser
                               from elseOp in StrValue("?")
                               from @else in parser
                               select new ChooseExpr(then, @else);

            return from predicate in parser
                   from ch in chooseParser
                   select new TernaryIfExpr(predicate, ch) as Expr;
        }
        public static Parser<Expr> IfElse(Parser<Expr> parser)
        {
            var ifParser = IfParser(parser);
            var elifParser = ElifParser(parser);
            var elseParser = ElseParser(parser);
            
            return from @if in ifParser
                from sp in many(space)
                from elifs in many(elifParser)
                from sp1 in many(space)
                from @else in optional(elseParser)
                select IfElse(@if, elifs, @else) as Expr;
        }
        public static Parser<Expr> ElseParser(Parser<Expr> parser)
        {
            return from s in StrValue(ReservedNames.Else)
                    from sp in many(space)
                   from scope in Scope(parser)
                   select new ElseExpr { Scope = scope as Scope } as Expr;
        }

        public static Parser<Expr> ElifParser(Parser<Expr> parser)
        {
            return from s in StrValue(ReservedNames.Elif)
                   from sp in many(space)
                   from expr in TokenParser.Parens(parser)
                   from sp1 in many(space)
                   from scope in Scope(parser)
                   select new ElifExpr(expr) { Scope = scope as Scope } as Expr;
        }

        public static Parser<Expr> IfParser(Parser<Expr> parser)
        {
            return from s in StrValue(ReservedNames.If)
                    from sp in many(space)
                   from expr in TokenParser.Parens(parser)
                    from sp1 in many(space)
                   from scope in Scope(parser)
                   select new IfExpr(expr) { Scope = scope as Scope } as Expr;

        }

        private static IfElseExpr IfElse(Expr @if, Seq<Expr> elifs, Option<Expr> @else) =>
            @else.Match(
                Some: some => new IfElseExpr(@if as IfExpr, elifs.Cast<ElifExpr>().AsEnumerable(), some as ElseExpr),
                None: () => new IfElseExpr(@if as IfExpr, elifs.Cast<ElifExpr>().AsEnumerable())
            );
    }
}
