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

            IReadOnlyDictionary<string, string> dict = null;
            var commandType = GetCommandType(matches[0]);

            try
            {
                dict = GetCommandArgs(matches);
            }
            catch { }

            return CommandFactory.Create(GetCommandType(matches[0]), dict);
        }

        private static CommandType GetCommandType(Match match) 
            => _aliasesToCmds.FirstOrDefault(x => x.aliases.Any(y => match.Groups[0].Value.Contains(y))).command;

        private static IReadOnlyDictionary<string, string> GetCommandArgs(MatchCollection matches) 
            => matches.ToDictionary(k => GetValueOrDefault(k.Groups, 2).Replace("--", string.Empty).Trim(),
                v => GetValueOrDefault(v.Groups, 3).Replace("=", string.Empty).Trim());

        private static string GetValueOrDefault(GroupCollection groups, int index)
        {
            var result = string.Empty;
            try
            {
                result = groups[index].Value;
            }
            finally { }

            return result;
        }
    }
}
