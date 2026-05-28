using Microsoft.AspNetCore.Mvc;
using UniManage.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace UniManage.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            ViewBag.TotalStudents =
                _context.Students.Count();

            ViewBag.TotalLecturers =
                _context.Lecturers.Count();

            ViewBag.TotalCourses =
                _context.Courses.Count();

            ViewBag.TotalEnrollments =
                _context.Enrollments.Count();

            return View();
        }

        public IActionResult Users(string roleFilter)
        {
            var users = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(roleFilter)
                && roleFilter != "ALL")
            {
                users = users.Where(u => u.Role == roleFilter);
            }

            return View(users.ToList());
        }

        public IActionResult Courses(string departmentFilter = "ALL")
        {
            // LOAD DEPARTMENTS

            ViewBag.Departments =
                _context.Departments.ToList();

            // LOAD COURSES

            var courses =
                _context.Courses.AsQueryable();

            // FILTER BY DEPARTMENT

            if (departmentFilter != "ALL")
            {
                courses = courses
                    .Where(c => c.Department == departmentFilter);
            }

            return View(courses.ToList());
        }

        public IActionResult Modules(string departmentFilter = "ALL")
        {
            // LOAD DEPARTMENTS

            ViewBag.Departments =
                _context.Departments.ToList();

            // LOAD MODULES

            var modules =
                _context.Modules.AsQueryable();

            // FILTER BY DEPARTMENT

            if (departmentFilter != "ALL")
            {
                modules = modules
                    .Where(m => m.Department == departmentFilter);
            }

            return View(modules.ToList());
        }

        public IActionResult Enrollments()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddStudent()
        {
            ViewBag.Departments =
                _context.Departments.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult AddStudent(AddStudentViewModel model)
        {
            // STUDENT COUNT

            int count = _context.Students.Count() + 1;

            // AUTO REG NUMBER

            string regNum = "STU" + count.ToString("D3");

            // AUTO USERNAME

            string username =
                model.FullName.Replace(" ", "").ToLower()
                + count;

            // AUTO PASSWORD

            string password = "UNI@" + count;

            // CREATE USER

            User user = new User()
            {
                FullName = model.FullName,

                Email = model.Email,

                Username = username,

                PasswordHash = password,

                Phone = model.Phone,

                Role = "Student",

                Department = model.Department,

                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);

            _context.SaveChanges();

            // CREATE STUDENT

            Student student = new Student()
            {
                UserID = user.UserID,

                RegNum = regNum,

                Department = model.Department,

                Semester = model.Semester
            };

            _context.Students.Add(student);

            _context.SaveChanges();

            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult AddLecturer()
        {
            ViewBag.Departments =
                _context.Departments.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult AddLecturer(AddLecturerViewModel model)
        {
            int count = _context.Lecturers.Count() + 1;

            string username =
                model.FullName.Replace(" ", "").ToLower()
                + count;

            string password = "LEC@" + count;

            User user = new User()
            {
                FullName = model.FullName,

                Email = model.Email,

                Username = username,

                PasswordHash = password,

                Phone = model.Phone,

                Role = "Lecturer",

                Department = model.Department,

                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);

            _context.SaveChanges();

            Lecturer lecturer = new Lecturer()
            {
                UserID = user.UserID,

                Department = model.Department,

                Specialization = model.Specialization
            };

            _context.Lecturers.Add(lecturer);

            _context.SaveChanges();

            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult AddAdmin()
        {
            ViewBag.Departments =
                _context.Departments.ToList();

            return View();
        }
        [HttpPost]
        public IActionResult AddAdmin(AddAdminViewModel model)
        {
            int count = _context.Users.Count() + 1;

            string username =
                model.FullName.Replace(" ", "").ToLower()
                + count;

            string password = "ADMIN@" + count;

            User user = new User()
            {
                FullName = model.FullName,

                Email = model.Email,

                Username = username,

                PasswordHash = password,

                Phone = model.Phone,

                Role = "Admin",

                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);

            _context.SaveChanges();

            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult AddModule()
        {

            // LOAD LECTURERS

            var lecturers = (from l in _context.Lecturers
                             join u in _context.Users
                             on l.UserID equals u.UserID
                             select new
                             {
                                 l.LecturerID,
                                 u.FullName
                             }).ToList();

            ViewBag.Lecturers = lecturers;

            // LOAD DEPARTMENTS

            ViewBag.Departments =
                _context.Departments.ToList();

            // AUTO MODULE CODE

            int count =
                _context.Modules.Count() + 1;

            string moduleCode =
                "MOD" + count.ToString("D3");

            ViewBag.ModuleCode =
                moduleCode;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddModule(AddModuleViewModel model)
        {
            Module module = new Module()
            {
                ModuleName = model.ModuleName ?? string.Empty,

                ModuleCode = model.ModuleCode ?? string.Empty,

                Department = model.Department ?? string.Empty,

                Credits = model.Credits,

                PdfPath = model.PdfFile?.FileName ?? "",

                PresentationPath = model.PresentationFile?.FileName ?? "",

                VideoPath = model.VideoFile?.FileName ?? "",

                Description = model.Description ?? string.Empty,

                LecturerID = model.LecturerID,

                Status = model.Status ?? "Inactive",

                CreatedAt = DateTime.Now
            };

            _context.Modules.Add(module);

            _context.SaveChanges();

            return RedirectToAction("Modules");
        }



        // Helper to repopulate ViewBag used both for GET and when returning the view after validation errors
        private void PopulateModuleForm()
        {
            var lecturers = (from l in _context.Lecturers
                             join u in _context.Users on l.UserID equals u.UserID
                             select new
                             {
                                 l.LecturerID,
                                 u.FullName
                             }).ToList();

            ViewBag.Lecturers = lecturers;

            int count = _context.Modules.Count() + 1;
            ViewBag.ModuleCode = "MOD" + count.ToString("D3");
        }

        [HttpGet]
        public IActionResult AddCourse()
        {
            // LOAD MODULES

            ViewBag.Modules =
                _context.Modules.ToList();

            // LOAD DEPARTMENTS

            ViewBag.Departments =
                _context.Departments.ToList();

            // AUTO COURSE CODE

            int count =
                _context.Courses.Count() + 1;

            ViewBag.CourseCode =
                "CRS" + count.ToString("D3");

            return View();
        }

        [HttpPost]
        public IActionResult AddCourse(AddCourseViewModel model)
        {
            int count = _context.Courses.Count() + 1;

            string courseCode =
                "CRS" + count.ToString("D3");

            Course course = new Course()
            {
                CourseCode = courseCode,

                CourseName = model.CourseName,

                Department = model.Department,

                Semesters = model.Semesters,

                Duration = model.Duration,

                CourseFee = model.CourseFee,

                QualificationType = model.QualificationType,

                Intake = string.Join(",", model.Intake),

                Description = model.Description,

                Status = "Active",

                CreatedAt = DateTime.Now
            };

            _context.Courses.Add(course);

            _context.SaveChanges();

            // SAVE MODULES

            if (model.SelectedModules != null)
            {
                foreach (var moduleId in model.SelectedModules)
                {
                    CourseModule courseModule =
                        new CourseModule()
                        {
                            CourseID = course.CourseID,

                            ModuleID = moduleId,

                            Semester = 1
                        };

                    _context.CourseModules.Add(courseModule);
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Courses");
        }

        [HttpGet]
        public JsonResult GetCoursesByDepartment(string department)
        {
            var courses = _context.Courses
                .Where(c => c.Department == department)
                .Select(c => new
                {
                    c.CourseID,
                    c.CourseName
                })
                .ToList();

            return new JsonResult(courses);
        }
    }



}