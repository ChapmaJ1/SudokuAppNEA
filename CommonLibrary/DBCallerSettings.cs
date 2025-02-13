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
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT avg(mistakes), avg(score), avg(hints) FROM Boards b JOIN Sessions s on b.SessionID = s.SessionID where s.UserID = @UserID";  // gets stats for a particular user, such as the average mistakes and average score per board
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                var dataReader = command.ExecuteReader();
                if (dataReader.Read() && !dataReader.IsDBNull(0))
                {
                    stats.Add(Math.Round(dataReader.GetDouble(0), 1).ToString());
                    stats.Add(dataReader.GetInt32(1).ToString());
                    stats.Add(Math.Round(dataReader.GetDouble(2), 1).ToString());
                    stats.Add(GetAverageTime(userID));  // calculates the user's average time per board 
                    stats.Add(GetBestSessionScore(userID).ToString());
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

        private string GetAverageTime(int userID)
        {
            int totalSeconds = 0;
            int boards = 0;
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT CompletionTime from Boards b JOIN Sessions s on b.SessionID = s.SessionID WHERE s.UserID = @UserID";  // selects the completion time for each board completed by the user
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

        private int GetBestSessionScore(int userID)  // gets the highest average score of a single session for a user
        {
            int bestSessionScore = 0;
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "SELECT avg(score) FROM Boards b JOIN Sessions s on b.SessionID = s.SessionID  WHERE s.userID = @UserID GROUP BY b.sessionID ORDER BY avg(score) DESC";
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = userID;
                var dataReader = command.ExecuteReader();
                if (dataReader.Read() && !dataReader.IsDBNull(0))
                {
                    bestSessionScore = dataReader.GetInt32(0);
                }
            }
            return bestSessionScore;
        }
    }
}
