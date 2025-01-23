using CommonLibrary;
using SQLDatabase;
using Sudoku_Solver_NEA;
using SudokuAppNEA.Components.Models;
using SudokuAppNEA.Components.Pages;

namespace SudokuAppNEA.Components.Clients
{
    public class UserClient
    {
        public User? User { get; private set; }

        public string? MistakeDetection { get; private set; }
        public string? SaveScores { get; private set; }
        public DatabaseEntry? Entry { get; private set; }

        public List<Board> FetchedBoards = new List<Board>();

        public void AddEntry(string difficulty, int mistakeCount, string completionTime)
        {
            Entry = new DatabaseEntry
            {
                Score = 0,//GenerateScore(difficulty),
                CalendarDay = DateOnly.FromDateTime(DateTime.Now).ToString(),
                Difficulty = difficulty,
                CompletionTime = completionTime,
                Mistakes = mistakeCount,
                UserId = User!.Id
            };
        }
        public void SetUser(User user)
        {
            User = user;
        }

        public void SetMistakeDetection(string input)
        {
            MistakeDetection = input;
        }

        public void SetSaveScores(string input)
        {
            SaveScores = input;
        }
    }
}
