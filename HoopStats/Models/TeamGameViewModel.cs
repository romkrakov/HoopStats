using System;
using System.Collections.Generic;

namespace HoopStats.Models
{
    public class TeamGameViewModel
    {
        public DateTime GameDate { get; set; }
        public required string Opponent { get; set; }
        public required string Result { get; set; }
        public int TotalPoints { get; set; }
        public required List<PlayerGameViewModel> TeamPlayers { get; set; }
    }
}
