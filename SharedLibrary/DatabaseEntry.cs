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
        public string? CalendarDay { get; private set; }
        public string? Difficulty { get; private set; }
        public string? CompletionTime { get; private set; }
        public int Mistakes { get; private set; }
        public int UserId { get; private set; }

        public DatabaseEntry(int score, string? calendarDay, string? difficulty, string? completionTime, int mistakes, int userId)
        {
            Score = score;
            CalendarDay = calendarDay;
            Difficulty = difficulty;
            CompletionTime = completionTime;
            Mistakes = mistakes;
            UserId = userId;
        }
    }
}
