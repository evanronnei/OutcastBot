using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
