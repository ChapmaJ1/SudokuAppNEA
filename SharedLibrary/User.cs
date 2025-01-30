using System.ComponentModel.DataAnnotations;

namespace SharedLibrary
{
    public class User
    {
        [Required]
        public int Id { get; private set; }
        [Required]
        public string Username { get; set; } = string.Empty;   // public so values can be directly bound from the form
        [Required]
        public string Password { get; set; } = string.Empty;

        public void SetId(int id)
        {
            Id = id;
        }
    }
}
