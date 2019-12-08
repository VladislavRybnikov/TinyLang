using System;
using System.Linq;
using System.Reflection.Emit;
using TinyLang.Compiler.Core.CodeGeneration.Types;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;
using TinyLang.Compiler.Core.CodeGeneration.Extensions;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.CompareOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.NumOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.BoolOperations;
using TinyLang.Compiler.Core.CodeGeneration.Utils;

namespace TinyLang.Compiler.Core.CodeGeneration.Types
{
    public class TypedLoader
    {
        public Type Type { get; }

        public Action Load { get; }

        public TypedLoader(Type type, Action load)
        {
            Type = type;
            Load = load;
        }

        public void Deconstruct(out Type type, out Action load)
        {
            type = Type;
            load = Load;
        }

        public static implicit operator TypedLoader((Type type, Action load) tuple)
            => new TypedLoader(tuple.type, tuple.load);

        public static TypedLoader FromValue(Expr expr, ILGenerator ilGenerator, CodeGenerationState state, ICodeGeneratorsFactory factory) => expr switch
        {
            VarExpr v => FromMethodScope(v, ilGenerator, state),
            PropExpr p => FromProperty(p, ilGenerator, state, factory),
            StrExpr str => (typeof(string), (Action)(() => ilGenerator?.Emit(OpCodes.Ldstr, str.Value))),
            IntExpr @int => (typeof(int), () => ilGenerator?.Emit(OpCodes.Ldc_I4, @int.Value)),
            BoolExpr @bool => (typeof(bool), () => ilGenerator?.Emit(@bool.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0)),
            RecordCreationExpr record => FromRecordCreation(record, ilGenerator, state, factory),
            FuncInvocationExpr f => FromFuncCall(f, ilGenerator, state, factory),
            BinaryExpr bin => FromBinaryExpr(bin, ilGenerator, state, factory),
            TernaryIfExpr t => FromTernaryExpr(t, ilGenerator, state, factory),
            LambdaExpr l => FromLambda(l, ilGenerator, state, factory),
            _ => throw new Exception("Unsupported variable type")
        };

        public static TypedLoader FromLambda(LambdaExpr lambda, ILGenerator ilGenerator,
            CodeGenerationState state, ICodeGeneratorsFactory factory)
        {
            factory.GeneratorFor<LambdaExpr>().Generate(lambda, state);
            var method = state.AnonymousMethodsCache.Peek();

            var argsTypes = lambda.Args.Select(x => TypesResolver.Resolve(x.Type, state.ModuleBuilder)).ToArray();

            var type = FuncTypeResolver.Resolve(method.ReturnType, argsTypes);

            var constructorInfo =
                type.GetConstructor(new [] { typeof(object), typeof(IntPtr) });

            return (type,
                () =>
                {
                    ilGenerator.Emit(OpCodes.Ldnull);
                    ilGenerator.Emit(OpCodes.Ldftn, method);
                    ilGenerator.Emit(OpCodes.Newobj, constructorInfo);
                }
            );
        }

        public static TypedLoader FromTernaryExpr(TernaryIfExpr expr, ILGenerator ilGenerator,
            CodeGenerationState state, ICodeGeneratorsFactory factory) 
        {
            if (!(expr.Then is ChooseExpr ch))
            {
                throw new Exception("Wrong ternary operator structure");
            }

            var leftType = FromValue(ch.Left, null, state, factory).Type;
            var rightType = FromValue(ch.Left, null, state, factory).Type;

            if (leftType != rightType)
            {
                throw new Exception($"Can not resolve return type of ternary operator. Left type: {leftType}, right type: {rightType}");
            }

            var generator = factory.GeneratorFor<IfElseExpr>();

            return (leftType, () => generator.Generate(expr, state));
        }

