using System;

namespace HoopStats.Models
{
    public class GameResultViewModel
    {
        public DateTime GameDate { get; set; }
        public string Team1 { get; set; } = "";
        public string Team2 { get; set; } = "";
        public string Result { get; set; } = "";
        public string Winner { get; set; } = "";
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public string TopScorer { get; set; } = "";
        public int TopScorerPoints { get; set; }
    }
}
