using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.Base;

namespace TinyLang.Compiler.Core.Assemblies.TinyCore
{
    public class SystemModule : TinyModule
    {
        public SystemModule(TinyAssembly assembly) : base(assembly, "System")
        {
        }

        public override void DefineTypes()
        {
            AddPrintMethod();
        }

        private void AddPrintMethod()
        {
            MethodBuilder print = ModuleBuilder.DefineGlobalMethod("print", MethodAttributes.Final | MethodAttributes.Public | MethodAttributes.Static, typeof(void), new []{typeof(string)});

            ILGenerator il = print.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            var writeLine = typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, 
                Type.DefaultBinder, new[] { typeof(string) }, null);

            il.EmitCall(OpCodes.Call, writeLine, new[] { typeof(string) });
            il.Emit(OpCodes.Ret);
        }
    }
}
