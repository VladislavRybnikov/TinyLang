using Newtonsoft.Json;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Operations
{
    public class BinaryExpr : Expr
    {
        [JsonProperty(Order = -1)]
        public virtual string Op { get; set; }

        public Expr Right { get; set; }
        public Expr Left { get; set; }

        public BinaryExpr(Expr left, Expr right)
        {
            Right = right;
            Left = left;
        }

        public BinaryExpr()
        {
        }

        public override string ToString()
        {
            return $"{GetType().Name.Replace(nameof(Expr), string.Empty)}({Left}, {Right})";
        }
    }
}
