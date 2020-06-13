using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TinyLang.Compiler.Core.Parsing.Expressions
{
    [JsonObject]
    public sealed class AST : Expr, IEnumerable<Expr>
    {
        [JsonProperty(Order = 2)]
        public Expr[] Statements { get; }

        public override string NodeType => null;

        public int Length => Statements.Length;

        public AST(params Expr[] args) => Statements = args;

        public AST(IEnumerable<Expr> exprs) => Statements = exprs.ToArray();

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, settings: new JsonSerializerSettings 
            {
                Formatting = Formatting.Indented,
                StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public IEnumerator<Expr> GetEnumerator() => Statements.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator AST(Expr[] exprs) => new AST(exprs);
    }
}
