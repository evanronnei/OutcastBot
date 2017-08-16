﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Commands.CommandHelpers
{
    class BuildHelper
    {
        private static string _required = "(REQUIRED)";
        private static string _skip = "0";
        private static string _optional = $"(OPTIONAL: Type \"{_skip}\" to skip this step)";
        private static string _delete = $"(Type \"{_skip}\" to remove this property)";

        public enum CommandType
        {
            New,
            Edit
        }

        public static async Task<string> GetPatchVersion(CommandType commandType, CommandContext context)
        {
            string patchVersion = null;

            string outMessage;
            if (commandType == CommandType.New)
            {
                outMessage = $"{_required} What patch is this build for? (i.e. 1.0.0.0)";
            }
            else
            {
                outMessage = "What patch is this build for? (i.e. 1.0.0.0)";
            }

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (response != null)
            {
                patchVersion = await ValidatePatchVersion(context, response.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Command Timeout");
            }

            await message.DeleteAsync();

            return patchVersion;
        }

        public static async Task<string> ValidatePatchVersion(CommandContext context, string message)
        {
            var match = new Regex(@"\d\.\d\.\d\.\d").Match(message);

            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                await context.RespondAsync("Invalid patch version, please re-enter your patch version.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (response == null) return null;
                return await ValidatePatchVersion(context, response.Content);
            }
        }

        public static async Task<string> GetTitle(CommandType commandType, CommandContext context)
        {
            string title = null;

            string outMessage;
            if (commandType == CommandType.New)
            {
                outMessage = $"{_required} What is the title of your build? (246 characters maximum)";
            }
            else
            {
                outMessage = $"What is the title of your build? (246 characters maximum)";
            }

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));
            if (response != null)
            {
                title = await ValidateTitle(context, response.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Command Timeout");
            }

            await message.DeleteAsync();

            return title;
        }

        public static async Task<string> ValidateTitle(CommandContext context, string message)
        {
            if (message.Length > 246)
            {
                await context.RespondAsync($"Title is too long ({message.Length}). Please shorten your title to 246 characters.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (response == null) return null;
                return await ValidateTitle(context, response.Content);
            }
            else
            {
                return message;
            }
        }

        public static async Task<string> GetDescription(CommandType commandType, CommandContext context)
        {
            string description = null;

            string outMessage;
            if (commandType == CommandType.New)
            {
                outMessage = $"{_required} What is the description of your build? (2048 characters maximum)";
            }
            else
            {
                outMessage = $"What is the description of your build? (2048 characters maximum)";
            }

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(10));
            if (response != null)
            {
                description = await ValidateDescription(context, response.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Command Timeout");
            }

            await message.DeleteAsync();

            return description;
        }

        public static async Task<string> ValidateDescription(CommandContext context, string message)
        {
            if (message.Length > 2048)
            {
                await context.RespondAsync($"Description is too long ({message.Length}). Please shorten your description to 2048 characters.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (response == null) return null;
                return await ValidateDescription(context, response.Content);
            }
            else
            {
                return message;
            }
        }

        public static async Task<string> GetBuildUrl(CommandType commandType, CommandContext context)
        {
            string buildUrl = null;

            string outMessage;
            if (commandType == CommandType.New)
            {
                outMessage = $"{_required} What is the grimtools URL for your build?";
            }
            else
            {
                outMessage = $"What is the grimtools URL for your build?";
            }

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (response != null)
            {
                buildUrl = await ValidateBuildUrl(context, response.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Command Timeout");
            }

            await message.DeleteAsync();

            return buildUrl;
        }

        public static async Task<string> ValidateBuildUrl(CommandContext context, string message)
        {
            var match = new Regex(@"(?<=grimtools.com/calc/)[a-zA-Z0-9]{8}").Match(message);

            if (match.Success)
            {
                return $"http://www.grimtools.com/calc/{match.Value}";
            }
            else
            {
                await context.RespondAsync("Invalid grimtools URL, please re-enter your grimtools URL.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (response == null) return null;
                return await ValidateBuildUrl(context, response.Content);
            }
        }

        public static async Task<string> GetImageUrl(CommandType commandType, CommandContext context)
        {
            string imageUrl = null;

            string outMessage;
            if (commandType == CommandType.New)
            {
                outMessage = $"{_optional} What is the thumbnail image for your build? (Upload attachment)";
            }
            else
            {
                outMessage = $"What is the thumbnail image for your build? (Upload attachment) {_delete}";
            }

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));
            if (response != null && response.Attachments.Count > 0 && response.Content.ToLower() != _skip)
            {
                imageUrl = response.Attachments[0].Url;
            }
            else if (response == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            await message.DeleteAsync();

            return imageUrl;
        }

        public static async Task<string> GetForumUrl(CommandType commandType, CommandContext context)
        {
            string forumUrl = null;

            string outMessage;
            if (commandType == CommandType.New)
            {
                outMessage = $"{_optional} What is the forum link for your build?";
            }
            else
            {
                outMessage = $"What is the forum link for your build? {_delete}";
            }

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (response != null && response.Content.ToLower() != _skip)
            {
                forumUrl = await ValidateForumUrl(context, response.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            await message.DeleteAsync();

            return forumUrl;
        }

        public static async Task<string> ValidateForumUrl(CommandContext context, string message)
        {
            var match = new Regex(@"(?<=grimdawn.com/forums/showthread.php\?t=)\d*").Match(message);

            if (match.Success)
            {
                return $"http://www.grimdawn.com/forums/showthread.php?t={match.Value}";
            }
            else
            {
                await context.RespondAsync("Invalid forum URL, please re-enter your forum URL.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (response == null) return null;
                return await ValidateForumUrl(context, response.Content);
            }
        }

        public static async Task<string> GetVideoUrl(CommandType commandType, CommandContext context)
        {
            string videoUrl = null;

            string outMessage;
            if (commandType == CommandType.New)
            {
                outMessage = $"{_optional} What is the video link for your build?";
            }
            else
            {
                outMessage = $"What is the video link for your build? {_delete}";
            }

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (response != null && response.Content.ToLower() != _skip)
            {
                videoUrl = ValidateVideoUrl(response.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            await message.DeleteAsync();

            return videoUrl;
        }

        // TODO implement video URL validation
        public static string ValidateVideoUrl(string message)
        {
            return message;
        }

        public static async Task<string> GetTags(CommandContext context)
        {
            string tags = null;

            var message = await context.RespondAsync($"{_optional} What are the tags (emojis) to your build? (Separate each emoji with a space)");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (response != null && response.Content.ToLower() != _skip)
            {
                tags = ValidateTags(context, response.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            await message.DeleteAsync();

            return tags;
        }

        public static string ValidateTags(CommandContext context, string message)
        {
            var output = "";
            var tags = message.Split(' ').ToList();

            var converter = new DiscordEmojiConverter();
            foreach (var tag in tags)
            {
                var emoji = new DiscordEmoji();
                if (converter.TryConvert(tag, context, out emoji))
                {
                    output += $"{tag} ";
                }
            }

            return output;
        }
    }
}
