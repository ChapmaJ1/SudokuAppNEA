using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class DatabaseEntry
    {
        public int Score { get; private set; }
        public string? Difficulty { get; private set; }
        public string? CompletionTime { get; private set; }
        public int Mistakes { get; private set; }
        public int Hints { get; private set; }
        public int SessionId { get; private set; }

        public DatabaseEntry(int score, string? difficulty, string? completionTime, int mistakes, int hints, int sessionId)
        {
            Score = score;
            Difficulty = difficulty;
            CompletionTime = completionTime;
            Mistakes = mistakes;
            Hints = hints;
            SessionId = sessionId;
        }
    }
}
