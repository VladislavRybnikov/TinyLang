using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TinyLang.Compiler.Core.Common.Attributes;
using TinyLang.Compiler.Core.Parsing.Parsers.Abstract;

namespace TinyLang.Compiler.Core.Parsing
{
    public class ParserResolver : IParserResolver
    {
        private static Lazy<IEnumerable<IParser>> _loaded 
            = new Lazy<IEnumerable<IParser>>(LoadParsers); 

        private static IEnumerable<IParser> LoadParsers() 
            => Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IParser).IsAssignableFrom(t) && !t.IsAbstract).OrderBy(t =>
                {
                    return t.GetCustomAttribute<ParserOrderAttribute>()?.Order ?? 100;
                }).Select(t => t.GetConstructor(new Type[] { })
                    .Invoke(new object[] { }) as IParser);

        public IEnumerable<IParser> ResolveParsers<TParser>() where TParser : IParser
        {
            return _loaded.Value.Where(p => p is TParser);
        }
    }
}
