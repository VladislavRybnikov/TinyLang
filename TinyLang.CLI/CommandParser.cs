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
                (new[] { "exit"}, CommandType.Exit)
            };

        public static ITinyCommand Parse(string input)
        {
            Regex regex = new Regex(@"([\w]*)*( --[\w]*)*( [\w]*)*");

            var match = regex.Match(input);
            var commandType = _aliasesToCmds.FirstOrDefault(x => x.aliases.Any(x => x == match.Groups[0].Value)).command;

            return CommandFactory.Create(commandType);
        }
    }
}
