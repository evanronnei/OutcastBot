using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot
{
    class EventHandler
    {
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
                var match = new Regex(@"\bc\s?r\s?a\s?b(\s?(c\s?o\s?)?m\s?m?\s?a\s?n\s?d\s?o)?(\s?s)?\b")
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

        public static async Task GrimDawnForumHandler(MessageCreateEventArgs e)
        {
            var match = new Regex(@"http://grimdawn.com/forums/showthread.php\?t=\d+").Match(e.Message.Content);

            if (match.Success)
            {
                var forumPost = new ForumPost(match.Value);

                await e.Message.ModifyAsync(e.Message.Content, forumPost.GetEmbed());
            }
        }
    }
}
