using Microsoft.AspNetCore.Mvc;
using UniManage.Models;
using System.Linq;

namespace UniManage.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessageController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? userId =
                HttpContext.Session.GetInt32("UserID");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var users = _context.Users
                .Where(u => u.UserID != userId)
                .ToList();

            return View(users);
        }
    }
}