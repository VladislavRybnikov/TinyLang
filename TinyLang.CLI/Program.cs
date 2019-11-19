using System;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using TinyLang.CLI.Types;
using TinyLang.Compiler.Core;
using TinyLang.Compiler.Core.CodeGeneration;
using TinyLang.Compiler.Core.Parsing;
using TinyLang.Compiler.Core.Parsing.Expressions;
using static TinyLang.Compiler.Core.ExpressionEvaluator;
using static TinyLang.Compiler.Core.TinyLanguage;

namespace TinyLang.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var funcExample = @"
                    record User(name: str, type: str)
                    func createAdmin(name: str)
                    {
                        return new User(name, ""admin"")
                    }
                    
                    u = createAdmin(""Vlad"")

                    print(u)
                    ";

            var typeExample = @"
                    record User(name: str, type: str)
                   
                    u = new User(""Vlad"", ""admin"")

                    print(u)
                    ";

            //TinyCLI.Run();
            TinyCompiler.Create(
                new ParserBuilder<Expr>(GetExprValueParser, TokenParser.ReservedOp),
                new ExprTokenizer(),
                CodeGeneratorsFactory.Instance)
                .WithAssemblyName("test")
                .WithCodeSource(funcExample, SourceType.String)
                .Run();

        }

    }
}
