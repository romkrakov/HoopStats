namespace HoopStats.Models
{
    public class PlayerGameViewModel
    {
        public required string PlayerName { get; set; }
        public int Points { get; set; }
        public int Rebounds { get; set; }
        public int Assists { get; set; }
    }
}
