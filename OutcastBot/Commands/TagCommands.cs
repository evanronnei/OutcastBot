using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using OutcastBot.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OutcastBot.Commands
{
    [Group("tag", CanInvokeWithoutSubcommand = true)]
    [Description("Gets a tag (key/value pair)")]
    public class TagCommands
    {
        public async Task ExecuteGroupAsync(CommandContext context, [Description("Tag name")]string key)
        {
            await context.TriggerTypingAsync();

            using (var db = new TagContext())
            {
                var tag = db.Tags.FirstOrDefault(t => t.Key.ToLower() == key.ToLower());

                if (tag == null)
                {
                    var error = await context.RespondAsync($"`{key}` is not a valid tag. " +
                        $"Type `{Program.AppSettings.CommandPrefix}tag list` for a list of tags.");
                    await Task.Delay(5000)
                        .ContinueWith(t => error.DeleteAsync())
                        .ContinueWith(t => context.Message.DeleteAsync());
                    return;
                }

                await context.RespondAsync(tag.Value);
            }
        }

        [Command("submit")]
        [Description("Submit a new tag for moderator approval")]
        public async Task SubmitTag(CommandContext context, [Description("Tag name")]string key, [Description("Tag value"), RemainingText]string value)
        {
            if (String.IsNullOrEmpty(value)) throw new ArgumentException();

            await context.TriggerTypingAsync();

            var moderation = context.Guild.Channels.FirstOrDefault(ch => ch.Name == "moderation");
            if (moderation == null) return;

            using (var db = new TagContext())
            {
                var tag = db.Tags.FirstOrDefault(t => t.Key.ToLower() == key.ToLower());
                if (tag != null)
                {
                    var error = await context.RespondAsync($"Tag `{key}` already exists");
                    await Task.Delay(5000)
                        .ContinueWith(t => error.DeleteAsync())
                        .ContinueWith(t => context.Message.DeleteAsync());
                    return;
                }
            }

            await context.RespondAsync($"{context.User.Mention} your tag `{key}` has been submitted for approval");

            var embed = new DiscordEmbedBuilder { Timestamp = context.Message.Timestamp };

            var author = context.User as DiscordMember;

            embed.WithAuthor($"{author.Nickname ?? author.Username}", "", author.AvatarUrl);
            embed.AddField("Key", $"`{key}`");
            embed.AddField("Value", value);

            var message = await moderation.SendMessageAsync("New tag submission:", false, embed.Build());

            var approval = DiscordEmoji.FromUnicode("✅");
            var denial = DiscordEmoji.FromUnicode("❌");
            await message.CreateReactionAsync(approval);
            await message.CreateReactionAsync(denial);

            await Task.Delay(2000);

            var response = await Program.Interactivity.WaitForMessageReactionAsync(
                e => e == approval || e == denial,
                message,
                null,
                TimeSpan.FromHours(8));

            await message.DeleteAsync();

            await moderation.TriggerTypingAsync();

            if (response == null || response.Emoji == denial)
            {
                await moderation.SendMessageAsync($"Denied tag `{key}`");

                await context.Member.SendMessageAsync($"{denial} Your tag has been denied", false, embed.Build());
            }
            else
            {
                await moderation.SendMessageAsync($"Created tag `{key}`");

                using (var db = new TagContext())
                {
                    db.Tags.Add(new Tag { Key = key, Value = value });
                    await db.SaveChangesAsync();
                }

                await context.Member.SendMessageAsync($"{approval} Your tag has been approved", false, embed.Build());
            }
        }

        [Command("list")]
        [Description("Lists all tags")]
        public async Task ListTags(CommandContext context)
        {
            await context.TriggerTypingAsync();

            var tags = new List<Tag>();
            using (var db = new TagContext())
            {
                tags = db.Tags.ToList();
            }

            if (tags.Count == 0)
            {
                var error = await context.RespondAsync("There are no created tags");
                await Task.Delay(5000)
                    .ContinueWith(t => error.DeleteAsync())
                    .ContinueWith(t => context.Message.DeleteAsync());
                return;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Tags",
                Description = String.Join(", ", tags.OrderBy(t => t.Key).Select(t => $"`{t.Key}`"))
            };

            await context.RespondAsync("", false, embed.Build());
        }

        [Command("new")]
        [Description("Creates a new tag")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task NewTag(CommandContext context, [Description("Tag name")]string key, [Description("Tag value"), RemainingText]string value)
        {
            if (String.IsNullOrEmpty(value)) throw new ArgumentException();

            await context.TriggerTypingAsync();

            using (var db = new TagContext())
            {
                var tag = db.Tags.FirstOrDefault(t => t.Key.ToLower() == key.ToLower());

                if (tag != null)
                {
                    var error = await context.RespondAsync($"Tag `{key}` already exists");
                    await Task.Delay(5000)
                        .ContinueWith(t => error.DeleteAsync())
                        .ContinueWith(t => context.Message.DeleteAsync());
                    return;
                }

                tag = new Tag { Key = key, Value = value };
                db.Add(tag);
                await db.SaveChangesAsync();
            }

            await context.RespondAsync($"Created tag `{key}`");
        }

        [Command("rename")]
        [Description("Rename the tag")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task RenameTag(CommandContext context, [Description("Old tag name")]string oldKey, [Description("New tag name")]string newKey)
        {
            await context.TriggerTypingAsync();

            using (var db = new TagContext())
            {
                var tag = db.Tags.FirstOrDefault(t => t.Key.ToLower() == oldKey.ToLower());

                if (tag == null)
                {
                    var error = await context.RespondAsync($"`{oldKey}` is not a valid tag. " +
                        $"Type `{Program.AppSettings.CommandPrefix}tag list` for a list of tags.");
                    await Task.Delay(5000)
                        .ContinueWith(t => error.DeleteAsync())
                        .ContinueWith(t => context.Message.DeleteAsync());
                    return;
                }

                tag.Key = newKey;
                db.Update(tag);
                await db.SaveChangesAsync();
            }

            await context.RespondAsync($"Renamed tag `{oldKey}` to `{newKey}`");
        }

        [Command("edit")]
        [Description("Edits an existing tag.")]
        [Aliases("modify")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task EditTag(CommandContext context, [Description("Tag name")]string key, [Description("Tag value"), RemainingText]string value)
        {
            if (String.IsNullOrEmpty(value)) throw new ArgumentException();

            await context.TriggerTypingAsync();

            using (var db = new TagContext())
            {
                var tag = db.Tags.FirstOrDefault(t => t.Key.ToLower() == key.ToLower());

                if (tag == null)
                {
                    var error = await context.RespondAsync($"`{key}` is not a valid tag. " +
                        $"Type `{Program.AppSettings.CommandPrefix}tag list` for a list of tags.");
                    await Task.Delay(5000)
                        .ContinueWith(t => error.DeleteAsync())
                        .ContinueWith(t => context.Message.DeleteAsync());
                    return;
                }

                tag.Value = value;
                db.Update(tag);
                await db.SaveChangesAsync();
            }

            await context.RespondAsync($"Edited tag `{key}`");
        }

        [Command("delete")]
        [Description("Deletes an existing tag")]
        [Aliases("remove")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task DeleteTag(CommandContext context, [Description("Tag name")]string key)
        {
            await context.TriggerTypingAsync();

            using (var db = new TagContext())
            {
                var tag = db.Tags.FirstOrDefault(t => t.Key.ToLower() == key.ToLower());

                if (tag == null)
                {
                    var error = await context.RespondAsync($"`{key}` is not a valid tag. " +
                        $"Type `{Program.AppSettings.CommandPrefix}tag list` for a list of tags.");
                    await Task.Delay(5000)
                        .ContinueWith(t => error.DeleteAsync())
                        .ContinueWith(t => context.Message.DeleteAsync());
                    return;
                }

                db.Remove(tag);
                await db.SaveChangesAsync();
            }

            await context.RespondAsync($"Deleted tag `{key}`");
        }
    }
}
