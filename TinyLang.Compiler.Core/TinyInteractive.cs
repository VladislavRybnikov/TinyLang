using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.Parsing.Expressions;
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
            Func<int, int> fact = x => x == 0 ? 1 : Enumerable.Range(1, x).Aggregate((acc, i) => acc * i);

            var mathExprParser = MathExpressionParserBuilder
                .CreateWithBaseOperations()
                .WithUnaryOperation("!", UnaryOperationType.Postfix, fact, MathExpressionParserBuilder.PrefixOperatorPriority)
                .Build();

            var logicExprParser = LogicExpressionParserBuilder
                .CreateWithBaseOperations()
                .Build();

            var mathExprParserAsString = from i in mathExprParser select i.ToString();
            var logicExprParserAsString = from b in logicExprParser select b.ToString();

            var parser = either(mathExprParserAsString, logicExprParserAsString);

            //var parsed2 = parse(parser, script);

            var parsed = ParseEither(mathExprParserAsString, logicExprParserAsString, script);

            return parsed.ToOption().IfNone(() => throw new Exception());
        }

        private static ParserResult<T> ParseEither<T>(Parser<T> p1, Parser<T> p2, string str)
        {
            var parsedFirst = parse(p1, str);
            return parsedFirst.IsFaulted ? parse(p2, str) : parsedFirst;
        }
    }
}
