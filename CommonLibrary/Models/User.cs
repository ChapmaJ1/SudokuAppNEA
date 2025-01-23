using System.ComponentModel.DataAnnotations;

namespace SudokuAppNEA.Components.Models
{
    public class User
    {
        [Required]
        public int Id { get; private set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

        public void SetId(int id)
        {
            Id = id;
        }
    }
}
