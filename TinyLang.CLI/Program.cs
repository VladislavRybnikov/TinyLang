﻿using System;
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
                @"x = 5
                     y = x + 1
                     if(x == y){
                        z=x + y
                        a = z + x + y
                     }
                     else{
                        if(true){
                            x = x + 1
                         }
                        while(true){
                            if(false)
                            {
                            }
                        }
                     }");
            parsed.ToList().ForEach(Console.WriteLine);
        }

    }
}
