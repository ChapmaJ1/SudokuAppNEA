using CommonLibrary;
using Sudoku_Solver_NEA;
//using SudokuAppNEA.Components.Models;
using SudokuAppNEA.Components.Pages;
using SharedLibrary;

namespace SudokuAppNEA.Components.Clients
{
    public class UserClient
    {
        public User? User { get; private set; }

        public string? MistakeDetection { get; private set; }
        public string? SaveScores { get; private set; }
        public DatabaseEntry? Entry { get; private set; }

        public List<Board> FetchedBoards = new List<Board>();

        public void AddEntry(int score, string difficulty, int mistakeCount, string completionTime)
        {
            Entry = new DatabaseEntry(score, DateOnly.FromDateTime(DateTime.Now).ToString(), difficulty, completionTime, mistakeCount, User!.Id);
        }
        internal void SetUser(User user)
        {
            User = user;
        }

        internal void SetMistakeDetection(string input)
        {
            MistakeDetection = input;
        }

        internal void SetSaveScores(string input)
        {
            SaveScores = input;
        }
    }
}
