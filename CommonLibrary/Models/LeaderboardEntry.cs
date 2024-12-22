using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace SQLDatabase.Models
{
    public class LeaderboardEntry
    {
        public required string Username { get; set; }
        public required string Score { get; set; }
        public required string Time { get; set; }
        public required string Difficulty { get; set; }
        public required string Date { get; set; }
    }
}
