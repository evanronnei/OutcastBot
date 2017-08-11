using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Commands.CommandHelpers
{
    class DeleteBuildHelper
    {
        public static async Task<int?> ValidateIndex(CommandContext context, string message, int count)
        {
            var match = new Regex(@"\b\d+\b").Match(message);

            if (!match.Success)
            {
                await context.RespondAsync("Invalid input. Use the index of the build to select it.");
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await ValidateIndex(context, msg.Content, count);
            }

            var index = Convert.ToInt32(message);

            if (index >= count || index < 0)
            {
                await context.RespondAsync("Invalid build to delete. Please select a valid build");
                var msg = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (msg == null) return null;
                return await ValidateIndex(context, msg.Content, count);
            }
            else
            {
                return index;
            }
        }
    }
}
