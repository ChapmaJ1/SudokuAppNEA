using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary.Models;

namespace CommonLibrary.Interfaces
{
    public interface IDBCaller
    {
        string _connectionString { get; set; }

        void AddUser();
        void AddEntry(LeaderboardEntry entry);
        List<LeaderboardEntry> GetLeaderboardEntries();
    }
}
