using Microsoft.AspNetCore.Mvc;

namespace UniManage.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }

        public IActionResult Courses()
        {
            return View();
        }

        public IActionResult Modules()
        {
            return View();
        }

        public IActionResult Enrollments()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        public IActionResult AddStudent()
        {
            return View();
        }

        public IActionResult AddLecturer()
        {
            return View();
        }

        public IActionResult AddAdmin()
        {
            return View();
        }

        public IActionResult AddModule()
        {
            return View();
        }

        public IActionResult AddCourse()
        {
            return View();
        }
    }
}