using DSharpPlus;
using DSharpPlus.CommandsNext;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OutcastBot.Commands.CommandHelpers
{
    class EditBuildHelper
    {
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

            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (response != null)
            {
                var index = await CommandHelper.ValidateIndex(context, response.Content, 7);
                if (index == null)
                {
                    await context.RespondAsync("Command Timeout");
                    return;
                }

                if (index == 0) // PatchVersion
                {
                    await context.RespondAsync("What patch is this build from? (i.e. 1.0.1.1)");
                    response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                    if (response != null)
                    {
                        build.PatchVersion = await BuildHelper.ValidatePatchVersion(context, response.Content);
                    }
                    else if (response == null || build.PatchVersion == null)
                    {
                        await context.RespondAsync("Command Timeout");
                        return;
                    }
                }
                else if (index == 1) // Title
                {
                    await context.RespondAsync("What is the title of your build? (246 characters maximum)");
                    response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));
                    if (response != null)
                    {
                        build.Title = await BuildHelper.ValidateTitle(context, response.Content);
                    }
                    else if (response == null || build.Title == null)
                    {
                        await context.RespondAsync("Command Timeout");
                        return;
                    }
                }
                else if (index == 2) // BuildUrl
                {
                    await context.RespondAsync("What is the grimtools URL for your build?");
                    response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                    if (response != null)
                    {
                        build.BuildUrl = await BuildHelper.ValidateBuildUrl(context, response.Content);
                    }
                    else if (response == null || build.BuildUrl == null)
                    {
                        await context.RespondAsync("Command Timeout");
                        return;
                    }
                }
                else if (index == 3) // Description
                {
                    await context.RespondAsync("What is the description of your build? (2048 characters maximum)");
                    response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(5));
                    if (response != null)
                    {
                        build.Description = await BuildHelper.ValidateDescription(context, response.Content);
                    }
                    else if (response == null || build.Description == null)
                    {
                        await context.RespondAsync("Command Timeout");
                        return;
                    }
                }
                else if (index == 4) // ImageUrl
                {
                    await context.RespondAsync("What would you like for the thumbnail image for your build? (Upload attachment)");
                    response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(2));
                    if (response != null)
                    {
                        build.ImageUrl = response.Attachments[0].Url;
                    }
                    else if (response == null)
                    {
                        await context.RespondAsync("Option Timeout");
                    }
                }
                else if (index == 5) // ForumUrl
                {
                    await context.RespondAsync("What would you life for the forum link for your build?");
                    response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                    if (response != null)
                    {
                        build.ForumUrl = await BuildHelper.ValidateForumUrl(context, response.Content);
                    }
                    else if (response == null)
                    {
                        await context.RespondAsync("Option Timeout");
                    }
                }
                else if (index == 6) // VideoUrl
                {
                    await context.RespondAsync("What would you like video link for your build?");
                    response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                    if (response != null)
                    {
                        build.VideoUrl = BuildHelper.ValidateVideoUrl(response.Content);
                    }
                    else if (response == null)
                    {
                        await context.RespondAsync("Option Timeout");
                    }
                }

                await context.RespondAsync("Would you like to edit another property? (Y/N)");
                response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (response != null && response.Content.ToLower().StartsWith("y")) await EditProperty(context, build);
            }
        }
    }
}
