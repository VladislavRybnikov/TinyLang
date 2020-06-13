using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Parsers.Abstract
{
    public interface IParserResolver
    {
        IEnumerable<IParser> ResolveParsers<TParser>() where TParser : IParser;
    }
}
