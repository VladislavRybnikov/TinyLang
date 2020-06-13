using System;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.Parsing.Expressions;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Prim;
using System.Linq;
using static TinyLang.Compiler.Core.Parsing.Parsers.IfElseParsers;
using static TinyLang.Compiler.Core.Parsing.Parsers.WhileParsers;
using static TinyLang.Compiler.Core.Parsing.Parsers.RecordParsers;
using static TinyLang.Compiler.Core.Parsing.Parsers.FuncParsers;
using static TinyLang.Compiler.Core.Parsing.Parsers.ForParser;
using TinyLang.Compiler.Core.Parsing.Parsers.Abstract;

namespace TinyLang.Compiler.Core.Parsing
{
    public delegate Parser<Expr> ParserPipe(Parser<Expr> parser);

    public interface ITokenizer<T>
    {
        Parser<T> Tokenize(IParserBuilder<T> expressionParserBuilder);
    }

    public class ExprTokenizer : ITokenizer<Expr>
    {
        public Parser<Expr> Tokenize(IParserBuilder<Expr> expressionParserBuilder)
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
                .WithBinaryOperation("?", Assoc.Left, Expr.If, 1)
                .WithBinaryOperation(":", Assoc.Left, Expr.Choose, 2)
                .WithUnaryOperation("!", UnaryOperationType.Prefix, Expr.Not, 2)
                .WithBinaryOperation("||", Assoc.Left, Expr.Or, 3)
                .Build();

            //exprParser = either(attempt(FuncInvocation(lazyp(() => exprParser))), exprParser);

            var actionParsers = TinyLanguage.ParserResolver.ResolveParsers<IActionParser>()
                .Select<IParser, ParserPipe>(p => p.Parse)
                .ToArray();

            var parser = Compose(exprParser, actionParsers);

            parser = Add(parser, Records);

            return parser;
        }

        private Parser<Expr> Compose(Parser<Expr> parser, params ParserPipe[] funcs)
        {
            var parsers = funcs.Select(f => f(attempt(lazyp(() => parser)))).ToArray();
            parser = either(attempt(choice(parsers)), parser);

            return parser;
        }

        private Parser<Expr> Add(Parser<Expr> parser, Func<Parser<Expr>> f)
        {
            return either(attempt(parser), f());
        }
    }
}
