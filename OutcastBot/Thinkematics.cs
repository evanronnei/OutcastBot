using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using OutcastBot.Enumerations;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot
{
    class Thinkematics
    {
        public static async Task ThinkingHandler(MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot && e.Message.Content.StartsWith("🤔"))
            {
                var temp = new Regex("🤔").Replace(e.Message.Content, "", 1).Trim();

                try
                {
                    var emoji = EnumExtensions.GetValueFromDescription<ThinkingEmoji>(temp);
                    await e.Message.DeleteAsync();
                    await e.Channel.SendMessageAsync(DiscordEmoji.FromGuildEmote(Program.Client, (ulong)emoji));
                }
                catch (ArgumentException)
                {
                    return;
                }
            }
        }
    }
}
