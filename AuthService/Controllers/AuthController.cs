// AuthService/Controllers/AuthController.cs
using AuthService.Data;
using AuthService.DTO; // Include the namespace for LoginRequest
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace AuthService.Controllers
{
    [Route("")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthContext _context;
        private readonly IConfiguration _config;

        public AuthController(AuthContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET /health
        [HttpGet("health")]
        public IActionResult Health() => Ok(new { status = "OK" });

        // POST /login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Query the database using only username and password.
            var user = _context.Users
                .FirstOrDefault(u =>
                    u.Username.ToLower() == request.Username.ToLower()
                    && u.Password == request.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }


        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("role", user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
              issuer: _config["Jwt:Issuer"],
              audience: _config["Jwt:Audience"],
              claims: claims,
              expires: DateTime.Now.AddHours(1),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
