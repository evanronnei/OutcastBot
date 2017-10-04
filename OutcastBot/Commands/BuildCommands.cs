using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using OutcastBot.Commands.CommandHelpers;
using OutcastBot.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OutcastBot.Commands
{
    [Group("build", CanInvokeWithoutSubcommand = true)]
    [Description("Commands for interacting with builds")]
    public class BuildCommands
    {
        // Get an existing build
        public async Task ExecuteGroupAsync(CommandContext context, [Description("ID of the build to get")]int id)
        {
            await context.TriggerTypingAsync();

            var build = new Build();
            using (var db = new BuildContext())
            {
                build = db.Builds.FirstOrDefault(b => b.BuildId == id);
            }

            if (build == null)
            {
                var error = await context.RespondAsync($"`{id}` is not a valid build ID");
                await Task.Delay(2500)
                    .ContinueWith(t => error.DeleteAsync())
                    .ContinueWith(t => context.Message.DeleteAsync());
                return;
            }

            await context.RespondAsync("", false, await build.GetEmbed());
        }

        [Command("new")]
        [Description("Create a new build.\n\n" +
            "Command will prompt you to fill in the following properties:\n" +
            "(REQUIRED) Patch Version\n" +
            "(REQUIRED) Title\n" +
            "(REQUIRED) Description\n" +
            "(REQUIRED) Build URL\n" +
            "(OPTIONAL) Forum URL\n" +
            "(OPTIONAL) Video URL\n" +
            "(OPTIONAL) Build Image")]
        public async Task NewBuild(CommandContext context)
        {
            var build = new Build()
            {
                AuthorId = context.User.Id,
                UpVotes = 0,
                DownVotes = 0
            };

            // PatchVersion
            build.PatchVersion = await BuildHelper.GetPatchVersionAsync(context);
            if (String.IsNullOrEmpty(build.PatchVersion)) return;

            // ExpansionRequired
            build.ExpansionRequired = await BuildHelper.GetExpansionRequiredAsync(context);

            // Title
            build.Title = await BuildHelper.GetTitleAsync(context);
            if (String.IsNullOrEmpty(build.Title)) return;

            // Description
            build.Description = await BuildHelper.GetDescriptionAsync(context);
            if (String.IsNullOrEmpty(build.Description)) return;

            // BuildUrl & Mastery
            var buildInfo = await BuildHelper.GetBuildInfoAsync(context);
            if (buildInfo == null) return;
            build.BuildUrl = buildInfo.BuildUrl;
            build.Mastery = buildInfo.Mastery;

            // ForumUrl
            build.ForumUrl = await BuildHelper.GetForumUrlAsync(context);

            // VideoUrl
            build.VideoUrl = await BuildHelper.GetVideoUrlAsync(context);

            // ImageUrl
            build.ImageUrl = await BuildHelper.GetImageUrlAsync(context);

            // Post Build
            await NewBuildHelper.PostBuild(context, build);
        }

        [Command("edit")]
        [Description("Edit an existing build")]
        public async Task EditBuild(CommandContext context, [Description("ID of the build to edit")]int id)
        {
            await context.TriggerTypingAsync();

            var build = new Build();
            using (var db = new BuildContext())
            {
                build = db.Builds.FirstOrDefault(b => b.BuildId == id);
            }

            if (build == null)
            {
                var error = await context.RespondAsync($"`{id}` is not a valid build ID");
                await Task.Delay(2500)
                    .ContinueWith(t => error.DeleteAsync())
                    .ContinueWith(t => context.Message.DeleteAsync());
                return;
            }

            if (build.AuthorId != context.User.Id)
            {
                var error = await context.RespondAsync("That build does not belong to you");
                await Task.Delay(2500)
                    .ContinueWith(t => error.DeleteAsync())
                    .ContinueWith(t => context.Message.DeleteAsync());
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

        [Command("delete")]
        [Description("Delete an existing build")]
        public async Task DeleteBuild(CommandContext context, [Description("ID of the build to delete")]int id)
        {
            await context.TriggerTypingAsync();

            var build = new Build();
            using (var db = new BuildContext())
            {
                build = db.Builds.FirstOrDefault(b => b.BuildId == id);
            }

            if (build == null)
            {
                var error = await context.RespondAsync($"`{id}` is not a valid build ID");
                await Task.Delay(2500)
                    .ContinueWith(t => error.DeleteAsync())
                    .ContinueWith(t => context.Message.DeleteAsync());
                return;
            }

            if (build.AuthorId != context.User.Id)
            {
                var error = await context.RespondAsync("That build does not belong to you");
                await Task.Delay(2500)
                    .ContinueWith(t => error.DeleteAsync())
                    .ContinueWith(t => context.Message.DeleteAsync());
                return;
            }

            var channel = context.Guild.Channels.FirstOrDefault(c => c.Name == "builds");
            if (channel == null) return;
            var message = await channel.GetMessageAsync(build.MessageId);
            await message.DeleteAsync();

            await context.RespondAsync($"Deleted build **{build.Title}**");
        }

        [Command("top")]
        [Description("Displays the top builds")]
        public async Task TopBuilds(CommandContext context, [Description("Optional: number of builds (5 max).")]int count = 5)
        {
            await context.TriggerTypingAsync();

            if (count > 5 || count < 1)
            {
                var error = await context.RespondAsync($"`{count}` is not a valid build amount (1-5)");
                await Task.Delay(2500)
                    .ContinueWith(t => error.DeleteAsync())
                    .ContinueWith(t => context.Message.DeleteAsync());
                return;
            }

            var builds = new List<Build>();
            using (var db = new BuildContext())
            {
                builds = db.Builds.OrderByDescending(b => b.UpVotes - b.DownVotes).ThenByDescending(b => b.UpVotes).Take(count).ToList();
            }

            var embed = new DiscordEmbedBuilder { Title = $"Top {builds.Count} build(s)" };

            for (int i = 1; i <= builds.Count; i++)
            {
                var build = builds[i - 1];
                var author = await context.Client.GetUserAsync(build.AuthorId);
                embed.AddField($"{i}. (+{build.UpVotes - 1} | -{build.DownVotes - 1}) {build.Title}", $" Author: {author.Mention}\n{build.BuildUrl}");
            }

            await context.RespondAsync("", false, embed.Build());
        }

        [Command("mybuilds")]
        [Description("Displays your builds")]
        public async Task MyBuilds(CommandContext context, [Description("Optional: user @mention")]DiscordMember member = null)
        {
            await context.TriggerTypingAsync();

            var builds = new List<Build>();

            using (var db = new BuildContext())
            {
                builds = (member == null)
                    ? db.Builds.Where(b => b.AuthorId == context.User.Id).ToList()
                    : db.Builds.Where(b => b.AuthorId == member.Id).ToList();
            }

            if (builds.Count == 0)
            {
                var error = await context.RespondAsync($"{member.DisplayName} hasn't created any builds");
                await Task.Delay(2500)
                    .ContinueWith(t => error.DeleteAsync())
                    .ContinueWith(t => context.Message.DeleteAsync());
                return;
            }

            var author = await context.Client.GetUserAsync(builds[0].AuthorId);

            var embed = new DiscordEmbedBuilder();
            embed.WithAuthor($"{author.Username}#{author.Discriminator}", null, author.AvatarUrl);

            foreach (var build in builds)
            {
                embed.AddField($"(+{build.UpVotes - 1} | -{build.DownVotes - 1}) {build.Title}", build.BuildUrl);
            }

            await context.RespondAsync("", false, embed.Build());
        }
    }
}
