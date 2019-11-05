using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace TinyLang.Compiler.Core.Base
{
    public abstract class TinyModule
    {
        protected readonly TinyAssembly Assembly;
        public ModuleBuilder ModuleBuilder { get; }

        protected TinyModule(TinyAssembly assembly, string name)
        {
            Assembly = assembly;
            ModuleBuilder = assembly.AssemblyBuilder.DefineDynamicModule(name);
        }

        public abstract void DefineTypes();
    }
}
