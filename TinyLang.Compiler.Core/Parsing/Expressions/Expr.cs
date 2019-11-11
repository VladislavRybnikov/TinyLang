using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.NumOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.BoolOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.CompareOperations;

namespace TinyLang.Compiler.Core.Parsing.Expressions
{
    public abstract class Expr
    {
        public static Expr Bool(bool value) => new BoolExpr(value);
        public static Expr Int(int value) => new IntExpr(value);
        public static Expr Str(string value) => new StrExpr(value);
        public static Expr Add(Expr left, Expr right) => new AddExpr(left, right);
        public static Expr Div(Expr left, Expr right) => new DivExpr(left, right);
        public static Expr Subtr(Expr left, Expr right) => new SubtrExpr(left, left);
        public static Expr Mul(Expr left, Expr right) => new MulExpr(left, right);
        public static Expr And(Expr left, Expr right) => new AndExpr(left, right);
        public static Expr Or(Expr left, Expr right) => new OrExpr(left, right);
        public static Expr Not(Expr value) => new NotExpr(value);
        public static Expr Var(string name) => new VarExpr(name);
        public static Expr Assign(Expr assignment, Expr value) => new AssignExpr(assignment, value);
        public static Expr Eq(Expr left, Expr right) => new EqExpr(left, right);
        public static Expr NotEq(Expr left, Expr right) => new NotEqExpr(left, right);
        public static Expr Less(Expr left, Expr right) => new LessExpr(left, right);
        public static Expr LessOrEq(Expr left, Expr right) => new LessOrEqExpr(left, right);
        public static Expr More(Expr left, Expr right) => new MoreExpr(left, right);
        public static Expr MoreOrEq(Expr left, Expr right) => new MoreOrEqExpr(left, right);
        public static Expr If(Expr condition, Expr result) => new IfExpr(condition, result);
        public static Expr Choose(Expr left, Expr right) => new ChooseExpr(left, right);
    }
}
