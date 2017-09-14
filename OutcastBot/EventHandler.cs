using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using OutcastBot.Ojects;
using System;
using System.IO;
using System.Linq;
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

            await Program.Client.UpdateStatusAsync(new Game($"{Program.Configuration["CommandPrefix"]}help"));
        }

        public static Task ClientErrorHandler(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(
                LogLevel.Error, 
                "OutcastBot", 
                $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}\n{e.Exception.StackTrace}", 
                DateTime.Now);

            return Task.CompletedTask;
        }

        public static Task CommandErrorHandler(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(
                LogLevel.Error, 
                "OutcastBot", 
                $"Exception occured on command: \"{e.Command.Name}\": {e.Exception.Message}\n{e.Exception.StackTrace}", 
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

                    if (build.UpVotes + build.DownVotes >= 10 && 
                        (double)build.DownVotes / (double)(build.UpVotes + build.DownVotes) >= 0.70)
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

                if (match.Success)
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
                using (var fs = new FileStream($"{Directory.GetCurrentDirectory()}/Images/ExpansionWhen.png", FileMode.Open))
                {
                    await e.Message.RespondWithFileAsync(fs);
                }
            }
        }
    }
}
