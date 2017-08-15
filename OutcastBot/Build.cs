using DSharpPlus;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OutcastBot
{
    public class BuildContext : DbContext
    {
        public DbSet<Build> Builds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=OutcastBotDatabase.db");
        }
    }

    /// <summary>
    /// Build object
    /// </summary>
    public class Build
    {
        #region Automatically Filled Properties
        public int BuildId { get; set; }
        public ulong AuthorId { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public ulong MessageId { get; set; }
        #endregion

        #region User Filled Properties
        // required
        public string PatchVersion { get; set; }
        public string Title { get; set; }
        public string BuildUrl { get; set; }
        public string Description { get; set; }

        // optional
        public string HeaderImageUrl { get; set; }
        public string ForumUrl { get; set; }
        public string VideoUrl { get; set; }
        public string Tags { get; set; }
        #endregion

        public async Task<DiscordEmbed> GetEmbed()
        {
            var embed = new DiscordEmbed();

            var author = await Program.Client.GetUserAsync(AuthorId);

            if (HeaderImageUrl != null) embed.Image = new DiscordEmbedImage() { Url = HeaderImageUrl };

            embed.Title = $"[{PatchVersion}] {Title}";

            embed.Fields.Add(new DiscordEmbedField() { Name = "Author", Value = author.Mention });
            embed.Fields.Add(new DiscordEmbedField() { Name = "Build", Value = BuildUrl });
            if (ForumUrl != null) embed.Fields.Add(new DiscordEmbedField() { Name = "Forum Post", Value = ForumUrl });
            if (VideoUrl != null) embed.Fields.Add(new DiscordEmbedField() { Name = "Video", Value = VideoUrl });

            embed.Description = Description;

            embed.Color = new Random().Next(0, 0xFFFFFF);

            return embed;
        }
    }
}
