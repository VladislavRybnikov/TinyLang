using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;
using TinyLang.Compiler.Core.Parsing.Expressions.Operations;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public class FuncReturnGenerator : CodeGenerator<RetExpr>
    {
        public FuncReturnGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }

        protected internal override CodeGenerationState GenerateInternal(RetExpr expression, CodeGenerationState state)
        {
            if (expression.Expr is GeneralOperations.VarExpr v)
            {
                LoadVar(v, state.MethodBuilder.GetILGenerator(), state).emitLoad();
            }
            else
            {
                Factory.GeneratorFor(expression.Expr.GetType()).Generate(expression.Expr, state);
            }

            return state;
        }
    }
}
