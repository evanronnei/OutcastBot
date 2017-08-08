using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using OutcastBot.CommandHelpers;
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
            var interactivity = c.Client.GetInteractivityModule();
            var helper = new NewBuildHelper(c);

            var build = new Build()
            {
                Author = c.User
            };

            await helper.GetBuildUrl(buildUrl, build);

            var message = await interactivity.WaitForMessageAsync(m => m.Author.Id == c.User.Id, TimeSpan.FromMinutes(5));

            if (message != null)
            {
                build.Title = message.Content;
                await c.RespondAsync("Do you have a forum post for your build? (OPTIONAL. Type \"No\" to skip this step)");
            }

            message = await interactivity.WaitForMessageAsync(m => m.Author.Id == c.User.Id, TimeSpan.FromMinutes(5));

            if (message != null)
            {
                if (!message.Content.ToLower().StartsWith("no"))
                {
                    var gdForumsRegex = new Regex(@"(?=<grimdawn.com/forums/showthread.php?t=)\d*");
                } 
            }
        }
    }
}
