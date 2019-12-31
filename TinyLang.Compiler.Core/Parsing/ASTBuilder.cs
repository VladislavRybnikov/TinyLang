using LanguageExt.Parsec;
using System;
using System.Collections.Generic;
using TinyLang.Compiler.Core.Parsing.Expressions;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using System.Linq;
using TinyLang.Compiler.Core.Common.Exceptions.Base;

namespace TinyLang.Compiler.Core.Parsing
{
    public interface IASTBuilder
    {
        IASTBuilder Empty();

        IASTBuilder FromStr(string str);

        IASTBuilder AddStatement(Expr e);

        AST Build();
    }

    public class ASTBuilder : IASTBuilder
    {
        private Parser<Expr> _parser;
        private AST _ast;

        public ASTBuilder(IParserBuilder<Expr> exprParserBuilder, ITokenizer<Expr> exprTokenizer)
        {
            _parser = exprTokenizer.Tokenize(exprParserBuilder);
        }

        public AST Build() => _ast;

        public IASTBuilder FromStr(string str)
        {
            var parser = 
                         from s in spaces from x in many1(_parser) 
                         select x.AsEnumerable();

            _ast = new AST(ParseWithExceptionThrow(parser, str));

            return this;
        }

        public IASTBuilder AddStatement(Expr e) 
        {
            _ast = _ast.Append(e).ToArray();

            return this;
        }

        private T ParseWithExceptionThrow<T>(Parser<T> parser, string line)
        {
            var parsed = parse(parser, line);
            var error = parsed.Reply.Error;

            if (error.Tag == ParserErrorTag.Message 
                || (error.Msg != "end of stream" && error.Tag == ParserErrorTag.Expect)) 
            {
                throw new PositionedException(new Position(error.Pos), "parser error");
            }

            return parsed.IsFaulted ? throw new PositionedException(new Position(error.Pos), "parser error")
                : parsed.Reply.Result;
        }

        public IASTBuilder Empty()
        {
            _ast = new AST();
            return this;
        }
    }
}
