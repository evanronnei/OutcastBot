using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using OutcastBot.Commands;
using OutcastBot.Objects;
using System.IO;
using System.Threading.Tasks;

namespace OutcastBot
{
    class Program
    {
        public static ApplicationSettings AppSettings { get; set; }
        public static DiscordClient Client { get; set; }
        public static InteractivityModule Interactivity { get; set; }
        public static CommandsNextModule Commands { get; set; }

        static void Main(string[] args)
        {
            using (var fs = new FileStream($"{Directory.GetCurrentDirectory()}/appsettings.json", FileMode.Open))
            using (var sr = new StreamReader(fs))
            {
                var settings = sr.ReadToEnd();
                AppSettings = JsonConvert.DeserializeObject<ApplicationSettings>(settings);
            }
            
            RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            #region Initialize Client
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = AppSettings.Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Info
            });

            Interactivity = Client.UseInteractivity();
            #endregion

            #region Register Commands
            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = AppSettings.CommandPrefix
            });

            Commands.RegisterCommands<Commands.Commands>();
            Commands.RegisterCommands<BuildCommands>();
            #endregion

            #region Client Events
            Client.Ready += EventHandler.ClientReadyHandler;
            Client.ClientErrored += EventHandler.ClientErrorHandler;
            Client.MessageReactionAdded += EventHandler.BuildVoteAddHandler;
            Client.MessageReactionRemoved += EventHandler.BuildVoteRemoveHandler;
            Client.MessageCreated += EventHandler.GrimToolsHandler;
            Client.MessageCreated += EventHandler.CrabHandler;
            Client.MessageCreated += EventHandler.ExpansionWhenHandler;
            Client.MessageCreated += Thinkematics.ThinkingHandler;
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