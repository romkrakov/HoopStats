using System;
using System.Collections.Generic;

namespace HoopStats.Models
{
    public class GameDetailsViewModel
    {
        public DateTime GameDate { get; set; }
        public string Team1 { get; set; } = "";
        public string Team2 { get; set; } = "";
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public string Winner { get; set; } = "";
        public List<PlayerGameStatsViewModel> Team1Players { get; set; } = new List<PlayerGameStatsViewModel>();
        public List<PlayerGameStatsViewModel> Team2Players { get; set; } = new List<PlayerGameStatsViewModel>();
    }

    public class PlayerGameStatsViewModel
    {
        public string PlayerName { get; set; } = "";
        public int Points { get; set; }
        public int Rebounds { get; set; }
        public int Assists { get; set; }
        public int Steals { get; set; }
        public int Blocks { get; set; }
        public double MinutesPlayed { get; set; } // Defined as double to match GameStats
        public int FGM { get; set; } // Field Goals Made
        public int FGA { get; set; } // Field Goals Attempted
        public double FGP { get; set; } // Field Goal Percentage
        public int TPM { get; set; } // Three Pointers Made
        public int TPA { get; set; } // Three Pointers Attempted
        public double TPP { get; set; } // Three Point Percentage
        public int FTM { get; set; } // Free Throws Made
        public int FTA { get; set; } // Free Throws Attempted
        public double FTP { get; set; } // Free Throw Percentage
        public int OffRebounds { get; set; } // Offensive Rebounds
        public int DefRebounds { get; set; } // Defensive Rebounds
        public int Turnovers { get; set; }
        public int Fouls { get; set; } // Personal Fouls
        
        // Added efficiency metrics
        public double EfficiencyRating 
        { 
            get 
            {
                // Formula: (PTS + REB + AST + STL + BLK) - ((FGA - FGM) + (FTA - FTM) + TO)
                return (Points + Rebounds + Assists + Steals + Blocks) - ((FGA - FGM) + (FTA - FTM) + Turnovers);
            } 
        }
        
        public double TrueShootingPercentage
        {
            get
            {
                if (FGA == 0 && FTA == 0) return 0;
                // Formula: Points / (2 * (FGA + 0.44 * FTA))
                return Points / (2.0 * (FGA + 0.44 * FTA));
            }
        }
        
        public double AssistToTurnoverRatio
        {
            get
            {
                if (Turnovers == 0) return Assists;
                return (double)Assists / Turnovers;
            }
        }
        
        public double PointsPerMinute
        {
            get
            {
                if (MinutesPlayed == 0) return 0;
                return Points / MinutesPlayed;
            }
        }
    }
}
