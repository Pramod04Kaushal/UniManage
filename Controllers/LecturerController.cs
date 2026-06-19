using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;
using UniManage.Models;

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
            LoadUnreadMessageCount();
            LoadNavbarData();

            // GET LOGGED-IN USER ID

            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // GET LECTURER

            var lecturer = (from l in _context.Lecturers
                            join u in _context.Users
                            on l.UserID equals u.UserID
                            where l.UserID == lecturerUserId.Value
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

            int totalAssignments = _context.Assignments
                .Count(a => a.LecturerID == lecturer.LecturerID);

            int pendingReviews = _context.AssignmentSubmissions
                .Count(s => s.Status == "Submitted");

            ViewBag.TotalModules = totalModules;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalAssignments = totalAssignments;
            ViewBag.PendingReviews = pendingReviews;
            // LOAD MODULES

            var modules = _context.Modules
                .Where(m => m.LecturerID == lecturer.LecturerID)
                .ToList();

            return View(modules);
        }

        public IActionResult MyModules()
        {
            LoadUnreadMessageCount();
            LoadNavbarData();

            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.UserID == lecturerUserId);

            if (lecturer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var modules = _context.Modules
                .Where(m => m.LecturerID == lecturer.LecturerID)
                .ToList();

            return View(modules);
        }

        public IActionResult ModuleDetails(int id)
        {
            LoadUnreadMessageCount();

            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var module = _context.Modules
                .FirstOrDefault(m => m.ModuleID == id);

            if (module == null)
            {
                return NotFound();
            }

            ViewBag.Materials = _context.Materials
                .Where(m => m.ModuleID == id)
                .OrderByDescending(m => m.UploadedAt)
                .ToList();

            return View(module);
        }

        [HttpPost]
        public async Task<IActionResult> UploadMaterial(AddMaterialViewModel model)
        {
            if (model == null)
            {
                return Content("ERROR: Model is NULL");
            }

            if (model.File == null)
            {
                return Content("ERROR: No file was uploaded");
            }

            if (!ModelState.IsValid)
            {
                return Content("ERROR: ModelState is Invalid");
            }

            string uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/uploads/materials");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = Guid.NewGuid().ToString() +
                              Path.GetExtension(model.File.FileName);

            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            Material material = new Material
            {
                ModuleID = model.ModuleID,
                Title = model.Title,
                MaterialType = model.MaterialType,
                FilePath = "/uploads/materials/" + fileName,
                UploadedAt = DateTime.Now
            };

            _context.Materials.Add(material);

            await _context.SaveChangesAsync();

            return RedirectToAction("ModuleDetails",
                new { id = model.ModuleID });
        }

        [HttpGet]
        public IActionResult CreateAssignment()
        {
            LoadUnreadMessageCount();

            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.UserID == lecturerUserId);

            if (lecturer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var lecturerCourses =
            (
                from cm in _context.CourseModules
                join m in _context.Modules
                    on cm.ModuleID equals m.ModuleID
                join c in _context.Courses
                    on cm.CourseID equals c.CourseID
                where m.LecturerID == lecturer.LecturerID
                select c
            )
            .Distinct()
            .ToList();

            ViewBag.Courses = lecturerCourses;

            ViewBag.Modules = _context.Modules
                .Where(m => m.LecturerID == lecturer.LecturerID)
                .ToList();

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateAssignment(
    AddAssignmentViewModel model)
        {
            string filePath = "";

            if (model.AttachmentFile != null)
            {
                string uploadFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/assignments");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(model.AttachmentFile.FileName);

                string fullPath =
                    Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.AttachmentFile.CopyToAsync(stream);
                }

                filePath = "/uploads/assignments/" + fileName;
            }

            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.UserID == lecturerUserId);

            Assignment assignment = new Assignment()
            {
                CourseID = model.CourseID,
                ModuleID = model.ModuleID,
                BatchID = model.BatchID,
                Title = model.Title,
                Description = model.Description,
                Deadline = model.Deadline,
                LecturerID = lecturer?.LecturerID,
                AttachmentPath = filePath,
                CreatedAt = DateTime.Now
            };

            _context.Assignments.Add(assignment);

            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }

        public IActionResult Submissions(int id)
        {
            var submissions =
            (
            from s in _context.AssignmentSubmissions

            join st in _context.Students
                on s.StudentID equals st.StudentID

            join u in _context.Users
                on st.UserID equals u.UserID

            where s.AssignmentID == id

            select new SubmissionViewModel
            {
                SubmissionID = s.SubmissionID,
                StudentName = u.FullName,
                RegNum = st.RegNum,
                SubmissionDate = s.SubmissionDate,
                Status = s.Status,
                FilePath = s.FilePath,
                Grade = s.Grade
            }
            ).ToList();


            ViewBag.TotalSubmissions = submissions.Count;

            ViewBag.GradedCount =
                submissions.Count(s => s.Status == "Graded");

            ViewBag.PendingCount =
                submissions.Count(s => s.Status != "Graded");

            var assignment =
                _context.Assignments
                .FirstOrDefault(a => a.AssignmentID == id);

            ViewBag.AssignmentTitle = assignment?.Title;

            ViewBag.Deadline = assignment?.Deadline;

            ViewBag.ModuleName =
            (
                from a in _context.Assignments
                join m in _context.Modules
                    on a.ModuleID equals m.ModuleID
                where a.AssignmentID == id
                select m.ModuleName
            ).FirstOrDefault();

            return View(submissions);


}


        public IActionResult DownloadSubmission(int id)
        {
            var submission = _context.AssignmentSubmissions
                .FirstOrDefault(s => s.SubmissionID == id);

            if (submission == null)
            {
                return NotFound();
            }

            string fullPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                submission.FilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);

            string fileName = Path.GetFileName(fullPath);

            return File(
                fileBytes,
                "application/octet-stream",
                fileName
            );
        }

        public IActionResult GradeSubmission(int id)
        {



            var submission =
                _context.AssignmentSubmissions
                .FirstOrDefault(s => s.SubmissionID == id);

            if (submission == null)
            {
                return NotFound();
            }

            var student =
                (from s in _context.Students
                 join u in _context.Users
                 on s.UserID equals u.UserID
                 where s.StudentID == submission.StudentID
                 select new
                 {
                     StudentName = u.FullName,
                     s.RegNum
                 }).FirstOrDefault();

            ViewBag.StudentName = student?.StudentName;
            ViewBag.RegNum = student?.RegNum;

            return View(submission);
        }

        [HttpPost]
        public IActionResult GradeSubmission(
    int submissionId,
    decimal grade,
    string? feedback)
        {
            var submission =
                _context.AssignmentSubmissions
                .FirstOrDefault(s => s.SubmissionID == submissionId);

            if (submission == null)
            {
                return NotFound();
            }

            submission.Grade = grade;
            submission.Feedback = feedback;
            submission.Status = "Graded";

            _context.SaveChanges();

            return RedirectToAction(
                "Submissions",
                new { id = submission.AssignmentID });
        }

        [HttpGet]
        public JsonResult GetModulesByCourse(int courseId)
        {
            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return Json(new List<object>());
            }

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.UserID == lecturerUserId);

            if (lecturer == null)
            {
                return Json(new List<object>());
            }

            var modules =
            (
                from cm in _context.CourseModules
                join m in _context.Modules
                    on cm.ModuleID equals m.ModuleID
                where cm.CourseID == courseId
                   && m.LecturerID == lecturer.LecturerID
                select new
                {
                    m.ModuleID,
                    m.ModuleName,
                    m.ModuleCode
                }
            ).ToList();

            return Json(modules);
        }

        [HttpGet]
        public JsonResult GetBatchesByCourse(int courseId)
        {
            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return Json(new List<object>());
            }

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.UserID == lecturerUserId);

            if (lecturer == null)
            {
                return Json(new List<object>());
            }

            bool hasAccess =
            (
                from cm in _context.CourseModules
                join m in _context.Modules
                    on cm.ModuleID equals m.ModuleID
                where cm.CourseID == courseId
                   && m.LecturerID == lecturer.LecturerID
                select cm
            ).Any();

            if (!hasAccess)
            {
                return Json(new List<object>());
            }

            var batches = _context.Batches
                .Where(b => b.CourseID == courseId
                         && b.Status == "Active")
                .Select(b => new
                {
                    b.BatchID,
                    b.BatchName
                })
                .ToList();

            return Json(batches);
        }



        public IActionResult Assignments()
        {
            LoadUnreadMessageCount();
            LoadNavbarData();

            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.UserID == lecturerUserId);

            if (lecturer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var assignments = _context.Assignments
                .Where(a => a.LecturerID == lecturer.LecturerID)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();

            ViewBag.Modules = _context.Modules
                .Where(m => m.LecturerID == lecturer.LecturerID)
                .OrderBy(m => m.ModuleName)
                .ToList();

            return View(assignments);
        }

        [HttpGet]
        public IActionResult EditAssignment(int id)
        {
            var assignment = _context.Assignments
                .FirstOrDefault(a => a.AssignmentID == id);

            if (assignment == null)
            {
                return NotFound();
            }

            ViewBag.Courses = _context.Courses.ToList();

            ViewBag.Modules = (from cm in _context.CourseModules
                               join m in _context.Modules
                               on cm.ModuleID equals m.ModuleID
                               where cm.CourseID == assignment.CourseID
                               select m).ToList();

            ViewBag.Batches = _context.Batches
                .Where(b => b.CourseID == assignment.CourseID)
                .ToList();

            return View(assignment);
        }

        [HttpPost]
        public IActionResult EditAssignment(Assignment model)
        {
            var assignment = _context.Assignments
                .FirstOrDefault(a => a.AssignmentID == model.AssignmentID);

            if (assignment == null)
            {
                return NotFound();
            }

            assignment.Title = model.Title;
            assignment.Description = model.Description;
            assignment.CourseID = model.CourseID;
            assignment.ModuleID = model.ModuleID;
            assignment.BatchID = model.BatchID;
            assignment.Deadline = model.Deadline;

            _context.SaveChanges();

            return RedirectToAction("Assignments");
        }

        public IActionResult Messages(int? groupId)
        {
            LoadUnreadMessageCount();
            LoadNavbarData();

            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.UserID == lecturerUserId);

            if (lecturer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var privateChats =
            (
                from c in _context.PrivateChats

                join u in _context.Users
                    on c.StudentUserID equals u.UserID

                where c.LecturerUserID == lecturerUserId.Value

                select new
                {
                    c.ChatID,
                    StudentName = u.FullName,
                    StudentUserID = u.UserID,
                    ProfileImage = u.ProfileImage,

                    UnreadCount =
                        _context.PrivateMessages.Count(m =>
                            m.ChatID == c.ChatID
                            && m.SenderUserID == u.UserID
                            && !m.IsRead)
                }
            ).ToList();

            ViewBag.PrivateChats = privateChats;

            var groups =
            (
                from gm in _context.GroupMembers

                join mg in _context.MessageGroups
                    on gm.GroupID equals mg.GroupID

                join b in _context.Batches
                    on mg.BatchID equals b.BatchID

                join c in _context.Courses
                    on mg.CourseID equals c.CourseID

                where gm.UserID == lecturerUserId.Value

                select new LecturerMessageGroupViewModel
                {
                    GroupID = mg.GroupID,

                    GroupName = mg.GroupName,

                    BatchID = b.BatchID,

                    BatchName = b.BatchName,

                    CourseName = c.CourseName,

                    StudentCount =
                        _context.Students.Count(s =>
                            s.BatchID == b.BatchID)
                }
            ).Distinct().ToList();

            ViewBag.SelectedGroupId = groupId;

            if (groupId != null)
            {
                var selectedGroup = _context.MessageGroups
                    .FirstOrDefault(g => g.GroupID == groupId);

                ViewBag.GroupName = selectedGroup?.GroupName;

                ViewBag.CourseName =
(
    from g in _context.MessageGroups
    join c in _context.Courses
        on g.CourseID equals c.CourseID
    where g.GroupID == groupId
    select c.CourseName
).FirstOrDefault();

                ViewBag.BatchName =
                (
                    from g in _context.MessageGroups
                    join b in _context.Batches
                        on g.BatchID equals b.BatchID
                    where g.GroupID == groupId
                    select b.BatchName
                ).FirstOrDefault();

                ViewBag.Messages =
                (
                    from m in _context.GroupMessages

                    join u in _context.Users
                        on m.SenderUserID equals u.UserID

                    where m.GroupID == groupId

                    orderby m.SentAt

                    select new MessageViewModel
                    {
                        MessageID = m.GroupMessageID,

                        SenderUserID = m.SenderUserID,

                        SenderName = u.FullName,

                        SenderRole = u.Role,

                        MessageText = m.MessageText,

                        FilePath = m.FilePath,

                        FileName = m.FileName,

                        SentAt = m.SentAt
                    }
                ).ToList();
            }
            ViewBag.CurrentUserId = lecturerUserId;
            return View(groups);
        }

        public IActionResult PrivateChat(int chatId)
        {
            LoadUnreadMessageCount();
            LoadNavbarData();

            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var chat = _context.PrivateChats
                .FirstOrDefault(c => c.ChatID == chatId);

            var unreadMessages = _context.PrivateMessages
                .Where(m =>
                    m.ChatID == chatId
                    && m.SenderUserID != lecturerUserId.Value
                    && !m.IsRead)
                .ToList();

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
            }

            _context.SaveChanges();

            if (chat == null)
            {
                return NotFound();
            }

            var student = _context.Users
                .FirstOrDefault(u => u.UserID == chat.StudentUserID);

            ViewBag.IsPrivateChat = true;
            ViewBag.ChatID = chatId;
            ViewBag.StudentName = student?.FullName;
            ViewBag.StudentProfileImage = student?.ProfileImage;
            ViewBag.CurrentUserId = lecturerUserId;


            ViewBag.PrivateMessages =
            (
                from m in _context.PrivateMessages

                join u in _context.Users
                    on m.SenderUserID equals u.UserID

                where m.ChatID == chatId

                orderby m.SentAt

                select new MessageViewModel
                {
                    SenderUserID = m.SenderUserID,
                    SenderName = u.FullName,
                    MessageText = m.MessageText,
                    FilePath = m.FilePath,
                    FileName = m.FileName,
                    SentAt = m.SentAt
                }
            ).ToList();

            var privateChats =
            (
                from c in _context.PrivateChats
                join u in _context.Users
                    on c.StudentUserID equals u.UserID
                where c.LecturerUserID == lecturerUserId.Value

                select new
                {
                    c.ChatID,
                    StudentName = u.FullName,
                    StudentUserID = u.UserID,
                    ProfileImage = u.ProfileImage,

                    UnreadCount =
                        _context.PrivateMessages.Count(m =>
                            m.ChatID == c.ChatID
                            && m.SenderUserID == u.UserID
                            && !m.IsRead)
                }
            ).ToList();

            ViewBag.PrivateChats = privateChats;

            var groups =
            (
                from gm in _context.GroupMembers

                join mg in _context.MessageGroups
                    on gm.GroupID equals mg.GroupID

                join b in _context.Batches
                    on mg.BatchID equals b.BatchID

                join c in _context.Courses
                    on mg.CourseID equals c.CourseID

                where gm.UserID == lecturerUserId.Value

                select new LecturerMessageGroupViewModel
                {
                    GroupID = mg.GroupID,
                    GroupName = mg.GroupName,
                    BatchID = b.BatchID,
                    BatchName = b.BatchName,
                    CourseName = c.CourseName,
                    StudentCount =
                        _context.Students.Count(s =>
                            s.BatchID == b.BatchID)
                }
            ).Distinct().ToList();

            return View("Messages", groups);
        }

        [HttpPost]
        public async Task<IActionResult> SendPrivateMessage(
     int chatId,
     string? messageText,
     IFormFile? file)
        {
            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
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
                PrivateMessage message =
                    new PrivateMessage
                    {
                        ChatID = chatId,
                        SenderUserID = lecturerUserId.Value,
                        MessageText = messageText,
                        FilePath = filePath,
                        FileName = fileName,
                        SentAt = DateTime.Now
                    };

                _context.PrivateMessages.Add(message);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(
                "PrivateChat",
                new { chatId });
        }

        [HttpPost]
        public async Task<IActionResult> SendGroupMessage(
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


        private void LoadUnreadMessageCount()
        {
            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
                return;

            ViewBag.TotalUnreadMessages =
            (
                from m in _context.PrivateMessages

                join c in _context.PrivateChats
                    on m.ChatID equals c.ChatID

                where c.LecturerUserID == lecturerUserId.Value
                   && m.SenderUserID != lecturerUserId.Value
                   && !m.IsRead

                select m
            ).Count();
        }

        public IActionResult Students()
        {
            LoadUnreadMessageCount();
            LoadNavbarData();

            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.UserID == lecturerUserId);

            if (lecturer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var students =
            (
                from e in _context.Enrollments

                join s in _context.Students
                    on e.StudentID equals s.StudentID

                join u in _context.Users
                    on s.UserID equals u.UserID

                join cm in _context.CourseModules
                    on e.CourseID equals cm.CourseID

                join m in _context.Modules
                    on cm.ModuleID equals m.ModuleID

                where m.LecturerID == lecturer.LecturerID

                select u
            )
            .Distinct()
            .ToList();

            return View(students);
        }

        public IActionResult ViewStudent(int id)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.UserID == id);

            if (user == null)
            {
                return NotFound();
            }

            var student = _context.Students
                .FirstOrDefault(s => s.UserID == id);

            if (student == null)
            {
                return NotFound();
            }

            ViewBag.Student = student;

            ViewBag.Batch = _context.Batches
                .FirstOrDefault(b => b.BatchID == student.BatchID);

            return View(user);
        }

        public IActionResult Profile()
        {
            LoadUnreadMessageCount();
            LoadNavbarData();

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
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult ChangePassword(
            string currentPassword,
            string newPassword,
            string confirmPassword)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

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

            if (user.PasswordHash != currentPassword)
            {
                TempData["Error"] = "Current password is incorrect.";
                return RedirectToAction("Profile");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Passwords do not match.";
                return RedirectToAction("Profile");
            }

            user.PasswordHash = newPassword;

            _context.SaveChanges();

            TempData["Success"] = "Password updated successfully.";

            return RedirectToAction("Profile");
        }

        public IActionResult EditProfile()
        {
            LoadUnreadMessageCount();
            LoadNavbarData();

            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.UserID == userId);

            if (user == null)
                return RedirectToAction("Profile");

            return View(user);
        }

        [HttpPost]
        public IActionResult EditProfile(User model, IFormFile? profileImage)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserID == model.UserID);

            if (user == null)
                return RedirectToAction("Profile");

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Address = model.Address;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;

            if (profileImage != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(profileImage.FileName);

                string path = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/images",
                    fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    profileImage.CopyTo(stream);
                }

                user.ProfileImage = fileName;
            }

            _context.SaveChanges();

            TempData["Success"] = "Profile updated successfully";

            return RedirectToAction("Profile");
        }

        private void LoadNavbarData()
        {
            int? userId =
                HttpContext.Session.GetInt32("UserID");

            if (userId == null)
                return;

            var user = _context.Users
                .FirstOrDefault(u => u.UserID == userId);

            if (user == null)
                return;

            ViewBag.NavbarLecturerName = user.FullName;
            ViewBag.NavbarProfileImage = user.ProfileImage;
        }

        public IActionResult Reports()
        {
            LoadUnreadMessageCount();
            LoadNavbarData();

            int? lecturerUserId = HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
                return RedirectToAction("Login", "Account");

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.UserID == lecturerUserId);

            if (lecturer == null)
                return RedirectToAction("Login", "Account");

            var modules = _context.Modules
                .Where(m => m.LecturerID == lecturer.LecturerID)
                .ToList();

            var moduleIds = modules
                .Select(m => m.ModuleID)
                .ToList();

            var assignments = _context.Assignments
                .Where(a => a.ModuleID.HasValue &&
                            moduleIds.Contains(a.ModuleID.Value))
                .ToList();

            var assignmentIds = assignments
                .Select(a => a.AssignmentID)
                .ToList();

            var submissions = _context.AssignmentSubmissions
                .Where(s => assignmentIds.Contains(s.AssignmentID))
                .ToList();

            var courseIds = assignments
                .Select(a => a.CourseID)
                .Distinct()
                .ToList();

            var totalStudents = _context.Enrollments
                .Where(e => courseIds.Contains(e.CourseID))
                .Select(e => e.StudentID)
                .Distinct()
                .Count();

            ViewBag.TotalModules = modules.Count;
            ViewBag.TotalAssignments = assignments.Count;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalSubmissions = submissions.Count;

            ViewBag.PendingReviews =
                submissions.Count(s => s.Grade == null);

            ViewBag.AverageGrade =
                submissions.Any(s => s.Grade.HasValue)
                    ? Math.Round(
                        submissions
                        .Where(s => s.Grade.HasValue)
                        .Average(s => (double)s.Grade.Value), 1)
                    : 0;

            var recentSubmissions =
            (
                from s in _context.AssignmentSubmissions

                join st in _context.Students
                    on s.StudentID equals st.StudentID

                join u in _context.Users
                    on st.UserID equals u.UserID

                join a in _context.Assignments
                    on s.AssignmentID equals a.AssignmentID

                where assignmentIds.Contains(s.AssignmentID)

                orderby s.SubmissionDate descending

                select new SubmissionViewModel
                {
                    StudentName = u.FullName,
                    RegNum = st.RegNum,
                    AssignmentTitle = a.Title,
                    SubmissionDate = s.SubmissionDate,
                    Grade = s.Grade,
                    Status = s.Status,
                    FilePath = s.FilePath
                }

            ).Take(5).ToList();

            ViewBag.RecentSubmissions = recentSubmissions;

            ViewBag.GradedCount =
                submissions.Count(s => s.Grade.HasValue);

            var topStudents =
            (
                from s in _context.AssignmentSubmissions

                join st in _context.Students
                    on s.StudentID equals st.StudentID

                join u in _context.Users
                    on st.UserID equals u.UserID

                join a in _context.Assignments
                    on s.AssignmentID equals a.AssignmentID

                where assignmentIds.Contains(s.AssignmentID)
                   && s.Grade != null

                orderby s.Grade descending

                select new SubmissionViewModel
                {
                    StudentName = u.FullName,
                    RegNum = st.RegNum,
                    AssignmentTitle = a.Title,
                    SubmissionDate = s.SubmissionDate,
                    Grade = s.Grade
                }

            ).Take(5).ToList();

            ViewBag.HighPerformers = topStudents;

            ViewBag.SubmissionRate =
                assignments.Count > 0
                    ? Math.Round(
                        (double)submissions.Count / assignments.Count,
                        1)
                    : 0;

            return View(modules);
        }

    }
}