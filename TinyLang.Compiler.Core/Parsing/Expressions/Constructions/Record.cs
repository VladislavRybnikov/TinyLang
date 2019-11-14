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

        public override string ToString()
        {
            return $"Record(Name({Name}), Props({string.Join(", ", Props)}))";
        }
    }
}
