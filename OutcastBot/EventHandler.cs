using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using OutcastBot.Enumerations;
using OutcastBot.Objects;
using OutcastBot.Ojects;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static OutcastBot.Enumerations.Attributes;

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

            await Program.Client.UpdateStatusAsync(new Game($"{Program.AppSettings.CommandPrefix}help"));
        }

        public static Task ClientErrorHandler(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(
                LogLevel.Error,
                "OutcastBot",
                $"Exception occured at {e.Exception.Source}: {e.Exception.GetType()}: " +
                    $"{e.Exception.Message}\n{e.Exception.StackTrace}",
                DateTime.Now);

            return Task.CompletedTask;
        }

        public static Task CommandErrorHandler(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(
                LogLevel.Error,
                "OutcastBot",
                $"Exception occured at {e.Exception.Source} on command '{e.Command.Name}': " +
                    $"{e.Exception.GetType()}: {e.Exception.Message}\n{e.Exception.StackTrace}",
                DateTime.Now);

            return Task.CompletedTask;
        }

        public static async Task BuildVoteAddHandler(MessageReactionAddEventArgs e)
        {
            if (e.Channel.Name == "builds")
            {
                using (var db = new BuildContext())
                {
                    var build = db.Builds.FirstOrDefault(b => b.MessageId == e.Message.Id);
                    if (build == null) return;

                    if (e.Emoji.Equals(DiscordEmoji.FromName(Program.Client, ":arrow_up:")))
                    {
                        build.UpVotes++;
                    }
                    else if (e.Emoji.Equals(DiscordEmoji.FromName(Program.Client, ":arrow_down:")))
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

                    if (e.Emoji.Equals(DiscordEmoji.FromName(Program.Client, ":arrow_up:")))
                    {
                        build.UpVotes--;
                    }
                    else if (e.Emoji.Equals(DiscordEmoji.FromName(Program.Client, ":arrow_down:")))
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
                    db.Remove(build);
                    await db.SaveChangesAsync();
                }
            }
        }

        public static async Task CrabHandler(MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot)
            {
                var match = new Regex(@"c\s?r\s?a\s?b(\s?(c\s?o\s?)?m\s?m?\s?a\s?n\s?d\s?o)?(\s?s)?")
                    .Match(e.Message.Content.ToLower());

                if (match.Success || e.Message.Content.Contains("🦀"))
                {
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromUnicode(Program.Client, "🦀"));
                }
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
                    Color = new DiscordColor(255, 0, 0)
                };
                embed.WithAuthor($"{e.Message.Author.Username}#{e.Message.Author.Discriminator} in #{e.Channel.Name}", null, e.Message.Author.AvatarUrl);

                var channel = e.Guild.Channels.FirstOrDefault(c => c.Name == "broomcloset");
                if (channel == null) return;
                await channel.SendMessageAsync($"Deleted message:", false, embed.Build());
            }
        }

        public static async Task ExpansionWhenHandler(MessageCreateEventArgs e)
        {
            var match = new Regex(@"\be?\s?x\s?p\s?a\s?((n\s?s\s?i\s?o\s?n)|c)\s?w\s?h\s?e\s?n\b")
                .Match(e.Message.Content.ToLower());

            if (match.Success)
            {
                await e.Message.RespondAsync("https://youtu.be/oz2Z6hN5YBs");
            }
        }

        public static async Task GrimToolsHandler(MessageCreateEventArgs e)
        {
            var match = new Regex(@"(?<=grimtools.com/calc/)[a-zA-Z0-9]{8}").Match(e.Message.Content);
            
            if (match.Success)
            {
                var url = $"http://www.grimtools.com/calc/{match.Value}";

                var grimToolsBuild = await GrimToolsBuild.GetGrimToolsBuildAsync(url);

                var masteryCombo = grimToolsBuild.GetMasteryCombination();

                var embed = new DiscordEmbedBuilder
                {
                    Url = url,
                    ThumbnailUrl = masteryCombo.GetAttribute<MasteryInfoAttribute>().ImageUrl,

                    Title = $"Level {grimToolsBuild.BuildData.BuildInfo.Level} " +
                        $"{Regex.Replace(masteryCombo.ToString(), @"(\B[A-Z])", " $1")}",

                    Description = $"`Physique` {((grimToolsBuild.BuildData.BuildInfo.Physique - 50) / 8).ToString()}\n" +
                                  $"`Cunning` {((grimToolsBuild.BuildData.BuildInfo.Cunning - 50) / 8).ToString()}\n" +
                                  $"`Spirit` {((grimToolsBuild.BuildData.BuildInfo.Spirit - 50) / 8).ToString()}"
                };

                foreach (var mastery in grimToolsBuild.BuildData.Masteries.OrderByDescending(m => m.Value))
                {
                    embed.AddField(mastery.Key.ToString(), mastery.Value.ToString(), true);
                }

                var sb = new StringBuilder();
                var sortedSkills = grimToolsBuild.BuildData.Skills.OrderByDescending(s => s.Value).ToList();
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

                embed.WithFooter($"Game version: {grimToolsBuild.GameVersion}");

                embed.WithColor(new DiscordColor(masteryCombo.GetAttribute<MasteryInfoAttribute>().Color));

                await e.Message.RespondAsync("", false, embed.Build());
            }
        }
    }
}
