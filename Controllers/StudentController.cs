using Microsoft.AspNetCore.Mvc;
using UniManage.Models;
using System.Linq;

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
            int studentUserId = _context.Students
    .Select(s => s.UserID)
    .FirstOrDefault();

            // GET STUDENT
            var student = (from s in _context.Students
                           join u in _context.Users
                           on s.UserID equals u.UserID
                           where s.UserID == studentUserId
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
            int totalModules = _context.Modules
                .Count(m => m.Department == student.Department);

            // VIEWBAG
            ViewBag.StudentName = student.FullName;
            ViewBag.RegNum = student.RegNum;
            ViewBag.Department = student.Department;
            ViewBag.Semester = student.Semester;

            ViewBag.TotalCourses = enrolledCourses;
            ViewBag.TotalModules = totalModules;

            // LOAD COURSES
            var courses = (from e in _context.Enrollments
                           join c in _context.Courses
                           on e.CourseID equals c.CourseID
                           where e.StudentID == student.StudentID
                           select c).ToList();

            return View(courses);
        }
    }
}