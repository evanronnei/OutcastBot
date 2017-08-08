using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.CommandHelpers
{
    class NewBuildHelper
    {
        private CommandContext commandContext;

        public NewBuildHelper(CommandContext c)
        {
            commandContext = c;
        }

        public async Task GetBuildUrl(string buildUrl, Build build)
        {
            var grimtoolsRegex = new Regex(@"(?<=grimtools.com/calc/)(\w|\d){8}");
            var match = grimtoolsRegex.Match(buildUrl);

            if (match.Success)
            {
                build.Url = $"http://www.grimtools.com/calc/{match.Value}";
                await commandContext.RespondAsync($"What would you like the title of your build to be? (REQUIRED)");
            }
        }
    }
}
