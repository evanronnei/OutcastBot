using DSharpPlus.CommandsNext;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Commands.CommandHelpers
{
    class CommandHelper
    {
        public static async Task<int?> ValidateIndex(CommandContext context, string message, int count)
        {
            var match = new Regex(@"\b\d+").Match(message);

            if (!match.Success)
            {
                await context.RespondAsync("Invalid input. Use the index to select and object.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (response == null) return null;
                return await ValidateIndex(context, response.Message.Content, count);
            }

            var index = Convert.ToInt32(message);

            if (index >= count || index < 0)
            {
                await context.RespondAsync("Invalid selection.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));
                if (response == null) return null;
                return await ValidateIndex(context, response.Message.Content, count);
            }
            else
            {
                return index;
            }
        }
    }
}
