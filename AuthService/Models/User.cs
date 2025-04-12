// AuthService/Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; } // In production, store hashed passwords!
        [Required]
        public string Role { get; set; }     // "student" or "faculty"
    }
}
