using System;
using System.Collections.Generic;
using System.Linq;

namespace HoopStats.Models
{
    public static class GameStatExtensions
    {
        public static PlayerGameStatsViewModel ToPlayerGameStatsViewModel(this GameStats gameStats)
        {
            return new PlayerGameStatsViewModel
            {
                PlayerName = gameStats.Player,
                Points = gameStats.Points,
                Rebounds = gameStats.TotalRebounds,
                Assists = gameStats.Assists,
                Steals = gameStats.Steals,
                Blocks = gameStats.Blocks,
                MinutesPlayed = gameStats.MinutesPlayed,
                FGM = gameStats.FieldGoalsMade,
                FGA = gameStats.FieldGoalsAttempted,
                FGP = gameStats.FieldGoalPercentage,
                TPM = gameStats.ThreePointersMade,
                TPA = gameStats.ThreePointersAttempted,
                TPP = gameStats.ThreePointPercentage,
                FTM = gameStats.FreeThrowsMade,
                FTA = gameStats.FreeThrowsAttempted,
                FTP = gameStats.FreeThrowPercentage,
                OffRebounds = gameStats.OffensiveRebounds,
                DefRebounds = gameStats.DefensiveRebounds,
                Turnovers = gameStats.Turnovers,
                Fouls = gameStats.PersonalFouls
            };
        }
    }
}
