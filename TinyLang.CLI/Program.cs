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
                        
                        if(true)
                        {
                            adminType = ""admin1""
                        }
                        elif(true)
                        {
                            adminType = ""admin2""
                        }
                        else
                        {
                            adminType = ""admin3""
                        }

                        result = new User(name, adminType, age)

                        return result
                    }
                    
                    u = createAdmin(""Vlad"", 20)

                    print(u)
                    ";

            var addExample = @"
a = 123
printF(""a = {0}"", 123)
";

            var checkAgeExample = @"
func checkAge(age: int)
{
    msg = ""Age is not 18""

    if(age >= 18)
    {
        msg = ""Age >= 18!""
    }

    return msg
}

print(checkAge(23))
";

            //var samplePath = @"C:\Users\Vladyslav_Rybnikov\source\repos\TinyLang\Examples\Sample01.tl";

            //TinyCLI.Run();
            TinyCompiler.Create(
                new ParserBuilder<Expr>(GetExprValueParser, TokenParser.ReservedOp),
                new ExprTokenizer(),
                CodeGeneratorsFactory.Instance)
                .WithAssemblyName("test")
                //.WithCodeSource(samplePath, SourceType.File)
                .WithCodeSource(addExample, SourceType.String)
                .Run();

        }

    }
}
