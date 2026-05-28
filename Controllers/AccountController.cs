using Microsoft.AspNetCore.Mvc;
using UniManage.Models;
using System.Linq;

namespace UniManage.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LOGIN PAGE
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN PROCESS
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // CHECK USER FROM DATABASE

            var user = _context.Users
                .FirstOrDefault(u =>
                    u.Email == email &&
                    u.PasswordHash == password);

            // USER FOUND

            if (user != null)
            {
                // ADMIN LOGIN

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                // STUDENT LOGIN

                if (user.Role == "Student")
                {
                    return RedirectToAction("Dashboard", "Student");
                }

                // LECTURER LOGIN

                if (user.Role == "Lecturer")
                {
                    return RedirectToAction("Dashboard", "Lecturer");
                }
            }

            // LOGIN FAILED

            ViewBag.Error = "Invalid Email or Password";

            return View();
        }

        // REGISTER PAGE
        public IActionResult Register()
        {
            return View();
        }
    }
}