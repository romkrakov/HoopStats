using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoopStats.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLatestGameStatsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LatestGameStats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LatestGameStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Assists = table.Column<int>(type: "INTEGER", nullable: false),
                    Blocks = table.Column<int>(type: "INTEGER", nullable: false),
                    DefensiveRebounds = table.Column<int>(type: "INTEGER", nullable: false),
                    FieldGoalPercentage = table.Column<double>(type: "REAL", nullable: false),
                    FieldGoalsAttempted = table.Column<int>(type: "INTEGER", nullable: false),
                    FieldGoalsMade = table.Column<int>(type: "INTEGER", nullable: false),
                    FreeThrowPercentage = table.Column<double>(type: "REAL", nullable: false),
                    FreeThrowsAttempted = table.Column<int>(type: "INTEGER", nullable: false),
                    FreeThrowsMade = table.Column<int>(type: "INTEGER", nullable: false),
                    GameDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GameScore = table.Column<double>(type: "REAL", nullable: false),
                    MinutesPlayed = table.Column<double>(type: "REAL", nullable: false),
                    OffensiveRebounds = table.Column<int>(type: "INTEGER", nullable: false),
                    Opponent = table.Column<string>(type: "TEXT", nullable: false),
                    PersonalFouls = table.Column<int>(type: "INTEGER", nullable: false),
                    Player = table.Column<string>(type: "TEXT", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    Result = table.Column<string>(type: "TEXT", nullable: false),
                    Steals = table.Column<int>(type: "INTEGER", nullable: false),
                    Team = table.Column<string>(type: "TEXT", nullable: false),
                    ThreePointPercentage = table.Column<double>(type: "REAL", nullable: false),
                    ThreePointersAttempted = table.Column<int>(type: "INTEGER", nullable: false),
                    ThreePointersMade = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalRebounds = table.Column<int>(type: "INTEGER", nullable: false),
                    Turnovers = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LatestGameStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LatestGameStats_GameDate",
                table: "LatestGameStats",
                column: "GameDate");

            migrationBuilder.CreateIndex(
                name: "IX_LatestGameStats_Player",
                table: "LatestGameStats",
                column: "Player");

            migrationBuilder.CreateIndex(
                name: "IX_LatestGameStats_Team_Opponent",
                table: "LatestGameStats",
                columns: new[] { "Team", "Opponent" });
        }
    }
}
