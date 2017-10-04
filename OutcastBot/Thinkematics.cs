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
            if (!e.Author.IsBot && e.Message.Content.StartsWith("🤔"))
            {
                var temp = e.Message.Content.Remove(0, 1);

                if (temp.EndsWith("👀"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.ThinkEyes).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🇯🇵"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.WeebThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("😡"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.ThinkRage).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("👌"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.OkThinking).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("👏"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.Clapking).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("✝️"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.ThinkusVult).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🤷"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.Thrugging).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🌊"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.ThinkWave).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🥔"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.SpudThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🦀"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.CrabThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("😉"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.Winking).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("😐"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.ThinkStare).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🖕"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.UpThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🔫"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.KmsThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("👈"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.LeftyThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("⬜"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.SquareThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("💻"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.ThinkPad).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🍞"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.Breading).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("💦"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.ThinkDrops).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🎩"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.Mthinking).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🍺"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.BeerThink).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("😫"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.ThinkYawn).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("☯️"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.ThinkYang).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("😰"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.ThinkCern).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🔄"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.ThinkSpinner).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🤔"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.Thinkception).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🐟"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.ThinkFish).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("🍆"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.ThinkPlant).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("😕"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.Thinkfusing).ToString());

                    await e.Message.DeleteAsync();
                }
                else if (temp.EndsWith("👍"))
                {
                    await e.Message.RespondAsync(DiscordEmoji.FromGuildEmote(
                        Program.Client, (ulong)ThinkingEmoji.Thinkup).ToString());

                    await e.Message.DeleteAsync();
                }
            }
        }
    }
}
