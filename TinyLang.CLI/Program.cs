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
                    type User(name: str, age: int)
                    type ValidatedUserResult(user: User, isValidated: bool)
                    
                    func valid(user: User) => new ValidatedUserResult(user, true)

                    func invalid(user: User) => new ValidatedUserResult(user, false)

                    func validateAge(user: User)
                    {
                        return user.age > 18 ? valid(user) : invalid(user)
                    }
                    
                    u = new User(""Vlad"", 20)

                    print(validateAge(u))
                    ";

            var lambdaEx = @"
                    add = (a: int, b: int) => a + b;

                    func binaryOp(a: int, b: int, op: (int, int)->int) => op(a, b)

                    res = binaryOp(2, 2, add)
                    print(res)
                    ";

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
