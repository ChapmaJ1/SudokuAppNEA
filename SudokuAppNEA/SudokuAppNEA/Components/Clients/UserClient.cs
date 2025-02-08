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

        public List<Board>? FetchedBoards { get; private set; }
        public int EasyFetched { get; private set; }
        public int MediumFetched { get; private set; }
        public int HardFetched { get; private set; }

        internal void InitialiseClient()
        {
            FetchedBoards = new List<Board>();
            EasyFetched = 0;
            MediumFetched = 0;
            HardFetched = 0;
        }

        internal void AddEntry(int score, string difficulty, int mistakeCount, int hintCount, string completionTime)
        {
            Entry = new DatabaseEntry(score, difficulty, completionTime, mistakeCount, hintCount, User!.SessionId);
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

        internal void IncrementDifficultiesFetched(string difficulty)
        {
            switch (difficulty)
            {
                case "Easy": 
                    EasyFetched++;
                    break;
                case "Medium":
                    MediumFetched++;
                    break;
                case "Hard":
                    HardFetched++;
                    break;
            }
        }
    }
}
