// CourseService/Controllers/CourseController.cs
using CourseService.Data;
using CourseService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CourseService.Controllers
{
    [Route("")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseContext _context;
        public CourseController(CourseContext context)
        {
            _context = context;
        }

        // GET /health
        [HttpGet("health")]
        public IActionResult Health() => Ok(new { status = "OK" });

        // GET /courses
        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            return Ok(new { courses });
        }

        // GET /myenrollments?username=...
        [HttpGet("myenrollments")]
        public async Task<IActionResult> GetMyEnrollments([FromQuery] string username)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.Username == username)
                .Select(e => e.CourseId)
                .ToListAsync();
            return Ok(new { enrolledCourses = enrollments });
        }

        // POST /enroll
        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll([FromBody] Enrollment enrollment)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == enrollment.CourseId);
            if (course == null)
            {
                return BadRequest(new { message = "Course not found" });
            }

            var exists = await _context.Enrollments.AnyAsync(e =>
                e.Username == enrollment.Username && e.CourseId == enrollment.CourseId);
            if (exists)
            {
                return BadRequest(new { message = "Already enrolled in this course" });
            }

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Enrolled in course {enrollment.CourseId}" });
        }

        // GET /grades?username=...
        [HttpGet("grades")]
        public async Task<IActionResult> GetGrades([FromQuery] string username)
        {
            var grades = await _context.Grades.Where(g => g.Username == username).ToListAsync();
            return Ok(new { grades });
        }

        // POST /upload-grade
        [HttpPost("upload-grade")]
        public async Task<IActionResult> UploadGrade([FromBody] Grade grade)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == grade.CourseId);
            if (course == null)
            {
                return BadRequest(new { message = "Course not found" });
            }
            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Uploaded grade for {grade.Username} in course {grade.CourseId}" });
        }

        // GET /student-details
        [HttpGet("student-details")]
        public async Task<IActionResult> GetStudentDetails()
        {
            var enrollments = await _context.Enrollments.ToListAsync();
            var grades = await _context.Grades.ToListAsync();

            var enrollmentMap = enrollments.GroupBy(e => e.Username)
                .ToDictionary(g => g.Key, g => g.Select(e => e.CourseId).ToList());

            var gradeMap = grades.GroupBy(g => g.Username)
                .ToDictionary(g => g.Key, g => g.Select(g => new { g.CourseId, g.Value }).ToList());

            var allUsernames = enrollmentMap.Keys.Union(gradeMap.Keys);
            var result = allUsernames.Select(username => new
            {
                Username = username,
                Enrollments = enrollmentMap.ContainsKey(username) ? enrollmentMap[username] : new List<string>(),
                Grades = gradeMap.ContainsKey(username) ? gradeMap[username].Cast<dynamic>().ToList() : new List<dynamic>()
            });

            return Ok(new { studentDetails = result });
        }
    }
}

