using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Sqlite.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SharedLibrary;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CommonLibrary
{
    public class DBCaller
    {
        private string _connectionString = @"Data Source=C:\Users\jchap\NEA.db;Mode=ReadWrite";

        public int AddUser(string username, string password)
        {
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO Users (Username, Password) VALUES (@Username,@Password)";  // adds a new user with the input username and password into the Users table
                    command.Parameters.Add("@Username", SqliteType.Text).Value = username;
                    command.Parameters.Add("@Password", SqliteType.Text).Value = password;
                    command.ExecuteNonQuery();
                }
            }
            return GetTableCount("users");
        }

        public int FindUser(string username, string password)
        {
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT UserID FROM Users WHERE Username = @Username and Password = @Password";
                    command.Parameters.Add("@Username", SqliteType.Text).Value = username;
                    command.Parameters.Add("@Password", SqliteType.Text).Value = password;
                    using (SqliteDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())  // if a user with the input username and password exists in the database
                        {
                            return dataReader.GetInt32(0);  // returns the user ID, with false representing the fact that the user is not new
                        }
                    }
                }
            }
            return 0;  // adds a new user with the given details to the database, with true representing the fact that the user is new
        }

        public void AddSession(int userId)
        {
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Sessions (CalendarDate, UserId) VALUES(@CalendarDate, @UserID)";
                command.Parameters.Add("@CalendarDate", SqliteType.Text).Value = DateOnly.FromDateTime(DateTime.Now).ToString();
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = userId;
                command.ExecuteNonQuery();
            }
        }

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

        public int GetTableCount(string parameter)
        {
            int count = 0;
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT Count(*) FROM {parameter}";  // outputs the number of entries in a given table
                    count = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return count;
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

        public List<string> GetUserStats(int userID)
        {
            List<string> stats = new List<string>();
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT avg(mistakes), avg(score), avg(hints) FROM Boards b JOIN Sessions s on b.SessionID = s.SessionID where s.UserID = @UserID";  // gets stats for a particular user, such as the average mistakes and average score per board
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                var dataReader = command.ExecuteReader();
                if (dataReader.Read() && !dataReader.IsDBNull(0))   
                {
                    stats.Add(Math.Round(dataReader.GetDouble(0),1).ToString());
                    stats.Add(dataReader.GetInt32(1).ToString());
                    stats.Add(Math.Round(dataReader.GetDouble(2),1).ToString());
                    stats.Add(GetAverageTime(userID));  // calculates the user's average time per board 
                }
            }
            return stats;
        }

        public string GetAverageTime(int userID)
        {
            int totalSeconds = 0;
            int boards = 0;
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT CompletionTime from Boards b join Sessions s on b.SessionID = s.SessionID where s.UserID = @UserID";  // selects the completion time for each board completed by the user
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                var dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    string[] timeSplit = dataReader.GetString(0).Split(':');  // splits each time into minutes and seconds
                    totalSeconds += Convert.ToInt32(timeSplit[0]) * 60 + Convert.ToInt32(timeSplit[1]);  // converts each time into seconds
                    boards++;  // increments the board count so average can be used
                }
            }
            int averageSeconds = totalSeconds / boards;
            return $"{averageSeconds / 60}:{averageSeconds % 60}";  // calculates average time, in seconds, and formats it in minutes:seconds
        }

        public (string,string) GetUserSettings(int userID)
        {
            (string, string) settings = ("", "");
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT MistakeDetection, SaveScores FROM UserSettings WHERE UserID = @UserID";  // gets the user's settings saved in the database
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                var dataReader = command.ExecuteReader();
                dataReader.Read();
                settings = (dataReader.GetString(0), dataReader.GetString(1));  // returns in (mistakeDetection, saveScores) format
            }
            return settings;
        }

        public void SetDefaultUserSettings(int userID)  // sets default settings for a new user, with mistake detection and score saves both on
        {
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"INSERT INTO UserSettings (MistakeDetection, SaveScores, UserID) VALUES(@MistakeDetection, @SaveScores, @UserID)";
                command.Parameters.Add("@MistakeDetection", SqliteType.Text).Value = "On";
                command.Parameters.Add("@SaveScores", SqliteType.Text).Value = "On";
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                command.ExecuteNonQuery();
            }
        }

        public void SaveUserSettings(string mistakeDetection, string saveScores, int userID)
        {
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"UPDATE UserSettings SET MistakeDetection = @MistakeDetection, SaveScores = @SaveScores WHERE UserID = @UserID";
                // updates the user's settings in the database according to their choices in the settings page
                command.Parameters.Add("@MistakeDetection", SqliteType.Text).Value = mistakeDetection;
                command.Parameters.Add("@SaveScores", SqliteType.Text).Value = saveScores;
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                command.ExecuteNonQuery();
            }
        }
    }
}
