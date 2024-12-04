using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace CommonLibrary
{
    public class DBCaller
    {
        private string _connectionString = @"Data Source=C:\Users\jchap\NEA.db;Mode=ReadWrite";
        public void AddUser()
        {
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.Open();
                connection.ConnectionString = _connectionString;
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "insert into ...";
                // var
            }
        }

        public void AddEntry(LeaderboardEntry entry)
        {
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.Open();
                connection.ConnectionString = _connectionString;
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "insert into  ";
                // var 
            }
        }

        public List<LeaderboardEntry> GetLeaderboardEntriesPersonal()
        {
            List<LeaderboardEntry> data = new();
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "select * from Boards b, GameSessions s, Users u on s.UserId = u.UserID where u.UserId = 1";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    LeaderboardEntry entry = new LeaderboardEntry
                    {
                        Username = reader.GetString(15),
                        Score = reader.GetString(1),
                        Date = reader.GetString(2)
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
                command.CommandText = "select * from Boards b, GameSessions s, Users u on s.UserId = u.UserID";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    LeaderboardEntry entry = new LeaderboardEntry
                    {
                        Username = reader.GetString(15),
                        Score = reader.GetString(1),
                        Date = reader.GetString(2)
                    };
                    data.Add(entry);
                }
            }
            return data;
        }
    }
}
