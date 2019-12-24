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

        public IStatement Invoke(string name, params IStatement[] args) => new FuncCall(name, args);

        public IStatement Print(IStatement value) => Invoke("print", value);

        public IStatement Print(string value) => Invoke("print", Val(value));

        public IStatement Val(string str) => new Val(str);

        public IStatement Val(int i) => new Val(i);

        public IStatement Val(bool b) => new Val(b);


        private Expr Parse(string str) => builder.FromStr(str).Build().Statements.FirstOrDefault();
    }
}
