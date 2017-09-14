using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using OutcastBot.Enumerations;
using OutcastBot.Objects;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Commands.CommandHelpers
{
    class BuildHelper
    {
        #region Message prefix and suffix strings
        private static string _required = "(REQUIRED) ";
        private static string _skip = "0";
        private static string _optional = $"(OPTIONAL: Type \"{_skip}\" to skip this step) ";
        private static string _delete = $" (Type \"{_skip}\" to remove this property)";
        #endregion    

        #region PatchVersion
        public static async Task<string> GetPatchVersionAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            var prefix = "";
            if (callerMethodName == "NewBuild")
            {
                prefix = _required;
            }

            var message = await context.RespondAsync($"{prefix}Enter the patch version of the build (i.e. 1.0.0.0).");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));

            await message.DeleteAsync();

            if (response == null)
            {
                await context.RespondAsync("Command Timeout");
                return null;
            }

            await response.Message.DeleteAsync();

            var patchVersion = await ValidatePatchVersionAsync(context, response.Message.Content);

            return patchVersion;
        }

        private static async Task<string> ValidatePatchVersionAsync(CommandContext context, string userInput)
        {
            var match = new Regex(@"\d\.\d\.\d\.\d").Match(userInput);

            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                var message = await context.RespondAsync("Invalid patch version, please re-enter the patch version. (i.e. 1.0.0.0)");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));

                await message.DeleteAsync();

                if (response == null)
                {
                    await context.RespondAsync("Command Timeout");
                    return null;
                }

                await response.Message.DeleteAsync();

                return await ValidatePatchVersionAsync(context, response.Message.Content);
            }
        }
        #endregion

        #region ExpansionRequired
        public static async Task<bool> GetExpansionRequiredAsync(CommandContext context)
        {
            var message = await context.RespondAsync($"{_required}Does this build require the expansion pack (Ashes of Malmouth)?");
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("🇾"));
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("🇳"));
            var reaction = await Program.Interactivity.WaitForMessageReactionAsync(
                e => e == DiscordEmoji.FromUnicode("🇾") || e == DiscordEmoji.FromUnicode("🇳"),
                message,
                TimeSpan.FromMinutes(1),
                context.User.Id);

            await message.DeleteAsync();

            if (reaction == null)
            {
                await context.RespondAsync("Command Timeout");
                return false;
            }

            if (reaction.Emoji == DiscordEmoji.FromUnicode("🇾"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Title
        public static async Task<string> GetTitleAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            var prefix = "";
            if (callerMethodName == "NewBuild")
            {
                prefix = _required;
            }

            var message = await context.RespondAsync($"{prefix}Enter the title of the build (256 characters maximum).");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));

            await message.DeleteAsync();

            if (response == null)
            {
                await context.RespondAsync("Command Timeout");
                return null;
            }

            await response.Message.DeleteAsync();

            var title = await ValidateTitleAsync(context, response.Message.Content);

            return title;
        }

        private static async Task<string> ValidateTitleAsync(CommandContext context, string userInput)
        {
            if (userInput.Length > 256)
            {
                var message = await context.RespondAsync($"Title is too long ({userInput.Length}). Please shorten the title to 246 characters.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));

                await message.DeleteAsync();

                if (response == null)
                {
                    await context.RespondAsync("Command Timeout");
                    return null;
                }

                await response.Message.DeleteAsync();

                return await ValidateTitleAsync(context, response.Message.Content);
            }
            else
            {
                return userInput;
            }
        }
        #endregion

        #region Description
        public static async Task<string> GetDescriptionAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            var prefix = "";
            if (callerMethodName == "NewBuild")
            {
                prefix = _required;
            }

            var message = await context.RespondAsync($"{prefix}Enter the description of the build.");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(10));

            await message.DeleteAsync();

            if (response == null)
            {
                await context.RespondAsync("Command Timeout");
                return null;
            }

            await response.Message.DeleteAsync();

            var description = response.Message.Content;

            return description;
        }
        #endregion

        #region BuildUrl
        public static async Task<(string, Mastery)> GetBuildUrlAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            var prefix = "";
            if (callerMethodName == "NewBuild")
            {
                prefix = _required;
            }

            var message = await context.RespondAsync($"{prefix}Enter the http://www.grimtools.com/calc/ for the build");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));

            await message.DeleteAsync();

            if (response == null)
            {
                await context.RespondAsync("Command Timeout");
                return (null, 0);
            }

            await response.Message.DeleteAsync();

            var buildUrl = await ValidateBuildUrlAsync(context, response.Message.Content);
            var grimToolsBuild = await GrimToolsBuild.GetGrimToolsBuildAsync(buildUrl);

            Mastery mastery = 0;
            foreach(var key in grimToolsBuild.BuildData.Masteries.Keys)
            {
                mastery |= key;
            }

            return (buildUrl, mastery);
        }

        private static async Task<string> ValidateBuildUrlAsync(CommandContext context, string userInput)
        {
            var match = new Regex(@"(?<=grimtools.com/calc/)[a-zA-Z0-9]{8}").Match(userInput);

            if (match.Success)
            {
                return $"http://www.grimtools.com/calc/{match.Value}";
            }
            else
            {
                var message = await context.RespondAsync("Invalid grimtools URL, please re-enter the grimtools URL.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));

                await message.DeleteAsync();

                if (response == null)
                {
                    await context.RespondAsync("Command Timeout");
                    return null;
                }

                await response.Message.DeleteAsync();

                return await ValidateBuildUrlAsync(context, response.Message.Content);
            }
        }
        #endregion

        #region ForumUrl
        public static async Task<string> GetForumUrlAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
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

            if (response == null)
            {
                await context.RespondAsync("Option Timeout");
                return null;
            }
            if (response.Message.Content == _skip)
            {
                await response.Message.DeleteAsync();
                return null;
            }

            await response.Message.DeleteAsync();
            
            var forumUrl = await ValidateForumUrlAysnc(context, response.Message.Content);

            return forumUrl;
        }

        private static async Task<string> ValidateForumUrlAysnc(CommandContext context, string userInput)
        {
            var match = new Regex(@"(?<=grimdawn.com/forums/showthread.php\?t=)\d*").Match(userInput);

            if (match.Success)
            {
                return $"http://www.grimdawn.com/forums/showthread.php?t={match.Value}";
            }
            else
            {
                var message = await context.RespondAsync("Invalid forum URL, please re-enter the forum URL.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));

                await message.DeleteAsync();

                if (response == null)
                {
                    await context.RespondAsync("Command Timeout");
                    return null;
                }

                await response.Message.DeleteAsync();

                return await ValidateForumUrlAysnc(context, response.Message.Content);
            }
        }
        #endregion

        #region VideoUrl
        public static async Task<string> GetVideoUrlAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
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

            var message = await context.RespondAsync($"{prefix}Enter the video URL for the build (YouTube or streamable).{suffix}");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));

            await message.DeleteAsync();

            if (response == null)
            {
                await context.RespondAsync("Option Timeout");
                return null;
            }
            if (response.Message.Content == _skip)
            {
                await response.Message.DeleteAsync();
                return null;
            }

            await response.Message.DeleteAsync();

            var videoUrl = await ValidateVideoUrlAsync(context, response.Message.Content);

            return videoUrl;
        }

        private static async Task<string> ValidateVideoUrlAsync(CommandContext context, string userInput)
        {
            var youTube = new Regex(@"(?<=youtu\.be\/|youtube\.com/(embed/|v/|watch\?v=|watch\?.+&v=))((\w|-){11})").Match(userInput);
            var streamable = new Regex(@"(?<=streamable\.com/)[a-z0-9]{5}").Match(userInput);

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
                var message = await context.RespondAsync("Invalid video URL, please re-enter the video URL. (YouTube or streamable)");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));

                await message.DeleteAsync();

                if (response == null)
                {
                    await context.RespondAsync("Command Timeout");
                    return null;
                }

                await response.Message.DeleteAsync();

                return await ValidateVideoUrlAsync(context, response.Message.Content);
            }
        }
        #endregion

        #region ImageUrl
        public static async Task<string> GetImageUrlAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
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

            var message = await context.RespondAsync($"{prefix}Upload an image for the build (Upload attachment).{suffix}");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));

            await message.DeleteAsync();

            if (response == null)
            {
                await context.RespondAsync("Option Timeout");
                return null;
            }
            if (response.Message.Content == _skip)
            {
                await response.Message.DeleteAsync();
                return null;
            }

            await response.Message.DeleteAsync();

            var imageUrl = response.Message.Attachments[0].Url;

            return imageUrl;
        }
        #endregion
    }
}
