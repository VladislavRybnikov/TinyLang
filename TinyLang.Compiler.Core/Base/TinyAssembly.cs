using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace TinyLang.Compiler.Core.Base
{
    public class TinyAssembly
    {
        private readonly IDictionary<Type, TinyModule> _modules = new Dictionary<Type, TinyModule>();

        public string Name { get; }

        public AssemblyBuilder AssemblyBuilder { get; }

        public TinyAssembly(string name)
        {
            Name = name;
            AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName
            {
                Name = Name
            }, AssemblyBuilderAccess.Run);
        }

        public TinyAssembly With<TModule>() where TModule : TinyModule
        {
            var moduleType = typeof(TModule);
            var module = moduleType.GetConstructor(new []{GetType() })?.Invoke(new object[] { this }) as TModule;
            module?.DefineTypes();
            _modules.Add(moduleType, module);
            return this;
        }

        public TModule Get<TModule>() where TModule : TinyModule =>
            _modules.TryGetValue(typeof(TModule), out var module) ? module as TModule : null;
    }
}
