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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Build>().Ignore(b => b.Message);
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
        public string BuildUrl { get; set; }
        public string PatchVersion { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // optional
        public string HeaderImageUrl { get; set; }
        public string ForumUrl { get; set; }
        public string VideoUrl { get; set; }
        public string Tags { get; set; }
        #endregion

        #region Discord Message
        public string Message
        {
            get
            {
                string message = "";

                message += $"**[{PatchVersion}] {Title}** by {GetDiscordUser().GetAwaiter().GetResult().Mention}\n\n";
                if (HeaderImageUrl != null) message += $"{HeaderImageUrl}\n\n";
                message += $"`Build Link:` {BuildUrl}\n";
                if (ForumUrl != null) message += $"`Forum Link:` {ForumUrl}\n";
                if (VideoUrl != null) message += $"`Video Link:` {VideoUrl}\n";
                message += $"```{Description}```";               

                return message;
            }
        }
        #endregion

        private async Task<DiscordUser> GetDiscordUser()
        {
            return await Program.Client.GetUserAsync(AuthorId);
        }
    }
}
