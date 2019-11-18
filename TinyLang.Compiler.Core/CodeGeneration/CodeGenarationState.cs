using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public enum CodeGenarationStates 
    {
        Assembly,
        Module, 
        Type,
        Method
    }

    public class CodeGenarationState
    {
        public CodeGenarationStates State { get; private set; }

        public AssemblyBuilder AssemblyBuilder { get; private set; }

        public ModuleBuilder ModuleBuilder { get; private set; }

        public TypeBuilder TypeBuilder { get; private set; }

        public MethodBuilder MethodBuilder { get; private set; }

        public CodeGenarationState WithAssembly(string name) 
        {
            MethodBuilder = null; TypeBuilder = null; ModuleBuilder = null;

            AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName
            {
                Name = name
            }, AssemblyBuilderAccess.RunAndCollect);
            State = CodeGenarationStates.Assembly;

            return this;
        }

        public CodeGenarationState WithModule(string name) 
        {
            if (AssemblyBuilder == null) throw new InvalidCodegenerationStateException();

            MethodBuilder = null; TypeBuilder = null;
            ModuleBuilder = AssemblyBuilder.GetDynamicModule(name) ?? AssemblyBuilder.DefineDynamicModule(name);
            State = CodeGenarationStates.Module;

            return this;
        }

        public CodeGenarationState WithType(string name) 
        {
            if(TypeBuilder == null) throw new InvalidCodegenerationStateException();

            TypeBuilder = ModuleBuilder.DefineType(name, TypeAttributes.Public);
            State = CodeGenarationStates.Type;
            return this;
        }

        public CodeGenarationState WithGlobalMethod(string name, Type returnType, Type[] parameterTypes) 
        {
            if(ModuleBuilder == null) throw new InvalidCodegenerationStateException();

            MethodBuilder = ModuleBuilder.DefineGlobalMethod(name, MethodAttributes.Final | MethodAttributes.Public | MethodAttributes.Static, returnType, parameterTypes);

            return this;
        }
    }

    public class InvalidCodegenerationStateException : Exception { }

}
