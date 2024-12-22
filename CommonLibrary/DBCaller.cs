﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Sqlite.Query.Internal;
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
                // int count = GetTableCount("users"); // getting count not required as boardID primary key set as autoincrement
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"insert into users (Username, Password) VALUES(@Username,@Password)";
                    command.Parameters.Add("@Username", SqliteType.Text).Value = username;
                    command.Parameters.Add("@Password", SqliteType.Text).Value = password;
                    command.ExecuteNonQuery();
                }
            }
            return GetTableCount("users");
        }
        
       /* private int GetID(string username, string password)
        {
            int id = 0;
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"select UserID from users where username = @Username and password = @Password";
                    command.Parameters.Add("@Username", SqliteType.Text).Value = username;
                    command.Parameters.Add("@Password", SqliteType.Text).Value = password;
                    var dataReader = command.ExecuteReader();
                    id = dataReader.GetInt32(0);
                }
            }
            return id;
        } */

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
                    Console.WriteLine($"Username: {username} Password: {password}");
                    var dataReader = command.ExecuteReader();
                    if (dataReader.Read())
                    {
                        return dataReader.GetInt32(0);
                    }
                }
            }
            return AddUser(username, password);
        }

        public int GetTableCount(string parameter)
        {
            int count = 0;
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"select Count(*) from {parameter}";
                var dataReader = command.ExecuteReader();
                count = Convert.ToInt32(dataReader.GetString(0));
            }
            return count;
        }

        public void AddEntry(BoardEntry entry)
        {
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "insert into Boards (Score, CalendarDay, Difficulty, CompletionTime, UserID) VALUES (@Score, @CalendarDay, @Difficulty, @CompletionTime, @UserID)";
                command.Parameters.Add("@Score", SqliteType.Integer).Value = entry.Score;
                command.Parameters.Add("@CalendarDay", SqliteType.Text).Value = entry.CalendarDay;
                command.Parameters.Add("@Difficulty", SqliteType.Text).Value = entry.Difficulty;
                command.Parameters.Add("@CompletionTime", SqliteType.Text).Value = entry.CompletionTime;
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
                command.CommandText = $"select Username, Score, CompletionTime, Difficulty, CalendarDay from Boards b, GameSessions s, Users u on s.UserId = u.UserID where u.UserId = @UserID";
                command.Parameters.Add("@UserID", SqliteType.Integer).Value = user.Id;
                var reader = command.ExecuteReader();
                while (reader.Read())
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
                command.CommandText = "select Username, Score, CompletionTime, Difficulty, CalendarDay from Boards b, GameSessions s, Users u on s.UserId = u.UserID ORDER BY b.Score DESC";
                var reader = command.ExecuteReader();
                while (reader.Read())
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
            string difficulty;
            using (SqliteConnection connection = new())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"select Difficulty, CompletionTime from Boards b, GameSessions s, Users u on b.SessionID = s.SessionID and s.UserID = {userID} ORDER BY b.Score DESC";
                var reader = command.ExecuteReader();
                //perform difficulty calculation
                difficulty = "Easy";  // example
            }
            return difficulty;
        }
    }
}
