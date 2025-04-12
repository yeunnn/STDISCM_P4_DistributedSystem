// Broker/Controllers/BrokerController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Broker.Controllers
{
    [Route("api")]
    [ApiController]
    public class BrokerController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client;
        public BrokerController(IConfiguration config)
        {
            _config = config;
            _client = new HttpClient();
        }

        [HttpPost("login")]
        public async Task<IActionResult> ForwardLogin([FromBody] object payload)
        {
            var authUrl = _config["ServiceEndpoints:AuthService"] + "/login";
            Console.WriteLine("Forwarding login to: " + authUrl);
            try
            {
                var response = await _client.PostAsJsonAsync(authUrl, payload);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response from AuthService: " + content);
                return StatusCode((int)response.StatusCode, content);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error in forwarding login: " + ex.Message);
                return StatusCode(503, new { message = "Auth service is down" });
            }
        }

        [HttpGet("courses")]
        public async Task<IActionResult> ForwardCourses()
        {
            var courseUrl = _config["ServiceEndpoints:CourseService"] + "/courses";
            Console.WriteLine("Forwarding login to: " + courseUrl);
            try
            {
                var response = await _client.GetAsync(courseUrl);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response from Service: " + content);
                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in forwarding login: " + ex.Message);
                return StatusCode(503, new { message = "Course service is down" });
            }
        }

        [HttpGet("myenrollments")]
        public async Task<IActionResult> ForwardMyEnrollments()
        {
            var query = Request.QueryString.ToString();
            var courseUrl = _config["ServiceEndpoints:CourseService"] + "/myenrollments" + query;
            Console.WriteLine("Forwarding login to: " + courseUrl);
            try
            {
                var response = await _client.GetAsync(courseUrl);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response from Service: " + content);
                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in forwarding login: " + ex.Message);
                return StatusCode(503, new { message = "Course service is down" });
            }
        }

        [HttpPost("enroll")]
        public async Task<IActionResult> ForwardEnroll([FromBody] object payload)
        {
            var courseUrl = _config["ServiceEndpoints:CourseService"] + "/enroll";
            Console.WriteLine("Forwarding login to: " + courseUrl);
            try
            {
                var response = await _client.PostAsJsonAsync(courseUrl, payload);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response from Service: " + content);
                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in forwarding login: " + ex.Message);
                return StatusCode(503, new { message = "Course service is down" });
            }
        }

        [HttpGet("grades")]
        public async Task<IActionResult> ForwardGrades()
        {
            var query = Request.QueryString.ToString();
            var courseUrl = _config["ServiceEndpoints:CourseService"] + "/grades" + query;
            Console.WriteLine("Forwarding login to: " + courseUrl);
            try
            {
                var response = await _client.GetAsync(courseUrl);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response from Service: " + content);
                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in forwarding login: " + ex.Message);
                return StatusCode(503, new { message = "Course service is down" });
            }
        }

        [HttpPost("upload-grade")]
        public async Task<IActionResult> ForwardUploadGrade([FromBody] object payload)
        {
            var courseUrl = _config["ServiceEndpoints:CourseService"] + "/upload-grade";
            Console.WriteLine("Forwarding login to: " + courseUrl);
            try
            {
                var response = await _client.PostAsJsonAsync(courseUrl, payload);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response from Service: " + content);
                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in forwarding login: " + ex.Message);
                return StatusCode(503, new { message = "Course service is down" });
            }
        }

        [HttpGet("student-details")]
        public async Task<IActionResult> ForwardStudentDetails()
        {
            var query = Request.QueryString.ToString();
            var courseUrl = _config["ServiceEndpoints:CourseService"] + "/student-details" + query;
            Console.WriteLine("Forwarding login to: " + courseUrl);
            try
            {
                var response = await _client.GetAsync(courseUrl);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response from Service: " + content);
                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in forwarding login: " + ex.Message);
                return StatusCode(503, new { message = "Course service is down" });
            }
        }
    }
}

