using System;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.Parsing.Expressions;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Prim;
using System.Linq;
using static TinyLang.Compiler.Core.Parsing.Parsers.IfElseParsers;
using static TinyLang.Compiler.Core.Parsing.Parsers.WhileParsers;

namespace TinyLang.Compiler.Core.Parsing
{
    public interface ITokenizer<T>
    {
        Parser<T> Tokenize(IExpressionParserBuilder<T> expressionParserBuilder);
    }

    public class ExprTokenizer : ITokenizer<Expr>
    {
        public Parser<Expr> Tokenize(IExpressionParserBuilder<Expr> expressionParserBuilder)
        {
            var exprParser = expressionParserBuilder
                .WithBinaryOperation("*", Assoc.Left, Expr.Mul, 4)
                .WithBinaryOperation("+", Assoc.Left, Expr.Add, 3)
                .WithBinaryOperation("-", Assoc.Left, Expr.Subtr, 3)
                .WithBinaryOperation("/", Assoc.Left, Expr.Div, 4)
                .WithBinaryOperation("&&", Assoc.Left, Expr.And, 3)
                .WithBinaryOperation("=", Assoc.Left, Expr.Assign, 0)
                .WithBinaryOperation("==", Assoc.Left, Expr.Eq, 3)
                .WithBinaryOperation("!=", Assoc.Left, Expr.NotEq, 3)
                .WithBinaryOperation("<", Assoc.Left, Expr.Less, 3)
                .WithBinaryOperation("<=", Assoc.Left, Expr.LessOrEq, 3)
                .WithBinaryOperation(">", Assoc.Left, Expr.More, 3)
                .WithBinaryOperation(">=", Assoc.Left, Expr.MoreOrEq, 3)
                //.WithBinaryOperation("?", Assoc.Left, Expr.If, 1)
                //.WithBinaryOperation(":", Assoc.Left, Expr.Choose, 2)
                .WithUnaryOperation("!", UnaryOperationType.Prefix, Expr.Not, 2)
                .WithBinaryOperation("||", Assoc.Left, Expr.Or, 3)
                .Build();

            return Either(exprParser, IfElse, While, DoWhile);
        }

        private Parser<Expr> Either(Parser<Expr> parser, params Func<Parser<Expr>, Parser<Expr>>[] funcs)
        {
            var parsers = funcs.Select(f => f(attempt(lazyp(() => parser)))).ToArray();
            parser = either(attempt(parser), choice(parsers));

            return parser;
        }
    }
}
