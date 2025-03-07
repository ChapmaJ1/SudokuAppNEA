using System.ComponentModel.DataAnnotations;

namespace SharedLibrary
{
    public class User
    {
        [Required]
        public string Username { get; set; } = string.Empty;   // public so values can be directly bound from the form
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public int Id { get; private set; }

        public void SetId(int id)
        {
            Id = id;
        }
    }
}
