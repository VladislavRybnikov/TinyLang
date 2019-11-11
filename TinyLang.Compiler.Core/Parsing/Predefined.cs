using System;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static TinyLang.Compiler.Core.Parsing.Expressions.Expr;

namespace TinyLang.Compiler.Core.Parsing
{
    public static class Predefined
    {
        public static Parser<Expr> IntParser = from n in TinyLanguage.TokenParser.Natural
            select Int(n);

        public static Parser<Expr> BoolParser = from w in asString(from w in many1(letter) from sc in many(symbolchar) from sp in spaces select w)
                                                where string.Equals(w, "true", StringComparison.OrdinalIgnoreCase) 
                                                || string.Equals(w, "false", StringComparison.OrdinalIgnoreCase)
                                                select Bool(bool.Parse(w));

        public static Parser<Expr> StrParser = from str in TinyLanguage.TokenParser.StringLiteral select Str(str);

        public static Parser<Expr> VarParser = from w in asString(from word in many1(letter)
                                                                  from sp in spaces select word)
                                               where !TinyLanguage.LanguageDef.ReservedOpNames.Contains(w)
                                               select Var(w);

        public static Parser<Expr> ExprValueParser = choice(attempt(BoolParser),
            attempt(IntParser), attempt(StrParser), VarParser);
    }
}
