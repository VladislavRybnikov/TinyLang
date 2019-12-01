using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Text;
using LanguageExt.Parsec;
using TinyLang.Compiler.Core.CodeGeneration;
using TinyLang.Compiler.Core.Parsing;
using TinyLang.Compiler.Core.Parsing.Expressions;
using Expr = TinyLang.Compiler.Core.Parsing.Expressions.Expr;

namespace TinyLang.Compiler.Core
{
    public enum SourceType 
    {
        String, File
    }

    public interface ICompiler
    {
        ICompiler WithCodeSource(string source, SourceType sourceType);
        ICompiler WithAssemblyName(string name);

        void Run();
    }

    public class TinyCompiler : ICompiler
    {
        private readonly IExprParser _parser;
        private readonly ICodeGeneratorsFactory _codeGeneratorsFactory;

        private string assemblyName, source;

        private SourceType sourceType;

        private TinyCompiler(IExprParser parser, ICodeGeneratorsFactory codeGeneratorsFactory) {
            _parser = parser;
            _codeGeneratorsFactory = codeGeneratorsFactory;
        }

        public static ICompiler Create
        (
            IParserBuilder<Expr> parserBuilder,
            ITokenizer<Expr> tokenizer,
            ICodeGeneratorsFactory codeGeneratorsFactory
        )
        {
            return new TinyCompiler(new ExprParser(parserBuilder, tokenizer), codeGeneratorsFactory);
        }

        public void Run()
        {
            var code = sourceType == SourceType.String ? source : File.ReadAllText(source);

            var parsed = _parser.Parse(code);

            var state = CodeGenerationState.BeginCodeGeneration(assemblyName, $"{assemblyName}.Module");
            foreach (var expr in parsed)
            {
                _codeGeneratorsFactory.GeneratorFor(expr.GetType()).Generate(expr, state);
            }
            state.MainMethodBuilder.GetILGenerator().Emit(OpCodes.Ret);
            state.ModuleBuilder.CreateGlobalFunctions();

            state.ModuleBuilder.GetMethod("main").Invoke(null, new object[0]);
        }

        public ICompiler WithAssemblyName(string name)
        {
            assemblyName = name;
            return this;
        }

        public ICompiler WithCodeSource(string source, SourceType sourceType)
        {
            this.source = source;
            this.sourceType = sourceType;
            return this;
        }
    }
}
