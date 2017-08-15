using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System;
using System.Linq;
using System.Threading.Tasks;
using OutcastBot.Commands;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace OutcastBot
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        public static DiscordClient Client { get; set; }
        public static InteractivityModule Interactivity { get; set; }
        public static CommandsNextModule Commands { get; set; }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            Client = new DiscordClient(new DiscordConfig
            {
                Token = Configuration["Token"],
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            Interactivity = Client.UseInteractivity();

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = Configuration["CommandPrefix"]
            });

            Commands.RegisterCommands<Commands.Commands>();
            Commands.RegisterCommands<BuildCommands>();

            Client.MessageReactionAdd += BuildVoteAddHandler;
            Client.MessageReactionRemove += BuildVoteRemoveHandler;
            Client.MessageCreated += CrabHandler;

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task BuildVoteAddHandler(MessageReactionAddEventArgs e)
        {
            if (e.Channel.Name == "builds")
            {
                using (var db = new BuildContext())
                {
                    var build = db.Builds.FirstOrDefault(b => b.MessageId == e.Message.Id);
                    if (build == null) return;

                    if (e.Emoji.Equals(DiscordEmoji.FromName(Client, ":arrow_up:")))
                    {
                        build.UpVotes++;
                    }
                    else if (e.Emoji.Equals(DiscordEmoji.FromName(Client, ":arrow_down:")))
                    {
                        build.DownVotes++;
                    }

                    db.Update(build);
                    await db.SaveChangesAsync();

                    await DeleteDownvotedBuild(build, e.Message);
                }
            }
        }

        private static async Task BuildVoteRemoveHandler(MessageReactionRemoveEventArgs e)
        {
            if (e.Channel.Name == "builds")
            {
                using (var db = new BuildContext())
                {
                    var build = db.Builds.FirstOrDefault(b => b.MessageId == e.Message.Id);
                    if (build == null) return;

                    if (e.Emoji.Equals(DiscordEmoji.FromName(Client, ":arrow_up:")))
                    {
                        build.UpVotes--;
                    }
                    else if (e.Emoji.Equals(DiscordEmoji.FromName(Client, ":arrow_down:")))
                    {
                        build.DownVotes--;
                    }

                    db.Update(build);
                    await db.SaveChangesAsync();
                }
            }
        }

        private static async Task DeleteDownvotedBuild(Build build, DiscordMessage message)
        {
            if (build.UpVotes + build.DownVotes >= 10 && (double)build.DownVotes / (double)(build.UpVotes + build.DownVotes) >= 0.70)
            {
                await message.DeleteAsync();

                using (var db = new BuildContext())
                {
                    db.Remove(build);
                    await db.SaveChangesAsync();
                }
            }
        }

        private static async Task CrabHandler(MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot)
            {
                var match = new Regex(@"\bc\s?r\s?a\s?b(\s?(c\s?o\s?)?m\s?m?\s?a\s?n\s?d\s?o)?\b").Match(e.Message.Content.ToLower());

                if (match.Success) await e.Message.CreateReactionAsync(DiscordEmoji.FromUnicode(Client, "🦀"));
            }
        }
    }
}