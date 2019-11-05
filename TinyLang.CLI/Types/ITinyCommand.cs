using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.CLI.Types
{
    public interface ITinyCommand
    {
        CommandType CommandType { get; }

        IReadOnlyDictionary<CommandArgument, string> Arguments { get; }

        ICommandResult Execute();
    }
}
