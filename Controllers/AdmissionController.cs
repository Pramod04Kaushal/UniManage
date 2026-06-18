using Microsoft.AspNetCore.Mvc;
using UniManage.Models;
using UniManage.Models.ViewModels;

namespace UniManage.Controllers
{
    public class AdmissionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdmissionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Programs()
        {
            var courses = _context.Courses
                .Where(c => c.Status == "Active")
                .ToList();

            ViewBag.Departments =
                _context.Departments.ToList();

            return View(courses);
        }

        public IActionResult ProgramDetails(int id)
        {
            var course = _context.Courses
                .FirstOrDefault(c => c.CourseID == id);

            if (course == null)
            {
                return NotFound();
            }

            // CHECK IF CURRENT USER IS ENROLLED

            int? userId =
                HttpContext.Session.GetInt32("UserID");

            bool isEnrolled = false;

            if (userId != null)
            {
                var student = _context.Students
                    .FirstOrDefault(s => s.UserID == userId.Value);

                if (student != null)
                {
                    isEnrolled = _context.Enrollments.Any(e =>
                        e.StudentID == student.StudentID &&
                        e.CourseID == id);
                }
            }

            ViewBag.IsEnrolled = isEnrolled;

            var modules =
                (from cm in _context.CourseModules
                 join m in _context.Modules
                 on cm.ModuleID equals m.ModuleID
                 where cm.CourseID == id
                 select new ProgramModuleViewModel
                 {
                     ModuleName = m.ModuleName,
                     ModuleCode = m.ModuleCode,
                     Semester = cm.Semester,
                     Credits = m.Credits
                 }).ToList();

            var model = new ProgramDetailsViewModel
            {
                Course = course,
                Modules = modules
            };

            return View(model);
        }

        public IActionResult ApplyCourse(int id)
        {
            int? userId =
                HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction(
                    "Register",
                    "Account");
            }

            var existingApplication =
                _context.CourseApplications
                .FirstOrDefault(a =>
                    a.UserID == userId.Value &&
                    a.Status == "Pending");

            if (existingApplication != null)
            {
                TempData["Message"] =
                    "You already have a pending application.";

                return RedirectToAction(
                    "ProgramDetails",
                    new { id });
            }

            CourseApplication application =
                new CourseApplication
                {
                    UserID = userId.Value,
                    CourseID = id,
                    AppliedDate = DateTime.Now,
                    Status = "Pending"
                };

            _context.CourseApplications.Add(application);

            _context.SaveChanges();

            TempData["Message"] =
                "Your application has been submitted successfully and is awaiting administrator approval.";

            return RedirectToAction(
                "ProgramDetails",
                new { id });
        }
    }
}