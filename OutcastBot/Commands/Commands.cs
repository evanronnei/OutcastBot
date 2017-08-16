using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
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
            build.PatchVersion = await BuildHelper.GetPatchVersion(context);
            if (build.PatchVersion == null) return;

            // Title
            build.Title = await BuildHelper.GetTitle(context);
            if (build.Title == null) return;

            // Description
            build.Description = await BuildHelper.GetDescription(context);
            if (build.Description == null) return;

            // BuildUrl
            build.BuildUrl = await BuildHelper.GetBuildUrl(context);
            if (build.BuildUrl == null) return;

            // ImageUrl
            build.ImageUrl = await BuildHelper.GetImageUrl(context);

            // ForumUrl
            build.ForumUrl = await BuildHelper.GetForumUrl(context);

            // VideoUrl
            build.VideoUrl = await BuildHelper.GetVidoeUrl(context);

            // Tags
            build.Tags = await BuildHelper.GetTags(context);

            // Post Build
            await NewBuildHelper.PostBuild(context, build);
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
            var message = await context.RespondAsync("Which build would you like to edit?", false, embed);

            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));

            var build = new Build();
            if (response != null)
            {
                var index = await CommandHelper.ValidateIndex(context, response.Content, builds.Count);
                if (index == null)
                {
                    await context.RespondAsync("Command Timeout");
                    return;
                }
                build = builds[(int)index];
            }

            await EditBuildHelper.EditProperty(context, build);

            var channel = context.Guild.Channels.FirstOrDefault(ch => ch.Name == "builds");
            if (channel == null) return;

            var msg = await channel.GetMessageAsync(build.MessageId);
            await msg.EditAsync("", await build.GetEmbed());

            using (var db = new BuildContext())
            {
                db.Builds.Update(build);
                await db.SaveChangesAsync();
            }

            await message.DeleteAsync();
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

            var deleteList = new StringBuilder();
            for (int i = 0; i < builds.Count(); i++)
            {
                deleteList.AppendLine($"**{i}** - [{builds[i].PatchVersion}] {builds[i].Title}");
            }
            var embed = new DiscordEmbed() { Description = deleteList.ToString() };
            var message = await context.RespondAsync("Which build would you like to delete?", false, embed);

            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (response != null)
            {
                var index = await CommandHelper.ValidateIndex(context, response.Content, builds.Count);
                if (index == null)
                {
                    await context.RespondAsync("Command Timeout");
                    return;
                }

                var build = builds[(int)index];

                var delete = await context.Guild.Channels.FirstOrDefault(c => c.Name == "builds")
                    .GetMessageAsync(build.MessageId);
                await delete.DeleteAsync();

                await context.RespondAsync($"Deleted build **[{build.PatchVersion}] {build.Title}**");
            }
            else
            {
                await context.RespondAsync("Command Timeout");
            }

            await message.DeleteAsync();
        }

        [Command("top"), Description("Shows the top builds")]
        public async Task TopBuilds(CommandContext context, [Description("Number of builds (10 max).")]int count = 5)
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

            var message = new StringBuilder();
            for (int i = 1; i <= builds.Count; i++)
            {
                var build = builds[i - 1];
                var author = await Program.Client.GetUserAsync(build.AuthorId);
                message.AppendLine($"{i}. (+{build.UpVotes} | -{build.DownVotes}) [{build.PatchVersion}] {build.Title} by {author.Username} - {build.BuildUrl}");
                await Task.Delay(100);
            }

            var embed = new DiscordEmbed()
            {
                Title = $"Top {builds.Count} build(s)",
                Description = message.ToString()
            };

            await context.RespondAsync("", false, embed);
        }
    }
}
