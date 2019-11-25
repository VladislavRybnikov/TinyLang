using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.CodeGeneration.Types;

namespace TinyLang.Compiler.Core.CodeGeneration.Extensions
{
    public static class OpCodeExtension
    {
        public static Action<ILGenerator> EmitSingle(this OpCode opcode) => il => il.Emit(opcode);

        public static Action<ILGenerator> EmitOr(this OpCode left, OpCode right,
            TypedLoader leftLoader, TypedLoader rightLoader) => il =>
        {
            var leftLb = il.DeclareLocal(leftLoader.Type);
            var rightLb = il.DeclareLocal(rightLoader.Type);

            var firstRes = il.DeclareLocal(typeof(bool));

            il.Emit(OpCodes.Stloc, rightLb);
            il.Emit(OpCodes.Stloc, leftLb);

            il.Emit(OpCodes.Ldloc, leftLb);
            il.Emit(OpCodes.Ldloc, rightLb);

            il.Emit(left);
            il.Emit(OpCodes.Stloc, firstRes);

            il.Emit(OpCodes.Ldloc, leftLb);
            il.Emit(OpCodes.Ldloc, rightLb);
            il.Emit(right);
            il.Emit(OpCodes.Ldloc, firstRes);

            il.Emit(OpCodes.Or);
        };
    }
}
