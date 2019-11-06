using System;
using System.Threading.Tasks;
using TinyLang.CLI.Types;
using TinyLang.Compiler.Core.Parsing;

namespace TinyLang.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            //TinyCLI.Run();
            SingleScriptParser.Parse("a = 4 + b");
        }

    }
}
