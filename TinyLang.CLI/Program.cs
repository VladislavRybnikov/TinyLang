using System;
using System.Linq;
using TinyLang.Compiler.Core;
using TinyLang.Compiler.Core.CodeGeneration;
using TinyLang.Compiler.Core.Parsing;
using TinyLang.Compiler.Core.Parsing.Expressions;
using static TinyLang.Compiler.Core.TinyLanguage;

namespace TinyLang.CLI
{
    class Program
    {
        static ICompiler Compiler = TinyCompiler.Create(
                new ParserBuilder<Expr>(GetExprValueParser, TokenParser.ReservedOp),
                new ExprTokenizer(),
                CodeGeneratorsFactory.Instance);

        static void Main(string[] args)
        {
            var funcExample = @"
                    type User(name: str, age: int)
                    type ValidatedUserResult(user: User, isValidated: bool)
                    
                    func valid(user: User) => new ValidatedUserResult(user, true)

                    func invalid(user: User) => new ValidatedUserResult(user, false)

                    func validateAge(user: User) => user.age > 18 ? valid(user) : invalid(user)

                    u = new User(""Vlad"", 20)

                    print(validateAge(u))
                    ";

            // 1) lambda functions
            var lambdaSimple = @"
                    unit = (x: int) => x
                    add = (x: int, y: int) => x + y
                   
                    print(add(2, 2))
                    ";

            var lambdaEx = @"
                    add = (a: int, b: int) => a + b;

                    func binaryOp(a: int, b: int, op: (int, int)->int) => op(a, b)

                    res = binaryOp(2, 2, add)
                    print(res)
                    ";

            // 2) type members

            // 3) ADT + Generics
            var adtExample = @"
                    type A = int * int
                    type B = int | int  

                    type IntAlias = int

                    type Option<T> = 
                        Some: T
                        | None

                    none = Option.None

                    some = Option.Some(5)
                    ";

            var exceptionEx = @"
                    func ifF(t: int) 
                    {
                        if(t)
                        {
                            print(""ok"")
                        }
                        
                        return t
                    }
                    
                    print(ifF(123))
                    ";

            Compiler
                .WithAssemblyName("test")
                .WithCodeSource(exceptionEx, SourceType.String)
                .Run(out var ast);

            Console.WriteLine(string.Concat(Enumerable.Repeat("-", 20)));
            Console.WriteLine("AST");
            Console.WriteLine(ast);
        }

    }
}
