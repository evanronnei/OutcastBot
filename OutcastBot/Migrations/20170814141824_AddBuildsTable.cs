using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OutcastBot.Migrations
{
    public partial class AddBuildsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Builds",
                columns: table => new
                {
                    BuildId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AuthorId = table.Column<ulong>(nullable: false),
                    BuildUrl = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    DownVotes = table.Column<int>(nullable: false),
                    ForumUrl = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    MessageId = table.Column<ulong>(nullable: false),
                    PatchVersion = table.Column<string>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: false),
                    UpVotes = table.Column<int>(nullable: false),
                    VideoUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Builds", x => x.BuildId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Builds");
        }
    }
}
