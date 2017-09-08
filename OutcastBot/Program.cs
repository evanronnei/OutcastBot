using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.Configuration;
using OutcastBot.Commands;
using System.IO;
using System.Threading.Tasks;

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
            Client = new DiscordClient(new DiscordConfiguration
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

            Client.MessageReactionAdded += EventHandler.BuildVoteAddHandler;
            Client.MessageReactionRemoved += EventHandler.BuildVoteRemoveHandler;
            Client.MessageCreated += EventHandler.CrabHandler;
            Client.MessageCreated += EventHandler.GrimDawnForumHandler;
            Client.MessageDeleted += EventHandler.BuildDeleteHandler;
            Client.MessageDeleted += EventHandler.JanitorDeleteHandler;

            Client.Ready += async e =>
            {
                await Client.UpdateStatusAsync(new Game($"{Configuration["CommandPrefix"]}help"));
            };

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}