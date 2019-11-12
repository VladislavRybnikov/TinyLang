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
                "x = 5\n" +
                "y = x + 1\n" +
                "if(x == y){\n" +
                "z=x + y\n" +
                "a = z + x + y}\n" +
                "else{" +
                "if(true){x = x + 1}" + 
                "}\n" );
            parsed.ToList().ForEach(Console.WriteLine);
        }

    }
}
