using System.Linq;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static TinyLang.Compiler.Core.TinyLanguage;
using static LanguageExt.Parsec.Char;

namespace TinyLang.Compiler.Core.Parsing.Parsers
{
    public static class RecordParsers
    {
        public static Parser<Expr> Records()
        {
            var propsParser = from x in TokenParser.ParensCommaSep(TypedVarParser)
                select x.AsEnumerable().Cast<TypedVar>();

            return from r in StrValue("record")
                from s in spaces
                from n in TokenParser.Identifier
                from p in propsParser
                select new Record(n, p) as Expr;
        }
    }
}
