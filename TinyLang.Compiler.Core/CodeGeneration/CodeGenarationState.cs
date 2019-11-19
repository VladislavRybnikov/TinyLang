using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.CodeGeneration.Types;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public enum CodeGenerationStates
    {
        Assembly,
        Module,
        Type,
        Method
    }

    public class CodeGenerationState
    {
        public Dictionary<string, LocalBuilder> MainVariables { get; } = new Dictionary<string, LocalBuilder>();

        public Dictionary<string, LocalBuilder> MethodVariables { get; private set; }

        public Dictionary<string, TypedArg> MethodArgs { get; private set; }

        public Dictionary<string, MethodBuilder> DefinedMethods { get; } = new Dictionary<string, MethodBuilder>();

        public CodeGenerationStates State { get; set; }

        public AssemblyBuilder AssemblyBuilder { get; private set; }

        public ModuleBuilder ModuleBuilder { get; private set; }

        public TypeBuilder TypeBuilder { get; private set; }

        public MethodBuilder MethodBuilder { get; private set; }

        public MethodBuilder MainMethodBuilder { get; private set; }

        private CodeGenerationState() { }

        public static CodeGenerationState BeginCodeGeneration(string assemblyName, string moduleName)
        {
            var state = new CodeGenerationState()
                .WithAssembly(assemblyName)
                .WithModule(moduleName);

            state.MainMethodBuilder = state.ModuleBuilder
                .DefineGlobalMethod("main", MethodAttributes.Final | MethodAttributes.Public | MethodAttributes.Static,
                typeof(void), new Type[0]);

            var print = state.ModuleBuilder.DefineGlobalMethod("print", MethodAttributes.Public | MethodAttributes.Static,
                typeof(void), new[] { typeof(object) });
            var il = print.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            var writeLine = typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, new[] { typeof(object) }, null);

            il.EmitCall(OpCodes.Call, writeLine, new[] { typeof(object) });
            il.Emit(OpCodes.Ret);

            state.DefinedMethods.Add("print", print);

            return state;
        }

        public CodeGenerationState EndGeneration()
        {
            State = CodeGenerationStates.Module;
            MethodVariables = null;
            MethodArgs = null;

            return this;
        }

        public CodeGenerationState WithAssembly(string name)
        {
            MethodBuilder = null; TypeBuilder = null; ModuleBuilder = null;

            AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName
            {
                Name = name
            }, AssemblyBuilderAccess.RunAndCollect);
            State = CodeGenerationStates.Assembly;

            return this;
        }

        public CodeGenerationState WithModule(string name)
        {
            if (AssemblyBuilder == null) throw new InvalidCodeGenerationStateException();

            MethodBuilder = null; TypeBuilder = null;
            ModuleBuilder = AssemblyBuilder.GetDynamicModule(name) ?? AssemblyBuilder.DefineDynamicModule(name);
            State = CodeGenerationStates.Module;

            return this;
        }

        public CodeGenerationState WithType(string name)
        {
            if (ModuleBuilder == null) throw new InvalidCodeGenerationStateException();

            TypeBuilder = ModuleBuilder.DefineType(name, TypeAttributes.Public |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit,
                typeof(object));
            State = CodeGenerationStates.Type;
            return this;
        }

        public CodeGenerationState WithGlobalMethod(string name, Type returnType, Type[] parameterTypes)
        {
            if (ModuleBuilder == null) throw new InvalidCodeGenerationStateException();

            MethodBuilder = ModuleBuilder.DefineGlobalMethod(name, MethodAttributes.Final | MethodAttributes.Public | MethodAttributes.Static, returnType, parameterTypes);
            State = CodeGenerationStates.Method;

            MethodArgs = new Dictionary<string, TypedArg>();
            MethodVariables = new Dictionary<string, LocalBuilder>();

            return this;
        }

        public CodeGenerationState AddVariable(string name, LocalBuilder lb)
        {
            if (State == CodeGenerationStates.Method)
            {
                MethodVariables.Add(name, lb);
            }
            else
            {
                MainVariables.Add(name, lb);
            }

            return this;
        }
    }

    public class InvalidCodeGenerationStateException : Exception { }

}
