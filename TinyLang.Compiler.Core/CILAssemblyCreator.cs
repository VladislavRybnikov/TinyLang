using TinyLang.Compiler.Core.Assemblies.TinyCore;
using TinyLang.Compiler.Core.Base;

namespace TinyLang.Compiler.Core
{
    public class CILAssemblyCreator
    {
        private static readonly TinyAssembly TinyCore
            = new TinyAssembly("TinyCore")
                .With<SystemModule>();

        public static void Print(string message)
        {
            var system = TinyCore.Get<SystemModule>().ModuleBuilder;

            system.CreateGlobalFunctions();

            system.GetMethod("Print").Invoke(null, new object[]{ message });
        }
    }
}
