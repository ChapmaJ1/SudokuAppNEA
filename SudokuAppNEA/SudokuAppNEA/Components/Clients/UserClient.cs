﻿using CommonLibrary;
using SQLDatabase;
using SudokuAppNEA.Components.Models;

namespace SudokuAppNEA.Components.Clients
{
    public class UserClient
    {
        public User? User { get; set; }

        public bool NewUser { get; set; }

        public string? MistakeDetection { get; set; }
        public string? SaveScores { get; set; }
        public BoardEntry? Entry { get; set; }
    }
}
