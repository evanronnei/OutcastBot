using Microsoft.EntityFrameworkCore;

namespace OutcastBot.Objects
{
    public class TagContext : DbContext
    {
        public DbSet<Tag> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source=OutcastBotDatabase.db");
        }
    }
    public class Tag
    {
        public int TagId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
