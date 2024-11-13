using SudokuAppNEA.Components.Models;

namespace SudokuAppNEA.Components.Clients
{
    public class UserClient
    {
        public User User { get; set; }
        public UserClient(User userInput)
        {
            User = userInput;
        }
    }
}
