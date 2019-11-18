using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.CodeGeneration;
using TinyLang.Compiler.Core.Parsing;
using TinyLang.Compiler.Core.Parsing.Expressions;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;

namespace TinyLang.Compiler.Core
{
    public interface ICompiler
    {

    }

    public class TinyCompiler : ICompiler
    {
        private TinyCompiler(IExprParser parser) { }

        public static ICompiler Create
        (
            Func<Parser<Expr>> singleValueParserFactory,
            ITokenizer<Expr> tokenizer
        )
        {
            return null;
        }
    }
}
