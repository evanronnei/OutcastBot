using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using OutcastBot.Commands.CommandHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Commands
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
    }

    [Group("build")]
    public class BuildCommands
    {
        [Command("new")]
        public async Task NewBuild(CommandContext context)
        {
            var interactivity = context.Client.GetInteractivityModule();

            var build = new Build()
            {
                AuthorId = context.User.Id,
                UpVotes = 0,
                DownVotes = 0
            };

            // PatchVersion
            await context.RespondAsync("(REQUIRED) What patch is this build from? (i.e. 1.0.1.1)");
            var message = await interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (message != null)
            {
                build.PatchVersion = await NewBuildHelper.GetPatchVersion(context, message.Content);
            }
            else if (message == null || build.PatchVersion == null)
            {
                await context.RespondAsync("Command Timeout");
                return;
            }

            // Title
            await context.RespondAsync("(REQUIRED) What is the title of your build? (100 characters maximum)");
            message = await interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));
            if (message != null)
            {
                build.Title = await NewBuildHelper.GetTitle(context, message.Content);
            }
            else if (message == null || build.Title == null)
            {
                await context.RespondAsync("Command Timeout");
                return;
            }

            // HeaderImage
            await context.RespondAsync("(OPTIONAL) Do you have a header image for your build? (Upload attachment) Type \"No\" to skip this step.");
            message = await interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));
            if (message != null && message.Content.ToLower() != "no")
            {
                build.HeaderImageUrl = message.Attachments[0].Url;
            }
            else if (message == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            // BuildUrl
            await context.RespondAsync("(REQUIRED) What is the grimtools URL for your build?");
            message = await interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (message != null)
            {
                build.BuildUrl = await NewBuildHelper.GetBuildUrl(context, message.Content);
            }
            else if (message == null || build.BuildUrl == null)
            {
                await context.RespondAsync("Command Timeout");
                return;
            }

            // ForumUrl
            await context.RespondAsync("(OPTIONAL) Do you have a forum link for your build? Type \"No\" to skip this step.");
            message = await interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (message != null && message.Content.ToLower() != "no")
            {
                build.ForumUrl = await NewBuildHelper.GetForumUrl(context, message.Content);
            }
            else if (message == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            // VideoUrl
            await context.RespondAsync("(OPTIONAL) Do you have a video link for your build? Type \"No\" to skip this step.");
            message = await interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (message != null && message.Content.ToLower() != "no")
            {
                build.VideoUrl = NewBuildHelper.GetVideoUrl(message.Content);
            }
            else if (message == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            // Description
            await context.RespondAsync("(REQUIRED) What is the description of your build? (1000 characters maximum)");
            message = await interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(5));
            if (message != null)
            {
                build.Description = await NewBuildHelper.GetDescription(context, message.Content);
            }
            else if (message == null || build.Description == null)
            {
                await context.RespondAsync("Command Timeout");
                return;
            }

            // Tags
            await context.RespondAsync("(OPTIONAL) Would you like to add any tags to your build? (Emojis) Separate each emoji with a space. Type \"No\" to skip this step.");
            message = await interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (message != null && message.Content.ToLower() != "no")
            {
                build.Tags = NewBuildHelper.GetTags(context, message.Content);
            }
            else if (message == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            // Post Build
            await NewBuildHelper.PostBuild(context, build);
        }
    }
}
