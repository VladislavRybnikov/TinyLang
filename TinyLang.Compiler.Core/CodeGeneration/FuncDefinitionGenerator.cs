using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public class FuncDefinitionGenerator : CodeGenerator<FuncExpr>
    {
        protected internal override CodeGenerationState GenerateInternal(FuncExpr expression, CodeGenerationState state)
        {
            var retExpr = expression.Body.Statements
                              .Select(x => x is RetExpr ret ? ret : null)
                              .FirstOrDefault(x => x != null);

            //state.WithGlobalMethod(expression.Name);



            throw new NotImplementedException();
        }

        public FuncDefinitionGenerator(ICodeGeneratorsFactory factory) : base(factory)
        {
        }
    }
}