        public static TypedLoader FromBinaryExpr(BinaryExpr expr, ILGenerator ilGenerator,
            CodeGenerationState state, ICodeGeneratorsFactory factory)
        {
            var leftLoader = FromValue(expr.Left, ilGenerator, state, factory);
            var rightLoader = FromValue(expr.Right, ilGenerator, state, factory);

            (Action<ILGenerator> operation, Type type) = expr switch
            {
                AddExpr _ => leftLoader.Type switch
                {
                    Type t when t == typeof(int) => (OpCodes.Add.EmitSingle(), typeof(int)),
                    Type t when t == typeof(string) => (EmitStringConcat(), typeof(string))
                },
                SubtrExpr _ => (OpCodes.Sub.EmitSingle(), typeof(int)),
                MulExpr _ => (OpCodes.Mul.EmitSingle(), typeof(int)),
                DivExpr _ => (OpCodes.Div.EmitSingle(), typeof(int)),
                EqExpr _ => (OpCodes.Ceq.EmitSingle(), typeof(bool)),
                LessExpr _ => (OpCodes.Clt.EmitSingle(), typeof(bool)),
                MoreExpr _ => (OpCodes.Cgt.EmitSingle(), typeof(bool)),
                LessOrEqExpr _ => (OpCodes.Ceq.EmitOr(OpCodes.Clt, leftLoader, rightLoader), typeof(bool)),
                MoreOrEqExpr _ => (OpCodes.Ceq.EmitOr(OpCodes.Cgt, leftLoader, rightLoader), typeof(bool)),
                AndExpr _ => (OpCodes.And.EmitSingle(), typeof(bool)),
                OrExpr _ => (OpCodes.Or.EmitSingle(), typeof(bool))
            };

            return LoadWithOperation(leftLoader, rightLoader, type, () => operation(ilGenerator));
        }

        public static TypedLoader FromProperty(PropExpr prop, ILGenerator ilGenerator, CodeGenerationState state, ICodeGeneratorsFactory factory) 
        {
            var (varType, varLoader) = FromValue(prop.Expr, ilGenerator, state, factory);

            varLoader();
            var property = varType.GetProperty(prop.Prop);
            var get = property.GetGetMethod();

            return (property.PropertyType, () => ilGenerator.Emit(OpCodes.Call, get));
        }

        public static TypedLoader FromRecordCreation(RecordCreationExpr expr, ILGenerator ilGenerator, CodeGenerationState state, ICodeGeneratorsFactory factory)
        {
            var type = state.ModuleBuilder.GetType(expr.Name);
            var ctor = type.GetConstructors()[0];
            var ctorParams = ctor.GetParameters();

            var providedParams = expr.Props.ToArray();

            if (ctorParams.Length != providedParams.Length)
                throw new IndexOutOfRangeException("Wrong args");

            for (int i = 0; i < ctorParams.Length; i++)
            {
                FromValue(providedParams[i], ilGenerator, state, factory).Load();
            }

            return (type, () => ilGenerator.Emit(OpCodes.Newobj, ctor));
        }

        public static TypedLoader FromFuncCall(FuncInvocationExpr expr, ILGenerator ilGenerator, CodeGenerationState state, ICodeGeneratorsFactory factory)
        {
            var generator = factory.GeneratorFor<FuncInvocationExpr>();

            var fromM = state.DefinedMethods.TryGetValue(expr.Name, out var m);
            var fromV = state.TryResolveVariable(expr.Name, out var lb);

            var returnType = fromM ? m.ReturnType : fromV ? lb.LocalType?.GetMethod("Invoke")?.ReturnType : throw new Exception("!");
            return (returnType, () => generator.Generate(expr, state));
        }

        public static TypedLoader FromMethodScope(VarExpr expr, ILGenerator ilGenerator,
            CodeGenerationState state)
        {
            if (state.Scope == CodeGenerationScope.Method)
            {
                if (state.MethodArgs.TryGetValue(expr.Name, out var arg))
                {
                    return (arg.Type, () => arg.EmitLoad?.Invoke(ilGenerator));
                }

                if (state.MethodVariables.TryGetValue(expr.Name, out var v))
                {
                    return (v.LocalType, () => ilGenerator?.Emit(OpCodes.Ldloc, v));
                }

            }

            if (state.MainVariables.TryGetValue(expr.Name, out var mv))
            {
                return (mv.LocalType, () => ilGenerator.Emit(OpCodes.Ldloc, mv));
            }

            throw new Exception("Can not resolve variable");
        }

        private static TypedLoader LoadWithOperation(TypedLoader left, TypedLoader right, Type returnType, Action EmitOp)
        {
            return new TypedLoader(returnType, () =>
            {
                left.Load();
                right.Load();

                EmitOp();
            }
            );
        }

        private static Action<ILGenerator> EmitStringConcat() => il =>
        {
            var strConcat = typeof(string).GetMethod(nameof(string.Concat),
                new[] { typeof(string), typeof(string) });

            il.EmitCall(OpCodes.Call, strConcat, new[] { typeof(string), typeof(string) });
        };
    }
}
