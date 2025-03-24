using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary;

namespace SQLDatabase
{
    [TestClass]
    public class TestDatabase
    {
        [TestMethod]
        public void TestGetLeaderboardEntries()
        {
            DBCallerBoard dbCaller = new();
            List<LeaderboardEntry> leaderboard = dbCaller.GetLeaderboardEntries();
            List<LeaderboardEntry> expectedLeaderboard = new()
            {
                new LeaderboardEntry("David", "5228", "7:02", "Hard", "14/03/2025"),
                new LeaderboardEntry("Chapmaj", "4847", "9:13", "Hard", "13/03/2025"),
                new LeaderboardEntry("Chapmaj", "4804", "7:26", "Medium", "11/03/2025"),
                new LeaderboardEntry("Adam", "4773", "3:47", "Easy", "15/03/2025"),
                new LeaderboardEntry("Chapmaj", "4066", "5:34", "Easy", "11/03/2025")
            };
            // validates correctness of the GetLeaderboardEntries() method output
            CollectionAssert.AreEqual(expectedLeaderboard, leaderboard);
        }

        [TestMethod]
        public void TestGetLeaderboardEntriesPersonal()
        {
            DBCallerBoard dbCaller = new();
            List<LeaderboardEntry> leaderboard = dbCaller.GetLeaderboardEntriesPersonal(1);
            List<LeaderboardEntry> expectedLeaderboard = new()
            {
                new LeaderboardEntry("Chapmaj", "4847", "9:13", "Hard", "13/03/2025"),
                new LeaderboardEntry("Chapmaj", "4804", "7:26", "Medium", "11/03/2025"),
                new LeaderboardEntry("Chapmaj", "4066", "5:34", "Easy", "11/03/2025"),
            };
            // validates correctness of the GetLeaderboardEntriesPersonal() method output
            CollectionAssert.AreEqual(expectedLeaderboard, leaderboard);
        }

        [TestMethod]
        public void TestGetRecommendedDifficulty()
        {
            DBCallerBoard dbCaller = new();
            string recommendedDifficulty = dbCaller.GetRecommendedDifficulty(1);
            // validates correctness of the GetRecommendedDifficulty() method output
            Assert.AreEqual("Hard", recommendedDifficulty);
        }

        [TestMethod]
        public void TestGetUserStats()
        {
            DBCallerSettings dbCaller = new();
            List<string> stats = dbCaller.GetUserStats(1);
            List<string> expectedStats = new() { "2.7", "4572", "1", "7:24", "4847"};
            // checks whether the correct stats are returned for the given user
            CollectionAssert.AreEqual(expectedStats, stats);
        }

        [TestMethod]
        public void TestGetUserSettings()
        {
            DBCallerSettings dbCaller = new();
            (string, string) settings = dbCaller.GetUserSettings(1);
            (string, string) expectedSettings = ("On", "On");
            // checks whether the correct settings are returned for the given user
            Assert.AreEqual(expectedSettings, settings);
        }

        [TestMethod]
        public void TestSaveUserSettings()
        {
            DBCallerSettings dbCaller = new();
            dbCaller.SaveUserSettings("Off", "Off", 2);
            // tests whether the newly assigned settings are reflected in the database
            Assert.AreEqual(("Off", "Off"), dbCaller.GetUserSettings(2));
        }

        [TestMethod]
        public void TestFindUser()
        {
            DBCallerUser dbCaller = new();
            // validates that if the user's details are found in the database, their user id is returned
            int id = dbCaller.FindUser("Chapmaj", "Hello123!");
            Assert.AreEqual(1, id);
            // validates that if the user's details are not found in the database, 0 is returned
            id = dbCaller.FindUser("James", "Hello123!");
            Assert.AreEqual(0, id);
        }

        [TestMethod]
        public void TestUsernameAlreadyRegistered()
        {
            DBCallerUser dbCaller = new();
            // validates that if the database already contains the input username, the method returns true
            bool registered = dbCaller.UsernameAlreadyRegistered("Chapmaj");
            Assert.IsTrue(registered);
            // validates that if the database does not contain the input username, the method returns false
            registered = dbCaller.UsernameAlreadyRegistered("James");
            Assert.IsFalse(registered);
        }

        [TestMethod]
        public void TestGetTableCount()
        {
            // validates correctness of the GetTableCount() method for multiple tables
            DBCallerUser dbCaller = new();
            int tableCountUsers = dbCaller.GetTableCount("Users");
            Assert.AreEqual(3, tableCountUsers);
            int tableCountBoards = dbCaller.GetTableCount("Boards");
            Assert.AreEqual(5, tableCountBoards);
        }
    }
}
