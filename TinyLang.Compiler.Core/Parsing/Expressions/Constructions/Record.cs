﻿using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Constructions
{
    public class RecordExpr : Expr
    {
        public string Name { get; set; }

        public IEnumerable<TypedVar> Props { get; set; }

        public RecordExpr(string name, IEnumerable<TypedVar> props)
        {
            Name = name;
            Props = props;
        }

        public RecordExpr()
        {
        }

        public static Expr Define(string name, IEnumerable<TypedVar> props) => new RecordExpr(name, props);

        public static Expr New(string name, IEnumerable<Expr> props) => new RecordCreationExpr(name, props);

        public override string ToString()
        {
            return $"Record(Name({Name}), Props({string.Join(", ", Props)}))";
        }
    }

    public class RecordCreationExpr : Expr
    {
        public string Name { get; set; }
        public IEnumerable<Expr> Props { get; set; }

        public RecordCreationExpr(string name, IEnumerable<Expr> props)
        {
            Name = name;
            Props = props;
        }

        public RecordCreationExpr()
        {
        }

        public override string ToString()
        {
            return $"New(Name({Name}), Props({string.Join(", ", Props)}))";
        }
    }
}
