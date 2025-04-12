// CourseService/Models/Enrollment.cs
using System.ComponentModel.DataAnnotations;

namespace CourseService.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string CourseId { get; set; }
    }
}
