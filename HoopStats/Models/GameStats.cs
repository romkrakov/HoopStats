using System;
using System.ComponentModel.DataAnnotations;

namespace HoopStats.Models
{
    public class GameStats
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public required string Player { get; set; }
        
        [Required]
        public required string Team { get; set; }
        
        [Required]
        public required string Opponent { get; set; }
        
        [Required]
        public required string Result { get; set; }
        
        public double MinutesPlayed { get; set; }
        public int FieldGoalsMade { get; set; }
        public int FieldGoalsAttempted { get; set; }
        public double FieldGoalPercentage { get; set; }
        public int ThreePointersMade { get; set; }
        public int ThreePointersAttempted { get; set; }
        public double ThreePointPercentage { get; set; }
        public int FreeThrowsMade { get; set; }
        public int FreeThrowsAttempted { get; set; }
        public double FreeThrowPercentage { get; set; }
        public int OffensiveRebounds { get; set; }
        public int DefensiveRebounds { get; set; }
        public int TotalRebounds { get; set; }
        public int Assists { get; set; }
        public int Steals { get; set; }
        public int Blocks { get; set; }
        public int Turnovers { get; set; }
        public int PersonalFouls { get; set; }
        public int Points { get; set; }
        public double GameScore { get; set; }
        public DateTime GameDate { get; set; }
    }
}
