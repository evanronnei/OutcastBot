using DSharpPlus.CommandsNext;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Commands.CommandHelpers
{
    class CommandHelper
    {
        public static async Task<int?> ValidateIndex(CommandContext context, string userInput, int count)
        {
            var match = new Regex(@"\b\d+\b").Match(userInput);

            if (!match.Success)
            {
                var message = await context.RespondAsync("Invalid input. Use the index to select an object.");
                var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));

                await message.DeleteAsync();

                if (response == null) return null;

                await response.Message.DeleteAsync();

                return await ValidateIndex(context, response.Message.Content, count);
            }
            else
            {
                var index = Convert.ToInt32(userInput);

                if (index >= count || index < 0)
                {
                    var message = await context.RespondAsync("Invalid selection.");
                    var response = await Program.Interactivity.WaitForMessageAsync(m => m.Author.Id == context.User.Id, TimeSpan.FromMinutes(1));

                    await message.DeleteAsync();

                    if (response == null) return null;

                    await response.Message.DeleteAsync();

                    return await ValidateIndex(context, response.Message.Content, count);
                }
                else
                {
                    return index;
                }
            }
        }
    }
}
