using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TinyLang.CLI.Types;

namespace TinyLang.CLI
{
    public class CommandParser
    {
        private static IEnumerable<(string[] aliases, CommandType command)> _aliasesToCmds =
            new List<(string[] aliases, CommandType command)>
            {
                (new[] {"clear", "cl"}, CommandType.Clear),
                (new[] { "exit"}, CommandType.Exit),
                (new[] { "config" }, CommandType.Config)
            };

        public static ITinyCommand Parse(string input)
        {
            Regex regex = new Regex(@"([\w]*)*( --[\w]*)*(=[\w]*)*");
            var matches = regex.Matches(input);

            return CommandFactory.Create(GetCommandType(matches[0]), GetCommandArgs(matches));
        }

        private static CommandType GetCommandType(Match match) 
            => _aliasesToCmds.FirstOrDefault(x => x.aliases.Any(x => match.Groups[0].Value.Contains(x))).command;

        private static IReadOnlyDictionary<string, string> GetCommandArgs(MatchCollection matches) 
            => matches.ToDictionary(k => k.Groups[2].Value.Replace("--", string.Empty).Trim(),
                v => v.Groups[3].Value.Replace("=", string.Empty).Trim());
    }
}
