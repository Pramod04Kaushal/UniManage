using Microsoft.AspNetCore.Mvc;

namespace UniManage.Controllers
{
    public class AccountController : Controller
    {
        // LOGIN PAGE
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN PROCESS
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // TEMP LOGIN

            if (email == "admin@gmail.com"
                && password == "123")
            {
                return RedirectToAction("Dashboard", "Admin");
            }

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