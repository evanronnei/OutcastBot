using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using OutcastBot.CommandHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot
{
    public class Commands
    {
        [Command("hi")]
        public async Task Hi(CommandContext c)
        {
            var interactivity = c.Client.GetInteractivityModule();
            await c.RespondAsync("sup");
            var msg = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == c.User.Id && xm.Content.ToLower() == "how are you?", TimeSpan.FromMinutes(1));
            if (msg != null) await c.RespondAsync("I'm fine, thank you!");
        }

        [Command("newbuild")]
        public async Task NewBuild(CommandContext c, string buildUrl)
        {
            var interactivity = c.Client.GetInteractivityModule();

            var build = new Build()
            {
                Author = c.User
            };

            //if ((build.Url = helper.GetBuildUrl(buildUrl)) == null) {
            //    await c.RespondAsync("Invalid grimtools URL");
            //    return;
            //}

            build.Url = NewBuildHelper.GetBuildUrl(buildUrl);
            //await c.RespondAsync(build.Url);

            await c.RespondAsync("(REQUIRED) What is the title of your build? (100 characters maximum)");
            var message = await interactivity.WaitForMessageAsync(m => m.Author.Id == c.User.Id, TimeSpan.FromMinutes(5));
            if (message != null)
            {
                build.Title = NewBuildHelper.GetTitle(message.Content);
                
                await c.RespondAsync(build.Title);

                //await c.RespondAsync("(OPTIONAL) Do you have a forum link for your build? Type \"No\" to skip this step");
                //message = await interactivity.WaitForMessageAsync(m => m.Author.Id == c.User.Id, TimeSpan.FromMinutes(5));
                //if (message != null)
                //{
                //    build.ForumUrl = helper.GetForumUrl(message.Content);

                //    await c.RespondAsync(build.ForumUrl);
                //}
            }
        }
    }
}
