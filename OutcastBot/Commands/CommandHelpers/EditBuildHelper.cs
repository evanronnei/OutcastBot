using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using OutcastBot.Ojects;
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
            var embed = new DiscordEmbedBuilder() { Description = propertyList.ToString() };
            var message = await context.RespondAsync("Which property would you like to edit?", false, embed.Build());

            var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
            if (response != null)
            {
                var index = await CommandHelper.ValidateIndex(context, response.Message.Content, 7);
                if (index == null)
                {
                    await context.RespondAsync("Command Timeout");
                    return;
                }

                await message.DeleteAsync();

                if (index == 0) // PatchVersion
                {
                    build.PatchVersion = await BuildHelper.GetPatchVersion(BuildHelper.CommandType.Edit, context) ?? build.PatchVersion;
                }
                else if (index == 1) // Title
                {
                    build.Title = await BuildHelper.GetTitle(BuildHelper.CommandType.Edit, context) ?? build.Title;
                }
                else if (index == 2) // BuildUrl
                {
                    build.BuildUrl = await BuildHelper.GetTitle(BuildHelper.CommandType.Edit, context) ?? build.BuildUrl;
                }
                else if (index == 3) // Description
                {
                    build.Description = await BuildHelper.GetDescription(BuildHelper.CommandType.Edit, context) ?? build.Description;
                }
                else if (index == 4) // ImageUrl
                {
                    build.ImageUrl = await BuildHelper.GetImageUrl(BuildHelper.CommandType.Edit, context);
                }
                else if (index == 5) // ForumUrl
                {
                    build.ForumUrl = await BuildHelper.GetForumUrl(BuildHelper.CommandType.Edit, context);
                }
                else if (index == 6) // VideoUrl
                {
                    build.VideoUrl = await BuildHelper.GetVideoUrl(BuildHelper.CommandType.Edit, context);
                }

                message = await context.RespondAsync("Would you like to edit another property?");
                await message.CreateReactionAsync(DiscordEmoji.FromUnicode("🇾"));
                await message.CreateReactionAsync(DiscordEmoji.FromUnicode("🇳"));
                var reaction = await Program.Interactivity.WaitForMessageReactionAsync(e => e == DiscordEmoji.FromUnicode("🇾") || e == DiscordEmoji.FromUnicode("🇳"), 
                    message, TimeSpan.FromMinutes(1), context.User.Id);
                await message.DeleteAsync();
                if (reaction != null && reaction.Emoji == DiscordEmoji.FromUnicode("🇾"))
                {
                    await EditProperty(context, build);
                }
            }
        }
    }
}
