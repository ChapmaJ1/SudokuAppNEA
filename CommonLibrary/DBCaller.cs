using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Sqlite.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SQLDatabase;
using SQLDatabase.Models;
using SudokuAppNEA.Components.Models;
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
                    command.CommandText = $"insert into users (Username, Password) VALUES(@Username,@Password)";  // adds a new user with the input username and password into the Users table
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
                    command.CommandText = $"select * from users where username = @Username and password = @Password";
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

        public int GetTableCount(string parameter)
        {
            int count = 0;
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"select Count(*) from {parameter}";  // outputs the number of entries in a given table
                    count = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return count;
        }

        public void AddEntry(DatabaseEntry entry)
        {
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "insert into Boards (Score, CalendarDay, Difficulty, CompletionTime, Mistakes, UserID) VALUES (@Score, @CalendarDay, @Difficulty, @CompletionTime, @Mistakes, @UserID)";
                // adds a new board entry to the Boards table, with all parameters provided below
                command.Parameters.Add("@Score", SqliteType.Integer).Value = entry.Score;
                command.Parameters.Add("@CalendarDay", SqliteType.Text).Value = entry.CalendarDay;
                command.Parameters.Add("@Difficulty", SqliteType.Text).Value = entry.Difficulty;
                command.Parameters.Add("@CompletionTime", SqliteType.Text).Value = entry.CompletionTime;
                command.Parameters.Add("@Mistakes", SqliteType.Integer).Value = entry.Mistakes;
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = entry.UserId;
                command.ExecuteNonQuery();
            }
        }

        public List<LeaderboardEntry> GetLeaderboardEntriesPersonal(User user)
        {
            List<LeaderboardEntry> data = new();
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "select Username, Score, CompletionTime, Difficulty, CalendarDay from Boards b, Users u on b.UserId = u.UserID where u.UserId = @UserID ORDER BY b.Score DESC";
                // selects details of all boards in the database for a particular user
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = user.Id;
                var reader = command.ExecuteReader();
                while (reader.Read())  // for every board, create a leaderboard entry object and add it to the output list
                {
                    LeaderboardEntry entry = new LeaderboardEntry
                    {
                        Username = reader.GetString(0),
                        Score = reader.GetString(1),
                        Time = reader.GetString(2),
                        Difficulty = reader.GetString(3),
                        Date = reader.GetString(4)
                    };
                    data.Add(entry);
                }
            }
            return data;
        }

        public List<LeaderboardEntry> GetLeaderboardEntries()
        {
            List<LeaderboardEntry> data = new();
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "select Username, Score, CompletionTime, Difficulty, CalendarDay from Boards b, Users u on b.UserId = u.UserID ORDER BY b.Score DESC";
                // selects details of all boards in the database
                var reader = command.ExecuteReader();
                while (reader.Read())  // same situation as above
                {
                    LeaderboardEntry entry = new LeaderboardEntry
                    {
                        Username = reader.GetString(0),
                        Score = reader.GetString(1),
                        Time = reader.GetString(2),
                        Difficulty = reader.GetString(3),
                        Date = reader.GetString(4)
                    };
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
                command.CommandText = $"select avg(score) from boards where userid = @UserID";  // gets average score of all board previously solved by user
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int score = reader.GetInt32(0);
                    if (score >= 5000)
                    {
                        return "Hard";
                    }
                    else if (score >= 4000)
                    {
                        return "Medium";
                    }
                }
            }
            return "Easy"; // returns a recommended difficulty based on the user's previous performances
        }

        public List<string> GetUserStats(int userID)
        {
            List<string> stats = new List<string>();
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"select avg(mistakes), avg(score) from boards where userid = @UserID";  // gets stats for a particular user, such as the average mistakes and average score per board
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                var dataReader = command.ExecuteReader();
                if (dataReader.Read() && !dataReader.IsDBNull(0))   
                {
                    stats.Add(dataReader.GetInt32(0).ToString());
                    stats.Add(dataReader.GetInt32(1).ToString());
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
                command.CommandText = $"select CompletionTime from boards where userid = @UserID";  // selects the completion time for each board completed by the user
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

        public void SetUserSettings(int userID)  // sets default settings for a new user, with mistake detection and score saves both on
        {
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"insert into UserSettings (MistakeDetection, SaveScores, UserID) VALUES(@MistakeDetection, @SaveScores, @UserID)";
                command.Parameters.Add("@MistakeDetection", SqliteType.Text).Value = "On";
                command.Parameters.Add("@SaveScores", SqliteType.Text).Value = "On";
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                command.ExecuteNonQuery();
            }
        }

        public (string,string) GetUserSettings(int userID)
        {
            (string, string) settings = ("", "");
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"select MistakeDetection, SaveScores from UserSettings where UserID = @UserID";  // gets the user's settings saved in the database
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                var dataReader = command.ExecuteReader();
                dataReader.Read();
                settings = (dataReader.GetString(0), dataReader.GetString(1));  // returns in (mistakeDetection, saveScores) format
            }
            return settings;
        }

        public void SaveUserSettings(string mistakeDetection, string saveScores, int userID)
        {
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"update UserSettings set MistakeDetection = @MistakeDetection, SaveScores = @SaveScores where UserID = @UserID";
                // updates the user's settings in the database according to their choices in the settings page
                command.Parameters.Add("@MistakeDetection", SqliteType.Text).Value = mistakeDetection;
                command.Parameters.Add("@SaveScores", SqliteType.Text).Value = saveScores;
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                command.ExecuteNonQuery();
            }
        }
    }
}
