using System;
using LanguageExt.Parsec;
using static LanguageExt.Parsec.Token;
using static TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using LanguageExt;
using TinyLang.Compiler.Core.Parsing;
using TinyLang.Compiler.Core.Parsing.Expressions;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;

namespace TinyLang.Compiler.Core
{
    public static class TinyLanguage
    {
        public static GenLanguageDef LanguageDef { get; }

        public static GenTokenParser TokenParser { get; }

        public static Parser<Expr> IntParser { get; }

        public static Parser<Expr> BoolParser { get; }

        public static Parser<Expr> StrParser { get; }

        public static Parser<Expr> VarParser { get; }

        public static Parser<Expr> ExprValueParser { get; }

        public static Parser<Expr> Scope(Parser<Expr> parser) =>
            from exprSet in TokenParser.Braces(many(parser))
            select new Scope(exprSet.ToList()) as Expr;

        static TinyLanguage()
        {
            LanguageDef = Language.JavaStyle.With(ReservedOpNames: new Lst<string>(new[] 
                { ReservedNames.If, ReservedNames.Elif, ReservedNames.Else })); 

            TokenParser = makeTokenParser(LanguageDef);
            IntParser = from n in TokenParser.Natural
                select Int(n);
            BoolParser = from w in asString(from w in many1(letter) from sc in many(symbolchar) from sp in spaces select w)
                         where string.Equals(w, "true", StringComparison.OrdinalIgnoreCase)
                               || string.Equals(w, "false", StringComparison.OrdinalIgnoreCase)
                         select Bool(bool.Parse(w));
            StrParser = from str in TokenParser.StringLiteral select Str(str);
            VarParser = from w in asString(from word in many1(letter)
                    from sp in spaces
                    select word)
                where !LanguageDef.ReservedOpNames.Contains(w)
                select Var(w);

            ExprValueParser = choice(attempt(BoolParser),
                attempt(IntParser), attempt(StrParser), VarParser);
        }
    }
}
