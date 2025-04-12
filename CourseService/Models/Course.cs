// CourseService/Models/Course.cs
using System.ComponentModel.DataAnnotations;

namespace CourseService.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CourseId { get; set; }    // e.g., "CSE101"
        [Required]
        public string Name { get; set; }
    }
}
