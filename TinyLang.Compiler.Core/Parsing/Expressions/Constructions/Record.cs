using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Constructions
{
    public class Record : Expr
    {
        public string Name { get; }

        public IEnumerable<TypedVar> Props { get; }

        public Record(string name, IEnumerable<TypedVar> props)
        {
            Name = name;
            Props = props;
        }

        public static Expr Define(string name, IEnumerable<TypedVar> props) => new Record(name, props);

        public static Expr New(string name, IEnumerable<Expr> props) => new RecordCreation(name, props);

        public override string ToString()
        {
            return $"Record(Name({Name}), Props({string.Join(", ", Props)}))";
        }
    }

    public class RecordCreation : Expr
    {
        public string Name { get; }
        public IEnumerable<Expr> Props { get; }

        public RecordCreation(string name, IEnumerable<Expr> props)
        {
            Name = name;
            Props = props;
        }

        public override string ToString()
        {
            return $"New(Name({Name}), Props({string.Join(", ", Props)}))";
        }
    }
}
