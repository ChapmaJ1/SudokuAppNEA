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

        public override bool Equals(object? obj)
        {
            // if the object input is a LeaderboardEntry object
            if (obj is LeaderboardEntry inputObject)
            {   

                // compares the called object and the input object by value
                return Username == inputObject.Username && Score == inputObject.Score && Time == inputObject.Time && Difficulty == inputObject.Difficulty && Date == inputObject.Date;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
