using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TinyLang.CLI.Types;
using TinyLang.Compiler.Core;

namespace TinyLang.CLI
{
    public class TinyCLI
    {
        public static string Input()
        {
            CILAssemblyCreator.Print("Hello");
            Print("$ ");
            return Console.ReadLine();
        }

        public static void Print(ICommandResult result) =>
            (result switch
            {
                { Type: CommandResultType.Info, IsEmpty: false } => () => Print(result.Message),
                { Type: CommandResultType.Error, IsEmpty: false } => () => Print(result.Message, ConsoleColor.Red),
                _ => (Action)(() => { })
            })();

        public static void Print(string message, ConsoleColor color = ConsoleColor.DarkGray, bool newLine = false)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            (newLine ? (Action<string>)Console.WriteLine : Console.Write)(message);
            Console.ForegroundColor = previousColor;
        }
        public static void Run()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Print("TinyLang Command Line Interface", color: ConsoleColor.Green, newLine: true);
            while (true)
            {
                Print(CommandParser.Parse(Input()).Execute());
                Task.Delay(1);
            }
        }

        public static void Clear()
        {
            Print("TinyLang Command Line Interface", color: ConsoleColor.Green, newLine: true);
            Console.Clear();
        }

        public static void Exit()
        {
            Environment.Exit(1);
        }
    }
}
