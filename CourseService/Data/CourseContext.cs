// CourseService/Data/CourseContext.cs
using CourseService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CourseService.Data
{
    public class CourseContext : DbContext
    {
        public CourseContext(DbContextOptions<CourseContext> options) : base(options) { }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Grade> Grades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, CourseId = "CSE101", Name = "Introduction to Computer Science" },
                new Course { Id = 2, CourseId = "CSE102", Name = "Data Structures" },
                new Course { Id = 3, CourseId = "CSE103", Name = "Distributed Systems" }
            );
        }
    }
}
