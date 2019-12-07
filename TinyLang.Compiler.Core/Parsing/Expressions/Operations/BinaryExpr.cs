using Newtonsoft.Json;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Operations
{
    public abstract class BinaryExpr : Expr
    {
        [JsonProperty(Order = -1)]
        public abstract string Op { get; }

        public Expr Right { get; }
        public Expr Left { get; }

        public BinaryExpr(Expr left, Expr right)
        {
            Right = right;
            Left = left;
        }

        public override string ToString()
        {
            return $"{GetType().Name.Replace(nameof(Expr), string.Empty)}({Left}, {Right})";
        }
    }
}
