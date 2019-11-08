using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.Parsing.Expressions;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Prelude;
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
                .WithBinaryOperation("*", Assoc.Left, Expr.Mul, 2)
                .WithBinaryOperation("+", Assoc.Left, Expr.Add, 1)
                .WithBinaryOperation("&&", Assoc.Left, Expr.And, 2)
                .Build();

            var parsed = parse(exprParser, Data);

            return  parsed.IsFaulted ? throw new Exception(parsed.Reply.Error.ToString()) : parsed.Reply.Result;
        }
    }
}
