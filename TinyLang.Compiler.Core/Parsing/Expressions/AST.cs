using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TinyLang.Compiler.Core.Parsing.Expressions
{
    public sealed class AST : Expr, IEnumerable<Expr>
    {
        public Expr[] Expressions { get; }

        public int Length => Expressions.Length;

        public Expr Head => Expressions.FirstOrDefault();

        public Expr Tail => Expressions.LastOrDefault();

        public AST(params Expr[] args) => Expressions = args;

        public AST(IEnumerable<Expr> exprs) => Expressions = exprs.ToArray();

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

        public IEnumerator<Expr> GetEnumerator() => Expressions.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
