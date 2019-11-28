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
            var shouldBeFixed = @"
                    type User(name: str, age: int)
                    type ValidatedUserResult(user: User, isValidated: bool)
                    
                    func valid(user: User)
                    {
                        return new ValidatedUserResult(user, true)
                    }

                    func invalid(user: User)
                    {
                        return new ValidatedUserResult(user, false)
                    }

                    func validateAge(user: User)
                    {
                        return user.age > 18 ? valid(user) : invalid(user)
                    }
                    
                    u = new User(""Vlad"", 20)

                    print(validateAge(u))
                    ";

            //var samplePath = @"C:\Users\Vladyslav_Rybnikov\source\repos\TinyLang\Examples\Sample01.tl";

            //TinyCLI.Run();
            TinyCompiler.Create(
                new ParserBuilder<Expr>(GetExprValueParser, TokenParser.ReservedOp),
                new ExprTokenizer(),
                CodeGeneratorsFactory.Instance)
                .WithAssemblyName("test")
                //.WithCodeSource(samplePath, SourceType.File)
                .WithCodeSource(shouldBeFixed, SourceType.String)
                .Run();
        }

    }
}
