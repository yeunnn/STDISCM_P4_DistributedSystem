// Frontend/Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client;

        public HomeController(IConfiguration config)
        {
            _config = config;
            _client = new HttpClient();
        }

        // GET: /Home/Index (Homepage)
        public IActionResult Index() => View();

        // GET: /Home/Login
        public IActionResult Login() => View();

        // POST: /Home/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var brokerUrl = _config["BrokerUrl"] + "/login";
            System.Diagnostics.Debug.WriteLine("Broker URL: " + brokerUrl);
            Console.WriteLine("DEBUG: Broker URL: " + brokerUrl);

            var payload = new { username, password };
            var response = await _client.PostAsJsonAsync(brokerUrl, payload);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.Error = JObject.Parse(errorContent)["message"]?.ToString() ?? "Login failed";
                return View();
            }
            var json = await response.Content.ReadAsStringAsync();
            var token = JObject.Parse(json)["token"]?.ToString();
            HttpContext.Session.SetString("Token", token);
            HttpContext.Session.SetString("Username", username);
            HttpContext.Session.SetString("Role", username == "faculty1" ? "faculty" : "student");
            return RedirectToAction("Dashboard");
        }

        // GET: /Home/Dashboard
        public IActionResult Dashboard() => View();

        // GET: /Home/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: /Home/Courses
        public async Task<IActionResult> Courses()
        {
            if (HttpContext.Session.GetString("Token") == null)
                return RedirectToAction("Login");

            var brokerUrl = _config["BrokerUrl"] + "/courses";
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("Token"));
            JArray courses;
            try
            {
                var response = await _client.GetAsync(brokerUrl);
                var json = await response.Content.ReadAsStringAsync();
                courses = JArray.Parse(JObject.Parse(json)["courses"].ToString());
            }
            catch
            {
                ViewBag.Message = "Unable to connect to course service.";
                courses = new JArray();
            }

            // Get enrollments for current user
            var enrollmentsUrl = _config["BrokerUrl"] + "/myenrollments?username=" + HttpContext.Session.GetString("Username");
            JArray enrolledCourses;
            try
            {
                var response = await _client.GetAsync(enrollmentsUrl);
                var json = await response.Content.ReadAsStringAsync();
                enrolledCourses = JArray.Parse(JObject.Parse(json)["enrolledCourses"].ToString());
            }
            catch
            {
                enrolledCourses = new JArray();
            }

            ViewBag.EnrolledCourses = enrolledCourses;
            return View(courses);
        }

        // POST: /Home/Enroll
        [HttpPost]
        public async Task<IActionResult> Enroll(string courseId)
        {
            if (HttpContext.Session.GetString("Token") == null)
                return RedirectToAction("Login");

            var brokerUrl = _config["BrokerUrl"] + "/enroll";
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("Token"));
            var payload = new { username = HttpContext.Session.GetString("Username"), courseId = courseId };
            try
            {
                var response = await _client.PostAsJsonAsync(brokerUrl, payload);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = JObject.Parse(errorContent)["message"]?.ToString() ?? "Error enrolling in course";
                }
                else
                {
                    TempData["Message"] = JObject.Parse(await response.Content.ReadAsStringAsync())["message"]?.ToString();
                }
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Courses");
        }

        // GET: /Home/Grades
        public async Task<IActionResult> Grades()
        {
            if (HttpContext.Session.GetString("Token") == null)
                return RedirectToAction("Login");

            var username = HttpContext.Session.GetString("Username");
            var brokerUrl = _config["BrokerUrl"] + "/grades?username=" + username;
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("Token"));
            JArray grades;
            try
            {
                var response = await _client.GetAsync(brokerUrl);
                var json = await response.Content.ReadAsStringAsync();
                grades = JArray.Parse(JObject.Parse(json)["grades"].ToString());
            }
            catch
            {
                ViewBag.Error = "Unable to connect to course service.";
                grades = new JArray();
            }
            return View(grades);
        }

        // POST: /Home/UploadGrade
        [HttpPost]
        public async Task<IActionResult> UploadGrade(string studentUsername, string courseId, string grade)
        {
            if (HttpContext.Session.GetString("Token") == null)
                return RedirectToAction("Login");

            var brokerUrl = _config["BrokerUrl"] + "/upload-grade";
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("Token"));
            // Make sure property names match the Grade model in CourseService
            var payload = new { Username = studentUsername, CourseId = courseId, Value = grade };
            try
            {
                var response = await _client.PostAsJsonAsync(brokerUrl, payload);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = JObject.Parse(errorContent)["message"]?.ToString() ?? "Error uploading grade";
                }
                else
                {
                    TempData["Message"] = JObject.Parse(await response.Content.ReadAsStringAsync())["message"]?.ToString();
                }
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Dashboard");
        }

        // GET: /Home/StudentDetails
        public async Task<IActionResult> StudentDetails()
        {
            if (HttpContext.Session.GetString("Role") != "faculty")
                return RedirectToAction("Dashboard");

            var brokerUrl = _config["BrokerUrl"] + "/student-details";
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("Token"));
            JArray details;
            try
            {
                var response = await _client.GetAsync(brokerUrl);
                var json = await response.Content.ReadAsStringAsync();
                details = JArray.Parse(JObject.Parse(json)["studentDetails"].ToString());
            }
            catch
            {
                ViewBag.Error = "Unable to connect to course service.";
                details = new JArray();
            }
            return View(details);
        }
    }
}


