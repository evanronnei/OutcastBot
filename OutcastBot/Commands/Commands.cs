using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using OutcastBot.Commands.CommandHelpers;
using OutcastBot.Ojects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Commands
{
    public class Commands
    {
        [Command("bug"), Description("Report a bug, give feedback, and/or offer a suggestion")]
        [Aliases("feedback", "suggestion")]
        public async Task ReportBug(CommandContext context)
        {
            await context.RespondAsync("Bugs, feedback, and suggestions are tracked on GitHub. <https://github.com/evanronnei/OutcastBot/issues>");
        }

        [Command("f"), Description("Pay respects")]
        [Aliases("payrespects")]
        public async Task PayRespects(CommandContext context)
        {
            var message = await context.RespondAsync($"Press {DiscordEmoji.FromUnicode("🇫")} to pay respects.");
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("🇫"));
        }
    }

    [Group("build", CanInvokeWithoutSubcommand = true), Description("Commands for interacting with builds")]
    public class BuildCommands
    {
        // Get an existing build
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
            build.PatchVersion = await BuildHelper.GetPatchVersionAsync(BuildHelper.CommandType.New, context);
            if (build.PatchVersion == null) return;

            // Title
            build.Title = await BuildHelper.GetTitleAsync(BuildHelper.CommandType.New, context);
            if (build.Title == null) return;

            // Description
            build.Description = await BuildHelper.GetDescriptionAsync(BuildHelper.CommandType.New, context);
            if (build.Description == null) return;

            // BuildUrl
            (build.BuildUrl, build.Mastery) = await BuildHelper.GetBuildUrlAsync(BuildHelper.CommandType.New, context);
            if (build.BuildUrl == null) return;

            // ForumUrl
            build.ForumUrl = await BuildHelper.GetForumUrlAsync(BuildHelper.CommandType.New, context);

            // VideoUrl
            build.VideoUrl = await BuildHelper.GetVideoUrlAsync(BuildHelper.CommandType.New, context);

            // ImageUrl
            build.ImageUrl = await BuildHelper.GetImageUrlAsync(BuildHelper.CommandType.New, context);

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

            var channel = context.Guild.Channels.FirstOrDefault(c => c.Name == "builds");
            if (channel == null) return;                
            var message = await channel.GetMessageAsync(build.MessageId);
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
                var match = new Regex(@"(?<=<@)\d+(?=>)").Match(user);

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
