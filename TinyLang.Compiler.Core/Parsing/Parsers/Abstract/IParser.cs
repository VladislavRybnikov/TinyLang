using LanguageExt.Parsec;
using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Parsers.Abstract
{
    public interface IParser
    {
        public string Name => GetType().Name;

        Parser<Expressions.Expr> Parse(Parser<Expressions.Expr> parser);
    }
}
