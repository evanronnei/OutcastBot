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
    class NewBuildHelper
    {
        public static async Task<string> GetBuildUrl(CommandContext context, string message)
        {
            var interactivity = context.Client.GetInteractivityModule();

            var grimtoolsRegex = new Regex(@"(?<=grimtools.com/calc/)[a-zA-Z0-9]{8}");
            var match = grimtoolsRegex.Match(message);
            
            if (match.Success)
            {             
                return $"http://www.grimtools.com/calc/{match.Value}";
            }
            else
            {
                await context.RespondAsync("Invalid grimtools URL, please re-enter your grimtools URL.");
                var msg = await interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await GetBuildUrl(context, msg.Content);
            }
        }

        public static async Task<string> GetPatchVersion(CommandContext context, string message)
        {
            var interactivity = context.Client.GetInteractivityModule();

            var patchRegex = new Regex(@"\d\.\d\.\d\.\d");
            var match = patchRegex.Match(message);

            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                await context.RespondAsync("Invalid patch version, please re-enter your patch version.");
                var msg = await interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await GetPatchVersion(context, msg.Content);
            }
        }

        public static async Task<string> GetTitle(CommandContext context, string message)
        {
            var interactivity = context.Client.GetInteractivityModule();

            if (message.Length > 100)
            {
                await context.RespondAsync($"Title is too long ({message.Length}). Please shorten your title to 100 characters.");
                var msg = await interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
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
            var interactivity = context.Client.GetInteractivityModule();

            if (message.Length > 1000)
            {
                await context.RespondAsync($"Description is too long ({message.Length}). Please shorten your description to 1000 characters.");
                var msg = await interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
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
            var interactivity = context.Client.GetInteractivityModule();

            var gdForumRegex = new Regex(@"(?<=grimdawn.com/forums/showthread.php\?t=)\d*");
            var match = gdForumRegex.Match(message);

            if (match.Success)
            {
                return $"http://www.grimdawn.com/forums/showthread.php?t={match.Value}";
            }
            else
            {
                await context.RespondAsync("Invalid forum URL, please re-enter your forum URL.");
                var msg = await interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await GetForumUrl(context, msg.Content);
            }
        }

        public static string GetVideoUrl(string message)
        {
            return message;
        }

        public static List<string> GetTags(CommandContext context, string message)
        {
            return message.Split(' ').ToList();
        }

        public static async Task PostBuild(CommandContext context, Build build)
        {
            var channel = context.Guild.Channels.FirstOrDefault(ch => ch.Name == "builds");
            if (channel == null) return;

            Shared.Builds.Add(build);

            await channel.SendMessageAsync(build.Message);
            await Task.Delay(100);

            build.MessageId = channel.LastMessageId;

            var buildMessage = await channel.GetMessageAsync(build.MessageId);
            await buildMessage.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":arrow_up:"));
            await Task.Delay(100);
            await buildMessage.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":arrow_down:"));

            var converter = new DiscordEmojiConverter();
            foreach (var tag in build.Tags)
            {
                var emoji = new DiscordEmoji();
                if(converter.TryConvert(tag, context, out emoji))
                {
                    await buildMessage.CreateReactionAsync(emoji);
                    await Task.Delay(100);
                }
            }
        }
    }
}
