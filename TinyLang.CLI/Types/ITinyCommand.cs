using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.CLI.Types
{
    public interface ITinyCommand
    {
        CommandType CommandType { get; }

        IReadOnlyDictionary<string, string> Arguments { get; }

        ICommandResult Execute();
    }
}
