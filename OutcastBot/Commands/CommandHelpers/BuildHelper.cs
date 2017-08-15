using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Commands.CommandHelpers
{
    class BuildHelper
    {
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
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await ValidateBuildUrl(context, msg.Content);
            }
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
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await ValidatePatchVersion(context, msg.Content);
            }
        }

        public static async Task<string> ValidateTitle(CommandContext context, string message)
        {
            if (message.Length > 246)
            {
                await context.RespondAsync($"Title is too long ({message.Length}). Please shorten your title to 246 characters.");
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await ValidateTitle(context, msg.Content);
            }
            else
            {
                return message;
            }
        }

        public static async Task<string> ValidateDescription(CommandContext context, string message)
        {
            if (message.Length > 2048)
            {
                await context.RespondAsync($"Description is too long ({message.Length}). Please shorten your description to 2048 characters.");
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await ValidateDescription(context, msg.Content);
            }
            else
            {
                return message;
            }
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
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await ValidateForumUrl(context, msg.Content);
            }
        }

        public static string ValidateVideoUrl(string message)
        {
            return message;
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

        public static async Task PostBuild(CommandContext context, Build build)
        {
            var channel = context.Guild.Channels.FirstOrDefault(ch => ch.Name == "builds");
            if (channel == null) return;

            var message = await channel.SendMessageAsync("", false, await build.GetEmbed());

            build.MessageId = message.Id;

            using (var db = new BuildContext())
            {
                db.Builds.Add(build);
                await db.SaveChangesAsync();
            }

            await message.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":arrow_up:"));
            await Task.Delay(250);
            await message.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":arrow_down:"));

            if (build.Tags != null)
            {
                var converter = new DiscordEmojiConverter();
                foreach (var tag in build.Tags.Split(' ').ToList())
                {
                    var emoji = new DiscordEmoji();
                    if (converter.TryConvert(tag, context, out emoji))
                    {
                        await message.CreateReactionAsync(emoji);
                        await Task.Delay(250);
                    }
                }
            }
        }

        public static async Task<int?> ValidateIndex(CommandContext context, string message, int count)
        {
            var match = new Regex(@"\b\d+\b").Match(message);

            if (!match.Success)
            {
                await context.RespondAsync("Invalid input. Use the index to select.");
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await ValidateIndex(context, msg.Content, count);
            }

            var index = Convert.ToInt32(message);

            if (index >= count || index < 0)
            {
                await context.RespondAsync("Invalid selection.");
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await ValidateIndex(context, msg.Content, count);
            }
            else
            {
                return index;
            }
        }

        public static async Task EditProperty(CommandContext context, Build build)
        {
            // TODO come up with a better solution to this
            var propertyList = new StringBuilder();
            propertyList.AppendLine("**0** - Patch Version");
            propertyList.AppendLine("**1** - Title");
            propertyList.AppendLine("**2** - Build URL");
            propertyList.AppendLine("**3** - Description");
            propertyList.AppendLine("**4** - Thumbnail Image");
            propertyList.AppendLine("**5** - Forum URL");
            propertyList.AppendLine("**6** - Video URL");
            var embed = new DiscordEmbed() { Description = propertyList.ToString() };
            await context.RespondAsync("Which property would you like to edit?", false, embed);

            var message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (message != null)
            {
                var index = await ValidateIndex(context, message.Content, 7);
                if (index == null)
                {
                    await context.RespondAsync("Command Timeout");
                    return;
                }

                if (index == 0) // PatchVersion
                {
                    await context.RespondAsync("What patch is this build from? (i.e. 1.0.1.1)");
                    message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                    if (message != null)
                    {
                        build.PatchVersion = await ValidatePatchVersion(context, message.Content);
                    }
                    else if (message == null || build.PatchVersion == null)
                    {
                        await context.RespondAsync("Command Timeout");
                        return;
                    }
                }
                else if (index == 1) // Title
                {
                    await context.RespondAsync("What is the title of your build? (246 characters maximum)");
                    message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));
                    if (message != null)
                    {
                        build.Title = await ValidateTitle(context, message.Content);
                    }
                    else if (message == null || build.Title == null)
                    {
                        await context.RespondAsync("Command Timeout");
                        return;
                    }
                }
                else if (index == 2) // BuildUrl
                {
                    await context.RespondAsync("What is the grimtools URL for your build?");
                    message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                    if (message != null)
                    {
                        build.BuildUrl = await ValidateBuildUrl(context, message.Content);
                    }
                    else if (message == null || build.BuildUrl == null)
                    {
                        await context.RespondAsync("Command Timeout");
                        return;
                    }
                }
                else if (index == 3) // Description
                {
                    await context.RespondAsync("What is the description of your build? (2048 characters maximum)");
                    message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(5));
                    if (message != null)
                    {
                        build.Description = await BuildHelper.ValidateDescription(context, message.Content);
                    }
                    else if (message == null || build.Description == null)
                    {
                        await context.RespondAsync("Command Timeout");
                        return;
                    }
                }
                else if (index == 4) // ImageUrl
                {
                    await context.RespondAsync("What would you like for the thumbnail image for your build? (Upload attachment)");
                    message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));
                    if (message != null)
                    {
                        build.ImageUrl = message.Attachments[0].Url;
                    }
                    else if (message == null)
                    {
                        await context.RespondAsync("Option Timeout");
                    }
                }
                else if (index == 5) // ForumUrl
                {
                    await context.RespondAsync("What would you life for the forum link for your build?");
                    message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                    if (message != null)
                    {
                        build.ForumUrl = await BuildHelper.ValidateForumUrl(context, message.Content);
                    }
                    else if (message == null)
                    {
                        await context.RespondAsync("Option Timeout");
                    }
                }
                else if (index == 6) // VideoUrl
                {
                    await context.RespondAsync("What would you like video link for your build?");
                    message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                    if (message != null)
                    {
                        build.VideoUrl = ValidateVideoUrl(message.Content);
                    }
                    else if (message == null)
                    {
                        await context.RespondAsync("Option Timeout");
                    }
                }

                await context.RespondAsync("Would you like to edit another property? (Y/N)");
                message = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (message != null && message.Content.ToLower().StartsWith("y")) await EditProperty(context, build);
            }
        }
    }
}
