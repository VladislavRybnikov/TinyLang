using System;
using TinyLang.Compiler.Core.Common.Exceptions.Base;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Operations
{
    public static class GeneralOperations
    {
        public class VarExpr : Expr
        {
            public string Name { get; set; }

            public VarExpr(string name)
            {
                Name = name;
            }

            public VarExpr()
            {
            }

            public override string ToString()
            {
                return $"Var({Name})";
            }
        }

        public class AssignExpr : Expr
        {
            public Expr Assigment { get; set; }
            public Expr Value { get; set; }
            public AssignExpr(Expr assigment, Expr value)
            {
                if (assigment is RecordCreationExpr || assigment is FuncInvocationExpr)
                {
                    throw new Exception("Assignment is not allowed");
                }

                Assigment = assigment;
                Value = value;
            }

            public AssignExpr()
            {
            }

            public override string ToString()
            {
                return $"Assign({Assigment}, {Value})";
            }
        }

        public class TernaryIfExpr : Expr
        {
            public Expr Condition { get; set; }

            public Expr Then { get; set; }

            public TernaryIfExpr(Expr condition, Expr then)
            {
                Condition = condition;
                Then = then;
            }

            public TernaryIfExpr()
            {
            }

            public override string ToString()
            {
                return $"If({Condition}, {Then})";
            }

            public IfElseExpr ToIfElse()
            {
                if (!(Then is ChooseExpr (var left, var right))) throw new PositionedException(Then.Pos, "invalid expression");

                var ifExpr = new IfExpr(Condition);
                ifExpr.Scope = new Scope(left);
                var elseExpr = new ElseExpr();
                elseExpr.Scope = new Scope(right);

                return new IfElseExpr(ifExpr, @else: elseExpr);

            }
        }

        public class ChooseExpr : Expr
        {
            public Expr Left { get; set; }
            public Expr Right { get; set; }

            public ChooseExpr(Expr left, Expr right)
            {
                Left = left;
                Right = right;
            }

            public ChooseExpr()
            {
            }

            public void Deconstruct(out Expr left, out Expr right)
            {
                left = Left;
                right = Right;
            }

            public override string ToString()
            {
                return $"Choose({Left}, {Right})";
            }
        }

        public class ArrExpr : Expr
        {
            public Expr[] Values { get; set; }

            public ArrExpr(Expr[] values)
            {
                Values = values;
            }

            public ArrExpr()
            {
            }

            public override string ToString()
            {
                return $"Arr({string.Join<Expr>(", ", Values)})";
            }
        }
    }
}
