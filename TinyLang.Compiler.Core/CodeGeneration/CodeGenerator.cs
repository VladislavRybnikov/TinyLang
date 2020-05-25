using System;
using System.Reflection.Emit;
using TinyLang.Compiler.Core.CodeGeneration.Types;
using TinyLang.Compiler.Core.Common.Exceptions;
using TinyLang.Compiler.Core.Parsing.Expressions;
using TinyLang.Compiler.Core.Parsing.Expressions.Constructions;

namespace TinyLang.Compiler.Core.CodeGeneration
{
    public interface ICodeGenerator
    {
        CodeGenerationState Generate(Expr expression, CodeGenerationState state);
    }

    public abstract class CodeGenerator<TExpr> : ICodeGenerator where TExpr : Expr
    {
        public readonly ICodeGeneratorsFactory Factory;
        protected CodeGenerator(ICodeGeneratorsFactory factory)
        {
            Factory = factory;
        }

        public CodeGenerationState Generate(Expr expression, CodeGenerationState state)
        {
            return GenerateInternal(Typed(expression), state);
        }

        protected virtual TExpr Typed(Expr expr) => expr is TExpr typed ? typed
                : throw new ExprTypeMismatchException(typeof(TExpr), expr.GetType(), expr.Pos);

        protected internal abstract CodeGenerationState GenerateInternal(TExpr expression, CodeGenerationState state);

        protected TypedLoader ValueLoader(Expr expr, ILGenerator ilGenerator, CodeGenerationState state)
            => TypedLoader.FromValue(expr, ilGenerator, state, Factory);

        protected void LoadScope(Scope scope, CodeGenerationState state)
        {
            foreach (var s in scope.Statements)
            {
                Factory.GeneratorFor(s.GetType()).Generate(s, state);
            }
        }


        protected TypedLoader RecordLoader
           (RecordCreationExpr expr, ILGenerator ilGenerator, CodeGenerationState state)
            => TypedLoader.FromRecordCreation(expr, ilGenerator, state, Factory); 
    }
}
