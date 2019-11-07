using System;
using System.Collections.Generic;
using System.Linq;
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
    public class MathExpressionParserBuilder : ExpressionParserBuilder<int>
    {
        public const int PrefixOperatorPriority = 1;
        public const int MulDivOperatorPriority = 2;
        public const int AddSubOperatorPriority = 3;

        static Func<int, int> negate = x => -x;
        static Func<int, int> id = x => x;
        static Func<int, int> incr = x => ++x;
        static Func<int, int> decr = x => --x;
        static Func<int, int, int> mult = (x, y) => x * y;
        static Func<int, int, int> div = (x, y) => x / y;
        static Func<int, int, int> add = (x, y) => x + y;
        static Func<int, int, int> subtr = (x, y) => x - y;

        private MathExpressionParserBuilder() : base(TinyLanguage.TokenParser.Natural, TinyLanguage.TokenParser.ReservedOp)
        {
        }

        public static MathExpressionParserBuilder CreateWithBaseOperations()
        {
            return new MathExpressionParserBuilder()
                .WithUnaryOperation("-", UnaryOperationType.Prefix, negate, PrefixOperatorPriority)
                .WithUnaryOperation("+", UnaryOperationType.Prefix, id, PrefixOperatorPriority)
                .WithUnaryOperation("--", UnaryOperationType.Prefix, decr, PrefixOperatorPriority)
                .WithUnaryOperation("++", UnaryOperationType.Prefix, incr, PrefixOperatorPriority)
                .WithBinaryOperation("*", Assoc.Left, mult, MulDivOperatorPriority)
                .WithBinaryOperation("/", Assoc.Left, div, MulDivOperatorPriority)
                .WithBinaryOperation("+", Assoc.Left, add, AddSubOperatorPriority)
                .WithBinaryOperation("-", Assoc.Left, subtr, AddSubOperatorPriority) as MathExpressionParserBuilder;
        }
    }
}
