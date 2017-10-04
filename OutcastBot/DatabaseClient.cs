using Microsoft.EntityFrameworkCore;
using OutcastBot.Objects;

namespace OutcastBot
{
    public class BuildContext : DbContext
    {
        public DbSet<Build> Builds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Program.AppSettings.DatabaseConnectionString);
        }
    }

    public class TagContext : DbContext
    {
        public DbSet<Tag> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Program.AppSettings.DatabaseConnectionString);
        }
    }
}
