using TinyLang.Compiler.Core.Parsing;
using TinyLang.Compiler.Core.Parsing.Expressions;
using static TinyLang.Compiler.Core.ExpressionEvaluator;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;

namespace TinyLang.Compiler.Core
{
    public static class TinyInteractive
    {
        public static EvaluationResult Execute(string line)
        {
            var tokenized = Parse(line);
            return Evaluate(tokenized);
        }

        public static Expr Parse(string line)
        {
            var exprParserBuilder = new ExpressionParserBuilder<Expr>(Predefined.ExprValueParser, TinyLanguage.TokenParser.ReservedOp);
            var tokenizer = new ExprTokenizer();

            var parser = new ExprParser(exprParserBuilder, tokenizer);
            return parser.ParseLine(line);
        }

        public static ExprParser Parser
        {
            get
            {
                var exprParserBuilder = new ExpressionParserBuilder<Expr>(Predefined.ExprValueParser, TinyLanguage.TokenParser.ReservedOp);
                var tokenizer = new ExprTokenizer();

                var parser = new ExprParser(exprParserBuilder, tokenizer);
                return parser;
            }
        }
    }
}
