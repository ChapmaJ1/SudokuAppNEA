using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabase
{
    public class DBCallerSettings
    {
        private string _connectionString = @"Data Source=C:\Users\jchap\NEA.db;Mode=ReadWrite";

        public List<string> GetUserStats(int userID)
        {
            List<string> stats = new List<string>();
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // selects the stats relating to a specified user and their completed boards
                    command.CommandText = $"SELECT avg(mistakes), avg(score), avg(hints) FROM Boards b JOIN Sessions s on b.SessionID = s.SessionID where s.UserID = @UserID";  // gets stats for a particular user, such as the average mistakes and average score per board
                    command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                    SqliteDataReader dataReader = command.ExecuteReader();
                    if (dataReader.Read() && !dataReader.IsDBNull(0))
                    {
                        stats.Add(Math.Round(dataReader.GetDouble(0), 1).ToString());
                        stats.Add(dataReader.GetInt32(1).ToString());
                        stats.Add(Math.Round(dataReader.GetDouble(2), 1).ToString());
                        // calculates and formats the user's average time per board
                        stats.Add(GetAverageTime(userID));
                        stats.Add(GetBestSessionScore(userID).ToString());
                    }
                }
            }
            return stats;
        }

        public (string, string) GetUserSettings(int userID)
        {
            (string, string) settings = ("", "");
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // fetches the user's settings saved in the database
                    command.CommandText = $"SELECT MistakeDetection, SaveScores FROM UserSettings WHERE UserID = @UserID";
                    command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                    SqliteDataReader dataReader = command.ExecuteReader();
                    dataReader.Read();
                    // returns the data in (mistakeDetection, saveScores) format
                    settings = (dataReader.GetString(0), dataReader.GetString(1));
                }
            }
            return settings;
        }

        // sets default settings for a new user, with mistake detection and score saving both on
        public void SetDefaultUserSettings(int userID)
        {
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // inserts a new entry into the UserSettings table, storing default settings and the user's ID
                    command.CommandText = $"INSERT INTO UserSettings (MistakeDetection, SaveScores, UserID) VALUES(@MistakeDetection, @SaveScores, @UserID)";
                    command.Parameters.Add("@MistakeDetection", SqliteType.Text).Value = "On";
                    command.Parameters.Add("@SaveScores", SqliteType.Text).Value = "On";
                    command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveUserSettings(string mistakeDetection, string saveScores, int userID)
        {
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // updates the user's settings in the database according to their choices in the settings page
                    command.CommandText = $"UPDATE UserSettings SET MistakeDetection = @MistakeDetection, SaveScores = @SaveScores WHERE UserID = @UserID";
                    command.Parameters.Add("@MistakeDetection", SqliteType.Text).Value = mistakeDetection;
                    command.Parameters.Add("@SaveScores", SqliteType.Text).Value = saveScores;
                    command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                    command.ExecuteNonQuery();
                }
            }
        }

        private string GetAverageTime(int userID)
        {
            int totalSeconds = 0;
            int boards = 0;
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // selects the completion time for each board completed by the user
                    command.CommandText = $"SELECT CompletionTime from Boards b JOIN Sessions s on b.SessionID = s.SessionID WHERE s.UserID = @UserID";
                    command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                    SqliteDataReader dataReader = command.ExecuteReader();
                    // for each board which the user has completed
                    while (dataReader.Read())
                    {
                        // splits each time into minutes and seconds
                        string[] timeSplit = dataReader.GetString(0).Split(':');
                        // converts each time into seconds
                        totalSeconds += Convert.ToInt32(timeSplit[0]) * 60 + Convert.ToInt32(timeSplit[1]);
                        // increments the board count so average can be used
                        boards++;
                    }
                }
            }
            int averageSeconds = totalSeconds / boards;
            // calculates average time, in seconds, and formats it in minutes:seconds
            return $"{averageSeconds / 60}:{averageSeconds % 60}";  
        }

        private int GetBestSessionScore(int userID)
        {
            int bestSessionScore = 0;
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // gets the highest average score of a single session for a user
                    command.CommandText = "SELECT avg(score) FROM Boards b JOIN Sessions s on b.SessionID = s.SessionID  WHERE s.userID = @UserID GROUP BY b.sessionID ORDER BY avg(score) DESC";
                    command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                    SqliteDataReader dataReader = command.ExecuteReader();
                    // if the query does not return NULL - the user has previously completed at least one board
                    if (dataReader.Read() && !dataReader.IsDBNull(0))
                    {
                        bestSessionScore = dataReader.GetInt32(0);
                    }
                }
            }
            return bestSessionScore;
        }
    }
}
