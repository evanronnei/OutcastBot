using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using OutcastBot.Commands.CommandHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Commands
{
    public class Commands
    {
        [Command("bug"), Description("Report a bug or give feedback on the bot")]
        [Aliases("feedback")]
        public async Task ReportBug(CommandContext context)
        {
            await context.RespondAsync("Bugs and feedback are tracked on GitHub. <https://github.com/evanronnei/OutcastBot/issues>");
        }
    }

    [Group("build", CanInvokeWithoutSubcommand = true), Description("Commands for interacting with builds")]
    public class BuildCommands
    {
        public async Task ExecuteGroupAsync(CommandContext context, [Description("ID of the build to get")]int id)
        {
            var build = new Build();
            using (var db = new BuildContext())
            {
                build = db.Builds.FirstOrDefault(b => b.BuildId == id);
            }

            if (build == null)
            {
                await context.RespondAsync("Invalid build ID");
                return;
            }

            await context.RespondAsync("", false, await build.GetEmbed());
        } 

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
            build.PatchVersion = await BuildHelper.GetPatchVersion(BuildHelper.CommandType.New, context);
            if (build.PatchVersion == null) return;

            // Title
            build.Title = await BuildHelper.GetTitle(BuildHelper.CommandType.New, context);
            if (build.Title == null) return;

            // Description
            build.Description = await BuildHelper.GetDescription(BuildHelper.CommandType.New, context);
            if (build.Description == null) return;

            // BuildUrl
            build.BuildUrl = await BuildHelper.GetBuildUrl(BuildHelper.CommandType.New, context);
            if (build.BuildUrl == null) return;

            // ImageUrl
            build.ImageUrl = await BuildHelper.GetImageUrl(BuildHelper.CommandType.New, context);

            // ForumUrl
            build.ForumUrl = await BuildHelper.GetForumUrl(BuildHelper.CommandType.New, context);

            // VideoUrl
            build.VideoUrl = await BuildHelper.GetVideoUrl(BuildHelper.CommandType.New, context);

            // Tags
            build.Tags = await BuildHelper.GetTags(context);

            // Post Build
            await NewBuildHelper.PostBuild(context, build);
        }

        [Command("edit"), Description("Edit an existing build")]
        public async Task EditBuild(CommandContext context, [Description("ID of the build to edit")]int id)
        {
            var build = new Build();
            using (var db = new BuildContext())
            {
                build = db.Builds.FirstOrDefault(b => b.BuildId == id);
            }

            if (build == null)
            {
                await context.RespondAsync("Invalid build ID");
                return;
            }

            if (build.AuthorId != context.User.Id)
            {
                await context.RespondAsync("This build does not belong to you");
                return;
            }

            await EditBuildHelper.EditProperty(context, build);

            var channel = context.Guild.Channels.FirstOrDefault(ch => ch.Name == "builds");
            if (channel == null) return;

            var buildMessage = await channel.GetMessageAsync(build.MessageId);
            await buildMessage.ModifyAsync("", await build.GetEmbed());

            using (var db = new BuildContext())
            {
                db.Builds.Update(build);
                await db.SaveChangesAsync();
            }
        }

        [Command("delete"), Description("Delete an existing build")]
        public async Task DeleteBuild(CommandContext context, [Description("ID of the build to delete")]int id)
        {
            var build = new Build();
            using (var db = new BuildContext())
            {
                build = db.Builds.FirstOrDefault(b => b.BuildId == id);
            }

            if (build == null)
            {
                await context.RespondAsync("Invalid build ID");
                return;
            }

            if (build.AuthorId != context.User.Id)
            {
                await context.RespondAsync("This build does not belong to you");
                return;
            }

            var message = await context.Guild.Channels.FirstOrDefault(c => c.Name == "builds")
                .GetMessageAsync(build.MessageId);
            await message.DeleteAsync();

            await context.RespondAsync($"Deleted build **[{build.PatchVersion}] {build.Title}**");
        }

        [Command("top"), Description("Displays the top builds")]
        public async Task TopBuilds(CommandContext context, [Description("Number of builds (10 max).")]int count = 5)
        {
            if (count > 10 || count < 1)
            {
                await context.RespondAsync("Invalid build amount (1-10)");
                return;
            }

            var builds = new List<Build>();
            using (var db = new BuildContext())
            {
                builds = db.Builds.OrderByDescending(b => b.UpVotes - b.DownVotes).ThenByDescending(b => b.UpVotes).Take(count).ToList();
            }

            var embed = new DiscordEmbedBuilder() { Title = $"Top {builds.Count} build(s)" };

            for (int i = 1; i <= builds.Count; i++)
            {
                var build = builds[i - 1];
                var author = await context.Client.GetUserAsync(build.AuthorId);
                embed.AddField($"{i}. (+{build.UpVotes} | -{build.DownVotes}) [{build.PatchVersion}] {build.Title}", $" Author: {author.Mention}\n{build.BuildUrl}");
            }

            await context.RespondAsync("", false, embed.Build());
        }

        [Command("mybuilds"), Description("Displays your builds")]
        public async Task MyBuilds(CommandContext context, [Description("User mention")]string user = null)
        {
            var builds = new List<Build>();

            if (user == null)
            {
                using (var db = new BuildContext())
                {
                    builds = db.Builds.Where(b => b.AuthorId == context.User.Id).ToList();
                }
            }
            else
            {
                var match = new Regex(@"\d+").Match(user);

                if (!match.Success)
                {
                    await context.RespondAsync("Invalid user");
                    return;
                }

                var id = Convert.ToUInt64(match.Value);

                using (var db = new BuildContext())
                {
                    builds = db.Builds.Where(b => b.AuthorId == id).ToList();
                }
            }

            if (builds.Count == 0)
            {
                await context.RespondAsync("User hasn't created any builds");
                return;
            }

            var author = await context.Client.GetUserAsync(builds[0].AuthorId);

            var embed = new DiscordEmbedBuilder();
            embed.WithAuthor($"{author.Username}#{author.Discriminator}", null, author.AvatarUrl);

            foreach (var build in builds)
            {
                embed.AddField($"(+{build.UpVotes} | -{build.DownVotes}) [{build.PatchVersion}] {build.Title}", build.BuildUrl);
            }

            await context.RespondAsync("", false, embed.Build());
        }
    }
}
