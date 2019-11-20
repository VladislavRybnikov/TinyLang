using TinyLang.Compiler.Core;
using TinyLang.Compiler.Core.CodeGeneration;
using TinyLang.Compiler.Core.Parsing;
using TinyLang.Compiler.Core.Parsing.Expressions;
using static TinyLang.Compiler.Core.TinyLanguage;

namespace TinyLang.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var funcExample = @"
                    type User(name: str, userType: str, age: int)

                    func createAdmin(name: str, age: int)
                    {
                        adminType = ""admin""
                        result = new User(name, adminType, age)

                        return result
                    }
                    
                    u = createAdmin(""Vlad"", 20)

                    print(u)
                    ";
            //var samplePath = @"C:\Users\Vladyslav_Rybnikov\source\repos\TinyLang\Examples\Sample01.tl";

            //TinyCLI.Run();
            TinyCompiler.Create(
                new ParserBuilder<Expr>(GetExprValueParser, TokenParser.ReservedOp),
                new ExprTokenizer(),
                CodeGeneratorsFactory.Instance)
                .WithAssemblyName("test")
                //.WithCodeSource(samplePath, SourceType.File)
                .WithCodeSource(funcExample, SourceType.String)
                .Run();

        }

    }
}
