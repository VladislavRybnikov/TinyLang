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
                @"x: str = ""hello""");
            parsed.ToList().ForEach(Console.WriteLine);
        }

    }
}
