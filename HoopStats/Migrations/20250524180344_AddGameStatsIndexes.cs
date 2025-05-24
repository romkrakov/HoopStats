using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoopStats.Migrations
{
    /// <inheritdoc />
    public partial class AddGameStatsIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_GameStats_GameDate",
                table: "GameStats",
                column: "GameDate");

            migrationBuilder.CreateIndex(
                name: "IX_GameStats_Player",
                table: "GameStats",
                column: "Player");

            migrationBuilder.CreateIndex(
                name: "IX_GameStats_Team_Opponent",
                table: "GameStats",
                columns: new[] { "Team", "Opponent" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameStats_GameDate",
                table: "GameStats");

            migrationBuilder.DropIndex(
                name: "IX_GameStats_Player",
                table: "GameStats");

            migrationBuilder.DropIndex(
                name: "IX_GameStats_Team_Opponent",
                table: "GameStats");
        }
    }
}
