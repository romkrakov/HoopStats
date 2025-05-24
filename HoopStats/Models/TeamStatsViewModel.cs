using System;

namespace HoopStats.Models
{
    public class TeamStatsViewModel
    {
        public string TeamName { get; set; } = "";
        public int GamesPlayed { get; set; }
        public double AveragePoints { get; set; }
        public string TopScorerName { get; set; } = "";
        public double TopScorerPPG { get; set; }
    }
}
