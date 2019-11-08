using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.Parsing;
using TinyLang.Compiler.Core.Parsing.Expressions;
using static LanguageExt.Prelude;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Expr;
using static LanguageExt.Parsec.Token;
using static TinyLang.Compiler.Core.ExpressionEvaluator;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;

namespace TinyLang.Compiler.Core
{
    public static class TinyInteractive
    {
        public static EvaluationResult Execute(string script)
        {
            var tokenized = Tokenize(script);
            return Evaluate(tokenized);
        }

        public static Expr Tokenize(string script)
        {
            var exprParser = new ExpressionParserBuilder<Expr>(Predefined.ExprValueParser, TinyLanguage.TokenParser.ReservedOp);

            var tokenizer = new ExprTokenizer(script);

            var expr = tokenizer.Tokenize(exprParser);
            return expr;
        }

    }
}
