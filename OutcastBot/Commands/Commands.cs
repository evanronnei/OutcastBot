using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using OutcastBot.Commands.CommandHelpers;
using OutcastBot.Objects;
using OutcastBot.Ojects;
using SixLabors.ImageSharp;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Commands
{
    public class Commands
    {
        [Command("credits")]
        [Description("Displays bot credits and source code link")]
        public async Task Credits(CommandContext context)
        {
            await context.TriggerTypingAsync();

            var user = await context.Client.GetUserAsync(125732531629719552);

            var embed = new DiscordEmbedBuilder
            {
                Title = "Developer",
                Description = user.Mention,
                ThumbnailUrl = user.AvatarUrl
            };

            embed.AddField("Source", "https://github.com/evanronnei/OutcastBot/");

            await context.RespondAsync("", false, embed.Build());
        }

        [Command("bug")]
        [Description("Report a bug, give feedback, and/or offer a suggestion")]
        [Aliases("feedback", "suggestion")]
        public async Task ReportBug(CommandContext context)
        {
            await context.TriggerTypingAsync();
            await context.RespondAsync("Bugs, feedback, and suggestions are tracked on GitHub: <https://github.com/evanronnei/OutcastBot/issues>");
        }

        [Command("f")]
        [Description("Pay respects")]
        [Aliases("payrespects")]
        public async Task PayRespects(CommandContext context, [RemainingText, Description("Optional: thing to pay respects to")]string victim = null)
        {
            await context.TriggerTypingAsync();

            var messageContent = $"Press {DiscordEmoji.FromUnicode("🇫")} to pay respects";
            messageContent += (victim == null) ? "." : $" to {victim}.";

            var message = await context.RespondAsync(messageContent);
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("🇫"));
        }

        [Command("quote")]
        [Description("Creates a quote of a Discord message using the message ID.\n\n" +
            "Message IDs can be obtained with developer mode enabled:\n" +
            "Settings > Appearance > Advanced > Developer Mode")]
        public async Task Quote(CommandContext context, [Description("ID of the message to quote")]ulong messageId)
        {
            await context.TriggerTypingAsync();

            var validChannels = context.Guild.Channels.Where(
                ch => Program.AppSettings.QuotableChannels.Contains(ch.Name));

            DiscordMessage message = null;
            foreach (var channel in validChannels)
            {
                try
                {
                    message = await channel.GetMessageAsync(messageId);
                    if (message != null) break;
                }
                catch (NotFoundException)
                {
                    continue;
                }
            }

            if (message == null)
            {
                await context.RespondAsync("Invalid message ID or channel");
                return;
            }

            var embed = new DiscordEmbedBuilder()
            {
                Description = message.Content,
                Timestamp = message.Timestamp,
            };
            embed.WithAuthor($"{message.Author.Username}#{message.Author.Discriminator} in #{message.Channel.Name}", null, message.Author.AvatarUrl);

            await context.RespondAsync("", false, embed.Build());
        }

        [Command("mobile")]
        [Description("Mobile Discord claims another victim")]
        public async Task MobileDiscord(CommandContext context, [Description("@mention of the victim")]DiscordMember member)
        {
            await context.TriggerTypingAsync();

            var avatarPath = $"Temp/{member.Id}_avatar.png";
            var outputPath = $"Temp/{member.Id}_mobile_discord.png";

            var client = new WebClient();
            client.DownloadFile(member.AvatarUrl, avatarPath);

            using (var baseImage = Image.Load("Images/MobileDiscord.png"))
            using (var avatar = Image.Load(avatarPath))
            {
                baseImage.Mutate(x => x.DrawImage(
                    avatar,
                    new Size(43, 43),
                    new Point(221, 148),
                    new GraphicsOptions()));

                baseImage.Save(outputPath);
            }

            using (var fs = new FileStream(outputPath, FileMode.Open))
            {
                await context.RespondWithFileAsync(fs);
            }

            File.Delete(avatarPath);
            File.Delete(outputPath);
        }

        //[Command("tag")]
        //[Description("Creates a new command with the given key/value pair, or gets a command with the given tag")]
        //[RequirePermissions(Permissions.ManageChannels)]
        //public async Task Tag(CommandContext context, [Description("Tag name")]string key, [Description("Tag value"), RemainingText]string value = null)
        //{
        //    await context.TriggerTypingAsync();

        //    using (var db = new TagContext())
        //    {
        //        var tag = db.Tags.FirstOrDefault(t => t.Key == key);

        //        if (value == null)
        //        {
        //            if (tag == null)
        //            {
        //                await context.RespondAsync($"`{key}` is not a valid tag.");
        //                return;
        //            }
        //            await context.RespondAsync(tag.Value);
        //        }
        //        else
        //        {
        //            if (tag != null)
        //            {
        //                await context.RespondAsync($"Tag `{key}` already exists.");
        //                return;
        //            }

        //            tag = new Tag { Key = key, Value = value };
        //            db.Add(tag);
        //            await db.SaveChangesAsync();
        //            await context.RespondAsync($"Created tag `{key}`");
        //        }
        //    }
        //}

        [Hidden]
        [RequirePermissions(Permissions.ManageMessages)]
        [Command("emojis")]
        public async Task Emojis(CommandContext context)
        {
            var embed = new DiscordEmbedBuilder();

            embed.WithAuthor($"{context.Guild.Name} Emojis", "", context.Guild.IconUrl);

            var sb = new StringBuilder();
            foreach (var emoji in context.Guild.Emojis)
            {
                sb.AppendLine($"{emoji.Name} {emoji} `{emoji.Id}`");
            }
            embed.WithDescription(sb.ToString());

            await context.RespondAsync("", false, embed.Build());
        }
    }

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
                await context.RespondAsync("Invalid build ID");
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
            //build.ExpansionRequired = await BuildHelper.GetExpansionRequiredAsync(context);

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

            await context.RespondAsync($"Deleted build **{build.Title}**");
        }

        [Command("top")]
        [Description("Displays the top builds")]
        public async Task TopBuilds(CommandContext context, [Description("Optional: number of builds (5 max).")]int count = 5)
        {
            await context.TriggerTypingAsync();

            if (count > 5 || count < 1)
            {
                await context.RespondAsync("Invalid build amount (1-5)");
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
                await context.RespondAsync($"{member.DisplayName} hasn't created any builds");
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

    [Group("tag", CanInvokeWithoutSubcommand = true)]
    [Description("Gets a tag (key/value pair)")]
    public class TagCommands
    {
        public async Task ExecuteGroupAsync(CommandContext context, [Description("Tag name")]string key)
        {
            await context.TriggerTypingAsync();

            using (var db = new TagContext())
            {
                var tag = db.Tags.FirstOrDefault(t => t.Key == key);

                if (tag == null)
                {
                    await context.RespondAsync($"`{key}` is not a valid tag");
                    return;
                }

                await context.RespondAsync(tag.Value);
            }
        }

        [Command("new")]
        [Description("Creates a new tag")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task NewTag(CommandContext context, [Description("Tag name")]string key, [Description("Tag value"), RemainingText]string value)
        {
            await context.TriggerTypingAsync();

            using (var db = new TagContext())
            {
                var tag = db.Tags.FirstOrDefault(t => t.Key == key);

                if (tag != null)
                {
                    await context.RespondAsync($"Tag `{key}` already exists");
                    return;
                }

                tag = new Tag { Key = key, Value = value };
                db.Add(tag);
                await db.SaveChangesAsync();
                await context.RespondAsync($"Created tag `{key}`");
            }
        }

        [Command("edit")]
        [Description("Edits an existing tag.")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task EditTag(CommandContext context , [Description("Tag name")]string key, [Description("Tag value"), RemainingText]string value)
        {
            await context.TriggerTypingAsync();

            using (var db = new TagContext())
            {
                var tag = db.Tags.FirstOrDefault(t => t.Key == key);

                if (tag == null)
                {
                    await context.RespondAsync($"`{key}` is not a valid tag");
                    return;
                }

                tag.Value = value;
                db.Update(tag);
                await db.SaveChangesAsync();
                await context.RespondAsync($"Edited tag `{key}`");
            }
        }

        [Command("delete")]
        [Description("Deletes an existing tag")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task DeleteTag(CommandContext context, [Description("Tag name")]string key)
        {
            await context.TriggerTypingAsync();

            using (var db = new TagContext())
            {
                var tag = db.Tags.FirstOrDefault(t => t.Key == key);

                if (tag == null)
                {
                    await context.RespondAsync($"`{key}` is not a valid tag");
                    return;
                }

                db.Remove(tag);
                await db.SaveChangesAsync();
                await context.RespondAsync($"Deleted tag `{key}`");
            }
        }

        [Command("list")]
        [Description("Lists all tags")]
        public async Task ListTags(CommandContext context)
        {
            await context.TriggerTypingAsync();

            var tags = new List<Tag>();
            using (var db = new TagContext())
            {
                tags = db.Tags.ToList();
            }

            if (tags.Count == 0)
            {
                await context.RespondAsync("There are no created tags");
                return;
            }

            var keys = new List<string>();
            foreach (var tag in tags)
            {
                keys.Add($"`{tag.Key}`");
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Tags",
                Description = String.Join(", ", keys)
            };

            await context.RespondAsync("", false, embed.Build());
        }
    }
}
