using DSharpPlus.CommandsNext;
using OutcastBot.Enumerations;
using OutcastBot.Objects;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
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

        #region PatchVersion
        public static async Task<string> GetPatchVersionAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            string patchVersion = null;

            var prefix = "";
            if (callerMethodName == "NewBuild")
            {
                prefix = _required;
            }

            var message = await context.RespondAsync($"{prefix}Enter the patch version of the build. (i.e. 1.0.0.0)");
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
        #endregion

        #region Title
        public static async Task<string> GetTitleAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            string title = null;

            var prefix = "";
            if (callerMethodName == "NewBuild")
            {
                prefix = _required;
            }

            var message = await context.RespondAsync($"{prefix}Enter the title of the build. (256 characters maximum)");
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
        #endregion

        #region Description
        public static async Task<string> GetDescriptionAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            string description = null;

            var prefix = "";
            if (callerMethodName == "NewBuild")
            {
                prefix = _required;
            }

            var message = await context.RespondAsync($"{prefix}Enter the description of the build.");
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
        #endregion

        #region BuildUrl
        public static async Task<(string, Mastery)> GetBuildUrlAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            string buildUrl = null;
            GrimToolsBuild grimToolsBuild = null;

            var prefix = "";
            if (callerMethodName == "NewBuild")
            {
                prefix = _required;
            }

            var message = await context.RespondAsync($"{prefix}Enter the http://www.grimtools.com/calc/ for the build");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            await message.DeleteAsync();

            if (response != null)
            {
                buildUrl = await ValidateBuildUrlAsync(context, response.Message.Content);
                grimToolsBuild = await GrimToolsBuild.GetGrimToolsBuildAsync(buildUrl);
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
        #endregion

        #region ForumUrl
        public static async Task<string> GetForumUrlAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            string forumUrl = null;

            var prefix = "";
            var suffix = "";
            if (callerMethodName == "NewBuild")
            {
                prefix = _optional;
            }
            else
            {
                suffix = _delete;
            }

            var message = await context.RespondAsync($"{prefix}Enter the forum URL for the build.{suffix}");
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
        #endregion

        #region VideoUrl
        public static async Task<string> GetVideoUrlAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            string videoUrl = null;

            var prefix = "";
            var suffix = "";
            if (callerMethodName == "NewBuild")
            {
                prefix = _optional;
            }
            else
            {
                suffix = _delete;
            }

            var message = await context.RespondAsync($"{prefix}Enter the video URL for the build. (YouTube or streamable){suffix}");
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
        #endregion

        #region ImageUrl
        public static async Task<string> GetImageUrlAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            string imageUrl = null;

            var prefix = "";
            var suffix = "";
            if (callerMethodName == "NewBuild")
            {
                prefix = _optional;
            }
            else
            {
                suffix = _delete;
            }

            var message = await context.RespondAsync($"{prefix}Upload an image for the build. (Upload attachment){suffix}");
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
        #endregion
    }
}
