using Microsoft.Data.Sqlite;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabase
{
    public class DBCallerBoard
    {
        private string _connectionString = @"Data Source=C:\Users\jchap\NEA.db;Mode=ReadWrite";

        public void AddEntry(DatabaseEntry entry)
        {
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Boards (Score, Difficulty, CompletionTime, Mistakes, Hints, SessionID) VALUES (@Score, @Difficulty, @CompletionTime, @Mistakes, @Hints, @SessionID)";
                // adds a new board entry to the Boards table, with all parameters provided below
                command.Parameters.Add("@Score", SqliteType.Integer).Value = entry.Score;
                command.Parameters.Add("@Difficulty", SqliteType.Text).Value = entry.Difficulty;
                command.Parameters.Add("@CompletionTime", SqliteType.Text).Value = entry.CompletionTime;
                command.Parameters.Add("@Mistakes", SqliteType.Integer).Value = entry.Mistakes;
                command.Parameters.Add("@Hints", SqliteType.Text).Value = entry.Hints;
                command.Parameters.Add("@SessionID", SqliteType.Integer).Value = entry.SessionId;
                command.ExecuteNonQuery();
            }
        }

        public List<LeaderboardEntry> GetLeaderboardEntries()
        {
            List<LeaderboardEntry> data = new();
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "SELECT u.Username, b.Score, b.CompletionTime, b.Difficulty, s.CalendarDate FROM Boards b JOIN Sessions s on b.SessionID = s.SessionID JOIN Users u on s.UserID = u.UserID order by b.Score DESC";
                // selects details of all boards in the database
                var reader = command.ExecuteReader();
                while (reader.Read())  // same situation as above
                {
                    LeaderboardEntry entry = new LeaderboardEntry(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                    data.Add(entry);
                }
            }
            return data;
        }

        public List<LeaderboardEntry> GetLeaderboardEntriesPersonal(User user)  // REMOVE OR ADD NEW PAGE
        {
            List<LeaderboardEntry> data = new();
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "select Username, Score, CompletionTime, Difficulty, CalendarDay from Boards b join Users u on b.UserId = u.UserID where u.UserId = @UserID ORDER BY b.Score DESC";
                // selects details of all boards in the database for a particular user
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = user.Id;
                var reader = command.ExecuteReader();
                while (reader.Read())  // for every board, create a leaderboard entry object and add it to the output list
                {
                    LeaderboardEntry entry = new LeaderboardEntry(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                    data.Add(entry);
                }
            }
            return data;
        }
        public string GetRecommendedDifficulty(int userID)
        {
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT avg(score) from Boards b JOIN Sessions s on b.SessionID = s.SessionID where s.UserID = @UserID";  // gets average score of all board previously solved by user
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                var reader = command.ExecuteReader();
                int score;   // does not use reader.Read() as avg operation returns null, instead of nothing (as select does)
                try
                {
                    score = reader.GetInt32(0);
                }
                catch (InvalidOperationException)
                {
                    return "Easy";
                }
                if (score >= 4000)
                {
                    return "Hard";
                }
                else if (score >= 3000)
                {
                    return "Medium";
                }
            }
            return "Easy"; // returns a recommended difficulty based on the user's previous performances, based on score
        }
    }
}
