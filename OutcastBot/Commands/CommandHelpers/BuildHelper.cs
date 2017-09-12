using DSharpPlus.CommandsNext;
using OutcastBot.Enumerations;
using OutcastBot.Objects;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
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

        public static async Task<string> GetPatchVersionAsync(CommandType commandType, CommandContext context)
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
                patchVersion = await ValidatePatchVersionAsync(context, response.Message.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Command Timeout");
            }

            return patchVersion;
        }

        private static async Task<string> ValidatePatchVersionAsync(CommandContext context, string message)
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
                return await ValidatePatchVersionAsync(context, response.Message.Content);
            }
        }

        public static async Task<string> GetTitleAsync(CommandType commandType, CommandContext context)
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
                title = await ValidateTitleAsync(context, response.Message.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Command Timeout");
            }

            return title;
        }

        private static async Task<string> ValidateTitleAsync(CommandContext context, string message)
        {
            if (message.Length > 256)
            {
                var msg = await context.RespondAsync($"Title is too long ({message.Length}). Please shorten the title to 246 characters.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                await msg.DeleteAsync();
                if (response == null) return null;
                return await ValidateTitleAsync(context, response.Message.Content);
            }
            else
            {
                return message;
            }
        }

        public static async Task<string> GetDescriptionAsync(CommandType commandType, CommandContext context)
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

        public static async Task<(string, Mastery)> GetBuildUrlAsync(CommandType commandType, CommandContext context)
        {
            string buildUrl = null;
            GrimToolsBuild grimToolsBuild = null;

            var prefix = "";
            if (commandType == CommandType.New) prefix = _required;
            var outMessage = $"{prefix}Enter the http://www.grimtools.com/calc/ for the build";

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            await message.DeleteAsync();
            if (response != null)
            {
                buildUrl = await ValidateBuildUrlAsync(context, response.Message.Content);
                grimToolsBuild = await GetGrimToolsBuildAsync(buildUrl);
            }
            else if (response == null)
            {
                await context.RespondAsync("Command Timeout");
            }

            Mastery mastery = 0;
            foreach(var key in grimToolsBuild.BuildData.Masteries.Keys)
            {
                mastery |= key;
            }

            return (buildUrl, mastery);
        }

        private static async Task<string> ValidateBuildUrlAsync(CommandContext context, string message)
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
                return await ValidateBuildUrlAsync(context, response.Message.Content);
            }
        }

        public static async Task<string> GetForumUrlAsync(CommandType commandType, CommandContext context)
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
                forumUrl = await ValidateForumUrlAysnc(context, response.Message.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            return forumUrl;
        }

        private static async Task<string> ValidateForumUrlAysnc(CommandContext context, string message)
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
                return await ValidateForumUrlAysnc(context, response.Message.Content);
            }
        }

        public static async Task<string> GetVideoUrlAsync(CommandType commandType, CommandContext context)
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
            var outMessage = $"{prefix}Enter the video URL for the build. (YouTube or streamable){suffix}";

            var message = await context.RespondAsync(outMessage);
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            await message.DeleteAsync();
            if (response != null && response.Message.Content.ToLower() != _skip)
            {
                videoUrl = await ValidateVideoUrlAsync(context, response.Message.Content);
            }
            else if (response == null)
            {
                await context.RespondAsync("Option Timeout");
            }

            return videoUrl;
        }

        private static async Task<string> ValidateVideoUrlAsync(CommandContext context, string message)
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
                var msg = await context.RespondAsync("Invalid video URL, please re-enter the video URL. (YouTube or streamable)");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                await msg.DeleteAsync();
                if (response == null) return null;
                return await ValidateVideoUrlAsync(context, response.Message.Content);
            }
        }

        public static async Task<string> GetImageUrlAsync(CommandType commandType, CommandContext context)
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
            var outMessage = $"{prefix}Upload an image for the build. (Upload attachment){suffix}";

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

        public static async Task<GrimToolsBuild> GetGrimToolsBuildAsync(string url)
        {
            var id = new Regex(@"(?<=http://www.grimtools.com/calc/)[a-zA-Z0-9]{8}").Match(url);

            var client = new HttpClient
            {
                BaseAddress = new Uri("http://www.grimtools.com/")
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var settings = new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true
            };
            var serializer = new DataContractJsonSerializer(typeof(GrimToolsBuild), settings);
            var response = await client.GetStreamAsync($"get_build_info.php/?id={id.Value}");
            var calc = serializer.ReadObject(response) as GrimToolsBuild;

            return calc;
        }
    }
}
