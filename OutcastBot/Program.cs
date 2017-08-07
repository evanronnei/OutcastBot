using DSharpPlus;
using DSharpPlus.CommandsNext;
using System;
using System.Threading.Tasks;

namespace OutcastBot
{
    class Program
    {
        static DiscordClient discord;
        static CommandsNextModule commands;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            discord = new DiscordClient(new DiscordConfig
            {
                Token = "MzQ0MTc4ODU3OTE1NDQ5MzU3.DGo9Gw.beEpo6jpg0tS6mgKIXd__boYWrA",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });

            commands.RegisterCommands<Commands>();

            discord.MessageCreated += async e =>
            {
                if (e.Message.Content.Contains(DiscordEmoji.FromName(discord, ":thinking:").ToString()))
                {
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discord, ":thinking:"));
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}