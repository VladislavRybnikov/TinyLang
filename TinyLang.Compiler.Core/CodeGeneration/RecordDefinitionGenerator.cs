using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Types;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public class RecordDefinitionGenerator : CodeGenerator<RecordExpr>
    {
        protected internal override CodeGenerationState GenerateInternal(RecordExpr expression, CodeGenerationState state)
        {
            if (state.State == CodeGenerationStates.Method) throw new Exception("Can not define record in methods");

            var tb = state.WithType(expression.Name).TypeBuilder;

            var props = expression.Props.Select(x => DefineProperty(x, tb, state)).ToArray();

            DefineCtor(props, tb);
            OverrideToStr(props, tb, expression.Name);
            state.TypeBuilder.CreateType();
            
            return state.EndGeneration();
        }

        private void OverrideToStr(PropInfo[] props, TypeBuilder tb, string name)
        {
            Type[] formatStringArgs = new[] { typeof(string) }
            .Append(Enumerable.Range(0, props.Length).Select(_ => typeof(object))).ToArray();

            MethodInfo formatString = typeof(string).GetMethod(nameof(string.Format), formatStringArgs);

            var mb = tb.DefineMethod("ToString", MethodAttributes.Public | MethodAttributes.HideBySig |
                    MethodAttributes.NewSlot | MethodAttributes.Virtual |
                    MethodAttributes.Final, CallingConventions.HasThis, typeof(string),
                    Type.EmptyTypes);

            var il = mb.GetILGenerator();
            il.Emit(OpCodes.Nop);

            var propsStr = string.Join(", ", props.Select((p, i) => (prop: p, index: i)).Select(x => $"{x.prop.Name}({{{x.index}}}))"));
            var str = $"({name}({propsStr}))";

            il.Emit(OpCodes.Ldstr, str);

            foreach (var prop in props)
            {
                il.Emit(OpCodes.Ldarg_0);

                //il.EmitCall(OpCodes.Call, prop.Getter, new[] { prop.Type});
               
                il.Emit(OpCodes.Call, prop.PropertyBuilder.GetMethod);

                if (prop.Type.IsValueType)
                {
                    il.Emit(OpCodes.Box, prop.Type);
                }
                il.Emit(OpCodes.Nop);
            }

            il.Emit(OpCodes.Call, formatString);
            il.Emit(OpCodes.Ret);

            tb.DefineMethodOverride(mb, typeof(object).GetMethod(nameof(object.ToString)));
        }

        private void DefineCtor(PropInfo[] props, TypeBuilder tb)
        {
            var parameters = props.Select(p => p.Type).ToArray();

            var cb = tb.DefineConstructor(MethodAttributes.Public
                | MethodAttributes.HideBySig
                | MethodAttributes.SpecialName
                | MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                parameters);

            var objCtor = typeof(object).GetConstructor(new Type[0]);

            var cil = cb.GetILGenerator();

            cil.Emit(OpCodes.Ldarg_0);
            cil.Emit(OpCodes.Call, objCtor);
            cil.Emit(OpCodes.Nop);
            cil.Emit(OpCodes.Nop);

            for (int i = 0; i < props.Length; i++)
            {
                cil.Emit(OpCodes.Ldarg_0);
                cil.Emit(OpCodes.Ldarg_S, (sbyte)(i + 1));
                cil.Emit(OpCodes.Call, props[i].PropertyBuilder.SetMethod);
                cil.Emit(OpCodes.Nop);
            }

            cil.Emit(OpCodes.Nop);
            cil.Emit(OpCodes.Ret);
        }

        private PropInfo DefineProperty
            (TypedVar prop, TypeBuilder tb, CodeGenerationState state) 
        {
            var p_Type = TypesResolver.Resolve(prop.Type.TypeName, state.ModuleBuilder);

            var fb = tb.DefineField($"f_{prop.Var.Name}", p_Type, FieldAttributes.Private);

            var pb = tb.DefineProperty(prop.Var.Name, PropertyAttributes.HasDefault,
                TypesResolver.Resolve(prop.Type.TypeName, state.ModuleBuilder), null);

            var pGet = tb.DefineMethod($"get_{prop.Var.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                p_Type, Type.EmptyTypes);

            var pGetILGenerator = pGet.GetILGenerator();

            pGetILGenerator.Emit(OpCodes.Ldarg_0);
            pGetILGenerator.Emit(OpCodes.Ldfld, fb);
            pGetILGenerator.Emit(OpCodes.Ret);

            var pSet = tb.DefineMethod($"set_{prop.Var.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null, new[] { p_Type });

            var pSetILGenerator = pSet.GetILGenerator();

            pSetILGenerator.Emit(OpCodes.Ldarg_0);
            pSetILGenerator.Emit(OpCodes.Ldarg_1);
            pSetILGenerator.Emit(OpCodes.Stfld, fb);
            pSetILGenerator.Emit(OpCodes.Ret);

            pb.SetGetMethod(pGet);
            pb.SetSetMethod(pSet);

            return new PropInfo(prop.Var.Name, p_Type, pGet, pSet, pb);
        }

        private class PropInfo 
        {
            public string Name { get; }

            public Type Type { get; }

            public MethodBuilder Setter { get; }

            public MethodBuilder Getter { get; }

            public PropertyBuilder PropertyBuilder { get; }

            public PropInfo(string name, Type type, MethodBuilder getter, MethodBuilder setter, PropertyBuilder pb)
            {
                Name = name;
                Type = type;
                Getter = getter;
                Setter = setter;
                PropertyBuilder = pb;
            }
        }

        public RecordDefinitionGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }
    }
}
