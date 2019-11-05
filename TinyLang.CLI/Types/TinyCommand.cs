using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLang.CLI.Types
{
    public abstract class TinyCommand : ITinyCommand
    {
        public CommandType CommandType { get; }

        public IReadOnlyDictionary<CommandArgument, string> Arguments { get; }
        public abstract ICommandResult Execute();

        public TinyCommand(CommandType type, IReadOnlyDictionary<CommandArgument, string> args) { }

        public TinyCommand(CommandType type) { }
    }

    public class BuildCommand : TinyCommand
    {
        public BuildCommand(IReadOnlyDictionary<CommandArgument, string> args) : base(Types.CommandType.Build, args) { }

        public override ICommandResult Execute()
        {
            throw new NotImplementedException();
        }
    }

    public class ClearCommand : TinyCommand
    {
        public ClearCommand() : base(CommandType.Clear) { }


        public override ICommandResult Execute()
        {
            TinyCLI.Clear();
            return CommandResult.Empty;
        }
    }

    public class ExitCommand : TinyCommand
    {
        public ExitCommand() : base(CommandType.Exit)
        {
        }

        public override ICommandResult Execute()
        {
            TinyCLI.Exit();
            return CommandResult.Empty;
        }
    }
}
