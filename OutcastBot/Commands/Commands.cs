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
        [Command("bug"), Description("Report a bug or give feedback about the bot")]
        [Aliases("feedback")]
        public async Task ReportBug(CommandContext context)
        {
            await context.RespondAsync("Bugs and feedback are tracked on GitHub. <https://github.com/evanronnei/OutcastBot/issues>");
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
                build.PatchVersion = await BuildHelper.ValidatePatchVersion(context, message.Content);
            }
            else if (message == null || build.PatchVersion == null)
            {
                await context.RespondAsync("Command Timeout");
                return;
            }

            // Title
            await context.RespondAsync("(REQUIRED) What is the title of your build? (246 characters maximum)");
            message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));
            if (message != null)
            {
                build.Title = await BuildHelper.ValidateTitle(context, message.Content);
            }
            else if (message == null || build.Title == null)
            {
                await context.RespondAsync("Command Timeout");
                return;
            }

            // HeaderImageUrl
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
                build.BuildUrl = await BuildHelper.ValidateBuildUrl(context, message.Content);
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
                build.ForumUrl = await BuildHelper.ValidateForumUrl(context, message.Content);
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
                build.VideoUrl = BuildHelper.ValidateVideoUrl(message.Content);
            }
            else if (message == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            // Description
            await context.RespondAsync("(REQUIRED) What is the description of your build? (2048 characters maximum)");
            message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(10));
            if (message != null)
            {
                build.Description = await BuildHelper.ValidateDescription(context, message.Content);
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
                build.Tags = BuildHelper.ValidateTags(context, message.Content);
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

            if (builds.Count == 0)
            {
                await context.RespondAsync("You have no builds to edit");
                return;
            }

            var editList = new StringBuilder();
            for (int i = 0; i < builds.Count(); i++)
            {
                editList.AppendLine($"**{i}** - [{builds[i].PatchVersion}] {builds[i].Title}");
            }
            var embed = new DiscordEmbed() { Description = editList.ToString() };
            await context.RespondAsync("Which build would you like to edit?", false, embed);

            var message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));

            var build = new Build();
            if (message != null)
            {
                var index = await BuildHelper.ValidateIndex(context, message.Content, builds.Count);
                if (index == null)
                {
                    await context.RespondAsync("Command Timeout");
                    return;
                }
                build = builds[(int)index];
            }

            await BuildHelper.EditProperty(context, build);

            var channel = context.Guild.Channels.FirstOrDefault(ch => ch.Name == "builds");
            if (channel == null) return;

            var msg = await channel.GetMessageAsync(build.MessageId);
            await msg.EditAsync("", await build.GetEmbed());

            using (var db = new BuildContext())
            {
                db.Builds.Update(build);
                await db.SaveChangesAsync();
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

            if (builds.Count == 0)
            {
                await context.RespondAsync("You have no builds to delete");
                return;
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

        [Command("top"), Description("Shows the top builds")]
        public async Task TopBuilds(CommandContext context, [Description("Number of builds (10 max)")]int count = 5)
        {
            if (count > 10 || count < 1)
            {
                await context.RespondAsync("Invalid input");
                return;
            }

            var builds = new List<Build>();
            using (var db = new BuildContext())
            {
                builds = db.Builds.OrderByDescending(b => b.UpVotes).ThenBy(b => b.DownVotes).Take(count).ToList();
            }

            var buildList = new StringBuilder();
            for (int i = 1; i <= builds.Count; i++)
            {
                var build = builds[i - 1];
                var author = await Program.Client.GetUserAsync(build.AuthorId);
                buildList.AppendLine($"{i}. (+{build.UpVotes} | -{build.DownVotes}) [{build.PatchVersion}] {build.Title} by {author.Username} - {build.BuildUrl}");
            }

            var embed = new DiscordEmbed()
            {
                Title = $"Top {builds.Count} build(s)",
                Description = buildList.ToString()
            };
            await context.RespondAsync("", false, embed);
        }
    }
}
