using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.CLI.Types
{
    public class CommandResult : ICommandResult
    {
        public CommandResultType Type { get; set; }
        public string Message { get; set; }
        public bool IsEmpty => string.IsNullOrEmpty(Message);

        public static ICommandResult Info(string message) =>
            new CommandResult {Message = message, Type = CommandResultType.Info};

        public static ICommandResult Empty => new CommandResult();
    }
}
