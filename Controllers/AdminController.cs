using Microsoft.AspNetCore.Mvc;
using UniManage.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;

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
        // GET: /Admin/UserDetails/5
        public IActionResult UserDetails(int id)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.UserID == id);

            if (user == null)
            {
                return NotFound();
            }

            // STUDENT

            if (user.Role == "Student")
            {
                var student = _context.Students
                    .FirstOrDefault(s => s.UserID == user.UserID);

                ViewBag.Student = student;

                return View("ViewStudent", user);
            }

            // LECTURER

            if (user.Role == "Lecturer")
            {
                var lecturer = _context.Lecturers
                    .FirstOrDefault(l => l.UserID == user.UserID);

                ViewBag.Lecturer = lecturer;

                return View("ViewLecturer", user);
            }

            // ADMIN

            if (user.Role == "Admin")
            {
                return View("ViewAdmin", user);
            }

            return RedirectToAction("Users");
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

            ViewBag.Batches =
                _context.Batches.ToList();

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

                Address = model.Address,

                DateOfBirth = model.DateOfBirth,

                Gender = model.Gender,

                Status = model.Status,

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

                Semester = model.Semester,

                Course = model.Course,

                EnrollmentYear = model.EnrollmentYear,

                BatchID = model.BatchID
            };

            _context.Students.Add(student);

            _context.SaveChanges();


            var course = _context.Courses
                .FirstOrDefault(c => c.CourseName == model.Course);

            if (course != null)
            {
                Enrollment enrollment = new Enrollment()
                {
                    StudentID = student.StudentID,
                    CourseID = course.CourseID,
                    EnrollmentDate = DateTime.Now,
                    Status = "Active"
                };

                _context.Enrollments.Add(enrollment);

                _context.SaveChanges();
            }

            // ADD STUDENT TO COURSE BATCH GROUP


            if (course != null)
            {
                var messageGroup = _context.MessageGroups
                    .FirstOrDefault(g =>
                        g.CourseID == course.CourseID &&
                        g.BatchID == model.BatchID);

                if (messageGroup != null)
                {
                    bool alreadyExists = _context.GroupMembers.Any(gm =>
                        gm.GroupID == messageGroup.GroupID &&
                        gm.UserID == user.UserID);

                    if (!alreadyExists)
                    {
                        GroupMember groupMember = new GroupMember()
                        {
                            GroupID = messageGroup.GroupID,
                            UserID = user.UserID
                        };

                        _context.GroupMembers.Add(groupMember);

                        _context.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult EditStudent(int id)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.UserID == id);

            var student = _context.Students
                .FirstOrDefault(s => s.UserID == id);

            if (user == null || student == null)
            {
                return NotFound();
            }

            ViewBag.Departments =
                _context.Departments.ToList();

            AddStudentViewModel model =
                new AddStudentViewModel()
                {
                    UserID = user.UserID,

                    FullName = user.FullName,

                    Email = user.Email,

                    Phone = user.Phone,

                    Address = user.Address,

                    Department = user.Department,

                    DateOfBirth = user.DateOfBirth,

                    Gender = user.Gender,

                    Status = user.Status,

                    ProfileImage = user.ProfileImage,

                    Semester = student.Semester,

                    Course = student.Course,

                    EnrollmentYear =
                        student.EnrollmentYear
                };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditStudent(AddStudentViewModel model)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.UserID == model.UserID);

            var student = _context.Students
                .FirstOrDefault(s => s.UserID == model.UserID);

            if (user == null || student == null)
            {
                return NotFound();
            }

            // UPDATE USER

            user.FullName = model.FullName;

            user.Email = model.Email;

            user.Phone = model.Phone;

            user.Address = model.Address;

            user.Department = model.Department;

            user.DateOfBirth = model.DateOfBirth;

            user.Gender = model.Gender;

            user.Status = model.Status;

            // UPDATE STUDENT

            student.Semester = model.Semester;

            student.Course = model.Course;

            student.EnrollmentYear =
                model.EnrollmentYear;

            if (model.ProfileImageFile != null)
            {
                string fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(
                        model.ProfileImageFile.FileName);

                string folderPath =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images");

                string filePath =
                    Path.Combine(folderPath, fileName);

                using (var stream =
                    new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImageFile.CopyTo(stream);
                }

                user.ProfileImage = fileName;
            }

            _context.SaveChanges();

            return RedirectToAction("Users");
        }

        public IActionResult EditLecturer(int id)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.UserID == id);

            if (user == null)
            {
                return NotFound();
            }

            var lecturer = _context.Lecturers
                .FirstOrDefault(x => x.UserID == id);

            var model = new AddLecturerViewModel()
            {
                UserID = user.UserID,

                FullName = user.FullName,

                Email = user.Email,

                Phone = user.Phone,

                Address = user.Address,

                Department = user.Department,

                DateOfBirth = user.DateOfBirth,

                Gender = user.Gender,

                Status = user.Status,

                Specialization = lecturer?.Specialization,

                Qualification = lecturer?.Qualification,

                ExperienceYears = lecturer?.ExperienceYears,

                ProfileImage = user.ProfileImage
            };

            ViewBag.Departments =
                _context.Departments.ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult EditLecturer(AddLecturerViewModel model)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.UserID == model.UserID);

            if (user == null)
            {
                return NotFound();
            }

            var lecturer = _context.Lecturers
                .FirstOrDefault(x => x.UserID == model.UserID);

            // UPDATE USER

            user.FullName = model.FullName;

            user.Email = model.Email;

            user.Phone = model.Phone;

            user.Address = model.Address;

            user.Department = model.Department;

            user.DateOfBirth = model.DateOfBirth;

            user.Gender = model.Gender;

            user.Status = model.Status;

            // PROFILE IMAGE

            if (model.ProfileImageFile != null)
            {
                string fileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(model.ProfileImageFile.FileName);

                string folder =
                    Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/images");

                string filePath =
                    Path.Combine(folder, fileName);

                using (var stream =
                    new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImageFile.CopyTo(stream);
                }

                user.ProfileImage = fileName;
            }

            // UPDATE LECTURER

            if (lecturer != null)
            {
                lecturer.Specialization = model.Specialization;

                lecturer.Qualification = model.Qualification;

                lecturer.ExperienceYears = model.ExperienceYears;
            }

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

                Address = model.Address,

                DateOfBirth = model.DateOfBirth,

                Gender = model.Gender,

                Status = model.Status,

                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);

            _context.SaveChanges();

            // ADD LECTURER TO ALL GROUPS OF HIS DEPARTMENT

            var groups = _context.MessageGroups
                .Where(g =>
                    _context.Courses.Any(c =>
                        c.CourseID == g.CourseID &&
                        c.Department == model.Department))
                .ToList();

            foreach (var group in groups)
            {
                bool alreadyExists = _context.GroupMembers.Any(gm =>
                    gm.GroupID == group.GroupID &&
                    gm.UserID == user.UserID);

                if (!alreadyExists)
                {
                    _context.GroupMembers.Add(
                        new GroupMember
                        {
                            GroupID = group.GroupID,
                            UserID = user.UserID
                        });
                }
            }

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

                Department = model.Department,

                Address = model.Address,

                DateOfBirth = model.DateOfBirth,

                Gender = model.Gender,

                Status = model.Status,

                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);

            _context.SaveChanges();

            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult EditAdmin(int id)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.UserID == id);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Departments =
                _context.Departments.ToList();

            var model = new AddAdminViewModel()
            {
                UserID = user.UserID,

                FullName = user.FullName,

                Email = user.Email,

                Phone = user.Phone,

                Address = user.Address,

                Department = user.Department,

                DateOfBirth = user.DateOfBirth,

                Gender = user.Gender,

                Status = user.Status,

                ProfileImage = user.ProfileImage
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditAdmin(AddAdminViewModel model)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.UserID == model.UserID);

            if (user == null)
            {
                return NotFound();
            }

            // UPDATE USER

            user.FullName = model.FullName;

            user.Email = model.Email;

            user.Phone = model.Phone;

            user.Address = model.Address;

            user.Department = model.Department;

            user.DateOfBirth = model.DateOfBirth;

            user.Gender = model.Gender;

            user.Status = model.Status;

            // PROFILE IMAGE

            if (model.ProfileImageFile != null)
            {
                string fileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(
                        model.ProfileImageFile.FileName);

                string folder =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images");

                string filePath =
                    Path.Combine(folder, fileName);

                using (var stream =
                    new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImageFile.CopyTo(stream);
                }

                user.ProfileImage = fileName;
            }

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
                                 u.FullName,
                                 l.Department
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

            // GET COURSES USING THIS MODULE

            var courseIds = _context.CourseModules
                .Where(cm => cm.ModuleID == module.ModuleID)
                .Select(cm => cm.CourseID)
                .ToList();

            // GET LECTURER USER ID

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.LecturerID == model.LecturerID);

            if (lecturer != null)
            {
                // GET ALL GROUPS OF THOSE COURSES

                var groups = _context.MessageGroups
                    .Where(g => courseIds.Contains(g.CourseID))
                    .ToList();

                foreach (var group in groups)
                {
                    bool exists = _context.GroupMembers.Any(gm =>
                        gm.GroupID == group.GroupID &&
                        gm.UserID == lecturer.UserID);

                    if (!exists)
                    {
                        _context.GroupMembers.Add(
                            new GroupMember
                            {
                                GroupID = group.GroupID,
                                UserID = lecturer.UserID
                            });
                    }
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Modules");
        }



        // Helper to repopulate ViewBag used both for GET and when returning the view after validation errors
        private void PopulateModuleForm()
        {
            var lecturers = (from l in _context.Lecturers
                             join u in _context.Users
                             on l.UserID equals u.UserID
                             select new
                             {
                                 l.LecturerID,
                                 u.FullName,
                                 l.Department
                             }).ToList();

            ViewBag.Lecturers = lecturers;

            int count = _context.Modules.Count() + 1;
            ViewBag.ModuleCode = "MOD" + count.ToString("D3");
        }

        [HttpGet]
        public IActionResult AddCourse()
        {
            // LOAD MODULES
            ViewBag.Modules = _context.Modules.ToList();

            // LOAD DEPARTMENTS
            ViewBag.Departments = _context.Departments.ToList();



            // AUTO COURSE CODE
            int count = _context.Courses.Count() + 1;

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

            Batch batch = new Batch()
            {
                BatchName = course.CourseName + " Batch",
                CourseID = course.CourseID,
                StartYear = DateTime.Now.Year,
                Status = "Active"
            };

            _context.Batches.Add(batch);
            _context.SaveChanges();

            MessageGroup group = new MessageGroup()
            {
                BatchID = batch.BatchID,
                CourseID = course.CourseID,
                GroupName = batch.BatchName
            };

            _context.MessageGroups.Add(group);
            _context.SaveChanges();

            // ADD MODULE LECTURERS TO COURSE GROUPS

            var lecturerIds = _context.Modules
                .Where(m => model.SelectedModules.Contains(m.ModuleID))
                .Select(m => m.LecturerID)
                .Distinct()
                .ToList();

            var lecturerUserIds = _context.Lecturers
                .Where(l => lecturerIds.Contains(l.LecturerID))
                .Select(l => l.UserID)
                .ToList();

            var groups = _context.MessageGroups
                .Where(g => g.CourseID == course.CourseID)
                .ToList();

            foreach (var userId in lecturerUserIds)
            {
                foreach (var messageGroup in groups)
                {
                    bool exists = _context.GroupMembers.Any(gm =>
                        gm.GroupID == messageGroup.GroupID &&
                        gm.UserID == userId);

                    if (!exists)
                    {
                        _context.GroupMembers.Add(new GroupMember
                        {
                            GroupID = messageGroup.GroupID,
                            UserID = userId
                        });
                    }
                }
            }

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

        [HttpGet]
        public JsonResult GetBatchesByCourse(string courseName)
        {
            var course = _context.Courses
                .FirstOrDefault(c => c.CourseName == courseName);

            if (course == null)
            {
                return Json(new List<object>());
            }

            var batches = _context.Batches
                .Where(b => b.CourseID == course.CourseID)
                .Select(b => new
                {
                    b.BatchID,
                    b.BatchName
                })
                .ToList();

            return Json(batches);
        }


        public IActionResult DisableUser(int id)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.UserID == id);

            if (user == null)
            {
                return NotFound();
            }

            user.Status =
    user.Status == "Inactive"
    ? "Active"
    : "Inactive";

            _context.SaveChanges();

            return RedirectToAction("Users");
        }

        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.UserID == id);

            if (user == null)
            {
                return NotFound();
            }

            // PREVENT SELF DELETE

            if (user.Email == User.Identity.Name)
            {
                return RedirectToAction("Users");
            }

            var student = _context.Students
                .FirstOrDefault(s => s.UserID == id);

            if (student != null)
            {
                _context.Students.Remove(student);
            }

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.UserID == id);

            if (lecturer != null)
            {
                _context.Lecturers.Remove(lecturer);
            }

            _context.Users.Remove(user);

            _context.SaveChanges();

            return RedirectToAction("Users");
        }

        public IActionResult ViewCourse(int id)
        {
            var course = _context.Courses
                .FirstOrDefault(c => c.CourseID == id);

            if (course == null)
                return NotFound();

            ViewBag.CourseModules =
                (from cm in _context.CourseModules
                 join m in _context.Modules
                 on cm.ModuleID equals m.ModuleID
                 where cm.CourseID == id
                 select new
                 {
                     m.ModuleName,
                     m.ModuleCode,
                     cm.Semester,
                     m.Credits
                 }).ToList();



            return View("ViewCourse", course);
        }

        public IActionResult DisableCourse(int id)
        {
            var course = _context.Courses
                .FirstOrDefault(c => c.CourseID == id);

            if (course == null)
            {
                return NotFound();
            }

            course.Status =
                course.Status == "Inactive"
                ? "Active"
                : "Inactive";

            _context.SaveChanges();

            return RedirectToAction("Courses");
        }

        public IActionResult DeleteCourse(int id)
        {
            var course = _context.Courses
                .FirstOrDefault(c => c.CourseID == id);

            if (course == null)
            {
                return NotFound();
            }

            var courseModules = _context.CourseModules
                .Where(cm => cm.CourseID == id)
                .ToList();

            _context.CourseModules.RemoveRange(courseModules);

            _context.Courses.Remove(course);

            _context.SaveChanges();

            return RedirectToAction("Courses");
        }

        [HttpGet]
        public IActionResult EditCourse(int id)
        {
            var course = _context.Courses
                .FirstOrDefault(c => c.CourseID == id);

            if (course == null)
                return NotFound();

            ViewBag.Departments =
                _context.Departments.ToList();

            ViewBag.Modules =
                _context.Modules.ToList();


            ViewBag.CourseModules =
                (from cm in _context.CourseModules
                 join m in _context.Modules
                 on cm.ModuleID equals m.ModuleID
                 where cm.CourseID == id
                 select m).ToList();

            return View(course);
        }

        [HttpPost]
        public IActionResult EditCourse(
    Course model,
    List<int> SelectedModules)
        {
            var course = _context.Courses
                .FirstOrDefault(c => c.CourseID == model.CourseID);

            if (course == null)
                return NotFound();

            course.CourseName = model.CourseName;
            course.Department = model.Department;
            course.Semesters = model.Semesters;
            course.Duration = model.Duration;
            course.CourseFee = model.CourseFee;
            course.QualificationType = model.QualificationType;
            course.Intake = model.Intake;
            course.Description = model.Description;
            course.Status = model.Status;

            // REMOVE OLD MODULES

            var oldModules =
                _context.CourseModules
                .Where(cm => cm.CourseID == course.CourseID)
                .ToList();

            _context.CourseModules.RemoveRange(oldModules);

            // ADD NEW MODULES

            if (SelectedModules != null)
            {
                foreach (var moduleId in SelectedModules)
                {
                    _context.CourseModules.Add(
                        new CourseModule
                        {
                            CourseID = course.CourseID,
                            ModuleID = moduleId,
                            Semester = 1
                        });
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Courses");
        }

        public IActionResult ViewModule(int id)
        {
            var module = _context.Modules
                .FirstOrDefault(m => m.ModuleID == id);

            if (module == null)
                return NotFound();

            return View("ViewModule", module);
        }

        [HttpGet]
        public IActionResult EditModule(int id)
        {
            var module = _context.Modules
                .FirstOrDefault(m => m.ModuleID == id);

            if (module == null)
                return NotFound();

            ViewBag.Departments =
                _context.Departments.ToList();

            ViewBag.Lecturers =
                (from l in _context.Lecturers
                 join u in _context.Users
                 on l.UserID equals u.UserID
                 select new
                 {
                     l.LecturerID,
                     u.FullName,
                     l.Department
                 }).ToList();

            return View(module);
        }

        public IActionResult DisableModule(int id)
        {
            var module = _context.Modules
                .FirstOrDefault(m => m.ModuleID == id);

            if (module == null)
                return NotFound();

            module.Status =
                module.Status == "Inactive"
                ? "Active"
                : "Inactive";

            _context.SaveChanges();

            return RedirectToAction("Modules");
        }

        public IActionResult DeleteModule(int id)
        {
            var module = _context.Modules
                .FirstOrDefault(m => m.ModuleID == id);

            if (module == null)
                return NotFound();

            _context.Modules.Remove(module);

            _context.SaveChanges();

            return RedirectToAction("Modules");
        }

        [HttpPost]
        public IActionResult EditModule(Module model)
        {
            var module = _context.Modules
                .FirstOrDefault(m => m.ModuleID == model.ModuleID);

            if (module == null)
                return NotFound();

            module.ModuleName = model.ModuleName;
            module.Department = model.Department;
            module.Credits = model.Credits;
            module.Description = model.Description;
            module.LecturerID = model.LecturerID;
            module.Status = model.Status;

            _context.SaveChanges();

            return RedirectToAction("Modules");
        }


    }
}
