using System;
using System.Collections.Generic;
using System.Text;
using TinyLang.CLI.Types;

namespace TinyLang.CLI
{
    public class CommandFactory
    {
        public static ITinyCommand Create(CommandType type, IReadOnlyDictionary<string, string> args = null)
            => type switch
            {
                CommandType.Clear => (ITinyCommand) new ClearCommand(),
                CommandType.Build => new BuildCommand(args),
                CommandType.Exit => new ExitCommand(),
                CommandType.Config => new ConfigCommand(args),
                _ => null
            };
    }
}
