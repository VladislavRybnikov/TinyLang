using System.Linq;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using static LanguageExt.Parsec.Char;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;
using static LanguageExt.Parsec.Prim;
using static TinyLang.Compiler.Core.TinyLanguage;
using System.Collections.Generic;

namespace TinyLang.Compiler.Core.Parsing.Parsers
{
    public static class FuncParsers
    {
        public static Parser<TypeExpr> FuncType()
            => from args in TokenParser.ParensCommaSep(TokenParser.Identifier)
               from s1 in spaces
               from _ in chain(ch('-'), ch('>'))
               from s2 in spaces
               from ret in TokenParser.Identifier
               select new FuncTypeExpr(args.Select(x => new TypeExpr(x)), new TypeExpr(ret)) as TypeExpr;

        public static Parser<Expr> Lambda(Parser<Expr> parser)
        {
            return from args in Args()
                   from ret in FuncRetExpr(parser)
                   from p in getPos
                   select new LambdaExpr(args, ret).WithPosition(p.Line, p.Column);
        }

        public static Parser<Expr> FuncDefinition(Parser<Expr> parser)
        {
            var argsParser = from x in TokenParser.ParensCommaSep(TypedVarParser)
                             select x.AsEnumerable().Cast<TypedVar>();

            return from f in StrValue("func")
                   from s in spaces
                   from name in TokenParser.Identifier
                   from args in Args()
                   from type in optional(TypeAssignParser)
                   from body in either(attempt(FuncExprScope(parser)), FuncScope(parser))
                   where !LanguageDef.ReservedOpNames.Contains(name)
                   select FuncExpr.Define(name, type, args, body);
        }

        public static Parser<Expr> FuncInvocation(Parser<Expr> parser)
        {
            return from p in getPos
                   from name in TokenParser.Identifier
                   from args in TokenParser.ParensCommaSep(parser)
                   where !LanguageDef.ReservedOpNames.Contains(name)
                   select FuncExpr.Invoke(name, args).WithPosition(p);
        }

        private static Parser<IEnumerable<TypedVar>> Args() =>
            from x in TokenParser.ParensCommaSep(TypedVarParser)
            select x.AsEnumerable().Cast<TypedVar>();

        private static Parser<Expr> FuncRetExpr(Parser<Expr> parser)
            => from s1 in spaces
               from _ in chain(ch('='), ch('>'))
               from s2 in spaces
               from expr in parser
               select expr;

        private static Parser<Scope> FuncExprScope(Parser<Expr> parser)
            => from expr in FuncRetExpr(parser)
               select new Scope(new RetExpr(expr));

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
