using LanguageExt.Parsec;
using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using System.Linq;

namespace TinyLang.Compiler.Core.Parsing
{
    public class ExprParser
    {
        private Parser<Expr> _parser;

        public ExprParser(IExpressionParserBuilder<Expr> exprParserBuilder, ITokenizer<Expr> exprTokenizer)
        {
            _parser = exprTokenizer.Tokenize(exprParserBuilder);
        }

        public IEnumerable<Expr> Parse(string str)
        {
            var parser = from x in many1(BracesParser()) select x.AsEnumerable();

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

        private Parser<Expr> BracesParser()
        {
            var scopeParser = ScopeParser();
            var ifParser = IfParser(scopeParser);
            var elifParser = ElifParser(scopeParser);
            var elseParser = ElseParser(scopeParser);

            var chainParser = chain(ifParser, attempt(elifParser), attempt(elseParser));

            return choice(attempt(_parser), attempt(ifParser), attempt(elifParser), attempt(elseParser));
        }

        private Parser<Expr> ScopeParser() 
        {
            return  from exprSet in TinyLanguage.TokenParser.Braces(many(_parser))
                              select new Scope(exprSet.ToList()) as Expr;

        }

        private Parser<Expr> ElseParser(Parser<Expr> scopeParser) 
        {
            var elifStrParser = from str in asString(many1(letter)) where str == "else" select str;
            return from s in elifStrParser
                   from scope in scopeParser
                   select new Else { Scope = scope as Scope } as Expr;
        }

        private Parser<Expr> ElifParser(Parser<Expr> scopeParser)
        {
            var elifStrParser = from str in asString(many1(letter)) where str == "elif" select str;
            return from s in elifStrParser
                   from expr in TinyLanguage.TokenParser.Parens(_parser)
                   from scope in scopeParser
                   select new Elif(expr) { Scope = scope as Scope } as Expr;
        }

        private Parser<Expr> IfParser(Parser<Expr> scopeParser) 
        { 
                var ifStrParser = from str in asString(many1(letter)) where str == "if" select str;
                return from s in ifStrParser
                               from expr in TinyLanguage.TokenParser.Parens(_parser)
                               from scope in scopeParser
                               select new If(expr) { Scope = scope as Scope } as Expr;
            
        } 
    }
}
