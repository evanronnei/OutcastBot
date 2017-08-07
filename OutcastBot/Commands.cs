﻿using DSharpPlus.CommandsNext;
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
            var grimtoolsPattern = new Regex(@"(?<=grimtools.com/calc/)(\w|\d){8}");

            if (grimtoolsPattern.Match(buildUrl).Success)
            {
                await c.RespondAsync($"valid grimtools url {grimtoolsPattern.Match(buildUrl).Value}");
            }
        }
    }
}
