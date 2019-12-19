using System.Linq;
using TinyLang.Compiler.Core.Parsing;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Fluent.Models;

namespace TinyLang.Fluent
{
    public class Statements
    {
        private readonly IASTBuilder builder;

        internal Statements(IASTBuilder builder) {
            this.builder = builder;
        }

        public IStatement New(string name, params string[] args) => new TypeCreation(name, args);

        public IStatement Invoke(string name, params string[] args) => new FuncCall(Parse, name, args);

        private Expr Parse(string str) => builder.FromStr(str).Build().Statements.FirstOrDefault();
    }
}
