using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OutcastBot;
using OutcastBot.Ojects;

namespace OutcastBot.Migrations
{
    [DbContext(typeof(BuildContext))]
    partial class BuildContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("OutcastBot.Build", b =>
                {
                    b.Property<int>("BuildId")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("AuthorId");

                    b.Property<string>("BuildUrl");

                    b.Property<string>("Description");

                    b.Property<int>("DownVotes");

                    b.Property<string>("ForumUrl");

                    b.Property<string>("ImageUrl");

                    b.Property<ulong>("MessageId");

                    b.Property<string>("PatchVersion");

                    b.Property<string>("Tags");

                    b.Property<string>("Title");

                    b.Property<int>("UpVotes");

                    b.Property<string>("VideoUrl");

                    b.HasKey("BuildId");

                    b.ToTable("Builds");
                });
        }
    }
}
