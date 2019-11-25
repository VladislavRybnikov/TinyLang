using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.CodeGeneration.Generators;
using TinyLang.Compiler.Core.CodeGeneration.Types;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.CompareOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.NumOperations;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public interface ICodeGenerator
    {
        CodeGenerationState Generate(Expr expression, CodeGenerationState state);
    }

    public abstract class CodeGenerator<TExpr> : ICodeGenerator where TExpr : Expr
    {
        protected readonly ICodeGeneratorsFactory Factory;
        protected CodeGenerator(ICodeGeneratorsFactory factory)
        {
            Factory = factory;
        }

        public CodeGenerationState Generate(Expr expression, CodeGenerationState state)
        {
            return GenerateInternal(expression as TExpr, state);
        }

        protected internal abstract CodeGenerationState GenerateInternal(TExpr expression, CodeGenerationState state);

        protected TypedLoader ValueLoader(Expr expr, ILGenerator ilGenerator, CodeGenerationState state) => expr switch
        {
            VarExpr v => MethodScopeLoader(v, ilGenerator, state),
            StrExpr str => (typeof(string), (Action)(() => ilGenerator.Emit(OpCodes.Ldstr, str.Value))),
            IntExpr @int => (typeof(int), () => ilGenerator.Emit(OpCodes.Ldc_I4, @int.Value)),
            BoolExpr @bool => (typeof(bool), () => ilGenerator.Emit(@bool.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0)),
            RecordCreationExpr record => RecordLoader(record, ilGenerator, state),
            FuncInvocationExpr f => FuncCallLoader(f, ilGenerator, state),
            BinaryExpr bin => BinaryExprLoader(bin, ilGenerator, state),
            _ => throw new Exception("Unsupported variable type")
        };

        protected void LoadScope(Scope scope, CodeGenerationState state)
        {
            foreach (var s in scope.Statements)
            {
                Factory.GeneratorFor(s.GetType()).Generate(s, state);
            }
        }

        protected TypedLoader BinaryExprLoader(BinaryExpr expr, ILGenerator ilGenerator,
            CodeGenerationState state)
        {
            var leftLoader = ValueLoader(expr.Left, ilGenerator, state);
            var rightLoader = ValueLoader(expr.Right, ilGenerator, state);

            (Action<ILGenerator> operation, Type type) = expr switch
            {
                AddExpr _ => (EmitSingle(OpCodes.Add), typeof(int)),
                SubtrExpr _ => (EmitSingle(OpCodes.Sub), typeof(int)),
                MulExpr _ => (EmitSingle(OpCodes.Mul), typeof(int)),
                DivExpr _ => (EmitSingle(OpCodes.Div), typeof(int)),
                EqExpr _ => (EmitSingle(OpCodes.Ceq), typeof(bool)),
                LessExpr _ => (EmitSingle(OpCodes.Clt), typeof(bool)),
                MoreExpr _ => (EmitSingle(OpCodes.Cgt), typeof(bool)),
                LessOrEqExpr _ => (EmitOr(OpCodes.Ceq, OpCodes.Clt, leftLoader, rightLoader), typeof(bool)),
                MoreOrEqExpr _ => (EmitOr(OpCodes.Ceq, OpCodes.Cgt, leftLoader, rightLoader), typeof(bool))
            };

            return LoadWithOperation(leftLoader, rightLoader, type, () => operation(ilGenerator));
        }

        protected TypedLoader MethodScopeLoader(VarExpr expr, ILGenerator ilGenerator,
            CodeGenerationState state)
        {
            if (state.State == CodeGenerationStates.Method)
            {
                if (state.MethodArgs.TryGetValue(expr.Name, out var arg))
                {
                    return (arg.Type, () => arg.EmitLoad(ilGenerator));
                }

                if (state.MethodVariables.TryGetValue(expr.Name, out var v))
                {
                    return (v.LocalType, () => ilGenerator.Emit(OpCodes.Ldloc, v));
                }

            }

            if (state.MainVariables.TryGetValue(expr.Name, out var mv))
            {
                return (mv.LocalType, () => ilGenerator.Emit(OpCodes.Ldloc, mv));
            }

            throw new Exception("Can not resolve variable");
        }

        protected TypedLoader RecordLoader
           (RecordCreationExpr expr, ILGenerator ilGenerator, CodeGenerationState state)
        {
            var type = state.ModuleBuilder.GetType(expr.Name);
            var ctor = type.GetConstructors()[0];
            var ctorParams = ctor.GetParameters();

            var providedParams = expr.Props.ToArray();

            if (ctorParams.Length != providedParams.Length)
                throw new IndexOutOfRangeException("Wrong args");

            for (int i = 0; i < ctorParams.Length; i++)
            {
                ValueLoader(providedParams[i], ilGenerator, state).Load();
            }

            return (type, () => ilGenerator.Emit(OpCodes.Newobj, ctor));
        }

        protected TypedLoader FuncCallLoader
            (FuncInvocationExpr expr, ILGenerator ilGenerator, CodeGenerationState state)
        {
            var generator = Factory.GeneratorFor<FuncInvocationExpr>();

            return (state.DefinedMethods[expr.Name].ReturnType, () => generator.Generate(expr, state));
        }

        private TypedLoader LoadWithOperation(TypedLoader left, TypedLoader right, Type returnType, Action EmitOp)
        {
            return new TypedLoader(returnType, () =>
            {
                left.Load();
                right.Load();

                EmitOp();
            }
            );
        }

        private Action<ILGenerator> EmitSingle(OpCode opCode) => il => il.Emit(opCode);

        private Action<ILGenerator> EmitOr(OpCode left, OpCode right,
            TypedLoader leftLoader, TypedLoader rightLoader) => il =>
        {
            var leftLb = il.DeclareLocal(leftLoader.Type);
            var rightLb = il.DeclareLocal(rightLoader.Type);

            var firstRes = il.DeclareLocal(typeof(bool));

            il.Emit(OpCodes.Stloc, rightLb);
            il.Emit(OpCodes.Stloc, leftLb);

            il.Emit(OpCodes.Ldloc, leftLb);
            il.Emit(OpCodes.Ldloc, rightLb);

            il.Emit(left);
            il.Emit(OpCodes.Stloc, firstRes);

            il.Emit(OpCodes.Ldloc, leftLb);
            il.Emit(OpCodes.Ldloc, rightLb);
            il.Emit(right);
            il.Emit(OpCodes.Ldloc, firstRes);

            il.Emit(OpCodes.Or);
        };
    }
}
