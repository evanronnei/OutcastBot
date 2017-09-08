using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Commands.CommandHelpers
{
    class BuildHelper
    {
        private static string _required = "(REQUIRED) ";
        private static string _skip = "0";
        private static string _optional = $"(OPTIONAL: Type \"{_skip}\" to skip this step) ";
        private static string _delete = $" (Type \"{_skip}\" to remove this property)";

        public enum CommandType
        {
            New,
            Edit
        }

        public static async Task<string> GetPatchVersion(CommandType commandType, CommandContext context)
        {
            string patchVersion = null;

            var prefix = "";
            if (commandType == CommandType.New) prefix = _required;
            var outMessage = $"{prefix}Enter the patch version of the build. (i.e. 1.0.0.0)";

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            await message.DeleteAsync();
            if (response != null)
            {
                patchVersion = await ValidatePatchVersion(context, response.Message.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Command Timeout");
            }

            return patchVersion;
        }

        private static async Task<string> ValidatePatchVersion(CommandContext context, string message)
        {
            var match = new Regex(@"\d\.\d\.\d\.\d").Match(message);

            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                var msg = await context.RespondAsync("Invalid patch version, please re-enter the patch version. (i.e. 1.0.0.0)");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                await msg.DeleteAsync();
                if (response == null) return null;
                return await ValidatePatchVersion(context, response.Message.Content);
            }
        }

        public static async Task<string> GetTitle(CommandType commandType, CommandContext context)
        {
            string title = null;

            var prefix = "";
            if (commandType == CommandType.New) prefix = _required;
            var outMessage = $"{prefix}Enter the title of the build. (256 characters maximum)";

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));
            await message.DeleteAsync();
            if (response != null)
            {
                title = await ValidateTitle(context, response.Message.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Command Timeout");
            }

            return title;
        }

        private static async Task<string> ValidateTitle(CommandContext context, string message)
        {
            if (message.Length > 256)
            {
                var msg = await context.RespondAsync($"Title is too long ({message.Length}). Please shorten the title to 246 characters.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                await msg.DeleteAsync();
                if (response == null) return null;
                return await ValidateTitle(context, response.Message.Content);
            }
            else
            {
                return message;
            }
        }

        public static async Task<string> GetDescription(CommandType commandType, CommandContext context)
        {
            string description = null;

            var prefix = "";
            if (commandType == CommandType.New) prefix = _required;
            var outMessage = $"{prefix}Enter the description of the build.";

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(10));
            await message.DeleteAsync();
            if (response != null)
            {
                description = response.Message.Content;
            }
            else if (response == null)
            {
                await context.RespondAsync("Command Timeout");
            }

            return description;
        }

        public static async Task<string> GetBuildUrl(CommandType commandType, CommandContext context)
        {
            string buildUrl = null;

            var prefix = "";
            if (commandType == CommandType.New) prefix = _required;
            var outMessage = $"{prefix}Enter the http://www.grimtools.com/calc/ for the build";

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            await message.DeleteAsync();
            if (response != null)
            {
                buildUrl = await ValidateBuildUrl(context, response.Message.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Command Timeout");
            }

            return buildUrl;
        }

        private static async Task<string> ValidateBuildUrl(CommandContext context, string message)
        {
            var match = new Regex(@"(?<=grimtools.com/calc/)[a-zA-Z0-9]{8}").Match(message);

            if (match.Success)
            {
                return $"http://www.grimtools.com/calc/{match.Value}";
            }
            else
            {
                var msg = await context.RespondAsync("Invalid grimtools URL, please re-enter the grimtools URL.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                await msg.DeleteAsync();
                if (response == null) return null;
                return await ValidateBuildUrl(context, response.Message.Content);
            }
        }

        public static async Task<string> GetImageUrl(CommandType commandType, CommandContext context)
        {
            string imageUrl = null;

            var prefix = "";
            var suffix = "";
            if (commandType == CommandType.New)
            {
                prefix = _optional;
            }
            else if (commandType == CommandType.Edit)
            {
                suffix = _delete;
            }
            var outMessage = $"{prefix}Upload the thumbnail image for the build. (Upload attachment){suffix}";

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));
            await message.DeleteAsync();
            if (response != null && response.Message.Attachments.Count > 0 && response.Message.Content.ToLower() != _skip)
            {
                imageUrl = response.Message.Attachments[0].Url;
            }
            else if (response == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            return imageUrl;
        }

        public static async Task<string> GetForumUrl(CommandType commandType, CommandContext context)
        {
            string forumUrl = null;

            var prefix = "";
            var suffix = "";
            if (commandType == CommandType.New)
            {
                prefix = _optional;
            }
            else if (commandType == CommandType.Edit)
            {
                suffix = _delete;
            }
            var outMessage = $"{prefix}Enter the forum URL for the build.{suffix}";

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            await message.DeleteAsync();
            if (response != null && response.Message.Content.ToLower() != _skip)
            {
                forumUrl = await ValidateForumUrl(context, response.Message.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            return forumUrl;
        }

        private static async Task<string> ValidateForumUrl(CommandContext context, string message)
        {
            var match = new Regex(@"(?<=grimdawn.com/forums/showthread.php\?t=)\d*").Match(message);

            if (match.Success)
            {
                return $"http://www.grimdawn.com/forums/showthread.php?t={match.Value}";
            }
            else
            {
                var msg = await context.RespondAsync("Invalid forum URL, please re-enter the forum URL.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                await msg.DeleteAsync();
                if (response == null) return null;
                return await ValidateForumUrl(context, response.Message.Content);
            }
        }

        public static async Task<string> GetVideoUrl(CommandType commandType, CommandContext context)
        {
            string videoUrl = null;

            var prefix = "";
            var suffix = "";
            if (commandType == CommandType.New)
            {
                prefix = _optional;
            }
            else if (commandType == CommandType.Edit)
            {
                suffix = _delete;
            }
            var outMessage = $"{prefix}Enter the video URL for the build.{suffix}";

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            await message.DeleteAsync();
            if (response != null && response.Message.Content.ToLower() != _skip)
            {
                videoUrl = await ValidateVideoUrl(context, response.Message.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            return videoUrl;
        }

        private static async Task<string> ValidateVideoUrl(CommandContext context, string message)
        {
            var youTube = new Regex(@"(?<=youtu\.be\/|youtube\.com/(embed/|v/|watch\?v=|watch\?.+&v=))((\w|-){11})").Match(message);
            var streamable = new Regex(@"(?<=streamable\.com/)[a-z0-9]{5}").Match(message);

            if (youTube.Success)
            {
                return $"https://youtu.be/{youTube.Value}";
            }
            else if (streamable.Success)
            {
                return $"https://streamable.com/{streamable.Value}";
            }
            else
            {
                var msg = await context.RespondAsync("Invalid video URL, please re-enter the video URL.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                await msg.DeleteAsync();
                if (response == null) return null;
                return await ValidateVideoUrl(context, response.Message.Content);
            }
        }

        public static async Task<string> GetTags(CommandContext context)
        {
            string tags = null;

            var message = await context.RespondAsync($"{_optional}Enter the tags (emojis) for the build. (Separate each emoji with a space)");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            await message.DeleteAsync();
            if (response != null && response.Message.Content.ToLower() != _skip)
            {
                tags = ValidateTags(context, response.Message.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            return tags;
        }

        private static string ValidateTags(CommandContext context, string message)
        {
            var output = "";
            var tags = message.Split(' ').ToList();

            var converter = new DiscordEmojiConverter();
            foreach (var tag in tags)
            {
                if (converter.TryConvert(tag, context, out DiscordEmoji emoji))
                {
                    output += $"{tag} ";
                }
            }

            return output;
        }
    }
}
