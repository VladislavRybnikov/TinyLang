using LanguageExt.Parsec;
using System;
using System.Collections.Generic;
using TinyLang.Compiler.Core.Parsing.Expressions;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Prim;
using System.Linq;
using static TinyLang.Compiler.Core.Parsing.Parsers.IfElseParsers;

namespace TinyLang.Compiler.Core.Parsing
{
    public class ExprParser
    {
        private Parser<Expr> _parser;

        public ExprParser(IExpressionParserBuilder<Expr> exprParserBuilder, ITokenizer<Expr> exprTokenizer)
        {
            _parser = exprTokenizer.Tokenize(exprParserBuilder);
            _parser = either(attempt(_parser), IfElse(lazyp(() => _parser)));
        }

        public IEnumerable<Expr> Parse(string str)
        {
            var parser = from x in many1(_parser) select x.AsEnumerable();

            return ParseWithExceptionThrow(parser, str);
        }

        public IEnumerable<Expr> Parse(IEnumerable<string> lines)
        {
            var enumerator = lines.SelectMany(ParseLineMany).GetEnumerator();

            return Parse(enumerator);
        }

        public IEnumerable<Expr> Parse(IEnumerator<Expr> enumerator)
        {
            var scope = new Scope();
            Expr prev = null;
            for (var expr = enumerator.Current; enumerator.MoveNext();)
            {
                switch (expr)
                {
                    case Scope.StartOfScope _:
                        {
                            if (prev is ScopedExpr scoped) scoped.Scope = ParseScope(enumerator);
                            else throw new Exception("Unexpected start of scope.");
                            break;
                        }
                    case Expr defaultExpr: scope.Statements.Add(defaultExpr); break;
                }
            }

            return scope.Statements;
        }

        Scope ParseScope(IEnumerator<Expr> enumerator)
        {
            var scope = new Scope();

            for (var expr = enumerator.Current; enumerator.MoveNext() ||
                !(expr is Scope.EndOfScope);)
            {
                scope.Statements.Add(expr);
            }

            return scope;
        }

        public Expr ParseLine(string line) => ParseWithExceptionThrow(_parser, line);

        private IEnumerable<Expr> ParseLineMany(string line)
        {
            var parser = from x in many1(_parser)
                         select x.AsEnumerable();

            return ParseWithExceptionThrow(parser, line);
        }

        private T ParseWithExceptionThrow<T>(Parser<T> parser, string line)
        {
            var parsed = parse(parser, line);
            return parsed.IsFaulted ? throw new Exception(parsed.Reply.Error.ToString()) : parsed.Reply.Result;
        }

    }
}
