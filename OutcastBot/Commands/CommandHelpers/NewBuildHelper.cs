using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using OutcastBot.Ojects;
using System.Linq;
using System.Threading.Tasks;

namespace OutcastBot.Commands.CommandHelpers
{
    class NewBuildHelper
    {
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

            await message.ModifyAsync("", await build.GetEmbed());
            await message.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":arrow_up:"));
            await message.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":arrow_down:"));
        }
    }
}
