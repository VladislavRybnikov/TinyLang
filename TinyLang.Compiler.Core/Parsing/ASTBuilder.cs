using LanguageExt.Parsec;
using System;
using System.Collections.Generic;
using TinyLang.Compiler.Core.Parsing.Expressions;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using System.Linq;

namespace TinyLang.Compiler.Core.Parsing
{
    public interface IASTBuilder
    {
        AST FromStr(string str);
    }

    public class ASTBuilder : IASTBuilder
    {
        private Parser<Expr> _parser;

        public ASTBuilder(IParserBuilder<Expr> exprParserBuilder, ITokenizer<Expr> exprTokenizer)
        {
            _parser = exprTokenizer.Tokenize(exprParserBuilder);
        }

        public AST FromStr(string str)
        {
            var parser = from s in spaces from x in many1(_parser) 
                         from p in getPos select x.AsEnumerable()
                            .Select(x => x.WithPosition(p.Line, p.Column));

            return new AST(ParseWithExceptionThrow(parser, str));
        }

        private T ParseWithExceptionThrow<T>(Parser<T> parser, string line)
        {
            var parsed = parse(parser, line);
            return parsed.IsFaulted ? throw new Exception(parsed.Reply.Error.ToString()) : parsed.Reply.Result;
        }

    }
}
