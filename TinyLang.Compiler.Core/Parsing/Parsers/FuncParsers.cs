using System.Linq;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using static LanguageExt.Parsec.Char;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Prim;
using static TinyLang.Compiler.Core.TinyLanguage;

namespace TinyLang.Compiler.Core.Parsing.Parsers
{
    public static class FuncParsers
    {
        public static Parser<Expr> FuncDefinition(Parser<Expr> parser)
        {
            var argsParser = from x in TokenParser.ParensCommaSep(TypedVarParser)
                select x.AsEnumerable().Cast<TypedVar>();

            return from f in StrValue("func")
                from s in spaces
                from name in TokenParser.Identifier
                from args in argsParser
                from type in optional(TypeAssignParser)
                from body in FuncScope(parser)
                where !LanguageDef.ReservedOpNames.Contains(name)
                select FuncExpr.Define(name, type, args, body);
        }

        public static Parser<Expr> FuncInvocation(Parser<Expr> parser)
        {
            return from name in TokenParser.Identifier
                from args in TokenParser.ParensCommaSep(parser)
                where !LanguageDef.ReservedOpNames.Contains(name)
                select FuncExpr.Invoke(name, args);
        }

        private static Parser<Scope> FuncScope(Parser<Expr> parser)
        {
            var ret = from _ in StrValue(ReservedNames.Return)
                from s in spaces
                from expr in parser
                select FuncExpr.Return(expr);

            var statements = from e in many(parser)
                             from r in optional(ret)
                             select e.Append(r.AsEnumerable());

            return from exprSet in TokenParser.Braces(statements) select new Scope(exprSet.ToList());
        }
    }
}
