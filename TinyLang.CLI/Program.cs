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
            var funcExample = @"
                    record User(name: str, type: str)
                    func createAdmin(name: str)
                    {
                        return new User(name, ""admin"")
                    }
                    
                    u = createAdmin(""Vlad"")

                    print(u)
                    ";

            var typeExample = @"
                    record User(name: str, type: str)
                   
                    u = new User(""Vlad"", ""admin"")

                    print(u)
                    ";

            //TinyCLI.Run();
            var parsed = TinyInteractive.Parser.Parse(funcExample);

            var state = CodeGenerationState.BeginCodeGeneration("test", "testM");

            foreach (var expr in parsed) 
            {
                CodeGeneratorsFactory.Instance.GeneratorFor(expr.GetType()).Generate(expr, state);
            }
            state.MainMethodBuilder.GetILGenerator().Emit(OpCodes.Ret);
            
            state.ModuleBuilder.CreateGlobalFunctions();

            state.ModuleBuilder.GetMethod("main").Invoke(null, new object[0]);

            // parsed.ToList().ForEach(Console.WriteLine);
        }

    }
}
