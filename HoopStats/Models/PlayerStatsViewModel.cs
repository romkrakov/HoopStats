using System;

namespace HoopStats.Models
{
    public class PlayerStatsViewModel
    {
        public string PlayerName { get; set; } = "";
        public int GamesPlayed { get; set; }
        public double AveragePoints { get; set; }
        public double AverageRebounds { get; set; }
        public double AverageAssists { get; set; }
    }
}
