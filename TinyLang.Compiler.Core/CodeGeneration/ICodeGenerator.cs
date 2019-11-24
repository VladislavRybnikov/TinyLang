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
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.GeneralOperations;
using static TinyLang.Compiler.Core.Parsing.Expressions.Operations.NumOperations;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public interface ICodeGenerator
    {
        CodeGenerationState Generate(Expr expression, CodeGenerationState state);
    }

    public interface ICodeGeneratorsFactory
    {
        ICodeGenerator GeneratorFor(Type type);
        ICodeGenerator GeneratorFor<T>() where T : Expr;
    }

    public class CodeGeneratorsFactory : ICodeGeneratorsFactory
    {
        private static CodeGeneratorsFactory _instance;

        private IDictionary<Type, ICodeGenerator> _genartors;

        private CodeGeneratorsFactory() { }

        public ICodeGenerator GeneratorFor(Type type) => _genartors[type];
        public ICodeGenerator GeneratorFor<T>() where T : Expr => _genartors[typeof(T)];

        public static ICodeGeneratorsFactory Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = new CodeGeneratorsFactory();
                _instance._genartors = new Dictionary<Type, ICodeGenerator>
                {
                    { typeof(AssignExpr), new VarDefinitionGenerator(_instance) },
                    { typeof(RecordExpr), new RecordDefinitionGenerator(_instance) },
                    { typeof(FuncInvocationExpr), new FuncCallGenerator(_instance) },
                    { typeof(FuncExpr), new FuncDefinitionGenerator(_instance) },
                    { typeof(RetExpr), new FuncReturnGenerator(_instance) },
                    { typeof(RecordCreationExpr), new RecordCreationGenerator(_instance) },
                    { typeof(IfElseExpr), new IfElseGenerator(_instance) }
                };

                return _instance;
            }
        }
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

        protected TypedLoader VarLoader(Expr expr, ILGenerator ilGenerator, CodeGenerationState state) => expr switch
        {
            VarExpr v => MethodScopeLoader(v, ilGenerator, state),
            StrExpr str => (typeof(string), (Action)(() => ilGenerator.Emit(OpCodes.Ldstr, str.Value))),
            IntExpr @int => (typeof(int), () => ilGenerator.Emit(OpCodes.Ldc_I4, @int.Value)),
            BoolExpr @bool => (typeof(bool), () => ilGenerator.Emit(@bool.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0)),
            RecordCreationExpr record => RecordLoader(record, ilGenerator, state),
            FuncInvocationExpr f => FuncLoader(f, ilGenerator, state),
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
            var leftLoader = VarLoader(expr.Left, ilGenerator, state);
            var rightLoader = VarLoader(expr.Right, ilGenerator, state);

            return expr switch
            {
                AddExpr add => (leftLoader.Type, () => 
                {
                    leftLoader.Load();
                    rightLoader.Load();

                    ilGenerator.Emit(OpCodes.Add);
                })
            };

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
                VarLoader(providedParams[i], ilGenerator, state).Load();
            }

            return (type, () => ilGenerator.Emit(OpCodes.Newobj, ctor));
        }

        protected TypedLoader FuncLoader
            (FuncInvocationExpr expr, ILGenerator ilGenerator, CodeGenerationState state)
        {
            var generator = Factory.GeneratorFor<FuncInvocationExpr>();

            return (state.DefinedMethods[expr.Name].ReturnType, () => generator.Generate(expr, state));
        }
    }
}
