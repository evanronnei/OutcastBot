using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;
using OutcastBot.Commands;

namespace OutcastBot
{
    class Program
    {
        static DiscordClient client;
        static CommandsNextModule commands;
        static InteractivityModule interactivity;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            client = new DiscordClient(new DiscordConfig
            {
                Token = "MzQ0Mjc3MjU0MDYwMjQ0OTkz.DGqYsA.EPzg-jTrABKT_MY8mctlQ5OBUl8",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            interactivity = client.UseInteractivity();

            commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });

            commands.RegisterCommands<Commands.Commands>();
            commands.RegisterCommands<BuildCommands>();

            client.MessageCreated += async e =>
            {
                if (e.Message.Content.Contains(DiscordEmoji.FromName(client, ":thinking:").ToString()))
                {
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName(client, ":thinking:"));
                }
            };

            //client.MessageReactionAdd += async e =>
            //{
            //    if (e.Channel.Name == "builds")
            //    {
            //        if (e.Emoji.Equals(DiscordEmoji.FromName(client, ":arrow_up:")))
            //        {

            //        }
            //        else if (e.Emoji.Equals(DiscordEmoji.FromName(client, ":arrow_down:")))
            //        {

            //        }
            //    }
            //};

            await client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}