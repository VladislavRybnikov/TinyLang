using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return "AST {" +
                $"\n\tLength: {Length} " +
                $"\n\tHead: {Head}" +
                $"\n\tTail: {Tail}" +
                "\n}";
        }

        public IEnumerator<Expr> GetEnumerator() => Expressions.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
