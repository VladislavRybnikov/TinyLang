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

            var writeLineObj = typeof(Console).GetMethod("Write", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, new[] { typeof(object) }, null);

            il.EmitCall(OpCodes.Call, writeLineObj, new[] { typeof(object) });
            il.Emit(OpCodes.Ret);

            state.DefinedMethods.Add("print", print);
        }

        public static void AddPrintLine(CodeGenerationState state)
        {
            var print = state.ModuleBuilder.DefineGlobalMethod("println", MethodAttributes.Public | MethodAttributes.Static,
                typeof(void), new[] { typeof(object) });

            var il = print.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);

            var writeLineObj = typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, new[] { typeof(object) }, null);

            il.EmitCall(OpCodes.Call, writeLineObj, new[] { typeof(object) });
            il.Emit(OpCodes.Ret);

            state.DefinedMethods.Add("println", print);
        }

        public static void AddPrintF(CodeGenerationState state) 
        {
            var printF = state.ModuleBuilder.DefineGlobalMethod("printF", MethodAttributes.Public | MethodAttributes.Static,
                typeof(void), new[] { typeof(string), typeof(object) });

            var ilF = printF.GetILGenerator();

            ilF.Emit(OpCodes.Ldarg_0);
            ilF.Emit(OpCodes.Ldarg_1);

            var writeLineF = typeof(Console).GetMethod(nameof(Console.Write), BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, new[] { typeof(string), typeof(object) }, null);

            ilF.EmitCall(OpCodes.Call, writeLineF, new[] { typeof(string), typeof(object) });
            ilF.Emit(OpCodes.Ret);

            state.DefinedMethods.Add("printF", printF);
        }
    }
}
