using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Token;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;

namespace TinyLang.Compiler.Core.Parsing
{
    public static class Predefined
    {
        public static Parser<Expr> IntParser = from n in TinyLanguage.TokenParser.Natural
            select Expr.Int(n);

        public static Parser<Expr> BoolParser = from w in asString(from w in many1(letter) from sc in many(symbolchar) from sp in spaces select w)
                                                where string.Equals(w, "true", StringComparison.OrdinalIgnoreCase) 
                                                || string.Equals(w, "false", StringComparison.OrdinalIgnoreCase)
                                                select Expr.Bool(bool.Parse(w));

        public static Parser<Expr> VarParser = from w in asString(from w in many1(letter) from sp in spaces select w) select Expr.Var(w);

        public static Parser<Expr> ExprValueParser = choice(attempt(BoolParser), attempt(IntParser), VarParser);
    }
}
