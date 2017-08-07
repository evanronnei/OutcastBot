using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot
{
    class Commands
    {
        [Command("hi")]
        public async Task Hi(CommandContext c)
        {
            await c.RespondAsync("sup");
        }

        [Command("newbuild")]
        public async Task NewBuild(CommandContext c, string buildUrl)
        {
            var build = new Build()
            {
                Author = c.User
            };

            var grimtoolsPattern = new Regex(@"(?<=grimtools.com/calc/)(\w|\d){8}");

            if (grimtoolsPattern.Match(buildUrl).Success)
            {
                build.Url = $"http://www.grimtools.com/calc/{grimtoolsPattern.Match(buildUrl).Value}";
                await c.RespondAsync($"valid grimtools url {grimtoolsPattern.Match(buildUrl).Value}");
            }
        }
    }
}
