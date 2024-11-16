namespace CommonLibrary.Models
{
    public class LeaderboardEntry
    {
        public required string Username { get; set; }
        public required int Score { get; set; }
        public required string Date { get; set; }
    }
}
