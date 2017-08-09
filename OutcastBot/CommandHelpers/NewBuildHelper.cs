using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.CommandHelpers
{
    class NewBuildHelper
    {
        public static string GetBuildUrl(string message)
        {
            var grimtoolsRegex = new Regex(@"(?<=grimtools.com/calc/)[a-zA-Z0-9]{8}");
            var match = grimtoolsRegex.Match(message);
            
            if (match.Success)
            {
                message = $"http://www.grimtools.com/calc/{match.Value}";                
                return message;
            }
            else
            {
                return null;
            }
        }

        public static string GetPatchVersion(string message)
        {
            var patchRegex = new Regex(@"\d\.\d\.\d\.\d");
            var match = patchRegex.Match(message);

            if (match.Success)
            {
                return message;
            }
            else
            {
                return null;
            }
        }

        public static string GetTitle(string message)
        {
            if (message.Length > 100)
            {
                return message.Substring(0, 100);
            }
            else
            {
                return message;
            }
        }

        public static string GetDescription(string message)
        {
            if (message.Length > 1000)
            {
                return message.Substring(0, 1000);
            }
            else
            {
                return message;
            }
        }

        public static string GetForumUrl(string message)
        {
            var gdForumRegex = new Regex(@"(?=<grimdawn.com/forums/showthread.php?t=)\d*");
            var match = gdForumRegex.Match(message);

            if (match.Success)
            {
                return $"http://www.grimdawn.com/forums/showthread.php?t={match.Value}";
            }
            else
            {
                return null;
            }
        }

        public static string GetVideoUrl(string message)
        {
            return message;
        }

        public static List<DiscordEmoji> GetTags(string message, CommandContext c)
        {
            List<DiscordEmoji> tags = new List<DiscordEmoji>();

            var converter = new DiscordEmojiConverter();

            var temp = message.Split(' ');

            foreach (string s in temp)
            {
                var emoji = new DiscordEmoji();
                if (converter.TryConvert(s, c, out emoji)) tags.Add(emoji);
            }

            return tags;
        }

        public static async Task PostBuild(Build build, CommandContext c)
        {
            var channel = c.Guild.Channels.FirstOrDefault(ch => ch.Name == "builds");
            if (channel == null) return;

            await channel.SendMessageAsync(build.Message);
            await Task.Delay(100);

            build.MessageId = channel.LastMessageId;

            var buildMessage = await channel.GetMessageAsync(build.MessageId);
            await buildMessage.CreateReactionAsync(DiscordEmoji.FromName(c.Client, ":arrow_up:"));
            await buildMessage.CreateReactionAsync(DiscordEmoji.FromName(c.Client, ":arrow_down:"));
            foreach (var tag in build.Tags)
            {
                await buildMessage.CreateReactionAsync(tag);
                await Task.Delay(100);
            }
        }
    }
}
