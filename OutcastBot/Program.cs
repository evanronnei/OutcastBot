﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System;
using System.Linq;
using System.Threading.Tasks;
using OutcastBot.Commands;
using System.Collections.Generic;

namespace OutcastBot
{
    class Program
    {
        //static DiscordClient client;
        static CommandsNextModule commands;
        static InteractivityModule interactivity;

        static void Main(string[] args)
        {
            Shared.Builds = new List<Build>();
            Shared.Client = new DiscordClient(new DiscordConfig
            {
                Token = "MzQ0Mjc3MjU0MDYwMjQ0OTkz.DGqYsA.EPzg-jTrABKT_MY8mctlQ5OBUl8",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            interactivity = Shared.Client.UseInteractivity();

            commands = Shared.Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });

            commands.RegisterCommands<Commands.Commands>();
            commands.RegisterCommands<BuildCommands>();

            Shared.Client.MessageCreated += async e =>
            {
                if (e.Message.Content.Contains(DiscordEmoji.FromName(Shared.Client, ":thinking:").ToString()))
                {
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName(Shared.Client, ":thinking:"));
                }
            };

            Shared.Client.MessageReactionAdd += BuildVoteAddHandler;
            Shared.Client.MessageReactionRemove += BuildVoteRemoveHandler;

            await Shared.Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task BuildVoteAddHandler(MessageReactionAddEventArgs e)
        {
            if (e.Channel.Name == "builds")
            {
                var build = Shared.Builds.FirstOrDefault(b => b.MessageId == e.Message.Id);
                if (build == null) return;

                if (e.Emoji.Equals(DiscordEmoji.FromName(Shared.Client, ":arrow_up:")))
                {
                    build.UpVotes++;
                }
                else if (e.Emoji.Equals(DiscordEmoji.FromName(Shared.Client, ":arrow_down:")))
                {
                    build.DownVotes++;
                }

                await DeleteDownvotedBuild(build, e.Message);
            }
        }

        private static Task BuildVoteRemoveHandler(MessageReactionRemoveEventArgs e)
        {
            if (e.Channel.Name == "builds")
            {
                var build = Shared.Builds.FirstOrDefault(b => b.MessageId == e.Message.Id);
                if (build == null) return Task.CompletedTask;

                if (e.Emoji.Equals(DiscordEmoji.FromName(Shared.Client, ":arrow_up:")))
                {
                    build.UpVotes--;
                }
                else if (e.Emoji.Equals(DiscordEmoji.FromName(Shared.Client, ":arrow_down:")))
                {
                    build.DownVotes--;
                }
            }

            return Task.CompletedTask;
        }

        private static async Task DeleteDownvotedBuild(Build build, DiscordMessage message)
        {
            if (build.UpVotes + build.DownVotes >= 10 && build.UpVotes / build.DownVotes <= 0.42)
            {
                await message.DeleteAsync();
                Shared.Builds.Remove(build);
            }
        }
    }
}