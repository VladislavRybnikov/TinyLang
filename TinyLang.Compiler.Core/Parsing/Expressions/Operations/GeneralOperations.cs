using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.Compiler.Core.Parsing.Expressions.Operations
{
    public static class GeneralOperations
    {
        public class VarExpr : Expr 
        {
            public string Name { get; }

            public VarExpr(string name) 
            {
                Name = name;
            }

            public override string ToString() 
            {
                return $"Var({Name})";
            }
        }

        public class AssignExpr : Expr 
        {
            public Expr Assigment { get; }
            public Expr Value { get; }
            public AssignExpr(Expr assigment, Expr value) 
            {
                Assigment = assigment;
                Value = value;
            }

            public override string ToString()
            {
                return $"Assign({Assigment}, {Value})";
            }
        }

        public class EqualsExpr : Expr 
        {
            public Expr Left { get; }
            public Expr Right { get; }
            public EqualsExpr(Expr left, Expr right)
            {
                Left = left;
                Right = right;
            }

            public override string ToString()
            {
                return $"Equals({Left}, {Right})";
            }
        }

        public class IfExpr : Expr
        {
            public Expr Condition { get; }

            public Expr Then { get; }

            public IfExpr(Expr condition, Expr then)
            {
                Condition = condition;
                Then = then;
            }

            public override string ToString() 
            {
                return $"If({Condition}, {Then})";
            }
        }

        public class ChooseExpr : Expr
        {
            public Expr Left { get; }
            public Expr Right { get; }

            public ChooseExpr(Expr left, Expr right) 
            {
                Left = left;
                Right = right;
            }

            public override string ToString()
            {
                return $"Choose({Left}, {Right})";
            }
        }
    }
}
