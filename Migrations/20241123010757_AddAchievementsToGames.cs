using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AchievementsPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddAchievementsToGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppId = table.Column<int>(type: "integer", nullable: false),
                    PlaytimeForever = table.Column<int>(type: "integer", nullable: false),
                    IconUrl = table.Column<string>(type: "text", nullable: false),
                    SteamUserId = table.Column<string>(type: "text", nullable: false),
                    UserAchievements = table.Column<int>(type: "integer", nullable: true),
                    TotalAchievements = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SteamAchievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApiName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Achieved = table.Column<bool>(type: "boolean", nullable: false),
                    GameName = table.Column<string>(type: "text", nullable: false),
                    SteamId = table.Column<string>(type: "text", nullable: false),
                    UnlockDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PlayerId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SteamAchievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SteamAchievements_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SteamAchievements_PlayerId",
                table: "SteamAchievements",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "SteamAchievements");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
