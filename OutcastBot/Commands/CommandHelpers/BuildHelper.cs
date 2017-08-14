using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Commands.CommandHelpers
{
    class BuildHelper
    {
        public static async Task<string> GetBuildUrl(CommandContext context, string message)
        {
            var match = new Regex(@"(?<=grimtools.com/calc/)[a-zA-Z0-9]{8}").Match(message);
            
            if (match.Success)
            {             
                return $"http://www.grimtools.com/calc/{match.Value}";
            }
            else
            {
                await context.RespondAsync("Invalid grimtools URL, please re-enter your grimtools URL.");
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await GetBuildUrl(context, msg.Content);
            }
        }

        public static async Task<string> GetPatchVersion(CommandContext context, string message)
        {
            var match = new Regex(@"\d\.\d\.\d\.\d").Match(message);

            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                await context.RespondAsync("Invalid patch version, please re-enter your patch version.");
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await GetPatchVersion(context, msg.Content);
            }
        }

        public static async Task<string> GetTitle(CommandContext context, string message)
        {
            if (message.Length > 100)
            {
                await context.RespondAsync($"Title is too long ({message.Length}). Please shorten your title to 100 characters.");
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await GetTitle(context, msg.Content);
            }
            else
            {
                return message;
            }
        }

        public static async Task<string> GetDescription(CommandContext context, string message)
        {
            if (message.Length > 1000)
            {
                await context.RespondAsync($"Description is too long ({message.Length}). Please shorten your description to 1000 characters.");
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await GetDescription(context, msg.Content);
            }
            else
            {
                return message;
            }
        }

        public static async Task<string> GetForumUrl(CommandContext context, string message)
        {
            var match = new Regex(@"(?<=grimdawn.com/forums/showthread.php\?t=)\d*").Match(message);

            if (match.Success)
            {
                return $"http://www.grimdawn.com/forums/showthread.php?t={match.Value}";
            }
            else
            {
                await context.RespondAsync("Invalid forum URL, please re-enter your forum URL.");
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await GetForumUrl(context, msg.Content);
            }
        }

        public static string GetVideoUrl(string message)
        {
            return message;
        }

        public static string GetTags(CommandContext context, string message)
        {
            var output = "";
            var tags = message.Split(' ').ToList();

            var converter = new DiscordEmojiConverter();
            foreach (var tag in tags)
            {
                var emoji = new DiscordEmoji();
                if (converter.TryConvert(tag, context, out emoji))
                {
                    output += $"{tag} ";
                }
            }

            return output;
        }

        public static async Task PostBuild(CommandContext context, Build build)
        {
            var channel = context.Guild.Channels.FirstOrDefault(ch => ch.Name == "builds");
            if (channel == null) return;

            await channel.SendMessageAsync(build.Message);
            await Task.Delay(500);

            build.MessageId = channel.LastMessageId;

            using (var db = new BuildContext())
            {
                db.Builds.Add(build);
                await db.SaveChangesAsync();
            }

            var buildMessage = await channel.GetMessageAsync(build.MessageId);
            await buildMessage.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":arrow_up:"));
            await Task.Delay(250);
            await buildMessage.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":arrow_down:"));

            var converter = new DiscordEmojiConverter();
            foreach (var tag in build.Tags.Split(' ').ToList())
            {
                var emoji = new DiscordEmoji();
                if(converter.TryConvert(tag, context, out emoji))
                {
                    await buildMessage.CreateReactionAsync(emoji);
                    await Task.Delay(250);
                }
            }
        }

        public static async Task<int?> ValidateIndex(CommandContext context, string message, int count)
        {
            var match = new Regex(@"\b\d+\b").Match(message);

            if (!match.Success)
            {
                await context.RespondAsync("Invalid input. Use the index of the build to select it.");
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await ValidateIndex(context, msg.Content, count);
            }

            var index = Convert.ToInt32(message);

            if (index >= count || index < 0)
            {
                await context.RespondAsync("Invalid build. Please select a valid build");
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await ValidateIndex(context, msg.Content, count);
            }
            else
            {
                return index;
            }
        }
    }
}
