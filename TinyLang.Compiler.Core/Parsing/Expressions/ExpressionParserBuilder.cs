using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Expr;

namespace TinyLang.Compiler.Core.Parsing.Expressions
{
    public enum UnaryOperationType { Prefix, Postfix }

    public delegate Operator<T> Binary<T>(string name, Func<T, T, T> f, Assoc assoc);

    public delegate Operator<T> Unary<T>(string name, Func<T, T> f);

    public class ExpressionParserBuilder<T>
    {
        private readonly Parser<T> _baseParser;

        private readonly Dictionary<int, List<Operator<T>>> _operators =
            new Dictionary<int, List<Operator<T>>>();

        private readonly Func<string, Parser<Unit>> _reservedOp;

        private Binary<T> binary => (name, f, assoc) =>
            Operator.Infix(assoc,
                from x in _reservedOp(name)
                select f);

        private Unary<T> prefix => (name, f) =>
            Operator.Prefix(from x in _reservedOp(name)
                select f);

        private Unary<T> postfix => (name, f) =>
            Operator.Postfix(from x in _reservedOp(name)
                select f);

        public ExpressionParserBuilder(Parser<T> baseParser, Func<string, Parser<Unit>> reservedOp)
        {
            _baseParser = baseParser;
            _reservedOp = reservedOp;
        }

        public ExpressionParserBuilder<T> WithBinaryOperation(string name, Assoc assoc, Func<T, T, T> operation, int priority)
        {
            if (_operators.TryGetValue(priority, out var list))
                list.Add(binary(name, operation, assoc));
            else
                _operators.Add(priority, new List<Operator<T>> {binary(name, operation, assoc)});

            return this;
        }

        public ExpressionParserBuilder<T> WithUnaryOperation(string name, UnaryOperationType type, Func<T, T> operation, int priority)
        {
            var @operator =
                (type switch { UnaryOperationType.Prefix => prefix, _ => postfix })(name,
                    operation);

            if (_operators.TryGetValue(priority, out var list))
                list.Add(@operator);
            else
                _operators.Add(priority, new List<Operator<T>> {@operator});

            return this;
        }

        public Parser<T> Build()
        {
            var table = _operators.Select(x => x.Value.ToArray()).ToArray();

            var natural = _baseParser;

            var expr = buildExpressionParser(table, natural).label("simple expression");

            var term = either(TinyLanguage.TokenParser.Parens(expr), natural).label("term");

            return buildExpressionParser(table, term).label("expression");
        }
    }
}
