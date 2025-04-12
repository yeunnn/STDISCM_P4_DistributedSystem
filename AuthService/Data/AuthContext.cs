// AuthService/Data/AuthContext.cs
using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AuthService.Data
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        // Seed dummy users
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "student1", Password = "password", Role = "student" },
                new User { Id = 2, Username = "faculty1", Password = "password", Role = "faculty" }
            );
        }
    }
}
