using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using OutcastBot.Enumerations;
using System.Threading.Tasks;
using static OutcastBot.Enumerations.Attributes;

namespace OutcastBot.Ojects
{
    public class BuildContext : DbContext
    {
        public DbSet<Build> Builds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=OutcastBotDatabase.db");
        }
    }

    public class Build
    {
        #region Automatically Filled Properties
        public int BuildId { get; set; }
        public Mastery Mastery { get; set; }
        public ulong AuthorId { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public ulong MessageId { get; set; }
        #endregion

        #region User Filled Properties
        // required
        public string PatchVersion { get; set; }
        public bool ExpansionRequired { get; set; }
        public string Title { get; set; }
        public string BuildUrl { get; set; }
        public string Description { get; set; }

        // optional
        public string ImageUrl { get; set; }
        public string ForumUrl { get; set; }
        public string VideoUrl { get; set; }
        #endregion

        public async Task<DiscordEmbed> GetEmbed()
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"{Title}",
                Description = Description,
                ThumbnailUrl = Mastery.GetAttribute<MasteryInfoAttribute>().ImageUrl,
                Color = new DiscordColor(Mastery.GetAttribute<MasteryInfoAttribute>().Color),
                Url = BuildUrl
            };

            if (ExpansionRequired)
            {
                embed.WithAuthor($"[{PatchVersion}][Expansion]");
            }
            else
            {
                embed.WithAuthor($"[{PatchVersion}]");
            }

            var author = await Program.Client.GetUserAsync(AuthorId);
            embed.AddField( "Author", author.Mention);
            embed.AddField("Build", BuildUrl);
            if (ForumUrl != null) embed.AddField("Forum Post", ForumUrl);
            if (VideoUrl != null) embed.AddField("Video", VideoUrl);
            if (ImageUrl != null) embed.ImageUrl = ImageUrl;

            embed.WithFooter($"ID: {BuildId}");

            return embed.Build();
        }
    }
}
