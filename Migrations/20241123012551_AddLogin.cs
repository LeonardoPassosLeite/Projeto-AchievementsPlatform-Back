using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AchievementsPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SteamId",
                table: "Players",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalGames",
                table: "Players",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalHoursPlayed",
                table: "Players",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SteamId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "TotalGames",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "TotalHoursPlayed",
                table: "Players");
        }
    }
}
