using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabase
{
    public class DBCallerUser
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
                    // adds a new user with the input username and password into the Users table
                    command.CommandText = $"INSERT INTO Users (Username, Password) VALUES (@Username,@Password)";
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
                    // selects the ID of a user within the database whose details match the input username and password
                    command.CommandText = $"SELECT UserID FROM Users WHERE Username = @Username and Password = @Password";
                    command.Parameters.Add("@Username", SqliteType.Text).Value = username;
                    command.Parameters.Add("@Password", SqliteType.Text).Value = password;
                    using (SqliteDataReader dataReader = command.ExecuteReader())
                    {
                        // if a user with the input username and password exists in the database
                        if (dataReader.Read())
                        {
                            // returns the user ID
                            return dataReader.GetInt32(0);
                        }
                    }
                }
            }
            // returns 0 to reflect that no user with matching details exists in the database
            return 0;
        }

        public void AddSession(int userId)
        {
            using (SqliteConnection connection = new SqliteConnection())
            {
                connection.ConnectionString = _connectionString;
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    // adds a new session with the date and user's ID into the Sessions table
                    command.CommandText = "INSERT INTO Sessions (CalendarDate, UserId) VALUES(@CalendarDate, @UserID)";
                    command.Parameters.Add("@CalendarDate", SqliteType.Text).Value = DateOnly.FromDateTime(DateTime.Now).ToString();
                    command.Parameters.Add("@UserID", SqliteType.Integer).Value = userId;
                    command.ExecuteNonQuery();
                }
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
                    // selects the number of entries in a given table
                    command.CommandText = $"SELECT Count(*) FROM {parameter}";
                    count = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return count;
        }
    }
}
