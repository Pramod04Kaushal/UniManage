using Microsoft.AspNetCore.Mvc;
using UniManage.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace UniManage.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // STUDENT DASHBOARD
        public IActionResult Dashboard()
        {
            int? studentUserId =
                HttpContext.Session.GetInt32("UserID");

            if (studentUserId == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            // GET STUDENT
            var student = (from s in _context.Students
                           join u in _context.Users
                           on s.UserID equals u.UserID
                           where s.UserID == studentUserId.Value
                           select new
                           {
                               s.StudentID,
                               s.RegNum,
                               s.Semester,
                               s.Department,
                               u.FullName
                           }).FirstOrDefault();

            if (student == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // TOTAL ENROLLED COURSES
            int enrolledCourses = _context.Enrollments
                .Count(e => e.StudentID == student.StudentID);

            // TOTAL MODULES
            var enrolledCourseIds = _context.Enrollments
                .Where(e => e.StudentID == student.StudentID)
                .Select(e => e.CourseID)
                .ToList();

            int totalModules = _context.CourseModules
                .Count(cm => enrolledCourseIds.Contains(cm.CourseID));

            //Pending Assignments
            int pendingAssignments =
(
                from a in _context.Assignments
                where enrolledCourseIds.Contains(a.CourseID)

            && !_context.AssignmentSubmissions.Any(s =>
                    s.AssignmentID == a.AssignmentID
                    && s.StudentID == student.StudentID)

    select a
).Count();

            // VIEWBAG
            ViewBag.StudentName = student.FullName;
            ViewBag.RegNum = student.RegNum;
            ViewBag.Department = student.Department;
            ViewBag.Semester = student.Semester;

            ViewBag.TotalCourses = enrolledCourses;
            ViewBag.TotalModules = totalModules;

            var notifications = _context.Notifications
    .Where(n => n.UserID == studentUserId.Value)
    .OrderByDescending(n => n.CreatedAt)
    .Take(5)
    .ToList();

            ViewBag.Notifications = notifications;

            // LOAD COURSES
            var courses = (from e in _context.Enrollments
                           join c in _context.Courses
                           on e.CourseID equals c.CourseID
                           where e.StudentID == student.StudentID
                           select c).ToList();

            ViewBag.PendingAssignments = 0;

            ViewBag.AverageGrade = 0;

            if (courses.Any())
            {
                ViewBag.Progress =
                    (student.Semester * 100)
                    / courses.First().Semesters;
            }
            else
            {
                ViewBag.Progress = 0;
            }

            return View(courses);
        }




        public IActionResult Profile()
        {
            int? userId =
                HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users
                .FirstOrDefault(u => u.UserID == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }

        public IActionResult EditProfile()
        {
            int? userId =
                HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users
                .FirstOrDefault(u => u.UserID == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }



        [HttpPost]
        public IActionResult EditProfile(
            User model,
            string CurrentPassword,
            string NewPassword,
            string ConfirmPassword)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.UserID == model.UserID);

            if (user == null)
            {
                return RedirectToAction("Profile");
            }

            // UPDATE PROFILE

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Address = model.Address;
            user.Username = model.Username;



            _context.SaveChanges();

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult ChangePassword(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword)
        {
            int? userId =
                HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users
                .FirstOrDefault(u => u.UserID == userId);

            if (user == null)
            {
                return RedirectToAction("Profile");
            }

            if (user.PasswordHash != CurrentPassword)
            {
                TempData["Error"] =
                    "Current password is incorrect.";

                return RedirectToAction("EditProfile");
            }

            if (NewPassword != ConfirmPassword)
            {
                TempData["Error"] =
                    "Passwords do not match.";

                return RedirectToAction("EditProfile");
            }

            user.PasswordHash = NewPassword;

            _context.SaveChanges();

            TempData["Success"] =
                "Password updated successfully.";

            return RedirectToAction("EditProfile");
        }
    }
}