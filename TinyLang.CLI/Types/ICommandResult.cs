using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.CLI.Types
{
    public enum CommandResultType
    {
        Info, Error
    }

    public interface ICommandResult
    {
        CommandResultType Type { get; set; }

        string Message { get; set; }
        bool IsEmpty { get; }
    }
}
