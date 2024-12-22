using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabase
{
    public class BoardEntry
    {
        public required int Score { get; set; }
        public required string CalendarDay { get; set; }
        public required string Difficulty { get; set; }
        public required string CompletionTime { get; set; }
        public required int UserId { get; set; }
    }
}
