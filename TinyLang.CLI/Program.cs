using System;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using TinyLang.CLI.Types;
using TinyLang.Compiler.Core;
using TinyLang.Compiler.Core.CodeGeneration;
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
                    record User(name: str, age: int)
                    
                    u = new User(""Vlad"", 20)

                    print(u)
                    ");

            var state = CodeGenerationState.BeginCodeGeneration("test", "testM");

            foreach (var expr in parsed) 
            {
                CodeGeneratorFactory.For(expr.GetType()).Generate(expr, state);
            }
            state.MainMethodBuilder.GetILGenerator().Emit(OpCodes.Ret);
            
            state.ModuleBuilder.CreateGlobalFunctions();

            state.ModuleBuilder.GetMethod("main").Invoke(null, new object[0]);

            // parsed.ToList().ForEach(Console.WriteLine);
        }

    }
}
