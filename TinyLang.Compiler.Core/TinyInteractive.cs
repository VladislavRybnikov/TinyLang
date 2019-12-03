using TinyLang.Compiler.Core.Parsing;
using TinyLang.Compiler.Core.Parsing.Expressions;
using static TinyLang.Compiler.Core.ExpressionEvaluator;
using static TinyLang.Compiler.Core.TinyLanguage;
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

        public static AST Parse(string line)
        {
            var exprParserBuilder = new ParserBuilder<Expr>(GetExprValueParser, TokenParser.ReservedOp);
            var tokenizer = new ExprTokenizer();

            var builder = new ASTBuilder(exprParserBuilder, tokenizer);
            return builder.FromStr(line);
        }

        public static ASTBuilder ASTBuilder
        {
            get
            {
                var exprParserBuilder = new ParserBuilder<Expr>(GetExprValueParser, TokenParser.ReservedOp);
                var tokenizer = new ExprTokenizer();

                var parser = new ASTBuilder(exprParserBuilder, tokenizer);
                return parser;
            }
        }
    }
}
