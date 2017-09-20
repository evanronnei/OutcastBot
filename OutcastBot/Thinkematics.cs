using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using OutcastBot.Enumerations;
using System.Threading.Tasks;

namespace OutcastBot
{
    class Thinkematics
    {
        public static async Task ThinkingHandler(MessageCreateEventArgs e)
        {
            if (e.Message.Content.StartsWith("🤔"))
            {
                var temp = e.Message.Content.Remove(0, 1);

                if (temp.EndsWith("👀"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.ThinkEyes).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🇯🇵"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.WeebThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("😡"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.ThinkRage).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("👌"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.OkThinking).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("👏"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.Clapking).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("✝️"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.ThinkusVult).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🤷"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.Thrugging).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🌊"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.ThinkWave).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🥔"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.SpudThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🦀"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.CrabThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("😉"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.Winking).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("😐"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.ThinkStare).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🖕"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.UpThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🔫"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.KmsThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("👈"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.LeftyThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("⬜"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.SquareThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("💻"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.ThinkPad).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🍞"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.Breading).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("💦"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.ThinkDrops).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🎩"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.Mthinking).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🍺"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.BeerThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("😫"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.ThinkYawn).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("☯️"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.ThinkYawn).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("😰"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.ThinkCern).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🔄"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.ThinkSpinner).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🤔"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.Thinkception).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🐟"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.ThinkFish).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🍆"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.ThinkFish).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("😕"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.Thinkfusing).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("👍"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmojis.Thinkup).ToString());

                    await e.Message.DeleteAsync();
                }
            }
        }
    }
}
