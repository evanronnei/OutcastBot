using DSharpPlus;
using DSharpPlus.CommandsNext;
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
            #region Initialize Client
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = Configuration["Token"],
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Info
            });

            Interactivity = Client.UseInteractivity();
            #endregion

            #region Register Commands
            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = Configuration["CommandPrefix"]
            });

            Commands.RegisterCommands<Commands.Commands>();
            Commands.RegisterCommands<BuildCommands>();
            #endregion

            #region Client Events
            Client.Ready += EventHandler.ClientReadyHandler;
            Client.ClientErrored += EventHandler.ClientErrorHandler;
            Client.MessageReactionAdded += EventHandler.BuildVoteAddHandler;
            Client.MessageReactionRemoved += EventHandler.BuildVoteRemoveHandler;
            Client.MessageCreated += EventHandler.CrabHandler;
            Client.MessageDeleted += EventHandler.BuildDeleteHandler;
            Client.MessageDeleted += EventHandler.JanitorDeleteHandler;
            #endregion

            #region Command Events
            Commands.CommandErrored += EventHandler.CommandErrorHandler;
            #endregion

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}