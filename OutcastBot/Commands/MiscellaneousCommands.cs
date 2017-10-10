using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.Primitives;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OutcastBot.Commands
{
    public class MiscellaneousCommands
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
        public async Task PayRespects(CommandContext context, [Description("Optional: thing to pay respects to"), RemainingText]string text)
        {
            await context.TriggerTypingAsync();

            var messageContent = $"Press {DiscordEmoji.FromUnicode("🇫")} to pay respects";
            messageContent += (String.IsNullOrEmpty(text)) ? "." : $" to {text}.";

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
                var error = await context.RespondAsync("Invalid message");
                await Task.Delay(5000)
                    .ContinueWith(t => error.DeleteAsync())
                    .ContinueWith(t => context.Message.DeleteAsync());
                return;
            }

            var embed = new DiscordEmbedBuilder()
            {
                Description = message.Content,
                Timestamp = message.Timestamp,
            };

            embed.WithAuthor($"{message.Author.Username} in #{message.Channel.Name}", null, message.Author.AvatarUrl);

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
}
