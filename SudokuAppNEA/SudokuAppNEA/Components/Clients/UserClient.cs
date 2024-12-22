using CommonLibrary;
using SQLDatabase;
using SudokuAppNEA.Components.Models;

namespace SudokuAppNEA.Components.Clients
{
    public class UserClient
    {
        public User? User { get; set; }
        public BoardEntry? Entry { get; set; }
        
    }
}
