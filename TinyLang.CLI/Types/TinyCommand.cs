using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace TinyLang.CLI.Types
{
    public abstract class TinyCommand : ITinyCommand
    {
        public CommandType CommandType { get; }

        public IReadOnlyDictionary<string, string> Arguments { get; }

        public virtual HashSet<string> AllowedArguments { get; } = null;

        public abstract ICommandResult Execute();

        public TinyCommand(CommandType type, IReadOnlyDictionary<string, string> args)
        {
            Arguments = args;
            CommandType = type;
        }

        public TinyCommand(CommandType type)
        {
            CommandType = type;
        }
    }

    public class BuildCommand : TinyCommand
    {
        public BuildCommand(IReadOnlyDictionary<string, string> args) : base(Types.CommandType.Build, args) { }

        public override HashSet<string> AllowedArguments { get; } = new HashSet<string> { "path", "output" };

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

    public class ConfigCommand : TinyCommand
    {
        public ConfigCommand(IReadOnlyDictionary<string, string> args) : base(CommandType.Config, args)
        {
        }
        private static bool IsValidValue(string s, string expected) => s == expected || expected.StartsWith(s);

        public override ICommandResult Execute()
        {
            var arg = Arguments.FirstOrDefault();

            (arg switch
            {
                var (k, v) when IsValidValue(k, "mode") && IsValidValue(v, "bash")  
                                 => () => TinyCLI.SetMode(TinyCliMode.Bash),
                var (k, v) when IsValidValue(k, "mode") && IsValidValue(v, "interactive")
                                => () => TinyCLI.SetMode(TinyCliMode.Interactive),
                _ => (Action) (() => TinyCLI.Print("Wrong arguments"))
            })();

            return CommandResult.Empty;
        }
    }
}
