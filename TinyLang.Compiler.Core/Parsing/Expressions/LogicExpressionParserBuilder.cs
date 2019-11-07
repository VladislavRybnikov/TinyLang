using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Token;

namespace TinyLang.Compiler.Core.Parsing.Expressions
{
    class LogicExpressionParserBuilder : ExpressionParserBuilder<bool>
    {
        public const int UnaryPriority = 1;
        public const int BinaryPriority = 2;

        static readonly Parser<bool> boolParser = from w in asString(from w in many1(letter) from sp in spaces select w)
                                                  where string.Equals(w, "true", StringComparison.OrdinalIgnoreCase) 
                                                  || string.Equals(w, "false", StringComparison.OrdinalIgnoreCase)
                                                  select bool.Parse(w);

        static Func<bool, bool> not = x => !x;
        static Func<bool, bool, bool> and = (x, y) => x && y;
        static Func<bool, bool, bool> or = (x, y) => x || y;

        private LogicExpressionParserBuilder() : base(boolParser, TinyLanguage.TokenParser.ReservedOp)
        {
        }

        public static LogicExpressionParserBuilder CreateWithBaseOperations()
        {
            return new LogicExpressionParserBuilder()
                    .WithUnaryOperation("!", UnaryOperationType.Prefix, not, UnaryPriority)
                    .WithBinaryOperation("and", Assoc.Right, and, BinaryPriority)
                    .WithBinaryOperation("or", Assoc.Right, or, BinaryPriority)
                as LogicExpressionParserBuilder;
        }
    }
}
