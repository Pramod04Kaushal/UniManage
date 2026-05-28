using Microsoft.AspNetCore.Mvc;
using UniManage.Models;
using System.Linq;

namespace UniManage.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LecturerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LECTURER DASHBOARD
        public IActionResult Dashboard()
        {
            // TEMP LECTURER USER ID

            int lecturerUserId = _context.Lecturers
                .Select(l => l.UserID)
                .FirstOrDefault();

            // GET LECTURER

            var lecturer = (from l in _context.Lecturers
                            join u in _context.Users
                            on l.UserID equals u.UserID
                            where l.UserID == lecturerUserId
                            select new
                            {
                                l.LecturerID,
                                l.Department,
                                l.Specialization,
                                u.FullName
                            }).FirstOrDefault();

            if (lecturer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // TOTAL MODULES

            int totalModules = _context.Modules
                .Count(m => m.LecturerID == lecturer.LecturerID);

            // TOTAL STUDENTS

            int totalStudents = _context.Students.Count();

            // VIEWBAG

            ViewBag.LecturerName = lecturer.FullName;
            ViewBag.Department = lecturer.Department;
            ViewBag.Specialization = lecturer.Specialization;

            ViewBag.TotalModules = totalModules;
            ViewBag.TotalStudents = totalStudents;

            // LOAD MODULES

            var modules = _context.Modules
                .Where(m => m.LecturerID == lecturer.LecturerID)
                .ToList();

            return View(modules);
        }
    }
}