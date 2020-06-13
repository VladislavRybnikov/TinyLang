using System;
using System.Linq;
using System.Net.Http;
using TinyLang.Fluent;

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

                    func validateAge(user: User) => user.age > 18 ? valid(user) : invalid(user)

                    u = new User(""Vlad"", 20)

                    print(validateAge(u))
                    ";
            var forEx = "for(x in list) | for(0 to 5 step 1)";
            // 1) lambda functions
            var lambdaEx = @"
                    add = (a: int, b: int) => a + b
                    mul = (a: int, b: int) => a * b

                    func binaryOp(a: int, b: int, op: (int, int)->int) => op(a, b)

                    addRes = binaryOp(3, 2, add)
                    mulRes = binaryOp(3, 2, mul)
                    printF(""3 + 2 = {0}"", addRes)
                    printF(""3 * 2 = {0}"", mulRes)
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

            using var engine = TinyLangEngine
                .FromScript(lambdaEx)
                .AddStatement(st => st.Print("Executed by TinyLangEngine"));

            engine.Execute(out var ast);

            Console.WriteLine(string.Concat(Enumerable.Repeat("-", 20)));
            Console.WriteLine("AST");
            Console.WriteLine(ast);

        }

    }
}
