using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.CLI.Types
{
    public enum CommandType
    {
        Build, Run, Exit, Clear
    }

    public enum CommandArgument
    {
        Path, Output
    }
}
