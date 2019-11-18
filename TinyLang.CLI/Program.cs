using System;
using System.Linq;
using System.Threading.Tasks;
using TinyLang.CLI.Types;
using TinyLang.Compiler.Core;
using TinyLang.Compiler.Core.Parsing;

namespace TinyLang.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            //TinyCLI.Run();
            var parsed = TinyInteractive.Parser.Parse(
                @"
                    record User(name: str)

                    func GetUserByName(name: str): User
                    {
                       x = new User(""Vlad"")
                    }

                    if(true)
                    {   
                        user = GetUserByName(x > 2 ? ""Vlad"" : ""Test"")
                        print(user)
                    }
        
                    ");
            parsed.ToList().ForEach(Console.WriteLine);
        }

    }
}
