﻿using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using OutcastBot.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            await context.TriggerTypingAsync();

            var prefix = (callerMethodName == "NewBuild") ? _required : "";

            var message = await context.RespondAsync($"{prefix}Enter the patch version of the build (i.e. 1.0.0.0).");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id && 
                m.Channel.Id == context.Channel.Id);

            await message.DeleteAsync();

            if (response == null)
            {
                await context.TriggerTypingAsync();
                message = await context.RespondAsync("Command Timeout");
                await Task.Delay(5000)
                        .ContinueWith(t => message.DeleteAsync())
                        .ContinueWith(t => context.Message.DeleteAsync());
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
                await context.TriggerTypingAsync();
                var message = await context.RespondAsync("Invalid patch version, please re-enter the patch version. (i.e. 1.0.0.0)");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id &&
                    m.Channel.Id == context.Channel.Id);

                await message.DeleteAsync();

                if (response == null)
                {
                    await context.TriggerTypingAsync();
                    message = await context.RespondAsync("Command Timeout");
                    await Task.Delay(5000)
                            .ContinueWith(t => message.DeleteAsync())
                            .ContinueWith(t => context.Message.DeleteAsync());
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
            await context.TriggerTypingAsync();
            var message = await context.RespondAsync($"{_required}Does this build require the expansion pack (Ashes of Malmouth)?");
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("🇾"));
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode("🇳"));
            var reaction = await Program.Interactivity.WaitForMessageReactionAsync(
                e => e == DiscordEmoji.FromUnicode("🇾") || e == DiscordEmoji.FromUnicode("🇳"),
                message,
                context.User);

            await message.DeleteAsync();

            if (reaction == null || reaction.Emoji == DiscordEmoji.FromUnicode("🇳"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region Title
        public static async Task<string> GetTitleAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            await context.TriggerTypingAsync();

            var prefix = (callerMethodName == "NewBuild") ? _required : "";

            var message = await context.RespondAsync($"{prefix}Enter the title of the build (256 characters maximum).");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id &&
                m.Channel.Id == context.Channel.Id, 
                TimeSpan.FromMinutes(2));

            await message.DeleteAsync();

            if (response == null)
            {
                await context.TriggerTypingAsync();
                message = await context.RespondAsync("Command Timeout");
                await Task.Delay(5000)
                        .ContinueWith(t => message.DeleteAsync())
                        .ContinueWith(t => context.Message.DeleteAsync());
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
                await context.TriggerTypingAsync();
                var message = await context.RespondAsync($"Title is too long ({userInput.Length}). Please shorten the title to 246 characters.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id &&
                    m.Channel.Id == context.Channel.Id, 
                    TimeSpan.FromMinutes(2));

                await message.DeleteAsync();

                if (response == null)
                {
                    await context.TriggerTypingAsync();
                    message = await context.RespondAsync("Command Timeout");
                    await Task.Delay(5000)
                            .ContinueWith(t => message.DeleteAsync())
                            .ContinueWith(t => context.Message.DeleteAsync());
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
            await context.TriggerTypingAsync();

            var prefix = (callerMethodName == "NewBuild") ? _required : "";

            var message = await context.RespondAsync($"{prefix}Enter the description of the build.");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id &&
                m.Channel.Id == context.Channel.Id, 
                TimeSpan.FromMinutes(10));

            await message.DeleteAsync();

            if (response == null)
            {
                await context.TriggerTypingAsync();
                message = await context.RespondAsync("Command Timeout");
                await Task.Delay(5000)
                        .ContinueWith(t => message.DeleteAsync())
                        .ContinueWith(t => context.Message.DeleteAsync());
                return null;
            }

            await response.Message.DeleteAsync();

            var description = response.Message.Content;

            return description;
        }
        #endregion

        #region BuildUrl & Mastery
        public static async Task<GetBuildInfoResults> GetBuildInfoAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            await context.TriggerTypingAsync();

            var prefix = (callerMethodName == "NewBuild") ? _required : "";

            var message = await context.RespondAsync($"{prefix}Enter the http://www.grimtools.com/calc/ for the build");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id &&
                m.Channel.Id == context.Channel.Id);

            await message.DeleteAsync();

            if (response == null)
            {
                await context.TriggerTypingAsync();
                message = await context.RespondAsync("Command Timeout");
                await Task.Delay(5000)
                        .ContinueWith(t => message.DeleteAsync())
                        .ContinueWith(t => context.Message.DeleteAsync());
                return null;
            }

            await response.Message.DeleteAsync();

            var buildUrl = await ValidateBuildUrlAsync(context, response.Message.Content);
            var grimToolsBuild = await GrimToolsCalc.GetGrimToolsCalcAsync(buildUrl);

            var mastery = grimToolsBuild.GetMasteryCombination();

            return new GetBuildInfoResults { BuildUrl = buildUrl, Mastery = mastery };
        }

        private static async Task<string> ValidateBuildUrlAsync(CommandContext context, string userInput)
        {
            var match = new Regex(@"(?<=(http://)?grimtools.com/calc/)[a-zA-Z0-9]{8}(?!>)").Match(userInput);

            if (match.Success)
            {
                return $"http://www.grimtools.com/calc/{match.Value}";
            }
            else
            {
                await context.TriggerTypingAsync();
                var message = await context.RespondAsync("Invalid grimtools URL, please re-enter the grimtools URL.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id &&
                    m.Channel.Id == context.Channel.Id);

                await message.DeleteAsync();

                if (response == null)
                {
                    await context.TriggerTypingAsync();
                    message = await context.RespondAsync("Command Timeout");
                    await Task.Delay(5000)
                            .ContinueWith(t => message.DeleteAsync())
                            .ContinueWith(t => context.Message.DeleteAsync());
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
            await context.TriggerTypingAsync();

            var prefix = (callerMethodName == "NewBuild") ? _optional : "";
            var suffix = (callerMethodName == "EditProperty") ? _delete : "";

            var message = await context.RespondAsync($"{prefix}Enter the forum URL for the build.{suffix}");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id &&
                m.Channel.Id == context.Channel.Id);

            await message.DeleteAsync();

            if (response == null) return null;

            await response.Message.DeleteAsync();

            if (response.Message.Content == _skip) return null;

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
                await context.TriggerTypingAsync();
                var message = await context.RespondAsync($"Invalid forum URL, please re-enter the forum URL, or type \"{_skip}\" to skip this step.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id &&
                    m.Channel.Id == context.Channel.Id);

                await message.DeleteAsync();

                if (response == null) return null;

                await response.Message.DeleteAsync();

                if (response.Message.Content == _skip) return null;

                return await ValidateForumUrlAysnc(context, response.Message.Content);
            }
        }
        #endregion

        #region VideoUrl
        public static async Task<string> GetVideoUrlAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            await context.TriggerTypingAsync();

            var prefix = (callerMethodName == "NewBuild") ? _optional : "";
            var suffix = (callerMethodName == "EditProperty") ? _delete : "";

            var message = await context.RespondAsync($"{prefix}Enter the video URL for the build (YouTube or streamable).{suffix}");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id &&
                m.Channel.Id == context.Channel.Id);

            await message.DeleteAsync();

            if (response == null) return null;

            await response.Message.DeleteAsync();

            if (response.Message.Content == _skip) return null;

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
                await context.TriggerTypingAsync();
                var message = await context.RespondAsync($"Invalid video URL, please re-enter the video URL (YouTube or streamable), or type \"{_skip}\" to skip this step");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id &&
                    m.Channel.Id == context.Channel.Id);

                await message.DeleteAsync();

                if (response == null) return null;

                await response.Message.DeleteAsync();

                if (response.Message.Content == _skip) return null;

                return await ValidateVideoUrlAsync(context, response.Message.Content);
            }
        }
        #endregion

        #region ImageUrl
        public static async Task<string> GetImageUrlAsync(CommandContext context, [CallerMemberName]string callerMethodName = "")
        {
            await context.TriggerTypingAsync();

            var prefix = (callerMethodName == "NewBuild") ? _optional : "";
            var suffix = (callerMethodName == "EditProperty") ? _delete : "";

            var message = await context.RespondAsync($"{prefix}Upload an image for the build.{suffix}");
            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id &&
                m.Channel.Id == context.Channel.Id, 
                TimeSpan.FromMinutes(2));

            await message.DeleteAsync();

            if (response == null) return null;

            if (response.Message.Attachments.Count == 0)
            {
                await response.Message.DeleteAsync();
            }

            if (response.Message.Content == _skip) return null;

            var imageUrl = await ValidateImageUrlAsync(context, response.Message.Attachments);

            return imageUrl;
        }

        private static async Task<string> ValidateImageUrlAsync(CommandContext context, IReadOnlyList<DiscordAttachment> userInput)
        {
            var acceptedExentions = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };

            if (userInput.Count == 0 || 
                acceptedExentions.FirstOrDefault(extension => extension == Path.GetExtension(userInput[0].Url.ToLower())) == null)
            {
                await context.TriggerTypingAsync();
                var message = await context.RespondAsync($"Invalid image, please re-upload your image, or type \"{_skip}\" to skip this step");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id &&
                    m.Channel.Id == context.Channel.Id, 
                    TimeSpan.FromMinutes(2));

                await message.DeleteAsync();

                if (response == null) return null;

                if (response.Message.Attachments.Count == 0)
                {
                    await response.Message.DeleteAsync();
                }

                if (response.Message.Content == _skip) return null;

                return await ValidateImageUrlAsync(context, response.Message.Attachments);
            }
            else
            {
                return userInput[0].Url;
            }
        }
        #endregion
    }
}
