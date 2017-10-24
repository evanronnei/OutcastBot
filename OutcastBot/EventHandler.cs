using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using OutcastBot.Enumerations;
using OutcastBot.Objects;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot
{
    class EventHandler
    {
        public static async Task ClientReadyHandler(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(
                LogLevel.Info,
                "OutcastBot",
                "Client is ready to process events.",
                DateTime.Now);

            await Program.Client.UpdateStatusAsync(new DiscordGame($"{Program.AppSettings.CommandPrefix}help"));
        }

        public static Task ClientErrorHandler(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(
                LogLevel.Error,
                "OutcastBot",
                $"Exception occured at {e.Exception.Source}: {e.Exception.GetType().Name}: {e.Exception.Message}\n" +
                    $"{e.Exception.InnerException}" +
                    $"{e.Exception.StackTrace}",
                DateTime.Now);

            return Task.CompletedTask;
        }

        public static async Task CommandErrorHandler(CommandErrorEventArgs e)
        {
            switch (e.Exception)
            {
                case CommandNotFoundException notFound:
                    var message = await e.Context.RespondAsync($"Invalid command. Type " +
                        $"`{Program.AppSettings.CommandPrefix}help` for a list of commands.");

                    await Task.Delay(5000)
                        .ContinueWith(t => message.DeleteAsync())
                        .ContinueWith(t => e.Context.Message.DeleteAsync());
                    return;
                case ArgumentException argument:
                    message = await e.Context.RespondAsync($"Invalid command arguments. " +
                        $"Type `{Program.AppSettings.CommandPrefix}help {e.Command.QualifiedName}` for more info.");

                    await Task.Delay(5000)
                        .ContinueWith(t => message.DeleteAsync())
                        .ContinueWith(t => e.Context.Message.DeleteAsync());
                    return;
                default:
                    e.Context.Client.DebugLogger.LogMessage(
                        LogLevel.Error,
                        "OutcastBot",
                        $"Exception occured at {e.Exception.Source}: {e.Exception.GetType().Name}: {e.Exception.Message}\n" +
                            $"{e.Exception.InnerException}\n" +
                            $"{e.Exception.StackTrace}",
                        DateTime.Now);
                    return;
            }
        }

        public static async Task BuildVoteAddHandler(MessageReactionAddEventArgs e)
        {
            if (e.Channel.Name == "builds")
            {
                using (var db = new BuildContext())
                {
                    var build = db.Builds.FirstOrDefault(b => b.MessageId == e.Message.Id);
                    if (build == null) return;

                    if (e.Emoji == DiscordEmoji.FromName(Program.Client, ":arrow_up:"))
                    {
                        build.UpVotes++;
                    }
                    else if (e.Emoji == DiscordEmoji.FromName(Program.Client, ":arrow_down:"))
                    {
                        build.DownVotes++;
                    }

                    db.Update(build);
                    await db.SaveChangesAsync();

                    if (build.UpVotes + build.DownVotes - 2 >= 10 && 
                        (double)build.DownVotes / (double)(build.UpVotes + build.DownVotes - 2) >= 0.70)
                    {
                        await e.Message.DeleteAsync();
                    }
                }
            }
        }

        public static async Task BuildVoteRemoveHandler(MessageReactionRemoveEventArgs e)
        {
            if (e.Channel.Name == "builds")
            {
                using (var db = new BuildContext())
                {
                    var build = db.Builds.FirstOrDefault(b => b.MessageId == e.Message.Id);
                    if (build == null) return;

                    if (e.Emoji == DiscordEmoji.FromName(Program.Client, ":arrow_up:"))
                    {
                        build.UpVotes--;
                    }
                    else if (e.Emoji == DiscordEmoji.FromName(Program.Client, ":arrow_down:"))
                    {
                        build.DownVotes--;
                    }

                    db.Update(build);
                    await db.SaveChangesAsync();
                }
            }
        }

        public static async Task BuildDeleteHandler(MessageDeleteEventArgs e)
        {
            if (e.Channel.Name == "builds")
            {
                using (var db = new BuildContext())
                {
                    var build = db.Builds.FirstOrDefault(b => b.MessageId == e.Message.Id);
                    if (build == null) return;

                    var author = await e.Guild.GetMemberAsync(build.AuthorId);
                    await author.SendMessageAsync(
                        $"{DiscordEmoji.FromUnicode("❌")} Your build has been deleted due to a large negative score " +
                            $"(+{(build.UpVotes - 1)} | -{(build.DownVotes - 1)}).",
                        false,
                        await build.GetEmbed());

                    db.Remove(build);
                    await db.SaveChangesAsync();
                }
            }
        }

        public static async Task CrabHandler(MessageCreateEventArgs e)
        {
            if (e.Author.IsBot) return;

            var match = new Regex(@"c\s?r\s?a\s?b(\s?(c\s?o\s?)?m\s?m?\s?a\s?n\s?d\s?o)?(\s?s)?")
                .Match(e.Message.Content.ToLower());

            if (match.Success || e.Message.Content.Contains("🦀"))
            {
                await e.Message.CreateReactionAsync(DiscordEmoji.FromUnicode(Program.Client, "🦀"));
            }
        }

        public static async Task JanitorDeleteHandler(MessageDeleteEventArgs e)
        {
            if (e.Channel.Name == "trade" || e.Channel.Name == "searching-players")
            {
                var embed = new DiscordEmbedBuilder()
                {          
                    Description = e.Message.Content,
                    Timestamp = e.Message.Timestamp,
                    Color = DiscordColor.Red
                };
                embed.WithAuthor($"{e.Message.Author.Username}#{e.Message.Author.Discriminator} in #{e.Channel.Name}", null, e.Message.Author.AvatarUrl);

                var channel = e.Guild.Channels.FirstOrDefault(c => c.Name == "broomcloset");
                if (channel == null) return;
                await channel.SendMessageAsync($"Deleted message:", false, embed.Build());
            }
        }

        public static async Task GrimToolsCalcHandler(MessageCreateEventArgs e)
        {
            if (e.Author.IsBot) return;

            var match = new Regex(@"(?<=(http://)?(www.)?grimtools.com/calc/)[a-zA-Z0-9]{8}(?!>)").Match(e.Message.Content);

            if (!match.Success) return;

            var url = $"http://www.grimtools.com/calc/{match.Value}";

            var calc = await GrimToolsCalc.GetGrimToolsCalcAsync(url);

            var masteryCombo = calc.GetMasteryCombination();

            var embed = new DiscordEmbedBuilder
            {
                Url = url,
                ThumbnailUrl = masteryCombo.GetAttribute<MasteryInfoAttribute>().ImageUrl,

                Title = $"Level {calc.Data.Info.Level} " +
                    $"{Regex.Replace(masteryCombo.ToString(), @"(\B[A-Z])", " $1")}",

                Description = $"`Physique` {((calc.Data.Info.Physique - 50) / 8).ToString()}\n" +
                    $"`Cunning` {((calc.Data.Info.Cunning - 50) / 8).ToString()}\n" +
                    $"`Spirit` {((calc.Data.Info.Spirit - 50) / 8).ToString()}"
            };

            foreach (var mastery in calc.Data.Masteries.OrderByDescending(m => m.Value))
            {
                embed.AddField(mastery.Key.ToString(), mastery.Value.ToString(), true);
            }

            var sb = new StringBuilder();
            var sortedSkills = calc.Data.Skills.OrderByDescending(s => s.Value).ToList();
            for (int i = 0; i < 10 && i < sortedSkills.Count; i++)
            {
                try
                {
                    var skillEmoji = EnumExtensions.GetValueFromDescription<SkillEmoji>(sortedSkills[i].Key);
                    sb.Append(DiscordEmoji.FromGuildEmote(Program.Client, (ulong)skillEmoji).ToString());
                }
                catch (ArgumentException)
                {
                    sortedSkills.RemoveAt(i);
                    i--;
                }
            }

            if (sb.Length > 0) embed.AddField("Top Skill(s)", sb.ToString());

            embed.WithFooter($"Game version: {calc.GameVersion}");

            embed.WithColor(new DiscordColor(masteryCombo.GetAttribute<MasteryInfoAttribute>().Color));

            await e.Message.RespondAsync("", false, embed.Build());
        }
    }
}
