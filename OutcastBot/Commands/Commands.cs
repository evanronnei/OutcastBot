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

    [Group("build"), Description("Commands for interacting with builds")]
    public class BuildCommands
    {
        [Command("new"), Description("Create a new build")]
        public async Task NewBuild(CommandContext context)
        {
            var build = new Build()
            {
                AuthorId = context.User.Id,
                UpVotes = 0,
                DownVotes = 0
            };

            // PatchVersion
            await context.RespondAsync("(REQUIRED) What patch is this build from? (i.e. 1.0.1.1)");
            var message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (message != null)
            {
                build.PatchVersion = await BuildHelper.GetPatchVersion(context, message.Content);
            }
            else if (message == null || build.PatchVersion == null)
            {
                await context.RespondAsync("Command Timeout");
                return;
            }

            // Title
            await context.RespondAsync("(REQUIRED) What is the title of your build? (100 characters maximum)");
            message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));
            if (message != null)
            {
                build.Title = await BuildHelper.GetTitle(context, message.Content);
            }
            else if (message == null || build.Title == null)
            {
                await context.RespondAsync("Command Timeout");
                return;
            }

            // HeaderImage
            await context.RespondAsync("(OPTIONAL) Do you have a header image for your build? (Upload attachment) Type \"No\" to skip this step.");
            message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));
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
            message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (message != null)
            {
                build.BuildUrl = await BuildHelper.GetBuildUrl(context, message.Content);
            }
            else if (message == null || build.BuildUrl == null)
            {
                await context.RespondAsync("Command Timeout");
                return;
            }

            // ForumUrl
            await context.RespondAsync("(OPTIONAL) Do you have a forum link for your build? Type \"No\" to skip this step.");
            message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (message != null && message.Content.ToLower() != "no")
            {
                build.ForumUrl = await BuildHelper.GetForumUrl(context, message.Content);
            }
            else if (message == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            // VideoUrl
            await context.RespondAsync("(OPTIONAL) Do you have a video link for your build? Type \"No\" to skip this step.");
            message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (message != null && message.Content.ToLower() != "no")
            {
                build.VideoUrl = BuildHelper.GetVideoUrl(message.Content);
            }
            else if (message == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            // Description
            await context.RespondAsync("(REQUIRED) What is the description of your build? (1000 characters maximum)");
            message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(5));
            if (message != null)
            {
                build.Description = await BuildHelper.GetDescription(context, message.Content);
            }
            else if (message == null || build.Description == null)
            {
                await context.RespondAsync("Command Timeout");
                return;
            }

            // Tags
            await context.RespondAsync("(OPTIONAL) Would you like to add any tags to your build? (Emojis) Separate each emoji with a space. Type \"No\" to skip this step.");
            message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (message != null && message.Content.ToLower() != "no")
            {
                build.Tags = BuildHelper.GetTags(context, message.Content);
            }
            else if (message == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            // Post Build
            await BuildHelper.PostBuild(context, build);
        }

        [Command("edit"), Description("Edit an existing build")]
        public async Task EditBuild(CommandContext context)
        {
            var builds = new List<Build>();
            using (var db = new BuildContext())
            {
                builds = db.Builds.Where(b => b.AuthorId == context.User.Id).ToList();
            }

            var editList = "Which build would you like to edit?";
            for (int i = 0; i < builds.Count(); i++)
            {
                editList += $"\n{i} - **[{builds[i].PatchVersion}] {builds[i].Title}**";
            }
            await context.RespondAsync(editList);

            var message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (message != null)
            {
                var index = await BuildHelper.ValidateIndex(context, message.Content, builds.Count);
                if (index == null)
                {
                    await context.RespondAsync("Command Timeout");
                    return;
                }

                await context.RespondAsync("Which property would you like to edit?");
            }
        }

        [Command("delete"), Description("Delete an existing build")]
        public async Task DeleteBuild(CommandContext context)
        {
            var builds = new List<Build>();
            using (var db = new BuildContext())
            {
                builds = db.Builds.Where(b => b.AuthorId == context.User.Id).ToList();
            }

            var deleteList = "Which build would you like to delete?";
            for (int i = 0; i < builds.Count(); i++)
            {
                deleteList += $"\n{i} - **[{builds[i].PatchVersion}] {builds[i].Title}**";
            }
            await context.RespondAsync(deleteList);

            var message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (message != null)
            {
                var index = await BuildHelper.ValidateIndex(context, message.Content, builds.Count);
                if (index == null)
                {
                    await context.RespondAsync("Command Timeout");
                    return;
                }

                var build = builds[(int)index];

                var delete = await context.Guild.Channels.FirstOrDefault(c => c.Name == "builds")
                    .GetMessageAsync(build.MessageId);
                await delete.DeleteAsync();

                using (var db = new BuildContext())
                {
                    db.Builds.Remove(build);
                    await db.SaveChangesAsync();
                }

                await context.RespondAsync($"Deleted build **[{build.PatchVersion}] {build.Title}**");
            }
            else
            {
                await context.RespondAsync("Command Timeout");
                return;
            }
        }
    }
}
