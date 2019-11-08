using System;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.Parsing.Expressions;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Prim;

namespace TinyLang.Compiler.Core.Parsing
{
    public interface ITokenizer<T>
    {
        T Tokenize(IExpressionParserBuilder<T> expressionParserBuilder);
    }

    public class ExprTokenizer : ITokenizer<Expr>
    {
        public string Data { get; }

        public ExprTokenizer(string data)
        {
            Data = data;
        }

        public Expr Tokenize(IExpressionParserBuilder<Expr> expressionParserBuilder)
        {
            var exprParser = expressionParserBuilder
                .WithBinaryOperation("*", Assoc.Left, Expr.Mul, 4)
                .WithBinaryOperation("+", Assoc.Left, Expr.Add, 3)
                .WithBinaryOperation("-", Assoc.Left, Expr.Subtr, 3)
                .WithBinaryOperation("/", Assoc.Left, Expr.Div, 4)
                .WithBinaryOperation("&&", Assoc.Left, Expr.And, 4)
                .WithBinaryOperation("=", Assoc.Left, Expr.Assign, 0)
                .WithBinaryOperation("==", Assoc.Left, Expr.Eq, 3)
                .WithBinaryOperation("?", Assoc.Left, Expr.If, 1)
                .WithBinaryOperation(":", Assoc.Left, Expr.Choose, 2)
                .WithUnaryOperation("!", UnaryOperationType.Prefix, Expr.Not, 2)
                .WithBinaryOperation("||", Assoc.Left, Expr.Or, 4)
                .Build();

            var parsed = parse(exprParser, Data);

            return  parsed.IsFaulted ? throw new Exception(parsed.Reply.Error.ToString()) : parsed.Reply.Result;
        }
    }
}
