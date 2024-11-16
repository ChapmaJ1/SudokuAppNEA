using Microsoft.Data.Sqlite;
using CommonLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary.Interfaces;

namespace SQLDatabase
{
    public class DBCaller: IDatabaseCaller
    {
        public string _connectionString { get; set; }
        public DBCaller(string connectionString)
        {
            _connectionString = connectionString;
        }

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
                command.CommandText = "insert into ...";
                // var 
            }
        }

        public List<LeaderboardEntry> GetLeaderboardEntries()
        {
            List<LeaderboardEntry> data = new();
            using (SqliteConnection connection = new())
            {
                connection.Open();
                connection.ConnectionString = _connectionString;
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "select ...";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // add
                }
            }
            return data;
        }
    }
}
