using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.NumOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.BoolOperations;

namespace TinyLang.Compiler.Core.Parsing.Expressions
{
    public abstract class Expr
    {
        public static Expr Bool(bool value) => new BoolExpr(value);
        public static Expr Int(int value) => new IntExpr(value);
        public static Expr Add(Expr right, Expr left) => new AddExpr(right, left);
        public static Expr Mul(Expr right, Expr left) => new MulExpr(right, left);
        public static Expr And(Expr right, Expr left) => new AndExpr(right, left);
    }
}
