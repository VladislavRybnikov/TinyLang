using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace TinyLang.Compiler.Core.CodeGeneration.Predefined
{
    public static class Methods
    {
        public static void AddPrint(CodeGenerationState state) 
        {
            var print = state.ModuleBuilder.DefineGlobalMethod("print", MethodAttributes.Public | MethodAttributes.Static,
                typeof(void), new[] { typeof(object) });

            var il = print.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);

            var writeLineObj = typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, new[] { typeof(object) }, null);

            il.EmitCall(OpCodes.Call, writeLineObj, new[] { typeof(string) });
            il.Emit(OpCodes.Ret);

            state.DefinedMethods.Add("print", print);
        }

        public static void AddPrintN(CodeGenerationState state)
        {
            var printN = state.ModuleBuilder.DefineGlobalMethod("printN", MethodAttributes.Public | MethodAttributes.Static,
                typeof(void), new[] { typeof(int) });

            var ilN = printN.GetILGenerator();

            ilN.Emit(OpCodes.Ldarg_0);

            var writeLineNum = typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, new[] { typeof(int) }, null);

            ilN.EmitCall(OpCodes.Call, writeLineNum, new[] { typeof(int) });
            ilN.Emit(OpCodes.Ret);

            state.DefinedMethods.Add("printN", printN);
        }

        public static void AddPrintB(CodeGenerationState state)
        {
            var printB = state.ModuleBuilder.DefineGlobalMethod("printB", MethodAttributes.Public | MethodAttributes.Static,
                typeof(void), new[] { typeof(bool) });

            var ilB = printB.GetILGenerator();

            ilB.Emit(OpCodes.Ldarg_0);

            var writeLineNum = typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, new[] { typeof(bool) }, null);

            ilB.EmitCall(OpCodes.Call, writeLineNum, new[] { typeof(bool) });
            ilB.Emit(OpCodes.Ret);

            state.DefinedMethods.Add("printB", printB);
        }
    }
}
