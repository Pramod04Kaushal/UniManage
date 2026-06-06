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

        public IActionResult Messages(int? groupId)
        {
            int? userId =
                HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var student = _context.Students
                .FirstOrDefault(s => s.UserID == userId);

            if (student == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var groups =
            (
                from gm in _context.GroupMembers
                join mg in _context.MessageGroups
                    on gm.GroupID equals mg.GroupID

                where gm.UserID == userId

                select new LecturerMessageGroupViewModel
                {
                    GroupID = mg.GroupID,
                    GroupName = mg.GroupName,
                    BatchName = "",
                    CourseName = "",
                    StudentCount = 0
                }
            ).ToList();

            ViewBag.SelectedGroupId = groupId;

            if (groupId != null)
            {
                var selectedGroup = _context.MessageGroups
                    .FirstOrDefault(g => g.GroupID == groupId);

                ViewBag.GroupName = selectedGroup?.GroupName;

                ViewBag.Messages = _context.GroupMessages
                    .Where(m => m.GroupID == groupId)
                    .OrderBy(m => m.SentAt)
                    .ToList();
            }

            return View("StudentMessages", groups);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(
    int groupId,
    string? messageText,
    IFormFile? file)
        {
            int? userId =
                HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string? filePath = null;
            string? fileName = null;

            if (file != null && file.Length > 0)
            {
                string uploadFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/chatfiles");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string savedFileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(file.FileName);

                string fullPath =
                    Path.Combine(uploadFolder, savedFileName);

                using (var stream =
                       new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                filePath = "/uploads/chatfiles/" + savedFileName;
                fileName = file.FileName;
            }

            if (!string.IsNullOrWhiteSpace(messageText) || file != null)
            {
                GroupMessage message = new GroupMessage
                {
                    GroupID = groupId,
                    SenderUserID = userId.Value,
                    MessageText = messageText,
                    FilePath = filePath,
                    FileName = fileName,
                    SentAt = DateTime.Now
                };

                _context.GroupMessages.Add(message);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Messages",
                new { groupId });
        }

    }
}