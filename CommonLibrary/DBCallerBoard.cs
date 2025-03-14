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
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // adds a new board entry to the Boards table, with all parameters provided below
                    command.CommandText = "INSERT INTO Boards (Score, Difficulty, CompletionTime, Mistakes, Hints, SessionID) VALUES (@Score, @Difficulty, @CompletionTime, @Mistakes, @Hints, @SessionID)";
                    command.Parameters.Add("@Score", SqliteType.Integer).Value = entry.Score;
                    command.Parameters.Add("@Difficulty", SqliteType.Text).Value = entry.Difficulty;
                    command.Parameters.Add("@CompletionTime", SqliteType.Text).Value = entry.CompletionTime;
                    command.Parameters.Add("@Mistakes", SqliteType.Integer).Value = entry.Mistakes;
                    command.Parameters.Add("@Hints", SqliteType.Text).Value = entry.Hints;
                    command.Parameters.Add("@SessionID", SqliteType.Integer).Value = entry.SessionId;
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<LeaderboardEntry> GetLeaderboardEntries()
        {
            List<LeaderboardEntry> data = new();
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // selects details of all boards in the database from the Boards table
                    command.CommandText = "SELECT u.Username, b.Score, b.CompletionTime, b.Difficulty, s.CalendarDate FROM Boards b JOIN Sessions s on b.SessionID = s.SessionID JOIN Users u on s.UserID = u.UserID order by b.Score DESC";
                    SqliteDataReader reader = command.ExecuteReader();
                    // fetches details on all boards returned by the query
                    while (reader.Read())
                    {
                        // creates an object for each entry and adds it to a list, which is returned to the page
                        LeaderboardEntry entry = new LeaderboardEntry(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                        data.Add(entry);
                    }
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
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // selects details of all boards in the database for a particular user
                    command.CommandText = "SELECT u.Username, b.Score, b.CompletionTime, b.Difficulty, s.CalendarDate FROM Boards b JOIN Sessions s on b.SessionID = s.SessionID JOIN Users u on s.UserID = u.UserID where u.UserID = @UserID order by b.Score DESC\r\n";
                    command.Parameters.Add("@UserID", SqliteType.Integer).Value = user.Id;
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // same process as GetLeaderboardEntries()
                        LeaderboardEntry entry = new LeaderboardEntry(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                        data.Add(entry);
                    }
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
                int score;
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // selects the average score for a user across all of their completed boards
                    command.CommandText = $"SELECT avg(score) from Boards b JOIN Sessions s on b.SessionID = s.SessionID where s.UserID = @UserID";  // gets average score of all board previously solved by user
                    command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                    var reader = command.ExecuteReader();
                    // attempts to fetch the average score from the query
                    try
                    {
                        score = reader.GetInt32(0);
                    }
                    // exception thrown if query returns NULL, which occurs when the user has played no board previously
                    // in this case recommend the "easy" difficulty
                    catch (InvalidOperationException)
                    {
                        return "Easy";
                    }
                }
                // return a recommended difficulty based on the user's previous performances, using their average score as a metric
                if (score >= 4000)
                {
                    return "Hard";
                }
                else if (score >= 3000)
                {
                    return "Medium";
                }
            }
            return "Easy";
        }
    }
}
