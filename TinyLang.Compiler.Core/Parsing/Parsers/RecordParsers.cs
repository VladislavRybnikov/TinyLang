using System.Linq;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static TinyLang.Compiler.Core.TinyLanguage;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Prim;

namespace TinyLang.Compiler.Core.Parsing.Parsers
{
    public static class RecordParsers
    {
        public static Parser<Expr> Records()
        {
            var propsParser = from x in TokenParser.ParensCommaSep(TypedVarParser)
                select x.AsEnumerable().Cast<TypedVar>();

            return from r in StrValue(ReservedNames.Record)
                from s in spaces
                from n in TokenParser.Identifier
                from p in propsParser
                select RecordExpr.Define(n, p);
        }

        public static Parser<Expr> RecordCreation(Parser<Expr> parser)
        {
            return from n in StrValue("new")
                from s in spaces
                from name in TokenParser.Identifier
                from props in TokenParser.ParensCommaSep(parser)
                where !LanguageDef.ReservedOpNames.Contains(name)
                select RecordExpr.New(name, props);
        }

        public static Parser<Expr> PropGetter(Parser<Expr> parser) 
        {
            return from e in choice(attempt(TokenParser.Parens(parser)), 
                   attempt(FuncParsers.FuncInvocation(parser)), VarParser)
                   from dot in TokenParser.Dot
                   from p in IdentifierParser
                   select new PropExpr(e, p) as Expr;
        }
    }
}
