using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Token;

namespace TinyLang.Compiler.Core
{
    public static class TinyInteractive
    {
        public static string Execute(string script)
        {
            // TODO: implement other expression parsers
            var mathExprParser = MathExpressionParser();

            return parse(mathExprParser, script).ToOption().IfNone(() => throw new Exception()).ToString();
        }

        private static Parser<int> MathExpressionParser()
        {
            Parser<int> expr = from d in asString(many1(digit))
                               from s in many(satisfy(char.IsWhiteSpace))
                               select int.Parse(d);

            expr = either(expr, from s in many(symbolchar) select 0);

            var tinyLanguage = Language.JavaStyle;

            tinyLanguage.ReservedNames.AddRange(
                Seq("print", "input", "function", "type", "record", "data"));

            var tokenParser = makeTokenParser(tinyLanguage);

            var reservedOp = tokenParser.ReservedOp;

            var binary = fun((string name, Func<int, int, int> f, Assoc assoc) =>
                     Operator.Infix<int>(assoc,
                                          from x in reservedOp(name)
                                          select f));

            var prefix = fun((string name, Func<int, int> f) =>
                     Operator.Prefix<int>(from x in reservedOp(name)
                                          select f));

            var postfix = fun((string name, Func<int, int> f) =>
                     Operator.Postfix<int>(from x in reservedOp(name)
                                           select f));

            Func<int, int> negate = x => -x;
            Func<int, int> id = x => x;
            Func<int, int> incr = x => ++x;
            Func<int, int> decr = x => --x;
            Func<int, int> fact = x => x == 0 ? 1 : Enumerable.Range(1, x).Aggregate((acc, i) => acc * i);
            Func<int, int, int> mult = (x, y) => x * y;
            Func<int, int, int> div = (x, y) => x / y;
            Func<int, int, int> add = (x, y) => x + y;
            Func<int, int, int> subtr = (x, y) => x - y;

            var table = new[]
            {
                new[] { prefix("-",negate), prefix("+",id), prefix("--", decr), prefix("++", incr) },
                new[] { postfix("!", fact)},
                new[] { binary("*", mult, Assoc.Left), binary("/", div, Assoc.Left) },
                new[]{ binary("+", add, Assoc.Left), binary("-", subtr, Assoc.Left) }
            };

            var natural = tokenParser.Natural;

            expr = buildExpressionParser(table, natural).label("simple expression");

            var term = either(tokenParser.Parens(expr), natural).label("term");

            return buildExpressionParser(table, term).label("expression");
        }
    }
}
