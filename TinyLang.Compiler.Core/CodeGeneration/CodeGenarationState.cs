﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.CodeGeneration.Predefined;
using TinyLang.Compiler.Core.CodeGeneration.Types;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public enum CodeGenerationScope
    {
        Assembly,
        Module,
        Type,
        Method
    }

    public class CodeGenerationState
    {
        public AnonymousMethodsCache AnonymousMethodsCache { get; } = new AnonymousMethodsCache();
        public Dictionary<string, LocalBuilder> MainVariables { get; } = new Dictionary<string, LocalBuilder>();

        public Dictionary<string, LocalBuilder> MethodVariables { get; private set; }

        public Dictionary<string, TypedArg> MethodArgs { get; private set; }

        public Dictionary<string, MethodBuilder> DefinedMethods { get; } = new Dictionary<string, MethodBuilder>();

        public CodeGenerationScope Scope { get; set; }

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

            Methods.AddPrint(state);
            Methods.AddPrintF(state);
            return state;
        }

        public CodeGenerationState EndGeneration()
        {
            Scope = CodeGenerationScope.Module;
            MethodVariables = null;
            MethodArgs = null;

            return this;
        }

        public CodeGenerationState WithAssembly(string name)
        {
            MethodBuilder = null; TypeBuilder = null; ModuleBuilder = null;

            AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName
            {
                Name = name,
            }, AssemblyBuilderAccess.RunAndCollect);
            Scope = CodeGenerationScope.Assembly;

            return this;
        }

        public CodeGenerationState WithModule(string name)
        {
            if (AssemblyBuilder == null) throw new InvalidCodeGenerationStateException();

            MethodBuilder = null; TypeBuilder = null;
            ModuleBuilder = AssemblyBuilder.GetDynamicModule(name) ?? AssemblyBuilder.DefineDynamicModule(name);
            Scope = CodeGenerationScope.Module;

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
            Scope = CodeGenerationScope.Type;
            return this;
        }

        public CodeGenerationState WithGlobalMethod(string name, Type[] parameterTypes, MethodAttributes attr)
        {
            if (ModuleBuilder == null) throw new InvalidCodeGenerationStateException();

            MethodBuilder = ModuleBuilder.DefineGlobalMethod(name, attr, null, parameterTypes);
            Scope = CodeGenerationScope.Method;
            DefinedMethods.Add(name, MethodBuilder);
            MethodArgs = new Dictionary<string, TypedArg>();
            MethodVariables = new Dictionary<string, LocalBuilder>();

            return this;
        }

        public CodeGenerationState AddVariable(string name, LocalBuilder lb)
        {
            if (Scope == CodeGenerationScope.Method)
            {
                MethodVariables.Add(name, lb);
            }
            else
            {
                MainVariables.Add(name, lb);
            }

            return this;
        }

        public Type ResolveVariableType(string name) 
        {
            LocalBuilder variable;
            Type t = null;

            if (Scope == CodeGenerationScope.Method)
            {
                if (MethodVariables.TryGetValue(name, out variable))
                {
                    t = variable.LocalType;
                }
                else
                {
                    if(MethodArgs.TryGetValue(name, out var arg))
                    {
                        t = arg.Type;
                    }
                }
            }

            if (t != null) return t;

            if (MainVariables.TryGetValue(name, out variable))
            {
                t = variable.LocalType;
            }

            return t;
        }

        public bool TryResolveVariable(string name, out LocalBuilder lb) => Scope == CodeGenerationScope.Method 
            ? MethodVariables.TryGetValue(name, out lb) 
            : MainVariables.TryGetValue(name, out lb);

        public MethodBuilder ResolveMethod(string name) => DefinedMethods[name];
    }

    public class InvalidCodeGenerationStateException : Exception { }

}
