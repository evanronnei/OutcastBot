using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;

namespace OutcastBot
{
    class Program
    {
        static DiscordClient discord;
        static CommandsNextModule commands;
        static InteractivityModule interactivity;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            discord = new DiscordClient(new DiscordConfig
            {
                Token = "MzQ0Mjc3MjU0MDYwMjQ0OTkz.DGqYsA.EPzg-jTrABKT_MY8mctlQ5OBUl8",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            interactivity = discord.UseInteractivity();

            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });

            commands.RegisterCommands<Commands>();
            commands.RegisterCommands<BuildCommands>();

            discord.MessageCreated += async e =>
            {
                if (e.Message.Content.Contains(DiscordEmoji.FromName(discord, ":thinking:").ToString()))
                {
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discord, ":thinking:"));
                }
            };

            //discord.MessageReactionAdd += async e =>
            //{
            //    if (e.Channel.Name == "builds")
            //    {
            //        if (e.Emoji.Equals(DiscordEmoji.FromName(discord, ":arrow_up:")))
            //        {

            //        }
            //        else if (e.Emoji.Equals(DiscordEmoji.FromName(discord, ":arrow_down:")))
            //        {

            //        }
            //    }
            //};

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}