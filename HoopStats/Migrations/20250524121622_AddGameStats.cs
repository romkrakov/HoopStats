using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoopStats.Migrations
{
    /// <inheritdoc />
    public partial class AddGameStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Player = table.Column<string>(type: "TEXT", nullable: false),
                    Team = table.Column<string>(type: "TEXT", nullable: false),
                    Opponent = table.Column<string>(type: "TEXT", nullable: false),
                    Result = table.Column<string>(type: "TEXT", nullable: false),
                    MinutesPlayed = table.Column<double>(type: "REAL", nullable: false),
                    FieldGoalsMade = table.Column<int>(type: "INTEGER", nullable: false),
                    FieldGoalsAttempted = table.Column<int>(type: "INTEGER", nullable: false),
                    FieldGoalPercentage = table.Column<double>(type: "REAL", nullable: false),
                    ThreePointersMade = table.Column<int>(type: "INTEGER", nullable: false),
                    ThreePointersAttempted = table.Column<int>(type: "INTEGER", nullable: false),
                    ThreePointPercentage = table.Column<double>(type: "REAL", nullable: false),
                    FreeThrowsMade = table.Column<int>(type: "INTEGER", nullable: false),
                    FreeThrowsAttempted = table.Column<int>(type: "INTEGER", nullable: false),
                    FreeThrowPercentage = table.Column<double>(type: "REAL", nullable: false),
                    OffensiveRebounds = table.Column<int>(type: "INTEGER", nullable: false),
                    DefensiveRebounds = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalRebounds = table.Column<int>(type: "INTEGER", nullable: false),
                    Assists = table.Column<int>(type: "INTEGER", nullable: false),
                    Steals = table.Column<int>(type: "INTEGER", nullable: false),
                    Blocks = table.Column<int>(type: "INTEGER", nullable: false),
                    Turnovers = table.Column<int>(type: "INTEGER", nullable: false),
                    PersonalFouls = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    GameScore = table.Column<double>(type: "REAL", nullable: false),
                    GameDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStats", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameStats");
        }
    }
}
