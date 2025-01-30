namespace SharedLibrary
{
    public class LeaderboardEntry
    {
        public string? Username { get; private set; }
        public string? Score { get; private set; }
        public string? Time { get; private set; }
        public string? Difficulty { get; private set; }
        public string? Date { get; private set; }

        public LeaderboardEntry(string username, string score, string time, string difficulty, string date)
        {
            Username = username;
            Score = score;
            Time = time;
            Difficulty = difficulty;
            Date = date;
        }
    }
}
