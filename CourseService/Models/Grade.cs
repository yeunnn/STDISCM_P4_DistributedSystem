// CourseService/Models/Grade.cs
using System.ComponentModel.DataAnnotations;

namespace CourseService.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string CourseId { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
